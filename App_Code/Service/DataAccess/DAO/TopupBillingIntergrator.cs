using mobileGW.Service.Framework;
using NapasBillPmt;
using ONEPAYBillPmt;
using PayooBillPmt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using VnPayBillPmt;

/// <summary>
/// Summary description for TopupBillingIntergrator
/// </summary>
public class TopupBillingIntergrator
{
    public TopupBillingIntergrator()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary>
    /// post Message Payment To NganLuong
    /// </summary>
    /// <param name="cusID"></param>
    /// <param name="invoiceObject"></param>
    /// <param name="InvSts"></param>
    /// <param name="Tranid"></param>
    /// <returns></returns>
    public string postMessagePaymentToNganLuong(string cusID, NLBillPmt.NLBillPmtVerifyResType invoiceObject, byte InvSts, string Tranid)
    {
        string respMsg = string.Empty;
        NLBillPmt.NLBillPmtTopupResType res = null;
        try
        {
            #region header
            NLBillPmt.AppHdrType appHdr = new NLBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            NLBillPmt.PairsType nsFrom = new NLBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            NLBillPmt.PairsType nsTo = new NLBillPmt.PairsType();
            nsTo.Id = "NL";
            nsTo.Name = "NL";

            NLBillPmt.PairsType[] listOfNsTo = new NLBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            NLBillPmt.PairsType BizSvc = new NLBillPmt.PairsType();
            BizSvc.Id = "EBANK";
            BizSvc.Name = "EBANK";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            NLBillPmt.NLBillPmtTopupReqType msgReq = new NLBillPmt.NLBillPmtTopupReqType();
            msgReq.AppHdr = appHdr;

            msgReq.NganLuongId = invoiceObject.NganLuongId;

            msgReq.InvSts = InvSts.ToString();
            msgReq.NLTxnId = invoiceObject.NLTxnId;
            msgReq.SHBTxnId = Tranid;
            msgReq.TxnTime = invoiceObject.TxnTime;
            msgReq.TxnAmt = invoiceObject.TxnAmt;

            //sign
            appHdr.Signature = Funcs.MD5HashEncoding(msgReq.NganLuongId + msgReq.NLTxnId + msgReq.SHBTxnId + Config.SharedKeyMD5).ToUpper();
            #endregion
            
            NLBillPmt.PortTypeClient ptc = new NLBillPmt.PortTypeClient();
            res = ptc.Topup(msgReq);

            Funcs.WriteLog("custid:" + cusID + "|postMessagePaymentToNganLuong|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("postMessagePaymentToNganLuong EXCEPTION FROM ESB: " + ex.ToString());
            //set timeout
            //OLD
            //respMsg = Config.ERR_CODE_TIMEOUT;

            //NEW
            //linhtn fix bug loi tra ve cua NGANLUONG 2017 09 25
            //neu timeout or exception chung chung
            respMsg = Config.RET_CODE_NLUONG[1].Split('|')[0]; //68
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return Config.RET_CODE_NLUONG[0].Split('|')[0];
        }

        if (res.RespSts != null && res.RespSts.ErrInfo != null && res.RespSts.ErrInfo.Length > 0 && res.RespSts.ErrInfo[0] != null)
        {
            return res.RespSts.ErrInfo[0].ErrCd;
        }

        return respMsg;
    }

    /// <summary>
    /// get NL Invoice Code
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="amount"></param>
    /// <param name="custId"></param>
    /// <returns></returns>
    public NLBillPmt.NLBillPmtVerifyResType getNLInvoiceCode(string orderId, string amount, string custId)
    {
        NLBillPmt.NLBillPmtVerifyResType res = null;

        try
        {
            #region header
            NLBillPmt.AppHdrType appHdr = new NLBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            NLBillPmt.PairsType nsFrom = new NLBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            NLBillPmt.PairsType nsTo = new NLBillPmt.PairsType();
            nsTo.Id = "NL";
            nsTo.Name = "NL";

            NLBillPmt.PairsType[] listOfNsTo = new NLBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            NLBillPmt.PairsType BizSvc = new NLBillPmt.PairsType();
            BizSvc.Id = "EBANK";
            BizSvc.Name = "EBANK";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            NLBillPmt.NLBillPmtVerifyReqType msgReq = new NLBillPmt.NLBillPmtVerifyReqType();
            msgReq.AppHdr = appHdr;

            msgReq.NganLuongId = orderId;
            msgReq.InvoiceID = "GET_INVOICE";
            msgReq.TxnAmt = amount;
            #endregion

            NLBillPmt.PortTypeClient ptc = new NLBillPmt.PortTypeClient();
            res = new NLBillPmt.NLBillPmtVerifyResType();
            res = ptc.Verify(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getNLInvoiceCode|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("getNLInvoiceCode EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    /// <summary>
    /// get Bill Info From Payoo
    /// </summary>
    /// <param name="custId"></param>
    /// <param name="billId"></param>
    /// <returns></returns>
    public PayooBillPmtInquiryResType getBillInfoFromPayoo(string custId, string billId)
    {
        PayooBillPmt.PayooBillPmtInquiryResType res = null;

        try
        {
            #region header
            PayooBillPmt.AppHdrType appHdr = new PayooBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            PayooBillPmt.PairsType nsFrom = new PayooBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            PayooBillPmt.PairsType nsTo = new PayooBillPmt.PairsType();
            nsTo.Id = "PayooBillPmt_Inquiry";
            nsTo.Name = "PayooBillPmt_Inquiry";

            PayooBillPmt.PairsType[] listOfNsTo = new PayooBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            PayooBillPmt.PairsType BizSvc = new PayooBillPmt.PairsType();
            BizSvc.Id = "PayooBillPmt_Inquiry";
            BizSvc.Name = "PayooBillPmt_Inquiry";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            PayooBillPmt.PayooBillPmtInquiryReqType msgReq = new PayooBillPmt.PayooBillPmtInquiryReqType();
            msgReq.AppHdr = appHdr;

            msgReq.UserId = Config.PayooConfig.UserId;
            msgReq.AgentId = Config.PayooConfig.AgentId;
            msgReq.CustId = billId;
            msgReq.PvdId = Config.PayooConfig.ProviderId;
            msgReq.SvcId = Config.PayooConfig.ServiceId;
            #endregion

            PayooBillPmt.PortTypeClient ptc = new PayooBillPmt.PortTypeClient();
            res = new PayooBillPmt.PayooBillPmtInquiryResType();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getBillInfoFromPayoo|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("getBillInfoFromPayoo EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    /// <summary>
    /// get Bill Info From Onepay
    /// </summary>
    /// <param name="tranId"></param>
    /// <param name="custId"></param>
    /// <param name="categoryCode"></param>
    /// <param name="serviceCode"></param>
    /// <param name="billId"></param>
    /// <returns></returns>
    public ONEPAYBillPmtInquiryResType getBillInfoFromOnepay(string tranId, string custId, string categoryCode, string serviceCode, string billId)
    {
        ONEPAYBillPmt.ONEPAYBillPmtInquiryResType res = null;

        try
        {
            #region header
            ONEPAYBillPmt.AppHdrType appHdr = new ONEPAYBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            ONEPAYBillPmt.PairsType nsFrom = new ONEPAYBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            ONEPAYBillPmt.PairsType nsTo = new ONEPAYBillPmt.PairsType();
            nsTo.Id = "CORE";
            nsTo.Name = "CORE";

            ONEPAYBillPmt.PairsType[] listOfNsTo = new ONEPAYBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            ONEPAYBillPmt.PairsType BizSvc = new ONEPAYBillPmt.PairsType();
            BizSvc.Id = "ONEPAYBillPmt_query";
            BizSvc.Name = "ONEPAYBillPmt_query";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            ONEPAYBillPmt.ONEPAYBillPmtInquiryReqType msgReq = new ONEPAYBillPmt.ONEPAYBillPmtInquiryReqType();
            msgReq.AppHdr = appHdr;

            msgReq.ProcCd = categoryCode;
            msgReq.PvdId = serviceCode;
            msgReq.CustId = billId;
            msgReq.AuditNo = Funcs.GetLast(tranId.ToString(), 6);
            #endregion

            ONEPAYBillPmt.PortTypeClient ptc = new ONEPAYBillPmt.PortTypeClient();
            res = new ONEPAYBillPmt.ONEPAYBillPmtInquiryResType();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getBillInfoFromOnepay|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("getBillInfoFromOnepay EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    /// <summary>
    /// get Bill Info From Vnpay
    /// </summary>
    /// <param name="tranId"></param>
    /// <param name="custId"></param>
    /// <param name="categoryCode"></param>
    /// <param name="serviceCode"></param>
    /// <param name="billId"></param>
    /// <returns></returns>
    public VNPAYBillPmtInquiryResType getBillInfoFromVnpay(string tranId, string custId, string categoryCode, string serviceCode, string billId)
    {
        VnPayBillPmt.VNPAYBillPmtInquiryResType res = null;

        try
        {
            #region header
            VnPayBillPmt.AppHdrType appHdr = new VnPayBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            VnPayBillPmt.PairsType nsFrom = new VnPayBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            VnPayBillPmt.PairsType nsTo = new VnPayBillPmt.PairsType();
            nsTo.Id = "CORE";
            nsTo.Name = "CORE";

            VnPayBillPmt.PairsType[] listOfNsTo = new VnPayBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            VnPayBillPmt.PairsType BizSvc = new VnPayBillPmt.PairsType();
            BizSvc.Id = "VnPayBillPmt_query";
            BizSvc.Name = "VnPayBillPmt_query";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            VnPayBillPmt.VNPAYBillPmtInquiryReqType msgReq = new VnPayBillPmt.VNPAYBillPmtInquiryReqType();
            msgReq.AppHdr = appHdr;

            msgReq.ProcCd = "000002";
            msgReq.AcctId = "0000000000";
            msgReq.CustId = custId;
            msgReq.AdditionalData = serviceCode + "@" + categoryCode + "@" + billId;
            msgReq.AuditNo = Funcs.GetLast(tranId.ToString(), 6);
            #endregion

            VnPayBillPmt.PortTypeClient ptc = new VnPayBillPmt.PortTypeClient();
            res = new VnPayBillPmt.VNPAYBillPmtInquiryResType();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getBillInfoFromVnpay|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("getBillInfoFromVnpay EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    /// <summary>
    /// get bill info from napas
    /// </summary>
    /// <param name="tranId"></param>
    /// <param name="custId"></param>
    /// <param name="categoryId"></param>
    /// <param name="serviceId"></param>
    /// <param name="billId"></param>
    /// <param name="partnerId"></param>
    /// <param name="serviceCode"></param>
    /// <returns></returns>
    public NAPASBillPmtInquiryResType getBillInfoFromNapas(string tranId, string custId, string categoryId, string serviceId, string billId, string partnerId, string serviceCode)
    {
        NapasBillPmt.NAPASBillPmtInquiryResType res = null;

        try
        {
            #region header
            NapasBillPmt.AppHdrType appHdr = new NapasBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            NapasBillPmt.PairsType nsFrom = new NapasBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            NapasBillPmt.PairsType nsTo = new NapasBillPmt.PairsType();
            nsTo.Id = "CORE";
            nsTo.Name = "CORE";

            NapasBillPmt.PairsType[] listOfNsTo = new NapasBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            NapasBillPmt.PairsType BizSvc = new NapasBillPmt.PairsType();
            BizSvc.Id = "NapasBillPmt_query";
            BizSvc.Name = "NapasBillPmt_query";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            NapasBillPmt.NAPASBillPmtInquiryReqType msgReq = new NapasBillPmt.NAPASBillPmtInquiryReqType();
            msgReq.AppHdr = appHdr;
            msgReq.AcctId = custId;
            msgReq.TxnAmt = "000000000000";
            msgReq.MerchantType = "7399";
            msgReq.TermId = "11111111";
            msgReq.CardAcceptor = "SHB                      EBANKING    VNM";
            msgReq.SvcCd = serviceCode;
            msgReq.AdditionalInfo = billId;
            msgReq.AuditNo = Funcs.GetLast(tranId.ToString(), 6);

            StringBuilder rawSign = new StringBuilder();
            rawSign.Append(msgReq.AcctId).Append(msgReq.TxnAmt).Append(msgReq.AuditNo).Append(msgReq.TermId).Append(Config.SharedKeyMD5);
            appHdr.Signature = Funcs.MD5HashEncoding(rawSign.ToString());
            #endregion

            NapasBillPmt.PortTypeClient ptc = new NapasBillPmt.PortTypeClient();
            res = new NapasBillPmt.NAPASBillPmtInquiryResType();
            res = ptc.Inquiry(msgReq);

            Funcs.WriteLog("custid:" + custId + "|getBillInfoFromNapas|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("getBillInfoFromNapas EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }


    /// <summary>
    /// post Message Payment To Payoo
    /// </summary>
    /// <param name="Tran_ID"></param>
    /// <param name="BillID"></param>
    /// <param name="TotalAmount"></param>
    /// <param name="CustomerId"></param>
    /// <param name="CustomerName"></param>
    /// <param name="Month"></param>
    /// <param name="acctno"></param>
    /// <param name="bank_custid"></param>
    /// <returns></returns>
    public string postMessagePaymentToPayoo(string Tran_ID, 
        string BillID, 
        decimal TotalAmount,
        string CustomerId,
        string CustomerName,
        string Month,
        string acctno,
        string bank_custid)
    {
        string respMsg = string.Empty;
        PayooBillPmt.PayooBillPmtCreateResType res = null;

        try
        {
            #region header
            PayooBillPmt.AppHdrType appHdr = new PayooBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            PayooBillPmt.PairsType nsFrom = new PayooBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            PayooBillPmt.PairsType nsTo = new PayooBillPmt.PairsType();
            nsTo.Id = "PayooBillPmt_Payment";
            nsTo.Name = "PayooBillPmt_Payment";

            PayooBillPmt.PairsType[] listOfNsTo = new PayooBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            PayooBillPmt.PairsType BizSvc = new PayooBillPmt.PairsType();
            BizSvc.Id = "PayooBillPmt_Payment";
            BizSvc.Name = "PayooBillPmt_Payment";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            PayooBillPmt.PayooBillPmtCreateReqType msgReq = new PayooBillPmt.PayooBillPmtCreateReqType();
            msgReq.AppHdr = appHdr;

            msgReq.UserId = Config.PayooConfig.UserId;
            msgReq.AgentId = Config.PayooConfig.AgentId;
            msgReq.CustId = CustomerId;
            msgReq.InvoiceNo = Tran_ID;
            msgReq.TraceId = Tran_ID;
            msgReq.TxnDt = TransDt.ToString("yyyyMMddHHmmss");
            msgReq.BillInfo = new PayooBillPmt.BillInfoType();
            msgReq.BillInfo.BillId = BillID;
            msgReq.BillInfo.TxnAmt = TotalAmount.ToString();
            msgReq.BillInfo.CustId = CustomerId;
            msgReq.BillInfo.CustName = CustomerName;
            msgReq.BillInfo.Month = Month;
            msgReq.BillInfo.PvdId = Config.PayooConfig.ProviderId;
            msgReq.BillInfo.SvcId = Config.PayooConfig.ServiceId;
            msgReq.BnkCustId = bank_custid;
            msgReq.AcctNo = acctno;

            appHdr.Signature = Funcs.MD5HashEncoding(msgReq.CustId + msgReq.InvoiceNo
                + msgReq.BillInfo.BillId.ToString()
                + msgReq.BillInfo.TxnAmt
                + msgReq.BillInfo.CustId 
                + msgReq.BillInfo.PvdId 
                + msgReq.BillInfo.SvcId 
                + msgReq.TraceId 
                + Config.SharedKeyMD5);
            #endregion

            PayooBillPmt.PortTypeClient ptc = new PayooBillPmt.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("custid:" + CustomerId + "|postMessagePaymentToPayoo|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("postMessagePaymentToPayoo EXCEPTION FROM ESB: " + ex.ToString());
            //set timeout
            //OLD
            //respMsg = Config.ERR_CODE_TIMEOUT;

            //NEW
            //linhtn fix bug loi tra ve cua PAYOO 2017 09 25
            //neu timeout or exception chung chung
            respMsg = Config.RET_CODE_PAYOO[1].Split('|')[0]; //68

        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return Config.RET_CODE_PAYOO[0].Split('|')[0];
        }

        if (res.RespSts != null && res.RespSts.ErrInfo != null && res.RespSts.ErrInfo.Length > 0 && res.RespSts.ErrInfo[0] != null)
        {
            return res.RespSts.ErrInfo[0].ErrCd;
        }

        return respMsg;
    }

    /// <summary>
    /// post Message Payment To Onepay
    /// </summary>
    /// <param name="cusId"></param>
    /// <param name="CustomerCode"></param>
    /// <param name="ProcessingCode"></param>
    /// <param name="SettlementAmount"></param>
    /// <param name="AuditNumber"></param>
    /// <param name="PayChannel"></param>
    /// <param name="TerminalId"></param>
    /// <param name="Location"></param>
    /// <param name="BankDescription"></param>
    /// <param name="ProviderId"></param>
    /// <returns></returns>
    public string postMessagePaymentToOnepay(string cusId, string CustomerCode, string ProcessingCode, double SettlementAmount, string AuditNumber, string PayChannel, string TerminalId, string Location, string BankDescription, string ProviderId)
    {
        string respMsg = string.Empty;
        ONEPAYBillPmt.ONEPAYBillPmtCreateResType res = null;
        try
        {
            #region header
            ONEPAYBillPmt.AppHdrType appHdr = new ONEPAYBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            ONEPAYBillPmt.PairsType nsFrom = new ONEPAYBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            ONEPAYBillPmt.PairsType nsTo = new ONEPAYBillPmt.PairsType();
            nsTo.Id = "ONEPAY";
            nsTo.Name = "ONEPAY";

            ONEPAYBillPmt.PairsType[] listOfNsTo = new ONEPAYBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            ONEPAYBillPmt.PairsType BizSvc = new ONEPAYBillPmt.PairsType();
            BizSvc.Id = "ONEPAYBillPmt_create";
            BizSvc.Name = "ONEPAYBillPmt_create";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;


            //sign
            StringBuilder signRaw = new StringBuilder();
            signRaw.Append(CustomerCode).Append(ProcessingCode).Append(SettlementAmount).Append(Config.SharedKeyMD5);
            appHdr.Signature = Funcs.MD5HashEncoding(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);
            #endregion

            #region body
            ONEPAYBillPmt.ONEPAYBillPmtCreateReqType msgReq = new ONEPAYBillPmt.ONEPAYBillPmtCreateReqType();
            msgReq.AppHdr = appHdr;

            msgReq.ProcCd = ProcessingCode;
            msgReq.PvdId = ProviderId;
            msgReq.CustId = CustomerCode;
            msgReq.TxnAmt = SettlementAmount.ToString();
            msgReq.TxnDate = DateTime.Now.ToString("yyyyMMdd");
            msgReq.PayChnl = PayChannel;
            msgReq.TerminalId = TerminalId;
            msgReq.Location = Location;
            msgReq.Remark = BankDescription;
            msgReq.AuditNo = AuditNumber;
            #endregion

            ONEPAYBillPmt.PortTypeClient ptc = new ONEPAYBillPmt.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("custid:" + cusId + "|postMessagePaymentToOnepay|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("postMessageTopupToVNPay EXCEPTION FROM ESB: " + ex.ToString());
            //set timeout
            //OLD
            //respMsg = Config.ERR_CODE_TIMEOUT;

            //NEW
            //linhtn fix bug loi tra ve cua ONEPAY 2017 09 25
            //neu timeout or exception chung chung
            respMsg = Config.RET_CODE_ONEPAY[1].Split('|')[0]; //68
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return Config.RET_CODE_ONEPAY[0].Split('|')[0];
        }

        if (res.RespSts != null && res.RespSts.ErrInfo != null && res.RespSts.ErrInfo.Length > 0 && res.RespSts.ErrInfo[0] != null)
        {
            return res.RespSts.ErrInfo[0].ErrCd;
        }

        return respMsg;
    }

    /// <summary>
    /// post payment message to vnpay
    /// </summary>
    /// <param name="custId"></param>
    /// <param name="tranId"></param>
    /// <param name="amount"></param>
    /// <param name="orderId"></param>
    /// <param name="price"></param>
    /// <returns></returns>
    public string postMessagePaymentToVNPay(string custId, string serviceCode, string categoryCode, string orderId, double tranId, double amount, string procCd)
    {
        string respMsg = string.Empty;
        VnPayBillPmt.VNPAYBillPmtCreateResType res = null;
        try
        {
            #region header
            VnPayBillPmt.AppHdrType appHdr = new VnPayBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            VnPayBillPmt.PairsType nsFrom = new VnPayBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            VnPayBillPmt.PairsType nsTo = new VnPayBillPmt.PairsType();
            nsTo.Id = "VNPAY";
            nsTo.Name = "VNPAY";

            VnPayBillPmt.PairsType[] listOfNsTo = new VnPayBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            VnPayBillPmt.PairsType BizSvc = new VnPayBillPmt.PairsType();
            BizSvc.Id = "VnPayBillPmt_create";
            BizSvc.Name = "VnPayBillPmt_create";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            VnPayBillPmt.VNPAYBillPmtCreateReqType msgReq = new VnPayBillPmt.VNPAYBillPmtCreateReqType();
            msgReq.AppHdr = appHdr;

            msgReq.ProcCd = procCd;
            msgReq.TxtAmount = amount + "00".ToString();
            msgReq.TxtAmount = msgReq.TxtAmount.PadLeft(12, '0');
            msgReq.CustId = custId;
            msgReq.AcctId = orderId;

            msgReq.AuditNo = Funcs.GetLast(tranId.ToString(), 6);

            if (string.IsNullOrEmpty(serviceCode) && string.IsNullOrEmpty(categoryCode))
            {
                msgReq.AdditionalData = orderId + "@" + orderId + "@" + (amount.ToString() + "00").PadLeft(12, '0');
            }
            else
            {
                msgReq.AdditionalData = serviceCode + "@" + categoryCode + "@" + orderId + "@" + (amount.ToString() + "00").PadLeft(12, '0');
            }

            //sign
            StringBuilder signRaw = new StringBuilder();
            signRaw.Append(custId).Append(msgReq.ProcCd).Append(msgReq.AuditNo).Append(msgReq.TxtAmount)
                .Append(msgReq.AcctId).Append(Config.SharedKeyMD5);
            appHdr.Signature = Funcs.MD5HashEncoding(signRaw.ToString());
            #endregion

            Funcs.WriteLog("custid:" + custId + "|postMessagePaymentToVNPay|id request: " + appHdr.MsgId);

            VnPayBillPmt.PortTypeClient ptc = new VnPayBillPmt.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("custid:" + custId + "|postMessagePaymentToVNPay|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("postMessageTopupToVNPay EXCEPTION FROM ESB: " + ex.ToString());
            //set timeout
            //OLD
            //respMsg = Config.ERR_CODE_TIMEOUT;


            //NEW
            //linhtn fix bug loi tra ve cua VNPAY 2017 09 25
            //neu timeout or exception chung chung
            respMsg = Config.RET_CODE_VNPAY[1].Split('|')[0]; //68
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return Config.RET_CODE_VNPAY[0].Split('|')[0];
        }

        if (res.RespSts != null && res.RespSts.ErrInfo != null && res.RespSts.ErrInfo.Length > 0 && res.RespSts.ErrInfo[0] != null)
        {
            return res.RespSts.ErrInfo[0].ErrCd;
        }
        return respMsg;
    }

    /// <summary>
    /// post payment message to napas
    /// </summary>
    /// <param name="custId"></param>
    /// <param name="srcAcct"></param>
    /// <param name="amount"></param>
    /// <param name="tranId"></param>
    /// <param name="serviceCode"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public string postMessagePaymentToNapas(string custId, string srcAcct, double amount, double tranId, string serviceCode, string orderId)
    {
        string respMsg = string.Empty;
        NapasBillPmt.NAPASBillPmtCreateResType res = null;

        try
        {
            #region header
            NapasBillPmt.AppHdrType appHdr = new NapasBillPmt.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            NapasBillPmt.PairsType nsFrom = new NapasBillPmt.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            NapasBillPmt.PairsType nsTo = new NapasBillPmt.PairsType();
            nsTo.Id = "VAS";
            nsTo.Name = "VAS";

            NapasBillPmt.PairsType[] listOfNsTo = new NapasBillPmt.PairsType[1];
            listOfNsTo[0] = nsTo;

            NapasBillPmt.PairsType BizSvc = new NapasBillPmt.PairsType();
            BizSvc.Id = "NapasBillPmt_create";
            BizSvc.Name = "NapasBillPmt_create";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            NapasBillPmt.NAPASBillPmtCreateReqType msgReq = new NapasBillPmt.NAPASBillPmtCreateReqType();
            msgReq.AppHdr = appHdr;

            msgReq.AcctId = srcAcct;
            msgReq.TxnAmt = (amount.ToString() + "00").PadLeft(12, '0');
            msgReq.AuditNo = Funcs.GetLast(tranId.ToString(), 6);
            msgReq.MerchantType = "7399";
            msgReq.TermId = "11111111";
            msgReq.CardAcceptor = "SHB                      EBANKING    VNM";
            msgReq.SvcCd = serviceCode;
            msgReq.AdditionalInfo = orderId;

            //sign
            StringBuilder signRaw = new StringBuilder();
            signRaw.Append(msgReq.AcctId).Append(msgReq.TxnAmt).Append(msgReq.AuditNo).Append(msgReq.TermId).Append(Config.SharedKeyMD5);
            appHdr.Signature = Funcs.MD5HashEncoding(signRaw.ToString());
            #endregion 

            NapasBillPmt.PortTypeClient ptc = new NapasBillPmt.PortTypeClient();
            res = ptc.Create(msgReq);

            Funcs.WriteLog("custid:" + custId + "|postMessagePaymentToNapas|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("postMessageTopupToNapas EXCEPTION FROM ESB: " + ex.ToString());
            //set timeout
            //respMsg = Config.ERR_CODE_TIMEOUT;
            //linhtn fix bug loi tra ve cua NAPAS 2017 09 25
            //neu timeout or exception chung chung
            respMsg = Config.RET_CODE_NAPAS_TOPUP[1].Split('|')[0]; //68

        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return Config.RET_CODE_NAPAS_TOPUP[0].Split('|')[0];
        }
        if (res.RespSts != null && res.RespSts.ErrInfo != null && res.RespSts.ErrInfo.Length > 0 && res.RespSts.ErrInfo[0] != null)
        {
            return res.RespSts.ErrInfo[0].ErrCd;
        }
        return respMsg;
    }
}