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
public class QUAWACOIntegration
{
    public QUAWACOIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static QUAWACOBill.GetCustomerInfoRespType GetCustomerInfo(string custId,string customerCd, string cusName, string PhoneNumber,
            string SoCMTND, string DiaChiKH)
    {
        QUAWACOBill.GetCustomerInfoRespType res = null;
        try
        {
            QUAWACOBill.AppHdrType appHdr = new QUAWACOBill.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            QUAWACOBill.PairsType nsFrom = new QUAWACOBill.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            QUAWACOBill.PairsType nsTo = new QUAWACOBill.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            QUAWACOBill.PairsType[] listOfNsTo = new QUAWACOBill.PairsType[1];
            listOfNsTo[0] = nsTo;

            QUAWACOBill.PairsType BizSvc = new QUAWACOBill.PairsType();
            BizSvc.Id = "QUAWACO";
            BizSvc.Name = "QUAWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            QUAWACOBill.GetCustomerInfoReqType msgReq = new QUAWACOBill.GetCustomerInfoReqType();
            msgReq.AppHdr = appHdr;
            msgReq.idkh = customerCd;
            msgReq.tenkh = cusName;
            msgReq.cmnd = SoCMTND;
            msgReq.sdt = PhoneNumber;
            msgReq.diachi = DiaChiKH;
            //"161007092552"

            Funcs.WriteLog("CIF_NO: "+ custId + " Hàm GetCustomerInfo: MsgId" + msgReq.AppHdr.MsgId + ", Đầu vào: CustomerCode:" + msgReq.idkh + ",diachi:" + msgReq.diachi + ",tenkh:" + msgReq.tenkh);

            QUAWACOBill.QuaWaCoBillPmtPortTypeClient ptc = new QUAWACOBill.QuaWaCoBillPmtPortTypeClient();
            try
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|req GetCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));
                res = ptc.GetCustomerInfo(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|res CustomeInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + subEx.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                object a = e.Message;
                Funcs.WriteLog("CIF_NO: " + custId + "|Lỗi Hàm CustomerInfoReqType: " + a);
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception: " + e.ToString());
            throw e;
        }
        finally
        {
            // Funcs.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }

    // Lấy thông tin chi tiết về điện lực.
    public static QUAWACOBill.GetBillInfoByKHIDRespType GetBillInfoByKHID(string custId,string customerCd)
    {
        QUAWACOBill.GetBillInfoByKHIDRespType res = null;
        try
        {
            QUAWACOBill.AppHdrType appHdr = new QUAWACOBill.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            QUAWACOBill.PairsType nsFrom = new QUAWACOBill.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            QUAWACOBill.PairsType nsTo = new QUAWACOBill.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            QUAWACOBill.PairsType[] listOfNsTo = new QUAWACOBill.PairsType[1];
            listOfNsTo[0] = nsTo;

            QUAWACOBill.PairsType BizSvc = new QUAWACOBill.PairsType();
            BizSvc.Id = "QUAWACO";
            BizSvc.Name = "QUAWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            QUAWACOBill.GetBillInfoByKHIDReqType msgReq = new QUAWACOBill.GetBillInfoByKHIDReqType();
            msgReq.AppHdr = appHdr;
            msgReq.idkh = customerCd;
            Funcs.WriteLog("CIF_NO:"+ custId +"| Hàm GetBillInfoByKHID: MsgId" + msgReq.AppHdr.MsgId + ", Đầu vào: CustomerCode:" + customerCd);
            QUAWACOBill.QuaWaCoBillPmtPortTypeClient ptc = new QUAWACOBill.QuaWaCoBillPmtPortTypeClient();
            try
            {
                Funcs.WriteLog("CIF_NO:" + custId + "|req GetBillInfoByKHID: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));
                res = ptc.GetBillInfoByKHID(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO:" + custId + "|res GetBillInfoByKHID: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO:" + custId + "|Exception when write log response: " + subEx.ToString());
                        return null;
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                //Funcs.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
                object a = e.Message;
                Funcs.WriteLog("CIF_NO:" + custId + "|Lỗi Hàm GetBillInfoByKHID: " + a);
                return null;
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO:" + custId + "|Exception: " + e.ToString());
            return null;
        }
        finally
        {
            // Funcs.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }

    public static QUAWACOBill.DoPaymentRespType DoPayment(string custId,string customerCd, string tranid, string transType, string amount, string thang, string nam)
    {
        QUAWACOBill.DoPaymentRespType res = null;
        try
        {
            QUAWACOBill.AppHdrType appHdr = new QUAWACOBill.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            QUAWACOBill.PairsType nsFrom = new QUAWACOBill.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            QUAWACOBill.PairsType nsTo = new QUAWACOBill.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            QUAWACOBill.PairsType[] listOfNsTo = new QUAWACOBill.PairsType[1];
            listOfNsTo[0] = nsTo;

            QUAWACOBill.PairsType BizSvc = new QUAWACOBill.PairsType();
            BizSvc.Id = "QUAWACO";
            BizSvc.Name = "QUAWACO";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            QUAWACOBill.DoPaymentReqType msgReq = new QUAWACOBill.DoPaymentReqType();
            msgReq.AppHdr = appHdr;
            msgReq.idkh = customerCd;
            msgReq.thang = thang;
            msgReq.nam = nam;
            msgReq.amount = amount;
            msgReq.kenhgiaodichid = transType;
            msgReq.tranid = tranid;
            //"161007092552"
            Funcs.WriteLog("CIF_NO:" + custId + "| Hàm DoPayment: MsgId" + msgReq.AppHdr.MsgId + ", Đầu vào: CustomerCode:" + msgReq.idkh + ",TranID:" + msgReq.tranid);
            QUAWACOBill.QuaWaCoBillPmtPortTypeClient ptc = new QUAWACOBill.QuaWaCoBillPmtPortTypeClient();
            try
            {
                Funcs.WriteLog("CIF_NO:" + custId + "|req DoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));
                res = ptc.DoPayment(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO:" + custId + "|res DoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO:" + custId + "|Exception when write log response: " + subEx.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                //Funcs.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
                object a = e.Message;
                Funcs.WriteLog("CIF_NO:" + custId + "|Lỗi Hàm DoPayment: " + e.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO:" + custId + "|Exception: " + e.ToString());
            throw e;
        }
        finally
        {
            // Funcs.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
}