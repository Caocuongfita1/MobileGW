using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for DAWACOIntegration
/// </summary>
public class DAWACOIntegration
{
    public DAWACOIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static DAWACOBillPayments.InfoCustCheckRespType GetCustomerInfo(string custId, string sohopdong, string customerCd, string SoCMTND, string PhoneNumber, string DiaChiKH)
    {
        DAWACOBillPayments.InfoCustCheckRespType res = null;
        try
        {
            DAWACOBillPayments.AppHdrType appHdr = new DAWACOBillPayments.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            DAWACOBillPayments.PairsType nsFrom = new DAWACOBillPayments.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            DAWACOBillPayments.PairsType nsTo = new DAWACOBillPayments.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            DAWACOBillPayments.PairsType[] listOfNsTo = new DAWACOBillPayments.PairsType[1];
            listOfNsTo[0] = nsTo;

            DAWACOBillPayments.PairsType BizSvc = new DAWACOBillPayments.PairsType();
            BizSvc.Id = "DAWACOBillPayments-InfoCustCheckRespType";
            BizSvc.Name = "DAWACOBillPayments-InfoCustCheckRespType";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            DAWACOBillPayments.InfoCustCheckReqType msgReq = new DAWACOBillPayments.InfoCustCheckReqType();
            msgReq.AppHdr = appHdr;
            msgReq.custId = sohopdong;
            msgReq.agentCode = "11";

            DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient ptc = new DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient();

            Funcs.WriteLog("CIF_NO: " + custId + "|req GetCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            try
            {

                res = ptc.InfoCustCheck(msgReq);
                if (res != null)
                {
                    try
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|res GetCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    }
                    catch (Exception subEx)
                    {
                        Funcs.WriteLog("CIF_NO: " + custId + "|EXCEPTION GetCustomerInfo: " + subEx.Message.ToString());
                    }
                }
                ptc.Close();
            }
            catch (Exception e)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|EXCEPTION GetCustomerInfo: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|EXCEPTION GetCustomerInfo: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }

    //Lấy thông tin nợ của khách hàng với mã khách hàng
    public static DAWACOBillPayments.DebtCheckRespType GetBillInfoByKHID(string custId, string customerCd)
    {
        DAWACOBillPayments.DebtCheckRespType res = null;
        try
        {
            DAWACOBillPayments.AppHdrType appHdr = new DAWACOBillPayments.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            DAWACOBillPayments.PairsType nsFrom = new DAWACOBillPayments.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            DAWACOBillPayments.PairsType nsTo = new DAWACOBillPayments.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            DAWACOBillPayments.PairsType[] listOfNsTo = new DAWACOBillPayments.PairsType[1];
            listOfNsTo[0] = nsTo;

            DAWACOBillPayments.PairsType BizSvc = new DAWACOBillPayments.PairsType();
            BizSvc.Id = "DAWACOBillPayments-DebtCheckRespType";
            BizSvc.Name = "DAWACOBillPayments-DebtCheckRespType";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            DAWACOBillPayments.DebtCheckReqType msgReq = new DAWACOBillPayments.DebtCheckReqType();
            msgReq.AppHdr = appHdr;
            msgReq.custId = customerCd;
            msgReq.agentCode = "11";

            Funcs.WriteLog("CIF_NO: " + custId + "|req GetBillInfoByKHID: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient ptc = new DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient();

            try
            {
                res = ptc.DebtCheck(msgReq);
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
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
    //Gạch nợ
    public static DAWACOBillPayments.DebtPaymentRespType DoPayment(string custId, string customerCd, string amount, string transId)
    {
        DAWACOBillPayments.DebtPaymentRespType res = null;
        try
        {
            DAWACOBillPayments.AppHdrType appHdr = new DAWACOBillPayments.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            DAWACOBillPayments.PairsType nsFrom = new DAWACOBillPayments.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            DAWACOBillPayments.PairsType nsTo = new DAWACOBillPayments.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            DAWACOBillPayments.PairsType[] listOfNsTo = new DAWACOBillPayments.PairsType[1];
            listOfNsTo[0] = nsTo;

            DAWACOBillPayments.PairsType BizSvc = new DAWACOBillPayments.PairsType();
            BizSvc.Id = "DAWACOBillPayments-DebtPaymentResponse";
            BizSvc.Name = "DAWACOBillPayments-DebtPaymentResponse";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            DAWACOBillPayments.DebtPaymentReqType msgReq = new DAWACOBillPayments.DebtPaymentReqType();
            msgReq.AppHdr = appHdr;
            msgReq.custId = customerCd;
            msgReq.transId = transId;
            msgReq.agentCode = "11";
            msgReq.payMoney = amount;
            msgReq.payF = "1";
            msgReq.paidDate = DateTime.Now.ToString("dd/MM/yyyy");

            Funcs.WriteLog("CIF_NO: " + custId + "|req DoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient ptc = new DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient();
            try
            {
                res = ptc.DebtPayment(msgReq);
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
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }

    //Xóa gạch nợ
    public static DAWACOBillPayments.DebtCancelRespType UNDoPayment(string custId, string customerCd, string amount, string transId)
    {
        DAWACOBillPayments.DebtCancelRespType res = null;

        try
        {
            DAWACOBillPayments.AppHdrType appHdr = new DAWACOBillPayments.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            DAWACOBillPayments.PairsType nsFrom = new DAWACOBillPayments.PairsType();
            nsFrom.Id = "MOB";
            nsFrom.Name = "MOB";

            DAWACOBillPayments.PairsType nsTo = new DAWACOBillPayments.PairsType();
            nsTo.Id = "ESB_SOA";
            nsTo.Name = "ESB_SOA";

            DAWACOBillPayments.PairsType[] listOfNsTo = new DAWACOBillPayments.PairsType[1];
            listOfNsTo[0] = nsTo;

            DAWACOBillPayments.PairsType BizSvc = new DAWACOBillPayments.PairsType();
            BizSvc.Id = "DAWACOBillPayments-DebtCancelRespType";
            BizSvc.Name = "DAWACOBillPayments-DebtCancelRespType";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Guid.NewGuid().ToString();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

            //Body
            DAWACOBillPayments.DebtCancelReqType msgReq = new DAWACOBillPayments.DebtCancelReqType();
            msgReq.AppHdr = appHdr;
            msgReq.custId = customerCd;
            msgReq.payMoney = Int32.Parse(amount);
            msgReq.agentCode = "11";
            msgReq.paidDate = DateTime.Now.ToString();
            msgReq.transId = transId;

            Funcs.WriteLog("CIF_NO: " + custId + "|req UNDoPayment: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient ptc = new DAWACOBillPayments.DAWACOBillPaymentsPortTypeClient();
            try
            {
                res = ptc.DebtCancel(msgReq);
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
                Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|Exception when write log response: " + e.Message.ToString());
            throw e;
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
        }

        return res;
    }
}