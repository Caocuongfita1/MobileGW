using iBanking.CustomModels;
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
public class SavingDAO
{
    public SavingDAO()
    {
        //
        // TODO: Add constructor logic here
        //
    }
	
    public SavingMenuRESModel GetSavingMenu(string custId, SavingMenuREQModel objQuery, string key, ref string resCode, ref string resDescVn, ref string resDescEn)
    {
        SavingMenuRESModel results = new SavingMenuRESModel();

        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_ACCT_NICE_API_EBANK");

            CallApiService<SavingMenuRESModel> _NewsCallApiService = new CallApiService<SavingMenuRESModel>(Funcs.getConfigVal("TOKEN_ACCT_NICE_API"), "");

            results = _NewsCallApiService.PostAPIClassT(BaseUrl, ApiSaving.URL_SAVING_MENU, objQuery, key);

            Funcs.WriteLog("CIF_NO: " + custId + "|LOGS API GetSavingMenu: " + JsonConvert.SerializeObject(results));

            try
            {
                if (results != null && results.status.Equals("0") && results.statusDetail != null && results.statusDetail.respCode.Equals("0"))
                {
                    resCode = Config.ERR_CODE_DONE;
                }
                else
                {
                    resCode = results.status;
                    resDescVn = results.statusDetail.respDescVn;
                    resDescEn = results.statusDetail.respDescEn;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|ERROR LOGS API GetSavingMenu: " + ex.ToString());
                resCode = Config.ERR_CODE_GENERAL;
                return null;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("CIF_NO: " + custId + "|EXCEPTION LOGS API GetSavingMenu: " + ex.ToString());
            resCode = Config.ERR_CODE_GENERAL;
            return null;
        }

        return results;
    }
}