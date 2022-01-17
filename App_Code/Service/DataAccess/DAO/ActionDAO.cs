using iBanking.Common;
using mobileGW.Service.DataAccess;
using mobileGW.Service.Framework;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ActionDAO
/// </summary>
public class ActionDAO
{
    //Funcs.WriteLog("GET_LIMIT_CARD EXCEPTION FROM ESB: " + ex.ToString());
    private List<LoginFailModel> lstLoginFail;
    public List<LoginFailModel> getLoginFail(String username)
    {
        try
        {
            lstLoginFail = new List<LoginFailModel>();
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PUSERNAME", username, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            lstLoginFail = (List<LoginFailModel>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<LoginFailModel>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.getLoginFail", dynParams);
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("getLoginFail: " + ex.Message.ToString());
        }
        return lstLoginFail;
    }

    public CustomerModel GetCustomerInfo(string custId, string channelId)
    {
        CustomerModel customerModel = new CustomerModel();
        try
        {


            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCUSTID", custId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PCHANNEL_ID", channelId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            dynParams.Add("out_cur", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            customerModel = (CustomerModel)new ConnectionFactory(Config.gEBANKConnstr)
                                .ExecuteData<CustomerModel>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.GET_USER_INFO", dynParams).First();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GetCustomerInfo: " + ex.Message.ToString());
        }

        return customerModel;
    }

    public RetCode CheckUserInfo(string custId, string channelId, string value)
    {
        RetCode retCode = new RetCode();
        try
        {


            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCUSTID", custId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PCHANNEL_ID", channelId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PVALUE", value, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            retCode = (RetCode)new ConnectionFactory(Config.gEBANKConnstr)
                               .ExecuteData<RetCode>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.CHECK_USER_INFO", dynParams).First();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CheckUserInfo: " + ex.Message.ToString());
        }

        return retCode;
    }


    public string ValidateUser(string username, string password)
    {
        bool resultReturn = false;
        string pwdSalt = string.Empty;
        List<LoginFailModel> lstLoginFail = new ActionDAO().getLoginFail_reg(username.Trim());
        string status_rs = "";
        if (lstLoginFail.Count > 0)
        {
            int count = 0;
            foreach (var item in lstLoginFail)
            {
                if (item.STATUS.Trim().Equals("FAILED"))
                {
                    count++;
                }
            }
            if (count >= 5)
            {
                status_rs = "10";
                return status_rs;
            }
        }


        List<Users> userTable = new ActionDAO().GetClientUser(username, password, ref pwdSalt);
        string CUSTID = "";

        //truong hop co duy nhat 1 ban ghi duy nhat
        if (userTable != null && userTable.Count > 0)
        {
            CUSTID = userTable[0].CUSTID;
            resultReturn = true;
        }

        if (resultReturn == false)
        {

            new ActionDAO().insAction("MOB", string.Empty, "LOGIN_REGISTER_MB", username, "",
                   "", "FAILED", "LOGIN KHONG THANH CONG", string.Empty, string.Empty, string.Empty, string.Empty,
                   string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                   string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                   string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                   string.Empty, string.Empty, 0, string.Empty);

            status_rs = "99";
        }
        else
        {
            status_rs = "00";

        }
        return status_rs + "|"  + CUSTID;
    }

    public List<Users> GetClientUser(String username, String hashPassword, ref string pwdSalt)
    {
        List<Users> users = new List<Users>();
        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCUSTID", username, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PNET_PWDMD5", hashPassword, dbType: OracleDbType.Varchar2, direction: ParameterDirection.InputOutput);
            dynParams.Add("PCHANNEL_ID", "MOB", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            users = (List<Users>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<Users>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.GET_USER_BY_USER_PWD", dynParams);

            pwdSalt = dynParams.Get<object>("PNET_PWDMD5").ToString();
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("GetClientUser: " + ex.Message.ToString());
        }
        return users;
    }

    public string GET_AUTH_METHOD_NET(String CUSTID)
    {
        List<Users> users = new List<Users>();
        string auth_method = "";
        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCIF_NO", CUSTID, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            users = (List<Users>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<Users>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.GET_AUTH_METHOD_NET", dynParams);

            if (users.Count > 0)
            {
                auth_method = users[0].AUTH_METHOD;
            }

        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("GET_AUTH_METHOD_NET: " + ex.Message.ToString());
        }
        return auth_method;
    }

    public List<Users> GET_INFO_USER_NET(String CUSTID)
    {
        List<Users> users = new List<Users>();
        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCIF_NO", CUSTID, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            users = (List<Users>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<Users>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.GET_AUTH_METHOD_NET", dynParams);

        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("GET_INFO_USER_NET: " + ex.Message.ToString());
        }
        return users;
    }

    public RetCode insAction(string channelId, string modCd, string tranType, string custid, string ip, string userAgent,
            string status, string txDesc, string lastChange, string BM1, string BM2, string BM3, string BM4, string BM5,
            string BM6, string BM7, string BM8, string BM9, string BM10, string BM11, string BM12, string BM13, string BM14,
            string BM15, string BM16, string BM17, string BM18, string BM19, string BM20, string BM21, string BM22, string BM23,
            string BM24, string BM25, string BM26, string BM27, string BM28, string BM29, int isProcessed, string timeProcessed)
    {
        RetCode retCode = new RetCode();
        try
        {


            var dynParams = new OracleDynamicParameters();
            dynParams.Add("pCHANNEL_ID", channelId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pMOD_CD", modCd, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTRAN_TYPE", tranType, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCUSTID", custid, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pIP", ip, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pUSER_AGENT", userAgent, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pSTATUS", status, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pTXDESC", txDesc, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pLAST_CHANGE", lastChange, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM1", BM1, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM2", BM2, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM3", BM3, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM4", BM4, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM5", BM5, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM6", BM6, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM7", BM7, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM8", BM8, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM9", BM9, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM10", BM10, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM11", BM11, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM12", BM12, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM13", BM13, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM14", BM14, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM15", BM15, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM16", BM16, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM17", BM17, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM18", BM18, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM19", BM19, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM20", BM20, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM21", BM21, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM22", BM22, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM23", BM23, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM24", BM24, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM25", BM25, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM26", BM26, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM27", BM27, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM28", BM28, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pBM29", BM29, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            dynParams.Add("pIS_PROCESSED", isProcessed, dbType: OracleDbType.Int16, direction: ParameterDirection.Input);
            dynParams.Add("pTIME_PROCESSED", timeProcessed, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            retCode = new ConnectionFactory(Config.gEBANKConnstr)
                            .ExecuteData<RetCode>(CommandType.StoredProcedure, "pkg_ebank_fraud.INS_TBL_EB_ACTION", dynParams).First();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("insAction: " + ex.Message.ToString());
        }

        return retCode;
    }

    public List<LoginFailModel> getLoginFail_reg(String username)
    {
        try
        {
            lstLoginFail = new List<LoginFailModel>();
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PUSERNAME", username, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            lstLoginFail = (List<LoginFailModel>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<LoginFailModel>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.getLoginFail_reg", dynParams);
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("getLoginFail_reg: " + ex.Message.ToString());
        }
        return lstLoginFail;
    }

    public string getSMS_CODE(String trans_id)
    {
        string Confirm_code = "";
        try
        {
            List<TBL_EB_TRAN_SMS_CODE> lst_smsCode = new List<TBL_EB_TRAN_SMS_CODE>();
            lst_smsCode = (List<TBL_EB_TRAN_SMS_CODE>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<TBL_EB_TRAN_SMS_CODE>(CommandType.Text, "select SMSCODE from tbl_eb_tran where TRAN_ID = " + trans_id);
            if (lst_smsCode.Count > 0)
            {
                Confirm_code = lst_smsCode[0].SMSCODE;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("getSMS_CODE: " + ex.Message.ToString());
        }
        return Confirm_code;
    }

    public double InsertCustomerShbMobileSetting(Users userSession, string otpOrEsecure)
    {
        Transfers tf = new Transfers();
        DataTable eb_tran = new DataTable();
        double eb_tran_id = 0;
        try
        {
            eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "SETTING" //mod_cd
                    , "MOB_REG_MOB" //tran_type
                    , userSession.CUSTID //custid
                    , ""//src_acct
                    , "" //des_acct
                    , 0 //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , 0 //lcy_amount
                    , "" //txdesc
                    , "110000" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , Int32.Parse(userSession.AUTH_METHOD) //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , otpOrEsecure //sms code
                    , "" //sign data
                    , "" //core err code
                    , "" //core err desc
                    , "" //core ref_no
                    , "" //core txdate
                    , "" //core txtime
                    , 0 // order_amount
                    , 0 // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , "" //bm1
                    , "" //bm2
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , "" //bm7
                    , "" //bm8
                    , "" //bm9
                    , "" //bm10
                    , "" //bm11
                    , "" //bm12
                    , "" //bm13
                    , "" //bm14
                    , "" //bm15
                    , "" //bm16
                    , "" //bm17
                    , "" //bm18
                    , "" //bm19
                    , "" //bm20
                    , "" //bm21
                    , "" //bm22
                    , "" //bm23
                    , "" //bm24
                    , "" //bm25
                    , "" //bm26
                    , "" //bm27
                    , "" //bm28
                    , ""//bm29                   
                );
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("InsertCustomerShbMobileSetting: " + ex.Message.ToString());
        }

        if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
        {
            eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
        }

        return eb_tran_id;
    }

    public static List<RetCode> CHECK_ESECURE(string pPOS, string pCode, string serial, out bool isException)
    {
        List<RetCode> retCode = new List<RetCode>();
        isException = false;

        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("pSERIAL", serial, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCODE", pCode, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pPOS", pPOS, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            retCode = (List<RetCode>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<RetCode>(CommandType.StoredProcedure, "PKG_IBANKING_NEW.CHECK_ESECURE", dynParams);

        }
        catch (Exception ex)
        {
            isException = true;
            Funcs.WriteLog("CHECK_ESECURE: " + ex.Message.ToString());

        }
        return retCode;
    }

    public RetCode UpdateUserMobile(string custId, decimal curAuthMethod, decimal newAuthMethod, string curMobileNo, string newMobileNo,
      string orgPwdClear, string branchCode, string auth_method_name)
    {

        RetCode retCode = new RetCode();
        try
        {


            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCUSTID", custId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PCUR_AUTH_METHOD", curAuthMethod, dbType: OracleDbType.Int32, direction: ParameterDirection.Input);
            dynParams.Add("PNEW_AUTH_METHOD", newAuthMethod, dbType: OracleDbType.Int32, direction: ParameterDirection.Input);
            dynParams.Add("PCUR_MOBILE_NO", curMobileNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PNEW_MOBILE_NO", newMobileNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PORG_PWD_CLEAR", orgPwdClear, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PBRANCH_CODE", branchCode, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PTYPE", auth_method_name, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            retCode = (RetCode)new ConnectionFactory(Config.gEBANKConnstr)
                                .ExecuteData<RetCode>(CommandType.StoredProcedure, "PKG_IBANKING_630.UPT_USER_INFO_MOB", dynParams).First();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("UpdateUserMobile: " + ex.Message.ToString());
        }

        return retCode;
    }

    public RetCode UpdateUserMobileToken(string custId, decimal curAuthMethod, decimal newAuthMethod, string curMobileNo, string newMobileNo,string branchCode,
        string LIMIT_AMOUNT, string LIMIT_AMOUNT_INTER, string LIMIT_AMOUNT_STOCK
        )
    {

        RetCode retCode = new RetCode();
        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCUSTID", custId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PCUR_AUTH_METHOD", curAuthMethod, dbType: OracleDbType.Int32, direction: ParameterDirection.Input);
            dynParams.Add("PNEW_AUTH_METHOD", newAuthMethod, dbType: OracleDbType.Int32, direction: ParameterDirection.Input);
            dynParams.Add("PCUR_MOBILE_NO", curMobileNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PNEW_MOBILE_NO", newMobileNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PBRANCH_CODE", branchCode, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            dynParams.Add("pLIMIT_AMT_INTRA", LIMIT_AMOUNT, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pLIMIT_AMT_INTER", LIMIT_AMOUNT_INTER, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pLIMIT_AMT_STOCK", LIMIT_AMOUNT_STOCK, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            retCode = (RetCode)new ConnectionFactory(Config.gEBANKConnstr)
                                .ExecuteData<RetCode>(CommandType.StoredProcedure, "PKG_IBANKING_630.UPT_USER_INFO_MOB_1", dynParams).First();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("UpdateUserMobileToken: " + ex.Message.ToString());
        }

        return retCode;
    }

    public EBANK_LIMIT_GROUP GET_LIMIT_GROUP(string name, string channel_id)
    {

        EBANK_LIMIT_GROUP retCode = new EBANK_LIMIT_GROUP();
        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("v_name", name, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("v_channel_id", channel_id, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            dynParams.Add("v_cur", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            retCode = (EBANK_LIMIT_GROUP)new ConnectionFactory(Config.gEBANKConnstr)
                                .ExecuteData<EBANK_LIMIT_GROUP>(CommandType.StoredProcedure, "PKG_EBANKMGR.GET_LIMIT_GROUP", dynParams).First();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GET_LIMIT_GROUP: " + ex.Message.ToString());
        }

        return retCode;
    }

    public List<Users> GET_INFO_USER_MOB(String CUSTID)
    {
        List<Users> users = new List<Users>();
        try
        {
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("PCIF_NO", CUSTID, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            users = (List<Users>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<Users>(CommandType.StoredProcedure, "PKG_IBANKING_630.GET_AUTH_METHOD_MOB", dynParams);

        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("GET_INFO_USER_MOB: " + ex.Message.ToString());
        }
        return users;
    }
}