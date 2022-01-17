using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for SMSIntegration
/// </summary>
public class SMSIntegration
{
    public static object SendOTP(Users userSession, string smsCode, double tranId)
    {
        string gSMSCodeMsg = "MA XAC THUC (OTP) CUA GIAO DICH GD[TRANO] LA [SMSCODE]. QUY KHACH HAY NHAP VAO TRANG WEB DE HOAN TAT GIAO DICH. HOTLINE HO TRO 1800588856.";
        //BillingModel result = new BillingModel();
        SMS_Service.AppHdrType appHdr = new SMS_Service.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        SMS_Service.PairsType nsFrom = new SMS_Service.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        SMS_Service.PairsType nsTo = new SMS_Service.PairsType();
        nsTo.Id = "SMS";
        nsTo.Name = "SMS";

        SMS_Service.PairsType[] listOfNsTo = new SMS_Service.PairsType[1];
        listOfNsTo[0] = nsTo;

        SMS_Service.PairsType BizSvc = new SMS_Service.PairsType();
        BizSvc.Id = "SMS";
        BizSvc.Name = "SMS";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        SMS_Service.SMSSendReqType msgReq = new SMS_Service.SMSSendReqType();
        msgReq.AppHdr = appHdr;


        msgReq.BrCd = ((!string.IsNullOrEmpty(userSession.REG_BRANCH)) ? userSession.REG_BRANCH : "110000"); //Config.Core.HO_BR_CODE;
        msgReq.MobileNo = userSession.AUTH_INFO_EXT1;

        //msgReq.MobileNo = msgReq.MobileNo.StartsWith("84") ? msgReq.MobileNo : "84" + msgReq.MobileNo.Substring(1);


        msgReq.MsgContent = gSMSCodeMsg.Replace("[SMSCODE]", smsCode).Replace("[TRANO]", tranId.ToString());
        msgReq.ChnlId = "MOB";
        msgReq.ReqId = tranId.ToString();
        msgReq.ReqTime = DateTime.Now.ToString("yyyyMMdd");
        msgReq.PartnerId = "INCOM";
        appHdr.Signature = "";

        SMS_Service.SMSSendResType res = null;

        var json = new JavaScriptSerializer().Serialize(msgReq);

        string pattern = @"(MA XAC THUC \(OTP\) CUA GIAO DICH){1} [A-Za-z0-9]{1,} LA{1} [A-Za-z0-9]{1,}";
        Regex rgx = new Regex(pattern);
        //string sentence = "Who writes these notes?";
        String needReplace = string.Empty;
        String replace = string.Empty;
        foreach (Match match in rgx.Matches(json))
        {

            //Console.WriteLine("Found '{0}' at position {1}", match.Value, match.Index);
            needReplace = match.Value;
            string patternIn = @"(MA XAC THUC \(OTP\) CUA GIAO DICH){1} [A-Za-z0-9]{1,} LA{1} ";
            Regex rgxInside = new Regex(patternIn);
            replace = rgxInside.Match(needReplace).Value;

            replace += "xxxxxx";

            break;
        }

        json = json.Replace(needReplace, replace);

        Funcs.WriteLog(userSession.CUSTID + " request SendOTP ");
        try
        {
            //portypeClient
            SMS_Service.PortTypeClient ptc = new SMS_Service.PortTypeClient();
            res = ptc.Send(msgReq);
            Funcs.WriteLog("RES: " + res + " - CUSTID: " + userSession.CUSTID);
            ptc.Close();


        }
        catch (Exception e)
        {
            Funcs.WriteLog("ERROR: " + e.Message + e.StackTrace + " - CUSTID: " + userSession.CUSTID);

        }
        finally
        {
            Funcs.WriteLog("res: " + res + " - CUSTID: " + userSession.CUSTID + " response SendOTP");
        }
        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return true;
        }
        return false;
    }
}