using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Enquiry_Card
/// </summary>
public class Enquiry_Card
{
    public Enquiry_Card()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    public static CardLimit.CardLimitInqResType getCardLimits(string cusID, string cardNo)
    {
        CardLimit.AppHdrType appHdr = new CardLimit.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardLimit.PairsType nsFrom = new CardLimit.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        CardLimit.PairsType nsTo = new CardLimit.PairsType();
        nsTo.Id = "CARD";
        nsTo.Name = "CARD";

        CardLimit.PairsType[] listOfNsTo = new CardLimit.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardLimit.PairsType BizSvc = new CardLimit.PairsType();
        BizSvc.Id = "CardLimit";
        BizSvc.Name = "CardLimit";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CardLimit.CardLimitInqReqType msgReq = new CardLimit.CardLimitInqReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CardId = cardNo;

        CardLimit.CardLimitInqResType res = null;
        //portypeClient
        try
        {
            CardLimit.PortTypeClient ptc = new CardLimit.PortTypeClient();
            res = ptc.Inquiry(msgReq);
  
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GET_LIMIT_CARD EXCEPTION FROM ESB: " + ex.ToString());
        }


        return res;
    }

    public static string ChangeCardLimit(
        String card_no 
        , string ecom_type
        , string ecom_limit
        , string cash_type
        , string cash_limit
        , string daily_type
        , string daily_limit
        , string monthly_type
        , string monthly_limit
        , out bool success)
    {
        success = false;

        CardLimit.AppHdrType appHdr = new CardLimit.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardLimit.PairsType nsFrom = new CardLimit.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        CardLimit.PairsType nsTo = new CardLimit.PairsType();
        nsTo.Id = "CARD";
        nsTo.Name = "CARD";

        CardLimit.PairsType[] listOfNsTo = new CardLimit.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardLimit.PairsType BizSvc = new CardLimit.PairsType();
        BizSvc.Id = "ChangeCardLimit";
        BizSvc.Name = "ChangeCardLimit";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CardLimit.CardLimitModifyReqType msgReq = new CardLimit.CardLimitModifyReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CardId = card_no;
        msgReq.LimitType = null;

        List<CardLimit.CardLimitModifyReqTypeLimitType> limitType = new List<CardLimit.CardLimitModifyReqTypeLimitType>();

        CardLimit.CardLimitModifyReqTypeLimitType ecom = new CardLimit.CardLimitModifyReqTypeLimitType();
        ecom.Type = ecom_type.Replace(",", "");
        ecom.Value = ecom_limit.Replace(",", "");
        limitType.Add(ecom);

        CardLimit.CardLimitModifyReqTypeLimitType cash = new CardLimit.CardLimitModifyReqTypeLimitType();
        cash.Type = cash_type.Replace(",", "");
        cash.Value = cash_limit.Replace(",", "");
        limitType.Add(cash);

        CardLimit.CardLimitModifyReqTypeLimitType daily = new CardLimit.CardLimitModifyReqTypeLimitType();
        daily.Type = daily_type.Replace(",", "");
        daily.Value = daily_limit.Replace(",", "");
        limitType.Add(daily);

        CardLimit.CardLimitModifyReqTypeLimitType monthly = new CardLimit.CardLimitModifyReqTypeLimitType();
        monthly.Type = monthly_type.Replace(",", "");
        monthly.Value = monthly_limit.Replace(",", "");
        limitType.Add(monthly);

        msgReq.LimitType = limitType.ToArray();

        //portypeClient
        try
        {
            Funcs.WriteLog("BEGIN CALL SET LIMIT CARD");

            CardLimit.PortTypeClient ptc = new CardLimit.PortTypeClient();
            CardLimit.CardLimitModifyResType res = ptc.Modify(msgReq);
            
            if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("00"))
            {
                success = true;
                Funcs.WriteLog("CALL SET LIMIT CARD DONE RESULT: OK");
                return Config.gResult_INTELLECT_Arr[0];
                
            }
            else
            {
                Funcs.WriteLog("CALL SET LIMIT CARD DONE RESULT: FAILED");
                return Config.gResult_INTELLECT_Arr[1];//res.ERROR_CODE;
            }
        }
        catch (Exception ex)
        {
            //result = null;
            // Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            Funcs.WriteLog("CALL SET LIMIT CARD EXCEPTION EX:" + ex.ToString() );
        }
        return Config.gResult_INTELLECT_Arr[1];
    }


    /// <summary>
    /// Đóng mở thẻ
    /// </summary>
    /// <param name="custId"></param>
    /// <param name="cardNo"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool HandleCard(string custId, string cardNo, string action)
    {
        bool result = false;

        CardInfo.AppHdrType appHdr = new CardInfo.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardInfo.PairsType nsFrom = new CardInfo.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        CardInfo.PairsType nsTo = new CardInfo.PairsType();
        nsTo.Id = "CARD";
        nsTo.Name = "CARD";

        CardInfo.PairsType[] listOfNsTo = new CardInfo.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardInfo.PairsType BizSvc = new CardInfo.PairsType();
        BizSvc.Id = "CardInfo_Handle";
        BizSvc.Name = "CardInfo_Handle";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CardInfo.CardInfoHandleReqType msgReq = new CardInfo.CardInfoHandleReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CardId = cardNo;
        msgReq.HandleType = action;

        //portypeClient
        try
        {
            CardInfo.PortTypeClient ptc = new CardInfo.PortTypeClient();
            CardInfo.CardInfoHandleResType res = ptc.Handle(msgReq);

            if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
            {
                if (res.RespSts.ErrCd != null && res.RespSts.ErrCd.Equals("00"))
                {
                    return true;
                }
            }
            Funcs.WriteLog("CALL HANDLE CARD DONE");
        }
        catch (Exception ex)
        {

            // Helper.WriteLog(l4NC, e.Message + e.StackTrace, custId);
            Funcs.WriteLog("CALL HANDLE CARD EXCEPTION " +  ex.ToString());
        }


        return result;
    }


    public static bool CardPosting(string custId, string AcctId, string TxnCur, Decimal TxnAmt)
    {
        CardPosting.AppHdrType appHdr = new CardPosting.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardPosting.PairsType nsFrom = new CardPosting.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        CardPosting.PairsType nsTo = new CardPosting.PairsType();
        nsTo.Id = "CARD";
        nsTo.Name = "CARD";

        CardPosting.PairsType[] listOfNsTo = new CardPosting.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardPosting.PairsType BizSvc = new CardPosting.PairsType();
        BizSvc.Id = "CardPosting_Create";
        BizSvc.Name = "CardPosting_Create";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;


        //appHdr.Signature = MD5Encoding.Hash(AcctId + TxnAmt + Config.SharedKeyMD5);

        appHdr.Signature =   Funcs.encryptMD5 (AcctId + TxnAmt + Config.SharedKeyMD5);

        //Body
        CardPosting.CardPostingCreateReqType msgReq = new CardPosting.CardPostingCreateReqType();
        msgReq.AppHdr = appHdr;
        msgReq.AcctId = AcctId;
        msgReq.TxnCur = TxnCur;
        msgReq.TxnAmt = TxnAmt;

        //portypeClient
        try
        {
            Funcs.WriteLog("BEGIN CALL CARD POSTING");

            CardPosting.PortTypeClient ptc = new CardPosting.PortTypeClient();
            CardPosting.CardPostingCreateResType res = ptc.Create(msgReq);

            if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
            {
                Funcs.WriteLog("CALL CARD POSTING DONE RESULT: OK");
                return true;
            }
        }
        catch (Exception ex)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, custId);
            Funcs.WriteLog("CALL CARD POSTING EXCEPTION EX: " + ex.ToString());

        }

        return false;
    }

    public CardList1.CardListInqResType getCardList(string custId)
    {
        CardList1.CardListInqResType resCardList = null;
        try
        {
            #region message header
            CardList1.AppHdrType appHdr = new CardList1.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardList1.PairsType nsFrom = new CardList1.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardList1.PairsType nsTo = new CardList1.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardList1.PairsType[] listOfNsTo = new CardList1.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardList1.PairsType BizSvc = new CardList1.PairsType();
            BizSvc.Id = "CardList";
            BizSvc.Name = "CardList";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            //Body
            CardList1.CardListInqReqType msgReq = new CardList1.CardListInqReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustId = custId;
            #endregion
            //request to ESB
            CardList1.PortTypeClient ptc = new CardList1.PortTypeClient();
            resCardList = ptc.Inquiry(msgReq);
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GET_CARD_LIST EXCEPTION FROM ESB: " + ex.ToString());
        }
        return resCardList;
    }
}