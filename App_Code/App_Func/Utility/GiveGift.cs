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

using System.Net;
using System.Xml.Linq;
using System.Net.Mail;
using System.IO;
using System.Web.Script.Serialization;
using System.Linq;

/// <summary>
/// Summary description for Utils
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class GiveGift
    {
        public GiveGift()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region "GET_GIFT_TYPE"
        public string GET_GIFT_TYPE(Hashtable hashTbl, string ip, string user_agent)
        {
            string custId = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

            string retStr = Config.ERR_MSG_GENERAL;
            string resCode = Config.ERR_CODE_GENERAL;
            string resDesc = String.Empty;
            string strTemp = String.Empty;

            try
            {
                AcctSurprise.GetGiftTypeResType res = GiveGiftIntegration.GET_GIFT_TYPE(custId);

                retStr = Config.GET_GIFT_TYPE;

                if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errorCode.Equals("00"))
                {
                    resCode = Config.ERR_CODE_DONE;
                    resDesc = "SUCCESSFULL";

                    foreach (var item in res.listGiftType)
                    {
                        strTemp += (string.IsNullOrEmpty(item.giftType) ? "" : item.giftType)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.giftTypeNameVn) ? "" : item.giftTypeNameVn)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.giftTypeNameEn) ? "" : item.giftTypeNameEn)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.urlIcon) ? "" : Funcs.getConfigVal("LINK_WEB_SITE") + "img/" + item.urlIcon)
                            + Config.ROW_REC_DLMT;
                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                }
                else
                {
                    resCode = Config.ERR_CODE_GENERAL;
                    resDesc = "LOI KHONG XAC DINH";
                }

                retStr = retStr.Replace("{ERR_CODE}", resCode);
                retStr = retStr.Replace("{ERR_DESC}", resDesc);
                retStr = retStr.Replace("{CIF_NO}", custId);
                retStr = retStr.Replace("{RECORD}", strTemp);

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|ERROR EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_GIFT_TYPE ERROR: " + retStr);

                return retStr;
            }

            Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_GIFT_TYPE: " + retStr);

            return retStr;

        }
        #endregion "GET_GIFT_TYPE"

        #region "GET_GIFT_TEMPLACE"
        public string GET_GIFT_TEMPLACE(Hashtable hashTbl, string ip, string user_agent)
        {

            string custId = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string giftType = Funcs.getValFromHashtbl(hashTbl, "GIFT_TYPE");
            string templaceType = Funcs.getValFromHashtbl(hashTbl, "TEMPLACE_TYPE");

            string retStr = Config.ERR_MSG_GENERAL;
            string resCode = Config.ERR_CODE_GENERAL;
            string resDesc = String.Empty;
            string strTemp = String.Empty;

            try
            {
                AcctSurprise.GetGiftTemplaceResType res = GiveGiftIntegration.GET_GIFT_TEMPLACE(custId, giftType, templaceType);

                retStr = Config.GET_GIFT_TEMPLACE;

                if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errorCode.Equals("00"))
                {
                    resCode = Config.ERR_CODE_DONE;
                    resDesc = "SUCCESSFULL";

                    foreach (var item in res.listGiftTemplace)
                    {
                        string valueVn = String.Empty;
                        string valueEn = String.Empty;

                        if (!templaceType.Equals("MESSAGE"))
                        {
                            valueVn = (string.IsNullOrEmpty(item.valueVn) ? "" : Funcs.getConfigVal("LINK_WEB_SITE") + "img/" + item.valueVn);
                            valueEn = (string.IsNullOrEmpty(item.valueEn) ? "" : Funcs.getConfigVal("LINK_WEB_SITE") + "img/" + item.valueEn);
                        }
                        else{
                            valueVn = (string.IsNullOrEmpty(item.valueVn) ? "" : item.valueVn);
                            valueEn = (string.IsNullOrEmpty(item.valueEn) ? "" : item.valueEn);
                        }

                        strTemp += valueVn
                            + Config.COL_REC_DLMT
                            + valueEn
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.position) ? "" : item.position)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.color) ? "" : item.color)
                            + Config.ROW_REC_DLMT;
                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                }
                else
                {
                    resCode = Config.ERR_CODE_GENERAL;
                    resDesc = "LOI KHONG XAC DINH";
                }

                retStr = retStr.Replace("{ERR_CODE}", resCode);
                retStr = retStr.Replace("{ERR_DESC}", resDesc);
                retStr = retStr.Replace("{CIF_NO}", custId);
                retStr = retStr.Replace("{GIFT_TYPE}", giftType);
                retStr = retStr.Replace("{RECORD}", strTemp);

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|ERROR EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_GIFT_TEMPLACE ERROR: " + retStr);

                return retStr;
            }

            Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_GIFT_TEMPLACE: " + retStr);

            return retStr;

        }
        #endregion "GET_GIFT_TEMPLACE"

        #region "GIVE_GIFT"
        public string GIVE_GIFT(Hashtable hashTbl, string ip, string user_agent)
        {

            string custId = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            string des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME").Trim().ToUpper();
            string giftType = Funcs.getValFromHashtbl(hashTbl, "GIFT_TYPE");
            string url = Funcs.getValFromHashtbl(hashTbl, "INVITATION_CARD_URL");
            string message = Funcs.getValFromHashtbl(hashTbl, "MESSAGE");
            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string email = String.Empty;

            string tran_type = Config.TRAN_TYPE_GIVE_GIFT;
            string txDesc = "";
            double eb_tran_id = 0;

            string retStr = Config.ERR_MSG_GENERAL;
            string resCode = Config.ERR_CODE_GENERAL;
            string resDesc = String.Empty;
            string strTemp = String.Empty;
            string check_before_trans = String.Empty;
            string ref_No = String.Empty;
            string reasonTXT = String.Empty;
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable dtGift = new DataTable();
            DateTime effDt = DateTime.Now;
            string errCode = String.Empty;
            string errDesc = String.Empty;

            double amount = 0;

            try
            {
                amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT").Replace(",", "").Replace(".", ""));

                //get Infor by CustId

                ds = da.GET_USER_BY_CIF(custId);

                if (ds != null && ds.Tables[0] != null)
                {
                    dt = ds.Tables[0];
                    reasonTXT = dt.Rows[0]["CUSTNAME"].ToString().Trim().ToUpper() + " TANG QUA " + des_name.Trim().ToUpper();
                }
                else
                {
                    Funcs.WriteLog("CIF_NO: " + custId + "|ERROR GET INFO BY CIF: ");
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR: " + retStr);

                    return retStr;
                }


                if(message.Length > 200)
                {
                    Funcs.WriteLog("CIF_NO: " + custId + "|ERROR LIMIT LENG MESSAGE: ");
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR: " + retStr);

                    return retStr;
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|ERROR EXCEPTION PARSE AMOUNT: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR: " + retStr);

                return retStr;
            }

            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custId);
            }

            #endregion

            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custId, src_acct);
            if (!check)
            {
                retStr = Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI CustIdMatchScrAcct").Replace("{CIF_NO}", custId);
                Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR: " + retStr);
                return retStr;
            }

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custId, src_acct, des_acct, amount, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custId, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custId, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                Transfers transfer = new Transfers();
                DataTable eb_tran = new DataTable();

                Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT : BEGIN INSERTRAN");

                #region "insert TBL_EB_TRAN"
                eb_tran = transfer.insTransferTx(
                Config.ChannelID
                , "" //mod_cd
                , tran_type //tran_type
                , custId //custid
                , src_acct//src_acct
                , des_acct //des_acct
                , amount //amount
                , Config.CCYCD_VND //ccy_cd
                , 1//convert rate
                , amount //lcy_amount
                , reasonTXT //txdesc
                , "" //pos_cd
                , "" //mkr_id
                , "" //mkr dt
                , "" //apr id 1
                , "" //apr dt 1
                , "" //apr id 2
                , "" //apr dt 2
                , typeOtp //auth_type
                , Config.TX_STATUS_WAIT // status
                , 0 // tran pwd idx
                , "" //sms code
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
                , "" //""//suspend account
                , "" //""//fee account
                , "" //""//vat account
                , amount //suppend amount
                , 0 //fee amount
                , 0 //vat amount
                , "" // des name ten tai khoan thu huogn
                , "" // bank code
                , "" // ten ngan hang  //linhtn add new 2017 02 21: luu them bank_name
                , "" // ten thanh pho
                , "" // ten chi nhanh
                , giftType //"" //bm1  //linhtn add new 2017 02 21: luu them ten nguoi thu huong
                , url //bm2
                , "" //bm3
                , reasonTXT //bm4
                , effDt.ToString() //bm5
                , effDt.AddDays(double.Parse(Funcs.getConfigVal("VALUE_EXP_DATE_GIFT"))).ToString() //bm6
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
                , "" // type tinh phi
                , "" //bm18
                , "" //bm19
                , "" //bm20
                , "" //bm21
                , "" //bm22
                , "" //bm23
                , "" //bm24
                , "" //bm25
                , "" //bm26
                , requestId //bm27
                , ip //"" //bm28
                , user_agent // ""//bm29                   
            );

                #endregion "insert TBL_EB_TRAN"

                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    try
                    {
                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                        ref_No = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');


                        //goi ham Block
                        AcctSurprise.SurpriseBlockResType res = GiveGiftIntegration.ACCT_SURPRISE_BLOCK(custId, src_acct, ref_No, amount, effDt.ToString("dd/MM/yyyy"), reasonTXT);

                        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errorCode.Equals("00"))
                        {
                            GiftModel model = new GiftModel();
                            model.CIF_SEND_GIFT = custId;
                            model.SRC_ACCT = src_acct;
                            model.DES_ACCT = des_acct;
                            model.GIFT_TYPE = giftType;
                            model.GIFT_CARD_URL = url.Replace(Funcs.getConfigVal("LINK_WEB_SITE") + "img/", "");
                            model.GIFT_CARD_MESSAGE = message;
                            model.AMOUNT = amount.ToString();
                            model.CCYCD = Config.CCYCD_VND;
                            model.REF_NO = ref_No;
                            model.REMARK = reasonTXT;
                            model.REMARK_EARMARK = reasonTXT;
                            model.EFF_DT = effDt.ToString("dd/MM/yyyy") + " 00:00:00";
                            model.EXP_DT = effDt.AddDays(double.Parse(Funcs.getConfigVal("VALUE_EXP_DATE_GIFT"))).ToString("dd/MM/yyyy") + " 23:59:59";
                            model.DES_NAME = des_name;
                            model.SRC_NAME = dt.Rows[0]["CUSTNAME"].ToString().Trim();
                            model.CHAR1 = dt.Rows[0]["EMAIL"].ToString().Trim(); //luu mail cua nguoi TANG QUA
                            model.CHAR3 = dt.Rows[0]["REG_BRANCH"].ToString().Trim(); //luu pos nguoi tang

                            Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT : BEGIN INSERT_TBL_EB_GIFT");

                            //Goi ham tang qua
                            dtGift = new GiftDAO().INSERT_TBL_EB_GIFT(custId, model);

                            if (dtGift != null && dtGift.Rows.Count > 0 && dtGift.Rows[0]["ERRORCODE"].Equals("00"))
                            {
                                Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT : DONE INSERT_TBL_EB_GIFT");

                                transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, ref_No, effDt.ToString(), Config.ChannelID);
                                
                                errCode = Config.ERR_CODE_DONE;
                                errDesc = "SUCCESSFULL";

                                email = dtGift.Rows[0]["EMAIL"].ToString();

                                #region SEND PUSH AND MAIL

                                try
                                {
                                    //sendMail
                                    string pathFile = String.Empty;
                                    string strContent = String.Empty;
                                    pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\GIVE_GIFT.html";
                                    strContent = Funcs.ReadAllFile(pathFile);

                                    strContent = strContent.Replace("P_CUSTNAME_P", des_name);
                                    strContent = strContent.Replace("P_FROM_P", dt.Rows[0]["CUSTNAME"].ToString());
                                    strContent = strContent.Replace("P_AMOUNT_P", Funcs.ConvertMoney(amount.ToString()).Trim());
                                    strContent = strContent.Replace("P_MESSAGE_P", message);
                                    strContent = strContent.Replace("P_TRAN_DATE_P", effDt.ToString("dd/MM/yyyy HH:mm:ss"));

                                    bool sendMail = new PushNotification().sendPushEmail(custId, email, "Thông báo quà tặng", strContent, "EMAIL", eb_tran_id, ref_No);
                                    
                                    string title = GET_NAME_GIFT_TYPE(custId, giftType);

                                    //sendPush
                                    string strContentPusht = "Quý khách nhận được quà " + title + " từ " + dt.Rows[0]["CUSTNAME"].ToString() + ". Vui lòng truy cập tính năng Tặng quà trên SHB Mobile để thực hiện Mở quà. Trân trọng cảm ơn.";

                                    bool sendPush = new PushNotification().sendPushEmail(dtGift.Rows[0]["CUSTID"].ToString(), dtGift.Rows[0]["CUSTID"].ToString(), "Thông báo quà tặng", strContentPusht, "OTT", eb_tran_id, ref_No);
                                }
                                catch (Exception ex)
                                {
                                    Funcs.WriteLog("custid:" + custId + "|REGIST GIVE_GIFT SEND MAIL FAIL" + ex.Message.ToString());
                                }

                                #endregion
                                
                                #region SAVE TO BENLIST

                                if (save_to_benlist == "1")
                                {
                                    Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT BEGIN SAVE TO BENLIST");

                                    Beneficiarys ben = new Beneficiarys();
                                    DataTable dtBen = new DataTable();
                                    dtBen = ben.INSERT_BEN(
                                        custId
                                        , Config.TransType.TRAN_TYPE_INTRA
                                        , des_acct
                                        , des_name
                                        , des_name //"XXXXXXXXXX"//des_nick_name
                                        , reasonTXT
                                        , ""//bank_code
                                        , ""//bank_name
                                        , ""//bank_branch
                                        , ""//bank_city
                                        , "" //category_id
                                        , "" //service_id
                                        , "" //lastchange default = sysdate da xu ly o db
                                        , ""// bm1
                                        , ""// bm2
                                        , ""// bm3
                                        , ""// bm4
                                        , ""// bm5
                                        , ""// bm6
                                        , ""// bm7
                                        , ""// bm8
                                        , ""// bm9
                                        , ""// bm10
                                        );

                                    if (dtBen != null && dtBen.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                                    {
                                        errCode = Config.CD_EB_TRANS_DONE;
                                        errDesc = "GIVE_GIFT IS COMPLETED TRAN_ID=" + eb_tran_id + " SAVE TO BENLIST DONE";

                                        Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT SAVE TO BENLIST DONE");

                                        //luu them action mantain benlist
                                        #region "INSERT_EB_ACTION"
                                        try
                                        {
                                            // linhtn: add new 20/11/2016
                                            // insert tbl_eb_action
                                            Utility utAction = new Utility();
                                            bool utActionRet = false;
                                            //if (err == Config.ERR_CODE_DONE) // doi mat khau thanh cong
                                            {
                                                utActionRet = utAction.INS_TBL_EB_ACTION
                                                   (Config.ChannelID
                                                   , "" //mod_cd
                                                   , Config.EB_ACTION_UPDATE_BENLIST
                                                   , custId
                                                   , ip
                                                   , user_agent
                                                   , Config.EB_ACTION_DONE
                                                   , "THEM MOI CAP NHAT DANH SACH THU HUONG TANG QUA THANH CONG"
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
                                        }
                                        catch (Exception ex)
                                        {
                                            Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT  SAVE BENLIST ACTION TO TBL_EB_ACTION FAILED EX:" + ex.ToString());
                                        }

                                        #endregion "INSERT_EB_ACTION"

                                    }
                                    else
                                    {
                                        errCode = Config.CD_EB_TRANS_DONE_BEN_FAILED;
                                        errDesc = "GIVE_GIFT IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id;

                                        Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT SAVE TO BENLIST FAILED");
                                    }
                                    //giai phong bo nho                            
                                    ben = null;
                                    dtBen = null;
                                }
                                #endregion
                            }
                            else
                            {
                                Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT : FAIL INSERT_TBL_EB_GIFT");

                                AcctSurprise.SurpriseUnblockResType resUnblock = GiveGiftIntegration.ACCT_SURPRISE_UNBLOCK(custId, ref_No);

                                if (resUnblock != null && resUnblock.RespSts != null && resUnblock.RespSts.Sts.Equals("0") && resUnblock.errorCode.Equals("00"))
                                {
                                    errCode = Config.ERR_CODE_GENERAL;
                                    errDesc = "LOI KHONG XAC DINH";
                                }
                                else
                                {
                                    errCode = Config.ERR_CODE_GENERAL;
                                    errDesc = "LOI KHONG XAC DINH";
                                }

                                transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, ref_No, effDt.ToString(), Config.ChannelID);
                            }

                        }
                        //block khong thanh cong
                        else
                        {
                            transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, ref_No, effDt.ToString(), Config.ChannelID);
                            Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR BLOCK ACCT: ");
                            errCode = Config.ERR_CODE_GENERAL;
                            errDesc = "LOI KHONG XAC DINH";
                        }
                    }
                    catch (Exception ex)
                    {
                        transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, ref_No, effDt.ToString(), Config.ChannelID);

                        Funcs.WriteLog("CIF_NO:" + custId + "|GIVE_GIFT EXCEPTION: " + ex.Message.ToString());
                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR: " + retStr);

                        return retStr;
                    }
                }
                //error inser trans
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR INSER TRAN: " + retStr);

                    return retStr;
                }
            }
            else  // Esle check before trans
            {
                retStr = check_before_trans;
                Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR check_before_trans: " + retStr);
                return retStr;
            }

            retStr = Config.RES_GIVE_GIFT;

            retStr = retStr.Replace("{ERR_CODE}", errCode);
            retStr = retStr.Replace("{ERR_DESC}", errDesc);
            retStr = retStr.Replace("{CIF_NO}", custId);
            retStr = retStr.Replace("{REF_NO}", ref_No);
            retStr = retStr.Replace("{CORE_DT}", effDt.ToString("HH:mm:ss dd/MM/yyyy"));

            Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT: " + retStr);

            return retStr;

        }
        #endregion "GIVE_GIFT"

        #region "OPEN_GIFT"
        public string OPEN_GIFT(Hashtable hashTbl, string ip, string user_agent)
        {

            string custId = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string refNoReq = Funcs.getValFromHashtbl(hashTbl, "REF_NO");
            string giftTab = Funcs.getValFromHashtbl(hashTbl, "GIFT_TAB");

            string retStr = Config.ERR_MSG_GENERAL;
            string resCode = Config.ERR_CODE_GENERAL;
            string resDesc = String.Empty;
            string strTemp = String.Empty;
            string tran_type = Config.TRAN_TYPE_RECEIVE_GIFT;
            DateTime effDT = DateTime.Now;
            double eb_tran_id = 0;
            string ref_No = "";
            DataTable dtGift;
            string core_txno_ref = "";
            string core_txdate_ref = "";

            try
            {
                //Get thong tin qua tang
                dtGift = new GiftDAO().GET_INFO_TBL_EB_GIFT(custId, refNoReq);

                if (dtGift == null || dtGift.Rows.Count < 1 || !dtGift.Rows[0]["ERRORCODE"].Equals("00"))
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("CIF_NO: " + custId + "|RES OPEN_GIFT ERROR GET_INFO_TBL_EB_GIFT : " + retStr);

                    return retStr;
                }

                #region OPEN QUA

                if (dtGift.Rows[0]["STATUS"].ToString().Equals(Config.GIFT_STATUS_START) && custId.Equals(dtGift.Rows[0]["CIF_RECEIVE_GIFT"].ToString()))
                {
                    //OPEN GIFT
                    Funcs.WriteLog("CIF_NO:" + custId + "|OPEN_GIFT : BEGIN OPEN GIFT");
                    Transfers transfer = new Transfers();
                    DataTable eb_tran = new DataTable();

                    Funcs.WriteLog("CIF_NO:" + custId + "|OPEN_GIFT : BEGIN INSERTRAN");

                    #region "insert TBL_EB_TRAN"
                    eb_tran = transfer.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custId //custid
                    , dtGift.Rows[0]["SRC_ACCT"].ToString() //src_acct
                    , dtGift.Rows[0]["DES_ACCT"].ToString() //des_acct
                    , Double.Parse(dtGift.Rows[0]["AMOUNT"].ToString()) //amount
                    , Config.CCYCD_VND //ccy_cd
                    , 1//convert rate
                    , Double.Parse(dtGift.Rows[0]["AMOUNT"].ToString()) //lcy_amount
                    , dtGift.Rows[0]["REMARK"].ToString() //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , 0 //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , "" //sms code
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
                    , "" //""//suspend account
                    , "" //""//fee account
                    , "" //""//vat account
                    , Double.Parse(dtGift.Rows[0]["AMOUNT"].ToString()) //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang  //linhtn add new 2017 02 21: luu them bank_name
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , giftTab //"" //bm1  //linhtn add new 2017 02 21: luu them ten nguoi thu huong
                    , dtGift.Rows[0]["CIF_SEND_GIFT"].ToString() //bm2
                    , "" //bm3
                    , "" //bm4
                    , effDT.ToString() //bm5
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
                    , "" // type tinh phi
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
                    , ip //"" //bm28
                    , user_agent // ""//bm29                   
                );

                    #endregion "insert TBL_EB_TRAN"

                    if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                    {
                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                        ref_No = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

                        //goi ham giai toa tai khoan
                        AcctSurprise.SurpriseUnblockResType resUnblock = GiveGiftIntegration.ACCT_SURPRISE_UNBLOCK(custId, refNoReq);

                        if (resUnblock != null && resUnblock.RespSts != null && resUnblock.RespSts.Sts.Equals("0") && resUnblock.errorCode.Equals("00"))
                        {
                            Funcs.WriteLog("CIF_NO:" + custId + "|OPEN_GIFT : ACCT_SURPRISE_UNBLOCK: SUCCESS");

                            Funcs.WriteLog("CIF_NO:" + custId + "|BEGIN POST TO CORE");

                            //string result = tf.postFINPOSTToCore
                            string result = CoreIntegration.postFINPOSTToCore
                            (custId
                            , tran_type
                            , eb_tran_id
                            , dtGift.Rows[0]["SRC_ACCT"].ToString()
                            , dtGift.Rows[0]["DES_ACCT"].ToString() //gl suspend
                            , "" //gl fee
                            , ""// gl vat
                            , Double.Parse(dtGift.Rows[0]["AMOUNT"].ToString()) // suspend amount
                            , 0 // fee amount
                            , 0 //vat amount
                            , dtGift.Rows[0]["REMARK"].ToString()
                            , dtGift.Rows[0]["CHAR3"].ToString()
                            , ref core_txno_ref
                            , ref core_txdate_ref
                            , dtGift.Rows[0]["REF_NO"].ToString()
                            );

                            if (result == Config.gResult_INTELLECT_Arr[0])
                            {
                                //DONE
                                Funcs.WriteLog("CIF_NO:" + custId + "|BEGIN POST TO CORE: SUCCESS");
                                transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, dtGift.Rows[0]["REF_NO"].ToString(), core_txdate_ref, Config.ChannelID);
                                new GiftDAO().UPDATE_TBL_EB_GIFT(custId, dtGift.Rows[0]["REF_NO"].ToString(), Config.GIFT_STATUS_DONE, null, "", result,"UPDATE - HOACH TOAN CORE SUCCESS");
                            }
                            else
                            {
                                //FAIL
                                Funcs.WriteLog("CIF_NO:" + custId + "|BEGIN POST TO CORE: FAIL");
                                transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);

                                retStr = Config.ERR_MSG_GENERAL;
                                Funcs.WriteLog("CIF_NO: " + custId + "|RES OPEN_GIFT ERROR GIAI NGAN KHONG THANH CONG : " + retStr);

                                new GiftDAO().UPDATE_TBL_EB_GIFT(custId, dtGift.Rows[0]["REF_NO"].ToString(), Config.GIFT_STATUS_FAIL, null, "", result, "UPDATE - HOACH TOAN CORE FAIL");

                                return retStr;
                            }

                        }
                        //giai toa khong thanh cong
                        else
                        {
                            retStr = Config.ERR_MSG_GENERAL;
                            Funcs.WriteLog("CIF_NO: " + custId + "|RES OPEN_GIFT ERROR GIAI TOA TAI KHOAN KHONG THANH CONG : " + retStr);

                            new GiftDAO().UPDATE_TBL_EB_GIFT(custId, dtGift.Rows[0]["REF_NO"].ToString(), Config.GIFT_STATUS_FAIL, "UNHOLD", "FAIL", "", "UPDATE - GIAI TOA TAI KHOAN FAIL");

                            return retStr;
                        }
                    }
                    //insert IB TRAN ERROR
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("CIF_NO: " + custId + "|RES GIVE_GIFT ERROR INSER TRAN: " + retStr);

                        return retStr;
                    }
                }

                #endregion

                AcctSurprise.GetDetailGiftResType res = GiveGiftIntegration.GET_GIFT_DETAIL(custId, giftTab, refNoReq, "");

                retStr = Config.GET_GIFT_DETAIL;

                if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errorCode.Equals("00"))
                {
                    string color = String.Empty;
                    string position = String.Empty;

                    resCode = Config.ERR_CODE_DONE;
                    resDesc = "SUCCESSFULL";

                    retStr = retStr.Replace("{CIF_NO}", custId);
                    retStr = retStr.Replace("{CUSTOMER_NAME}", res.giftTo);
                    retStr = retStr.Replace("{SRC_ACCT}", res.srcAcct);
                    retStr = retStr.Replace("{AMOUNT}", res.amount);
                    retStr = retStr.Replace("{MESSAGE}", res.message);
                    retStr = retStr.Replace("{REF_NO}", res.refNo);
                    retStr = retStr.Replace("{CORE_DT}", res.coreDt);
                    retStr = retStr.Replace("{STATUS}", GET_STR_STATUS(res.status));
                    retStr = retStr.Replace("{CORE_DT_OPEN}", res.coreDtOpen);
                    retStr = retStr.Replace("{URL}", Funcs.getConfigVal("LINK_WEB_SITE") + "img/" + res.url);

                    bool getProp = GET_COLOR_POSITION_GIFT_TEMP(custId, dtGift.Rows[0]["GIFT_TYPE"].ToString(), "CARD", res.url,out color, out position);

                    retStr = retStr.Replace("{POSITION}", position);
                    retStr = retStr.Replace("{COLOR}", color);
                }
                else
                {
                    resCode = Config.ERR_CODE_GENERAL;
                    resDesc = "LOI KHONG XAC DINH";
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|ERROR EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_INFO_TBL_EB_GIFT ERROR: " + retStr);

                return retStr;
            }

            retStr = retStr.Replace("{ERR_CODE}", resCode);
            retStr = retStr.Replace("{ERR_DESC}", resDesc);

            Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_INFO_TBL_EB_GIFT: " + retStr);

            return retStr;

        }
        #endregion "OPEN_GIFT"

        #region "GIFT_HISTORY"
        public string GIFT_HISTORY(Hashtable hashTbl, string ip, string user_agent)
        {

            string custId = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string giftTab = Funcs.getValFromHashtbl(hashTbl, "GIFT_TAB");

            string retStr = Config.ERR_MSG_GENERAL;
            string resCode = Config.ERR_CODE_GENERAL;
            string resDesc = String.Empty;
            string strTemp = String.Empty;

            try
            {
                AcctSurprise.GetHistoryGiftResType res = GiveGiftIntegration.GET_HISTORY_GIFT(custId, giftTab);

                retStr = Config.GET_HISTORY_GIFT;

                if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errorCode.Equals("00"))
                {
                    resCode = Config.ERR_CODE_DONE;
                    resDesc = "SUCCESSFULL";

                    foreach (var item in res.listHistoryGift)
                    {
                        strTemp += (string.IsNullOrEmpty(item.refNo) ? "" : item.refNo)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.giftTypeNameVn) ? "" : GET_STR_GIFT_TYPE(item.giftTypeNameVn.ToLower(),"VN"))
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.giftTypeNameEn) ? "" : GET_STR_GIFT_TYPE(Funcs.UpperFirstCharacter(item.giftTypeNameEn.ToLower()), "EN"))
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.customerName) ? "" : item.customerName)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.coreDt) ? "" : item.coreDt)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.amount) ? "" : item.amount)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.status) ? "" : item.status)
                            + Config.ROW_REC_DLMT;
                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                }
                else
                {
                    resCode = Config.ERR_CODE_GENERAL;
                    resDesc = "LOI KHONG XAC DINH";
                }

                retStr = retStr.Replace("{ERR_CODE}", resCode);
                retStr = retStr.Replace("{ERR_DESC}", resDesc);
                retStr = retStr.Replace("{CIF_NO}", custId);
                retStr = retStr.Replace("{RECORD}", strTemp);

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|ERROR EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_GIFT_TEMPLACE ERROR: " + retStr);

                return retStr;
            }

            Funcs.WriteLog("CIF_NO: " + custId + "|RES GET_GIFT_TEMPLACE: " + retStr);

            return retStr;

        }
        #endregion "GIFT_HISTORY"

        #region FUNCS

        public string GET_STR_STATUS(string input)
        {
            string res = String.Empty;

            switch (input)
            {
                case Config.GIFT_STATUS_DONE:
                    res = "Đã mở quà$Opened";
                    break;
                case Config.GIFT_STATUS_START:
                    res = "Chưa mở quà$Not open";
                    break;
                case Config.GIFT_STATUS_FAIL:
                    res = "Đã hết hạn$Expired";
                    break;
                case Config.GIFT_STATUS_EXP:
                    res = "Đã hết hạn$Expired";
                    break;
            }

            return res;
        }

        public string GET_STR_GIFT_TYPE(string input, string key)
        {
            string res = String.Empty;

            switch (key)
            {
                case "VN":
                    res = "Quà " + input;
                    break;
                case "EN":
                    res = input + " gift";
                    break;
            }

            return res;
        }

        public string GET_NAME_GIFT_TYPE(string custId, string input)
        {
            string res = String.Empty;

            AcctSurprise.GetGiftTypeResType resp = GiveGiftIntegration.GET_GIFT_TYPE(custId);

            if (resp != null && resp.RespSts != null && resp.RespSts.Sts.Equals("0") && resp.errorCode.Equals("00"))
            {
                foreach (var item in resp.listGiftType)
                {
                    if (item.giftType.ToUpper().Equals(input.ToUpper()))
                    {
                        return  Funcs.UpperFirstCharacter(item.giftTypeNameVn);
                    }
                }
            }

            return res;
        }

        public bool GET_COLOR_POSITION_GIFT_TEMP(string custId, string giftType, string templaceType, string input,out string color, out string position)
        {
            color = String.Empty;
            position = String.Empty;

            bool resp = false;

            try
            {
                AcctSurprise.GetGiftTemplaceResType res = GiveGiftIntegration.GET_GIFT_TEMPLACE(custId, giftType, templaceType);

                if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errorCode.Equals("00"))
                {
                    foreach (var item in res.listGiftTemplace)
                    {
                        if(input.ToUpper().Equals(item.valueVn.ToUpper()))
                        {
                            color = item.color;
                            position = item.position;

                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|ERROR GET_GIF_TYPE: " + ex.ToString());
                return false;
            }

            return resp;
        }

        #endregion
    }
}
