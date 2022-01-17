using System;
using System.Collections.Generic;
using mobileGW.Service.Framework;
using System.Collections;
using System.Xml;
using System.Web.Script.Serialization;

using System.Text;



//namespace mobileGW.Integration
//{
public class CoreIntegration
{

    public CoreIntegration()
    {
        //
        //
    }

    public static TideRate.TideRateInquiryResType getTideRate(String cusID)
    {
        TideRate.AppHdrType appHdr = new TideRate.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TideRate.PairsType nsFrom = new TideRate.PairsType();
        nsFrom.Id = "EB";
        nsFrom.Name = "EB";

        TideRate.PairsType nsTo = new TideRate.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        TideRate.PairsType[] listOfNsTo = new TideRate.PairsType[1];
        listOfNsTo[0] = nsTo;

        TideRate.PairsType BizSvc = new TideRate.PairsType();
        BizSvc.Id = "TideRateService";
        BizSvc.Name = "TideRateService";

        DateTime TransDt = DateTime.Now;
        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        TideRate.TideRateInquiryReqType msgReq = new TideRate.TideRateInquiryReqType();
        TideRate.TideRateInquiryResType res = new TideRate.TideRateInquiryResType();

        msgReq.AppHdr = appHdr;
        msgReq.Amt = 0;
        msgReq.Cur = "704";
        msgReq.Tenure = "ALL";
        msgReq.TenureUnit = "ALL";
        try
        {
            TideRate.PortTypeClient ptc = new TideRate.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + cusID + "|getTideRate|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("custid:" + cusID + "|getTideRate exception|" + e.ToString());
            res = null;
        }
        return res;
    }
    
    public static String GetCoreTime(string custId)
    {
        CoreDate.CoreDateInquiryResType res = null;

        //Object result = new Object();
        CoreDate.AppHdrType appHdr = new CoreDate.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CoreDate.PairsType nsFrom = new CoreDate.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        CoreDate.PairsType nsTo = new CoreDate.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        CoreDate.PairsType[] listOfNsTo = new CoreDate.PairsType[1];
        listOfNsTo[0] = nsTo;

        CoreDate.PairsType BizSvc = new CoreDate.PairsType();
        BizSvc.Id = "CoreDate";
        BizSvc.Name = "CoreDate";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CoreDate.CoreDateInquiryReqType msgReq = new CoreDate.CoreDateInquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.BrCode = "BR0001";
        msgReq.Mode = "INTER";

        try
        {
            CoreDate.PortTypeClient ptc = new CoreDate.PortTypeClient();
            res = ptc.Inquiry(msgReq);

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("GetCoreTime|" + e.ToString());
            Funcs.WriteLog("Get core date failed");
        }

        if (res != null && !string.IsNullOrEmpty(res.ReturnDate))
        {
            Funcs.WriteLog("Get core date succesfull");
            return res.ReturnDate;
        }

        return string.Empty;
    }

    public static String getCASAAccountNameInfo(String acctNo, String custID)
    {
        //Object result = new Object();
        AcctInfo.AppHdrType appHdr = new AcctInfo.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctInfo.PairsType nsFrom = new AcctInfo.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        AcctInfo.PairsType nsTo = new AcctInfo.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        AcctInfo.PairsType[] listOfNsTo = new AcctInfo.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctInfo.PairsType BizSvc = new AcctInfo.PairsType();
        BizSvc.Id = "AcctInfo";
        BizSvc.Name = "AcctInfo";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        AcctInfo.AcctInfoInqResType result = new AcctInfo.AcctInfoInqResType();
        AcctInfo.AcctInfoInqReqType msgReq = new AcctInfo.AcctInfoInqReqType();
        msgReq.AppHdr = appHdr;

        msgReq.CustId = custID;
        msgReq.AcctInfo = new AcctInfo.BankAcctIdType();
        msgReq.AcctInfo.AcctId = acctNo;
        msgReq.AcctInfo.AcctCur = "%";

        try
        {
            AcctInfo.PortTypeClient ptc = new AcctInfo.PortTypeClient();
            result = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + custID + "|getCASAAccountNameInfo|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(result)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("custid:" + custID + "|getCASAAccountNameInfo|" + e.ToString());
        }

        if (result != null && result.AcctRec != null && result.AcctRec.Length > 0
                && !string.IsNullOrEmpty(result.AcctRec[0].CustName))
        {
            return result.AcctRec[0].CustName;
        }

        return string.Empty;
    }


    public static AcctPaySched.AcctPaySchedInqResType getPaymentScheduleDate(string cif, string acctNo, string enType, string fromDate, string toDate)
    {
        AcctPaySched.AcctPaySchedInqResType res = null;

        AcctPaySched.AppHdrType appHdr = new AcctPaySched.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctPaySched.PairsType nsFrom = new AcctPaySched.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        AcctPaySched.PairsType nsTo = new AcctPaySched.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        AcctPaySched.PairsType[] listOfNsTo = new AcctPaySched.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctPaySched.PairsType BizSvc = new AcctPaySched.PairsType();
        BizSvc.Id = "AcctPaySched";
        BizSvc.Name = "AcctPaySched";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctPaySched.AcctPaySchedInqReqType msgReq = new AcctPaySched.AcctPaySchedInqReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CustId = cif;
        msgReq.EnqType = enType;
        msgReq.FromDt = fromDate;
        msgReq.ToDt = toDate;
        msgReq.AcctId = acctNo;

        try
        {
            //portypeClient
            AcctPaySched.PortTypeClient ptc = new AcctPaySched.PortTypeClient();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + cif + "|getPaymentScheduleDate|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("custid:" + cif + "|getPaymentScheduleDate|" + e.ToString());
        }

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="custId"></param>
    /// <param name="ChkValue"></param>
    /// <param name="ValueType">- SI: CHECK_SI_BELONG_CIF; CASA: CHECK_CASA_BELONG_CIF; TIDE: CHECK_TIDE_AC_BELONG_CIF; DEPOSIT: CHECK_DEPOSIT_BELONG_CIF</param>
    /// <returns></returns>
    public static bool CheckAccountBelongCif(String custId, String ChkValue, String ValueType)
    {
        BelongChk.BelongChkVerifyResType res = null;

        BelongChk.AppHdrType appHdr = new BelongChk.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        BelongChk.PairsType nsFrom = new BelongChk.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        BelongChk.PairsType nsTo = new BelongChk.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        BelongChk.PairsType[] listOfNsTo = new BelongChk.PairsType[1];
        listOfNsTo[0] = nsTo;

        BelongChk.PairsType BizSvc = new BelongChk.PairsType();
        BizSvc.Id = "BelongChk";
        BizSvc.Name = "BelongChk";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        BelongChk.BelongChkVerifyReqType msgReq = new BelongChk.BelongChkVerifyReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CustId = custId;
        msgReq.ChkValue = ChkValue;
        msgReq.ValueType = ValueType;

        Funcs.WriteLog("CIF:" + custId + "|call CheckAccountBelongCif BEGIN: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            BelongChk.PortTypeClient ptc = new BelongChk.PortTypeClient();
            res = ptc.Verify(msgReq);

            Funcs.WriteLog("CIF:" + custId + "|call CheckAccountBelongCif DONE|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustId: " + custId + "|CheckAccountBelongCif|" + e.ToString());
        }

        if (res != null && res.ChkSts != null && res.ChkSts.Equals("1"))
        {
            return true;
        }

        return false;
    }

    public static AcctBalHist.AcctBalHistInquiryResType GetBalanceForChartCasa(string custId, string acctNo)
    {
        AcctBalHist.AcctBalHistInquiryResType res = null;

        //Object result = new Object();
        AcctBalHist.AppHdrType appHdr = new AcctBalHist.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctBalHist.PairsType nsFrom = new AcctBalHist.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        AcctBalHist.PairsType nsTo = new AcctBalHist.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        AcctBalHist.PairsType[] listOfNsTo = new AcctBalHist.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctBalHist.PairsType BizSvc = new AcctBalHist.PairsType();
        BizSvc.Id = "AcctBalHist";
        BizSvc.Name = "AcctBalHist";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctBalHist.AcctBalHistInquiryReqType msgReq = new AcctBalHist.AcctBalHistInquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CustId = custId;
        msgReq.AcctId = acctNo;

        try
        {
            //portypeClient
            AcctBalHist.PortTypeClient ptc = new AcctBalHist.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("CIF:" + custId + "|call ACCT BALANCE HIST DONE|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {

            //AccountIntegration.GetAccountInfo(custId);
            Funcs.WriteLog("CIF:" + custId + "|exception when call ACCT BALANCE HIST" + e.ToString());
        }
        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="custId"></param>
    /// <param name="acct_type"> -	CA: Deposit-	SA: Saving-	LN: LOAN</param>
    /// <param name="acctNo"></param>
    /// <returns></returns>
    public static AcctInfo.AcctInfoInqResType GetAccInfoByAccNo(string custId, string acct_type, string acctNo)
    {
           
        AcctInfo.AppHdrType appHdr = new AcctInfo.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctInfo.PairsType nsFrom = new AcctInfo.PairsType();
        nsFrom.Id = "EB";
        nsFrom.Name = "EB";

        AcctInfo.PairsType nsTo = new AcctInfo.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        AcctInfo.PairsType[] listOfNsTo = new AcctInfo.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctInfo.PairsType BizSvc = new AcctInfo.PairsType();
        BizSvc.Id = "AcctInfo";
        BizSvc.Name = "AcctInfo";

        //String tranDTString = "02/10/2017 00:00:00";  //"2017-02-10T00:00:00"    //"01/08/2008 14:50:50.42";

        DateTime TransDt = DateTime.Now;
        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctInfo.AcctInfoInqReqType msgReq = new AcctInfo.AcctInfoInqReqType();
        AcctInfo.AcctInfoInqResType res = new AcctInfo.AcctInfoInqResType();

        msgReq.AppHdr = appHdr;
        msgReq.AcctInfo = new AcctInfo.BankAcctIdType();
        //msgReq.AcctInfo.AcctType = "LN";
        msgReq.AcctInfo.AcctType = acct_type;
        msgReq.AcctInfo.AcctId = acctNo;
        msgReq.CustId = custId;
        msgReq.AcctInfo.AcctCur = "%";
            
        try
        {
            AcctInfo.PortTypeClient ptc = new AcctInfo.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("CIF:" + custId + "|call ACCT INFO done|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
              
            Funcs.WriteLog("CIF:" + custId + "|exception when call ACCT INFO|" + ex.ToString());
        }
        return res;
    }
            

    public static AcctHist.AcctHistInqResType GetAcctHist(string custId, string acctNo, string acctType, string enquiryType, string fromDate, string toDate)
    {
        AcctHist.AcctHistInqResType res = null;

        //Object result = new Object();
        AcctHist.AppHdrType appHdr = new AcctHist.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctHist.PairsType nsFrom = new AcctHist.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        AcctHist.PairsType nsTo = new AcctHist.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        AcctHist.PairsType[] listOfNsTo = new AcctHist.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctHist.PairsType BizSvc = new AcctHist.PairsType();
        BizSvc.Id = "AcctHist";
        BizSvc.Name = "AcctHist";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctHist.AcctHistInqReqType msgReq = new AcctHist.AcctHistInqReqType();
        msgReq.AppHdr = appHdr;
        msgReq.CustId = custId;
        msgReq.AcctId = acctNo;
        msgReq.AcctType = acctType;
        msgReq.InqType = enquiryType;
        msgReq.FromDt = fromDate;
        msgReq.ToDt = toDate;

        try
        {
            //portypeClient
            AcctHist.PortTypeClient ptc = new AcctHist.PortTypeClient();                
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("CIF:" + custId + "|call ACCT HIST done|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CIF:" + custId + "|exception when call ACCT HIST|" + ex.ToString());
        }
        return res;
    }
        

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cusId">Customer Infomation File array(so cif cua khach hang)</param>
    /// <param name="cusType">CIF: số CIF, CMT: số CMT,HC: số Hộ chiếu,SO_DT: số điện thoại</param>
    /// <param name="accType">Loai tai khoan: 001 CASA, 002 Tide booking, 003 tai khoan vay , ALL: tat ca</param>
    /// <returns></returns>
    public static  AccList.AcctListInqResType getAllAccountByConditions(
        String[] cusId,
        String cusType,
        String accType)
    {
        AccList.AcctListInqResType res = null;
        try
        {
            //Object result = new Object();
            AccList.AppHdrType appHdr = new AccList.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "2.0";

            AccList.PairsType nsFrom = new AccList.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            AccList.PairsType nsTo = new AccList.PairsType();
            nsTo.Id = "CORE";
            nsTo.Name = "CORE";

            AccList.PairsType[] listOfNsTo = new AccList.PairsType[1];
            listOfNsTo[0] = nsTo;

            AccList.PairsType BizSvc = new AccList.PairsType();
            BizSvc.Id = "AccList";
            BizSvc.Name = "AccList";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();

            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            //Body
            AccList.AcctListInqReqType msgReq = new AccList.AcctListInqReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustId = cusId;
            msgReq.CustType = cusType;
            msgReq.AcctType = accType;

            //portypeClient
            AccList.PortTypeClient ptc = new AccList.PortTypeClient();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + cusId + "|getAllAccountByConditions|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("getAllAccountByConditions EXCEPTION FROM ESB: " + ex.ToString());
        }
        return res;
    }

    public static string revFinPost(double TranId, string txDesc, string refNo = "_NULL_")
    {

        string result = Config.gResult_INTELLECT_Arr[1];
        string refno = "";

        if (refNo.Substring(0, 3).Equals(Config.refFormatSHBFC) && !refno.Equals("_NULL_"))
        {
            refno = Config.refFormatSHBFC + TranId.ToString().PadLeft(9, '0');
        }
        else
        {
            refno = Config.refFormat + TranId.ToString().PadLeft(9, '0');
        }

        FinancialPosting.AppHdrType appHdr = new FinancialPosting.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        FinancialPosting.PairsType nsFrom = new FinancialPosting.PairsType();
        nsFrom.Id = "EB";
        nsFrom.Name = "EB";

        FinancialPosting.PairsType nsTo = new FinancialPosting.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        FinancialPosting.PairsType[] listOfNsTo = new FinancialPosting.PairsType[1];
        listOfNsTo[0] = nsTo;

        FinancialPosting.PairsType BizSvc = new FinancialPosting.PairsType();
        BizSvc.Id = "FinancialPostingReversal";
        BizSvc.Name = "FinancialPostingReversal";
        DateTime TransDt = DateTime.Now;
        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        
        //Body
        FinancialPosting.FinancialPostingReversalReqType msgReq = new FinancialPosting.FinancialPostingReversalReqType();
        msgReq.AppHdr = appHdr;
        msgReq.ChnlId = Config.ChannelID;
        msgReq.ItfId = Config.InterfaceID;
        msgReq.ReversalRefNo = refno;
        msgReq.RefNo = "*";
        msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
        FinancialPosting.FinancialPostingReversalResType res = new FinancialPosting.FinancialPostingReversalResType();

        appHdr.Signature = Funcs.encryptMD5(msgReq.ChnlId + msgReq.TxnDt + msgReq.RefNo + msgReq.ReversalRefNo + Config.SharedKeyMD5).ToUpper();

        try
        {
            FinancialPosting.PortTypeClient ptc = new FinancialPosting.PortTypeClient();
            res = ptc.Reversal(msgReq);

            Funcs.WriteLog("|SHB MB FinancialPostingReversal |" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("|SHB MB FinancialPostingReversal |" + e.ToString());
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0"))
        {
            result = Config.gResult_INTELLECT_Arr[0];
            return result;
        }
        else
        {
            result = Config.gResult_INTELLECT_Arr[1];
        }

        return result;
    }

    public static string pstTransderTx(String CustID, double TranId, string SrcAcct, string DesAcct, double Amount,
            double feeAmount, String TxDesc, ref string core_txno_ref, ref string core_txdate_ref)
    {
        core_txno_ref = Config.refFormat + TranId.ToString().PadLeft(9, '0');

        InternalPosting.IntXferAddResType res = null;

        InternalPosting.AppHdrType appHdr = new InternalPosting.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        InternalPosting.PairsType nsFrom = new InternalPosting.PairsType();
        nsFrom.Id = "EB";
        nsFrom.Name = "EB";

        InternalPosting.PairsType nsTo = new InternalPosting.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        InternalPosting.PairsType[] listOfNsTo = new InternalPosting.PairsType[1];
        listOfNsTo[0] = nsTo;

        InternalPosting.PairsType BizSvc = new InternalPosting.PairsType();
        BizSvc.Id = "IntXfer";
        BizSvc.Name = "IntXfer";

        //String tranDTString = "02/10/2017 00:00:00";  //"2017-02-10T00:00:00"    //"01/08/2008 14:50:50.42";

        DateTime TransDt = DateTime.Now;
        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        InternalPosting.IntXferAddReqType msgReq = new InternalPosting.IntXferAddReqType();
        msgReq.AppHdr = appHdr;
        msgReq.ChnlId = Config.ChannelID;
        msgReq.ItfId = Config.InterfaceID;
        msgReq.RefNo = core_txno_ref;
        msgReq.DbAcct = new InternalPosting.BankAcctIdType();
        msgReq.CrAcct = new InternalPosting.BankAcctIdType();
        msgReq.DbAcct.AcctId = SrcAcct;
        msgReq.CrAcct.AcctId = DesAcct;
        msgReq.TxAmt = decimal.Parse(Amount.ToString());
        msgReq.InRemark = TxDesc;
        msgReq.ExRemark = TxDesc;
        msgReq.DealType = "SXM";
        appHdr.Signature = Funcs.encryptMD5(msgReq.DbAcct.AcctId + msgReq.CrAcct.AcctId + msgReq.TxAmt + msgReq.InRemark + msgReq.ExRemark + msgReq.DealType + Config.SharedKeyMD5).ToUpper();

        //portypeClient
        InternalPosting.PortTypeClient ptc = new InternalPosting.PortTypeClient();

        try
        {
            res = ptc.IntXferAdd(msgReq);

            Funcs.WriteLog("custid:" + CustID + "|SHB MB FUND TRANSFER IntXfer |" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CustID:" + CustID + "|res from ESB exception:" + ex.ToString());
        }


        //NEU GAP LOI CHUNG CHUNG
        if (res == null ||
            res.RespSts == null ||
            res.RespSts.Sts == null ||
            !res.RespSts.Sts.Equals("0"))
        {
            core_txdate_ref = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            return Config.gResult_INTELLECT_Arr[1];
        }

        //Hach toan thanh cong
        if (Config.gResult_INTELLECT_Arr[0].Split('|')[0].Equals(res.ResStatus))
        {
            core_txdate_ref = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            return Config.gResult_INTELLECT_Arr[0];
        }
        //hach toan khong thanh cong, lay duoc ra ma loi
        else
        {
            //neu co loi cu the thi tra ve ma loi cu the, thong bao cho khach hang
            for (int i = 2; i < Config.gResult_INTELLECT_Arr.Length; i++)
            {
                if (Config.gResult_INTELLECT_Arr[i].Split('|')[0].Equals(res.ResStatus))
                {
                    return Config.gResult_INTELLECT_Arr[i];
                }
            }

            //neu loi ngoai bang ma loi --> quy ve loi chung chung
            return Config.gResult_INTELLECT_Arr[1];
        }
    }

    public static String postFINPOSTToCore(
                string custid, //for log only
                string tran_type, //for log only
                double tran_id
                , string src_acct
                , string gl_suspend
                , string gl_fee
                , string gl_vat
                , double amount_suspend
                , double amount_fee
                , double amount_vat
                , string txdesc
                , string pos_cd
                , ref string core_txno_ref
                , ref string core_txdate_ref
                , string core_ref = null
        )
    {

        if (gl_suspend.Equals(Funcs.getConfigVal("ACCT_SUSPEND_SHBFC")))
        {
            core_txno_ref = Config.refFormatSHBFC + tran_id.ToString().PadLeft(9, '0');
        }
        else
        {
            core_txno_ref = Config.refFormat + tran_id.ToString().PadLeft(9, '0');
        }

        if (tran_type.Equals(Config.TRAN_TYPE_RECEIVE_GIFT))
        {
            core_txno_ref = core_ref;
        }

        core_txdate_ref = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

        try
        {

            //TAO XAU DU LIEU 3 CHAN OR 4 CHAN
            string postInfo;

            switch (tran_type)
            {
                case Config.TRAN_TYPE_ACCT_NICE:
                    postInfo = buildMessageToCoreOnlyVAT(pos_cd, src_acct, amount_suspend, amount_vat, gl_suspend, gl_vat);
                    break;
                default:
                    postInfo = buildMessageToCore(pos_cd, src_acct, amount_suspend, amount_fee, amount_vat, gl_suspend, gl_fee, gl_vat);
                    break;
            }

            string resut = "";
            //coreTxNoRef = Config.Core.refFormat + Convert.ToString(tran_id).PadLeft(9, '0');
            //string postInfo = buildMessageToCore(pos_cd, src_acct, amount_suspend, amount_fee, amount_vat, gl_suspend, gl_fee, gl_vat);

            FinancialPosting.FinancialPostingCreateResType res = null;
            //message = Config.Core.gResult_INTELLECT_Arr[1];
            //coreTxNoRef = Config.Core.refFormat + Convert.ToString(tran_id).PadLeft(9, '0');

            FinancialPosting.AppHdrType appHdr = new FinancialPosting.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            FinancialPosting.PairsType nsFrom = new FinancialPosting.PairsType();
            nsFrom.Id = "EB";
            nsFrom.Name = "EB";

            FinancialPosting.PairsType nsTo = new FinancialPosting.PairsType();
            nsTo.Id = "CORE";
            nsTo.Name = "CORE";

            FinancialPosting.PairsType[] listOfNsTo = new FinancialPosting.PairsType[1];
            listOfNsTo[0] = nsTo;

            FinancialPosting.PairsType BizSvc = new FinancialPosting.PairsType();
            BizSvc.Id = "FinancialPosting";
            BizSvc.Name = "FinancialPosting";
            DateTime TransDt = DateTime.Now;
            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            // appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);
            StringBuilder rawSign = new StringBuilder();


            //Body
            FinancialPosting.FinancialPostingCreateReqType msgReq = new FinancialPosting.FinancialPostingCreateReqType();
            msgReq.AppHdr = appHdr;
            msgReq.ChnlId = Config.ChannelID;
            msgReq.ItfId = Config.InterfaceID;
            msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");

            msgReq.RefNo = core_txno_ref;
            msgReq.SrcBranchCd = "110000";
            msgReq.PostingFlg = "F";
            msgReq.TxnBalFlg = "Y";
            msgReq.PostAllSegFlg = "N";

            try
            {

                string[] myArray = postInfo.Split('~');

                msgReq.NoOfSeq = myArray.Length + "";

                msgReq.SegInfo = new FinancialPosting.FinancialPostingCreateReqTypeSegInfo[4];
                //debit

                for (int i = 0; i < myArray.Length; i++)
                {
                    msgReq.SegInfo[i] = new FinancialPosting.FinancialPostingCreateReqTypeSegInfo();
                    msgReq.SegInfo[i].TxnCd = "FP";
                    msgReq.SegInfo[i].SegNo = Convert.ToString(i + 1);
                    msgReq.SegInfo[i].AcctBrCd = myArray[i].Split('|')[0];
                    msgReq.SegInfo[i].AcctId = myArray[i].Split('|')[1];
                    msgReq.SegInfo[i].DrCrFlg = myArray[i].Split('|')[2];
                    msgReq.SegInfo[i].TxnAmt = decimal.Parse(myArray[i].Split('|')[3]); //amount la kieu so
                    msgReq.SegInfo[i].TxnCur = myArray[i].Split('|')[4];
                    msgReq.SegInfo[i].IntRemark = txdesc;
                    msgReq.SegInfo[i].ExtRemark = txdesc;
                    rawSign.Append(msgReq.SegInfo[i].AcctId).Append(msgReq.SegInfo[i].TxnAmt).Append(msgReq.SegInfo[i].ExtRemark);
                }

                rawSign.Append(Config.SharedKeyMD5);
                appHdr.Signature = Funcs.encryptMD5(rawSign.ToString()).ToUpper(); //. MD5Encoding

                Funcs.WriteLog("custid:" + custid + "|SHB MB FINPOST FinancialPosting REQ |" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

                FinancialPosting.PortTypeClient ptc = new FinancialPosting.PortTypeClient();

                Funcs.WriteLog("custid:" + custid + "|SHB MB FINPOST FinancialPosting REQ|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

                res = ptc.Create(msgReq);

                //log ket qua tra ve
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, custid);
                Funcs.WriteLog("custid:" + custid + "|SHB MB FINPOST FinancialPosting |" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                ptc.Close();
            }
            catch (Exception e)
            {
                //log neu co exception
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, custid);
                Funcs.WriteLog("CustID " + custid + "|RESP FINPOST FROM ESB exception: " + e.ToString());
            }

            resut = Config.gResult_INTELLECT_Arr[1];

            if (res == null ||
            res.RespSts == null ||
            res.RespSts.Sts == null || !res.RespSts.Sts.Equals("0"))
            {

                resut = Config.gResult_INTELLECT_Arr[1];
                try
                {
                    for (int i = 0; i < Config.gResult_INTELLECT_Arr.Length; i++)
                        if (Config.gResult_INTELLECT_Arr[i].Split('|')[0].Equals(res.RespSts.ErrCd))
                        {
                            resut = Config.gResult_INTELLECT_Arr[i];
                            break;
                        }
                }
                catch (Exception e)
                {

                    // Helper.WriteLog(l4NC, e.Message + e.StackTrace, custid);
                    resut = Config.gResult_INTELLECT_Arr[1];
                }

            }

            else // goi vao core thanh cong
            {
                resut = Config.gResult_INTELLECT_Arr[0];
            }
            return resut;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            // revert transaction                
            return Config.gResult_INTELLECT_Arr[1];
        }
        finally
        {
            
        }
    }

    public static string getPosCdByCif(string custid, string accNo)
    {
        string result = string.Empty;
        GetPosCd.GetPosCdInquiryResType res = null;

        GetPosCd.AppHdrType appHdr = new GetPosCd.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        GetPosCd.PairsType nsFrom = new GetPosCd.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        GetPosCd.PairsType nsTo = new GetPosCd.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        GetPosCd.PairsType[] listOfNsTo = new GetPosCd.PairsType[1];
        listOfNsTo[0] = nsTo;

        GetPosCd.PairsType BizSvc = new GetPosCd.PairsType();
        BizSvc.Id = "GetPosCd";
        BizSvc.Name = "GetPosCd";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        GetPosCd.GetPosCdInquiryReqType msgReq = new GetPosCd.GetPosCdInquiryReqType();
        msgReq.AppHdr = appHdr;

        msgReq.AcctNo = accNo;
        msgReq.CustId = custid;

        try
        {
            //portypeClient
            GetPosCd.PortTypeClient ptc = new GetPosCd.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + custid + "|getPosCdByCif|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP getPosCdByCif FROM ESB exception: " + e.ToString());
        }


        if (res != null && !string.IsNullOrEmpty(res.PosCd))
        {
            return res.PosCd;
        }

        return result;
    }

    public static int getAllTideBooking(String cifNo)
    {
        int countDeposit = 0;
        try
        {
            string[] cusId = new String[1] { cifNo };
            String cusType = "CIF";

            AccList.AcctListInqResType res = getAllAccountByConditions(cusId, cusType, Config.prod_cd_TIDE);
            if (res == null)
            {
                countDeposit = -1;
            }

            if (res != null && res.AcctRec != null)
            {
                countDeposit = 0;
                foreach (var item in res.AcctRec)
                {
                    if (item.ProdCD.Equals(Config.PROD_CD_TIDE_ONLINE))
                    {
                        countDeposit += 1;
                    }
                } 
            }
        }
        catch (Exception ex)
        {
            countDeposit = -1;
            Funcs.WriteLog("CustID " + cifNo + "|RESP getPosCdByCif FROM ESB exception: " + ex.ToString());
        }
        return countDeposit;
    }

    public static string getEODStatus(Hashtable hashTbl)
    {
        string result = Config.ERR_MSG_FORMAT;
        string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string errCode = Config.ERR_CODE_TIMEOUT;
        string errMsg = Funcs.getConfigVal("MSG_EOD_STATUS");

        GetEODStatus.InquiryResType res = null;

        GetEODStatus.AppHdrType appHdr = new GetEODStatus.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        GetEODStatus.PairsType nsFrom = new GetEODStatus.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        GetEODStatus.PairsType nsTo = new GetEODStatus.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        GetEODStatus.PairsType[] listOfNsTo = new GetEODStatus.PairsType[1];
        listOfNsTo[0] = nsTo;

        GetEODStatus.PairsType BizSvc = new GetEODStatus.PairsType();
        BizSvc.Id = "GetEODStatus";
        BizSvc.Name = "GetEODStatus";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        GetEODStatus.InquiryReqType msgReq = new GetEODStatus.InquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.requestId = Funcs.GenESBMsgId();

        try
        {
            //portypeClient
            GetEODStatus.PortTypeClient ptc = new GetEODStatus.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + custid + "| RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

            if (res != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals("OUT_EOD"))
            {
                return Config.SUCCESS_MSG_GENERAL;
            }
        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP getEODStatus FROM ESB exception: " + e.ToString());
            return Config.ERR_MSG_GENERAL;
        }

        result = result.Replace("{0}", errCode);
        result = result.Replace("{1}", errMsg);

        return result;
    }

    public static string flexTideRate(Hashtable hashTbl)
    {
        string result = Config.GET_FLEX_TIDE_RATE;
        string errCode = Config.ERR_CODE_GENERAL;
        string errDesc = string.Empty;
        string record = string.Empty;

        string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string amout = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");
        string ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD");
        string prodCd = Funcs.getValFromHashtbl(hashTbl, "PRODCD");
        string tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE");
        string unitTenure = Funcs.getValFromHashtbl(hashTbl, "UNITTENURE");

        FlexTideRate.InquiryResType res = null;

        FlexTideRate.AppHdrType appHdr = new FlexTideRate.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        FlexTideRate.PairsType nsFrom = new FlexTideRate.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        FlexTideRate.PairsType nsTo = new FlexTideRate.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        FlexTideRate.PairsType[] listOfNsTo = new FlexTideRate.PairsType[1];
        listOfNsTo[0] = nsTo;

        FlexTideRate.PairsType BizSvc = new FlexTideRate.PairsType();
        BizSvc.Id = "FlexTideRate";
        BizSvc.Name = "FlexTideRate";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        FlexTideRate.InquiryReqType msgReq = new FlexTideRate.InquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.amount = amout;
        msgReq.ccycd = ccycd;
        msgReq.prodCd = prodCd;
        msgReq.tenure = tenure;
        msgReq.unitTenure = unitTenure;

        Funcs.WriteLog("custid:" + custid + "| REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            FlexTideRate.PortTypeClient ptc = new FlexTideRate.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + custid + "| RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

            if (res != null && res.RespSts.Sts.Equals("0"))
            {
                List<FlexTideRate.TideRateType> listRate = new List<FlexTideRate.TideRateType>(res.TideRate);
                if (listRate.Count > 0)
                {
                    errCode = Config.ERR_CODE_DONE;
                    errDesc = "SUCCESSFUL";

                    string strTemp = "";
                    foreach (FlexTideRate.TideRateType item in listRate)
                    {
                        strTemp = strTemp + "VND" + Config.COL_REC_DLMT;
                        //strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.TENURE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.TENURE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + item.tenure + Config.COL_REC_DLMT;
                        //strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.UNIT_TENURE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.UNIT_TENURE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + item.unitTenure + Config.COL_REC_DLMT;
                        //strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.RATE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.RATE].ToString()) + Config.COL_REC_DLMT
                        strTemp = strTemp + item.rate + Config.COL_REC_DLMT
                                + Config.ROW_REC_DLMT;
                        ;

                        //strTemp += (string.IsNullOrEmpty(item.effDt) ? "_NULL_" : item.effDt)
                        //        + Config.COL_REC_DLMT
                        //        + (string.IsNullOrEmpty(item.rate) ? "_NULL_" : item.rate)
                        //        + Config.COL_REC_DLMT
                        //        + (string.IsNullOrEmpty(item.tenure) ? "_NULL_" : item.tenure)
                        //        + Config.COL_REC_DLMT
                        //        + (string.IsNullOrEmpty(item.unitTenure) ? "_NULL_" : item.unitTenure)
                        //        + Config.COL_REC_DLMT
                        //        + Config.ROW_REC_DLMT;
                    }

                    record = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                }
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP flexTideRate FROM ESB exception: " + e.ToString());
        }

        result = result.Replace("{ERR_CODE}", errCode);
        result = result.Replace("{ERR_DESC}", errDesc);
        result = result.Replace("{RECORD}", record);

        return result;
    }
    //CMD#GET_FLEX_TIDE_DETAIL|CIF_NO#{CIF_NO}|TOKEN#{TOKEN}|ACCTNO#{ACCTNO}|CCYCD#{CCYCD}
    public static string getAcctTideDetail(Hashtable hashTbl)
    {
        string result = Config.GET_FLEX_TIDE_DETAIL;
        FlexTide.GetFlexTideDetailResType res = null;
        string record = "";
        string errCode = Config.ERR_CODE_GENERAL;
        string errDesc = "";
        string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
        string ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD");

        FlexTide.AppHdrType appHdr = new FlexTide.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        FlexTide.PairsType nsFrom = new FlexTide.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        FlexTide.PairsType nsTo = new FlexTide.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        FlexTide.PairsType[] listOfNsTo = new FlexTide.PairsType[1];
        listOfNsTo[0] = nsTo;

        FlexTide.PairsType BizSvc = new FlexTide.PairsType();
        BizSvc.Id = "GetFlexTideDetail";
        BizSvc.Name = "GetFlexTideDetail";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        FlexTide.GetFlexTideDetailReqType msgReq = new FlexTide.GetFlexTideDetailReqType();
        msgReq.AppHdr = appHdr;
        msgReq.ccycd = ccycd;
        msgReq.acctNo = acct_no;
        msgReq.custId = custid;

        Funcs.WriteLog("custid:" + custid + "|GetFlexTideDetail REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            FlexTide.PortTypeClient ptc = new FlexTide.PortTypeClient();
            res = ptc.GetLexTideDetail(msgReq);
            Funcs.WriteLog("custid:" + custid + "|GetFlexTideDetail RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

            if (res != null && res.RespSts.Sts.Equals("0"))
            {
                errCode = Config.ERR_CODE_DONE;
                errDesc = "SUCCESSFUL";

                result = result.Replace("{ERR_CODE}", errCode);
                result = result.Replace("{ERR_DESC}", errDesc);
                result = result.Replace("{SAVINGNO}", res.parentDepositNo);
                result = result.Replace("{CCYCD}", res.CcyCd);
                result = result.Replace("{PRINAMT}", res.PrinAmt);
                result = result.Replace("{MATAMT}", res.MatAmt);
                result = result.Replace("{INTAMT}", res.IntAmt);
                result = result.Replace("{TENURE}", res.Tenure);
                result = result.Replace("{UNITTENUREEN}", res.UnitTenureEn);
                result = result.Replace("{UNITTENUREVN}", res.UnitTenureVn);
                result = result.Replace("{INTRATE}", res.IntRate);
                result = result.Replace("{VALDT}", res.ValDt);
                result = result.Replace("{MATDT}", res.MatDt);
                result = result.Replace("{AUTORENNO}", res.AutoRenNo);
                result = result.Replace("{INSTRUCTION}", res.Instruction);
                result = result.Replace("{POSCD}", res.PosCd);
                result = result.Replace("{POSDES}", res.PosDes);
                result = result.Replace("{UNITTENURE}", res.UnitTenure);
                result = result.Replace("{ISPARENT}", res.isParent);
                result = result.Replace("{PARENTDEPOSITNO}", res.parentDepositNo);
                result = result.Replace("{PARENTACCOUNTNO}", res.parentAccountNo);
                result = result.Replace("{TOTALINTERESTAMOUNTORG}", res.totalInterestAmountOrg);
                result = result.Replace("{TOTALPRINCIPLEAMOUNTREMAIN}", res.totalPrincipleAmountRemain);
                result = result.Replace("{TOTALINTERESTAMOUNTREMAIN}", res.totalInterestAmountRemain);
                result = result.Replace("{TOTALMATAMOUNTREMAIN}", res.totalMatAmountRemain);
                result = result.Replace("{TOTALMATAMOUNTORG}", res.totalMatAmountOrg);
                result = result.Replace("{TOTALPRINCIPLEAMOUNTORG}", res.totalPrincipleAmountOrg);


                if (res.ListChildDeposits != null)
                {
                    List<FlexTide.ListChildDepositsType> listDeposits = new List<FlexTide.ListChildDepositsType>(res.ListChildDeposits);
                    if (listDeposits != null && listDeposits.Count > 0)
                    {
                        string strTemp = "";
                        foreach (FlexTide.ListChildDepositsType item in listDeposits)
                        {
                            strTemp = strTemp
                                + (string.IsNullOrEmpty(item.acctNo) ? "_NULL_" : item.acctNo)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.depositNo) ? "_NULL_" : item.depositNo)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.ccycd) ? "_NULL_" : item.ccycd)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.posCd) ? "_NULL_" : item.posCd)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.posDesc) ? "_NULL_" : item.posDesc)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.prodCd) ? "_NULL_" : item.prodCd)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.currPrinAmt) ? "_NULL_" : item.currPrinAmt)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.currMatAmt) ? "_NULL_" : item.currMatAmt)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.intAmt) ? "_NULL_" : item.intAmt)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.tenure) ? "_NULL_" : item.tenure)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.unitTenure) ? "_NULL_" : item.unitTenure)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.unitTenureVn) ? "_NULL_" : item.unitTenureVn)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.unitTenureEn) ? "_NULL_" : item.unitTenureEn)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.intRate) ? "_NULL_" : item.intRate)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.valDt) ? "_NULL_" : item.valDt)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.matDt) ? "_NULL_" : item.matDt)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.autoRenNo) ? "_NULL_" : item.autoRenNo)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.prodDesc) ? "_NULL_" : item.prodDesc)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.instruction) ? "_NULL_" : item.instruction)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.custName) ? "_NULL_" : item.custName)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.parentDepositNo) ? "_NULL_" : item.parentDepositNo)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.numOfTideOrg) ? "_NULL_" : item.numOfTideOrg)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.amountOfTideOrg) ? "_NULL_" : item.amountOfTideOrg)
                                + Config.COL_REC_DLMT
                                + (string.IsNullOrEmpty(item.numOfTideRemain) ? "_NULL_" : item.numOfTideRemain)
                                + Config.COL_REC_DLMT
                                + Config.ROW_REC_DLMT;
                            ;
                        }

                        record = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    }
                }

                result = result.Replace("{LISTCHILDDEPOSITS}", record);
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP GetFlexTideDetail FROM ESB exception: " + e.ToString());
            return Config.ERR_MSG_GENERAL;
        }

        return result;
    }

    public static AccList.AcctListInqResType getAcctTideOlInfoList(string custid, string acctType, string custType)
    {
        AccList.AcctListInqResType res = null;

        AccList.AppHdrType appHdr = new AccList.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "2.0";

        AccList.PairsType nsFrom = new AccList.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        AccList.PairsType nsTo = new AccList.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        AccList.PairsType[] listOfNsTo = new AccList.PairsType[1];
        listOfNsTo[0] = nsTo;

        AccList.PairsType BizSvc = new AccList.PairsType();
        BizSvc.Id = "AcctListInq";
        BizSvc.Name = "AcctListInq";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        string[] custIdArr = new string[1] { custid };

        //Body
        AccList.AcctListInqReqType msgReq = new AccList.AcctListInqReqType();
        msgReq.AppHdr = appHdr;
        msgReq.AcctType = acctType;
        msgReq.CustId = custIdArr;
        msgReq.CustType = custType;

        Funcs.WriteLog("custid:" + custid + "|AcctListInq REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            AccList.PortTypeClient ptc = new AccList.PortTypeClient();
            res = ptc.Inquiry(msgReq);
            Funcs.WriteLog("custid:" + custid + "|AcctListInq RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP AcctListInq FROM ESB exception: " + e.ToString());
            res = null;
        }

        return res;
    }

    #region Auto Saving
    public static TideAutoSaving.RegistResType REGIST_AUTO_SAVING(string custid, AutoSavingModel model)
    {
        TideAutoSaving.RegistResType res = null;

        TideAutoSaving.AppHdrType appHdr = new TideAutoSaving.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TideAutoSaving.PairsType nsFrom = new TideAutoSaving.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TideAutoSaving.PairsType nsTo = new TideAutoSaving.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        TideAutoSaving.PairsType[] listOfNsTo = new TideAutoSaving.PairsType[1];
        listOfNsTo[0] = nsTo;

        TideAutoSaving.PairsType BizSvc = new TideAutoSaving.PairsType();
        BizSvc.Id = "Regist";
        BizSvc.Name = "Regist";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        string[] custIdArr = new string[1] { custid };

        //Body
        TideAutoSaving.RegistReqType msgReq = new TideAutoSaving.RegistReqType();
        msgReq.AppHdr = appHdr;

        msgReq.authDt = model.AUTH_DT;
        msgReq.authId = model.AUTH_ID;
        msgReq.ccyCd = model.CCY_CD;
        msgReq.cifNo = model.CIF_NO;
        msgReq.depositType = model.DEPOSIT_TYPE.ToString();
        msgReq.freqBooking = model.FREQ_BOOKING;
        msgReq.lastBookDt = model.LAST_BOOK_DT;
        msgReq.legacyAc = model.LEGACY_AC;
        msgReq.matType = model.MAT_TYPE.ToString();
        msgReq.minBal = model.MIN_BAL.ToString();
        msgReq.mkrDt = model.MKR_DT;
        msgReq.mkrId = model.MKR_ID;
        msgReq.posCd = model.POS_CD;
        msgReq.prinAmt = model.PRIN_AMT.ToString();
        msgReq.refNo = model.REF_NO;
        msgReq.srcReg = model.SRC_REG;
        msgReq.startDt = model.START_DT;
        msgReq.tenure = model.TENURE;
        msgReq.tenureUnit = model.TENURE_UNIT;

        Funcs.WriteLog("custid:" + custid + "|Regist REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            TideAutoSaving.PortTypeClient ptc = new TideAutoSaving.PortTypeClient();
            res = ptc.Regist(msgReq);
            Funcs.WriteLog("custid:" + custid + "|Regist RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP Regist FROM ESB exception: " + e.ToString());
            res = null;
        }

        return res;
    }

    public static TideAutoSaving.UpdateResType UPDATE_AUTO_SAVING(string custid, AutoSavingModel model)
    {
        TideAutoSaving.UpdateResType res = null;

        TideAutoSaving.AppHdrType appHdr = new TideAutoSaving.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TideAutoSaving.PairsType nsFrom = new TideAutoSaving.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TideAutoSaving.PairsType nsTo = new TideAutoSaving.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        TideAutoSaving.PairsType[] listOfNsTo = new TideAutoSaving.PairsType[1];
        listOfNsTo[0] = nsTo;

        TideAutoSaving.PairsType BizSvc = new TideAutoSaving.PairsType();
        BizSvc.Id = "Update";
        BizSvc.Name = "Update";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        string[] custIdArr = new string[1] { custid };

        //Body
        TideAutoSaving.UpdateReqType msgReq = new TideAutoSaving.UpdateReqType();
        msgReq.AppHdr = appHdr;

        msgReq.freqBooking = model.FREQ_BOOKING;
        msgReq.legacyAc = model.LEGACY_AC;
        msgReq.matType = model.MAT_TYPE.ToString();
        msgReq.minBal = model.MIN_BAL.ToString();
        msgReq.prinAmt = model.PRIN_AMT.ToString();
        msgReq.startDt = model.START_DT;
        msgReq.tenure = model.TENURE;
        msgReq.tenureUnit = model.TENURE_UNIT;
        msgReq.srcReg = model.SRC_REG;

        Funcs.WriteLog("custid:" + custid + "|Update REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            TideAutoSaving.PortTypeClient ptc = new TideAutoSaving.PortTypeClient();
            res = ptc.Update(msgReq);
            Funcs.WriteLog("custid:" + custid + "|Update RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP Update FROM ESB exception: " + e.ToString());
            res = null;
        }

        return res;
    }

    public static TideAutoSaving.CancelResType CANCEL_AUTO_SAVING(string custid, string legacyAc)
    {
        TideAutoSaving.CancelResType res = null;

        TideAutoSaving.AppHdrType appHdr = new TideAutoSaving.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TideAutoSaving.PairsType nsFrom = new TideAutoSaving.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TideAutoSaving.PairsType nsTo = new TideAutoSaving.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        TideAutoSaving.PairsType[] listOfNsTo = new TideAutoSaving.PairsType[1];
        listOfNsTo[0] = nsTo;

        TideAutoSaving.PairsType BizSvc = new TideAutoSaving.PairsType();
        BizSvc.Id = "Cancel";
        BizSvc.Name = "Cancel";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        string[] custIdArr = new string[1] { custid };

        //Body
        TideAutoSaving.CancelReqType msgReq = new TideAutoSaving.CancelReqType();
        msgReq.AppHdr = appHdr;
        msgReq.legacyAc = legacyAc;
        msgReq.srcReg = Config.ChannelID;

        Funcs.WriteLog("custid:" + custid + "|Cancel REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            TideAutoSaving.PortTypeClient ptc = new TideAutoSaving.PortTypeClient();
            res = ptc.Cancel(msgReq);
            Funcs.WriteLog("custid:" + custid + "|Cancel RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP Cancel FROM ESB exception: " + e.ToString());
            res = null;
        }

        return res;
    }

    public static ListAutoSavingModel GET_LIST_AUTO_SAVING(string custid, string depositType)
    {
        ListAutoSavingModel rest = new ListAutoSavingModel();

        TideAutoSaving.GetListAutoSavingResType res = null;

        TideAutoSaving.AppHdrType appHdr = new TideAutoSaving.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TideAutoSaving.PairsType nsFrom = new TideAutoSaving.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TideAutoSaving.PairsType nsTo = new TideAutoSaving.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        TideAutoSaving.PairsType[] listOfNsTo = new TideAutoSaving.PairsType[1];
        listOfNsTo[0] = nsTo;

        TideAutoSaving.PairsType BizSvc = new TideAutoSaving.PairsType();
        BizSvc.Id = "GetListAutoSaving";
        BizSvc.Name = "GetListAutoSaving";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        string[] custIdArr = new string[1] { custid };

        //Body
        TideAutoSaving.GetListAutoSavingReqType msgReq = new TideAutoSaving.GetListAutoSavingReqType();
        msgReq.AppHdr = appHdr;

        msgReq.cifNo = custid;
        msgReq.depositType = depositType;
        msgReq.srcReg = Config.ChannelID;

        Funcs.WriteLog("custid:" + custid + "|GetListAutoSaving REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            TideAutoSaving.PortTypeClient ptc = new TideAutoSaving.PortTypeClient();
            res = ptc.GetListAutoSaving(msgReq);
            Funcs.WriteLog("custid:" + custid + "|GetListAutoSaving RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP GetListAutoSaving FROM ESB exception: " + e.ToString());

            rest.ErrCode = Config.ERR_CODE_GENERAL;
            rest.ErrDesc = "EXCEPTION EXCUTE SERVICE GETLISTAUTOSAVING";

            return rest;
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0"))
        {
            rest.ErrCode = res.errCode;
            rest.ErrDesc = res.errDesc;
            rest.ListAutoSaving = new List<AutoSavingItemModel>();

            

            foreach (var i in res.ListAutoSaving)
            {
                AutoSavingItemModel item = new AutoSavingItemModel();

                item.LEGACY_AC = i.legacyAc;
                item.MIN_BAL = Double.Parse(i.minBal);
                item.PRIN_AMT = Double.Parse(i.prinAmt);

                rest.ListAutoSaving.Add(item);
            }
        }
        else
        {
            rest.ErrCode = Config.ERR_CODE_GENERAL;
            rest.ErrDesc = "DATA NOT FOUND";
        }

        return rest;
    }

    public static ListHistAutoSavingModel GET_HIST_AUTO_SAVING(string custid, string legacyAc, string enqType, string fromDt, string toDt)
    {
        ListHistAutoSavingModel rest = new ListHistAutoSavingModel();

        TideAutoSaving.GetHistAutoSavingResType res = null;

        TideAutoSaving.AppHdrType appHdr = new TideAutoSaving.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TideAutoSaving.PairsType nsFrom = new TideAutoSaving.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TideAutoSaving.PairsType nsTo = new TideAutoSaving.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        TideAutoSaving.PairsType[] listOfNsTo = new TideAutoSaving.PairsType[1];
        listOfNsTo[0] = nsTo;

        TideAutoSaving.PairsType BizSvc = new TideAutoSaving.PairsType();
        BizSvc.Id = "GetHistAutoSaving";
        BizSvc.Name = "GetHistAutoSaving";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        string[] custIdArr = new string[1] { custid };

        //Body
        TideAutoSaving.GetHistAutoSavingReqType msgReq = new TideAutoSaving.GetHistAutoSavingReqType();
        msgReq.AppHdr = appHdr;
        msgReq.legacyAc = legacyAc;
        msgReq.srcReg = Config.ChannelID;
        msgReq.enqType = enqType;
        msgReq.fromDt = fromDt;
        msgReq.toDt = toDt;

        Funcs.WriteLog("custid:" + custid + "|GetHistAutoSaving REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            TideAutoSaving.PortTypeClient ptc = new TideAutoSaving.PortTypeClient();
            res = ptc.GetHistAutoSaving(msgReq);
            Funcs.WriteLog("custid:" + custid + "|GetHistAutoSaving RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP GetHistAutoSaving FROM ESB exception: " + e.ToString());
            rest.ErrCode = Config.ERR_CODE_GENERAL;
            rest.ErrDesc = "EXCEPTION EXCUTE SERVICE GETHISTAUTOSAVING";

            return rest;
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0"))
        {
            rest.ErrCode = res.errCode;
            rest.ErrDesc = res.errDesc;
            rest.ListHistAutoSaving = new List<HistAutoSavingItemModel>();

            

            foreach (var i in res.ListHistAutoSaving)
            {
                HistAutoSavingItemModel item = new HistAutoSavingItemModel();

                item.ACCOUNT_NUMBER = i.accountNumber;
                item.AMOUNT = Double.Parse(i.amount);
                item.OPEN_DATE = i.openDate;

                rest.ListHistAutoSaving.Add(item);
            }
        }
        else
        {
            rest.ErrCode = Config.ERR_CODE_GENERAL;
            rest.ErrDesc = "DATA NOT FOUND";
        }

        return rest;
    }

    public static TideAutoSaving.GetDetailAutoSavingResType GET_DETAIL_AUTO_SAVING(string custid, string legacyAc)
    {
        TideAutoSaving.GetDetailAutoSavingResType res = null;

        TideAutoSaving.AppHdrType appHdr = new TideAutoSaving.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TideAutoSaving.PairsType nsFrom = new TideAutoSaving.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TideAutoSaving.PairsType nsTo = new TideAutoSaving.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        TideAutoSaving.PairsType[] listOfNsTo = new TideAutoSaving.PairsType[1];
        listOfNsTo[0] = nsTo;

        TideAutoSaving.PairsType BizSvc = new TideAutoSaving.PairsType();
        BizSvc.Id = "GetHistAutoSaving";
        BizSvc.Name = "GetHistAutoSaving";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        string[] custIdArr = new string[1] { custid };

        //Body
        TideAutoSaving.GetDetailAutoSavingReqType msgReq = new TideAutoSaving.GetDetailAutoSavingReqType();
        msgReq.AppHdr = appHdr;
        msgReq.legacyAc = legacyAc;

        Funcs.WriteLog("custid:" + custid + "|GetDetailAutoSaving REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        try
        {
            //portypeClient
            TideAutoSaving.PortTypeClient ptc = new TideAutoSaving.PortTypeClient();
            res = ptc.GetDetailAutoSaving(msgReq);
            Funcs.WriteLog("custid:" + custid + "|GetDetailAutoSaving RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CustID " + custid + "|RESP GetDetailAutoSaving FROM ESB exception: " + e.ToString());
            return null;
        }

        return res;
    }

    #endregion

    /// <summary>
    /// Build message for Core
    /// </summary>
    /// <param name="posCd"></param>
    /// <param name="srcAcct"></param>
    /// <param name="amountSuspend"></param>
    /// <param name="amountFee"></param>
    /// <param name="amountVat"></param>
    /// <param name="glSuspend"></param>
    /// <param name="glFee"></param>
    /// <param name="glVat"></param>
    /// <returns></returns>
    private static string buildMessageToCoreOnlyVAT(string posCd, string srcAcct, double amountSuspend, double amountVat,
        string glSuspend, string glVat)
    {
        string postinfo = String.Empty;

        postinfo = posCd + "|"
               + srcAcct + "|D|"
               + (amountSuspend + amountVat).ToString() + "|VND~"
               + posCd + "|"
               + glSuspend + "|C|"
               + amountSuspend.ToString() + "|VND~"
               + posCd + "|"
               + glVat + "|C|"
               + amountVat.ToString() + "|VND";

        return postinfo;
    }

    /// <summary>
    /// Build message for Core
    /// </summary>
    /// <param name="posCd"></param>
    /// <param name="srcAcct"></param>
    /// <param name="amountSuspend"></param>
    /// <param name="amountFee"></param>
    /// <param name="amountVat"></param>
    /// <param name="glSuspend"></param>
    /// <param name="glFee"></param>
    /// <param name="glVat"></param>
    /// <returns></returns>
    private static string buildMessageToCore(string posCd, string srcAcct, double amountSuspend, double amountFee, double amountVat,
        string glSuspend, string glFee, string glVat)
    {
        string postinfo = String.Empty;
        if (amountFee > 0)
        {
            postinfo = posCd + "|"
               + srcAcct + "|D|"
               + (amountSuspend + amountFee + amountVat).ToString() + "|VND~"
               + posCd + "|"
               + glSuspend + "|C|"
               + amountSuspend.ToString() + "|VND~"
               + posCd + "|"
               + glFee + "|C|"
               + amountFee.ToString() + "|VND~"
               + posCd + "|"
               + glVat + "|C|"
               + amountVat.ToString() + "|VND";
        }
        else
        {
            postinfo = posCd + "|"
                     + srcAcct + "|D|"
                     + (amountSuspend).ToString() + "|VND~"
                     + posCd + "|"
                     + glSuspend + "|C|"
                     + amountSuspend.ToString() + "|VND";
        }
        return postinfo;
    }
}