using mobileGW.Service.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using CardInfoUtils;

/// <summary>
/// Summary description for CardIntegration
/// </summary>
public class CardUtilsIntegration
{
    public CardUtilsIntegration()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static CardInforDetailResType GET_PREPAID_CARD_DETAIL(string custId,string cardNum, string cardMD5)
    {
        CardInfoUtils.AppHdrType appHdr = new CardInfoUtils.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        CardInfoUtils.PairsType nsFrom = new CardInfoUtils.PairsType();
        nsFrom.Id = "MOB";
        nsFrom.Name = "MOB";

        CardInfoUtils.PairsType nsTo = new CardInfoUtils.PairsType();
        nsTo.Id = "ESB-SOA";
        nsTo.Name = "ESB-SOA";

        CardInfoUtils.PairsType[] listOfNsTo = new CardInfoUtils.PairsType[1];
        listOfNsTo[0] = nsTo;

        CardInfoUtils.PairsType BizSvc = new CardInfoUtils.PairsType();
        BizSvc.Id = "CardInforDetail";
        BizSvc.Name = "CardInforDetail";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        CardInforDetailReqType msgReq = new CardInforDetailReqType();
        msgReq.AppHdr = appHdr;
        msgReq.cardMD5 = cardMD5;
        msgReq.cardNum = cardNum;
        
        CardInforDetailResType res = null;

        //portypeClient
        try
        {
            CardInfoUtils.PortTypeClient ptc = new CardInfoUtils.PortTypeClient();
            Funcs.WriteLog("custid:" + custId + "|CardInforDetail|REQ = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

            res = ptc.CardInfoDetail(msgReq);

            Funcs.WriteLog("custid:" + custId + "|CardInforDetail|RES = " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("CardInforDetail EXCEPTION FROM ESB: " + ex.ToString());
            return null;
        }

        return res;
    }
}