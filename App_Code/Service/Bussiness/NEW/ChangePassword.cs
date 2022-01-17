using iBanking.Common;
using mobileGW.Service.DataAccess;
using mobileGW.Service.Framework;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for PushNotification
/// </summary>
public class ChangePassword
{
    public ChangePassword()
    {

    }
    /// <summary>
    /// Format
    /// CMD#CHANGE_PWD_VERIFY|CIF_NO#{CIF_NO}|TOKEN#{TOKEN}|MOBILE#{MOBILE}|PASSNO#{PASSNO}|LEGACYAC#{LEGACYAC}
    /// </summary>
    /// <param name="hashTbl"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public string Verify(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.RESPONE_RESSET_PASSWORD;
        //get parameters from client
        //string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string mobile = Funcs.getValFromHashtbl(hashTbl, "MOBILE");
        string passno = Funcs.getValFromHashtbl(hashTbl, "PASSNO");
        string legacyac = Funcs.getValFromHashtbl(hashTbl, "LEGACYAC");
        string challenge = "";
        double request_id = 0;
        int auth_method = -1;
        Transfers tf = new Transfers();
        bool check_success = false;
        try
        {
            //get all function that get allow user register notification
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            dynParams.Add("PMOBILE_NUM", mobile, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PPASS_NO", passno, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("PLEGACY_AC", legacyac, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            ChangePasswordModel respModel = (ChangePasswordModel)new ConnectionFactory(Config.gEBANKConnstr)
                               .ExecuteData<ChangePasswordModel>(CommandType.StoredProcedure, "PKG_RESET_PWD.CHK_USERNAME_PASSNO_LEGACY_MOB", dynParams).First();
            
            if(respModel != null && !String.IsNullOrEmpty(respModel.CUSTID))
            {

                String OTP = Funcs.getAlphabets(6).ToUpper();
                //String OTP = "123789";
                //tf.uptTransferTx("", Config.TX_STATUS_FAIL, string.Empty, string.Empty, Config.ChannelID);
                double eb_tran_id = 0;
                DataTable eb_tran = new DataTable();
                auth_method = Int32.Parse(respModel.AUTH_METHOD);

                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , "RESET PASSWORD" //tran_type
                    , respModel.CUSTID //custid
                    , ""//src_acct
                    , "" //des_acct
                    , Double.Parse("0")  //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , Double.Parse("0")  //lcy_amount
                    , "" //txdesc
                    , Config.HO_BR_CODE //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , auth_method //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , (respModel.AUTH_METHOD.Equals(Config.TypeMToken)? "":OTP) //sms code
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
                    , ip// "" //bm28
                    , userAgent // ""//bm29                   
                );
                #endregion "insert TBL_EB_TRAN"

                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    Funcs.WriteLog("CUSTID:" + respModel.CUSTID + "|CARD HANDLE CARDNO:" + respModel.LEGACY_AC
                        + "ACTION:" + "RESSET PASSWORD" + "INSERT TBL EB DONE"
                        );
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    bool iSendSMS = false;

                    switch (auth_method)
                    {
                        case 5:
                            // TRUONG HOP GET QUESTION TU SOFT TOKEN
                            TokenOTP.CreateTransactionResType res = null;
                            OtpDA otpDa = new OtpDA();
                            DataTable dt = otpDa.insertTokenOtp(respModel.CUSTID, "_NULL_", auth_method, 0.0, 0);

                            if ((dt != null) && (dt.Rows.Count > 0))
                            {
                                try
                                {
                                    res = TokenOTPIntegration.CreateTransaction(respModel.CUSTID, dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString());

                                    if (res != null && res.RespSts.Sts.Equals("0"))
                                    {
                                        request_id = double.Parse(dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString());
                                        Funcs.WriteLog("mob_user:" + respModel.CUSTID + "|challengeCode: " + res.challenge);
                                        challenge = res.challenge;
                                        dt = otpDa.updateTokenOtp(dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString(), res.challenge);

                                        resultStr = resultStr.Replace("{REQUEST_ID}", request_id.ToString());
                                        resultStr = resultStr.Replace("{CHALLENGE_CODE}", challenge);

                                        iSendSMS = true;
                                    } else
                                    {
                                        iSendSMS = true;
                                        resultStr = resultStr.Replace("{AUTH_METHOD}", auth_method.ToString());
                                        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_CIF_NOT_REG_TOKEN);
                                        resultStr = resultStr.Replace("{CIF_NO}", respModel.CUSTID);
                                        resultStr = resultStr.Replace("{ERR_DESC}", "Vui lòng kích hoạt Smart OTP trên SHB Mobile và thực hiện lại thao tác!");

                                        return resultStr;
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
                        default:
                            
                            String SMSContent = Config.gSMSCodeMsg_RessetPassword;

                            SMSContent = SMSContent.Replace("[ACTIVE_CODE]", OTP);

                            Funcs.WriteLog("mobile_no:" + mobile + "|RESSET_PASSWORD begin insert sms content:**");
                            iSendSMS = new SmsIntergration().sendOTP(respModel.CUSTID, mobile, SMSContent, Funcs.getConn("SMS_PARTNER"), 0);
                            break;
                    }

                    if (iSendSMS)
                    {
                        check_success = true;
                        resultStr = resultStr.Replace("{AUTH_METHOD}", auth_method.ToString());
                        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                        resultStr = resultStr.Replace("{ERR_DESC}", "SUCCESSFUL");
                        resultStr = resultStr.Replace("{CIF_NO}", respModel.CUSTID);
                        resultStr = resultStr.Replace("{TRAN_ID}", eb_tran_id.ToString());

                        return resultStr;
                    }
                }
            }

            if (!check_success)
            {
                resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                resultStr = resultStr.Replace("{ERR_DESC}", "LOI KHONG XAC DINH");
                resultStr = resultStr.Replace("{CIF_NO}", "-1");
                resultStr = resultStr.Replace("{TRAN_ID}", "-1");
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("ChangepasswordVerify: " + ex.Message.ToString());
            resultStr = resultStr.Replace("{ERR_CODE}", "99");
            resultStr = resultStr.Replace("{ERR_DESC}", "LOI KHONG XAC DINH");
            resultStr = resultStr.Replace("{CIF_NO}", "-1");
            resultStr = resultStr.Replace("{TRAN_ID}", "-1");
        }

        return resultStr;
    }

    /// <summary>
    /// Ham confirm cua reset password
    /// </summary>
    /// <param name="hashTbl"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public string Confirm(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.RESPONE_RESSET_PASSWORD_CONFIRM;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string tranId = Funcs.getValFromHashtbl(hashTbl, "TRAN_ID");
        string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
        string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
        int typeOtp = Int16.Parse(typeOtpStr);
        string otp = Funcs.getValFromHashtbl(hashTbl, "OTP");

        Transfers tf = new Transfers();
        bool check_success = false;
        var dynParams = new OracleDynamicParameters();
        dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
        dynParams.Add("PCUSTID", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
        dynParams.Add("PTRAN_ID", tranId, dbType: OracleDbType.Decimal, direction: ParameterDirection.Input);

            ChangePasswordQueryModel respModel = (ChangePasswordQueryModel)new ConnectionFactory(Config.gEBANKConnstr)
                               .ExecuteData<ChangePasswordQueryModel>(CommandType.StoredProcedure, "PKG_RESET_PWD.GET_INFO_CONFIRM_RESSET_MOB", dynParams).First();

        string password = Funcs.getAlphabets(6);
        //string password = "12345Aa@";

        try
        {
            switch (typeOtp)
            {
                case 5:
                    TokenOTP.VerifyOTPCRResType res = null;

                    try
                    {
                        res = TokenOTPIntegration.VerifyOTPCR(cifNo, requestId, otp);

                        if (res != null && res.RespSts.Sts.Equals("0"))
                        {
                            //String strContent = "MAT KHAU CUA BAN LA: " + password;

                            dynParams = new OracleDynamicParameters();
                            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                            dynParams.Add("PCUSTID", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
                            dynParams.Add("PNEW_PWD", password, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

                            ResponseModel respSetPasswordModel = (ResponseModel)new ConnectionFactory(Config.gEBANKConnstr)
                                               .ExecuteData<ResponseModel>(CommandType.StoredProcedure, "PKG_RESET_PWD.SET_NEW_PWD_MOB", dynParams).First();

                            if (respSetPasswordModel != null && respSetPasswordModel.STATUS_CODE.Equals(Config.ERR_CODE_DONE))
                            {
                                string pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\ResetPassword-SHB.html";
                                String strContent = Funcs.ReadAllFile(pathFile);
                                strContent = strContent.Replace("{DATE}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                                strContent = strContent.Replace("{CUSTNAME}", respModel.CUST_NAME);
                                strContent = strContent.Replace("{CIF}", cifNo);
                                strContent = strContent.Replace("{PACKAGE}", respModel.PACKAGE);
                                strContent = strContent.Replace("{PASSWORD}", password);

                                AlternateView view = getEmbeddedImageResetpwd(strContent);

                                if (!"".Equals(respModel.EMAIL) && respModel.EMAIL != null && !respModel.EMAIL.ToUpper().Equals("NULL") && !respModel.EMAIL.ToUpper().Equals("_NULL_"))
                                {
                                    check_success = Funcs.sendEmail(respModel.EMAIL, "CAP LAI MAT KHAU TREN UNG DUNG SHB MOBILE - " + respModel.CUSTID, strContent, view);

                                    if (check_success)
                                    {
                                        Funcs.WriteLog("cifno: " + cifNo + " Send email done");
                                        tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_DONE, "", "", Config.ChannelID);
                                        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                                        resultStr = resultStr.Replace("{ERR_DESC}", "SUCCESSFUL");
                                        resultStr = resultStr.Replace("{EMAIL}", Funcs.maskingEmail(respModel.EMAIL));
                                    }
                                }
                                else
                                {
                                    Funcs.WriteLog("cifno: " + cifNo + " Send SMS begin");

                                    String SMSContent = Config.gSMSCodeMsg_ChangePassWord;

                                    SMSContent = SMSContent.Replace("[PASSWORD]", password);
                                    bool iSendSMS = new SmsIntergration().sendOTP(respModel.CUSTID, respModel.MOBILE_NO, SMSContent, Funcs.getConn("SMS_PARTNER"), 0);

                                    if (iSendSMS)
                                    {
                                        Funcs.WriteLog("cifno: " + cifNo + " Send SMS done");
                                        tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_DONE, "", "", Config.ChannelID);
                                        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                                        resultStr = resultStr.Replace("{ERR_DESC}", "SUCCESSFUL");
                                        resultStr = resultStr.Replace("{MOBILE}", Funcs.maskingMobile(respModel.MOBILE_NO));
                                        resultStr = resultStr.Replace("{EMAIL}", Funcs.maskingMobile(respModel.MOBILE_NO));
                                    }
                                    else
                                    {
                                        Funcs.WriteLog("cifno: " + cifNo + " Send SMS fail");
                                    }
                                    
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Funcs.WriteLog("Exception: " + e.Message);
                        object a = e.Message;
                        Funcs.WriteLog("Lỗi Hàm VerifyOTPCR: " + a);
                        resultStr = Config.ERR_MSG_GENERAL;
                    }

                    break;
                default:
                    //get all function that get allow user register notification
                    if (respModel != null && !String.IsNullOrEmpty(respModel.CUSTID))
                    {
                        Funcs.WriteLog("cifno:" + cifNo + " getOTP done | Email:" + respModel.EMAIL + "| Next confirm OTP -->");

                        if (respModel.SMSCODE.ToUpper().Equals(otp.ToUpper()))
                        {
                            
                            //String strContent = "MAT KHAU CUA BAN LA: " + password;

                            dynParams = new OracleDynamicParameters();
                            dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                            dynParams.Add("PCUSTID", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
                            dynParams.Add("PNEW_PWD", password, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

                            ResponseModel respSetPasswordModel = (ResponseModel)new ConnectionFactory(Config.gEBANKConnstr)
                                               .ExecuteData<ResponseModel>(CommandType.StoredProcedure, "PKG_RESET_PWD.SET_NEW_PWD_MOB", dynParams).First();

                            if (respSetPasswordModel != null && respSetPasswordModel.STATUS_CODE.Equals(Config.ERR_CODE_DONE))
                            {
                                string pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\ResetPassword-SHB.html";
                                String strContent = Funcs.ReadAllFile(pathFile);
                                strContent = strContent.Replace("{DATE}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                                strContent = strContent.Replace("{CUSTNAME}", respModel.CUST_NAME);
                                strContent = strContent.Replace("{CIF}", cifNo);
                                strContent = strContent.Replace("{PACKAGE}", respModel.PACKAGE);
                                strContent = strContent.Replace("{PASSWORD}", password);

                                AlternateView view = getEmbeddedImageResetpwd(strContent);

                                if (!"".Equals(respModel.EMAIL) && respModel.EMAIL != null && !respModel.EMAIL.ToUpper().Equals("NULL") && !respModel.EMAIL.ToUpper().Equals("_NULL_"))
                                {
                                    check_success = Funcs.sendEmail(respModel.EMAIL, "CAP LAI MAT KHAU TREN UNG DUNG SHB MOBILE - " + respModel.CUSTID, strContent, view);

                                    if (check_success)
                                    {
                                        Funcs.WriteLog("cifno: " + cifNo + " Send email done");
                                        tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_DONE, "", "", Config.ChannelID);
                                        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                                        resultStr = resultStr.Replace("{ERR_DESC}", "SUCCESSFUL");
                                        resultStr = resultStr.Replace("{EMAIL}", Funcs.maskingEmail(respModel.EMAIL));
                                    }
                                    else
                                    {
                                        Funcs.WriteLog("cifno: " + cifNo + " Send email fail");
                                        tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                                        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                                        resultStr = resultStr.Replace("{ERR_DESC}", "FAIL SEND MAIL");
                                        resultStr = resultStr.Replace("{EMAIL}", "");
                                    }
                                }
                                else
                                {
                                    Funcs.WriteLog("cifno: " + cifNo + " Send SMS begin");

                                    String SMSContent = Config.gSMSCodeMsg_ChangePassWord;

                                    SMSContent = SMSContent.Replace("[PASSWORD]", password);
                                    bool iSendSMS = new SmsIntergration().sendOTP(respModel.CUSTID, respModel.MOBILE_NO, SMSContent, Funcs.getConn("SMS_PARTNER"), 0);

                                    if (iSendSMS)
                                    {
                                        Funcs.WriteLog("cifno: " + cifNo + " Send SMS done");
                                        tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_DONE, "", "", Config.ChannelID);
                                        resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                                        resultStr = resultStr.Replace("{ERR_DESC}", "SUCCESSFUL");
                                        resultStr = resultStr.Replace("{MOBILE}", Funcs.maskingMobile(respModel.MOBILE_NO));
                                        resultStr = resultStr.Replace("{EMAIL}", Funcs.maskingMobile(respModel.MOBILE_NO));
                                    }
                                    else
                                    {
                                        Funcs.WriteLog("cifno: " + cifNo + " Send SMS fail");
                                    }
                                }

                                    
                            }
                            else
                            {
                                Funcs.WriteLog("cifno: " + cifNo + " ERROR VERIFY");
                                tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                                resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                                resultStr = resultStr.Replace("{ERR_DESC}", "FAIL SEND MAIL");
                                resultStr = resultStr.Replace("{EMAIL}", "");
                            }
                        }
                        else
                        {
                            tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                            resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_INVALID_ACTIVE_CODE);
                            resultStr = resultStr.Replace("{ERR_DESC}", "INVALID ACTIVE CODE");
                            resultStr = resultStr.Replace("{EMAIL}", "_NULL_");
                            Funcs.WriteLog("cifno: " + cifNo + " OTP Incorrect. Check OTP Request");
                        }
                    }
                    break;
            }

            if (!check_success)
            {
                tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                resultStr = resultStr.Replace("{ERR_DESC}", "LOI KHONG XAC DINH");
                resultStr = resultStr.Replace("{EMAIL}", "_NULL_");
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("ChangepasswordVerify: " + ex.Message.ToString());
            tf.uptTransferTx(Double.Parse(tranId), Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
            resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
            resultStr = resultStr.Replace("{ERR_DESC}", "LOI KHONG XAC DINH");
            resultStr = resultStr.Replace("{EMAIL}", "_NULL_");
        }

        return resultStr;
    }

    public AlternateView getEmbeddedImageResetpwd(string bodyContent)
    {

        AlternateView av1 = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyContent, null, System.Net.Mime.MediaTypeNames.Text.Html);

        return av1;
    }
}