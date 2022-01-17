using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using CardDPP;

/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class SHBFCIntegration
{
    public SHBFCIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public  SHBFC.GetLoanInforResType getLoanInfo(string custid,string contractNum)
    // public  string doPayment()
    {
        SHBFC.AppHdrType appHdr = new SHBFC.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        SHBFC.PairsType nsFrom = new SHBFC.PairsType();
        nsFrom.Id = "SOA";
        nsFrom.Name = "SOA";

        SHBFC.PairsType nsTo = new SHBFC.PairsType();
        nsTo.Id = "SHBFC";
        nsTo.Name = "SHBFC";

        SHBFC.PairsType[] listOfNsTo = new SHBFC.PairsType[1];
        listOfNsTo[0] = nsTo;

        SHBFC.PairsType BizSvc = new SHBFC.PairsType();
        BizSvc.Id = "SHBFC";
        BizSvc.Name = "SHBFC";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        SHBFC.GetLoanInforReqType msgReq = new SHBFC.GetLoanInforReqType();
        msgReq.AppHdr = appHdr;
        msgReq.contractNumber = contractNum;

        Funcs.WriteLog("custid:" + custid + "|GetTransactionList|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));
        SHBFC.GetLoanInforResType res = null;
        try
        {

            SHBFC.PortTypeClient ptc = new SHBFC.PortTypeClient();
            Funcs.WriteLog("Vào hàm : ");

            res = ptc.GetLoanInfor(msgReq);
            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("custid:" + custid + "|Res GetLoanInfor: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("custid:" + custid + "|Exception when write log response: " + subEx.Message);
                }
                // result = res.EvnHNBillDetailItem;
            }

            // Log.WriteLog("Kết thúc hàm PolicyInquiry: ");
            ptc.Close();

        }
        catch (Exception e)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            object a = e.Message;
            Funcs.WriteLog("custid:" + custid + "|Lỗi Hàm getDisbursement: " + a);
        }

        return res;
    }
    public  SHBFC.GetDisbursementResType getDisbursement(string custid,string contractNum, string NationalID)
    // public  string doPayment()
    {
        SHBFC.AppHdrType appHdr = new SHBFC.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        SHBFC.PairsType nsFrom = new SHBFC.PairsType();
        nsFrom.Id = "SOA";
        nsFrom.Name = "SOA";

        SHBFC.PairsType nsTo = new SHBFC.PairsType();
        nsTo.Id = "SHBFC";
        nsTo.Name = "SHBFC";

        SHBFC.PairsType[] listOfNsTo = new SHBFC.PairsType[1];
        listOfNsTo[0] = nsTo;

        SHBFC.PairsType BizSvc = new SHBFC.PairsType();
        BizSvc.Id = "SHBFC";
        BizSvc.Name = "SHBFC";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        SHBFC.GetDisbursementReqType msgReq = new SHBFC.GetDisbursementReqType();
        msgReq.AppHdr = appHdr;
        msgReq.contractNumber = contractNum;
        msgReq.nationalID = NationalID;
        Funcs.WriteLog("custid:" + custid + "|Hàm GetDisbursement: " + msgReq.AppHdr.MsgId + "Đầu vào hàm GetDisbursement: contractNumber:" + msgReq.contractNumber + ",nationalID:" + msgReq.nationalID);
        SHBFC.GetDisbursementResType res = null;
        try
        {

            SHBFC.PortTypeClient ptc = new SHBFC.PortTypeClient();
            Funcs.WriteLog("Vào hàm : ");

            res = ptc.GetDisbursement(msgReq);
            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("custid:" + custid + "|Res getDisbursement: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("custid:" + custid + "|Exception when write log response: " + subEx.Message);
                }
                // result = res.EvnHNBillDetailItem;
            }
            else
            {
                throw new Exception("custid:" + custid + "|Xảy ra lỗi trong kết nối với SHBFC");
            }

            // Log.WriteLog("Kết thúc hàm PolicyInquiry: ");
            ptc.Close();

        }
        catch (Exception e)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            object a = e.Message;
            Funcs.WriteLog("custid:" + custid + "|Lỗi Hàm getDisbursement: " + e.ToString());
        }
        return res;
    }


    public  SHBFC.DisbursementResType Disbursement(string custid,string contractNumber, string customerName, string nationalID, string disbursementAmount, string disbursementDate, string referenceNumber, string token, string referenceDate)
    // public  string doPayment()
    {
        SHBFC.AppHdrType appHdr = new SHBFC.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        SHBFC.PairsType nsFrom = new SHBFC.PairsType();
        nsFrom.Id = "SOA";
        nsFrom.Name = "SOA";

        SHBFC.PairsType nsTo = new SHBFC.PairsType();
        nsTo.Id = "SHBFC";
        nsTo.Name = "SHBFC";

        SHBFC.PairsType[] listOfNsTo = new SHBFC.PairsType[1];
        listOfNsTo[0] = nsTo;

        SHBFC.PairsType BizSvc = new SHBFC.PairsType();
        BizSvc.Id = "SHBFC";
        BizSvc.Name = "SHBFC";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        SHBFC.DisbursementReqType msgReq = new SHBFC.DisbursementReqType();
        msgReq.AppHdr = appHdr;
        msgReq.contractNumber = contractNumber;
        msgReq.nationalID = nationalID;
        msgReq.customerName = customerName;
        msgReq.disbursementAmount = disbursementAmount;
        msgReq.disbursementDate = disbursementDate;
        msgReq.referenceNumber = referenceNumber;
        msgReq.token = token;
        msgReq.referenceDate = referenceDate;
        Funcs.WriteLog("custid:" + custid + "|Hàm Disbursement: " + msgReq.AppHdr.MsgId + "Đầu vào hàm Disbursement: contractNumber:" + msgReq.contractNumber + ",nationalID:" + msgReq.nationalID + ",customerName:" + msgReq.customerName
            + "disbursementAmount:" + msgReq.disbursementAmount + ",disbursementDate:" + msgReq.disbursementDate + ",referenceNumber:" + msgReq.referenceNumber + ",referenceNumber:" + msgReq.token);
        SHBFC.DisbursementResType res = null;
        try
        {

            SHBFC.PortTypeClient ptc = new SHBFC.PortTypeClient();
            Funcs.WriteLog("Vào hàm : ");

            res = ptc.Disbursement(msgReq);
            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("custid:" + custid + "|Res Disbursement: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("custid:" + custid + "|Exception when write log response: " + subEx.Message);
                }
                // result = res.EvnHNBillDetailItem;
            }

            // Log.WriteLog("Kết thúc hàm PolicyInquiry: ");
            ptc.Close();

        }
        catch (Exception e)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            object a = e.Message;
            Funcs.WriteLog("custid:" + custid + "|Lỗi Hàm Disbursement: " + a);
        }
        return res;
    }
    public string DebtPayment(string custid,string contractNumber, string paymentAmt, string partnerLocationId, string refNo, string orgTranDate, string valDate, string paymentDetails, string shbAccountNumber)
    // public  string doPayment()
    {
        SHBFC.AppHdrType appHdr = new SHBFC.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        SHBFC.PairsType nsFrom = new SHBFC.PairsType();
        nsFrom.Id = "SOA";
        nsFrom.Name = "SOA";

        SHBFC.PairsType nsTo = new SHBFC.PairsType();
        nsTo.Id = "SHBFC";
        nsTo.Name = "SHBFC";

        SHBFC.PairsType[] listOfNsTo = new SHBFC.PairsType[1];
        listOfNsTo[0] = nsTo;

        SHBFC.PairsType BizSvc = new SHBFC.PairsType();
        BizSvc.Id = "SHBFC";
        BizSvc.Name = "SHBFC";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        SHBFC.DebtPaymentReqType msgReq = new SHBFC.DebtPaymentReqType();
        msgReq.AppHdr = appHdr;
        msgReq.contractNumber = contractNumber;
        msgReq.paymentAmt = paymentAmt;
        msgReq.partnerLocationId = partnerLocationId;
        msgReq.refNo = refNo;
        msgReq.orgTranDate = orgTranDate;
        msgReq.valDate = valDate;
        msgReq.shbAccountNumber = shbAccountNumber;
        msgReq.paymentDetails = paymentDetails;
        Funcs.WriteLog("custid:" + custid + "|Hàm DebtPayment: " + msgReq.AppHdr.MsgId + "Đầu vào hàm DebtPayment: contractNumber:" + msgReq.contractNumber + ",paymentAmt:" + msgReq.paymentAmt + ",partnerLocationId:" + msgReq.partnerLocationId
            + "refNo:" + msgReq.refNo + ",orgTranDate:" + msgReq.orgTranDate + ",valDate:" + msgReq.valDate + ",shbAccountNumber:" + msgReq.shbAccountNumber);
        SHBFC.DebtPaymentResType res = null;
        try
        {

            SHBFC.PortTypeClient ptc = new SHBFC.PortTypeClient();
            Funcs.WriteLog("Vào hàm : ");

            res = ptc.DebtPayment(msgReq);
            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("custid:" + custid + "|Res DebtPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("Exception when write log response: " + subEx.Message);
                }
                // result = res.EvnHNBillDetailItem;
            }

            // Log.WriteLog("Kết thúc hàm PolicyInquiry: ");
            ptc.Close();

        }
        catch (Exception e)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            object a = e.Message;
            Funcs.WriteLog("custid:" + custid + "|Lỗi Hàm DebtPayment: " + a);
        }

        if (res != null)
        {
            if (res.statusCode.Equals("000"))
            {
                return Config.ERR_CODE_DONE;
            }

            else if (res.statusCode.Equals("400"))
            {
                return res.result.errorCode;
            }
            else
            {
                return Config.ERR_CODE_GENERAL;
            }

        }

        return Config.ERR_CODE_GENERAL;
    }
    public SHBFC.RevertPaymentResType RevertPayment(string custid,string revertRefNo, string revertTranDate, string transactionDate, string refNo)
    // public  string doPayment()
    {
        SHBFC.AppHdrType appHdr = new SHBFC.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        SHBFC.PairsType nsFrom = new SHBFC.PairsType();
        nsFrom.Id = "SOA";
        nsFrom.Name = "SOA";

        SHBFC.PairsType nsTo = new SHBFC.PairsType();
        nsTo.Id = "SHBFC";
        nsTo.Name = "SHBFC";

        SHBFC.PairsType[] listOfNsTo = new SHBFC.PairsType[1];
        listOfNsTo[0] = nsTo;

        SHBFC.PairsType BizSvc = new SHBFC.PairsType();
        BizSvc.Id = "SHBFC";
        BizSvc.Name = "SHBFC";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        SHBFC.RevertPaymentReqType msgReq = new SHBFC.RevertPaymentReqType();
        msgReq.AppHdr = appHdr;
        msgReq.revertRefNo = revertRefNo;
        msgReq.revertTranDate = revertTranDate;
        msgReq.transactionDate = transactionDate;
        msgReq.refNo = refNo;

        Funcs.WriteLog("custid:" + custid + "|Hàm RevertPaymentReqType: " + msgReq.AppHdr.MsgId + "Đầu vào hàm RevertPaymentReqType: revertRefNo:" + msgReq.revertRefNo + ",revertTranDate:" + msgReq.revertTranDate + ",transactionDate:" + msgReq.transactionDate
            + "refNo:" + msgReq.refNo);
        SHBFC.RevertPaymentResType res = null;
        try
        {

            SHBFC.PortTypeClient ptc = new SHBFC.PortTypeClient();
            Funcs.WriteLog("Vào hàm RevertPayment: ");

            res = ptc.RevertPayment(msgReq);
            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("custid:" + custid + "|Res RevertPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("custid:" + custid + "|Exception when write log response: " + subEx.Message);
                }
                // result = res.EvnHNBillDetailItem;
            }

            // Log.WriteLog("Kết thúc hàm PolicyInquiry: ");
            ptc.Close();

        }
        catch (Exception e)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            object a = e.Message;
            Funcs.WriteLog("custid:" + custid + "|Lỗi Hàm RevertPayment: " + a);
        }
        return res;
    }
}