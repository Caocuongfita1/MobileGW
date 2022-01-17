using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using CardDPP;

/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class CardDPPIntegration
{
    public CardDPPIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public GetTransactionListResType GetCardHist
        (string custId, 
        string cardNo,
        string enquiryType, 
        string fromDate, 
        string toDate)
    {
        CardDPP.AppHdrType appHdr = new CardDPP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardDPP.PairsType nsFrom = new CardDPP.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CardDPP.PairsType nsTo = new CardDPP.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CardDPP.PairsType[] listOfNsTo = new CardDPP.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardDPP.PairsType BizSvc = new CardDPP.PairsType();
        BizSvc.Id = "GetTransactionList";
        BizSvc.Name = "GetTransactionList";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        GetTransactionListReqType msgReq = new GetTransactionListReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cardNo = cardNo;
        msgReq.custId = custId;
        msgReq.enquiryType = enquiryType;
        msgReq.fromDate = fromDate;
        msgReq.toDate = toDate;
        msgReq.partnerCode = Funcs.getConfigVal("CARD_DPP_PARTNER");
        msgReq.tranSign = Funcs.MD5HashEncoding(msgReq.partnerCode + msgReq.custId + msgReq.cardNo + msgReq.enquiryType + msgReq.fromDate + msgReq.toDate + Funcs.getConfigVal("CARD_DPP_SHAREKEY"));

        GetTransactionListResType res = null;

        //portypeClient
        try
        {
            CardDPP.PortTypeClient ptc = new CardDPP.PortTypeClient();
            Funcs.WriteLog("custid:" + custId + "|GetTransactionList|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.GetTransactionList(msgReq);

            Funcs.WriteLog("custid:" + custId + "|GetTransactionList|RES = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GetTransactionList EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    public GetInterestAndFeeResType GetCardInstallmentPeriod
        (string custId,
        string tranId,
        string cardNo,
        string amount)
    {
        CardDPP.AppHdrType appHdr = new CardDPP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardDPP.PairsType nsFrom = new CardDPP.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CardDPP.PairsType nsTo = new CardDPP.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CardDPP.PairsType[] listOfNsTo = new CardDPP.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardDPP.PairsType BizSvc = new CardDPP.PairsType();
        BizSvc.Id = "GetInterestAndFee";
        BizSvc.Name = "GetInterestAndFee";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        GetInterestAndFeeReqType msgReq = new GetInterestAndFeeReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cardNo = cardNo;
        msgReq.amount = amount;
        msgReq.tranId = tranId;
        msgReq.partnerCode = Funcs.getConfigVal("CARD_DPP_PARTNER");
        msgReq.tranSign = Funcs.MD5HashEncoding(msgReq.partnerCode + msgReq.tranId + msgReq.cardNo + msgReq.amount + Funcs.getConfigVal("CARD_DPP_SHAREKEY"));

        GetInterestAndFeeResType res = null;

        //portypeClient
        try
        {
            CardDPP.PortTypeClient ptc = new CardDPP.PortTypeClient();
            Funcs.WriteLog("custid:" + custId + "|GetInterestAndFee|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.GetInterestAndFee(msgReq);

            Funcs.WriteLog("custid:" + custId + "|GetInterestAndFee|RES = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GetInterestAndFee EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    public RegisterTranResType InsertCardInstallmentPeriod
        (string custId,
        string tranId,
        string cardNo,
        string periodId,
        string amount,
        string phone,
        string email)
    {
        CardDPP.AppHdrType appHdr = new CardDPP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardDPP.PairsType nsFrom = new CardDPP.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CardDPP.PairsType nsTo = new CardDPP.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CardDPP.PairsType[] listOfNsTo = new CardDPP.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardDPP.PairsType BizSvc = new CardDPP.PairsType();
        BizSvc.Id = "RegisterTran";
        BizSvc.Name = "RegisterTran";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        RegisterTranReqType msgReq = new RegisterTranReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cardNo = cardNo;
        msgReq.tranId = tranId;
        msgReq.periodId = periodId;
        msgReq.amount = amount;
        msgReq.custId = custId;
        msgReq.email = email;
        msgReq.phone = phone;
        msgReq.partnerCode = Funcs.getConfigVal("CARD_DPP_PARTNER");
        msgReq.tranSign = Funcs.MD5HashEncoding(msgReq.partnerCode 
            + msgReq.tranId 
            + msgReq.cardNo 
            + msgReq.periodId
            + msgReq.custId
            + msgReq.amount
            + Funcs.getConfigVal("CARD_DPP_SHAREKEY"));

        RegisterTranResType res = null;

        //portypeClient
        try
        {
            CardDPP.PortTypeClient ptc = new CardDPP.PortTypeClient();
            Funcs.WriteLog("custid:" + custId + "|RegisterTran|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.RegisterTran(msgReq);

            Funcs.WriteLog("custid:" + custId + "|RegisterTran|RES = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("RegisterTran EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    public GetCardInstallmentPeriodByTranResType GetCardInstallmentPeriodByTran
        (string custId,
        string tranId,
        string cardNo)
    {
        CardDPP.AppHdrType appHdr = new CardDPP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardDPP.PairsType nsFrom = new CardDPP.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CardDPP.PairsType nsTo = new CardDPP.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CardDPP.PairsType[] listOfNsTo = new CardDPP.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardDPP.PairsType BizSvc = new CardDPP.PairsType();
        BizSvc.Id = "GetCardInstallmentPeriodByTran";
        BizSvc.Name = "GetCardInstallmentPeriodByTran";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        GetCardInstallmentPeriodByTranReqType msgReq = new GetCardInstallmentPeriodByTranReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cardNo = cardNo;
        msgReq.tranId = tranId;
        msgReq.partnerCode = Funcs.getConfigVal("CARD_DPP_PARTNER");
        msgReq.tranSign = Funcs.MD5HashEncoding(msgReq.partnerCode + msgReq.tranId + msgReq.cardNo + Funcs.getConfigVal("CARD_DPP_SHAREKEY"));

        GetCardInstallmentPeriodByTranResType res = null;

        //portypeClient
        try
        {
            CardDPP.PortTypeClient ptc = new CardDPP.PortTypeClient();
            Funcs.WriteLog("custid:" + custId + "|GetCardInstallmentPeriodByTran|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.GetCardInstallmentPeriodByTran(msgReq);

            Funcs.WriteLog("custid:" + custId + "|GetCardInstallmentPeriodByTran|RES = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GetCardInstallmentPeriodByTran EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }

    public GetScheduleListResType GetCardInstallmentSchedule
        (string custId,
        string tranId,
        string cardNo,
        string periodId,
        string amount)
    {
        CardDPP.AppHdrType appHdr = new CardDPP.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardDPP.PairsType nsFrom = new CardDPP.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CardDPP.PairsType nsTo = new CardDPP.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CardDPP.PairsType[] listOfNsTo = new CardDPP.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardDPP.PairsType BizSvc = new CardDPP.PairsType();
        BizSvc.Id = "GetScheduleList";
        BizSvc.Name = "GetScheduleList";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        GetScheduleListReqType msgReq = new GetScheduleListReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cardNo = cardNo;
        msgReq.tranId = tranId;
        msgReq.periodId = periodId;
        msgReq.amount = amount;
        msgReq.partnerCode = Funcs.getConfigVal("CARD_DPP_PARTNER");
        msgReq.tranSign = Funcs.MD5HashEncoding(msgReq.partnerCode + msgReq.tranId + msgReq.cardNo + msgReq.periodId + msgReq.amount + Funcs.getConfigVal("CARD_DPP_SHAREKEY"));

        GetScheduleListResType res = null;

        //portypeClient
        try
        {
            CardDPP.PortTypeClient ptc = new CardDPP.PortTypeClient();
            Funcs.WriteLog("custid:" + custId + "|GetScheduleList|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.GetScheduleList(msgReq);

            Funcs.WriteLog("custid:" + custId + "|GetScheduleList|RES = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("GetScheduleList EXCEPTION FROM ESB: " + ex.ToString());
        }

        return res;
    }
}