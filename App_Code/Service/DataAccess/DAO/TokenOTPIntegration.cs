using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for DLVNIntegration
/// </summary>
public class TokenOTPIntegration
{
    public TokenOTPIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //private static log4net.ILog l4NC = log4net.LogManager.GetLogger(typeof(TokenOTPIntegration));
    public static TokenOTP.GetActivationCodeResType GetActivationCode(string branchID, string cifNumber, string customerName, string customerTypeID, string email, string phoneNumber, string userID, string userName)
    {
        TokenOTP.AppHdrType appHdr = new TokenOTP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TokenOTP.PairsType nsFrom = new TokenOTP.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TokenOTP.PairsType nsTo = new TokenOTP.PairsType();
        nsTo.Id = "ESB_TOKEN";
        nsTo.Name = "ESB_TOKEN";

        TokenOTP.PairsType[] listOfNsTo = new TokenOTP.PairsType[1];
        listOfNsTo[0] = nsTo;

        TokenOTP.PairsType BizSvc = new TokenOTP.PairsType();
        BizSvc.Id = "Global\\Utilities\\TokenOTP\\GetActivationCode";
        BizSvc.Name = "Global\\Utilities\\TokenOTP\\GetActivationCode";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        TokenOTP.GetActivationCodeReqType msgReq = new TokenOTP.GetActivationCodeReqType();
        msgReq.AppHdr = appHdr;
        msgReq.branchID = branchID;
        msgReq.cifNumber = cifNumber;
        msgReq.customerName = customerName;
        msgReq.customerTypeID = customerTypeID;
        msgReq.email = email;
        msgReq.phoneNumber = phoneNumber;
        msgReq.userID = userID;
        msgReq.userName = userName;

        Funcs.WriteLog("Hàm GetActivationCode REQUEST: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        TokenOTP.GetActivationCodeResType res = null;
        try
        {

            TokenOTP.PortTypeClient ptc = new TokenOTP.PortTypeClient();

            res = ptc.GetActivationCode(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("Hàm GetActivationCode RESPONSE: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
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
            Funcs.WriteLog("Exception: " + e.Message);
            object a = e.Message;
            Funcs.WriteLog("Lỗi Hàm GetActivationCode: " + a);
        }

        return res;
    }

    public static TokenOTP.VerifyOTPCRResType VerifyOTPCR (string userID, string transactionID, string otp)
    {
        TokenOTP.AppHdrType appHdr = new TokenOTP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TokenOTP.PairsType nsFrom = new TokenOTP.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TokenOTP.PairsType nsTo = new TokenOTP.PairsType();
        nsTo.Id = "ESB_TOKEN";
        nsTo.Name = "ESB_TOKEN";

        TokenOTP.PairsType[] listOfNsTo = new TokenOTP.PairsType[1];
        listOfNsTo[0] = nsTo;

        TokenOTP.PairsType BizSvc = new TokenOTP.PairsType();
        BizSvc.Id = "Global\\Utilities\\TokenOTP\\VerifyOTPCR";
        BizSvc.Name = "Global\\Utilities\\TokenOTP\\VerifyOTPCR";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        TokenOTP.VerifyOTPCRReqType msgReq = new TokenOTP.VerifyOTPCRReqType();
        msgReq.AppHdr = appHdr;
        msgReq.otp = "09" + otp;
        msgReq.transactionID = transactionID.PadLeft(24, '0');
        msgReq.userID = userID;

        Funcs.WriteLog("Hàm VerifyOTPCR REQUEST: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        TokenOTP.VerifyOTPCRResType res = null;
        try
        {

            TokenOTP.PortTypeClient ptc = new TokenOTP.PortTypeClient();

            res = ptc.VerifyOTPCR(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("Hàm VerifyOTPCR RESPONSE: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
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
            Funcs.WriteLog("Exception: " + e.Message);
            object a = e.Message;
            Funcs.WriteLog("Lỗi Hàm VerifyOTPCR: " + a);
        }

        return res;
    }

    public static TokenOTP.CreateTransactionResType CreateTransaction(string cifno, string transactionID)
    {
        TokenOTP.AppHdrType appHdr = new TokenOTP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TokenOTP.PairsType nsFrom = new TokenOTP.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TokenOTP.PairsType nsTo = new TokenOTP.PairsType();
        nsTo.Id = "ESB_TOKEN";
        nsTo.Name = "ESB_TOKEN";

        TokenOTP.PairsType[] listOfNsTo = new TokenOTP.PairsType[1];
        listOfNsTo[0] = nsTo;

        TokenOTP.PairsType BizSvc = new TokenOTP.PairsType();
        BizSvc.Id = "Global\\Utilities\\TokenOTP\\CreateTransaction";
        BizSvc.Name = "Global\\Utilities\\TokenOTP\\CreateTransaction";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        TokenOTP.CreateTransactionReqType msgReq = new TokenOTP.CreateTransactionReqType();
        msgReq.AppHdr = appHdr;
        msgReq.callbackUrl = "";
        msgReq.challenge = "";
        msgReq.channelID = "0";
        msgReq.eSignerTypeID = "0";
        msgReq.isOnline = "0";
        msgReq.isPush = "0";
        msgReq.transactionData = "";
        msgReq.transactionID = transactionID.PadLeft(24, '0');
        msgReq.transactionTypeID = "1";
        msgReq.userID = cifno;

        Funcs.WriteLog("Hàm CreateTransaction REQUEST: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        TokenOTP.CreateTransactionResType res = null;
        try
        {

            TokenOTP.PortTypeClient ptc = new TokenOTP.PortTypeClient();

            res = ptc.CreateTransaction(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("Hàm CreateTransaction RESPONSE: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
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
            Funcs.WriteLog("Exception: " + e.Message);
            object a = e.Message;
            Funcs.WriteLog("Lỗi Hàm CreateTransaction: " + a);
        }

        return res;
    }

    public static TokenOTP.SynchronizeOTPResType SynchronizeOTP(string cifno, string operatorID, string otp1, string otp2)
    {
        TokenOTP.AppHdrType appHdr = new TokenOTP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        TokenOTP.PairsType nsFrom = new TokenOTP.PairsType();
        nsFrom.Id = Config.ChannelID;
        nsFrom.Name = Config.ChannelID;

        TokenOTP.PairsType nsTo = new TokenOTP.PairsType();
        nsTo.Id = "ESB_TOKEN";
        nsTo.Name = "ESB_TOKEN";

        TokenOTP.PairsType[] listOfNsTo = new TokenOTP.PairsType[1];
        listOfNsTo[0] = nsTo;

        TokenOTP.PairsType BizSvc = new TokenOTP.PairsType();
        BizSvc.Id = "Global\\Utilities\\TokenOTP\\SynchronizeOTP";
        BizSvc.Name = "Global\\Utilities\\TokenOTP\\SynchronizeOTP";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Guid.NewGuid().ToString();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        //appHdr.Signature = MD5Encoding.Hash(CustomerCode + ProcessingCode + SettlementAmount.ToString() + Config.SharedKeyMD5);

        //Body
        TokenOTP.SynchronizeOTPReqType msgReq = new TokenOTP.SynchronizeOTPReqType();
        msgReq.AppHdr = appHdr;
        msgReq.userID = cifno;
        msgReq.operatorID = operatorID;
        msgReq.otp1 = "09" + otp1;
        msgReq.otp2 = "09" + otp2;

        Funcs.WriteLog("Hàm SynchronizeOTP REQUEST: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        TokenOTP.SynchronizeOTPResType res = null;
        try
        {

            TokenOTP.PortTypeClient ptc = new TokenOTP.PortTypeClient();

            res = ptc.SynchronizeOTP(msgReq);

            if (res != null)
            {
                try
                {
                    Funcs.WriteLog("Hàm SynchronizeOTP RESPONSE: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
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
            Funcs.WriteLog("Exception: " + e.Message);
            object a = e.Message;
            Funcs.WriteLog("Lỗi Hàm SynchronizeOTP: " + a);
        }

        return res;
    }
}