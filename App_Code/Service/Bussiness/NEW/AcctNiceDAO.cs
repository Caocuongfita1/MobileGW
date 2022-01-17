using mobileGW.Service.API;
using mobileGW.Service.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AcctNiceDAO
/// </summary>
public class AcctNiceDAO
{
    public AcctNiceDAO()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string Check_Ebank_Login(AcctNiceCheckLogin objQuery)
    {
        string response = String.Empty;
        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_ACCT_NICE_API_EBANK");

            CallApiService<string> _NewsCallApiService = new CallApiService<string>(Funcs.getConfigVal("TOKEN_ACCT_NICE_API"),"");

            ResponseListNew<string> objResponse = _NewsCallApiService.PostAPIList(BaseUrl, ApiAcctNice.URL_CHECK_LOGIN, objQuery);
            
            if (objResponse != null && objResponse.status != null && objResponse.status.Equals("0"))
            {
                response = objResponse.jwtToken;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("LOGS API GetListAcctNices: " + ex.ToString());
        }

        return response;
    }

    /// <summary>
    /// Get list Promotions by mobile
    /// </summary>
    /// <param name="mobileNum"></param>
    /// <returns></returns>
    public List<AcctNiceItemModel> GetListAcctNices(string custId, AcctNiceQueryList objQuery, string key, ref string resCode, ref string resDescVn, ref string resDescEn)
    {
        List<AcctNiceItemModel> listAcctNices = new List<AcctNiceItemModel>();
        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_ACCT_NICE_API_CORE");

            CallApiService<AcctNiceItemModel> _NewsCallApiService = new CallApiService<AcctNiceItemModel>(Funcs.getConfigVal("TOKEN_ACCT_NICE_API"), "");

            ResponseListNew<AcctNiceItemModel> objResponse = _NewsCallApiService.PostAPIList(BaseUrl, ApiAcctNice.URL_GET_ACCT_NICE_LIST, objQuery, key);

            Funcs.WriteFileLog("CIF_NO: " + custId + "|LOGS API GetListAcctNices: " + JsonConvert.SerializeObject(objResponse));

            if (objResponse != null && objResponse.status != null && objResponse.status.Equals("0"))
            {
                listAcctNices = objResponse.listAccountNices;
                resCode = Config.ERR_CODE_DONE;
            }
            else
            {
                if (objResponse != null && objResponse.status != null && !objResponse.status.Equals("0") && objResponse.statusDetail != null)
                {
                    resCode = objResponse.statusDetail.respCode;
                    resDescVn = objResponse.statusDetail.respDescVn;
                    resDescEn = objResponse.statusDetail.respDescEn;
                }
                else
                {
                    resCode = Config.ERR_CODE_GENERAL;
                }
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("LOGS API GetListAcctNices: " + ex.ToString());
        }

        return listAcctNices;
    }

    /// <summary>
    /// Get list Promotions by mobile
    /// </summary>
    /// <param name="mobileNum"></param>
    /// <returns></returns>
    public bool CreateAcctNice(string custId,AcctNiceCreateModel objQuery, ref string errCode, ref string errDescVn, ref string errDescEn, ref string core_ref_no, string key = null)
    {
        bool isDone = false;
        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_ACCT_NICE_API_CORE");

            CallApiService<string> _NewsCallApiService = new CallApiService<string>(Funcs.getConfigVal("TOKEN_ACCT_NICE_API"),"");

            ResponseListNew<string> objResponse = _NewsCallApiService.PostAPIList(BaseUrl, ApiAcctNice.URL_CREATE_ACCT, objQuery, key);

            Funcs.WriteFileLog("CIF_NO: " + custId + "|LOGS API GetListAcctNices: " + JsonConvert.SerializeObject(objResponse));

            if (objResponse != null && objResponse.status != null && objResponse.status.Equals("0"))
            {
                isDone = true;
                core_ref_no = objResponse.refNo;
                errCode = Config.ERR_CODE_DONE;
            }
            else
            {
                isDone = false;
                if (objResponse != null && objResponse.status != null && !objResponse.status.Equals("0") && objResponse.statusDetail != null)
                {
                    errCode = objResponse.statusDetail.respCode;
                    errDescVn = objResponse.statusDetail.respDescVn;
                    errDescEn = objResponse.statusDetail.respDescEn;
                }
                else
                {
                    errCode = Config.ERR_CODE_GENERAL;
                }
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("LOGS API CreateAcctNice: " + ex.ToString());
            isDone = false;
        }

        return isDone;
    }
}