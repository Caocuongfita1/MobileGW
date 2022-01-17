using System;
using System.Collections.Generic;
using mobileGW.Service.Framework;

//namespace mobileGW.Integration
//{
    public class Enquiry_Core
    {

        public Enquiry_Core()
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

        //String tranDTString = "02/10/2017 00:00:00";  //"2017-02-10T00:00:00"    //"01/08/2008 14:50:50.42";

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
        }
        catch (Exception e)
        {
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
                //portypeClient
                CoreDate.PortTypeClient ptc = new CoreDate.PortTypeClient();
                res = ptc.Inquiry(msgReq);
                if (res != null && !string.IsNullOrEmpty(res.ReturnDate))
                {
                    return res.ReturnDate;
                }
                Funcs.WriteLog("Get core date succesfull");
            }
            catch (Exception e)
            {

                //AccountIntegration.GetAccountInfo(custId);
                Funcs.WriteLog("Get core date failed");
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
            //msgReq.AcctInfo.AcctType = "CA";
            msgReq.AcctInfo.AcctCur = "%";
            //msgReq.AcctInfo.AcctSts = "";

            try
            {
                AcctInfo.PortTypeClient ptc = new AcctInfo.PortTypeClient();
                result = ptc.Inquiry(msgReq);
                //Helper.WriteLog(l4NC, result, custID);
                if (result != null && result.AcctRec != null && result.AcctRec.Length > 0)
                {

                    return result.AcctRec[0].CustName;
                }

            }
            catch (Exception e)
            {
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, custID);
            }


            return string.Empty;
        }


        public static AcctPaySched.AcctPaySchedInqResType getPaymentScheduleDate(string cif, string acctNo, string enType, string fromDate, string toDate)
        {
            
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
                AcctPaySched.AcctPaySchedInqResType res = ptc.Inquiry(msgReq);
                return res;
            }
            catch (Exception e)
            {

                //JavaScriptSerializer js = new JavaScriptSerializer();
                //string json = js.Serialize(cif);
                ////AccountIntegration.GetAccountInfo(custId);
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, json);
            }


            return null ;
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


            try
            {
                //portypeClient
                BelongChk.PortTypeClient ptc = new BelongChk.PortTypeClient();
                BelongChk.BelongChkVerifyResType res = ptc.Verify(msgReq);
                if (res != null && res.ChkSts != null && res.ChkSts.Equals("1"))
                {
                    return true;
                }

            }
            catch (Exception e)
            {

                //JavaScriptSerializer js = new JavaScriptSerializer();
                //string json = js.Serialize(custId);
                ////AccountIntegration.GetAccountInfo(custId);
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, json);
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
                Funcs.WriteLog("CIF:" + custId + "|call ACCT BALANCE HIST DONE");
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
            //AcctInfo.AcctSts
            
            try
            {

                AcctInfo.PortTypeClient ptc = new AcctInfo.PortTypeClient();
                res = ptc.Inquiry(msgReq);
                Funcs.WriteLog("CIF:" + custId + "|call ACCT INFO done");
            }
            catch (Exception ex)
            {
              
                Funcs.WriteLog("CIF:" + custId + "|exception when call ACCT INFO");
            }
            return res;
        }

        

        public static AcctHist.AcctHistInqResType GetAcctHist(string custId, string acctNo, string acctType, string enquiryType, string fromDate, string toDate)
        {
            //List<StatementModel> result = new List<StatementModel>();

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
                Funcs.WriteLog("CIF:" + custId + "|call ACCT HIST done");
            }
            catch (Exception ex)
            {
                //JavaScriptSerializer js = new JavaScriptSerializer();
                //string json = js.Serialize(custId);
                ////AccountIntegration.GetAccountInfo(custId);
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, json);
                Funcs.WriteLog("CIF:" + custId + "|exception when call ACCT HIST");
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
            //Object result = new Object();
            AccList.AppHdrType appHdr = new AccList.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

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
            AccList.AcctListInqResType res = ptc.Inquiry(msgReq);
            return res;

        }

    }
//}