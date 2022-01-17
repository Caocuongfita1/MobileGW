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
using mobileGW.Service.DataAccess;
using mobileGW.Service.Models;
using iBanking.Common;
using Oracle.ManagedDataAccess.Client;

/// <summary>
/// Summary description for QRPayment
/// </summary>
public class DLVNSOA_
{
    public OracleCommand dynParams;
    public DLVNSOA_()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string getBillDaiichi(Hashtable hashTbl, string ip, string userAgent){
        string retStr = Config.RESPONE_GET_BILL_DAIICHI;
        string errCode = Config.gResult_DaiiCHI_Arr[1].Split('|')[0].ToString();
        string errDesc = Config.gResult_DaiiCHI_Arr[1].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.gResult_DaiiCHI_Arr[1].Split('|')[2].ToString();
        string policydetail = "";

        string policyNumber = (Funcs.getValFromHashtbl(hashTbl, "POLICYNUMBER"));
        string userName = Funcs.getConfigVal("USERNAME_DAIICHI");
        string passWord = Funcs.getConfigVal("PASSWORD_DAIICHI");
        DLVNSOA.PolicyInquiryResType res = null;

        try
        {
            res = DLVNIntegration.inquiryPolicy(userName, passWord, policyNumber);

            Funcs.WriteLog("Daiichi RES: " + new JavaScriptSerializer().Serialize(res));

            if (res == null)
            {
                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                retStr = retStr.Replace(Config.ERR_DESC_VAL, errDesc);
                retStr = retStr.Replace("{POLICY_DETAIL}", policydetail);

                return retStr;
            }

            List<DLVNSOA.POLICY_DETAIL> lstPOLICY_DETAIL = new List<DLVNSOA.POLICY_DETAIL>(res.POLICY_DETAIL);

            if (lstPOLICY_DETAIL.Count < 1 || lstPOLICY_DETAIL == null || !res.ERROR_CODE.Equals("0"))
            {
                for (int i = 1; i < Config.gResult_DaiiCHI_Arr.Length; i++)
                {
                    if (Config.gResult_DaiiCHI_Arr[i].Split('|')[0].Equals(res.ERROR_CODE))
                    {
                        errCode = Config.gResult_DaiiCHI_Arr[i].Split('|')[0].ToString();
                        errDesc = Config.gResult_DaiiCHI_Arr[i].Split('|')[1].ToString() 
                            + Config.COL_REC_DLMT 
                            + Config.gResult_DaiiCHI_Arr[i].Split('|')[2].ToString();
                    }
                }
            }
            else {
                errCode = Config.ERR_CODE_DONE;
                errDesc = res.ERROR_MSG;

                string strTemp = "";
                foreach (DLVNSOA.POLICY_DETAIL item in lstPOLICY_DETAIL)
                {
                    strTemp = strTemp +
                            "ADDRESS"
                            + Config.COL_REC_DLMT +
                            (item.ADDRESS == "" ? "" : item.ADDRESS)
                            + Config.COL_REC_DLMT +
                            "AMOUNT"
                            + Config.COL_REC_DLMT +
                            (item.AMOUNT == "" ? "" : item.AMOUNT.Replace(",",""))
                            + Config.COL_REC_DLMT +
                            "DATA01"
                            + Config.COL_REC_DLMT +
                            (item.DATA01 == "" ? "" : item.DATA01)
                            + Config.COL_REC_DLMT +
                            "DATA02"
                            + Config.COL_REC_DLMT +
                            (item.DATA02 == "" ? "" : item.DATA02)
                            + Config.COL_REC_DLMT +
                            "DLVN_REF"
                            + Config.COL_REC_DLMT +
                            (item.DLVN_REF == "" ? "" : item.DLVN_REF)
                            + Config.COL_REC_DLMT +
                            "FREQUENCE_PREMIUM"
                            + Config.COL_REC_DLMT +
                            (item.FREQUENCE_PREMIUM == "" ? "" : item.FREQUENCE_PREMIUM)
                            + Config.COL_REC_DLMT +
                            "ID_NUMBER"
                            + Config.COL_REC_DLMT +
                            (item.ID_NUMBER == "" ? "" : item.ID_NUMBER)
                            + Config.COL_REC_DLMT +
                            "LI_NAME"
                            + Config.COL_REC_DLMT +
                            (item.LI_NAME == "" ? "" : item.LI_NAME)
                            + Config.COL_REC_DLMT +
                            "LI_NUMBER"
                            + Config.COL_REC_DLMT +
                            (item.LI_NUMBER == "" ? "" : item.LI_NUMBER)
                            + Config.COL_REC_DLMT +
                            "PAID_TO_DATE"
                            + Config.COL_REC_DLMT +
                            (item.PAID_TO_DATE == "" ? "" : item.PAID_TO_DATE)
                            + Config.COL_REC_DLMT +
                            "PHONE_NUMBER"
                            + Config.COL_REC_DLMT +
                            (item.PHONE_NUMBER == "" ? "" : item.PHONE_NUMBER)
                            + Config.COL_REC_DLMT +
                            "POLICY_NUMBER"
                            + Config.COL_REC_DLMT +
                            (item.POLICY_NUMBER == "" ? "" : item.POLICY_NUMBER)
                            + Config.COL_REC_DLMT +
                            "POLICY_OWNERNAME"
                            + Config.COL_REC_DLMT +
                            (item.POLICY_OWNERNAME == "" ? "" : item.POLICY_OWNERNAME)
                            + Config.COL_REC_DLMT +
                            "POLICY_STATUS"
                            + Config.COL_REC_DLMT +
                            (item.POLICY_STATUS == "" ? "" : item.POLICY_STATUS)
                            + Config.COL_REC_DLMT +
                            "PREM_TYPE"
                            + Config.COL_REC_DLMT +
                            (item.PREM_TYPE == "" ? "" : item.PREM_TYPE)
                            + Config.COL_REC_DLMT +
                            "SERVICING_BRANCH"
                            + Config.COL_REC_DLMT +
                            (item.SERVICING_BRANCH == "" ? "" : item.SERVICING_BRANCH)
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                }

                policydetail = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("policyNumber:" + policyNumber + "ERROR Call Esb" + ex.Message);
            errCode = Config.ERR_CODE_GENERAL;
        }

        retStr = retStr.Replace(Config.ERR_CODE_VAL, errCode);
        retStr = retStr.Replace(Config.ERR_DESC_VAL, errDesc);
        retStr = retStr.Replace("{POLICY_DETAIL}", policydetail);

        return retStr;
    }

    public string doPayment(string policyNumber, string dlvnRef, string PaymentAmount, string prmAmount, string paymentDate, string freqPremium,
         string premType, string payerName, string payerAddress, string payerPhone, string comment, string refNumber, string channelID)
    {
        string retStr = Config.RESPONE_GET_BILL_DAIICHI;
        string errCode = Config.ERR_CODE_GENERAL;

        string userName = Funcs.getConfigVal("USERNAME_DAIICHI");
        string passWord = Funcs.getConfigVal("PASSWORD_DAIICHI");

        DLVNSOA.PaymentBNKResType res = null;
        try
        {
            String _str = payerPhone;
            string phone = "";
            string[] words = _str.Split(';');
            if (words != null && words.Length > 0)
            {
                phone = words[0];
            }

            res = DLVNIntegration.paymentBNK(userName, passWord, policyNumber, dlvnRef, PaymentAmount, prmAmount,paymentDate, freqPremium,
         premType, payerName, payerAddress, phone, comment, refNumber, channelID);

           
            Funcs.WriteLog("thong tin res:" + new JavaScriptSerializer().Serialize(res));

            if (res.ERROR_CODE.Equals("DLVN0"))
            {
                return errCode = Config.ERR_CODE_DONE;
            }
            else {
                for (int i = 1; i < Config.gResult_DaiiCHI_Arr.Length; i++)
                {
                    if (Config.gResult_DaiiCHI_Arr[i].Split('|')[0].Equals(res.ERROR_CODE))
                    {
                        errCode = Config.ERR_CODE_REVERT;
                        return errCode;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("doPayment:" + policyNumber + "ERROR Call Esb" + ex.Message);
            return errCode = Config.ERR_CODE_GENERAL;
        }

        return errCode;
    }

    public static List<PolicyDetailDLVN> getBillingList(string[] billing)
    {
        List<PolicyDetailDLVN> list = new List<PolicyDetailDLVN>();
        for (int i = 0; i < billing.Length; i++)
        {
            PolicyDetailDLVN item = new PolicyDetailDLVN();
            item.ADDRESS = billing[i].Split('$')[1];
            item.AMOUNT = billing[i].Split('$')[3];
            item.AMOUNT_TRANSFER = "";
            item.CHECK_INSURANCE = "";
            item.DATA01 = billing[i].Split('$')[5];
            item.DATA02 = billing[i].Split('$')[7];
            item.DLVN_REF = billing[i].Split('$')[9];
            item.FREQUENCE_PREMIUM = billing[i].Split('$')[11];
            item.ID_NUMBER = billing[i].Split('$')[13];
            item.LI_NAME = billing[i].Split('$')[17];
            item.LI_NUMBER = billing[i].Split('$')[15];
            item.PAID_TO_DATE = billing[i].Split('$')[19];
            item.PHONE_NUMBER = billing[i].Split('$')[21];
            item.POLICY_NUMBER = billing[i].Split('$')[23];
            item.POLICY_OWNERNAME = billing[i].Split('$')[25];
            item.POLICY_STATUS = billing[i].Split('$')[27];
            item.PREM_TYPE = billing[i].Split('$')[29];
            item.SERVICING_BRANCH = billing[i].Split('$')[31];

            list.Add(item);
        }

        return list;
    }

    public static void ins_payment_daichi(
                string BILL_ID,
                string BILL_CODE,
                string SERIE,
                string SERVICE_TYPE_ID,
                string SERVICE_PROVIDER_ID,
                string CLIENT_SERVICE_NO,
                string CLIENT_SERVICE_ID,
                string BILL_DATE,
                string ISSUED_DATE,
                string FROM_DATE,
                string TO_DATE,
                string PERIOD,
                string AMOUNT,
                string PRICE,
                string VALUE,
                string TAX_PERCENT,
                string TAX_VALUE,
                string TOTAL_VALUE,
                string SETTLED_VALUE,
                string SETTLEMENT_ID,
                string CCY,
                string DESCRIPTION,
                string BILL_TYPE,
                string IS_PRINTED,
                string EXTRA1,
                string EXTRA2,
                string EXTRA3,
                string EXTRA4,
                string EXTRA5,
                string ATV,
                string MKR_ID,
                string MKR_DT,
                string AUTH_ID,
                string AUTH_DT,
                string BILL_TYPE_DESC,
                string ORIGINAL_BILL_ID,
                string CLIENT_SERVICE_NAME,
                string CLIENT_PASSPORT_NO,
                string CLIENT_ADDRESS,
                string CLIENT_PHONE_NO,
                string TOKEN_KEY,
                string CLIENT_PASSPORT_PLACE,
                string CLIENT_PASSPORT_DATE,
                string TRANID,
                string STATUS
            )
    {
        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("pBILL_ID", BILL_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBILL_CODE", BILL_CODE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pSERIE", SERIE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pSERVICE_TYPE_ID", SERVICE_TYPE_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pSERVICE_PROVIDER_ID", SERVICE_PROVIDER_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_SERVICE_NO", CLIENT_SERVICE_NO, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_SERVICE_ID", CLIENT_SERVICE_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBILL_DATE", BILL_DATE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pISSUED_DATE", ISSUED_DATE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pFROM_DATE", FROM_DATE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTO_DATE", TO_DATE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pPERIOD", PERIOD, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pAMOUNT", AMOUNT, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pPRICE", PRICE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pVALUE", VALUE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTAX_PERCENT", TAX_PERCENT, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTAX_VALUE", TAX_VALUE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTOTAL_VALUE", TOTAL_VALUE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pSETTLED_VALUE", SETTLED_VALUE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pSETTLEMENT_ID", SETTLEMENT_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCCY", CCY, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pDESCRIPTION", DESCRIPTION, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBILL_TYPE", BILL_TYPE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pIS_PRINTED", IS_PRINTED, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pEXTRA1", EXTRA1, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pEXTRA2", EXTRA2, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pEXTRA3", EXTRA3, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pEXTRA4", EXTRA4, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pEXTRA5", EXTRA5, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pATV", ATV, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pMKR_ID", MKR_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pMKR_DT", MKR_DT, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pAUTH_ID", AUTH_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pAUTH_DT", AUTH_DT, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBILL_TYPE_DESC", BILL_TYPE_DESC, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pORIGINAL_BILL_ID", ORIGINAL_BILL_ID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_SERVICE_NAME", CLIENT_SERVICE_NAME, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_PASSPORT_NO", CLIENT_PASSPORT_NO, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_ADDRESS", CLIENT_ADDRESS, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_PHONE_NO", CLIENT_PHONE_NO, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTOKEN_KEY", TOKEN_KEY, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_PASSPORT_PLACE", CLIENT_PASSPORT_PLACE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCLIENT_PASSPORT_DATE", CLIENT_PASSPORT_DATE, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTRANID", TRANID, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pSTATUS", STATUS, dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCHANNEL", "MOB", dbType: OracleDbType.NVarchar2, direction: ParameterDirection.Input);

            new ConnectionFactory(Config.gEBANKConnstr)
                                    .ExecuteData<bool>(CommandType.StoredProcedure, "PKG_TX_NEW.PAYMENT_DAICHI", dynParams);
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
        }
    }
}