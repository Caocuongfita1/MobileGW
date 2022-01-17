using System;
using System.Collections.Generic;
using mobileGW.Service.Framework;
using System.Collections;
using System.Xml;
using System.Web.Script.Serialization;

using System.Text;


/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class IBT_integration
{
    public IBT_integration()
    {
        //
        // TODO: Add constructor logic here
        //
    }



    //public String postTF247toNAPAS(
    //      double tranid
    //      , string custid //for log only
    //      , string tran_type // temp for log only
    //       , string src_acct
    //      , string des_acct
    //      , double amount
    //      , string txdesc
    //      , string des_name
    //      , string bank_code
    //      , ref string info_ref
    //  )

    public static String  postTF247toNAPAS(
        string tranid
         , string custid //for log only
         , string tran_type // temp for log only
          , string src_acct
         , string des_acct
         , double amount
         , string txdesc
         , string des_name
         , string bank_code
         , ref string info_ref
         )
    {

        IBXfer.AppHdrType appHdr = new IBXfer.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "2.0";//2.0

        IBXfer.PairsType nsFrom = new IBXfer.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        IBXfer.PairsType nsTo = new IBXfer.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        IBXfer.PairsType[] listOfNsTo = new IBXfer.PairsType[1];
        listOfNsTo[0] = nsTo;

        IBXfer.PairsType BizSvc = new IBXfer.PairsType();
        BizSvc.Id = "postToNapas";
        BizSvc.Name = "postToNapas";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        IBXfer.IBXferCreateReqType msgReq = new IBXfer.IBXferCreateReqType();
        msgReq.AppHdr = appHdr;

        IBXfer.IBXferCreateResType res = null;
        string finalRet = Config.ERR_CODE_GENERAL;
        info_ref = Config.gResult_SML_Arr[1];

        //amount gui sang IBT them 02 so 00 o cuoi truoc khi padleft
        string tmpAmt = amount.ToString() + "00";
        tmpAmt = tmpAmt.ToString().PadLeft(12, '0');

        Funcs.WriteLog("custid:" + custid + "|SHB MB RES CALL NAPAS IBT POSTING|REQUEST ID: " + appHdr.MsgId);

        //if (Config.TransType.ACQ_247CARD.Equals(tran_type))
        if (Config.TRAN_TYPE_ACQ_247CARD.Equals(tran_type))
        {
            msgReq.DbAcctId = src_acct;
            msgReq.ServiceCd = "912000"; // IBFT 912000
            msgReq.CrAcctId = des_acct;
            msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
            msgReq.TxnAmt = tmpAmt;
            msgReq.AuditNo = /*"100027";*/Funcs.GetLast(tranid, 6);
            msgReq.MerchantType = "7399";
            msgReq.TermId = "11111111";
            msgReq.CardAcceptor = "SHB IBT                  HANOI       VNM".PadRight(40, ' ');
            //msgReq.BenId = bank_code;
            msgReq.Remark = txdesc;
        }
        else
        {
            msgReq.DbAcctId = src_acct;
            msgReq.ServiceCd = "912020";
            msgReq.CrAcctId = des_acct;
            msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
            msgReq.TxnAmt = tmpAmt;
            msgReq.AuditNo = /*"100027";*/Funcs.GetLast(tranid, 6);
            msgReq.MerchantType = "7399";
            msgReq.TermId = "11111111";
            msgReq.CardAcceptor = "SHB IBT                  HANOI       VNM".PadRight(40, ' ');
            msgReq.BenId = bank_code;
            msgReq.Remark = txdesc;
        }

        try
        {
            appHdr.Signature = Funcs.encryptMD5(msgReq.DbAcctId + msgReq.CrAcctId + msgReq.AuditNo + msgReq.TxnAmt + Config.SharedKeyMD5).ToUpper();

            IBXfer.PortTypeClient ptc = new IBXfer.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING" + e.ToString());
        }

        if (res == null || res.RespSts == null || res.RespSts.Sts == null)
        {
            info_ref = Config.gResult_SML_Arr[1];
            finalRet = Config.ERR_CODE_GENERAL;
        }
        else if (res.RespSts.Sts.Equals("0"))
        {
            finalRet = Config.ERR_CODE_DONE;
            info_ref = Config.gResult_SML_Arr[0];
        }
        else if (res.RespSts.Sts.Equals("1"))
        {
            for (int i = 3; i < Config.gResult_SML_Arr.Length; i++)
            {
                Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING|" + Config.gResult_SML_Arr[i].Split('|')[0] + "|" +res.RespSts.ErrInfo[0].ErrCd);
                if (Config.gResult_SML_Arr[i].Split('|')[0].Equals(res.RespSts.ErrInfo[0].ErrCd))
                {
                    info_ref = Config.gResult_SML_Arr[i];
                    finalRet = Config.ERR_CODE_REVERT;
                    Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING|" + finalRet);
                    return finalRet;
                }
            }

            for (int j = 0; j < res.RespSts.ErrInfo.Length; j++)
            {
                if (res.RespSts.ErrInfo[j].ErrCd.Equals(Config.gResult_SML_Arr[2].Split('|')[0])) //timeout
                {
                    info_ref = Config.gResult_SML_Arr[2];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                }
            }
        }
        else
        {
            info_ref = Config.gResult_SML_Arr[1];
            finalRet = Config.ERR_CODE_GENERAL;
        }

        return finalRet;
    }

    public static String postTF247toNAPASV1(
        string tranid
         , string custid //for log only
         , string tran_type // temp for log only
          , string src_acct
         , string des_acct
         , double amount
         , string txdesc
         , string des_name
         , string bank_code
         , ref string info_ref
         )
    {

        IBXferV1.AppHdrType appHdr = new IBXferV1.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.1";//2.0

        IBXferV1.PairsType nsFrom = new IBXferV1.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        IBXferV1.PairsType nsTo = new IBXferV1.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        IBXferV1.PairsType[] listOfNsTo = new IBXferV1.PairsType[1];
        listOfNsTo[0] = nsTo;

        IBXferV1.PairsType BizSvc = new IBXferV1.PairsType();
        BizSvc.Id = "postToNapas";
        BizSvc.Name = "postToNapas";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        IBXferV1.IBXferCreateReqType msgReq = new IBXferV1.IBXferCreateReqType();
        msgReq.AppHdr = appHdr;

        IBXferV1.IBXferCreateResType res = null;
        string finalRet = Config.ERR_CODE_GENERAL;
        info_ref = Config.gResult_SML_Arr[1];

        //amount gui sang IBT them 02 so 00 o cuoi truoc khi padleft
        string tmpAmt = amount.ToString() + "00";
        tmpAmt = tmpAmt.ToString().PadLeft(12, '0');

        Funcs.WriteLog("custid:" + custid + "|SHB MB RES CALL NAPAS IBT POSTING|REQUEST ID: " + appHdr.MsgId);

        //if (Config.TransType.ACQ_247CARD.Equals(tran_type))
        if (Config.TRAN_TYPE_ACQ_247CARD.Equals(tran_type))
        {
            msgReq.DbAcctId = src_acct;
            msgReq.ServiceCd = "910000"; // IBFT 912000
            msgReq.CrAcctId = des_acct;
            msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
            msgReq.TxnAmt = tmpAmt;
            msgReq.AuditNo = /*"100027";*/Funcs.GetLast(tranid, 6);
            msgReq.MerchantType = "7399";
            msgReq.TermId = "11111111";
            msgReq.CardAcceptor = "SHB IBT                  HANOI       VNM".PadRight(40, ' ');
            //msgReq.BenId = bank_code;
            msgReq.Remark = txdesc;
        }
        else
        {
            msgReq.DbAcctId = src_acct;
            msgReq.ServiceCd = "912020";
            msgReq.CrAcctId = des_acct;
            msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
            msgReq.TxnAmt = tmpAmt;
            msgReq.AuditNo = /*"100027";*/Funcs.GetLast(tranid, 6);
            msgReq.MerchantType = "7399";
            msgReq.TermId = "11111111";
            msgReq.CardAcceptor = "SHB IBT                  HANOI       VNM".PadRight(40, ' ');
            msgReq.BenId = bank_code;
            msgReq.Remark = txdesc;
        }

        try
        {
            appHdr.Signature = Funcs.encryptMD5(msgReq.DbAcctId + msgReq.CrAcctId + msgReq.AuditNo + msgReq.TxnAmt + Config.SharedKeyMD5).ToUpper();

            IBXferV1.PortTypeClient ptc = new IBXferV1.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING" + e.ToString());
        }

        if (res == null || res.RespSts == null || res.RespSts.Sts == null)
        {
            info_ref = Config.gResult_SML_Arr[1];
            finalRet = Config.ERR_CODE_GENERAL;
        }
        else if (res.RespSts.Sts.Equals("0"))
        {
            finalRet = Config.ERR_CODE_DONE;
            info_ref = Config.gResult_SML_Arr[0];
        }
        else if (res.RespSts.Sts.Equals("1"))
        {
            for (int i = 3; i < Config.gResult_SML_Arr.Length; i++)
            {
                Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING|" + Config.gResult_SML_Arr[i].Split('|')[0] + "|" + res.RespSts.ErrInfo[0].ErrCd);
                if (Config.gResult_SML_Arr[i].Split('|')[0].Equals(res.RespSts.ErrInfo[0].ErrCd))
                {
                    info_ref = Config.gResult_SML_Arr[i];
                    finalRet = Config.ERR_CODE_REVERT;
                    Funcs.WriteLog("custid:" + custid + "|SHB MB RES AFTER CALL NAPAS IBT POSTING|" + finalRet);
                    return finalRet;
                }
            }

            for (int j = 0; j < res.RespSts.ErrInfo.Length; j++)
            {
                if (res.RespSts.ErrInfo[j].ErrCd.Equals(Config.gResult_SML_Arr[2].Split('|')[0])) //timeout
                {
                    info_ref = Config.gResult_SML_Arr[2];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                }
            }
        }
        else
        {
            info_ref = Config.gResult_SML_Arr[1];
            finalRet = Config.ERR_CODE_GENERAL;
        }

        return finalRet;
    }
}