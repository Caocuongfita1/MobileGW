using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using HABECO;

/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class HabecoIntegration
{
    public HabecoIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public HabecoBill.GetCustomerRespType GetCustomer(string cif_no, string customerId)
    {
        //string rest = Config.RES_HABECO_GETCUSTOMER;

        HabecoBill.AppHdrType appHdr = new HabecoBill.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        HabecoBill.PairsType nsFrom = new HabecoBill.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        HabecoBill.PairsType nsTo = new HabecoBill.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        HabecoBill.PairsType[] listOfNsTo = new HabecoBill.PairsType[1];
        listOfNsTo[0] = nsTo;

        HabecoBill.PairsType BizSvc = new HabecoBill.PairsType();
        BizSvc.Id = "HabecoBill";
        BizSvc.Name = "HabecoBill";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        HabecoBill.GetCustomerReqType msgReq = new HabecoBill.GetCustomerReqType();
        msgReq.AppHdr = appHdr;
        msgReq.custCode = customerId;

        HabecoBill.GetCustomerRespType res = null;
        //portypeClient
        try
        {
            HabecoBill.PortTypeClient ptc = new HabecoBill.PortTypeClient();
            Funcs.WriteLog("custid:" + cif_no + "|HabecoBill.GetCustomer|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.GetCustOperation(msgReq);

            Funcs.WriteLog("custid:" + cif_no + "|HabecoBill.GetCustomer|RES = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("HabecoBill.GetCustomer EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    public string AddCreditAdvice(string cif_no, string customerId, string tranId, string tranDes, string tranAmount)
    {
        string rest = Config.ERR_MSG_GENERAL;

        HabecoBill.AppHdrType appHdr = new HabecoBill.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        HabecoBill.PairsType nsFrom = new HabecoBill.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        HabecoBill.PairsType nsTo = new HabecoBill.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        HabecoBill.PairsType[] listOfNsTo = new HabecoBill.PairsType[1];
        listOfNsTo[0] = nsTo;

        HabecoBill.PairsType BizSvc = new HabecoBill.PairsType();
        BizSvc.Id = "HabecoBill";
        BizSvc.Name = "HabecoBill";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //Body
        HabecoBill.AddCreditAdviceReqMsgType msgReq = new HabecoBill.AddCreditAdviceReqMsgType();
        msgReq.AppHdr = appHdr;
        msgReq.custCode = customerId;
        msgReq.transAmount = tranAmount;
        msgReq.transDate = DateTime.Now.ToString();
        msgReq.transDes = tranDes;
        msgReq.transId = tranId;

        HabecoBill.AddCreditAdviceRespMsgType res = null;
        //portypeClient
        try
        {
            Funcs.WriteLog("custid:" + cif_no + "|HabecoBill.AddCreditOperation|REQ= " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            HabecoBill.PortTypeClient ptc = new HabecoBill.PortTypeClient();
            res = ptc.AddCreditOperation(msgReq);

            Funcs.WriteLog("custid:" + cif_no + "|HabecoBill.AddCreditOperation|RES= " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();


            if (res != null && res.ErrorCode.Equals("00"))
            {
                return Config.ERR_CODE_DONE;

            }
            else
            {
                return Config.ERR_CODE_GENERAL;
            }

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("HabecoBill.AddCreditOperation EXCEPTION FROM ESB: " + ex.ToString());
            return Config.ERR_CODE_GENERAL;
        }
    }

    public static  HabecoSAPGetCustomerRespType HabecoSAPGetCustomer (string custID)
    {
        HabecoSAPGetCustomerRespType res = null;
        try
        {
            HABECO.AppHdrType appHdr = buildHeaderInfo();

            HabecoSAPGetCustomerReqType msgReq = new HabecoSAPGetCustomerReqType();
            msgReq.AppHdr = appHdr;
            msgReq.custID = custID;

            Funcs.WriteLog("|req QueryCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            HABECO.PortTypeClient ptc = new HABECO.PortTypeClient();
            try
            {
                res = ptc.HabecoSAPGetCustomer(msgReq);
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
                Funcs.WriteLog("CIF_NO: " + custID + "|ERROR PayBillInfo: " + "L?i Hàm PayBill: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custID + "|ERROR PayBillInfo: " + "Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
        }

        return res;
    }

    public static HabecoSAPBankThuRespType HabecoSAPITInputBankThu(string custID, string tenKH, string amount)
    {
        HabecoSAPBankThuRespType res = null;
        try
        {
            HABECO.AppHdrType appHdr = buildHeaderInfo();
            DateTime TransDt = DateTime.Now;

            HabecoSAPITInputBankThuType input = new HabecoSAPITInputBankThuType();
            input.bukrs = "HABECOHNB";
            input.budat = TransDt.ToString("yyyy-MM-ddTHH:MM:ss.FFFZ");
            input.bldat = TransDt.ToString("yyyy-MM-ddTHH:MM:ss.FFFZ");
            input.ztrnu = Guid.NewGuid().ToString();
            input.bankn = Funcs.getConfigVal("ACCT_SUSPEND_HABECO");
            input.bpext = custID;
            input.zname = tenKH;
            input.wrbtr = amount;
            input.sgtxt = "THANHTOAN";

            HabecoSAPBankThuReqType msgReq = new HabecoSAPBankThuReqType();
            msgReq.AppHdr = appHdr;
            msgReq.ITInputs = new HabecoSAPITInputBankThuType[1];
            msgReq.ITInputs[0] = input;

            Funcs.WriteLog("|req QueryCustomerInfo: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));
            HABECO.PortTypeClient ptc = new HABECO.PortTypeClient();
            try
            {
                res = ptc.HabecoSAPBankThu(msgReq);
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
                Funcs.WriteLog("CIF_NO: " + custID + "|ERROR PayBillInfo: " + "L?i Hàm PayBill: " + e.Message.ToString());
            }

        }
        catch (Exception e)
        {
            Funcs.WriteLog("CIF_NO: " + custID + "|ERROR PayBillInfo: " + "Exception: " + e.Message.ToString());
            throw e;
        }
        finally
        {
        }

        return res;
    }

    public static HABECO.AppHdrType buildHeaderInfo()
    {

        HABECO.AppHdrType appHdr = new HABECO.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        HABECO.PairsType nsFrom = new HABECO.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        HABECO.PairsType nsTo = new HABECO.PairsType();
        nsTo.Id = "ESB_SOA";
        nsTo.Name = "ESB_SOA";

        HABECO.PairsType[] listOfNsTo = new HABECO.PairsType[1];
        listOfNsTo[0] = nsTo;

        HABECO.PairsType BizSvc = new HABECO.PairsType();
        BizSvc.Id = "HABECO";
        BizSvc.Name = "HABECO";

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