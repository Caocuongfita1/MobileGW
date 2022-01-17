using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using mobileGW;
using mobileGW.Service.Bussiness;
using mobileGW.Service.Framework;
using System.Threading;
using mobileGW.Service.DataAccess;
using System.Collections.Generic;
using System.Globalization;

namespace mobileGW.Service.AppFuncs
{
    /// <summary>
    /// Summary description for Auth
    /// </summary>
    public class Auth
    {
        public Auth()
        {

        }

        public static string ACTIVE_MOB(String mobile_no, String pwd, string ip, string user_agent)
        {
            #region "CMD ACTIVE_MOB"

            //Funcs.WriteLog("mobile_no:" + mobile_no + "|ACTIVE_MOB BEGIN: PWD" + pwd);
            Funcs.WriteLog("mobile_no:" + mobile_no + "|ACTIVE_MOB BEGIN");

            String retStr = Config.ACTIVE_MOB;
            string err = "";
            string custname = "";
            string auth_method = "";
            string default_acct = "";
            string custid = "";
            string active_code = "";


            try
            {

                //MD5 + salt
                //pwd = Funcs.encryptMD5(pwd + mobile_no);
                // goi ham check login 
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                DataSet ds = da.CHECK_LOGIN_AND_ACTIVE(mobile_no, pwd);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                    active_code = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNELs.ACTIVE_CODE].ToString();

                    custname = "";
                    auth_method = "";
                    default_acct = "";

                //if ((err != Config.ERR_CODE_INVALID_ACTIVE_CODE))
                if ((err == Config.ERR_CODE_DONE))
                {

                    custname = ds.Tables[0].Rows[0]["CUSTNAME"].ToString();
                    //linhtn add new 20/11/2016
                    //lay them custid de insert tbl-action
                    custid = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUSTID].ToString();

                    auth_method = ds.Tables[0].Rows[0]["AUTH_METHOD"].ToString();

                    default_acct = ds.Tables[0].Rows[0]["DEFAULT_ACCT"] == DBNull.Value ? "_NULL_" : ds.Tables[0].Rows[0]["DEFAULT_ACCT"].ToString();


                    String SMSContent = Config.gSMSCodeMsg_Active;
                    SMSContent = SMSContent.Replace("[ACTIVE_CODE]", active_code);

                    //Gui SMS
                    mobile_no = SmsIntergration.convertToNewMobile(mobile_no);
                    Funcs.WriteLog("mobile_no:" + mobile_no + "|ACTIVE_MOB begin insert sms content:" + SMSContent);
                    //Utility ut = new Utility();
                    //bool iSendSMS = ut.INSERT_SMS(0, Config.gSMS_CENTER, Config.HO_BR_CODE, mobile_no, SMSContent, 0, "SHB", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), 1, 1, 0);

                    bool iSendSMS = new SmsIntergration().sendOTP(custid, mobile_no, SMSContent, Funcs.getConn("SMS_PARTNER"), 0);

                    Funcs.WriteLog("mobile_no:" + mobile_no + "|ACTIVE_MOB end insert sms content:" + SMSContent + "|ret:"+ iSendSMS.ToString());
                    if (iSendSMS)
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);

                        retStr = retStr.Replace("{CUST_NAME}", custname);

                        retStr = retStr.Replace("{DEFAULT_ACCT}", default_acct);

                        retStr = retStr.Replace("{AUTH_METHOD}", auth_method);

                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "ACTIVE CODE GENERATED SUCCESSFULL");
                        //anhnd2
                        //06/07/2016
                        //Debug only                            
                            retStr = retStr + "=" + "*";
                    }
                    else
                    {
                        err = Config.ERR_CODE_GENERAL;
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "COULD NOT SENT ACTIVE CODE VIA SMS");
                    }
                }    
                else
                {
                        err = Config.ERR_CODE_INVALID_ACTIVE_CODE;
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_ACTIVE_CODE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "ACTIVE INFORMATION INVALID");
                }
                //giai phong bo nho: 
                    ds = null;
                }
                else
                {
                        err = Config.ERR_CODE_INVALID_ACTIVE_CODE;
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_ACTIVE_CODE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "ACTIVE INFORMATION INVALID");
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                err = Config.ERR_CODE_GENERAL;
                retStr = Config.ERR_MSG_GENERAL;
            }

            #region "INSERT_EB_ACTION"
            //linhtn add new 20/11/2016
            //insert to TBL_EB_ACTION
            Utility utAction = new Utility();
            bool utActionRet = false;
            if (err == Config.ERR_CODE_DONE) // kich hoat thanh cong
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
                   (Config.ChannelID
                   , "" //mod_cd
                   , Config.EB_ACTION_GET_ACTIVE_CODE //lay ma kich hoat thanh cong
                   , custid
                   , ip
                   , user_agent
                   , Config.EB_ACTION_DONE
                   , "LAY MA KICH HOAT THANH CONG"
                   , mobile_no// //BM1 --truong hop lay ma kich hoat thanh cong luu so dien thoai vao bm1
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , "" //BM29
                   , 0 //IS_PROCESSED
                   , ""
                   );
            }// end if 
            else
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
               (Config.ChannelID
               , "" //mod_cd
               , Config.EB_ACTION_GET_ACTIVE_CODE
               , mobile_no // truong hop khong lay duoc ma kich hoat --> khong tim duoc custid -->luu so dien thoai
               , ip
               , user_agent
               , Config.EB_ACTION_FAILED
               , "LAY MA KICH HOAT KHONG THANH CONG"
               , mobile_no //BM1
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , "" //BM29
               , 0 //IS_PROCESSED
               , ""
               );
            }
            #endregion "INSERT_EB_ACTION"

            Funcs.WriteLog("mobile_no:" + mobile_no + "|ACTIVE_MOB END");

            return retStr;
            #endregion "CMD ACTIVE_MOB"

        }

        public static string ACTIVE_MOB_CONFIRM(String mobile_no, String mob_active_code, string ip, string user_agent)
        {
            #region "CMD ACTIVE_MOB_CONFIRM"
            string retStr = Config.ACTIVE_MOB_CONFIRM;
            string require_pwd_change = "0";
            string token = "";
            string custid = "";
            string err = "";

            Funcs.WriteLog("mobile_no:" + mobile_no + "|ACTIVE_MOB_CONFIRM BEGIN ");

            try
            {
                // goi ham check login 
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                DataSet ds = da.ACTIVE_CONFIRM(mobile_no, mob_active_code);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                    if (err == Config.ERR_CODE_INVALID_ACTIVE_CODE)
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_ACTIVE_CODE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "ACTIVE INFORMATION INVALID");
                    }
                    else if (err == Config.ERR_CODE_GENERAL)
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                    }
                    else
                    {
                        custid = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUSTID].ToString();
                        require_pwd_change = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.REQ_PWD_CHANGE].ToString();
                        token = GET_MOBILE_TOKEN(custid, Config.ChannelID, ip, user_agent);
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "ACTIVE SUCCESSFULL");
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{REQ_PWD_CHANGE}", require_pwd_change);
                        retStr = retStr.Replace("{TOKEN}", token);
                        retStr = retStr.Replace("{EMAIL}", ds.Tables[0].Rows[0]["EMAIL"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["EMAIL"].ToString());
                    }
                }
                else
                {
                    err = Config.ERR_CODE_INVALID_ACTIVE_CODE;
                    retStr = Config.ERR_MSG_GENERAL;
                }
                if (da != null) da.Dispose();
                ds = null;


            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                err = Config.ERR_CODE_GENERAL;
                retStr = Config.ERR_MSG_GENERAL;
            }

            #region "INSERT_EB_ACTION"
            // linhtn: add new 20/11/2016
            // insert tbl_eb_action
            Utility utAction = new Utility();
            bool utActionRet = false;
            if (err == Config.ERR_CODE_DONE) // doi mat khau thanh cong
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
                   (Config.ChannelID
                   , "" //mod_cd
                   , Config.EB_ACTION_ACTIVE_CONFIRM
                   , custid
                   , ip
                   , user_agent
                   , Config.EB_ACTION_DONE
                   , "ACTIVE THANH CONG"
                   , "" //BM1
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , "" //BM29
                   , 0 //IS_PROCESSED
                   , ""
                   );
            }// end if doi mat khau thanh cong
            else //doi mat khau khong thanh cong
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
               (Config.ChannelID
               , "" //mod_cd
               , Config.EB_ACTION_ACTIVE_CONFIRM
               , mobile_no
               , ip
               , user_agent
               , Config.EB_ACTION_FAILED
               , "ACTIVE KHONG THANH CONG"
               , "" //BM1
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , "" //BM29
               , 0 //IS_PROCESSED
               , ""
               );
            }

            #endregion "INSERT_EB_ACTION"


            Funcs.WriteLog("mobile_no:" + mobile_no + "|ACTIVE_MOB_CONFIRM END ");

            return retStr;
            #endregion "CMD ACTIVE_MOB_CONFIRM"
        }

        public static string CHECK_LOGIN(String mob_user, String pwd, String active_code, string ip, string user_agent)
        {
            #region "CMD CHECK_LOGIN"
            String retStr = Config.CHECK_LOGIN;

            Funcs.WriteLog("custid:" + mob_user + "|CHECK_LOGIN BEGIN");


            string require_pwd_change = "0";
            string is_actived = "0";
            string token = "";
            //LINHTN ADD NEW 20161001: THEM AUTH_METHOD DE CHECK MBASIC
            string auth_method = "0";
            string custid = "";
            string err = "";
            //TUANNM10 ADD
            string DATE_EXP_PWD = "";//ngay het han password

            try
            {
                require_pwd_change = "0";
                is_actived = "0";
                token = "";
                //LINHTN ADD NEW 20161001: THEM AUTH_METHOD DE CHECK MBASIC
                auth_method = "0";
                DATE_EXP_PWD = "";

                // goi ham check login 
                DataSet ds = new DataSet();
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                ds = da.GET_USER_BY_USER_PWD(mob_user, pwd);
                if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                {

                     auth_method = ds.Tables[0].Rows[0]["AUTH_METHOD"].ToString();

                    if (ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.AUTH_INFO_EXT2].ToString() != active_code)
                    {
                        retStr = Config.ERR_MSG_INVALID_ACTIVE_CODE;
                        if (ds != null) ds.Dispose();
                        return retStr;
                    }
                    require_pwd_change = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.REQ_PWD_CHANGE].ToString();
                    DATE_EXP_PWD = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.BM3].ToString();
                    DateTime parsedDate = DateTime.ParseExact(DATE_EXP_PWD, "dd/MM/yyyy", null);
                    DATE_EXP_PWD = parsedDate.AddYears(1).ToString("dd/MM/yyyy");
                    //anhnd2  05/01/2017 Them sdt tra ve.
                   string  auth_info_ext1 = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.AUTH_INFO_EXT1].ToString();


                    //anhnd2 
                    //15.05.2015
                    is_actived = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.IS_ACTIVATED].ToString();

                    if (is_actived == "0")
                    {
                        retStr = Config.ERR_MSG_INVALID_ACTIVE_CODE;
                        if (ds != null) ds.Dispose();
                        return retStr;
                    }

                    token = GET_MOBILE_TOKEN(mob_user, Config.ChannelID, ip, user_agent);

                    if (token == Config.ERR_CODE_GENERAL)
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        if (ds != null) ds.Dispose();
                        return retStr;
                    }

					
                    // Reset FPtoken moi khi dang nhap lai bang pwd
                    TBL_EB_FINGERPRINTs fg = new TBL_EB_FINGERPRINTs();
                    bool temp = fg.RESET_USER_FP_TOKEN(mob_user, token);
                    if (fg != null) fg.Dispose();
					
                    err = Config.ERR_CODE_DONE;
                    //DUNG TAM TRUONG MOB_CLIENT
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGIN SUCCESSFULL");
                    custid =  ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUSTID].ToString();
                    retStr = retStr.Replace("{CIF_NO}", custid);

                    retStr = retStr.Replace("{AUTH_METHOD}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.AUTH_METHOD].ToString());

                    //(ds.Tables[0].Rows[j][CARD.TAD] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.TAD].ToString()) + Config.COL_REC_DLMT + //= PURCHASE_DEBIT

                    retStr = retStr.Replace("{DEFAULT_ACCT}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_ACCT]==DBNull.Value? "_NULL_": ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_ACCT].ToString());

                    retStr = retStr.Replace("{DEFAULT_LANG}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_LANG] == DBNull.Value ? "_NULL_" : ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_LANG].ToString());

                    retStr = retStr.Replace("{CUST_NAME}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUSTNAME].ToString());


                    retStr = retStr.Replace("{REQ_PWD_CHANGE}", require_pwd_change);
                    //retStr = retStr.Replace("{REQ_PWD_CHANGE}", "1");
                    retStr = retStr.Replace("{DATE_EXP_PWD}", DATE_EXP_PWD);
                    //retStr = retStr.Replace("{DATE_EXP_PWD}", "16/05/2018");
                    //parsedDate = DateTime.ParseExact(DATE_EXP_PWD, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DATE_EXP_PWD = parsedDate.AddYears(1).ToString();
                    //anhnd2  05/01/2017 Them sdt tra ve.
                    retStr = retStr.Replace("{AUTH_INFO_EXT1}", auth_info_ext1);

                    retStr = retStr.Replace("{TOKEN}", token);
                    retStr = retStr.Replace("{FP_TOKEN}", token);
                    retStr = retStr.Replace("{EMAIL}", ds.Tables[0].Rows[0]["EMAIL"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["EMAIL"].ToString());
                }
                else
                {
                    err = Config.ERR_CODE_GENERAL;
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_LOGIN);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGIN FAIL");
                }
                //giai phong bo nho: 
                if (ds != null) ds.Dispose();
                ds = null;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                err = Config.ERR_CODE_GENERAL;
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }

            #region "INSERT_EB_ACTION"
            // linhtn: add new 20/11/2016
            // insert tbl_eb_action
            Utility utAction = new Utility();
            bool utActionRet = false;
            if (err == Config.ERR_CODE_DONE) // doi mat khau thanh cong
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
                   (Config.ChannelID
                   , "" //mod_cd
                   , Config.EB_ACTION_LOGIN
                   , custid
                   , ip
                   , user_agent
                   , Config.EB_ACTION_DONE
                   , "LOGIN MOBILE THANH CONG"
                   , "" //BM1
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , "" //BM29
                   , 0 //IS_PROCESSED
                   , ""
                   );
            }// end if
            else //
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
               (Config.ChannelID
               , "" //mod_cd
               , Config.EB_ACTION_LOGIN
               , mob_user //login khong thanh cong
               , ip
               , user_agent
               , Config.EB_ACTION_FAILED
               , "LOGIN KHONG THANH CONG"
               , "" //BM1
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , "" //BM29
               , 0 //IS_PROCESSED
               , ""
               );
            }

            #endregion "INSERT_EB_ACTION"


            Funcs.WriteLog("custid:" + mob_user + "|CHECK_LOGIN END");

            return retStr;
            #endregion "CMD CHECK_LOGIN"
        }

        public static string LOGIN_FP(Hashtable hashTbl, String ip, String user_agent)
        {
            #region "CMD LOGIN_FP"
            String retStr = Config.CHECK_LOGIN;
            string require_pwd_change = "0";
            string DATE_EXP_PWD = "";
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string setup_type = Funcs.getValFromHashtbl(hashTbl, "TYPE");
            string device_id = Funcs.getValFromHashtbl(hashTbl, "UUID");
            string device_type = Funcs.getValFromHashtbl(hashTbl, "DEVICE_TYPE");
            string fp_token = Funcs.getValFromHashtbl(hashTbl, "FP_TOKEN");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "PWD");
            string token = "";
            //LINHTN ADD NEW 20161001: THEM AUTH_METHOD DE CHECK MBASIC
            string auth_method = "0";
            string err = "";
            string is_actived = "";
            string active_code = Funcs.getValFromHashtbl(hashTbl, "ACTIVE_CODE");
            Funcs.WriteLog("custid:" + custid + "|LOGIN_FP BEGIN");
            try
            {
                require_pwd_change = "0";
                DATE_EXP_PWD = "";
                is_actived = "0";
                token = "";
                //LINHTN ADD NEW 20161001: THEM AUTH_METHOD DE CHECK MBASIC
                auth_method = "0";

                // goi ham check login 
                DataSet ds = new DataSet();
                TBL_EB_FINGERPRINTs da = new TBL_EB_FINGERPRINTs();
                ds = da.GET_USER_BY_USER_FP(custid, pwd, setup_type, device_id, device_type, fp_token, ip, user_agent);
                if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                {
                    try
                    {
                        auth_method = ds.Tables[0].Rows[0]["AUTH_METHOD"].ToString();
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("CIF_NO: " + custid + "|ERROR: "+ ex.ToString());
                        return retStr = Config.ERR_MSG_INVALID_TOKEN;
                    }
                    

                    if (ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.AUTH_INFO_EXT2].ToString() != active_code)
                    {
                        retStr = Config.ERR_MSG_INVALID_ACTIVE_CODE;
                        if (ds != null) ds.Dispose();
                        return retStr;
                    }
                    require_pwd_change = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.REQ_PWD_CHANGE].ToString();
                    DATE_EXP_PWD = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.BM3].ToString();
                    DateTime parsedDate = DateTime.ParseExact(DATE_EXP_PWD, "dd/MM/yyyy", null);
                    DATE_EXP_PWD = parsedDate.AddYears(1).ToString("dd/MM/yyyy");
                    //anhnd2  05/01/2017 Them sdt tra ve.
                    string auth_info_ext1 = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.AUTH_INFO_EXT1].ToString();


                    //anhnd2 
                    //15.05.2015
                    is_actived = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.IS_ACTIVATED].ToString();

                    if (is_actived == "0")
                    {
                        retStr = Config.ERR_MSG_INVALID_ACTIVE_CODE;
                        if (ds != null) ds.Dispose();
                        return retStr;
                    }
                    fp_token = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.FP_TOKEN].ToString();
                    token = GET_MOBILE_TOKEN(custid, Config.ChannelID, ip, user_agent);

                    if (token == Config.ERR_CODE_GENERAL)
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        if (ds != null) ds.Dispose();
                        return retStr;
                    }

                    err = Config.ERR_CODE_DONE;
                    //DUNG TAM TRUONG MOB_CLIENT
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGIN SUCCESSFULL");
                    custid = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUSTID].ToString();
                    retStr = retStr.Replace("{CIF_NO}", custid);

                    retStr = retStr.Replace("{AUTH_METHOD}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.AUTH_METHOD].ToString());

                    //(ds.Tables[0].Rows[j][CARD.TAD] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.TAD].ToString()) + Config.COL_REC_DLMT + //= PURCHASE_DEBIT

                    retStr = retStr.Replace("{DEFAULT_ACCT}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_ACCT] == DBNull.Value ? "_NULL_" : ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_ACCT].ToString());

                    retStr = retStr.Replace("{DEFAULT_LANG}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_LANG] == DBNull.Value ? "_NULL_" : ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_LANG].ToString());

                    retStr = retStr.Replace("{CUST_NAME}", ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUSTNAME].ToString());


                    retStr = retStr.Replace("{REQ_PWD_CHANGE}", require_pwd_change);
                    retStr = retStr.Replace("{DATE_EXP_PWD}", DATE_EXP_PWD);
                    //parsedDate = DateTime.ParseExact(DATE_EXP_PWD, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DATE_EXP_PWD = parsedDate.AddYears(1).ToString();
                    //anhnd2  05/01/2017 Them sdt tra ve.
                    retStr = retStr.Replace("{AUTH_INFO_EXT1}", auth_info_ext1);

                    retStr = retStr.Replace("{TOKEN}", token);
                    retStr = retStr.Replace("{FP_TOKEN}", fp_token);
                    retStr = retStr.Replace("{EMAIL}", ds.Tables[0].Rows[0]["EMAIL"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["EMAIL"].ToString());
                }
                else
                {
                    err = Config.ERR_CODE_GENERAL;
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_LOGIN);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGIN FAIL");
                }
                //giai phong bo nho: 
                if (ds != null) ds.Dispose();
                ds = null;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                err = Config.ERR_CODE_GENERAL;
                retStr = Config.ERR_MSG_INVALID_TOKEN;
                //return retStr;
            }

            #region "INSERT_EB_ACTION"
            // linhtn: add new 20/11/2016
            // insert tbl_eb_action
            Utility utAction = new Utility();
            bool utActionRet = false;
            if (err == Config.ERR_CODE_DONE) // doi mat khau thanh cong
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
                   (Config.ChannelID
                   , "" //mod_cd
                   , Config.EB_ACTION_LOGIN_FP
                   , custid
                   , ip
                   , user_agent
                   , Config.EB_ACTION_DONE
                   , "LOGIN FINGER PRINT THANH CONG"
                   , "" //BM1
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , "" //BM29
                   , 0 //IS_PROCESSED
                   , ""
                   );
            }// end if
            else //
            {
                utActionRet = utAction.INS_TBL_EB_ACTION
               (Config.ChannelID
               , "" //mod_cd
               , Config.EB_ACTION_LOGIN_FP
               , custid //login khong thanh cong
               , ip
               , user_agent
               , Config.EB_ACTION_FAILED
               , "LOGIN FINGER PRINT KHONG THANH CONG"
               , "" //BM1
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , ""
               , "" //BM29
               , 0 //IS_PROCESSED
               , ""
               );
            }

            #endregion "INSERT_EB_ACTION"


            Funcs.WriteLog("custid:" + custid + "|CHECK_LOGIN END");

            return retStr;
            #endregion "CMD LOGIN_FP""
        }

        public static string LOGOUT(Hashtable hashTbl, String ip, String user_agent)
        {
            String retStr = Config.LOGOUT;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");
            Funcs.WriteLog("custid:" + custid + "|LOGOUT BEGIN");
            try
            {

                TBL_EB_FINGERPRINTs da = new TBL_EB_FINGERPRINTs();
                if (da.LOGOUT_AND_EXPIRE_TOKEN(custid, token))
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGOUT SUCCESSFULL");
                }
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_LOGIN);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGOUT FAIL");
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_LOGIN);
                retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGOUT FAIL");

            }

            return retStr;
        }

        public static string CHECK_TOKEN(String cif, String token)
        {
            #region "CMD CHECK_TOKEN"
            String err = "";
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            DataSet ds = new DataSet();
             String received_time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            try
            {
                ds = da.CHECK_TOKEN(cif, Config.ChannelID, token, received_time);

                //Thread.Sleep(5000);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                     err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                else
                    err = Config.ERR_CODE_TOKEN_INVALID;
                if (da != null) da.Dispose();
                ds = null;
            }
            catch (Exception ex)
            {
               Funcs.WriteLog(ex.ToString());
               err = Config.ERR_CODE_GENERAL;
               return err;
            }
            return err;
            #endregion "CMD CHECK_LOGIN"
        }

        public static string GET_MOBILE_TOKEN(String cif, String channel_id, string ip, string user_agent)
        {
            #region "CMD GET_MOBILE_TOKEN"
            String err = "";
            string token = "";
            String received_time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            try
            {
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                DataSet ds = da.GET_MOBILE_TOKEN(cif, Config.ChannelID, received_time, ip, user_agent);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                    if (err == Config.ERR_CODE_DONE)
                    {
                        token = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNELs.TOKEN].ToString();
                        return token;
                    }
                    else
                        err = Config.ERR_CODE_GENERAL;
                        
                }
                return err;

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                err = Config.ERR_CODE_GENERAL;
                return err;
            }
            #endregion "CMD GET_MOBILE_TOKEN"
        }

        //public static string CHANGE_PWD(String mob_user, String cur_pwd, String new_pwd)
        public static string CHANGE_PWD(String mob_user, String cur_pwd, String new_pwd, string ip, string user_agent)
        {
            #region "CMD CHANGE_PWD"
            String retStr = Config.CHANGE_PWD;

            Funcs.WriteLog("custid:" + mob_user + "|CHANGE_PWD BEGIN");

            try
            {
                string err = "";
                DataSet ds = new DataSet();
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                ds = da.GET_USER_BY_USER_PWD(mob_user, cur_pwd);

                if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                {
                    DataSet ds1 = new DataSet();
                    ds1 = da.UPDATE_PWD_BY_USER_PWD(mob_user, new_pwd);

                    //retStr = ret.Tables[0].Rows[0]["ret"].ToString();

                    if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
                    {
                        err = ds1.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                    }
                    else
                    {
                        err = Config.ERR_CODE_INVALID_LOGIN;
                    }


                    //retStr = ret.Tables[0].Rows[0]["ret"].ToString();

                    if (err == Config.ERR_CODE_DONE)
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGIN SUCCESSFUL");
                    }
                    else
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_LOGIN);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGIN FAIL");
                    }

                }
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_LOGIN);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LOGIN FAIL");
                }

                #region "INSERT_EB_ACTION"
                // linhtn: add new 20/11/2016
                // insert tbl_eb_action
                Utility utAction = new Utility();
                bool utActionRet = false;
                if (err == Config.ERR_CODE_DONE) // doi mat khau thanh cong
                {
                    utActionRet = utAction.INS_TBL_EB_ACTION
                       (Config.ChannelID
                       , "" //mod_cd
                       , Config.EB_ACTION_CHANGE_PWD
                       , mob_user
                       , ip
                       , user_agent
                       , Config.EB_ACTION_DONE
                       , "DOI MAT KHAU THANH CONG"
                       , "" //BM1
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , ""
                       , "" //BM29
                       , 0 //IS_PROCESSED
                       , ""
                       );
                }// end if doi mat khau thanh cong
                else //doi mat khau khong thanh cong
                {
                    utActionRet = utAction.INS_TBL_EB_ACTION
                   (Config.ChannelID
                   , "" //mod_cd
                   , Config.EB_ACTION_CHANGE_PWD
                   , mob_user
                   , ip
                   , user_agent
                   , Config.EB_ACTION_FAILED
                   , "DOI MAT KHAU KHONG THANH CONG"
                   , "" //BM1
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , ""
                   , "" //BM29
                   , 0 //IS_PROCESSED
                   , ""
                   );
                }

                #endregion "INSERT_EB_ACTION"


            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }

            Funcs.WriteLog("custid:" + mob_user + "|CHANGE_PWD END");



            return retStr;
            #endregion "CMD CHANGE_PWD"
        }

        public static bool isVitualMoney(string description)
        {
            description = description.ToLower().Trim();
            bool flag = false;
            List<string> lstMoney = new List<string>();
            try
            {
                lstMoney = (List<string>)new ConnectionFactory(Config.gEBANKConnstr)
                                 .GetItems<string>(CommandType.Text, "select * from VITUAL_MONEY where 1 = 1");
                var arr = description.Split(' ');
                /*foreach (var item in lstMoney)
                {
                    for (var i = 0; i < arr.Length; i++)
                    {
                        if (item.ToLower().ToString() == arr[i])
                        {
                            flag = true;
                        }
                    }

                }*/
                foreach (var item in lstMoney)
                {
                    if (description.Contains(item.ToLower()))
                    {
                        flag = true;
                    }

                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public static string CHECK_BEFORE_TRANSACTION(String mob_user, String src_acct, String des_acct, double amount, String pwd, String trans_type,String txtDesc)
        {
            bool isVirtualMoney = Auth.isVitualMoney(txtDesc);
            Funcs.WriteLog("mob_user:" + mob_user + "|isVirtualMoney: " + isVirtualMoney);
            if (isVirtualMoney)
            {
                string retErrVirtualMoney = string.Format("ERR_CODE#{0}|ERR_DESC_VI#{1}|ERR_DESC_EN#{2}"
                    , "90", LanguageConfig.ErrorVirtualMoneyVi, LanguageConfig.ErrorVirtualMoneyEn);
                Funcs.WriteLog("mob_user:" + mob_user + "|retErrVirtualMoney: " + retErrVirtualMoney);
                return retErrVirtualMoney;
            }
            return CHECK_BEFORE_TRANSACTION(mob_user, src_acct, des_acct, amount, pwd, trans_type);
        }
        public static string CHECK_BEFORE_TRANSACTION(String mob_user, String src_acct, String des_acct, double amount, String pwd, String trans_type)
        {
            #region "CMD CHECK_BEFORE_TRANSACTION"

            string retStr = Config.ERR_CODE_DONE;
            string limitCard = "";
            string limitAcc = "";
            string limitDomestic = "";

            //check mat khau
            DataSet ds = new DataSet();
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            ds = da.GET_USER_BY_USER_PWD(mob_user, pwd);

            //begin if check mat khau
            if (ds != null && ds.Tables[0].Rows.Count >0)
            {
                Utils utils = new Utils();
                string retStrLimit = utils.GetLimitTransactionBy(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUSTID].ToString(), Config.ChannelID, "DOMESTIC_ACC");
                Hashtable hashTblCheck = Funcs.stringToHashtbl(retStrLimit);

                if (Funcs.getValFromHashtbl(hashTblCheck, "ERR_CODE").Equals(Config.ERR_CODE_GENERAL))
                {
                    return Config.ERR_MSG_GENERAL;
                }
                else
                {
                    limitCard = Funcs.getValFromHashtbl(hashTblCheck, "LIMIT_247CARD");
                    limitAcc = Funcs.getValFromHashtbl(hashTblCheck, "LIMIT_247AC");
                    limitDomestic = Funcs.getValFromHashtbl(hashTblCheck, "LIMIT_DOMESTIC");
                }

                switch (trans_type)
                {
                    case  Config.TRAN_TYPE_INTRA:
                    case Config.TRAN_TYPE_TOPUP_MOBILE:
                    case Config.TRAN_TYPE_TOPUP_OTHER:
                    case Config.TRAN_TYPE_BILL_MOBILE:
                    case Config.TRAN_TYPE_BILL_OTHER:
                    case Config.TRAN_TYPE_BATCH:
                    case Config.TRAN_TYPE_QR_PAYMENT:
                    case Config.TRAN_TYPE_TOPUP_SHS:
                    case Config.TRAN_TYPE_TOPUP_SHBS:
                    case Config.TRAN_TYPE_AUTO_DEBIT:
                    case Config.TRAN_TYPE_GIVE_GIFT:
                        if (amount + double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()) > double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()))
                        {
                            retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                            Funcs.WriteLog("CIF:" + mob_user
                            + "|amount:" + amount
                            + "curr_amt_intra:" + ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()
                            + "limit_amt_intra:" + ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()
                            + "ERR_MSG_VIOLATE_LIMIT");

                        }

                        break;
                    case Config.TRAN_TYPE_DOMESTIC:
                    case Config.TRAN_TYPE_ACQ_247AC:
                    case Config.TRAN_TYPE_ACQ_247CARD:
                        //if ((amount > Config.AMOUNT_LIMIT_247_PERTRAN) && ((trans_type == Config.TRAN_TYPE_ACQ_247AC) 
                        //    || (trans_type == Config.TRAN_TYPE_ACQ_247CARD)))
                        if(((amount > double.Parse(limitAcc)) && (trans_type == Config.TRAN_TYPE_ACQ_247AC))
                           || ((amount > double.Parse(limitCard)) && (trans_type == Config.TRAN_TYPE_ACQ_247CARD))
                           || ((amount > double.Parse(limitDomestic)) && (trans_type == Config.TRAN_TYPE_DOMESTIC))
                           )
                        {
                                retStr = Config.ERR_MSG_VIOLATE_LIMIT_PERTRAN;
                                Funcs.WriteLog("CIF:" + mob_user
                                + "|amount:" + amount
                                + "ERR_MSG_VIOLATE_LIMIT PERTRAN");
                        }                       
                        else 
                        {
                                if (amount + double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString())
                                > double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()))
                            { 
                                    //check them han muc tren 1 giao dich
                                    retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                                Funcs.WriteLog("CIF:" + mob_user
                                + "|amount:" + amount
                                + "curr_amt_inter:" + ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString()
                                + "limit_amt_inter:" + ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()
                                + "ERR_MSG_VIOLATE_LIMIT");
                            }
                        }

                        break;
                       

                    //case Config.TRAN_TYPE_TOPUP_SHS:
                    //case Config.TRAN_TYPE_TOPUP_SHBS:
                    //     if (amount + +double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_STOCK].ToString())
                    //         > double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_STOCK].ToString()))
                    //    {
                    //        retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                    //        Funcs.WriteLog("CIF:" + mob_user
                    //        + "|amount:" + amount
                    //        + "curr_amt_stock:" + ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_STOCK].ToString()
                    //        + "limit_amt_stock:" + ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_STOCK].ToString()
                    //        + "ERR_MSG_VIOLATE_LIMIT");
                    //    }
                    //     break;
                    default:
                        // You can use the default case.                      
                       break;
                };
                
          
            }//end if check mat khau
            else //mat khau khong hop le
            {
                retStr = Config.ERR_MSG_INVALID_TRANPWD;
                //giai phong bo nho
                if (da != null) da.Dispose();
                ds = null;
                return retStr;
            }


            //CHECK HAN MUC GIAO DICH --> CHI CHECK HAN MUC VOI CAC GIAO DICH KHAC SELF TRANSFER
            
            ////CHECK MULTI CURRENCY
            //Utility objUti = new Utility();
            //DataTable dtMulti_ccy = new DataTable();
            //dtMulti_ccy = objUti.CHECK_MULTI_CURRENCY(src_acct, des_acct);
            //if (dtMulti_ccy != null && dtMulti_ccy.Rows.Count > 0 && trans_type == Config.TRAN_TYPE_INTRA)
            //{
            //    if (dtMulti_ccy.Rows[0][0].ToString().ToUpper() == "N")
            //    {
            //        retStr = Config.ERR_MSG_INVALID_CARD_NO;
            //    }
            //}
            //else if ((ds == null) || (ds.Tables[0].Rows.Count == 0))
            //{
            //    retStr = Config.ERR_MSG_INVALID_TRANPWD;
            //    //giai phong bo nho
            //    if (da != null) da.Dispose();
            //    ds = null;
            //    return retStr;
            //}

            //giai phong bo nho
            if (ds != null) ds.Dispose();
            ds = null;
            return retStr;

            #endregion "CMD CHECK_BEFORE_TRANSACTION"
        }
        
        //linhtn 11/2016
        // Ktra tai khoan chuyen va CIF co trung nhau khong.
        public static bool CustIdMatchScrAcct(string custid, string acctno)
        {
            bool result = false;
            try
            {
                result = CoreIntegration.CheckAccountBelongCif(custid, acctno, "CASA");
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            Funcs.WriteLog("CHECK MATCH ACCOUNT :" + acctno + " WITH CUSTID: " + custid + "|RESULT = " + result.ToString());
            return result;
        }



        //linhtn 11/2016
        //Kiem tra so so tiet kiem DEPOSIT belong CIF.
        public static bool CheckDepositBelongCIF(string custid, string deposit_no)
        {
            bool result = false;
            try
            {
                result = CoreIntegration.CheckAccountBelongCif(custid, deposit_no, "DEPOSIT");
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            Funcs.WriteLog("CHECK MATCH DEPOSIT_NO :" + deposit_no + " WITH CUSTID: " + custid + "|RESULT = " + result.ToString());
            return result;
        }



        //linhtn 02/2017
        //Kiem tra so the masking (cardno masking) belong CIF.
        public static bool CheckCardBelongCIF(string custid, string cardno)
        {
            bool result = false;
            try
            {
                result = CardIntegration.CheckCardBelongCif(custid, cardno);
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            Funcs.WriteLog("CHECK MATCH CARDNO :" + cardno + " WITH CUSTID: " + custid + "|RESULT = " + result.ToString());
            return result;
        }


    }
}