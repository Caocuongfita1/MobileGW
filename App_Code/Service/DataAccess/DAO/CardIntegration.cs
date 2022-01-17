using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class CardIntegration
{
    public CardIntegration()
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
            Funcs.WriteLog("custid:" + cusID + "|getCardLimits|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GET_LIMIT_CARD EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    public static string setCardLimit(
        string custId
        , String card_no
        , string ecom_type
        , string ecom_limit
        , string cash_type
        , string cash_limit
        , string daily_type
        , string daily_limit
        , string monthly_type
        , string monthly_limit
        , out string ws_card_result)
    {
        ws_card_result = string.Empty;
        CardLimit.CardLimitModifyResType res = null;

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

        try
        {
            Funcs.WriteLog("BEGIN CALL SET LIMIT CARD");

            CardLimit.PortTypeClient ptc = new CardLimit.PortTypeClient();
            res = ptc.Modify(msgReq);

            Funcs.WriteLog("custid:" + custId + "|setCardLimit|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            ws_card_result = "E99";
            Funcs.WriteLog("CALL SET LIMIT CARD EXCEPTION EX:" + ex.ToString());
        }

        //if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("00"))
        //anhnd2 fix loi phan set limit 25/09/2017
        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            ws_card_result = res.ERROR_CODE;
            Funcs.WriteLog("CALL SET LIMIT CARD DONE RESULT: OK");
            return Config.gResult_INTELLECT_Arr[0];
        }
        else
        {
            ws_card_result = "E99";
            Funcs.WriteLog("CALL SET LIMIT CARD DONE RESULT: FAILED");
            return Config.gResult_INTELLECT_Arr[1];
        }
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
        CardInfo.CardInfoHandleResType res = null;

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

        Funcs.WriteLog("custid:" + custId + "|HandleCard|MsgId=" + appHdr.MsgId);

        //portypeClient
        try
        {
            CardInfo.PortTypeClient ptc = new CardInfo.PortTypeClient();
            res = ptc.Handle(msgReq);

            Funcs.WriteLog("custid:" + custId + "|HandleCard|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CALL HANDLE CARD EXCEPTION " + ex.ToString());
        }
        if ("ACTIVE".Equals(action))
        {
            /*try
            {
                if (String.IsNullOrWhiteSpace(res.RespSts.ErrMsg))
                {
                    Funcs.WriteLog("IF ACTIVE NULL");
                }
                else
                {
                    Funcs.WriteLog("IF ACTIVE " + res.RespSts.ErrMsg);
                }
            }
            catch(Exception ex) {

            }*/

            if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0") && res.RespSts.ErrMsg == null)
                return true;
            else
                return false;
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            if (res.RespSts.ErrCd != null && res.RespSts.ErrCd.Equals("00"))
            {
                Funcs.WriteLog("CALL HANDLE CARD DONE");
                return true;
            }
        }

        return result;
    }


    public static bool CardPosting(string custId, string AcctId, string TxnCur, Decimal TxnAmt, out string out_err_code, out string out_err_desc, out string out_utranno)
    {
        out_err_code = string.Empty;
        out_err_desc = string.Empty;
        out_utranno = string.Empty;

        CardPosting.CardPostingCreateResType res = null;

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
        appHdr.Signature = Funcs.encryptMD5(AcctId + TxnAmt + Config.SharedKeyMD5).ToUpper();

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
            Funcs.WriteLog("custid:" + custId + "|CardPosting| INPUT:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            CardPosting.PortTypeClient ptc = new CardPosting.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("custid:" + custId + "|CardPosting| OUT:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
        }
        catch (Exception ex)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, custId);
            Funcs.WriteLog("CALL CARD POSTING EXCEPTION EX: " + ex.ToString());
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            Funcs.WriteLog("CALL CARD POSTING DONE RESULT: OK");

            return true;
        }

        return false;
    }


    public static CardList.CardListInqResType getCardList(string custId)
    {
        CardList.CardListInqResType resCardList = null;
        try
        {
            #region message header
            CardList.AppHdrType appHdr = new CardList.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "2.0";

            CardList.PairsType nsFrom = new CardList.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardList.PairsType nsTo = new CardList.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardList.PairsType[] listOfNsTo = new CardList.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardList.PairsType BizSvc = new CardList.PairsType();
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
            CardList.CardListInqReqType msgReq = new CardList.CardListInqReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustId = custId;
            #endregion
            //request to ESB
            CardList.PortTypeClient ptc = new CardList.PortTypeClient();

            resCardList = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getCardList|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(resCardList)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GET_CARD_LIST EXCEPTION FROM ESB: " + ex.ToString());
        }
        return resCardList;
    }


    public static CardHist.CardHistInqResType getCardHist(string custId, string cardNo, string enqType, string fromDate, string toDate)
    {
        CardHist.CardHistInqResType res = null;

        try
        {
            #region message header
            CardHist.AppHdrType appHdr = new CardHist.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardHist.PairsType nsFrom = new CardHist.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardHist.PairsType nsTo = new CardHist.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardHist.PairsType[] listOfNsTo = new CardHist.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardHist.PairsType BizSvc = new CardHist.PairsType();
            BizSvc.Id = "CardHist";
            BizSvc.Name = "CardHist";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            //Body
            CardHist.CardHistInqReqType msgReq = new CardHist.CardHistInqReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustId = custId;
            msgReq.CardId = cardNo;
            msgReq.InqType = enqType;
            msgReq.ToDt = toDate;
            msgReq.FromDt = fromDate;
            #endregion

            CardHist.PortTypeClient ptc = new CardHist.PortTypeClient();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getCardHist|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("getCardHist EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    public static bool CheckCardBelongCif(String cifID, String cardNo)
    {
        CardBelongCif.CardBelongCifVerifyResType res = null;

        CardBelongCif.AppHdrType appHdr = new CardBelongCif.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardBelongCif.PairsType nsFrom = new CardBelongCif.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        CardBelongCif.PairsType nsTo = new CardBelongCif.PairsType();
        nsTo.Id = "CARD";
        nsTo.Name = "CARD";

        CardBelongCif.PairsType[] listOfNsTo = new CardBelongCif.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardBelongCif.PairsType BizSvc = new CardBelongCif.PairsType();
        BizSvc.Id = "CardBelongCif";
        BizSvc.Name = "CardBelongCif";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CardBelongCif.CardBelongCifVerifyReqType msgReq = new CardBelongCif.CardBelongCifVerifyReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CardNo = cardNo;
        msgReq.CifNo = cifID;

        //portypeClient
        try
        {
            CardBelongCif.PortTypeClient ptc = new CardBelongCif.PortTypeClient();
            res = ptc.Verify(msgReq);

            Funcs.WriteLog("custid:" + cifID + "|CheckCardBelongCif|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("getCardHist EXCEPTION FROM ESB: " + e.ToString());
        }

        if (res != null && res.RetCd != null && res.RetCd.Trim().Equals("1"))
        {
            return true;
        }

        return false;
    }

    public static string getCardHolderName(string accNo, string custId)
    {
        CardName.CardNameInquiryResType res = null;
        try
        {
            #region header
            CardName.AppHdrType appHdr = new CardName.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardName.PairsType nsFrom = new CardName.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardName.PairsType nsTo = new CardName.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardName.PairsType[] listOfNsTo = new CardName.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardName.PairsType BizSvc = new CardName.PairsType();
            BizSvc.Id = "CardName";
            BizSvc.Name = "CardName";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            CardName.CardNameInquiryReqType msgReq = new CardName.CardNameInquiryReqType();
            msgReq.AppHdr = appHdr;
            msgReq.AcctNo = accNo;

            CardName.PortTypeClient ptc = new CardName.PortTypeClient();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getCardHolderName|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("getCardHolderName EXCEPTION FROM ESB: " + ex.ToString());
        }

        if (res != null)
        {
            return string.IsNullOrEmpty(res.CardHolder) ? string.Empty : res.CardHolder;
        }

        return string.Empty;
    }


    //chuyenlt1 Purchase CardAPI 
    public static CardAPI.PurchaseResType purcharseCardAPI(string custId, string cardNumber, string auditNo, string amout, string merchantType, string termId, string serviceCd, string merchantId)
    {
        CardAPI.PurchaseResType res = new CardAPI.PurchaseResType();
        try
        {
            #region header
            CardAPI.AppHdrType appHdr = new CardAPI.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardAPI.PairsType nsFrom = new CardAPI.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardAPI.PairsType nsTo = new CardAPI.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardAPI.PairsType[] listOfNsTo = new CardAPI.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardAPI.PairsType BizSvc = new CardAPI.PairsType();
            BizSvc.Id = "PurcharseCardAPI";
            BizSvc.Name = "PurcharseCardAPI";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            appHdr.Signature = Hash(custId + cardNumber + auditNo + amout + merchantType + termId + serviceCd + Config.ShareKey);
            #endregion

            CardAPI.PurchaseReqType msgReq = new CardAPI.PurchaseReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustId = custId;
            msgReq.CardNumber = cardNumber;
            msgReq.AuditNo = auditNo;
            msgReq.MerchantType = merchantType;
            msgReq.Amount = amout;
            msgReq.TermId = termId;
            msgReq.ServiceCd = serviceCd;
            msgReq.MerchantId = merchantId;

            Funcs.WriteLog("|PurcharseCardAPI REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            CardAPI.PortTypeClient ptc = new CardAPI.PortTypeClient();
            res = ptc.Purchase(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("|PurcharseCardAPI RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("Exception when write log response: " + subEx.Message);
                }
            }
            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("PurcharseCardAPI EXCEPTION FROM ESB: " + e.ToString());
        }
        return res;
    }

    //chuyenlt1 Revert CardAPI 
    public static CardAPI.ReversalResType revertCardAPI(string custId, string cardNumber, string auditNo, string amout, string refNo, string settDate,
        string merchantType, string termId, string serviceCd, string merchantId)
    {
        CardAPI.ReversalResType res = new CardAPI.ReversalResType();
        try
        {
            #region header
            CardAPI.AppHdrType appHdr = new CardAPI.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardAPI.PairsType nsFrom = new CardAPI.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardAPI.PairsType nsTo = new CardAPI.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardAPI.PairsType[] listOfNsTo = new CardAPI.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardAPI.PairsType BizSvc = new CardAPI.PairsType();
            BizSvc.Id = "PurcharseCardAPI";
            BizSvc.Name = "PurcharseCardAPI";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            appHdr.Signature = Hash(custId + cardNumber + auditNo + amout + merchantType + termId + serviceCd + refNo + settDate + Config.ShareKey);
            #endregion

            CardAPI.ReversalReqType msgReq = new CardAPI.ReversalReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustId = custId;
            msgReq.CardNumber = cardNumber;
            msgReq.AuditNo = auditNo;
            msgReq.MerchantType = merchantType;
            msgReq.Amount = amout;
            msgReq.TermId = termId;
            msgReq.ServiceCd = serviceCd;
            msgReq.RefNo = refNo;
            msgReq.SettDate = settDate;
            msgReq.MerchantId = merchantId;

            Funcs.WriteLog("|ReversalCardAPI REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            CardAPI.PortTypeClient ptc = new CardAPI.PortTypeClient();
            res = ptc.Reversal(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("|ReversalCardAPI RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("Exception when write log response: " + subEx.Message);
                }
            }
            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("ReversalCardAPI EXCEPTION FROM ESB: " + e.ToString());
        }
        return res;
    }
    public static string Hash(String input)
    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);
        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            if (hash[i] < 16)
                sb.Append("0" + hash[i].ToString("x"));
            //ret += "0" + a.ToString ("x");
            else
                sb.Append(hash[i].ToString("x"));
        }
        string str = sb.ToString().ToUpper();
        return str;
    }

    public static string GetLast(string source, int tail_length)
    {
        if (tail_length >= source.Length)
            return source;
        return source.Substring(source.Length - tail_length);
    }
    public static CardVerify.CardVerifyInqResType verifyCardNum(string cardNum, string custId, string cardType, string cardMD5)
    {
        CardVerify.CardVerifyInqResType res = new CardVerify.CardVerifyInqResType();
        try
        {
            #region header
            CardVerify.AppHdrType appHdr = new CardVerify.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardVerify.PairsType nsFrom = new CardVerify.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardVerify.PairsType nsTo = new CardVerify.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardVerify.PairsType[] listOfNsTo = new CardVerify.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardVerify.PairsType BizSvc = new CardVerify.PairsType();
            BizSvc.Id = "CardVerifyInq";
            BizSvc.Name = "CardVerifyInq";
            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            CardVerify.CardVerifyInqReqType msgReq = new CardVerify.CardVerifyInqReqType();
            msgReq.AppHdr = appHdr;
            msgReq.cardNum = cardNum;
            msgReq.cardType = cardType;
            msgReq.custInfo = custId;
            msgReq.channelId = Config.ChannelID;
            msgReq.mobileNum = string.Empty;
            msgReq.cardMD5 = cardMD5;

            Funcs.WriteLog("|CardVerifyInq REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            CardVerify.PortTypeClient ptc = new CardVerify.PortTypeClient();
            res = ptc.CardVerifyInq(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("|CardVerifyInq RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("Exception when write log response: " + subEx.Message);
                }
            }
            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CardVerifyInq EXCEPTION FROM ESB: " + e.ToString());
            return null;
        }
        return res;
    }

    public static CardPIN.changePinRespType changePin(string custId, string channelId, string cardMD5, string newPin)
    {
        CardPIN.changePinRespType res = new CardPIN.changePinRespType();
        try
        {
            #region header
            CardPIN.AppHdrType appHdr = new CardPIN.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardPIN.PairsType nsFrom = new CardPIN.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardPIN.PairsType nsTo = new CardPIN.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardPIN.PairsType[] listOfNsTo = new CardPIN.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardPIN.PairsType BizSvc = new CardPIN.PairsType();
            BizSvc.Id = "changePin";
            BizSvc.Name = "changePin";
            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            CardPIN.changePinReqType msgReq = new CardPIN.changePinReqType();
            msgReq.AppHdr = appHdr;
            msgReq.cardNum = cardMD5;
            msgReq.channel = channelId;
            msgReq.cifNum = custId;
            msgReq.newPin = newPin;

            Funcs.WriteLog("|changePin REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            CardPIN.CardPINPortTypeClient ptc = new CardPIN.CardPINPortTypeClient();

            res = ptc.ChangePin(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("|changePin RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("Exception when write log response: " + subEx.Message);
                }
            }
            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("changePin EXCEPTION FROM ESB: " + e.ToString());
            return null;
        }

        return res;
    }

    public static CardAutoDebit.InquiryResType getAutoDebit(string custId, string cardNum)
    {
        CardAutoDebit.InquiryResType res = new CardAutoDebit.InquiryResType();

        try
        {
            #region header
            CardAutoDebit.AppHdrType appHdr = new CardAutoDebit.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardAutoDebit.PairsType nsFrom = new CardAutoDebit.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardAutoDebit.PairsType nsTo = new CardAutoDebit.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardAutoDebit.PairsType[] listOfNsTo = new CardAutoDebit.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardAutoDebit.PairsType BizSvc = new CardAutoDebit.PairsType();
            BizSvc.Id = "CardAutoDebitInquiry";
            BizSvc.Name = "CardAutoDebitInquiry";
            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            appHdr.Signature = "";
            #endregion

            CardAutoDebit.InquiryReqType msgReq = new CardAutoDebit.InquiryReqType();

            msgReq.AppHdr = appHdr;
            msgReq.cardNum = cardNum;

            Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Inquiry REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            CardAutoDebit.PortTypeClient ptc = new CardAutoDebit.PortTypeClient();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Inquiry RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Inquiry EXCEPTION FROM ESB: " + e.ToString());
        }

        return res;
    }

    public static bool HandleAutoDebit(string custId, string cardNum, string debitAcct, string debitAmount, string refNum, string autoDebitStatus, string txdesc)
    {
        CardAutoDebit.CreateResType res = new CardAutoDebit.CreateResType();

        try
        {
            #region header
            CardAutoDebit.AppHdrType appHdr = new CardAutoDebit.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            CardAutoDebit.PairsType nsFrom = new CardAutoDebit.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            CardAutoDebit.PairsType nsTo = new CardAutoDebit.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            CardAutoDebit.PairsType[] listOfNsTo = new CardAutoDebit.PairsType[1];
            listOfNsTo[0] = nsTo;

            CardAutoDebit.PairsType BizSvc = new CardAutoDebit.PairsType();
            BizSvc.Id = "CardAutoDebitInquiry";
            BizSvc.Name = "CardAutoDebitInquiry";
            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            appHdr.Signature = "";
            #endregion

            CardAutoDebit.CreateReqType msgReq = new CardAutoDebit.CreateReqType();

            msgReq.AppHdr = appHdr;
            msgReq.cardNum = cardNum;
            msgReq.debitAcct = debitAcct;
            msgReq.debitAmount = debitAmount;
            msgReq.autoDebitStatus = autoDebitStatus;
            msgReq.custId = custId;
            msgReq.refNo = refNum;
            msgReq.txDesc = txdesc;

            Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Create REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            CardAutoDebit.PortTypeClient ptc = new CardAutoDebit.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Create RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Create EXCEPTION FROM ESB: " + e.ToString());
            return false;
        }

        if (res == null)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Create NULL: ");
            return false;
        }

        Funcs.WriteLog("CIF_NO: " + custId + "|CardAutoDebit-Create RES: " + res.errCode);

        if (res != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals("00"))
        {
            return true;
        }

        return false;
    }

    public static UnblockCard.InquiryResType GET_UNBLOCK_CARD_DETAIL(string custId, string cardMD5)
    {
        UnblockCard.InquiryResType res = new UnblockCard.InquiryResType();
        try
        {
            #region header
            UnblockCard.AppHdrType appHdr = new UnblockCard.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            UnblockCard.PairsType nsFrom = new UnblockCard.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            UnblockCard.PairsType nsTo = new UnblockCard.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            UnblockCard.PairsType[] listOfNsTo = new UnblockCard.PairsType[1];
            listOfNsTo[0] = nsTo;

            UnblockCard.PairsType BizSvc = new UnblockCard.PairsType();
            BizSvc.Id = "CardInternational-Inquiry";
            BizSvc.Name = "CardInternational-Inquiry";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            #endregion

            UnblockCard.InquiryReqType msgReq = new UnblockCard.InquiryReqType();
            msgReq.AppHdr = appHdr;
            msgReq.custId = custId;
            msgReq.cardMD5 = cardMD5;

            Funcs.WriteLog("CIF_NO: " + custId + "|GET_UNBLOCK_CARD_DETAIL REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            UnblockCard.PortTypeClient ptc = new UnblockCard.PortTypeClient();

            res = ptc.Inquiry(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("CIF_NO: " + custId + "|GET_UNBLOCK_CARD_DETAIL RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.Message);
                }
            }
            ptc.Close();

        }
        catch (Exception e)
        {

            Funcs.WriteLog("CIF_NO: " + custId + "|GET_UNBLOCK_CARD_DETAIL EXCEPTION FROM ESB: " + e.ToString());
            return null;
        }

        return res;
    }

    public static UnblockCard.CreateResType UNBLOCK_CARD(string custId, string cardMD5, string unblockType, string action, string validDate, string expDate, string amount)
    {
        UnblockCard.CreateResType res = new UnblockCard.CreateResType();

        try
        {
            #region header
            UnblockCard.AppHdrType appHdr = new UnblockCard.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            UnblockCard.PairsType nsFrom = new UnblockCard.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            UnblockCard.PairsType nsTo = new UnblockCard.PairsType();
            nsTo.Id = "CARD";
            nsTo.Name = "CARD";

            UnblockCard.PairsType[] listOfNsTo = new UnblockCard.PairsType[1];
            listOfNsTo[0] = nsTo;

            UnblockCard.PairsType BizSvc = new UnblockCard.PairsType();
            BizSvc.Id = "CardInternational-Create";
            BizSvc.Name = "CardInternational-Create";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            UnblockCard.CreateReqType msgReq = new UnblockCard.CreateReqType();
            msgReq.AppHdr = appHdr;
            msgReq.custId = custId;
            msgReq.cardMD5 = cardMD5;
            msgReq.tranType = unblockType;
            msgReq.status = "";
            msgReq.expFrom = action.Equals("REGIST") ? validDate : "";
            msgReq.expTo = action.Equals("REGIST") ? expDate : "";
            msgReq.amount = amount;
            msgReq.action = action;

            Funcs.WriteLog("CIF_NO: " + custId + "|UNBLOCK_CARD REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            UnblockCard.PortTypeClient ptc = new UnblockCard.PortTypeClient();

            res = ptc.Create(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("CIF_NO: " + custId + "|UNBLOCK_CARD RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.Message);
                    return null;
                }
            }
            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|UNBLOCK_CARD EXCEPTION FROM ESB: " + e.ToString());
            return null;
        }

        return res;
    }

}