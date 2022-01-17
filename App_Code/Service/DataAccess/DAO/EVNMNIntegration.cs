using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using EVNMN;

/// <summary>
/// Summary description for DAWACOIntegration
/// </summary>
public class EVNMNIntegration
{
    public EVNMNIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //Lấy thông tin khách hàng
    #region Lấy thông tin khách hàng
    public static GetCustomerInfoResType GetCustomerInfo(string bankId, string customerCode)
    {
        GetCustomerInfoResType res = null;
        try
        {

            EVNMN.AppHdrType appHdr = buildHeaderInfo();

            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            //QueryCustomerAddressReqType msgReq = new QueryCustomerAddressReqType();
            GetCustomerInfoReqType msgReq = new GetCustomerInfoReqType();

            msgReq.AppHdr = appHdr;
            msgReq.BankID = bankId;
            msgReq.CustomerCode = customerCode;

            Funcs.WriteLog("CUSTOMER_CODE: " + customerCode + "|BANK_ID: " + bankId + "|req QueryCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            EVNMN.PortTypeClient ptc = new EVNMN.PortTypeClient();
            try
            {
                res = ptc.GetCustomerInfo(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CUSTOMER_CODE: " + customerCode + "|BANK_ID: " + bankId + "| GetCustomerInfoResponse: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CUSTOMER_CODE: " + customerCode + "|BANK_ID: " + bankId + "| Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                Funcs.WriteLog("CUSTOMER_CODE: " + customerCode + "|BANK_ID: " + bankId + "Lỗi Hàm CustomerInfoReqType: " + e.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CUSTOMER_CODE: " + customerCode + "|BANK_ID: " + bankId + "|ERROR QueryCustomerAddress: " + "Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    #endregion


    //Thanh toan hoa don
    #region Thanh toan hoa don theo mang
    public static PayBillsByCustomerCodeResType PayBillsByCustomerCode(string custID, string customerCode, string bankId,
        string transType, string[] billCodeArr, long[] amountArr, string[] transactionCodeArr, string departCode, string kyhieuHD)
    {
        PayBillsByCustomerCodeResType res = null;
        try
        {
            EVNMN.AppHdrType appHdr = buildHeaderInfo();
            DateTime TransDt = DateTime.Now;

            //Body
            PayBillsByCustomerCodeReqType msgReq = new PayBillsByCustomerCodeReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustomerCode = customerCode;
            msgReq.BankID = bankId;
            msgReq.DaInHD = "0";
            msgReq.DepartCode = departCode;
            msgReq.KyHieuHoaDon = kyhieuHD;

            msgReq.Amount = amountArr;
            msgReq.BillCodes = billCodeArr;

            msgReq.PayDate = TransDt.ToString("yyyy-MM-ddTHH:MM:ss.FFFZ");
            msgReq.TransactionCode = transactionCodeArr;


            Funcs.WriteLog("CIF_NO: " + custID + "|req PayBillInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            EVNMN.PortTypeClient ptc = new EVNMN.PortTypeClient();
            try
            {
                res = ptc.PayBillsByCustomerCode(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|res PayBillInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|ERROR PayBillInfo: " + "| Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                Funcs.WriteLog("CIF_NO: " + custID + "|ERROR PayBillInfo: " + "Lỗi Hàm PayBill: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custID + "|ERROR PayBillInfo: " + "Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    #endregion

    //Lấy thông tin nợ
    #region Cancel thanh toan hoa don
    public static CancelBillsByCustomerCodeResType CancelBills(string custID, string customerCode, string bankId, string[] billCodeArr, long [] amountArr, string [] transactionCodeArr)
    {
        CancelBillsByCustomerCodeResType res = null;
        try
        {
            EVNMN.AppHdrType appHdr = buildHeaderInfo();
            DateTime TransDt = DateTime.Now;

            //Body
            CancelBillsByCustomerCodeReqType msgReq = new CancelBillsByCustomerCodeReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustomerCode = customerCode;
            msgReq.BankID = bankId;
            msgReq.Amount = amountArr;
            msgReq.BillCodes = billCodeArr;
            msgReq.TransactionCode = transactionCodeArr;

            msgReq.CancelDate = TransDt.ToString("yyyy-MM-ddTHH:MM:ss.FFFZ");


            Funcs.WriteLog("CIF_NO: " + custID + "|req CancelBill: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            EVNMN.PortTypeClient ptc = new EVNMN.PortTypeClient();
            try
            {
                res = ptc.CancelBillsByCustomerCode(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|res CancelBill: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|ERROR CancelBill: " + "| Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                Funcs.WriteLog("CIF_NO: " + custID + "|ERROR CancelBill: " + "Lỗi Hàm CancelBill: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custID + "|ERROR CancelBill: " + "Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    #endregion

    ////Service xác nhận khách hàng thanh toán 1 hóa đơn nợ
    //#region Service xác nhận khách hàng thanh toán 1 hóa đơn nợ
    //public static TransConfirmV2RespType TransConfirmV2(string custID, string customerCd, string auditNumber, string transType, string IdHD, string MaDL, string ServiceType, string Amount, string Point_of_service, string BillPrintStatus)
    //{
    //    TransConfirmV2RespType res = null;
    //    try
    //    {
    //        EVNMN.AppHdrType appHdr = new EVNMN.AppHdrType();
    //        appHdr.CharSet = "UTF-8";
    //        appHdr.SvcVer = "2.0";

    //        EVNMN.PairsType nsFrom = new EVNMN.PairsType();
    //        nsFrom.Id = "ESB";
    //        nsFrom.Name = "ESB";

    //        EVNMN.PairsType nsTo = new EVNMN.PairsType();
    //        nsTo.Id = "ESB_SOA";
    //        nsTo.Name = "ESB_SOA";

    //        EVNMN.PairsType[] listOfNsTo = new EVNMN.PairsType[1];
    //        listOfNsTo[0] = nsTo;

    //        EVNMN.PairsType BizSvc = new EVNMN.PairsType();
    //        BizSvc.Id = "EVNNPC";
    //        BizSvc.Name = "EVNNPC";

    //        DateTime TransDt = DateTime.Now;

    //        appHdr.From = nsFrom;
    //        appHdr.To = listOfNsTo;
    //        appHdr.MsgId = Guid.NewGuid().ToString();
    //        appHdr.MsgPreId = "";
    //        appHdr.BizSvc = BizSvc;
    //        appHdr.TransDt = TransDt;
    //        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

    //        //Body
    //        TransConfirmV2ReqType msgReq = new TransConfirmV2ReqType();
    //        msgReq.AppHdr = appHdr;
    //        msgReq.CustomersID = customerCd;
    //        msgReq.PaymentAmount = Amount;
    //        msgReq.AuditNumber = auditNumber;
    //        msgReq.TransDate = TransDt.ToString("dd/MM/yyyy HH:mm:ss");
    //        msgReq.TransType = transType;
    //        msgReq.IdHD = IdHD;
    //        msgReq.Point_of_service = Point_of_service;
    //        msgReq.MaDL = MaDL;
    //        msgReq.BillPrintStatus = BillPrintStatus;
    //        msgReq.LoaiDichVu = ServiceType;

    //        Funcs.WriteLog("CIF_NO: " + custID + "|req TransConfirmV2: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

    //        EVNMN.PortTypeClient ptc = new EVNMN.PortTypeClient();
    //        try
    //        {
    //            res = ptc.TransConfirmV2(msgReq);
    //            if (res != null)
    //            {
    //                try
    //                {
    //                    Funcs.WriteLog("CIF_NO: " + custID + "|res TransConfirmV2: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
    //                }
    //                catch (Exception subEx)
    //                {
    //                    Funcs.WriteLog("CIF_NO: " + custID + "|Exception when write log response: " + subEx.Message.ToString());
    //                }
    //            }
    //            ptc.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            Funcs.WriteLog("CIF_NO: " + custID + "|Lỗi Hàm TransConfirmV2: " + e.Message.ToString());
    //        }

    //    }
    //    catch (Exception e)
    //    {
    //        Funcs.WriteLog("CIF_NO: " + custID + "|Exception: " + e.Message.ToString());
    //        throw e;
    //    }
    //    finally
    //    {
    //        // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
    //    }

    //    return res;
    //}
    //#endregion


    // Build header info

    public static EVNMN.AppHdrType buildHeaderInfo()
    {

        EVNMN.AppHdrType appHdr = new EVNMN.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        EVNMN.PairsType nsFrom = new EVNMN.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        EVNMN.PairsType nsTo = new EVNMN.PairsType();
        nsTo.Id = "ESB_SOA";
        nsTo.Name = "ESB_SOA";

        EVNMN.PairsType[] listOfNsTo = new EVNMN.PairsType[1];
        listOfNsTo[0] = nsTo;

        EVNMN.PairsType BizSvc = new EVNMN.PairsType();
        BizSvc.Id = "EVNMN";
        BizSvc.Name = "EVNMN";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        return appHdr;

    }
}