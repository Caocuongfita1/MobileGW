using iBanking.Common;
using mobileGW.Service.Framework;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using RestSharp;
using mobileGW.Service.API;
/// <summary>
/// Summary description for PushNotification
/// </summary>
public class EVoucher
{
    public EVoucher()
    {

    }
    public static string GetVoucherList(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.RESPONE_GET_VOUCHER_LIST;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string trantype = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
        string amount = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");
        string list_voucher = "";
        
        List<EbankVoucherBaseModel> respModel = GetVoucherListAPIs(cifNo,Config.ChannelIDVoucher,trantype,amount);
        if (respModel != null)
        {
            string strTemp = "";
            foreach (EbankVoucherBaseModel item in respModel)
            {
                strTemp = strTemp +
                        "VOUCHERID"
                        + Config.COL_REC_DLMT +
                        item.VoucherId
                        + Config.COL_REC_DLMT +
                        "VOUCHERCODE"
                        + Config.COL_REC_DLMT +
                        item.VoucherCode
                        + Config.COL_REC_DLMT +
                        "SERIALNUM"
                        + Config.COL_REC_DLMT +
                        item.SerialNum
                        + Config.COL_REC_DLMT +
                        "PINNUM"
                        + Config.COL_REC_DLMT +
                        item.PinNum
                        + Config.COL_REC_DLMT +
                        "VOUCHERDESCRIPTIONVN"
                        + Config.COL_REC_DLMT +
                        item.VoucherDescriptionVn
                        + Config.COL_REC_DLMT +
                        "VOUCHERDESCRIPTIONEN"
                        + Config.COL_REC_DLMT +
                        item.VoucherDescriptionEn
                        + Config.COL_REC_DLMT +
                        "EFFECTIVEDATE"
                        + Config.COL_REC_DLMT +
                        item.EffectiveDate
                        + Config.COL_REC_DLMT +
                        "EXPIREDATE"
                        + Config.COL_REC_DLMT +
                        item.ExpireDate
                        + Config.COL_REC_DLMT +
                        "VALUETYPE"
                        + Config.COL_REC_DLMT +
                        item.ValueType
                        + Config.COL_REC_DLMT +
                        "AMOUNTVAL"
                        + Config.COL_REC_DLMT +
                        item.AmountVal
                        + Config.COL_REC_DLMT +
                        "PERCENTVAL"
                        + Config.COL_REC_DLMT +
                        item.PercentVal
                        + Config.COL_REC_DLMT +
                        "MINTRANSAMOUNT"
                        + Config.COL_REC_DLMT +
                        item.MinTransAmount
                        + Config.COL_REC_DLMT +
                        "MAXTRANSAMOUNT"
                        + Config.COL_REC_DLMT +
                        item.MaxTransAmout
                        + Config.COL_REC_DLMT +
                        "ISELIGIBLE"
                        + Config.COL_REC_DLMT +
                        item.IsEligible
                        + Config.COL_REC_DLMT +
                        "REMAINQUANTITY"
                        + Config.COL_REC_DLMT +
                        item.RemainQuantity
                        + Config.COL_REC_DLMT +
                        "MAXQUANTITY"
                        + Config.COL_REC_DLMT +
                        item.MaxQuantity
                        + Config.COL_REC_DLMT +
                        "REALAMOUT"
                        + Config.COL_REC_DLMT +
                        (Double.Parse(amount) - item.AmountVal).ToString()
                        + Config.COL_REC_DLMT
                        + Config.ROW_REC_DLMT;
            }

            list_voucher = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

        }
        
        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        resultStr = resultStr.Replace("{ERR_DESC}", "SUCCESSFULL");
        resultStr = resultStr.Replace("{CIF}", cifNo);
        resultStr = resultStr.Replace("{LIST_VOUCHER}", list_voucher);

        return resultStr;
    }

    public static List<EbankVoucherBaseModel> GetVoucherListAPIs(string cif, string channelId, string tranType, string tranAmount)
    {
        List<EbankVoucherBaseModel> responseData = new List<EbankVoucherBaseModel>();
        string strBaseURL = Funcs.getConfigVal("BASE_URL_EVOUCHER");
        string URL_SelectAllActive = "/api/ebankvoucher/list?customerId=" + cif + "&channelId="+channelId+"&tranType="+tranType+"&tranAmount=" + tranAmount;
        string strBearerToken = Funcs.getConfigVal("BEARER_TOKEN_EVOUCHER");
        CallApiService<EbankVoucherBaseModel> _VoucherCallApiService = new CallApiService<EbankVoucherBaseModel>(strBearerToken,"");

        try
        {
            Funcs.WriteLog("custid:" + cif + "|GetVoucherListAPIs REQ channelId: " + channelId + "|tranType: " + tranType + "|tranAmount: " + tranAmount);

            ResponseList<EbankVoucherBaseModel> objResponse = _VoucherCallApiService.GetListAPI(strBaseURL, URL_SelectAllActive);

            if (objResponse != null)
            {
                responseData = objResponse.Data;

                Funcs.WriteLog("custid:" + cif + "|GetVoucherListAPIs RES: " + JsonConvert.SerializeObject(objResponse));
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + cif + "|GetVoucherListAPIs RES: ERROR" + ex);
            return null;
        }

        return responseData;
    }

    public static bool UpdateVoucher(EbankVoucherUpdateModel objUpdate, string UserName)
    {
        bool checkUpdate = false;
        string strBaseURL = Funcs.getConfigVal("BASE_URL_EVOUCHER");
        string updateString = JsonConvert.SerializeObject(objUpdate);
        string URL_UpdateVoucher = "/api/ebankvoucher/update?update=" + updateString;
        string strBearerToken = Funcs.getConfigVal("BEARER_TOKEN_EVOUCHER");
        CallApiService<ResponseModelOracle> _VoucherCallApiService = new CallApiService<ResponseModelOracle>(strBearerToken, UserName);

        try
        {
            Funcs.WriteLog("custid:" + objUpdate.CustomerId + "|UpdateVoucher REQ: " + JsonConvert.SerializeObject(objUpdate));

            Response<ResponseModelOracle> objResponse = _VoucherCallApiService.GetAPI(strBaseURL, URL_UpdateVoucher);

            if (objResponse != null && objResponse.Data != null)
            {
                checkUpdate = objResponse.Data.Status.Equals("00") ? true : false;
                Funcs.WriteLog("custid:" + objUpdate.CustomerId + "|UpdateVoucher RES: " + JsonConvert.SerializeObject(objResponse));
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + objUpdate.CustomerId + "|UpdateVoucher RES: ERROR" + ex);
            return false;
        }
        
        return checkUpdate;
    }

    public static bool CheckVoucher(EbankVoucherCheckModel objCheckVoucher, string UserName)
    {
        bool checkVoucher = false;
        string strBaseURL = Funcs.getConfigVal("BASE_URL_EVOUCHER");
        string checkString = JsonConvert.SerializeObject(objCheckVoucher);
        string URL_CheckVoucher = "/api/ebankvoucher/checkvoucher?check=" + checkString;
        string strBearerToken = Funcs.getConfigVal("BEARER_TOKEN_EVOUCHER");
        CallApiService<ResponseModelOracle> _VoucherCallApiService = new CallApiService<ResponseModelOracle>(strBearerToken,UserName);
        try
        {
            Funcs.WriteLog("custid:" + objCheckVoucher.CustomerId + "|CheckVoucher REQ: " + JsonConvert.SerializeObject(objCheckVoucher));

            Response<ResponseModelOracle> objResponse = _VoucherCallApiService.GetAPI(strBaseURL, URL_CheckVoucher);

            if (objResponse != null && objResponse.Data != null)
            {
                Funcs.WriteLog("custid:" + objCheckVoucher.CustomerId + "|CheckVoucher RES: " + JsonConvert.SerializeObject(objResponse));
                checkVoucher = objResponse.Data.Status.Equals("00") ? true : false;
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + objCheckVoucher.CustomerId + "|CheckVoucher RES: ERROR" + ex);
            return false;
        }

        return checkVoucher;
    }
}