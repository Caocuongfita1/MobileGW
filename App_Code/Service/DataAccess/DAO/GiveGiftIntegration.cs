using mobileGW.Service.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class GiveGiftIntegration
{
    public GiveGiftIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static AcctSurprise.GetGiftTypeResType GET_GIFT_TYPE(string cusID)
    {
        AcctSurprise.AppHdrType appHdr = new AcctSurprise.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctSurprise.PairsType nsFrom = new AcctSurprise.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        AcctSurprise.PairsType nsTo = new AcctSurprise.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        AcctSurprise.PairsType[] listOfNsTo = new AcctSurprise.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctSurprise.PairsType BizSvc = new AcctSurprise.PairsType();
        BizSvc.Id = "GET_GIFT_TYPE";
        BizSvc.Name = "GET_GIFT_TYPE";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctSurprise.GetGiftTypeReqType msgReq = new AcctSurprise.GetGiftTypeReqType();
        msgReq.AppHdr = appHdr;

        Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_TYPE REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        AcctSurprise.GetGiftTypeResType res = null;
        //portypeClient
        try
        {
            AcctSurprise.PortTypeClient ptc = new AcctSurprise.PortTypeClient();
            res = ptc.GetGiftType(msgReq);

            Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_TYPE RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_TYPE EXCEPTION FROM ESB: " + ex.ToString());
            return null;
        }

        return res;
    }

    public static AcctSurprise.GetGiftTemplaceResType GET_GIFT_TEMPLACE(string cusID, string giftType, string templaceType)
    {
        AcctSurprise.AppHdrType appHdr = new AcctSurprise.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctSurprise.PairsType nsFrom = new AcctSurprise.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        AcctSurprise.PairsType nsTo = new AcctSurprise.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        AcctSurprise.PairsType[] listOfNsTo = new AcctSurprise.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctSurprise.PairsType BizSvc = new AcctSurprise.PairsType();
        BizSvc.Id = "GET_GIFT_TEMPLACE";
        BizSvc.Name = "GET_GIFT_TEMPLACE";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctSurprise.GetGiftTemplaceReqType msgReq = new AcctSurprise.GetGiftTemplaceReqType();
        msgReq.AppHdr = appHdr;
        msgReq.giftType = giftType;
        msgReq.templaceType = templaceType;

        Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_TEMPLACE REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        AcctSurprise.GetGiftTemplaceResType res = null;
        //portypeClient
        try
        {
            AcctSurprise.PortTypeClient ptc = new AcctSurprise.PortTypeClient();
            res = ptc.GetGiftTemplace(msgReq);

            Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_TEMPLACE|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_TEMPLACE EXCEPTION FROM ESB: " + ex.ToString());
            return null;
        }

        return res;
    }

    public static AcctSurprise.SurpriseBlockResType ACCT_SURPRISE_BLOCK(string cusID, string src_acct, string refNo,double amount, string effDT, string reasonTXT)
    {
        AcctSurprise.AppHdrType appHdr = new AcctSurprise.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctSurprise.PairsType nsFrom = new AcctSurprise.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        AcctSurprise.PairsType nsTo = new AcctSurprise.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        AcctSurprise.PairsType[] listOfNsTo = new AcctSurprise.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctSurprise.PairsType BizSvc = new AcctSurprise.PairsType();
        BizSvc.Id = "ACCT_SURPRISE_BLOCK";
        BizSvc.Name = "ACCT_SURPRISE_BLOCK";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctSurprise.SurpriseBlockReqType msgReq = new AcctSurprise.SurpriseBlockReqType();
        msgReq.AppHdr = appHdr;
        msgReq.acctNo = src_acct;
        msgReq.refNo = refNo;
        msgReq.orgEarAmount = amount.ToString();
        msgReq.reasonTXT = reasonTXT;
        msgReq.effDT = effDT;

        Funcs.WriteLog("custid:" + cusID + "|ACCT_SURPRISE_BLOCK REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        AcctSurprise.SurpriseBlockResType res = null;
        //portypeClient
        try
        {
            AcctSurprise.PortTypeClient ptc = new AcctSurprise.PortTypeClient();
            res = ptc.SurpriseBlock(msgReq);

            Funcs.WriteLog("custid:" + cusID + "|ACCT_SURPRISE_BLOCK RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("custid:" + cusID + "|ACCT_SURPRISE_BLOCK EXCEPTION FROM ESB: " + ex.ToString());
            return null;
        }

        return res;
    }

    public static AcctSurprise.SurpriseUnblockResType ACCT_SURPRISE_UNBLOCK(string cusID, string refNo)
    {
        AcctSurprise.AppHdrType appHdr = new AcctSurprise.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctSurprise.PairsType nsFrom = new AcctSurprise.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        AcctSurprise.PairsType nsTo = new AcctSurprise.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        AcctSurprise.PairsType[] listOfNsTo = new AcctSurprise.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctSurprise.PairsType BizSvc = new AcctSurprise.PairsType();
        BizSvc.Id = "ACCT_SURPRISE_UNBLOCK";
        BizSvc.Name = "ACCT_SURPRISE_UNBLOCK";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctSurprise.SurpriseUnblockReqType msgReq = new AcctSurprise.SurpriseUnblockReqType();
        msgReq.AppHdr = appHdr;
        msgReq.refNo = refNo;

        Funcs.WriteLog("custid:" + cusID + "|ACCT_SURPRISE_UNBLOCK REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        AcctSurprise.SurpriseUnblockResType res = null;
        //portypeClient
        try
        {
            AcctSurprise.PortTypeClient ptc = new AcctSurprise.PortTypeClient();
            res = ptc.SurpriseUnblock(msgReq);

            Funcs.WriteLog("custid:" + cusID + "|ACCT_SURPRISE_UNBLOCK RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("custid:" + cusID + "|ACCT_SURPRISE_UNBLOCK EXCEPTION FROM ESB: " + ex.ToString());
            return null;
        }

        return res;
    }

    public static AcctSurprise.GetHistoryGiftResType GET_HISTORY_GIFT (string cusID, string giftTab)
    {
        AcctSurprise.AppHdrType appHdr = new AcctSurprise.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctSurprise.PairsType nsFrom = new AcctSurprise.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        AcctSurprise.PairsType nsTo = new AcctSurprise.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        AcctSurprise.PairsType[] listOfNsTo = new AcctSurprise.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctSurprise.PairsType BizSvc = new AcctSurprise.PairsType();
        BizSvc.Id = "GET_HISTORY_GIFT";
        BizSvc.Name = "GET_HISTORY_GIFT";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctSurprise.GetHistoryGiftReqType msgReq = new AcctSurprise.GetHistoryGiftReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cifNo = cusID;
        msgReq.giftTab = giftTab;

        Funcs.WriteLog("custid:" + cusID + "|GET_HISTORY_GIFT REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        AcctSurprise.GetHistoryGiftResType res = null;
        //portypeClient
        try
        {
            AcctSurprise.PortTypeClient ptc = new AcctSurprise.PortTypeClient();
            res = ptc.GetHistoryGift(msgReq);

            Funcs.WriteLog("custid:" + cusID + "|GET_HISTORY_GIFT RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("custid:" + cusID + "|GET_HISTORY_GIFT EXCEPTION FROM ESB: " + ex.ToString());
            return null;
        }

        return res;
    }

    public static AcctSurprise.GetDetailGiftResType GET_GIFT_DETAIL(string cusID, string giftTab, string refNo, string status)
    {
        AcctSurprise.AppHdrType appHdr = new AcctSurprise.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        AcctSurprise.PairsType nsFrom = new AcctSurprise.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        AcctSurprise.PairsType nsTo = new AcctSurprise.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        AcctSurprise.PairsType[] listOfNsTo = new AcctSurprise.PairsType[1];
        listOfNsTo[0] = nsTo;

        AcctSurprise.PairsType BizSvc = new AcctSurprise.PairsType();
        BizSvc.Id = "GET_GIFT_DETAIL";
        BizSvc.Name = "GET_GIFT_DETAIL";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        AcctSurprise.GetDetailGiftReqType msgReq = new AcctSurprise.GetDetailGiftReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cifNo = cusID;
        msgReq.giftTab = giftTab;
        msgReq.refNo = refNo;
        msgReq.status = status;

        Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_DETAIL REQ: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

        AcctSurprise.GetDetailGiftResType res = null;
        //portypeClient
        try
        {
            AcctSurprise.PortTypeClient ptc = new AcctSurprise.PortTypeClient();
            res = ptc.GetDetailGift(msgReq);

            Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_DETAIL RES: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();

        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("custid:" + cusID + "|GET_GIFT_DETAIL EXCEPTION FROM ESB: " + ex.ToString());
            return null;
        }

        return res;
    }
}