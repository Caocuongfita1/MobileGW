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
public class SOWACOIntegration
{
    public SOWACOIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //-------------------SOWACO---------------------------
    //Lấy thông tin khách hàng
    public static SOWACO.GetCustomerInfoRespType GetCustomerInfo(string custId,string sohopdong, string customerCd, string SoCMTND, string PhoneNumber, string DiaChiKH)
    {
        SOWACO.GetCustomerInfoRespType res = null;
        try
        {
            SOWACO.AppHdrType appHdr = new SOWACO.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            SOWACO.PairsType nsFrom = new SOWACO.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            SOWACO.PairsType nsTo = new SOWACO.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            SOWACO.PairsType[] listOfNsTo = new SOWACO.PairsType[1];
            listOfNsTo[0] = nsTo;

            SOWACO.PairsType BizSvc = new SOWACO.PairsType();
            BizSvc.Id = "SOWACO";
            BizSvc.Name = "SOWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            SOWACO.GetCustomerInfoReqType msgReq = new SOWACO.GetCustomerInfoReqType();
            msgReq.AppHdr = appHdr;
            msgReq.sohopdong = sohopdong;
            msgReq.maKhachHang = customerCd;

            SOWACO.SonLaWacoBillPmtPortTypeClient ptc = new SOWACO.SonLaWacoBillPmtPortTypeClient();

            Funcs.WriteLog("CIF_NO: " + custId + "|req GetCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            try
            {
                
                res = ptc.GetCustomerInfo(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|res GetCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
                object a = e.Message;
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + a.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }

    //Lấy thông tin nợ của khách hàng với mã khách hàng
    public static SOWACO.GetBillInfoRespType GetBillInfoByKHID(string custId,string customerCd)
    {
        SOWACO.GetBillInfoRespType res = null;
        try
        {
            SOWACO.AppHdrType appHdr = new SOWACO.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            SOWACO.PairsType nsFrom = new SOWACO.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            SOWACO.PairsType nsTo = new SOWACO.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            SOWACO.PairsType[] listOfNsTo = new SOWACO.PairsType[1];
            listOfNsTo[0] = nsTo;

            SOWACO.PairsType BizSvc = new SOWACO.PairsType();
            BizSvc.Id = "SOWACO";
            BizSvc.Name = "SOWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            SOWACO.GetBillInfoReqType msgReq = new SOWACO.GetBillInfoReqType();
            msgReq.AppHdr = appHdr;
            msgReq.maKhachHang = customerCd;

            Funcs.WriteLog("CIF_NO: " + custId + "|req GetBillInfoByKHID: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            SOWACO.SonLaWacoBillPmtPortTypeClient ptc = new SOWACO.SonLaWacoBillPmtPortTypeClient();
            try
            {
                res = ptc.GetBillInfo(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|res GetBillInfoByKHID: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
                object a = e.Message;
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + a.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    //Gạch nợ
    public static SOWACO.DoPaymentRespType DoPayment(string custId,string idHD)
    {
        SOWACO.DoPaymentRespType res = null;
        try
        {
            SOWACO.AppHdrType appHdr = new SOWACO.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            SOWACO.PairsType nsFrom = new SOWACO.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            SOWACO.PairsType nsTo = new SOWACO.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            SOWACO.PairsType[] listOfNsTo = new SOWACO.PairsType[1];
            listOfNsTo[0] = nsTo;

            SOWACO.PairsType BizSvc = new SOWACO.PairsType();
            BizSvc.Id = "SOWACO";
            BizSvc.Name = "SOWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            SOWACO.DoPaymentReqType msgReq = new SOWACO.DoPaymentReqType();
            msgReq.AppHdr = appHdr;
            msgReq.idHoaDon = idHD;

            Funcs.WriteLog("CIF_NO: " + custId + "|req DoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            SOWACO.SonLaWacoBillPmtPortTypeClient ptc = new SOWACO.SonLaWacoBillPmtPortTypeClient();
            try
            {
                res = ptc.DoPayment(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|res DoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
                object a = e.Message;
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + a.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    //Check payment
    public static SOWACO.CheckPaymentRespType CheckPayment(string custId, string IDHD)
    {
        SOWACO.CheckPaymentRespType res = null;
        try
        {
            SOWACO.AppHdrType appHdr = new SOWACO.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            SOWACO.PairsType nsFrom = new SOWACO.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            SOWACO.PairsType nsTo = new SOWACO.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            SOWACO.PairsType[] listOfNsTo = new SOWACO.PairsType[1];
            listOfNsTo[0] = nsTo;

            SOWACO.PairsType BizSvc = new SOWACO.PairsType();
            BizSvc.Id = "SOWACO";
            BizSvc.Name = "SOWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            SOWACO.CheckPaymentReqType msgReq = new SOWACO.CheckPaymentReqType();
            msgReq.AppHdr = appHdr;
            msgReq.idHoaDon = IDHD;

            Funcs.WriteLog("CIF_NO: " + custId + "|req CheckPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            SOWACO.SonLaWacoBillPmtPortTypeClient ptc = new SOWACO.SonLaWacoBillPmtPortTypeClient();
            try
            {
                res = ptc.CheckPayment(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|res CheckPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
                object a = e.Message;
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + a.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    //Xóa gạch nợ
    public static SOWACO.CancelPaymentRespType UNDoPayment(string custId,string idHD)
    {
        SOWACO.CancelPaymentRespType res = null;
        try
        {
            SOWACO.AppHdrType appHdr = new SOWACO.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            SOWACO.PairsType nsFrom = new SOWACO.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            SOWACO.PairsType nsTo = new SOWACO.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            SOWACO.PairsType[] listOfNsTo = new SOWACO.PairsType[1];
            listOfNsTo[0] = nsTo;

            SOWACO.PairsType BizSvc = new SOWACO.PairsType();
            BizSvc.Id = "SOWACO";
            BizSvc.Name = "SOWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            SOWACO.CancelPaymentReqType msgReq = new SOWACO.CancelPaymentReqType();
            msgReq.AppHdr = appHdr;
            msgReq.idHoaDon = idHD;
           
            Funcs.WriteLog("CIF_NO: " + custId + "|req UNDoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            SOWACO.SonLaWacoBillPmtPortTypeClient ptc = new SOWACO.SonLaWacoBillPmtPortTypeClient();
            try
            {
                res = ptc.CancelPayment(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|res UNDoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
                object a = e.Message;
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + a.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
}