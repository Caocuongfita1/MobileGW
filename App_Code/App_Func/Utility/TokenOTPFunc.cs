using mobileGW.Service.AppFuncs;
using mobileGW.Service.Bussiness;
using mobileGW.Service.DataAccess;
using mobileGW.Service.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for TokenOTP
/// </summary>
public class TokenOTPFunc
{
    public string GetActivationCode(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.RESPONE_GET_ACTIVATION_CODE;

        string branchID = "";
        string cifNumber = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string optionChangePackage = Funcs.getValFromHashtbl(hashTbl, "OPTION");
        string customerName = "";
        string customerTypeID = "0"; //KHCN: 0, KHDN: 1
        string email = "";
        string phoneNumber = "";
        string userName = "";
        string changeStatus = String.Empty;
        string mobile_no = Funcs.getValFromHashtbl(hashTbl, "MOBILE_NO");
        string position = Funcs.getValFromHashtbl(hashTbl, "POSITION");
        //string pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "PWD") + cifNumber);
        string pwd = Funcs.getValFromHashtbl(hashTbl, "PWD");
        pwd = Funcs.encryptMD5(pwd + cifNumber);

        if (position.Equals("OUTSIDE"))
        {
            DataSet ds = new DataSet();
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            ds = da.GET_USER_BY_MOBILE(mobile_no);
            if (ds != null && (ds.Tables[0].Rows.Count > 0))
            {
                //kich hoat token nhung phuong thuc xac thuc khong phai la mToken : 5
                if (!ds.Tables[0].Rows[0]["AUTH_METHOD"].ToString().Equals(Config.TypeMToken))
                {
                    return Config.ERR_NOT_REGIS_MTOKEN;
                }

                cifNumber = ds.Tables[0].Rows[0]["CUSTID"].ToString();
                
            }
            else
            {
                return Config.ERR_NOT_REGIS_MTOKEN;
            }
        }
        //TRUONG HOP INSIDE
        else
        {
            
            DataSet ds = new DataSet();
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            ds = da.GET_USER_BY_USER_PWD(cifNumber, pwd);
            if (ds == null || ds.Tables[0].Rows.Count <= 0)
            {
                return Config.ERR_LOGIN_MTOKEN;
                Funcs.WriteLog("GetActivationCode: " + cifNumber + "PWD ERROR");
            }

            mobile_no = ds.Tables[0].Rows[0]["AUTH_INFO_EXT1"].ToString();
        }

        //CASE OUTSIDE DEFAULT LA NOTCHANGE, INSIDE THI DUA VAO AUTH_METHOD DE TRUYEN CHANGE(=5) OR NOTCHANGE
        if (optionChangePackage.Equals("CHANGE"))
        {
            Utils util = new Utils();
            changeStatus = util.REGISTER_MTOKEN(cifNumber);

            if (changeStatus.Split('|')[0].Equals("E00"))
            {
                retStr = retStr.Replace("{AUTH_METHOD}", Config.TypeMToken);
            }
        }

        if (!optionChangePackage.Equals("CHANGE") || changeStatus.Split('|')[0].Equals("E00"))
        {
            TokenOTP.GetActivationCodeResType res = null;
            try
            {
                //LAY ACTIVATIONCODE API
                res = TokenOTPIntegration.GetActivationCode(branchID, cifNumber, customerName, customerTypeID, email, phoneNumber, cifNumber, userName);

                if (res != null && res.RespSts.Sts.Equals("0"))
                {

                    string SMSContent = Config.gSMSCodeMsg_Active_Token;
                    SMSContent = SMSContent.Replace("[ACTIVE_CODE]", res.activationCode);
                    bool iSendSMS = new SmsIntergration().sendOTP(cifNumber, mobile_no, SMSContent, Funcs.getConn("SMS_PARTNER"), 0);
                    //bool iSendSMS = true;
                    if (iSendSMS)
                    {
                        Funcs.WriteLog("GetActivationCode: " + cifNumber + "SEND ACTIVE CODE VIA SMS SUCCESS, mobile_num: " + mobile_no);
                        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{ERR_DESC}", res.message);
                        retStr = retStr.Replace("{ACTIVATION_CODE}", "");
                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("GetActivationCode: " + cifNumber + "SEND  ACTIVE CODE VIA SMS FAIL, mobile_num: " + mobile_no);
                        return Config.ERR_MSG_GENERAL;
                    }
                }
                //GOI KEYPASS API NHUNG TRA LOI
                if (res != null && !res.RespSts.Sts.Equals("0"))
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                    retStr = retStr.Replace("{ERR_DESC}", "RESPONSECODE:" + res.responseCode + "-" + res.message);
                    retStr = retStr.Replace("{ACTIVATION_CODE}", "-1");
                    return retStr;
                }

                return Config.ERR_MSG_GENERAL;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("GetActivationCode: " + cifNumber + "ERROR Call Esb" + ex.Message);
                return Config.ERR_MSG_GENERAL;
            }
        }
        else
        {
            Funcs.WriteLog("GetActivationCode: " + cifNumber + "|Loi trong qua trinh doi goi sang MTOKEN");
            return Config.ERR_MSG_GENERAL;
        }


    }

    //public string VerifyOTPCR(Hashtable hashTbl, string ip, string userAgent)
    //{
    //    string retStr = Config.ERR_MSG_GENERAL;

    //    string cifNumber = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
    //    string otp = Funcs.getValFromHashtbl(hashTbl, "OTP");
    //    string transactionID = Funcs.getValFromHashtbl(hashTbl, "TRANSACTIONID");

    //    TokenOTP.VerifyOTPCRResType res = null;
    //    try
    //    {
    //        res = TokenOTPIntegration.VerifyOTPCR(cifNumber, transactionID, otp);

    //        if (res != null && res.RespSts.Sts.Equals("0"))
    //        {
    //            return Config.SUCCESS_MSG_GENERAL;
    //        }
    //        else
    //        {
    //            if (res.responseCode.Equals("17"))
    //            {
    //                return String.Format(Config.ERR_MSG_FORMAT, res.responseCode, res.message);
    //            }

    //            return Config.ERR_MSG_GENERAL;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Funcs.WriteLog("VerifyOTPCR: " + cifNumber + "ERROR Call Esb" + ex.Message);
    //        return Config.ERR_MSG_GENERAL;
    //    }
    //}

    //public string CreateTransaction(Hashtable hashTbl, string ip, string userAgent)
    //{
    //    string retStr = Config.RESPONE_CREATE_TRANSACTION;
    //    string cifNumber = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
    //    string transactionID = "";

    //    TokenOTP.CreateTransactionResType res = null;
    //    try
    //    {
    //        res = TokenOTPIntegration.CreateTransaction(cifNumber, transactionID);

    //        if (res != null && res.RespSts.Sts.Equals("0"))
    //        {
    //            retStr = retStr.Replace("{ERR_CODE}", Config.SUCCESS_MSG_GENERAL);
    //            retStr = retStr.Replace("{ERR_DESC}", res.message);
    //            retStr = retStr.Replace("{USERID}", res.userID);
    //            retStr = retStr.Replace("{TRANSACTIONID}", res.transactionID);
    //            retStr = retStr.Replace("{CHALLENGE}", res.challenge);
    //            return Config.SUCCESS_MSG_GENERAL;
    //        }

    //        if (res != null && !res.RespSts.Sts.Equals("0"))
    //        {
    //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
    //            retStr = retStr.Replace("{ERR_DESC}", "RESPONSECODE:" + res.responseCode + "-" + res.message);
    //            retStr = retStr.Replace("{USERID}", res.userID);
    //            retStr = retStr.Replace("{TRANSACTIONID}", res.transactionID);
    //            retStr = retStr.Replace("{CHALLENGE}", res.challenge);
    //            return Config.SUCCESS_MSG_GENERAL;
    //        }
    //        else
    //        {
    //            return Config.ERR_MSG_GENERAL;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Funcs.WriteLog("VerifyOTPCR: " + cifNumber + "ERROR Call Esb" + ex.Message);
    //        return Config.ERR_MSG_GENERAL;
    //    }
    //}

    public string SynchronizeOTP(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.ERR_MSG_GENERAL;
        string cifNumber = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string operatorID = Funcs.getValFromHashtbl(hashTbl, "OPERATORID");
        string otp1 = Funcs.getValFromHashtbl(hashTbl, "OTP1");
        string otp2 = Funcs.getValFromHashtbl(hashTbl, "OTP2");

        TokenOTP.SynchronizeOTPResType res = null;
        try
        {
            res = TokenOTPIntegration.SynchronizeOTP(cifNumber, operatorID, otp1, otp2);

            if (res != null && res.RespSts.Sts.Equals("0"))
            {
                return Config.SUCCESS_MSG_GENERAL;
            }
            else
            {
                return Config.ERR_MSG_GENERAL;
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("VerifyOTPCR: " + cifNumber + "ERROR Call Esb" + ex.Message);
            return Config.ERR_MSG_GENERAL;
        }
    }

    //REQ=CMD#GET_OPT|CIF_NO#{CIF_NO}|TOKEN#{TOKEN}|MOBILE_NO#{MOBILE_NO}|TYPE_OTP#{TYPE_OTP}
    static Hashtable GetRequestOTPById(Hashtable hashTbl, string ip, string user_agent)
    {
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string mobileNo = "";
        string typeOtpParam = Funcs.getValFromHashtbl(hashTbl, TBL_EB_TOKEN_OTP.TYPE_OTP);

        int typeOtp = 0;
        Int32.TryParse(typeOtpParam, out typeOtp);
        string tokenOtp = null;
        OtpDA otpDa = new OtpDA();
        DataTable dt = new DataTable();
        try
        {
            switch (typeOtp)
            {
                case 4: //XAC THUC BANG SMS OTP CHO KENH MOB
                    double expireTimeInSecond = 120;
                    tokenOtp = Funcs.getAlphabets(6);//Goi service send SMS OTP

                    //Insert OTP vao DB

                    TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                    DataSet ds = new DataSet();
                    ds = da.GET_USER_BY_CIF(cifNo);

                    if (ds != null && ds.Tables[0] != null)
                    {
                        //dt = ds.Tables[0];
                        mobileNo = ds.Tables[0].Rows[0]["AUTH_INFO_EXT1"].ToString();

                        dt = otpDa.insertTokenOtp(cifNo, tokenOtp, typeOtp, expireTimeInSecond, 0);
                        if ((dt != null) && (dt.Rows.Count > 0))
                        {
                            double request_id = double.Parse(dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString());
                            hashTbl.Add(TBL_EB_TOKEN_OTP.REQUEST_ID, Convert.ToString(request_id));
                            string requestId = Convert.ToString(request_id);
                            string tranNo = String.Format("GD{0}", requestId.PadLeft(8, '0'));
                            string smsContent = String.Format(Config.gFormatSmsOtp, tranNo, tokenOtp);

                            bool iSendSMS = new SmsIntergration().sendOTP(cifNo, mobileNo, smsContent, Funcs.getConn("SMS_PARTNER"), 0);

                            Funcs.WriteLog("mob_user:" + cifNo + "|mobileNo: " + mobileNo + "|Get OTP AUTH_METHOD:" + typeOtp.ToString() + "|Status: " + iSendSMS);
                        }
                    }
                    break;
                case 5:
                    // TRUONG HOP GET QUESTION TU SOFT TOKEN
                    TokenOTP.CreateTransactionResType res = null;

                    dt = otpDa.insertTokenOtp(cifNo, "_NULL_", typeOtp, 0.0, 0);

                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        try
                        {
                            //CREATETRANSACTION API
                            res = TokenOTPIntegration.CreateTransaction(cifNo, dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString());

                            if (res != null && res.RespSts.Sts.Equals("0"))
                            {
                                double request_id = double.Parse(dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString());
                                hashTbl.Add(TBL_EB_TOKEN_OTP.REQUEST_ID, Convert.ToString(request_id));
                                string requestId = Convert.ToString(request_id);
                                string tranNo = String.Format("GD{0}", requestId.PadLeft(8, '0'));
                                Funcs.WriteLog("mob_user:" + cifNo + "|challengeCode: " + res.challenge);
                                hashTbl.Add("CHALLENGE_CODE", res.challenge);
                                dt = otpDa.updateTokenOtp(dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString(), res.challenge);
                            }
                            else
                            {
                                //GOI API NHUNG BI LOI KHONG DONG BO
                                if (res.responseCode.Equals("17"))
                                {
                                    //return  res.responseCode;
                                    hashTbl.Add("ERR_CODE", res.responseCode);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Funcs.WriteLog("Exception: " + e.Message);
                            object a = e.Message;
                            Funcs.WriteLog("Lỗi Hàm GetRequestOTPById: " + a);
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("Exception: " + ex.Message);
        }
        
        return hashTbl;
    }

    public static String CheckBeforeTransaction(String transtype, double amount, String CifNo, String inputTokenOtp, String requestId, int typeOtp, String txtDesc)
    {
        bool isVirtualMoney = Auth.isVitualMoney(txtDesc);
        Funcs.WriteLog("mob_user:" + CifNo + "|isVirtualMoney: " + isVirtualMoney);
        if (isVirtualMoney)
        {
            string retErrVirtualMoney = string.Format("ERR_CODE#{0}|ERR_DESC_VI#{1}|ERR_DESC_EN#{2}"
                , "90", LanguageConfig.ErrorVirtualMoneyVi, LanguageConfig.ErrorVirtualMoneyEn);
            Funcs.WriteLog("mob_user:" + CifNo + "|retErrVirtualMoney: " + retErrVirtualMoney);
            return retErrVirtualMoney;
        }
        return CheckBeforeTransaction(transtype, amount, CifNo, inputTokenOtp, requestId, typeOtp);
    }
    public static String CheckBeforeTransaction(String transtype, double amount, String CifNo, String inputTokenOtp, String requestId, int typeOtp)
    {
        string retStr = Config.ERR_CODE_GENERAL;
        string limitCard = "";
        string limitAcc = "";
        string limitDomestic = "";
        OtpDA otpDa = new OtpDA();
        Funcs.WriteLog("CIF:" + CifNo + "|CheckOtpBeforeTransaction: BEGIN transtype:" + transtype + "-inputTokenOtp:" + inputTokenOtp);
        DataTable dt = otpDa.checkOtpBeforeTransaction(CifNo, inputTokenOtp, requestId, typeOtp);
        try
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                Funcs.WriteLog("CIF:" + CifNo + "|CheckOtpBeforeTransaction: START");
                if (dt.Columns.Count == 1)
                {
                    retStr = dt.Rows[0][0].ToString();
                    retStr = string.Format(Config.ERR_MSG_FORMAT, retStr, retStr.Equals("11") ? "INVALID OTP" : "OTP TIMEOUT");
                    Funcs.WriteLog("CIF:" + CifNo + "|err retStr : " + retStr);
                }
                else
                {
                    Utils utils = new Utils();
                    string retStrLimit = utils.GetLimitTransactionBy(dt.Rows[0][TBL_EB_USER_CHANNEL.CUSTID].ToString(), Config.ChannelID, "DOMESTIC_ACC");
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

                    switch (transtype)
                    {

                        // case Config.TRAN_TYPE_CHARITY:
                        //case Config.TRAN_TYPE_SCHOOL_FEE:
                        case Config.TRAN_TYPE_INTRA:
                        case Config.TRAN_TYPE_TOPUP_SHS:
                        case Config.TRAN_TYPE_TOPUP_SHBS:
                        case Config.TRAN_TYPE_AUTO_DEBIT:
                        case Config.TRAN_TYPE_GIVE_GIFT:

                            if (amount + double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()) > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()))
                            {
                                retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                                Funcs.WriteLog("CIF:" + CifNo
                                + "|amount:" + amount
                                + "curr_amt_intra:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()
                                + "limit_amt_intra:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()
                                + "ERR_MSG_VIOLATE_LIMIT");

                                return retStr;
                            }

                            break;
                        case Config.TRAN_TYPE_TOPUP_MOBILE:
                        case Config.TRAN_TYPE_TOPUP_OTHER:
                        case Config.TRAN_TYPE_BILL_MOBILE:
                        case Config.TRAN_TYPE_BILL_OTHER:
                        case Config.TRAN_TYPE_BATCH:
                        case Config.TRAN_TYPE_QR_PAYMENT:
                            if (amount + double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()) > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()))
                            {
                                retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                                Funcs.WriteLog("CIF:" + CifNo
                                + "|amount:" + amount
                                + "curr_amt_payment:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()
                                + "limit_amt_payment:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()
                                + "ERR_MSG_VIOLATE_LIMIT");

                                return retStr;
                            }
                            break;
                        case Config.TRAN_TYPE_DOMESTIC:
                        case Config.TRAN_TYPE_ACQ_247AC:
                        case Config.TRAN_TYPE_ACQ_247CARD:

                            //if ((amount > Config.AMOUNT_LIMIT_247_PERTRAN) && ((transtype == Config.TRAN_TYPE_ACQ_247AC) || (transtype == Config.TRAN_TYPE_ACQ_247CARD)))
                            //if (amount + double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString())
                            //> double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()))
                        	if (((amount > double.Parse(limitAcc)) && (transtype == Config.TRAN_TYPE_ACQ_247AC))
                           || ((amount > double.Parse(limitCard)) && (transtype == Config.TRAN_TYPE_ACQ_247CARD))
                           || ((amount > double.Parse(limitDomestic)) && (transtype == Config.TRAN_TYPE_DOMESTIC))
                           )
							{
                                retStr = Config.ERR_MSG_VIOLATE_LIMIT_PERTRAN;
                                Funcs.WriteLog("CIF:" + CifNo
                                + "|amount:" + amount
                                + "ERR_MSG_VIOLATE_LIMIT PERTRAN");

                                return retStr;
                            }
                            else
                            {
                                if (amount + double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString())
                                > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()))
                                {
                                    //check them han muc tren 1 giao dich
                                    retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                                    Funcs.WriteLog("CIF:" + CifNo
                                    + "|amount:" + amount
                                    + "curr_amt_inter:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString()
                                    + "limit_amt_inter:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()
                                    + "ERR_MSG_VIOLATE_LIMIT");

                                    return retStr;
                                }
                            }

                            break;


                        //case Config.TRAN_TYPE_TOPUP_SHS:
                        //case Config.TRAN_TYPE_TOPUP_SHBS:
                        //    if (amount + +double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_STOCK].ToString())
                        //        > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_STOCK].ToString()))
                        //    {
                        //        retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                        //        Funcs.WriteLog("CIF:" + CifNo
                        //        + "|amount:" + amount
                        //        + "curr_amt_stock:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_STOCK].ToString()
                        //        + "limit_amt_stock:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_STOCK].ToString()
                        //        + "ERR_MSG_VIOLATE_LIMIT");
                        //    }
                        //    break;
                        default:
                            // You can use the default case.                      
                            break;
                    };
                    String tokenOtp = dt.Rows[0][TBL_EB_TOKEN_OTP.TOKEN_OTP].ToString().Trim().ToUpper();
                    String countOTP = dt.Rows[0][TBL_EB_TOKEN_OTP.COUNTOTP].ToString();
                    Funcs.WriteLog("CIF:" + CifNo + "|tokenOtp:" + tokenOtp);
                    Funcs.WriteLog("CIF:" + CifNo + "|inputTokenOtp:" + inputTokenOtp);

                    Funcs.WriteLog("CIF:" + CifNo + "|countOTP:" + countOTP);
                    if (!tokenOtp.Equals(inputTokenOtp.Trim().ToUpper()))
                    {
                        retStr = Config.ERR_MSG_INVALID_TRANPWD;
                        if (countOTP.Equals("3"))
                            retStr = Config.ERR_MSG_OVER_OTP;

                        Funcs.WriteLog("CIF:" + CifNo + "|STATUS: FAIL");
                    } else
                    {
                        retStr = Config.ERR_CODE_DONE;
                        Funcs.WriteLog("CIF:" + CifNo + "|STATUS: DONE");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("cif:" + CifNo + "exception-" + ex.Message);
            retStr = Config.ERR_MSG_GENERAL;
        }
        return retStr;
    }

    public static String CheckBeforeTransactionTOKEN(String transtype, double amount, String CifNo, String inputTokenOtp, String requestId, int typeOtp)
    {
        string retStr = Config.ERR_CODE_GENERAL;
        string limitCard = "";
        string limitAcc = "";
        string limitDomestic = "";
        OtpDA otpDa = new OtpDA();
        Funcs.WriteLog("CIF:" + CifNo + "|CheckOtpBeforeTransactionTOKEN: BEGIN transtype:" + transtype + "-inputTokenOtp:" + inputTokenOtp);

        //get Infor by CustId
        TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
        DataSet ds = new DataSet();
        ds = da.GET_USER_BY_CIF(CifNo);
        //ds.Tables[0].Rows[0]["AUTH_METHOD"].ToString();
        DataTable dt = new DataTable();
        if (ds != null && ds.Tables[0] != null)
        {
            dt = ds.Tables[0];
        }

        try
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                Funcs.WriteLog("CIF:" + CifNo + "|CheckOtpBeforeTransactionTOKEN: START");
				
				Utils utils = new Utils();
                string retStrLimit = utils.GetLimitTransactionBy(dt.Rows[0][TBL_EB_USER_CHANNEL.CUSTID].ToString(), Config.ChannelID, "DOMESTIC_ACC");
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
				
                switch (transtype)
                {

                    // case Config.TRAN_TYPE_CHARITY:
                    //case Config.TRAN_TYPE_SCHOOL_FEE:
                    case Config.TRAN_TYPE_INTRA:
                    case Config.TRAN_TYPE_TOPUP_SHS:
                    case Config.TRAN_TYPE_TOPUP_SHBS:
                    case Config.TRAN_TYPE_AUTO_DEBIT:
                    case Config.TRAN_TYPE_ACCT_NICE:
                    case Config.TRAN_TYPE_GIVE_GIFT:

                        if (amount + double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()) > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()))
                        {
                            retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                            Funcs.WriteLog("CIF:" + CifNo
                            + "|amount:" + amount
                            + "curr_amt_intra:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()
                            + "limit_amt_intra:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()
                            + "ERR_MSG_VIOLATE_LIMIT");

                            return retStr;
                        }

                        break;
                    case Config.TRAN_TYPE_TOPUP_MOBILE:
                    case Config.TRAN_TYPE_TOPUP_OTHER:
                    case Config.TRAN_TYPE_BILL_MOBILE:
                    case Config.TRAN_TYPE_BILL_OTHER:
                    case Config.TRAN_TYPE_BATCH:
                    case Config.TRAN_TYPE_QR_PAYMENT:
                        if (amount + double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()) > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()))
                        {
                            retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                            Funcs.WriteLog("CIF:" + CifNo
                            + "|amount:" + amount
                            + "curr_amt_payment:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTRA].ToString()
                            + "limit_amt_payment:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTRA].ToString()
                            + "ERR_MSG_VIOLATE_LIMIT");

                            return retStr;
                        }
                        break;
                    case Config.TRAN_TYPE_DOMESTIC:
                    case Config.TRAN_TYPE_ACQ_247AC:
                    case Config.TRAN_TYPE_ACQ_247CARD:

                        //if ((amount > Config.AMOUNT_LIMIT_247_PERTRAN) && ((transtype == Config.TRAN_TYPE_ACQ_247AC) || (transtype == Config.TRAN_TYPE_ACQ_247CARD)))
                        //if (amount + double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString())
                        //> double.Parse(ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()))
                        if (((amount > double.Parse(limitAcc)) && (transtype == Config.TRAN_TYPE_ACQ_247AC))
                           || ((amount > double.Parse(limitCard)) && (transtype == Config.TRAN_TYPE_ACQ_247CARD))
                           || ((amount > double.Parse(limitDomestic)) && (transtype == Config.TRAN_TYPE_DOMESTIC))
                           )
                        {
                            retStr = Config.ERR_MSG_VIOLATE_LIMIT_PERTRAN;
                            Funcs.WriteLog("CIF:" + CifNo
                            + "|amount:" + amount
                            + "ERR_MSG_VIOLATE_LIMIT PERTRAN");

                            return retStr;
                        }
                        else
                        {
                            if (amount + double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString())
                            > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()))
                            {
                                //check them han muc tren 1 giao dich
                                retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                                Funcs.WriteLog("CIF:" + CifNo
                                + "|amount:" + amount
                                + "curr_amt_inter:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_INTER].ToString()
                                + "limit_amt_inter:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_INTER].ToString()
                                + "ERR_MSG_VIOLATE_LIMIT");

                                return retStr;
                            }
                        }

                        break;


                    //case Config.TRAN_TYPE_TOPUP_SHS:
                    //case Config.TRAN_TYPE_TOPUP_SHBS:
                    //    if (amount + +double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_STOCK].ToString())
                    //        > double.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_STOCK].ToString()))
                    //    {
                    //        retStr = Config.ERR_MSG_VIOLATE_LIMIT;
                    //        Funcs.WriteLog("CIF:" + CifNo
                    //        + "|amount:" + amount
                    //        + "curr_amt_stock:" + dt.Rows[0][TBL_EB_USER_CHANNEL.CUR_AMT_STOCK].ToString()
                    //        + "limit_amt_stock:" + dt.Rows[0][TBL_EB_USER_CHANNEL.LIMIT_AMT_STOCK].ToString()
                    //        + "ERR_MSG_VIOLATE_LIMIT");
                    //    }
                    //    break;
                    default:
                        // You can use the default case.                      
                        break;
                };

                Funcs.WriteLog("CIF:" + CifNo + "|inputTokenOtp:" + inputTokenOtp);

                TokenOTP.VerifyOTPCRResType res = null;

                try
                {
                    //goi verifyOTPCR 
                    res = TokenOTPIntegration.VerifyOTPCR(CifNo, requestId, inputTokenOtp);

                    if (res != null && res.RespSts.Sts.Equals("0"))
                    {
                        retStr = Config.ERR_CODE_DONE;
                        Funcs.WriteLog("CIF:" + CifNo + "|STATUS: DONE");
                    }
                    else
                    {
                        if (res.responseCode.Equals("17"))
                        {
                            retStr = String.Format(Config.ERR_MSG_FORMAT,res.responseCode,res.message);
                            
                        } else
                        {
                            retStr = Config.ERR_TOKEN_MSG_INVALID_TRANPWD;
                        }

                        Funcs.WriteLog("CIF:" + CifNo + "|ERR MSG:" + res.responseCode + "-" + res.message);

                    }
                }
                catch (Exception e)
                {
                    Funcs.WriteLog("Exception: " + e.Message);
                    object a = e.Message;
                    Funcs.WriteLog("Lỗi Hàm VerifyOTPCR: " + a);
                    retStr = Config.ERR_MSG_GENERAL;
                }

                Funcs.WriteLog("CIF:" + CifNo + "|CheckOtpBeforeTransactionTOKEN: DONE");
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("cif:" + CifNo + "exception-" + ex.Message);
            retStr = Config.ERR_MSG_GENERAL;
        }
        return retStr;
    }

    public string GetOtp(Hashtable hashTbl, String ip, String user_agent)
    {
        String retRequest = Config.GET_OTP;
        String cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

        try
        {
            Hashtable requestHastable = GetRequestOTPById(hashTbl, ip, user_agent);

            String typeOtpParam = Funcs.getValFromHashtbl(requestHastable, TBL_EB_TOKEN_OTP.TYPE_OTP);
            String requestId = Funcs.getValFromHashtbl(requestHastable, TBL_EB_TOKEN_OTP.REQUEST_ID);
            

            if (Funcs.getValFromHashtbl(requestHastable, "ERR_CODE").Equals("17"))
            {
                return String.Format(Config.ERR_MSG_FORMAT, "17", "Out of synchronization");
            }

            if (!requestId.Equals(Config.NOT_FOUND))
            {
                retRequest = retRequest.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                retRequest = retRequest.Replace(Config.ERR_DESC_VAL, "SUCCESS");
                
                retRequest = retRequest.Replace("{CIF_NO}", cifNo);
                retRequest = retRequest.Replace("{REQUEST_ID}", requestId);
                retRequest = retRequest.Replace("{TYPE_OTP}", typeOtpParam);

                if (typeOtpParam.Equals(Config.TypeMToken))
                {
                    String challengeCode = Funcs.getValFromHashtbl(requestHastable, "CHALLENGE_CODE");
                    retRequest = retRequest.Replace("{CHALLENGE_CODE}", challengeCode);
                }
                else
                {
                    retRequest = retRequest.Replace("{CHALLENGE_CODE}", "_NULL_");
                }
                return retRequest;
            }
            else
            {
                return Config.ERR_MSG_GENERAL;
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("cif:" + cifNo + "exception-" + ex.Message);
            return Config.ERR_MSG_GENERAL;
        }
        
    }

    public string GetInfoTokenPopup(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.RESPONSE_MSG_TOKEN_POPUP;
        string cifNumber = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string check_token_popup = Funcs.getConfigVal("STATUS_PROJECT_TOKEN");
        string content_en = "";
        string content_vn = "";

        if (check_token_popup != null && check_token_popup.Equals("TRIAL"))
        {
            content_en = Funcs.getConfigVal("CONTENT_TOKEN_POPUP_TRIAL_EN");
            content_vn = Funcs.getConfigVal("CONTENT_TOKEN_POPUP_TRIAL_VN");
        } else
        {
            content_en = Funcs.getConfigVal("CONTENT_TOKEN_POPUP_FINAL_EN");
            content_vn = Funcs.getConfigVal("CONTENT_TOKEN_POPUP_FINAL_VN");
        }

        DataSet ds = new DataSet();
        TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
        ds = da.GET_USER_BY_CIF(cifNumber);

        if (ds != null && (ds.Tables[0].Rows.Count > 0))
        {
            //int check_click = (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["BM9"].ToString()) ? 1: Int32.Parse(ds.Tables[0].Rows[0]["BM9"].ToString()));

            if (ds.Tables[0].Rows[0]["AUTH_METHOD"].ToString().Equals(Config.TypeMToken))
            {
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{ERR_DESC}", "SUCCESSFUL");
                retStr = retStr.Replace("{ISHOWPOPUP}", "FALSE");
                retStr = retStr.Replace("{CONTENT_VN}", content_vn);
                retStr = retStr.Replace("{CONTENT_EN}", content_en);
                retStr = retStr.Replace("{STATE}", check_token_popup);

                return retStr;
            }

            if (check_token_popup != null && check_token_popup.Equals("TRIAL"))
            {
                retStr = retStr.Replace("{ISHOWPOPUP}", "TRUE");
                retStr = retStr.Replace("{STATE}", check_token_popup);
            }
            else if(check_token_popup != null && check_token_popup.Equals("FINAL"))
            {

                retStr = retStr.Replace("{ISHOWPOPUP}", "TRUE");
                retStr = retStr.Replace("{STATE}", check_token_popup);
            }
            else
            {
                retStr = retStr.Replace("{ISHOWPOPUP}", "FALSE");
                retStr = retStr.Replace("{STATE}", check_token_popup);
            }

            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
            retStr = retStr.Replace("{ERR_DESC}", "SUCCESSFUL");
            retStr = retStr.Replace("{CONTENT_VN}", content_vn);
            retStr = retStr.Replace("{CONTENT_EN}", content_en);

        }
        else
        {
            return Config.ERR_MSG_GENERAL;
        }

        return retStr;
    }

    public string UpdateStatusShowTokenPoup(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.ERR_MSG_GENERAL;
        string cifNumber = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string check_token_popup = Funcs.getConfigVal("STATUS_PROJECT_TOKEN");

        if (check_token_popup.Equals("FINAL"))
        {
            DataSet ds = new DataSet();
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            ds = da.UPDATE_BM_TOKEN_POPUP(cifNumber);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                string err = ds.Tables[0].Rows[0]["RET_CODE"].ToString();

                if (err.Equals(Config.ERR_CODE_DONE))
                {
                    retStr = Config.SUCCESS_MSG_GENERAL;
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }

        }

        return retStr;
    }
}