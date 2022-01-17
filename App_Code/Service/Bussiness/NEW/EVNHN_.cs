using mobileGW.Service.AppFuncs;
using mobileGW.Service.DataAccess;
using mobileGW.Service.Framework;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for QRPayment
/// </summary>
public class EVNHN_
{
	
	private class CustomerInfoItemType
    {
        public CustomerInfoItemType() { }
        public string customerAddr { get; set; }
        public string customerCd { get; set; }
        public string customerNm { get; set; }
        public string GCScd { get; set; }
    }

    private class BillingItemType {
        public BillingItemType() { }
        public string billingCd { get; set; }
        public string debtAmount { get; set; }
        public string tenure { get; set; }
    }

    public class billList
    {
        public string billingCd;
        public DateTime tenure;
        public string debtAmount;
    }
    public EVNHN_()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string getBill(Hashtable hashTbl){
        string retStr = Config.GET_BILL_INFO;
        string errCode = Config.ERR_CODE_GENERAL;
        string errDesc = "";
        string customerCdRes = "";
        string serviceCdRes = "";
        string totalDebt = "";
        string bankCdRes = "";
        string regionCdRes = "";
        string backupCd1 = "";
        string customerInfo = "";
        string billingList = "";
    
        string serviceCd = (Funcs.getValFromHashtbl(hashTbl, "SERVICECD"));
        string providerCd = (Funcs.getValFromHashtbl(hashTbl, "PROVIDERCD"));
        string customerCd = (Funcs.getValFromHashtbl(hashTbl, "BILL_ID")).ToUpper();
        string bankCd = (Funcs.getValFromHashtbl(hashTbl, "BANKCD"));
        string regionCd = (Funcs.getValFromHashtbl(hashTbl, "REGIONCD"));
        string branchCd = (Funcs.getValFromHashtbl(hashTbl, "BRANCHCD"));
        
        #region config header, param for request
        EVNHN.AppHdrType appHdr = new EVNHN.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        EVNHN.PairsType nsFrom = new EVNHN.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        EVNHN.PairsType nsTo = new EVNHN.PairsType();
        nsTo.Id = "ESB_EVNHN";
        nsTo.Name = "ESB_EVNHN";

        EVNHN.PairsType[] listOfNsTo = new EVNHN.PairsType[1];
        listOfNsTo[0] = nsTo;

        EVNHN.PairsType BizSvc = new EVNHN.PairsType();
        BizSvc.Id = "EVNHN";
        BizSvc.Name = "EVNHN";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;

        appHdr.TransDt = TransDt;
        #endregion
        try
        {
            EVNHN.EvnHNGetBillReqType msgReq = new EVNHN.EvnHNGetBillReqType();
            msgReq.AppHdr = appHdr;
            msgReq.serviceCd = Config.ElectricityEvnHN.MA_DV;
            msgReq.providerCd = Config.ElectricityEvnHN.PROVIDER_CD;
            msgReq.customerCd = customerCd;
            msgReq.bankCd = Config.ElectricityEvnHN.BANK_CD;
            msgReq.regionCd = "_NULL_";
            msgReq.branchCd = "_NULL_";

            EVNHN.EvnHNGetBillResType res = null;

            Funcs.WriteLog("customerCd:" + customerCd + "|Request message getBill:" + new JavaScriptSerializer().Serialize(msgReq));
            EVNHN.PortTypeClient ptc = new EVNHN.PortTypeClient();

            res = ptc.GetBill(msgReq);

            Funcs.WriteLog("customerCd:" + customerCd + "|getBill|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            errCode = res.respCd;
            errDesc = res.respDes;
            
            if (errCode.Equals("000"))
            {
                errCode = Config.ERR_CODE_DONE;
                customerCdRes = res.customerCd;
                serviceCdRes = res.serviceCd;
                totalDebt = res.totalDebt;
                bankCdRes = res.bankCd;
                regionCdRes = res.regionCd;
                backupCd1 = res.backupCd1;

                if (res.customerInfo != null)
                {
                    customerInfo = res.customerInfo.customerNm;
                }

                if (res.billingList != null)
                {
                    string strTemp = "";
                    for (int i = 0; i < res.billingList.Count(); i++)
                    {
                        strTemp = strTemp +
                            (res.billingList[i].billingCd.ToString() == "" ? "_NULL_" : res.billingList[i].billingCd.ToString())
                            + Config.COL_REC_DLMT +
                            (res.billingList[i].tenure.ToString() == "" ? "_NULL_" : res.billingList[i].tenure.ToString())
                            + Config.COL_REC_DLMT +
                            (res.billingList[i].debtAmount.ToString() == "" ? "_NULL_" : res.billingList[i].debtAmount.ToString())
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                    }

                    billingList = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                }
            }
            else {
                errCode = Config.ERR_CODE_GENERAL;
            }
            ptc.Close();
        }
        catch (Exception ex) {
            Funcs.WriteLog("customerCd:" + customerCd + "ERROR Call Esb" +ex.Message);
            errCode = Config.ERR_CODE_GENERAL;
        }

        retStr = retStr.Replace(Config.ERR_CODE_VAL, errCode);
        retStr = retStr.Replace(Config.ERR_DESC_VAL, errDesc);
        retStr = retStr.Replace("{BILL_AMOUNT}", totalDebt);
        retStr = retStr.Replace("{BILL_INFO_EXT1}", customerInfo);
        retStr = retStr.Replace("{BILL_INFO_EXT2}", billingList);
        retStr = retStr.Replace("{BILL_ID}", customerCd);
        retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

        DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(customerCd);

        if (dt.Rows.Count > 0)
        {
            retStr = retStr.Replace("{PARTNER_ID}", dt.Rows[0]["PARTNER"].ToString());
            retStr = retStr.Replace("{CATEGORY_CODE}", dt.Rows[0]["CATEGORY_CODE"].ToString());
            retStr = retStr.Replace("{SERVICE_CODE}", dt.Rows[0]["SERVICE_CODE"].ToString());

            switch (dt.Rows[0]["PARTNER"].ToString())
            {
                case "PAYOO":
                    retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVNHN");
                    break;
                case "VNPAY":
                    retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVN_ALL");
                    break;
                case "EVNHN":
                    retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVNHN");
                    break;

                default:
                    retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVN_ALL");
                    break;
            }

        }
        else
        {
            retStr = retStr.Replace("{PARTNER_ID}", "VNPAY");
            retStr = retStr.Replace("{CATEGORY_CODE}", "_NULL_");
            retStr = retStr.Replace("{SERVICE_CODE}", "_NULL_");
            retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVN_ALL");
            retStr = retStr.Replace("{DESCRIPTION}", "");
        }

        return retStr;
    }

    public string doPayment(
        double tranid,
        string customerCd,
        string serviceCd,
        string debtAmount ,
        string transactionChannel,
        string regionCd,
        string address,
        string bankCd,
        string billingCd,
        string backupCd1,
        string providerCd)
    {
        string respMsg = string.Empty;
        EVNHN.EvnHNDoPayment1ResType res = null;
        try
        {
            #region header
            EVNHN.AppHdrType appHdr = new EVNHN.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            EVNHN.PairsType nsFrom = new EVNHN.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            EVNHN.PairsType nsTo = new EVNHN.PairsType();
            nsTo.Id = "ESB_EVNHN";
            nsTo.Name = "ESB_EVNHN";

            EVNHN.PairsType[] listOfNsTo = new EVNHN.PairsType[1];
            listOfNsTo[0] = nsTo;

            EVNHN.PairsType BizSvc = new EVNHN.PairsType();
            BizSvc.Id = "EVNHN";
            BizSvc.Name = "EVNHN";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            #region body
            EVNHN.EvnHNDoPayment1ReqType msgReq = new EVNHN.EvnHNDoPayment1ReqType();
            msgReq.AppHdr = appHdr;
            msgReq.address = address;
            msgReq.backupCd1 = backupCd1;
            msgReq.bankCd = bankCd;
            msgReq.billCd = billingCd;
            msgReq.customerCd = customerCd;
            msgReq.debtAmount = debtAmount;
            msgReq.debtTransactionCd = Convert.ToString(tranid);
            msgReq.paymentDt = TransDt.ToString("yyyyMMddHHmmss");
            msgReq.providerCd = providerCd;
            msgReq.regionCd = regionCd;
            msgReq.serviceCd = serviceCd;
            msgReq.transactionChannel = transactionChannel;
            #endregion

            EVNHN.PortTypeClient ptc = new EVNHN.PortTypeClient();
            
            res = ptc.DoPayment1(msgReq);

            Funcs.WriteLog("customerCd:" + customerCd + "|doPayment|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("doPayment EXCEPTION FROM ESB: " + ex.ToString());
            //set timeout
            return respMsg = Config.ERR_CODE_TIMEOUT;
        }

        if (res != null && res.respCd != null && res.respCd != null && res.respCd.Equals("000"))
        {
            return Config.ERR_CODE_DONE;
        }

        if (res != null && res.respCd != null && res.respCd != null)
        {
            return Config.ERR_CODE_GENERAL;
        }

        return respMsg;
    }

    public static List<billList> getBillingList(string[] billing)
    {
        List<billList> list = new List<billList>();
        for (int i = 0; i < billing.Length; i++)
        {
            billList item = new billList();
            item.billingCd = billing[i].Split('$')[0];
            item.tenure = Convert.ToDateTime(billing[i].Split('$')[1]);
            item.debtAmount = billing[i].Split('$')[2];

            list.Add(item);
        }

        list.Sort(new Comparison<billList>((x, y) => DateTime.Compare(x.tenure, y.tenure)));

        return list;
    }
}