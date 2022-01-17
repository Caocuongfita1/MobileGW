
using mobileGW.Service.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for CardUltilsESB
/// </summary>
public class CardUltilsESB
{
    public CardUltilsESB()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string getCardList(Hashtable hashTbl)
    {
        string retStr = Config.GET_CARD_LIST;
     
        try
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            //string custid = "0000172509";//Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            //string cardno = "40464869cf9c93216259f9954ef93b5736482e6495";

            //get card list
            CardList.CardListInqResType cardListResp = CardIntegration.getCardList(custid);
            //CardLimit.CardLimitInqResType cardLimitResp = new CardIntegration().getCardLimits(custid, cardno);

            if (cardListResp != null && cardListResp.CardRec != null && cardListResp.CardRec.Count() > 0)
            {
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{ERR_DESC}", "GET CARD LIST SUCCESSFUL");

                //create temp string
                StringBuilder strTemp = new StringBuilder();

                //foreach (CardInfoType item in cardListResp.CardRec)
                ////foreach (CardListInqResType item in cardListResp.CardRec)
                //{
                //    strTemp.Append(item.CardId.ToString()).Append(Config.COL_REC_DLMT);
                //    strTemp.Append(Funcs.MaskCardNo(item.CardId)).Append(Config.COL_REC_DLMT);

                //}

                strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                retStr = retStr.Replace("{RECORD}", strTemp.ToString());
            }
            else
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("GET_CARD_LIST|CIF:" + custid + "NO DATA FOUND");
                return retStr;
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GET_CARD_LIST EXCEPTION: " + ex.ToString());
            retStr = Config.ERR_MSG_GENERAL;
        }

        return retStr;
    }
}