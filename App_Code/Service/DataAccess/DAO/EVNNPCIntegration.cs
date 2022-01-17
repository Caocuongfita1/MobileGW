using EvnNPCBillPayment;
using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for DAWACOIntegration
/// </summary>
public class EVNNPCIntegration
{
    public EVNNPCIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //Lấy thông tin khách hàng
    #region Lấy thông tin khách hàng
    public static QueryCustomerAddressRespType QueryCustomerAddress(string custID, string customerCd, string auditNumber, string transType, string PhoneNumber,
        string SoCMTND, string TenKH, string DiaChiKH, string MaDL)
    {
        QueryCustomerAddressRespType res = null;
        try
        {
            EvnNPCBillPayment.AppHdrType appHdr = new EvnNPCBillPayment.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "2.0";

            EvnNPCBillPayment.PairsType nsFrom = new EvnNPCBillPayment.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            EvnNPCBillPayment.PairsType nsTo = new EvnNPCBillPayment.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            EvnNPCBillPayment.PairsType[] listOfNsTo = new EvnNPCBillPayment.PairsType[1];
            listOfNsTo[0] = nsTo;

            EvnNPCBillPayment.PairsType BizSvc = new EvnNPCBillPayment.PairsType();
            BizSvc.Id = "EVNNPC";
            BizSvc.Name = "EVNNPC";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            QueryCustomerAddressReqType msgReq = new QueryCustomerAddressReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustomersID = customerCd;
            msgReq.AuditNumber = auditNumber;
            msgReq.PhoneNumber = PhoneNumber;
            msgReq.SoCMTND = SoCMTND;
            msgReq.TransType = transType;
            msgReq.TenKH = TenKH;
            msgReq.DiaChiKH = DiaChiKH;
            msgReq.MaDL = MaDL;

            Funcs.WriteLog("CIF_NO: " + custID + "|req QueryCustomerAddress: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            EvnNPCBillPayment.PortTypeClient ptc = new EvnNPCBillPayment.PortTypeClient();
            try
            {
                res = ptc.QueryCustomerAddress(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|res QueryCustomerAddress: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|ERROR QueryCustomerAddress: " + "| Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                Funcs.WriteLog("CIF_NO: " + custID + "|ERROR QueryCustomerAddress: " + "Lỗi Hàm CustomerInfoReqType: " + e.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custID + "|ERROR QueryCustomerAddress: " + "Exception: " + e.Message.ToString());
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
    #region Lấy thông tin nợ
    public static QueryBillInfoRespType QueryBillInfo(string custID, string customerCd, string auditNumber, string transType)
    {
        QueryBillInfoRespType res = null;
        try
        {
            EvnNPCBillPayment.AppHdrType appHdr = new EvnNPCBillPayment.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "2.0";

            EvnNPCBillPayment.PairsType nsFrom = new EvnNPCBillPayment.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            EvnNPCBillPayment.PairsType nsTo = new EvnNPCBillPayment.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            EvnNPCBillPayment.PairsType[] listOfNsTo = new EvnNPCBillPayment.PairsType[1];
            listOfNsTo[0] = nsTo;

            EvnNPCBillPayment.PairsType BizSvc = new EvnNPCBillPayment.PairsType();
            BizSvc.Id = "EVNNPC";
            BizSvc.Name = "EVNNPC";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            QueryBillInfoReqType msgReq = new QueryBillInfoReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustomersID = customerCd;
            msgReq.AuditNumber = auditNumber;
            msgReq.TransType = transType;

            Funcs.WriteLog("CIF_NO: " + custID + "|req QueryBillInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            EvnNPCBillPayment.PortTypeClient ptc = new EvnNPCBillPayment.PortTypeClient();
            try
            {
                res = ptc.QueryBillInfo(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|res QueryBillInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|ERROR QueryBillInfo: " + "| Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                Funcs.WriteLog("CIF_NO: " + custID + "|ERROR QueryBillInfo: " + "Lỗi Hàm QueryBillInfo: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custID + "|ERROR QueryBillInfo: " + "Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    #endregion

    //Service xác nhận khách hàng thanh toán 1 hóa đơn nợ
    #region Service xác nhận khách hàng thanh toán 1 hóa đơn nợ
    public static TransConfirmV2RespType TransConfirmV2(string custID, string customerCd, string auditNumber, string transType, string IdHD, string MaDL, string ServiceType, string Amount, string Point_of_service, string BillPrintStatus)
    {
        TransConfirmV2RespType res = null;
        try
        {
            EvnNPCBillPayment.AppHdrType appHdr = new EvnNPCBillPayment.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "2.0";

            EvnNPCBillPayment.PairsType nsFrom = new EvnNPCBillPayment.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            EvnNPCBillPayment.PairsType nsTo = new EvnNPCBillPayment.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            EvnNPCBillPayment.PairsType[] listOfNsTo = new EvnNPCBillPayment.PairsType[1];
            listOfNsTo[0] = nsTo;

            EvnNPCBillPayment.PairsType BizSvc = new EvnNPCBillPayment.PairsType();
            BizSvc.Id = "EVNNPC";
            BizSvc.Name = "EVNNPC";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            TransConfirmV2ReqType msgReq = new TransConfirmV2ReqType();
            msgReq.AppHdr = appHdr;
            msgReq.CustomersID = customerCd;
            msgReq.PaymentAmount = Amount;
            msgReq.AuditNumber = auditNumber;
            msgReq.TransDate = TransDt.ToString("dd/MM/yyyy HH:mm:ss");
            msgReq.TransType = transType;
            msgReq.IdHD = IdHD;
            msgReq.Point_of_service = Point_of_service;
            msgReq.MaDL = MaDL;
            msgReq.BillPrintStatus = BillPrintStatus;
            msgReq.LoaiDichVu = ServiceType;

            Funcs.WriteLog("CIF_NO: " + custID + "|req TransConfirmV2: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            EvnNPCBillPayment.PortTypeClient ptc = new EvnNPCBillPayment.PortTypeClient();
            try
            {
                res = ptc.TransConfirmV2(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|res TransConfirmV2: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custID + "|Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                Funcs.WriteLog("CIF_NO: " + custID + "|Lỗi Hàm TransConfirmV2: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custID + "|Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    #endregion
}