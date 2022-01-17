using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for DLVNIntegration
/// </summary>
public class DLVNIntegration
{
    public DLVNIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static DLVNSOA.PolicyInquiryResType inquiryPolicy(string username, string password, string policyNumber)
    // public static string doPayment()
    {
        DLVNSOA.AppHdrType appHdr = new DLVNSOA.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        DLVNSOA.PairsType nsFrom = new DLVNSOA.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        DLVNSOA.PairsType nsTo = new DLVNSOA.PairsType();
        nsTo.Id = "ESB_DLVN";
        nsTo.Name = "ESB_DLVN";

        DLVNSOA.PairsType[] listOfNsTo = new DLVNSOA.PairsType[1];
        listOfNsTo[0] = nsTo;

        DLVNSOA.PairsType BizSvc = new DLVNSOA.PairsType();
        BizSvc.Id = "DLVN";
        BizSvc.Name = "DLVN";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        DLVNSOA.PolicyInquiryReqType msgReq = new DLVNSOA.PolicyInquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.Username = username;
        msgReq.Password = password;
        msgReq.PolicyNumber = policyNumber;
        
        Funcs.WriteLog("Hàm IDInquiry: MsgId:" + msgReq.AppHdr.MsgId + ", |Đầu vào IDInquiry: username:" + msgReq.Username + "|password:" + msgReq.Password + "|PolicyNumber:" + msgReq.PolicyNumber);
        DLVNSOA.PolicyInquiryResType res = null;
        try
        {

            DLVNSOA.PortTypeClient ptc = new DLVNSOA.PortTypeClient();
            //Helper.WriteLog(l4NC, msgReq, "Start function: ");

            res = ptc.PolicyInquiry(msgReq);
            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("Res PolicyInquiry: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
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
            Funcs.WriteLog("Exception: " + e.Message);
            object a = e.Message;
            Funcs.WriteLog("Lỗi Hàm PolicyInquiry: " + a);
        }
        finally
        {
            Funcs.WriteLog(" response Inquiry: " + res);
        }

        return res;
    }

    public static DLVNSOA.IDInquiryResType inquiryID(string username, string password, string idNumber)//bo
                                                                                               // public static string doPayment()
    {
        DLVNSOA.AppHdrType appHdr = new DLVNSOA.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        DLVNSOA.PairsType nsFrom = new DLVNSOA.PairsType();
        nsFrom.Id = "ESB_SHB";
        nsFrom.Name = "ESB_SHB";

        DLVNSOA.PairsType nsTo = new DLVNSOA.PairsType();
        nsTo.Id = "ESB_DLVN";
        nsTo.Name = "ESB_DLVN";

        DLVNSOA.PairsType[] listOfNsTo = new DLVNSOA.PairsType[1];
        listOfNsTo[0] = nsTo;

        DLVNSOA.PairsType BizSvc = new DLVNSOA.PairsType();
        BizSvc.Id = "DLVN";
        BizSvc.Name = "DLVN";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        DLVNSOA.IDInquiryReqType msgReq = new DLVNSOA.IDInquiryReqType();
        msgReq.AppHdr = appHdr;
        msgReq.Username = username;
        msgReq.Password = password;
        msgReq.IDNumber = idNumber;
        //Helper.WriteLog(l4NC, msgReq, " Hàm IDInquiry: MsgId" + msgReq.AppHdr.MsgId + ", Đầu vào IDInquiry: username:" + msgReq.Username + ",password:" + msgReq.Password + ",IDNumber:" + msgReq.IDNumber);
        Funcs.WriteLog(" Hàm IDInquiry: MsgId:" + msgReq.AppHdr.MsgId + "|Đầu vào IDInquiry: username:" + msgReq.Username + "|password:" + msgReq.Password + "|IDNumber:" + msgReq.IDNumber);
        DLVNSOA.IDInquiryResType res = null;
        try
        {

            DLVNSOA.PortTypeClient ptc = new DLVNSOA.PortTypeClient();

            //Helper.WriteLog(l4NC, msgReq, "Vào hàm IDInquiry: ");
            res = ptc.IDInquiry(msgReq);
            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("Res IDInquiry: " + res);
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("Exception when write log response: " + subEx.Message);
                }

            }
            ptc.Close();


        }
        catch (Exception e)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            object a = e.Message;
            Funcs.WriteLog("Lỗi hàm IDInquiry: " + a);
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
            Funcs.WriteLog(" response Inquiry:" + res);
        }

        return res;
    }

    public static DLVNSOA.PaymentBNKResType paymentBNK(string username, string password, string policyNumber, string dlvnRef, string PaymentAmount, string prmAmount, string paymentDate, string freqPremium,
         string premType, string payerName, string payerAddress, string payerPhone, string comment, string refNumber, string channelID)
    // public static string doPayment()
    {
        DLVNSOA.AppHdrType appHdr = new DLVNSOA.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        DLVNSOA.PairsType nsFrom = new DLVNSOA.PairsType();
        nsFrom.Id = "ESB_SHB";
        nsFrom.Name = "ESB_SHB";

        DLVNSOA.PairsType nsTo = new DLVNSOA.PairsType();
        nsTo.Id = "ESB_DLVN";
        nsTo.Name = "ESB_DLVN";

        DLVNSOA.PairsType[] listOfNsTo = new DLVNSOA.PairsType[1];
        listOfNsTo[0] = nsTo;

        DLVNSOA.PairsType BizSvc = new DLVNSOA.PairsType();
        BizSvc.Id = "DLVN";
        BizSvc.Name = "DLVN";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        DLVNSOA.PaymentBNKReqType msgReq = new DLVNSOA.PaymentBNKReqType();
        msgReq.AppHdr = appHdr;
        msgReq.Username = username;
        msgReq.Password = password;
        msgReq.PolicyNumber = policyNumber;
        msgReq.DLVNRef = dlvnRef;
        msgReq.PaymentAmount = PaymentAmount;
        msgReq.PrmAmout = prmAmount;
        msgReq.PaymentDate = paymentDate;
        msgReq.FreqPremium = freqPremium;
        msgReq.Prem_Type = premType;
        msgReq.PayerName = payerName;
        msgReq.PayerAddress = payerAddress;
        msgReq.PayerPhone = payerPhone;
        msgReq.Comment = comment;
        msgReq.RefNumber = refNumber;
        msgReq.ChannelID = channelID;

        Funcs.WriteLog(" Hàm PaymentBNK: MsgId: " + msgReq.AppHdr.MsgId + "|Đầu vào Hàm PaymentBNK:" + msgReq.AppHdr.MsgId + "|username:" + msgReq.Username + "|password:" + msgReq.Password + "|PolicyNumber:" + msgReq.PolicyNumber + "|DLVNRef:" + msgReq.DLVNRef +
            "|PaymentAmount:" + msgReq.PaymentAmount + "|prmAmount:" + msgReq.PrmAmout
              + "|PaymentDate:" + msgReq.PaymentDate + "|freqPremium:" + msgReq.FreqPremium
                + "|premType:" + msgReq.Prem_Type + "|PayerName:" + msgReq.PayerName
                  + "|PayerAddress:" + msgReq.PayerAddress + "|PayerPhone:" + msgReq.PayerPhone
                      + "|Comment:" + msgReq.Comment + "|RefNumber:" + msgReq.RefNumber + "|ChannelID:" + msgReq.ChannelID);
        DLVNSOA.PaymentBNKResType res = null;
        try
        {

            DLVNSOA.PortTypeClient ptc = new DLVNSOA.PortTypeClient();

            Funcs.WriteLog("Vào hàm PaymentBNK: " + msgReq);
            Funcs.WriteLog("thong tin req:" + new JavaScriptSerializer().Serialize(msgReq));

            res = ptc.PaymentBNK(msgReq);
            if (res != null)
            {
                try
                {

                    Funcs.WriteLog("Res PaymentBNK: " + res);
                }
                catch (Exception subEx)
                {
                    Funcs.WriteLog("Exception when write log response: " + subEx.Message);
                }

            }
            ptc.Close();


        }
        catch (Exception e)
        {
            //Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusID);
            object a = e.Message;
            //Helper.WriteLog(l4NC, msgReq, "Lỗi Hàm PaymentBNK: " + a);
            Funcs.WriteLog("Lỗi Hàm PaymentBNK: " + a);
        }
        finally
        {
            // Helper.WriteLog(l4NC, res, cusID + " response Inquiry ");
            Funcs.WriteLog(" response Inquiry: " + res);
        }

        return res;
    }
}