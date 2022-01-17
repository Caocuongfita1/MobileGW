using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for SmsIntergration
/// </summary>
public class SmsIntergration
{
    public SmsIntergration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static string convertToNewMobile(string mobileNo)
    {
        //',0186',',056'),',0188',',058'),',0199',',059'),',0120',',070'),',0121',',079'),',0122',',077'),',0126',',076')
        //,',0128',',078'),',0123',',083'),',0124',',084'),',0125',',085'),',0127',',081'),',0129',',082'),',0162',',032')
        //,',0163',',033'),',0164',',034'),',0165',',035'),',0166',',036'),',0167',',037'),',0168',',038'),',0169',',039')
        System.Text.StringBuilder newMobileBuilder = new System.Text.StringBuilder(",").Append(mobileNo);
        newMobileBuilder = newMobileBuilder
            .Replace(",0186", ",056")
            .Replace(",0188", ",058").Replace(",0199", ",059").Replace(",0120", ",070").Replace(",0121", ",079")
            .Replace(",0122", ",077").Replace(",0126", ",076").Replace(",0128", ",078").Replace(",0123", ",083")
            .Replace(",0124", ",084").Replace(",0125", ",085").Replace(",0127", ",081").Replace(",0129", ",082")
            .Replace(",0162", ",032").Replace(",0163", ",033").Replace(",0164", ",034").Replace(",0165", ",035")
            .Replace(",0166", ",036").Replace(",0167", ",037").Replace(",0168", ",038").Replace(",0169", ",039")
            ;

        newMobileBuilder = newMobileBuilder.Replace(",", "");
        return newMobileBuilder.ToString();
    }

    public bool sendOTP(string custid, string mobileNo, string smsContent, string partnerId, double tranId)
    {
        SMS.SMSSendResType res = null;
        try
        {
            #region header
            SMS.AppHdrType appHdr = new SMS.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            SMS.PairsType nsFrom = new SMS.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            SMS.PairsType nsTo = new SMS.PairsType();
            nsTo.Id = "SMS";
            nsTo.Name = "SMS";

            SMS.PairsType[] listOfNsTo = new SMS.PairsType[1];
            listOfNsTo[0] = nsTo;

            SMS.PairsType BizSvc = new SMS.PairsType();
            BizSvc.Id = "SMS";
            BizSvc.Name = "SMS";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            SMS.SMSSendReqType msgReq = new SMS.SMSSendReqType();
            msgReq.AppHdr = appHdr;
            appHdr.Signature = string.Empty;

            msgReq.BrCd = Config.HO_BR_CODE;
            msgReq.MobileNo = mobileNo;
            msgReq.MsgContent = smsContent;
            msgReq.ChnlId = Config.ChannelID;
            msgReq.ReqId = tranId.ToString();
            msgReq.ReqTime = DateTime.Now.ToString("yyyyMMdd");
            msgReq.PartnerId = Funcs.getConfigVal("SMS_PARTNER");
            #endregion

            SMS.PortTypeClient ptc = new SMS.PortTypeClient();
            res = ptc.Send(msgReq);

            Funcs.WriteLog("custid:" + custid + "|mobileNo:" + mobileNo + "|sendOTP|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("sendOTP EXCEPTION FROM ESB: " + ex.ToString());
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return true;
        }

        return false;
    }

    public bool sendSMS(string custid, string mobileNo, string smsContent, string partnerId, double tranId)
    {
        SMS.SMSSendResType res = null;
        try
        {
            #region header
            SMS.AppHdrType appHdr = new SMS.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            SMS.PairsType nsFrom = new SMS.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            SMS.PairsType nsTo = new SMS.PairsType();
            nsTo.Id = "SMS";
            nsTo.Name = "SMS";

            SMS.PairsType[] listOfNsTo = new SMS.PairsType[1];
            listOfNsTo[0] = nsTo;

            SMS.PairsType BizSvc = new SMS.PairsType();
            BizSvc.Id = "SMS";
            BizSvc.Name = "SMS";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            SMS.SMSSendReqType msgReq = new SMS.SMSSendReqType();
            msgReq.AppHdr = appHdr;
            appHdr.Signature = string.Empty;

            msgReq.BrCd = Config.HO_BR_CODE;
            msgReq.MobileNo = mobileNo;
            msgReq.MsgContent = smsContent;
            msgReq.ChnlId = Config.ChannelID;
            msgReq.ReqId = tranId.ToString();
            msgReq.ReqTime = DateTime.Now.ToString("yyyyMMdd");
            msgReq.PartnerId = Funcs.getConfigVal("SMS_PARTNER");
            #endregion

            SMS.PortTypeClient ptc = new SMS.PortTypeClient();
            res = ptc.Send(msgReq);

            Funcs.WriteLog("custid:" + custid + "|mobileNo:" + mobileNo + "|SendSMS|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("sendOTP EXCEPTION FROM ESB: " + ex.ToString());
        }

        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
        {
            return true;
        }

        return false;
    }
}