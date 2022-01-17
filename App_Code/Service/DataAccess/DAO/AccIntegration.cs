using mobileGW.Service.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class AccIntegration
{
    public AccIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    public static string checkNationality(string cusID)
    {
        string rest = Config.RES_CHECK_NATIONALITY;
        CheckNationlity.AppHdrType appHdr = new CheckNationlity.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CheckNationlity.PairsType nsFrom = new CheckNationlity.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CheckNationlity.PairsType nsTo = new CheckNationlity.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CheckNationlity.PairsType[] listOfNsTo = new CheckNationlity.PairsType[1];
        listOfNsTo[0] = nsTo;

        CheckNationlity.PairsType BizSvc = new CheckNationlity.PairsType();
        BizSvc.Id = "CheckNationality";
        BizSvc.Name = "CheckNationality";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CheckNationlity.CheckNationalityInqReqType msgReq = new CheckNationlity.CheckNationalityInqReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CIF_NO = cusID;

        CheckNationlity.CheckNationalityInqResType res = null;
        //portypeClient
        try
        {
            CheckNationlity.PortTypeClient ptc = new CheckNationlity.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + cusID + "|CheckNationality|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

            if (res.RespSts.Sts.Equals("0"))
            {
                rest = rest.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                rest = rest.Replace("{ERR_DESC}", "SUCCESSFULL");
                rest = rest.Replace("{RET_CODE}", res.RET_CODE);

                return rest;
            }
            else
            {
                return Config.ERR_MSG_GENERAL;
            }

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("CheckNationality EXCEPTION FROM ESB: " + ex.ToString());
            return Config.ERR_MSG_GENERAL;
        }
    }

    public static string GetComboInfo(Hashtable hashTbl, string ip, string userAgent)
    {
        string result = Config.RESPONE_COMBO_INFO;
        string custId = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

        ComboInfo.AppHdrType appHdr = new ComboInfo.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        ComboInfo.PairsType nsFrom = new ComboInfo.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        ComboInfo.PairsType nsTo = new ComboInfo.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        ComboInfo.PairsType[] listOfNsTo = new ComboInfo.PairsType[1];
        listOfNsTo[0] = nsTo;

        ComboInfo.PairsType BizSvc = new ComboInfo.PairsType();
        BizSvc.Id = "ComboInfo";
        BizSvc.Name = "ComboInfo";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        ComboInfo.InquiryReqType msgReq = new ComboInfo.InquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.custId = custId;

        ComboInfo.InquiryResType res = new ComboInfo.InquiryResType();

        Funcs.WriteLog("custid:" + custId + "|ComboInfo REQ| " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));
        try
        {
            //portypeClient
            ComboInfo.PortTypeClient ptc = new ComboInfo.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + custId + "|ComboInfo RES| " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CIF_NO: " + custId + " |ComboInfo EXCEPTION FROM ESB: " + ex.ToString());
            return Config.ERR_CODE_GENERAL;
        }
        finally
        {

        }

        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals("00"))
        {
            result = result.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
            result = result.Replace("{ERR_DESC}", "SUCCESSFULL");
            result = result.Replace("{DESCRIPTION_VN}", res.descriptionVn);
            result = result.Replace("{DESCRIPTION_ENG}", res.descriptionEng);
            result = result.Replace("{CONTENT_VN}", res.contentVn);
            result = result.Replace("{CONTENT_ENG}", res.contentEng);
        }
        else
        {
            result = result.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
            result = result.Replace("{ERR_DESC}", "KHONG RA THONG TIN");
            result = result.Replace("{DESCRIPTION_VN}", "");
            result = result.Replace("{DESCRIPTION_ENG}", "");
            result = result.Replace("{CONTENT_VN}", "");
            result = result.Replace("{CONTENT_ENG}", "");
        }

        return result;
    }

    public static bool GET_CHARGE_FEE(String custid, string src_acct, string channel_id, string tran_type, double tran_amount, string benf_account, string bank_code, string ccyCd, ref double total_fee, ref double fee_not_vat, ref double vat_amt)
    {
        ChargeFee.AppHdrType appHdr = new ChargeFee.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        ChargeFee.PairsType nsFrom = new ChargeFee.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        ChargeFee.PairsType nsTo = new ChargeFee.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        ChargeFee.PairsType[] listOfNsTo = new ChargeFee.PairsType[1];
        listOfNsTo[0] = nsTo;

        ChargeFee.PairsType BizSvc = new ChargeFee.PairsType();
        BizSvc.Id = "ChargeFee-Inquiry";
        BizSvc.Name = "ChargeFee-Inquiry";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        ChargeFee.InquiryReqType msgReq = new ChargeFee.InquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cifNo = custid;
        msgReq.amount = tran_amount.ToString();
        msgReq.benAc = benf_account;
        msgReq.benBank = bank_code;
        msgReq.ccyCd = ccyCd;
        msgReq.channelId = channel_id;
        msgReq.sendAc = src_acct;
        msgReq.type = tran_type;

        ChargeFee.InquiryResType res = null;
        //portypeClient
        try
        {
            ChargeFee.PortTypeClient ptc = new ChargeFee.PortTypeClient();
            Funcs.WriteLog("custid:" + custid + "| request ChargeFee-Inquiry|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.Inquiry(msgReq);
            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO:" + custid + "|CheckNationality EXCEPTION FROM ESB: " + e.Message.ToString());
            return false;
        }
        finally
        {
            Funcs.WriteLog("custid:" + custid + "| response ChargeFee-Inquiry |" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0"))
        {
            Funcs.WriteLog("CIF_NO:" + custid + " response ChargeFee-Inquiry |GET_FEE SUCCESS");

            fee_not_vat = Double.Parse(res.chargeAmt);
            vat_amt = Double.Parse(res.vatChargeAmt);
            total_fee = Double.Parse(res.totalCharge);

            return true;
        }
        else
        {
            Funcs.WriteLog("CIF_NO:" + custid + " response ChargeFee-Inquiry |GET_FEE FAIL");
            return false;
        }
    }

    public static CoreHandleCIF.InquiryResType getCifInfo(string custID)
    {
        CoreHandleCIF.AppHdrType appHdr = new CoreHandleCIF.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CoreHandleCIF.PairsType nsFrom = new CoreHandleCIF.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CoreHandleCIF.PairsType nsTo = new CoreHandleCIF.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CoreHandleCIF.PairsType[] listOfNsTo = new CoreHandleCIF.PairsType[1];
        listOfNsTo[0] = nsTo;

        CoreHandleCIF.PairsType BizSvc = new CoreHandleCIF.PairsType();
        BizSvc.Id = "CoreHandleCIF-Inquiry";
        BizSvc.Name = "CoreHandleCIF-Inquiry";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CoreHandleCIF.InquiryReqType msgReq = new CoreHandleCIF.InquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.reqId = DateTime.Now.ToString("yyyyMMddHHmmss");
        msgReq.reqTime = DateTime.Now.ToString("yyyyMMdd");
        msgReq.partnerCode = "BRANCH";
        msgReq.customerType = "I";
        msgReq.infoType = "CIF_INFO_BY_CIF";
        msgReq.docType = "";
        msgReq.docID = "";
        msgReq.cifNo = custID;
        msgReq.accountNo = "";
        msgReq.transSign = Funcs.encryptMD5(
            msgReq.reqId
            + msgReq.reqTime
            + msgReq.partnerCode
            + msgReq.customerType
            + msgReq.infoType
            + msgReq.docType
            + msgReq.docID
            + msgReq.cifNo
            + msgReq.accountNo
            + "IOWUETUIEUTIWJZHDFJHASJRGHASJGFA"
            );

        Funcs.WriteLog("CIF_NO:" + custID + " request CoreHandleCIF-Inquiry" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        CoreHandleCIF.InquiryResType res = null;
        //portypeClient
        try
        {
            CoreHandleCIF.PortTypeClient ptc = new CoreHandleCIF.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("CIF_NO:" + custID + " response CoreHandleCIF-Inquiry" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO:" + custID + " request CoreHandleCIF-Inquiry - EXCEPTION: " + e.Message.ToString());
        }
        finally
        {
            Funcs.WriteLog("CIF_NO:" + custID + " request CoreHandleCIF-Inquiry - DONE: ");
        }

        return res;
    }
}