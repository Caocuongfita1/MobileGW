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
using System.Text.RegularExpressions;

namespace mobileGW.Service.AppFuncs
{
    /// <summary>
    /// Summary description for Auth
    /// </summary>
    public class CardUltils
    {
        public CardUltils()
        {

        }


        #region "GET_CARD_LIST OLD"
        //public string GET_CARD_LIST(Hashtable hashTbl)
        //{
        //    #region "CMD GET_CARD_LIST"
        //    String retStr = Config.GET_CARD_LIST;
        //    String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        Cards cards = new Cards();
        //        ds = cards.getListCard(custid);

        //        //CARD_NO ^ CARD_NO_MASK ^ CARD_HOLDER_NAME ^ OPEN_DATE ^ EXP_DATE ^STATUS ^ ACCTNO ^ BALANCE ^ CARD_TYPE ^ CARD_PROD_NAME ^ CAN_ACTION ^STATUS_VN ^ STATUS_EN
        //        if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace("{CIF_NO}", custid);
        //            retStr = retStr.Replace("{ERR_DESC}", "GET CARD LIST SUCCESSFUL");

        //            string strTemp = "";
        //            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
        //            {
        //                //CARD_NO ^ CARD_NO_MASK ^ CARD_HOLDER_NAME ^ OPEN_DATE 
        //                //    ^ EXP_DATE ^STATUS ^ ACCTNO ^ AVAI_BAL ^ CARD_TYPE 
        //                //    ^ PRODUCT_NAME^ CAN_ACTION ^STATUS_VN ^ STATUS_EN 
        //                //    ^  EXCEED_LIMIT ^ PURCHASE_DEBIT ^ OUTSTANDING_BALANCE 
        //                //    ^ TAD ^ MAD ^ WAIT_PAID
        //                strTemp = strTemp +
        //                  ds.Tables[0].Rows[j][CARD.CARD_NO].ToString() + Config.COL_REC_DLMT +
        //                  Funcs.MaskCardNo(ds.Tables[0].Rows[j][CARD.CARD_NO].ToString()) + Config.COL_REC_DLMT + // số thẻ ở dạng 6 đầu + xxxxxx + 4 cuối
        //                                                                                                          //dt.Rows[j][CARD.CARD_NO_MASK].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.CARD_HOLDER_NAME].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.ISSUE_DATE].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.EXP_DATE].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.STATUS].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.ACCTNO].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.AVAIL_BAL].ToString() + Config.COL_REC_DLMT + //AVAI_BAL
        //                  ds.Tables[0].Rows[j][CARD.CARD_TYPE].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.PROD_NAME].ToString() + Config.COL_REC_DLMT +
        //                  ds.Tables[0].Rows[j][CARD.CARD_ACTION].ToString() + Config.COL_REC_DLMT +
        //                  //decodeCanAction(
        //                  //ds.Tables[0].Rows[j][CARD.CARD_ACTION].ToString()
        //                  //) + Config.COL_REC_DLMT +
        //                  //trang thai bang tieng viet
        //                  decodeCardStatus(ds.Tables[0].Rows[j][CARD.STATUS].ToString(), ds.Tables[0].Rows[j][CARD.CARD_TYPE].ToString()).Split('*')[0] + Config.COL_REC_DLMT +
        //                  //trang thai bang tieng anh
        //                  decodeCardStatus(ds.Tables[0].Rows[j][CARD.STATUS].ToString(), ds.Tables[0].Rows[j][CARD.CARD_TYPE].ToString()).Split('*')[1] + Config.COL_REC_DLMT +
        //                  //strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM1] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM1].ToString()) + Config.COL_REC_DLMT;
        //                  (ds.Tables[0].Rows[j][CARD.EXCEED_LIMIT] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.EXCEED_LIMIT].ToString()) + Config.COL_REC_DLMT +
        //                     //ds.Tables[0].Rows[j][CARD.PURCHASE_DEBIT].ToString() + Config.COL_REC_DLMT +
        //                     (ds.Tables[0].Rows[j][CARD.TAD] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.TAD].ToString()) + Config.COL_REC_DLMT + //= PURCHASE_DEBIT
        //                      (ds.Tables[0].Rows[j][CARD.OUTSTANDING_BALANCE] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.OUTSTANDING_BALANCE].ToString()) + Config.COL_REC_DLMT +
        //                     (ds.Tables[0].Rows[j][CARD.TAD] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.TAD].ToString()) + Config.COL_REC_DLMT +
        //                      (ds.Tables[0].Rows[j][CARD.MAD] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.MAD].ToString()) +
        //               //+ Config.COL_REC_DLMT +
        //               //ds.Tables[0].Rows[j][CARD.WAIT_PAID].ToString() + //bo cot nay
        //               Config.ROW_REC_DLMT;
        //            }
        //            strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
        //            retStr = retStr.Replace("{RECORD}", strTemp);
        //        }
        //        else
        //        {
        //            retStr = Config.ERR_MSG_GENERAL;
        //            Funcs.WriteLog("GET_CARD_LIST|CIF:" + custid + "NO DATA FOUND");
        //            return retStr;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog("GET_CARD_LIST" + ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;

        //    }
        //    return retStr;
        //    #endregion "CMD GET_CARD_LIST"
        //}
        #endregion "GET_CARD_LIST OLD"

        /// <summary>
        /// lấy danh sách thẻ 
        /// linhtn 23 jul 2016
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public string GET_CARD_LIST(Hashtable hashTbl)
        {
            #region "CMD GET_CARD_LIST"
            String retStr = Config.GET_CARD_LIST;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            try
            {
                //DataSet ds = new DataSet();
                //Cards cards = new Cards();
                //ds = cards.getListCard(custid);


                CardList.CardListInqResType res = CardIntegration.getCardList(custid);

                //CARD_NO ^ CARD_NO_MASK ^ CARD_HOLDER_NAME ^ OPEN_DATE ^ EXP_DATE ^STATUS ^ ACCTNO ^ BALANCE ^ CARD_TYPE ^ CARD_PROD_NAME ^ CAN_ACTION ^STATUS_VN ^ STATUS_EN
                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                if (res != null && res.CardRec != null)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{CIF_NO}", custid);
                    retStr = retStr.Replace("{ERR_DESC}", "GET CARD LIST SUCCESSFUL");

                    string strTemp = "";
                    foreach (var item in res.CardRec)
                    {
                        //CARD_NO ^ CARD_NO_MASK ^ CARD_HOLDER_NAME ^ OPEN_DATE 
                        //    ^ EXP_DATE ^STATUS ^ ACCTNO ^ AVAI_BAL ^ CARD_TYPE 
                        //    ^ PRODUCT_NAME^ CAN_ACTION ^STATUS_VN ^ STATUS_EN 
                        //    ^  EXCEED_LIMIT ^ PURCHASE_DEBIT ^ OUTSTANDING_BALANCE 
                        //    ^ TAD ^ MAD ^ WAIT_PAID

                        if (item == null) continue;

                        strTemp = strTemp +
                          item.CardInfo.CardId + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.CARD_NO].ToString() + Config.COL_REC_DLMT +
                          Funcs.MaskCardNo(item.CardInfo.CardId) + Config.COL_REC_DLMT + //Funcs.MaskCardNo(ds.Tables[0].Rows[j][CARD.CARD_NO].ToString()) + Config.COL_REC_DLMT + // số thẻ ở dạng 6 đầu + xxxxxx + 4 cuối
                                                                                         //dt.Rows[j][CARD.CARD_NO_MASK].ToString() + Config.COL_REC_DLMT +
                          item.CardInfo.CardHolder + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.CARD_HOLDER_NAME].ToString() + Config.COL_REC_DLMT +
                          item.CardInfo.IssueDt + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.ISSUE_DATE].ToString() + Config.COL_REC_DLMT +
                          item.CardInfo.ExpDt + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.EXP_DATE].ToString() + Config.COL_REC_DLMT +
                          item.CardInfo.CardSts + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.STATUS].ToString() + Config.COL_REC_DLMT +
                          item.CardInfo.AcctNo + Config.COL_REC_DLMT +  //ds.Tables[0].Rows[j][CARD.ACCTNO].ToString() + Config.COL_REC_DLMT +
                          item.CardInfo.VailBal + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.AVAIL_BAL].ToString() + Config.COL_REC_DLMT + //AVAI_BAL
                          item.CardInfo.CardType + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.CARD_TYPE].ToString() + Config.COL_REC_DLMT +
                          item.CardInfo.ProdName + Config.COL_REC_DLMT + //ds.Tables[0].Rows[j][CARD.PROD_NAME].ToString() + Config.COL_REC_DLMT +
                          item.CardAction + Config.COL_REC_DLMT +  //ds.Tables[0].Rows[j][CARD.CARD_ACTION].ToString() + Config.COL_REC_DLMT +
                                                                   //decodeCanAction(
                                                                   //ds.Tables[0].Rows[j][CARD.CARD_ACTION].ToString()
                                                                   //) + Config.COL_REC_DLMT +
                                                                   //trang thai bang tieng viet
                          decodeCardStatus(item.CardInfo.CardSts, item.CardInfo.CardType).Split('*')[0] + Config.COL_REC_DLMT + //decodeCardStatus(ds.Tables[0].Rows[j][CARD.STATUS].ToString(), ds.Tables[0].Rows[j][CARD.CARD_TYPE].ToString()).Split('*')[0] + Config.COL_REC_DLMT +
                                                                                                                                //trang thai bang tieng anh
                          decodeCardStatus(item.CardInfo.CardSts, item.CardInfo.CardType).Split('*')[1] + Config.COL_REC_DLMT + //decodeCardStatus(ds.Tables[0].Rows[j][CARD.STATUS].ToString(), ds.Tables[0].Rows[j][CARD.CARD_TYPE].ToString()).Split('*')[1] + Config.COL_REC_DLMT +
                                                                                                                                //strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM1] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM1].ToString()) + Config.COL_REC_DLMT;
                          (item.CardInfo.ExceedLimit == string.Empty ? "0" : item.CardInfo.ExceedLimit) + Config.COL_REC_DLMT + //(ds.Tables[0].Rows[j][CARD.EXCEED_LIMIT] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.EXCEED_LIMIT].ToString()) + Config.COL_REC_DLMT +
                                                                                                                                //ds.Tables[0].Rows[j][CARD.PURCHASE_DEBIT].ToString() + Config.COL_REC_DLMT +
                           (item.CardInfo.TadAmt == string.Empty ? "0" : item.CardInfo.TadAmt) + Config.COL_REC_DLMT + // (ds.Tables[0].Rows[j][CARD.TAD] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.TAD].ToString()) + Config.COL_REC_DLMT + //= PURCHASE_DEBIT
                            (item.CardInfo.OutStandingBal == string.Empty ? "0" : item.CardInfo.OutStandingBal) + Config.COL_REC_DLMT + // (ds.Tables[0].Rows[j][CARD.OUTSTANDING_BALANCE] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.OUTSTANDING_BALANCE].ToString()) + Config.COL_REC_DLMT +
                              (item.CardInfo.TadAmt == string.Empty ? "0" : item.CardInfo.TadAmt) + Config.COL_REC_DLMT + // (ds.Tables[0].Rows[j][CARD.TAD] == DBNull.Value ? "0" : ds.Tables[0].Rows[j][CARD.TAD].ToString()) + Config.COL_REC_DLMT +
                              (item.CardInfo.MadAmt == string.Empty ? "0" : item.CardInfo.MadAmt) + Config.COL_REC_DLMT +
                              (item.CardInfo.autoDebitStatus.Equals("0") ? Config.AutoDebit.ACTION_REGISTER : Config.AutoDebit.ACTION_UPDATE) +
                        //+ Config.COL_REC_DLMT +
                        //ds.Tables[0].Rows[j][CARD.WAIT_PAID].ToString() + //bo cot nay
                        Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("GET_CARD_LIST|CIF:" + custid + "NO DATA FOUND");
                    return retStr;
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("GET_CARD_LIST" + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;

            }
            return retStr;
            #endregion "CMD GET_CARD_LIST"
        }


        /// <summary>
        /// Hiển thị trạng thái thẻ  trạng thái tiếng việt * trạng thái tiếng Anh
        /// </summary>
        /// <param name="cardStatus"></param>
        /// <returns></returns>
        public static string decodeCardStatus(string cardStatus, string card_type)
        {

            //"0|Bình thường|VALID CARD",
            //"1|Chưa kích hoạt|CALL ISSUER",
            //"2|Chưa kích hoạt|WARM CARD",
            //"3|Tạm khóa|DO NOT HONOR",
            //"4|Chưa kích hoạt|HONOR WITH ID",
            //"5|Tạm khóa|NOT PERMITTED",
            //"6|Khóa vĩnh viễn|LOST CARD,CAPTURE",
            //"7|Khóa vĩnh viễn|STOLEN CARD",
            //"8|Chưa kích hoạt|CALL SECURITY",
            //"9|Tạm khóa|INVALID CARD",
            //"10|Chưa kích hoạt|PICK UP CARD,SPECIAL CONDITION",
            //"11|Chưa kích hoạt|CALL ACQUIRER SECURITY",
            //"12|Chưa kích hoạt|CARD IS NOT ACTIVATED",
            //"13|Chưa kích hoạt|PIN ATTEMPTS EXCEEDED",
            //"14|Chưa kích hoạt|FORCED PIN_CHANGE",
            //"15|Tạm khóa|CREDIT DEBTS",
            //"16|Chưa kích hoạt|VIRTUAL INACTIVE",
            //"17|Bình thường|PIN ACTIVATION",
            //"18|Chưa kích hoạt|INSTANT CARD PERSONIFICATION WAITING",
            //"19|Tạm khóa|FRAUD PREVENTION",
            //"20|Tạm khóa|TEMPORARY BLOCKED BY CLIENT",
            //"21|Khóa vĩnh viễn|PERMANENT BLOCKED BY CLIENT"
            switch (cardStatus)
            {
                case "0":
                    return "Bình thường*VALID CARD";
                case "1":
                    return "Chưa kích hoạt*CALL ISSUER";
                case "2":
                    return " Tạm khóa*WARM CARD";
                case "3":
                    return " Tạm khóa*DO NOT HONOR";
                case "4":
                    return " Tạm khóa*HONOR WITH ID";
                case "5":
                    return " Chưa kích hoạt*NOT PERMITTED";
                case "6":
                    return "Mất cắp, thất lạc*LOST CARD CAPTURE";
                case "7":
                    return "Tạm khóa*STOLEN CARD";
                case "8":
                    return " Tạm khóa*CALL SECURITY";
                case "9":
                    return "Thẻ không hợp lệ*INVALID CARD";
                case "10":
                    return " Tạm khóa*PICK UP CARD,SPECIAL CONDITION";
                case "11":
                    return " Tạm khóa*CALL ACQUIRER SECURITY";
                case "12":
                    return " Tạm khóa*CARD IS NOT ACTIVATED";
                case "13":
                    return "Sai PIN quá số lần quy định*PIN ATTEMPTS EXCEEDED";
                case "14":
                    return "Bắt buộc đổi PIN lần đầu*FORCED PIN CHANGE";
                case "15":
                    return " Tạm khóa*CREDIT DEBTS";
                case "16":
                    return "Chưa kích hoạt*VIRTUAL INACTIVE";
                case "17":
                    return " Bình thường*PIN ACTIVATION";
                case "18":
                    return "Chưa kích hoạt*INSTANT CARD PERSONIFICATION WAITING";
                case "19":
                    return "Tạm khóa*FRAUD PREVENTION";
                case "20":
                    return "Tạm khóa*TEMPORARY BLOCKED BY CLIENT";
                case "21":
                    return "Khóa vĩnh viễn*PERMANENT BLOCKED BY CLIENT";
                default:
                    return "Khác*Other";
                    //return GetValueKey("key02");
            }
        }


        public static string decodeCanAction(string card_action)
        {
            switch (card_action)
            {
                case "LOCK":
                    return "CLOSE_CARD";
                case "UNLOCK":
                    //case "12":
                    return "OPEN_CARD";
                default:
                    return "_NULL_";
            }
        }


        /// <summary>
        /// khóa mở thẻ áp dụng cả thẻ debit, credit
        /// (check thêm phần gửi email)
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public static string HANDLE_CARD(Hashtable hashTbl, string ip, string user_agent)
        {

            string custid = "";

            string src_acct = "";
            string des_acct = "";
            double amount = 0;
            string txdesc = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = 0;

            try
            {
                typeOtp = Int16.Parse(typeOtpStr);

                if (typeOtp == 2)
                {
                    pwd = Funcs.encryptMD5(pwd + custid);
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CUSTID:" + custid + "|CARD HANDLE GET OTP :" + typeOtpStr + "exception: " + ex.ToString());

            }

            #endregion
            // string pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "TRANPWD") + custid);
            string check_before_trans = "";


            src_acct = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");

            //OPEN_CARD, CLOSE_CARD --> update lại list tran_type

            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            string cardNum = Funcs.getValFromHashtbl(hashTbl, "CARDNUM");
            string cardMD5 = src_acct;

            //fix dien giải
            txdesc = "THAY DOI TRANG THAI THE ";

            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CheckCardBelongCIF(custid, src_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }


            //TRUYEN VAO WS LA OPEN VA CLOSE
            string tempTranType = "";
            if (tran_type == "OPEN_CARD")
            {
                tempTranType = "OPEN";
                //check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);
                #region FOR TOKEN
                switch (typeOtp)
                {
                    case 2:
                        check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);
                        break;
                    case 4:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    case 5:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    default:
                        break;
                }
                #endregion
            }
            else if (tran_type == "CLOSE_CARD")
            {
                tempTranType = "CLOSE";
                check_before_trans = Config.ERR_CODE_DONE;
            }
            else if (tran_type == "ACT_CARD")
            {
                tempTranType = "ACTIVE";
                //check_before_trans = Config.ERR_CODE_DONE;
                #region FOR TOKEN
                switch (typeOtp)
                {
                    case 2:
                        check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);
                        break;
                    case 4:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    case 5:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    default:
                        break;
                }
                #endregion

                #region VERIFY CARD NUM
                CardVerify.CardVerifyInqResType res = CardIntegration.verifyCardNum(cardNum, custid, Config.PIN_CARD_TYPE, cardMD5);

                if (res == null || !Config.gResult_PIN_VERIFY_Arr[0].Split('|')[0].ToString().Equals(res.errCode))
                {
                    return Config.ERR_MSG_GENERAL;
                }
                #endregion
            }
            else
            {
                return Config.ERR_MSG_GENERAL;
            }

            double eb_tran_id = 0;
            string retStr = "";

            if (check_before_trans == Config.ERR_CODE_DONE)
            //   if (1==1)
            {
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txdesc //txdesc
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
                    , requestId //bm27
                    , ip// "" //bm28
                    , user_agent // ""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    Funcs.WriteLog("CUSTID:" + custid + "|CARD HANDLE CARDNO:" + src_acct
                        + "ACTION:" + tran_type + "INSERT TBL EB DONE --> CALL FUNCTION HANDLE CARD"
                        );
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    //GOI HAM KHOA/ MO THE
                    bool retWS = CardIntegration.HandleCard(custid, src_acct, tempTranType);

                    // khoa/mo the thanh cong
                    Funcs.WriteLog("CUSTID:" + custid + "|CARD HANDLE CARDNO:" + src_acct
                       + "ACTION:" + tran_type + "CALL FUNCTION HANDLE CARD RET:"
                       + retWS.ToString()
                       );

                    if (retWS)
                    {
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, "", "", Config.ChannelID);
                        retStr = Config.HANDLE_CARD;
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "HANDLE CARD IS COMPLETED");
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        return retStr;

                    }
                    // khoa/mo the khong thanh cong
                    else
                    {
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                        retStr = Config.ERR_MSG_GENERAL;
                        return retStr;

                    }

                }
                //KHONG INSERT DU LIEU DUOC VAO EBANK TRANSACTION 
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;
                }
                //GIAI PHONG DU LIEU 
            }//end if check before tran
            else //invalid tranpwd
            {
                //retStr = Config.ERR_MSG_GENERAL;
                //return retStr;

                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "|HANDLE CARD INVALID TRANPWD END");
                return retStr;
            }

        }

        //CMD#CREDIT_CARD_PAYMENT|CIF_NO#0310008705|SRC_ACCT#1000013376|DES_ACCT#533398YYYYYYYYYYYYYYYYYYYYYYYYYYYYY898|AMOUNT#100000|TRANPWD#fksdfjf385738jsdfjsdf9| SAVE_TO_BENLIST#0|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX

        /*
         21.5. THÔNG TIN CHI TIẾT THẺ CREDIT
REQ=CMD#GET_CREDIT_CARD_INFO|CIF_NO#0310008705|CARD_NO#533333xxxxxxxxxx|TRANPWD#fksdfjf385738jsdfjsdf9|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
      
Response

public static String GET_CREDIT_CARD_INFO= "ERR_CODE" + COL_DLMT + "{ERR_CODE}" 
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
+ ROW_DLMT + "CREDIT_LIMIT" + COL_DLMT + "{CREDIT_LIMIT}"
+ ROW_DLMT + "REMAIN_CREDIT_LIMIT" + COL_DLMT + "{REMAIN_CREDIT_LIMIT}"
+ ROW_DLMT + "CUR_OUTSTANDING_BALANCE" + COL_DLMT + "{CUR_OUTSTANDING_BALANCE}"
+ ROW_DLMT + "STS_OUTSTANDING_BALANCE" + COL_DLMT + "{STS_OUTSTANDING_BALANCE}"
+ ROW_DLMT + "MIN_PAID" + COL_DLMT + "{MIN_PAID}"
+ ROW_DLMT + "MAX_PAID" + COL_DLMT + "{MAX_PAID}"
+ ROW_DLMT + "WAIT_PAID" + COL_DLMT + "{WAIT_PAID}"
+ ROW_DLMT + "OPEN_DATE" + COL_DLMT + "{OPEN_DATE}"
+ ROW_DLMT + "ERR_CODE_RECORD_ACTIVITY" + COL_DLMT + "{ERR_CODE_RECORD_ACTIVITY}"
+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}";
        */
        public static string GET_DEBIT_CARD_INFO(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string card_no = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            String retStr = Config.GET_DEBIT_CARD_INFO;
            try
            {
                CardHist.CardHistInqResType res = CardIntegration.getCardHist(custid, card_no, Config.ENQ_TYPE_LAST5, string.Empty, string.Empty);

                if (res != null && res.InqRes != null && res.InqRes.Length > 0)
                {
                    string strTemp = string.Empty;
                    foreach (var item in res.InqRes)
                    {
                        strTemp = strTemp + item.TxDt + " " + item.TxTime + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.AmtOrg) ? "0" : item.AmtOrg) + Config.COL_REC_DLMT
                            + Funcs.NoHack(item.TxDesc) + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.TxCur) ? "0" : item.TxCur) + Config.ROW_REC_DLMT;
                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);
                }
                else
                {
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);
                }

                Funcs.WriteLog("custid:" + custid + "|GET_CREDIT_CARD_INFO END CARD_NO:" + card_no);
                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        public static string GET_CREDIT_CARD_INFO(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string card_no = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            String retStr = Config.GET_CREDIT_CARD_INFO;
            Funcs.WriteLog("custid:" + custid + "|GET_CREDIT_CARD_INFO BEGIN CARD_NO:" + card_no);
            try
            {
                CardHist.CardHistInqResType res = CardIntegration.getCardHist(custid, card_no, Config.ENQ_TYPE_LAST5, string.Empty, string.Empty);

                if (res != null && res.InqRes != null && res.InqRes.Length > 0)
                {
                    string strTemp = string.Empty;
                    foreach (var item in res.InqRes)
                    {
                        strTemp = strTemp + item.TxDt + " " + item.TxTime + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.AmtOrg) ? "0" : item.AmtOrg) + Config.COL_REC_DLMT
                            + Funcs.NoHack(item.TxDesc) + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.TxCur) ? "0" : item.TxCur) + Config.ROW_REC_DLMT;
                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);
                }
                else
                {
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);
                }

                Funcs.WriteLog("custid:" + custid + "|GET_CREDIT_CARD_INFO END CARD_NO:" + card_no);
                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }

        }

        public static string GET_CARD_TRAN_BY_ENQ_TYPE(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string card_no = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            string enquiry_type = Funcs.getValFromHashtbl(hashTbl, "ENQUIRY_TYPE");

            String retStr = Config.GET_CARD_TRAN_BY_ENQ_TYPE;

            Funcs.WriteLog("custid:" + custid + "|GET_CARD_TRAN_BY_ENQ_TYPE BEGIN CARD_NO:" + card_no);
            try
            {
                CardDPP.GetTransactionListResType res = new CardDPPIntegration().GetCardHist(custid, card_no, enquiry_type, string.Empty, string.Empty);

                if (res != null && res.TransactionList != null && res.TransactionList.Length > 0)
                {
                    string strTemp = string.Empty;
                    foreach (var item in res.TransactionList)
                    {
                        strTemp = strTemp + item.txDt + " " + item.txTime + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.amtOrg) ? "0" : item.amtOrg) + Config.COL_REC_DLMT
                            + Funcs.NoHack(item.txDesc) + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.mccCd) ? "" : item.mccCd)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.status) ? "0" : item.status)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.tranId) ? "" : item.tranId)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.crDr) ? "" : item.crDr)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.txAmt) ? "0" : item.txAmt)
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);
                }
                else
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);
                }

                Funcs.WriteLog("custid:" + custid + "|GET_CARD_TRAN_BY_ENQ_TYPE END CARD_NO:" + card_no);

                return retStr;

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        #region "GET_LIMIT_CREDIT_CARD OLD"
        //public string GET_LIMIT_CREDIT_CARD(Hashtable hashTbl)
        //{
        //    #region "CMD GET_LIMIT_CREDIT_CARD"
        //    String retStr = Config.GET_LIMIT_CREDIT_CARD;
        //    string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    string cardno = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
        //    string ecom_limit = "_NULL_";
        //    string ecom_max = "_NULL_";
        //    string ecom_type = "_NULL_";

        //    string daily_limit = "_NULL_";
        //    string daily_max = "_NULL_";
        //    string daily_type = "_NULL_";

        //    string cash_limit = "_NULL_";
        //    string cash_max = "_NULL_";
        //    string cash_type = "_NULL_";

        //    string monthly_limit = "_NULL_";
        //    string monthly_max = "_NULL_";
        //    string monthly_type = "_NULL_";

        //    //LINHTN ADD NEW 2017 02 07
        //    //KIEM TRA TAI KHOAN MO SO THUOC CIF
        //    bool check = Auth.CheckCardBelongCIF(custid, cardno);
        //    if (!check)
        //    {
        //        return Config.ERR_MSG_GENERAL;
        //    }

        //    Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT BEGIN");

        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        Cards cards = new Cards();
        //        dt = cards.getCreditCardLimit(cardno);
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            for (int j = 0; j < dt.Rows.Count; j++)
        //            {
        //                if (dt.Rows[j]["LIMIT_NAME"].ToString() == "PURCHASE_DAILY")
        //                {
        //                    //daily_max = dt.Rows[j][CARD.AVAIABLE_LIMIT].ToString();
        //                    //linhtn modify 12/12/2016
        //                    daily_max = dt.Rows[j][CARD.EXCEED_LIMIT].ToString();
        //                    daily_limit = dt.Rows[j][CARD.CURRENT_LIMIT].ToString();
        //                    daily_type = dt.Rows[j][CARD.LIMIT_TYPE].ToString();
        //                }
        //                else if (dt.Rows[j]["LIMIT_NAME"].ToString() == "CASH_DAILY")
        //                {
        //                    //cash_max = dt.Rows[j][CARD.AVAIABLE_LIMIT].ToString();
        //                    //linhtn modify 12/12/2016
        //                    cash_max = dt.Rows[j][CARD.EXCEED_LIMIT].ToString();
        //                    cash_limit = dt.Rows[j][CARD.CURRENT_LIMIT].ToString();
        //                    cash_type = dt.Rows[j][CARD.LIMIT_TYPE].ToString();
        //                }
        //                else if (dt.Rows[j]["LIMIT_NAME"].ToString() == "ECOMMERCE")
        //                {
        //                    //ecom_max = dt.Rows[j][CARD.AVAIABLE_LIMIT].ToString();
        //                    //linhtn modify 12/12/2016
        //                    ecom_max = dt.Rows[j][CARD.EXCEED_LIMIT].ToString();
        //                    ecom_limit = dt.Rows[j][CARD.CURRENT_LIMIT].ToString();
        //                    ecom_type = dt.Rows[j][CARD.LIMIT_TYPE].ToString();
        //                }

        //                //linhtn add new 2016 12 02
        //                //bo sung them han muc chi tieu thang
        //                else if (dt.Rows[j]["LIMIT_NAME"].ToString() == "PURCHASE_MONTHLY")
        //                {
        //                    //monthly_max = dt.Rows[j][CARD.AVAIABLE_LIMIT].ToString();
        //                    //linhtn modify 12/12/2016
        //                    monthly_max = dt.Rows[j][CARD.EXCEED_LIMIT].ToString();
        //                    monthly_limit = dt.Rows[j][CARD.CURRENT_LIMIT].ToString();
        //                    monthly_type = dt.Rows[j][CARD.LIMIT_TYPE].ToString();
        //                }
        //            }
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace("{ERR_DESC}", "GET LIMIT CARD SUCCESSFUL");
        //            retStr = retStr.Replace("{CARD_NO}", cardno);

        //            retStr = retStr.Replace("{ECOMMERCE_LIMIT}", ecom_limit);
        //            retStr = retStr.Replace("{ECOMMERCE_MAX}", ecom_max);
        //            retStr = retStr.Replace("{ECOMMERCE_TYPE}", ecom_type);

        //            retStr = retStr.Replace("{PURCHASE_DAILY_LIMIT}", daily_limit);
        //            retStr = retStr.Replace("{PURCHASE_DAILY_MAX}", daily_max);
        //            retStr = retStr.Replace("{PURCHASE_DAILY_TYPE}", daily_type);

        //            retStr = retStr.Replace("{CASH_DAILY_LIMIT}", cash_limit);
        //            retStr = retStr.Replace("{CASH_DAILY_MAX}", cash_max);
        //            retStr = retStr.Replace("{CASH_DAILY_TYPE}", cash_type);

        //            retStr = retStr.Replace("{PURCHASE_MONTHLY_LIMIT}", monthly_limit);
        //            retStr = retStr.Replace("{PURCHASE_MONTHLY_MAX}", monthly_max);
        //            retStr = retStr.Replace("{PURCHASE_MONTHLY_TYPE}", monthly_type);


        //            Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT DONE");
        //        }
        //        else
        //        {
        //            retStr = Config.ERR_MSG_GENERAL;
        //            Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT FAILED");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT FAILED: EX=" + ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;

        //    }
        //    return retStr;
        //    #endregion "CMD GET_LIMIT_CREDIT_CARD"
        //}

        #endregion "GET_LIMIT_CREDIT_CARD OLD"


        public string GET_LIMIT_CREDIT_CARD(Hashtable hashTbl)
        {
            #region "CMD GET_LIMIT_CREDIT_CARD"
            String retStr = Config.GET_LIMIT_CREDIT_CARD;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string cardno = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
            string ecom_limit = "_NULL_";
            string ecom_max = "_NULL_";
            string ecom_type = "_NULL_";

            string daily_limit = "_NULL_";
            string daily_max = "_NULL_";
            string daily_type = "_NULL_";

            string cash_limit = "_NULL_";
            string cash_max = "_NULL_";
            string cash_type = "_NULL_";

            string monthly_limit = "_NULL_";
            string monthly_max = "_NULL_";
            string monthly_type = "_NULL_";

            //LINHTN ADD NEW 2017 02 07
            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CheckCardBelongCIF(custid, cardno);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }

            Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT BEGIN");

            try
            {
                CardLimit.CardLimitInqResType res = CardIntegration.getCardLimits(custid, cardno);

                if (res != null && res.LimitRec != null)
                {

                    foreach (var item in res.LimitRec)
                    {
                        if (item == null) continue;
                        if (item.LimitType == null) continue;
                        switch (item.LimitName)
                        {
                            case "PURCHASE_MONTHLY":
                                monthly_max = item.ExLimit;
                                monthly_limit = item.CurrentLimit;
                                monthly_type = item.LimitType;
                                break;
                            case "ECOMMERCE":
                                ecom_max = item.ExLimit;
                                ecom_limit = item.CurrentLimit;
                                ecom_type = item.LimitType;
                                break;
                            case "CASH_DAILY":
                                cash_max = item.ExLimit;
                                cash_limit = item.CurrentLimit;
                                cash_type = item.LimitType;

                                break;
                            case "PURCHASE_DAILY":
                                daily_max = item.ExLimit;
                                daily_limit = item.CurrentLimit;
                                daily_type = item.LimitType;
                                break;
                        }
                    }
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET LIMIT CARD SUCCESSFUL");
                    retStr = retStr.Replace("{CARD_NO}", cardno);

                    retStr = retStr.Replace("{ECOMMERCE_LIMIT}", ecom_limit);
                    retStr = retStr.Replace("{ECOMMERCE_MAX}", ecom_max);
                    retStr = retStr.Replace("{ECOMMERCE_TYPE}", ecom_type);

                    retStr = retStr.Replace("{PURCHASE_DAILY_LIMIT}", daily_limit);
                    retStr = retStr.Replace("{PURCHASE_DAILY_MAX}", daily_max);
                    retStr = retStr.Replace("{PURCHASE_DAILY_TYPE}", daily_type);

                    retStr = retStr.Replace("{CASH_DAILY_LIMIT}", cash_limit);
                    retStr = retStr.Replace("{CASH_DAILY_MAX}", cash_max);
                    retStr = retStr.Replace("{CASH_DAILY_TYPE}", cash_type);

                    retStr = retStr.Replace("{PURCHASE_MONTHLY_LIMIT}", monthly_limit);
                    retStr = retStr.Replace("{PURCHASE_MONTHLY_MAX}", monthly_max);
                    retStr = retStr.Replace("{PURCHASE_MONTHLY_TYPE}", monthly_type);

                    Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT DONE");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT FAILED");
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteInfo("CIF:" + custid + ":GET CREDIT LIMIT FAILED: EX=" + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;
            #endregion "CMD GET_LIMIT_CREDIT_CARD"
        }

        #region "SET_LIMIT_CREDIT_CARD OLD"
        //public string SET_LIMIT_CREDIT_CARD(Hashtable hashTbl, string ip, string user_agent)
        //{
        //    //CMD#SET_LIMIT_CREDIT_CARD|CIF_NO#0310003896|CARD_NO#970443xxxxxxxxxx|ECOMMERCE_LIMIT#999999|PURCHASE_DAILY_LIMIT#99999|CASH_DAILY_LIMIT#9999346|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX"
        //    string retStr = Config.SET_LIMIT_CREDIT_CARD;
        //    string custid = "";
        //    custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    string card_no = "";
        //    card_no = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
        //    string ecommerce_limit = "";
        //    ecommerce_limit = Funcs.getValFromHashtbl(hashTbl, "ECOMMERCE_LIMIT");
        //    string ecommerce_type = "";
        //    ecommerce_type = Funcs.getValFromHashtbl(hashTbl, "ECOMMERCE_TYPE");
        //    string purchase_limit = "";
        //    purchase_limit = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_DAILY_LIMIT");
        //    string purchase_type = "";
        //    purchase_type = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_DAILY_TYPE");

        //    string daily_limit = "";
        //    daily_limit = Funcs.getValFromHashtbl(hashTbl, "CASH_DAILY_LIMIT");
        //    string daily_type = "";
        //    daily_type = Funcs.getValFromHashtbl(hashTbl, "CASH_DAILY_TYPE");


        //    string monthly_limit = "";
        //    monthly_limit = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_MONTHLY_LIMIT");
        //    string monthly_type = "";
        //    monthly_type = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_MONTHLY_TYPE");

        //    string txdesc = "CAI DAT HAN MUC THE TIN DUNG";
        //    double eb_tran_id = 0;
        //    string tran_type = Config.TRAN_TYPE_SET_LIMIT_CREDIT_CARD;

        //    // INSERT VAO BANG EB_TRAN

        //    string limit_type = "";
        //    limit_type = ecommerce_type + ":" + ecommerce_limit + ";" + purchase_type + ":" + purchase_limit + ";" + daily_type + ":" + daily_limit
        //        + ";" + monthly_type + ":" + monthly_limit;

        //    Transfers tf = new Transfers();
        //    DataTable eb_tran = new DataTable();


        //    //anhnd2 PCIDSS 02.02.2015 
        //    //Them salt cho PWD
        //    string pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "TRANPWD") + custid);

        //    Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT BEGIN");

        //    //LINHTN ADD NEW 2017 02 07 
        //    //KIEM TRA SO THE THUOC CIF
        //    bool check = Auth.CheckCardBelongCIF(custid, card_no);
        //    if (!check)
        //    {
        //        return Config.ERR_MSG_GENERAL;
        //    }


        //    string check_before_trans = Auth.CHECK_BEFORE_TRANSACTION
        //   (custid
        //   , ""
        //   , ""
        //   , 0
        //   , pwd, Config.TRAN_TYPE_SET_LIMIT_CREDIT_CARD);

        //    if (check_before_trans == Config.ERR_CODE_DONE)
        //    {
        //        Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT CHECK TRANPWD DONE");
        //        //  eb_tran = transfer.insTransferTx
        //        #region "insert TBL_EB_TRAN"
        //        eb_tran = tf.insTransferTx(
        //            Config.ChannelID
        //            , "CARD" //mod_cd
        //            , tran_type //tran_type
        //            , custid //custid
        //            , "" //card_no // ""//src_acct -- so the de
        //            , card_no //des_acct
        //            , 0//amount //amount
        //            , "VND" //ccy_cd
        //            , 1//convert rate
        //            , 0 //amount //lcy_amount
        //            , txdesc //txdesc
        //            , "" //pos_cd
        //            , "" //mkr_id
        //            , "" //mkr dt
        //            , "" //apr id 1
        //            , "" //apr dt 1
        //            , "" //apr id 2
        //            , "" //apr dt 2
        //            , Config.AUTH_MODE_MOB_GOLD //auth_type
        //            , Config.TX_STATUS_WAIT // status
        //            , 0 // tran pwd idx
        //            , "" //sms code
        //            , "" //sign data
        //            , "" //core err code
        //            , "" //core err desc
        //            , "" //core ref_no
        //            , "" //core txdate
        //            , "" //core txtime
        //            , 0 // order_amount
        //            , 0 // order_dis
        //            , "" //order_id
        //            , "" //partner code
        //            , "" //category code
        //            , "" //service code
        //            , "" //merchant code
        //            , ""//suspend account
        //            , ""//fee account
        //            , ""//vat account
        //            , 0 //suppend amount
        //            , 0 //fee amount
        //            , 0 //vat amount
        //            , "" // des name ten tai khoan thu huogn
        //            , "" // bank code
        //            , "" // ten ngan hang
        //            , "" // ten thanh pho
        //            , "" // ten chi nhanh
        //        //    string limit_type = "";
        //        //limit_type = ecommerce_type + ":" + ecommerce_limit + ";" + purchase_type + ":" + purchase_limit + ";" + daily_type + ":" + daily_limit
        //        //    + ";" + monthly_type + ":" + monthly_limit;
        //            , ecommerce_limit // "" //bm1
        //            , purchase_limit //"" //bm2
        //            , daily_limit //"" //bm3
        //            , monthly_limit //"" //bm4
        //            , "" //bm5
        //            , "" //bm6
        //            , "" //bm7
        //            , "" //bm8
        //            , "" //bm9
        //            , "" //bm10
        //            , "" //bm11
        //            , "" //bm12
        //            , "" //bm13
        //            , "" //bm14
        //            , "" //bm15
        //            , "" //bm16
        //            , "" //bm17
        //            , "" //bm18
        //            , "" //bm19
        //            , "" //bm20
        //            , "" //bm21
        //            , "" //bm22
        //            , "" //bm23
        //            , "" //bm24  
        //            , "" //bm25
        //            , "" //bm26
        //            , "" //bm27
        //            , ip //  //bm28
        //            , user_agent //""//bm29                   
        //        );
        //        #endregion "insert TBL_EB_TRAN"
        //        //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
        //        if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
        //        {

        //            Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT INSERT TBL_EB_TRAN DONE");

        //            eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
        //            #region "Goi sang ws the"

        //            Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT BEGIN CALL WS CARD");

        //            SHBCardGW.Service myGW = new SHBCardGW.Service();
        //            myGW.Url = Config.URL_WS_CARD_SHB;
        //            myGW.Timeout = Config.TIMEOUT_WITH_CARD_SHB;
        //            string localtime = DateTime.Now.ToString("yyyyMMddHHmmss");
        //            string retXML = string.Empty;
        //            string ret = "";
        //            retXML = myGW.SetLimitTypeByCardNo(
        //                "100000",
        //                Config.ChannelID,
        //                localtime,
        //                card_no,
        //                limit_type,
        //                Funcs.encryptMD5(
        //                "100000"
        //                + Config.ChannelID
        //                + localtime
        //                + card_no
        //                + limit_type
        //                + Config.key_credit_card_gw
        //                ).ToUpper()
        //            );

        //            XmlDocument XmlDoc = new XmlDocument();
        //            XmlDoc.LoadXml(retXML);
        //            Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT retXML=" + retXML);
        //            ret = XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText;
        //            if (ret == Config.ERR_CODE_DONE)
        //            {
        //                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //                retStr = retStr.Replace("{ERR_DESC}", "SET LIMIT DONE");

        //                //Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

        //                retStr = retStr.Replace("{TRANID}", Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0'));

        //                //retStr = retStr.Replace("{TRAN_DATE}", DateTime.Now.ToString("hh:mm:ss dd/MM/YYYY"));

        //                //DateTime.Now.ToString("yyyyMMdd HH:mm");
        //                //retStr = retStr.Replace("{TRAN_DATE}", DateTime.Now.ToString("yyyyMMdd HH:mm"));
        //                //retStr = retStr.Replace("{TRAN_DATE}", DateTime.Now.ToString("yyyyMMdd HH:mm"));
        //                retStr = retStr.Replace("{TRAN_DATE}", String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now));


        //                //, ecommerce_limit // "" //bm1
        //                //, purchase_limit //"" //bm2
        //                //, daily_limit //"" //bm3
        //                //, monthly_limit //"" //bm4
        //                //ERR CODE BM5
        //                //ERROR_DESC BM6

        //                tf.uptTransferTx_Full(
        //                         eb_tran_id
        //                         , Config.TX_STATUS_DONE
        //                         , "" //CORE TXNO
        //                         , "" //CORE TXDESC
        //                         , Config.ChannelID
        //                         , ecommerce_limit // bm1
        //                         , purchase_limit // bm2
        //                         , daily_limit // bm3
        //                         , monthly_limit //bm4
        //                         , XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText ////bm5
        //                         , "" //bm6
        //                         , ""//bm7
        //                         , ""//bm8
        //                         , ""//bm9
        //                         );

        //            }
        //            else
        //            {
        //                Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT CALL WS FAILED");
        //                retStr = Config.ERR_MSG_GENERAL;

        //                tf.uptTransferTx_Full(
        //                        eb_tran_id
        //                        , Config.TX_STATUS_FAIL
        //                        , "" //CORE TXNO
        //                        , "" //CORE TXDESC
        //                        , Config.ChannelID
        //                        , ecommerce_limit // bm1
        //                        , purchase_limit // bm2
        //                        , daily_limit // bm3
        //                        , monthly_limit //"" //bm4
        //                        , XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText ////bm5
        //                        , "" //bm6
        //                        , ""//bm7
        //                        , ""//bm8
        //                        , ""//bm9
        //                        );
        //            }


        //            #endregion "Goi sang ws the"
        //        }
        //        //KHONG INSERT DU LIEU DUOC VAO EBANK TRANSACTION 
        //        else
        //        {
        //            Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT CAN NOT INSERT TBL_EB_TRAN");
        //            retStr = Config.ERR_MSG_GENERAL;

        //        }

        //        return retStr;
        //    }
        //    else //INVALID TRANPWD
        //    {

        //        retStr = check_before_trans;
        //        Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT INVALID TRANPWD END");
        //        return retStr;
        //    }


        //}

        #endregion "SET_LIMIT_CREDIT_CARD OLD"

        public string SET_LIMIT_CREDIT_CARD(Hashtable hashTbl, string ip, string user_agent)
        {
            //CMD#SET_LIMIT_CREDIT_CARD|CIF_NO#0310003896|CARD_NO#970443xxxxxxxxxx|ECOMMERCE_LIMIT#999999|PURCHASE_DAILY_LIMIT#99999|CASH_DAILY_LIMIT#9999346|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX"
            string retStr = Config.SET_LIMIT_CREDIT_CARD;
            string custid = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string card_no = "";
            card_no = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
            string ecommerce_limit = "";
            ecommerce_limit = Funcs.getValFromHashtbl(hashTbl, "ECOMMERCE_LIMIT");
            string ecommerce_type = "";
            ecommerce_type = Funcs.getValFromHashtbl(hashTbl, "ECOMMERCE_TYPE");
            string purchase_limit = "";
            purchase_limit = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_DAILY_LIMIT");
            string purchase_type = "";
            purchase_type = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_DAILY_TYPE");

            string daily_limit = "";
            daily_limit = Funcs.getValFromHashtbl(hashTbl, "CASH_DAILY_LIMIT");
            string daily_type = "";
            daily_type = Funcs.getValFromHashtbl(hashTbl, "CASH_DAILY_TYPE");


            string monthly_limit = "";
            monthly_limit = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_MONTHLY_LIMIT");
            string monthly_type = "";
            monthly_type = Funcs.getValFromHashtbl(hashTbl, "PURCHASE_MONTHLY_TYPE");

            string txdesc = "CAI DAT HAN MUC THE TIN DUNG";
            double eb_tran_id = 0;
            string tran_type = Config.TRAN_TYPE_SET_LIMIT_CREDIT_CARD;

            // INSERT VAO BANG EB_TRAN

            string limit_type = "";
            limit_type = ecommerce_type + ":" + ecommerce_limit + ";" + purchase_type + ":" + purchase_limit + ";" + daily_type + ":" + daily_limit
                + ";" + monthly_type + ":" + monthly_limit;

            Transfers tf = new Transfers();
            DataTable eb_tran = new DataTable();


            //anhnd2 PCIDSS 02.02.2015 
            //Them salt cho PWD
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT BEGIN");

            //LINHTN ADD NEW 2017 02 07 
            //KIEM TRA SO THE THUOC CIF
            bool check = Auth.CheckCardBelongCIF(custid, card_no);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }


            string check_before_trans = "";


            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, "", "", 0, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, 0, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, 0, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT CHECK TRANPWD DONE");
                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "CARD" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , "" //card_no // ""//src_acct -- so the de
                    , card_no //des_acct
                    , 0//amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , 0 //amount //lcy_amount
                    , txdesc //txdesc
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
                //    string limit_type = "";
                //limit_type = ecommerce_type + ":" + ecommerce_limit + ";" + purchase_type + ":" + purchase_limit + ";" + daily_type + ":" + daily_limit
                //    + ";" + monthly_type + ":" + monthly_limit;
                    , ecommerce_limit // "" //bm1
                    , purchase_limit //"" //bm2
                    , daily_limit //"" //bm3
                    , monthly_limit //"" //bm4
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
                    , requestId //bm27
                    , ip //  //bm28
                    , user_agent //""//bm29                   
                );
                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {

                    Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT INSERT TBL_EB_TRAN DONE");

                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    #region "Goi sang ws the"

                    Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT BEGIN CALL WS CARD");

                    //SHBCardGW.Service myGW = new SHBCardGW.Service();
                    //myGW.Url = Config.URL_WS_CARD_SHB;
                    //myGW.Timeout = Config.TIMEOUT_WITH_CARD_SHB;
                    //string localtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    //string retXML = string.Empty;
                    //string ret = "";
                    //retXML = myGW.SetLimitTypeByCardNo(
                    //    "100000",
                    //    Config.ChannelID,
                    //    localtime,
                    //    card_no,
                    //    limit_type,
                    //    Funcs.encryptMD5(
                    //    "100000"
                    //    + Config.ChannelID
                    //    + localtime
                    //    + card_no
                    //    + limit_type
                    //    + Config.key_credit_card_gw
                    //    ).ToUpper()
                    //);


                    //XmlDocument XmlDoc = new XmlDocument();
                    //XmlDoc.LoadXml(retXML);
                    //Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT retXML=" + retXML);
                    string ws_card_result = string.Empty;
                    string ret = CardIntegration.setCardLimit(
                        custid
                        , card_no
                        , ecommerce_type
                        , ecommerce_limit
                        , purchase_type
                        , purchase_limit
                        , daily_type
                        , daily_limit
                        , monthly_type
                        , monthly_limit
                        , out ws_card_result
                        );

                    //if (ret == Config.ERR_CODE_DONE)
                    if (ret == Config.gResult_INTELLECT_Arr[0])
                    {
                        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{ERR_DESC}", "SET LIMIT DONE");

                        //Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

                        retStr = retStr.Replace("{TRANID}", Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0'));

                        //retStr = retStr.Replace("{TRAN_DATE}", DateTime.Now.ToString("hh:mm:ss dd/MM/YYYY"));

                        //DateTime.Now.ToString("yyyyMMdd HH:mm");
                        //retStr = retStr.Replace("{TRAN_DATE}", DateTime.Now.ToString("yyyyMMdd HH:mm"));
                        //retStr = retStr.Replace("{TRAN_DATE}", DateTime.Now.ToString("yyyyMMdd HH:mm"));
                        retStr = retStr.Replace("{TRAN_DATE}", String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now));


                        //, ecommerce_limit // "" //bm1
                        //, purchase_limit //"" //bm2
                        //, daily_limit //"" //bm3
                        //, monthly_limit //"" //bm4
                        //ERR CODE BM5
                        //ERROR_DESC BM6

                        tf.uptTransferTx_Full(
                                 eb_tran_id
                                 , Config.TX_STATUS_DONE
                                 , "" //CORE TXNO
                                 , "" //CORE TXDESC
                                 , Config.ChannelID
                                 , ecommerce_limit // bm1
                                 , purchase_limit // bm2
                                 , daily_limit // bm3
                                 , monthly_limit //bm4
                                 , ws_card_result //XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText ////bm5
                                 , "" //bm6
                                 , ""//bm7
                                 , ""//bm8
                                 , ""//bm9
                                 );

                    }
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT CALL WS FAILED");
                        retStr = Config.ERR_MSG_GENERAL;

                        tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_FAIL
                                , "" //CORE TXNO
                                , "" //CORE TXDESC
                                , Config.ChannelID
                                , ecommerce_limit // bm1
                                , purchase_limit // bm2
                                , daily_limit // bm3
                                , monthly_limit //"" //bm4
                                , ws_card_result// XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText ////bm5
                                , "" //bm6
                                , ""//bm7
                                , ""//bm8
                                , ""//bm9
                                );
                    }


                    #endregion "Goi sang ws the"
                }
                //KHONG INSERT DU LIEU DUOC VAO EBANK TRANSACTION 
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT CAN NOT INSERT TBL_EB_TRAN");
                    retStr = Config.ERR_MSG_GENERAL;

                }

                return retStr;
            }
            else //INVALID TRANPWD
            {

                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "|SET CREDIT LIMIT INVALID TRANPWD END");
                return retStr;
            }


        }

        public static string CREDIT_CARD_PAYMENT(Hashtable hashTbl, string ip, string user_agent)
        {

            //CMD#FUNDTRANSFER_2_CREDIT_CARD_FULL|ACTIVE_CODE#ABC|CIF_NO#0310008705|SRC_ACCT#1000013376|DES_ACCT#1000010000|AMOUNT#1000|TXDESC#CK SML TEST|TRANPWD#fksdfjf385738jsdfjsdf9";
            //public static String FundTransfer_Intra(String CIF,String SrcAcc, String DesAcc, String Amount, String TxDesc,String TxPass)

            //gui len: custid, 

            string custid = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = "";
            string des_acct = "";
            double amount = 0;
            string txdesc = "";
            // string tranPWD = "";


            string tran_type = Config.TRAN_TYPE_CREDIT_PAYMENT;

            src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");

            //so tai khoan the
            des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");

            //des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
            amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));

            //fix dien giải
            //txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

            //txdesc = txdesc + "TK THE " + des_acct;

            txdesc = "THANH TOAN THE TIN DUNG SO TAI KHOAN THE " + des_acct;

            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");
            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME");


            //anhnd2 PCIDSS 02.02.2015 
            //Them salt cho PWD
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

            //tungdt8 fix 2020 07 22
            //KIEM TRA HAN MUC TAI KHOAN THAU CHI
            Utils util = new Utils();
            bool checkLimitThauChi = util.checkLimitThauChi(custid, Config.TRAN_TYPE_CREDIT_PAYMENT, src_acct, des_acct, amount);
            if (!checkLimitThauChi)
            {
                return Config.ERR_MSG_FORMAT.Replace("{0}", Config.ERR_CODE_LIMIT_THAU_CHI).Replace("{1}", Funcs.getConfigVal("LIMIT_THAU_CHI_DES")).Replace("{2}", custid);
            }

            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            Funcs.WriteLog("custid:" + custid + "|CREDIT CARD PAYMENT BEGIN");

            double eb_tran_id = 0;

            string gl_suspend = Config.CREDIT_CARD_GL_SUSPEND;

            string core_txno_ref = "";
            string core_txdate_ref = "";
            string retStr = "";

            //linhtn fix 2017 02 06

            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }


            string check_before_trans = "";

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            if (check_before_trans == Config.ERR_CODE_DONE)
            {

                Funcs.WriteLog("custid:" + custid + "|CREDIT CARD PAYMENT CHECK PWD DONE");

                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "CARD" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txdesc //txdesc
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
                    , gl_suspend ////suspend account 
                    , ""//fee account
                    , ""//vat account
                    , amount //suppend amount
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
                    , requestId //bm27
                    , ip // "" //bm28
                    , user_agent //""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {

                    Funcs.WriteLog("custid:" + custid + "|CREDIT CARD PAYMENT INSERT TBL_EB_TRAN DONE");

                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    //GOI HAM HACH TOAN VAO CORE							
                    //string result = tf.postFINPOSTToCore(
                    //    custid
                    //    , tran_type
                    //    , eb_tran_id
                    //    , src_acct
                    //    , gl_suspend
                    //    , ""
                    //    , ""
                    //    , amount
                    //    , 0
                    //    , 0
                    //    , txdesc
                    //    , Config.HO_BR_CODE
                    //    , ref core_txno_ref
                    //    , ref core_txdate_ref
                    //    );

                    string result = CoreIntegration.postFINPOSTToCore(
                        custid
                        , tran_type
                        , eb_tran_id
                        , src_acct
                        , gl_suspend
                        , ""
                        , ""
                        , amount
                        , 0
                        , 0
                        , txdesc
                        , Config.HO_BR_CODE
                        , ref core_txno_ref
                        , ref core_txdate_ref
                        );

                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {
                        retStr = Config.CREDIT_CARD_PAYMENT;
                        //retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        //retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT CARD PAYMENT  IS COMPLETED");
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{TRANID}", core_txno_ref);
                        retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);

                        Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT : BEGIN CALL WEBSERVICES MAKETRANBYVOICE");
                        Cards cards = new Cards();
                        string localtime = DateTime.Now.ToString("yyyyMMddHHmmss");

                        #region "Goi sang ws the"
                        try
                        {
                            //cards.INS_EBANK_CARD_MESSAGE("100000", "EBANK", "CARD", "MAKETRANBYVOICE", Config.ChannelID, custid,
                            //   des_acct, txDesc, ref_no, "_NULL_", "_NULL_", src_acct, Double.Parse(amount),
                            //    "704", "705", localtime, "", "", "", "", "", "", "", "", "", "");

                            //string res_xml = cards.MakeTranByVoice("100000", Config.ChannelID, localtime, des_acct, "704", "705", amount.ToString());
                            //Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT : MAKETRANBYVOICE " + res_xml);
                            //string err_code_ws_card = "";
                            //if (res_xml.Trim() != string.Empty)
                            //{
                            //    XmlDocument XmlDoc = new XmlDocument();
                            //    XmlDoc.LoadXml(res_xml);


                            //    err_code_ws_card =  XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText;

                            //    Funcs.WriteLog("custid:" + custid + "|CREDIT CARD PAYMENT err_code_ws_card=" + err_code_ws_card);
                            string out_err_code = string.Empty;
                            string out_err_desc = string.Empty;
                            string out_err_utranno = string.Empty;
                            bool ret = CardIntegration.CardPosting(custid, des_acct, "704", decimal.Parse(amount.ToString()), out out_err_code, out out_err_desc, out out_err_utranno);

                            // neu thanh cong
                            //if (err_code_ws_card == Config.ERR_CODE_DONE)
                            if (ret)
                            {

                                retStr = Config.CREDIT_CARD_PAYMENT;
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT CARD PAYMENT IS DONE TRAN_ID:" + eb_tran_id);
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                                                                         // return retStr;
                            }
                            else //if ( err_code_ws_card != Config.ERR_CODE_DONE)
                            {
                                Funcs.WriteLog("custid:" + custid + "|CREDIT CARD PAYMENT TIMEOUT OR NOT COMPLETE");
                                retStr = Config.CREDIT_CARD_PAYMENT;
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_TIMEOUT);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT CARD PAYMENT IS PROCESSING=" + eb_tran_id);
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                return retStr;
                            }


                            //update lại các thông tin vào bảng TBL_EB_TRAN
                            // chi can update cac thong tin 

                            //cards.INS_EBANK_CARD_MESSAGE(
                            //    XmlDoc.GetElementsByTagName("MTI")[0].InnerText,
                            //    "CARD", "EBANK", "MAKETRANBYVOICE", Config.ChannelID,
                            //    custid,
                            //   XmlDoc.GetElementsByTagName("ACCOUNT_NO")[0].InnerText,
                            //   txDesc,
                            //   ref_no,
                            //   XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText,
                            //   XmlDoc.GetElementsByTagName("ERROR_DESC")[0].InnerText,
                            //   src_acct,
                            //   Double.Parse(XmlDoc.GetElementsByTagName("AMOUNT")[0].InnerText.Replace(",", "")),
                            //   XmlDoc.GetElementsByTagName("CURRENCY")[0].InnerText,
                            //   XmlDoc.GetElementsByTagName("TRAN_TYPE")[0].InnerText,
                            //   XmlDoc.GetElementsByTagName("LOCAL_TIME")[0].InnerText,
                            //   res_xml, XmlDoc.GetElementsByTagName("UTRNNO")[0].InnerText,
                            //   "", "", "", "", "", "", "", "");

                            Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT: BEGIN UPDATE EB TRAN STATUS=DONE");


                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_DONE
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , out_err_code //XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText// bm1
                                , out_err_desc // XmlDoc.GetElementsByTagName("ERROR_DESC")[0].InnerText //bm2
                                , out_err_utranno //XmlDoc.GetElementsByTagName("UTRNNO")[0].InnerText //bm3
                                , ""//bm4
                                , ""//bm5
                                , ""//bm6
                                , ""//bm7
                                , ""//bm8
                                , ""//bm9
                                );

                            //neu da hach toan sang the thanh cong

                            //save to benlist
                            //SAVE TO BEN LIST

                            if (save_to_benlist == "0")
                            {
                                Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT : SAVE TO BENLIST = 0");
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT CARD PAYMENT IS COMPLETED TRAN_ID=" + eb_tran_id);
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                            }
                            else if (save_to_benlist == "1")
                            {
                                Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT : BEGIN SAVE TO BENLIST = 1");
                                Beneficiarys ben = new Beneficiarys();
                                DataTable dt = new DataTable();
                                dt = ben.INSERT_BEN(
                                    custid
                                    , tran_type
                                    , des_acct
                                    , des_name
                                    , des_name
                                    , txdesc
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
                                    , Config.ChannelID //  ""// bm8
                                    , ip // ""// bm9
                                    , user_agent // ""// bm10
                                    );

                                if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                                {
                                    Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT : END SAVE TO BENLIST = 1 DONE");

                                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT CARD PAYMENT IS COMPLETED TRAN_ID=" + eb_tran_id + " SAVE TO BENLIST DONE");

                                    retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                    retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                }
                                else
                                {
                                    Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT : END SAVE TO BENLIST = 1 FAILED");

                                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT CARD PAYMENT IS COMPLETED TRAN_ID, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);
                                }
                                //giai phong bo nho                            
                                ben = null;
                                dt = null;
                            }

                        }
                        catch (Exception ex)
                        {
                            //cards.INS_EBANK_CARD_MESSAGE(
                            //       "100001",
                            //       "CARD", "EBANK", "MAKETRANBYVOICE", Config.ChannelID, custid,
                            //       des_acct, txDesc, ref_no, "03", "CALL WEBSERVICES EXCEPTION",
                            //       src_acct, Double.Parse(amount),
                            //       "704", "705", localtime, "Exception", "", "", "", "", "", "", "", "", "");
                            Funcs.WriteLog("CIF:" + custid + ":CREDIT CARD PAYMENT : MAKETRANBYVOICE EX" + ex.ToString());
                            tf.uptTransferTx_Full(
                                   eb_tran_id
                                   , Config.TX_STATUS_FAIL
                                   , core_txno_ref
                                   , core_txdate_ref
                                   , Config.ChannelID
                                   , "" //XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText// bm1
                                   , "" //XmlDoc.GetElementsByTagName("ERROR_DESC")[0].InnerText //bm2
                                   , "" //XmlDoc.GetElementsByTagName("UTRNNO")[0].InnerText //bm3
                                   , ""//bm4
                                   , ""//bm5
                                   , ""//bm6
                                   , ""//bm7
                                   , ""//bm8
                                   , "exception" + ex.ToString().Substring(1, 50) //bm9
                                   );


                            Funcs.WriteLog("custid:" + custid + "|CREDIT CARD PAYMENT TIMEOUT OR NOT COMPLETE");
                            retStr = Config.CREDIT_CARD_PAYMENT;
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_TIMEOUT);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT CARD PAYMENT IS PROCESSING=" + eb_tran_id);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                            return retStr;
                        }
                        finally
                        {
                            Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT : END CALL WEBSERVICES MAKETRANBYVOICE");
                        }
                        #endregion "Goi sang ws the"

                        return retStr;
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {
                        Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT CALL TO CORE FAILED");
                        retStr = Config.ERR_MSG_GENERAL;
                        return retStr;
                    }
                }
                //KHONG INSERT DU LIEU DUOC VAO EBANK TRANSACTION 
                else
                {
                    Funcs.WriteInfo("CIF:" + custid + ":CREDIT CARD PAYMENT CAN NOT INSERT TBL_EB_TRAN");
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;
                }
                //GIAI PHONG DU LIEU 
            }//end if check before tran
            else //INVALID TRANPWD
            {
                //retStr = Config.ERR_MSG_GENERAL;
                //return retStr;

                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "|CREDIT CARD PAYMENT INVALID TRANPWD END");
                return retStr;
            }
        }

        /*
         "REQ=CMD#GET_CARD_NAME_BY_ACCTNO|CIF_NO#0310005018|ACCTNO#123456|CATEGORY_ID#_NULL_|SERVICE_ID#_NULL_|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";


public static String GET_CARD_NAME_BY_ACCTNO= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "CARD_NAME" + COL_DLMT + "{CARD_NAME}"
        ;

         */
        public static string GET_CARD_NAME_BY_ACCTNO(Hashtable hashTbl)
        {
            string custid = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string acct_card_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
            string retStr = Config.GET_CARD_NAME_BY_ACCTNO;

            Funcs.WriteLog("custid:" + custid + "|GET_CARD_NAME_BY_ACCTNO END CARD ACCT NO:" + acct_card_no);

            string holderName = CardIntegration.getCardHolderName(acct_card_no, custid);

            Funcs.WriteLog("custid:" + custid + "|GET_CARD_NAME_BY_ACCTNO CARD ACCT NO:" + acct_card_no + "|HOLDER NAME: " + holderName);

            if (string.IsNullOrEmpty(holderName))
            {
                retStr = Config.ERR_MSG_GENERAL;
            }
            else
            {
                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CREDIT GET_CARD_NAME_BY_ACCTNO DONE");
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{CARD_NAME}", holderName);
            }

            Funcs.WriteLog("custid:" + custid + "|GET_CARD_NAME_BY_ACCTNO END CARD ACCT NO:" + acct_card_no);

            return retStr;
        }
        public static string verifyCardNum(Hashtable hashTbl)
        {
            string custid = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string cardNum = Funcs.getValFromHashtbl(hashTbl, "CARDNUM");
            string cardMD5 = Funcs.getValFromHashtbl(hashTbl, "CARDMD5");
            string retStr = Config.ERR_MSG_FORMAT;
            string errCode = Config.gResult_PIN_VERIFY_Arr[1].Split('|')[0].ToString();
            string errDesc = Config.gResult_PIN_VERIFY_Arr[1].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.gResult_PIN_VERIFY_Arr[1].Split('|')[2].ToString();

            Funcs.WriteLog("custid:" + custid + "|verifyCardNum cardNum:" + cardNum + "| BEGIN :");

            CardVerify.CardVerifyInqResType res = CardIntegration.verifyCardNum(cardNum, custid, Config.PIN_CARD_TYPE, cardMD5);

            if (res == null)
            {
                return Config.ERR_MSG_GENERAL;
            }
            else
            {
                for (int i = 0; i < Config.gResult_PIN_VERIFY_Arr.Length; i++)
                {
                    if (Config.gResult_PIN_VERIFY_Arr[i].Split('|')[0].ToString().Equals(res.errCode))
                    {
                        errCode = Config.gResult_PIN_VERIFY_Arr[i].Split('|')[0].ToString();
                        errDesc = Config.gResult_PIN_VERIFY_Arr[i].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.gResult_PIN_VERIFY_Arr[i].Split('|')[2].ToString();
                    }
                }
            }

            Funcs.WriteLog("custid:" + custid + "|verifyCardNum cardNum:" + cardNum + "| END");

            retStr = retStr.Replace("{0}", errCode);
            retStr = retStr.Replace("{1}", errDesc);

            return retStr;
        }
        public static string setPinCard(Hashtable hashTbl, string ip, string user_agent)
        {
            string custid = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string newPin = Funcs.getValFromHashtbl(hashTbl, "PIN");
            string cardMD5 = Funcs.getValFromHashtbl(hashTbl, "CARDMD5");

            string retStr = Config.ERR_MSG_FORMAT;
            string errCode = Config.gResult_SET_PIN_Arr[1].Split('|')[0].ToString();
            string errDesc = Config.gResult_SET_PIN_Arr[1].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.gResult_SET_PIN_Arr[1].Split('|')[2].ToString();

            string txdesc = "SET PIN CARD";

            Funcs.WriteLog("custid:" + custid + "|setPinCard | BEGIN :");

            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

            string check_before_trans = "";
            string tran_type = Config.PIN_TRANS_TYPE;
            double eb_tran_id = 0;

            #region VERIFY PIN
            CardVerify.CardVerifyInqResType resVerify = CardIntegration.verifyCardNum("", custid, Config.PIN_TRANS_TYPE, cardMD5);

            if (resVerify == null)
            {
                return Config.ERR_MSG_GENERAL;
            }
            else if (!Config.gResult_PIN_VERIFY_Arr[0].Split('|')[0].ToString().Equals(resVerify.errCode))
            {
                for (int i = 1; i < Config.gResult_PIN_VERIFY_Arr.Length; i++)
                {
                    if (Config.gResult_PIN_VERIFY_Arr[i].Split('|')[0].ToString().Equals(resVerify.errCode))
                    {
                        errCode = Config.gResult_PIN_VERIFY_Arr[i].Split('|')[0].ToString();
                        errDesc = Config.gResult_PIN_VERIFY_Arr[i].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.gResult_PIN_VERIFY_Arr[i].Split('|')[2].ToString();
                    }
                }

                return Config.ERR_MSG_FORMAT.Replace("{0}", errCode).Replace("{1}", errDesc);
            }

            #endregion

            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, "", "", 0, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, 0, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, 0, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                Funcs.WriteLog("custid:" + custid + "|setPinCard CHECK TRANPWD DONE");

                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "CARD" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , "" //card_no // ""//src_acct -- so the de
                    , cardMD5 //des_acct
                    , 0//amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , 0 //amount //lcy_amount
                    , txdesc //txdesc
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
                //    string limit_type = "";
                //limit_type = ecommerce_type + ":" + ecommerce_limit + ";" + purchase_type + ":" + purchase_limit + ";" + daily_type + ":" + daily_limit
                //    + ";" + monthly_type + ":" + monthly_limit;
                    , "" // "" //bm1
                    , "" //"" //bm2
                    , "" //"" //bm3
                    , "" //"" //bm4
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
                    , requestId //bm27
                    , ip //  //bm28
                    , user_agent //""//bm29                   
                );
                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    CardPIN.changePinRespType res = CardIntegration.changePin(custid, Config.ChannelID, cardMD5, newPin);

                    if (res == null)
                    {
                        tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_FAIL
                                , "" //CORE TXNO
                                , "" //CORE TXDESC
                                , Config.ChannelID
                                , String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now) // bm1
                                , "" // bm2
                                , "" // bm3
                                , "" //"" //bm4
                                , ""//bm5
                                , "" //bm6
                                , ""//bm7
                                , ""//bm8
                                , ""//bm9
                                );

                        return Config.ERR_MSG_GENERAL;
                    }
                    else if (Config.gResult_SET_PIN_Arr[0].Split('|')[0].ToString().Equals(res.respCode))
                    {
                        tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_DONE
                                , "" //CORE TXNO
                                , res.respDesc //CORE TXDESC
                                , Config.ChannelID
                                , String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now) // bm1
                                , res.respCode // bm2
                                , res.respDesc // bm3
                                , "" //"" //bm4
                                , ""//bm5
                                , "" //bm6
                                , ""//bm7
                                , ""//bm8
                                , ""//bm9
                                );
                        return Config.SUCCESS_MSG_GENERAL;
                    }
                    else
                    {
                        for (int i = 1; i < Config.gResult_SET_PIN_Arr.Length; i++)
                        {
                            if (Config.gResult_SET_PIN_Arr[i].Split('|')[0].ToString().Equals(res.respCode))
                            {
                                errCode = Config.gResult_SET_PIN_Arr[i].Split('|')[0].ToString();
                                errDesc = Config.gResult_SET_PIN_Arr[i].Split('|')[1].ToString()
                                    + Config.COL_REC_DLMT
                                    + Config.gResult_SET_PIN_Arr[i].Split('|')[2].ToString();
                            }
                        }
                    }

                    tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_FAIL
                                , "" //CORE TXNO
                                , "" //CORE TXDESC
                                , Config.ChannelID
                                , String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now) // bm1
                                , res.respCode // bm2
                                , res.respDesc // bm3
                                , "" //"" //bm4
                                , ""// XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText ////bm5
                                , "" //bm6
                                , ""//bm7
                                , ""//bm8
                                , ""//bm9
                                );
                }
                //KHONG INSERT DU LIEU DUOC VAO EBANK TRANSACTION 
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|setPinCard CAN NOT INSERT TBL_EB_TRAN");
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;
                }
            }
            else //INVALID TRANPWD
            {

                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "|setPinCard INVALID TRANPWD END");
                return retStr;
            }

            Funcs.WriteLog("custid:" + custid + "|setPinCard | END");

            retStr = retStr.Replace("{0}", errCode);
            retStr = retStr.Replace("{1}", errDesc);

            return retStr;
        }

        #region TOPUP PRE-PAID CARD
        public string GET_PREPAID_CARD_DETAIL(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string cardNum = Funcs.getValFromHashtbl(hashTbl, "CARD_NUM");
            string cardMD5 = Funcs.getValFromHashtbl(hashTbl, "CARD_MD5");

            string retStr = Config.PRE_PAID_CARD_INFO;

            try
            {
                CardInfoUtils.CardInforDetailResType res = CardUtilsIntegration.GET_PREPAID_CARD_DETAIL(custid, cardNum, cardMD5);

                if (res == null)
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_PREPAID_CARD_DETAIL EXCEPTION FROM ESB: null");
                    return Config.ERR_MSG_GENERAL;
                }
                else
                {
                    if (!res.RespSts.Sts.Equals("0"))
                    {
                        retStr = retStr.Replace("{ERR_CODE}", Config.RES_PRE_PAID_CARD_INFO_DETAIL[1].Split('|')[0]);
                        retStr = retStr.Replace("{ERR_DESC}", Config.RES_PRE_PAID_CARD_INFO_DETAIL[1].Split('|')[1]
                            + Config.COL_REC_DLMT + Config.RES_PRE_PAID_CARD_INFO_DETAIL[1].Split('|')[2]);
                        retStr = retStr.Replace("{CIF_NO}", custid);

                        return retStr;
                    }

                    if (res.RespSts.Sts.Equals("0") && res.errorCode.Equals("00"))
                    {
                        retStr = retStr.Replace("{ERR_CODE}", Config.RES_PRE_PAID_CARD_INFO_DETAIL[0].Split('|')[0]);
                        retStr = retStr.Replace("{ERR_DESC}", Config.RES_PRE_PAID_CARD_INFO_DETAIL[0].Split('|')[1]
                            + Config.COL_REC_DLMT + Config.RES_PRE_PAID_CARD_INFO_DETAIL[0].Split('|')[2]);
                        retStr = retStr.Replace("{CIF_NO}", custid);

                        retStr = retStr.Replace("{CARD_NUM}", res.BM1);
                        retStr = retStr.Replace("{CARD_MD5}", res.cardMD5);
                        retStr = retStr.Replace("{EMBOSS_NAME}", res.embossName);
                        retStr = retStr.Replace("{CARD_TYPE_CODE}", res.cardTypeCode);
                        retStr = retStr.Replace("{CARD_TYPE_DESC}", res.cardTypeDesc);
                        retStr = retStr.Replace("{CARD_STATUS}", res.cardStatus);
                        retStr = retStr.Replace("{AVAL_BALANCE}", res.avalBalance);
                        retStr = retStr.Replace("{CURRENCY}", res.currency);
                        retStr = retStr.Replace("{TOPUP_LIMIT_AMOUNT}", res.topupLimitAmt);
                        retStr = retStr.Replace("{BM1}", "");
                        retStr = retStr.Replace("{BM2}", "");
                        retStr = retStr.Replace("{BM3}", "");
                        retStr = retStr.Replace("{BM4}", "");
                        retStr = retStr.Replace("{BM5}", "");

                        return retStr;
                    }
                    else
                    {
                        string errCode = "";
                        string errDesc = "";

                        for (int i = 1; i < Config.RES_PRE_PAID_CARD_INFO_DETAIL.Length; i++)
                        {
                            if (Config.RES_PRE_PAID_CARD_INFO_DETAIL[i].Split('|')[0].Equals(res.errorCode))
                            {
                                errCode = Config.RES_PRE_PAID_CARD_INFO_DETAIL[i].Split('|')[0].ToString();
                                errDesc = Config.RES_PRE_PAID_CARD_INFO_DETAIL[i].Split('|')[1]
                                    + Config.COL_REC_DLMT + Config.RES_PRE_PAID_CARD_INFO_DETAIL[i].Split('|')[2];

                                retStr = retStr.Replace("{ERR_CODE}", Config.RES_PRE_PAID_CARD_INFO_DETAIL[i].Split('|')[0]);
                                retStr = retStr.Replace("{ERR_DESC}", Config.RES_PRE_PAID_CARD_INFO_DETAIL[i].Split('|')[1]
                                    + Config.COL_REC_DLMT + Config.RES_PRE_PAID_CARD_INFO_DETAIL[i].Split('|')[2]);
                                retStr = retStr.Replace("{CIF_NO}", custid);

                                return retStr;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("custid:" + custid + "|GET_PREPAID_CARD_DETAIL EXCEPTION FROM ESB: " + ex.ToString());

                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                retStr = retStr.Replace("{ERR_DESC}", "Lỗi không xác định$LOI KHONG XAC DINH");
                retStr = retStr.Replace("{CIF_NO}", custid);

                return retStr;
            }

            return Config.ERR_MSG_GENERAL;
        }

        public string DO_TOPUP_PREPAID_CARD(Hashtable hashTbl, string ip, string user_agent)
        {
            //CMD#DO_TOPUP_PREPAID_CARD|CIF_NO#%s|SRC_ACCT#%s|CARD_NUM#%s|TRAN_AMOUNT#%s|TXT_DESC#%s|SAVE_TO_BENLIST#%s|BEN_NAME#%s|TOKEN#%s|REQUEST_ID#%s|TYPE_OTP#%s
            string retStr = Config.PRE_PAID_TOPUP;

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string cardMD5 = Funcs.getValFromHashtbl(hashTbl, "CARD_NUM");

            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            double amount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "TRAN_AMOUNT"));
            string txtDesc = Funcs.getValFromHashtbl(hashTbl, "TXT_DESC");
            if (txtDesc == null || txtDesc == "") txtDesc = "TOPUP THE TRA TRUOC";

            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            string benName = Funcs.getValFromHashtbl(hashTbl, "BEN_NAME");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string tran_type = Config.TOPUP_PREPAID_CARD;
            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }
            #endregion

            string core_txno_ref = "";
            string retPartnerWS = "";
            string core_txdate_ref = "";
            string channel_id = Config.ChannelID;
            string cardNumDisplay = "";

            double eb_tran_id = 0;

            string check_before_trans = "";
            string pos_cd = "";
            pos_cd = Config.HO_BR_CODE;

            bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }

            CardInfoUtils.CardInforDetailResType cardInfor = CardUtilsIntegration.GET_PREPAID_CARD_DETAIL(custid, "", cardMD5);

            if (cardInfor == null
                || !cardInfor.errorCode.Equals("00"))
            {
                Funcs.WriteLog("custid:" + custid + "| DO TOPUP PREPAID CARD : CardND5 INVALID");
                return Config.ERR_MSG_GENERAL;
            }

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, cardMD5, amount, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            if (amount <= 0)
            {
                Funcs.WriteLog("custid:" + custid + "| DO TOPUP PREPAID CARD : AMOUNT < 0");
                return Config.ERR_MSG_GENERAL;
            }

            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();
                Funcs.WriteLog("custid:" + custid + "|BILL DES_ACCT:" + cardMD5
             + "BEGIN INSERT EB TRAN");

                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , cardMD5 //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txtDesc //txdesc
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
                    , amount // order_amount
                    , amount // order_dis
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
                    , benName // des name ten tai khoan thu huong
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , "" //bm1 --> category_id
                    , "" //bm2 --> service_id
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
                    , requestId //bm27
                    , ip // "" //bm28
                    , user_agent //""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"

                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    Funcs.WriteLog("custid:" + custid + "|TOPUP PREPAID DES_ACCT:" + cardMD5 + "BEGIN POST TO CORE");

                    string des_acct = Funcs.getConfigVal("GL_TOPUP_PREPAID_CARD"); //GL trung gian

                    //string result = tf.postFINPOSTToCore
                    string result = CoreIntegration.postFINPOSTToCore
                    (custid
                    , tran_type
                    , eb_tran_id
                    , src_acct
                    , des_acct //gl suspend
                    , "" //gl fee
                    , ""// gl vat
                    , amount  // suspend amount
                    , 0 // fee amount
                    , 0 //vat amount
                    , txtDesc
                    , pos_cd
                    , ref core_txno_ref
                    , ref core_txdate_ref
                    );

                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {
                        Funcs.WriteLog("custid:" + custid + "|TOPUP PREPAID DES_ACCT:" + cardMD5
                        + "BEGIN CASH DEPOSIT");
                        // GOI HAM TOPUP
                        string info_ref = "";

                        string out_err_code = string.Empty;
                        string out_err_desc = string.Empty;
                        string out_err_utranno = string.Empty;
                        bool ret = CardIntegration.CardPosting(custid, cardInfor.atmAcct, "704", decimal.Parse(amount.ToString()), out out_err_code, out out_err_desc, out out_err_utranno);


                        if (ret)
                        {
                            retPartnerWS = Config.ERR_CODE_DONE;
                            //retStr = retStr.Replace("{ERR_CODE}", retStr);
                            retStr = retStr.Replace("{ERR_DESC}", "SUCCESSFULL");
                        }
                        else
                        {
                            retPartnerWS = Config.ERR_CODE_GENERAL;
                            retStr = retStr.Replace("{ERR_DESC}", "LOI KHONG XAC DINH");
                        }

                        cardNumDisplay = cardInfor.BM1;

                        retStr = retStr.Replace("{TRANID}", eb_tran_id.ToString());
                        retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);
                        retStr = retStr.Replace("{TRAN_TYPE}", tran_type);
                        retStr = retStr.Replace("{CARD_NUM}", cardInfor.BM1);
                        retStr = retStr.Replace("{EMBOSS_NAME}", cardInfor.embossName);
                        //retstr = retstr.Replace("{BEN_NAME}", "");
                        retStr = retStr.Replace("{CARD_TYPE_CODE}", cardInfor.cardTypeCode);
                        retStr = retStr.Replace("{CARD_TYPE_DESC}", cardInfor.cardTypeDesc);
                        retStr = retStr.Replace("{AVAL_BALANCE}", "");
                        retStr = retStr.Replace("{CURRENCY}", cardInfor.currency);
                        retStr = retStr.Replace("{TXT_DESC}", txtDesc);
                        retStr = retStr.Replace("{BM1}", "");
                        retStr = retStr.Replace("{BM2}", "");
                        retStr = retStr.Replace("{BM3}", "");
                        retStr = retStr.Replace("{BM4}", "");
                        retStr = retStr.Replace("{BM5}", "");

                        Funcs.WriteLog("custid:" + custid + "TOPUP PREPAID TO SVFE tran_id:" + eb_tran_id
                                        + "|RETWS:" + retPartnerWS);
                        // neu topup thanh cong
                        if (retPartnerWS == Config.ERR_CODE_DONE)
                        {
                            //xu ly tiep save to ben list o duoi
                        }

                        if (retPartnerWS == Config.ERR_CODE_GENERAL)//                          
                        {
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                            return retStr;
                        }

                        // CAP NHAT TRANG THAI VAO BANG TBL_EB_TRAN
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                        //SAVE TO BEN LIST
                        //khong can save to ben list
                        if (save_to_benlist != "1")
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                        }
                        //can save to ben list
                        else if (save_to_benlist == "1")
                        {
                            // Goi ham save to BEN   
                            Beneficiarys ben = new Beneficiarys();
                            DataTable dt = new DataTable();
                            dt = ben.INSERT_BEN(
                                custid
                                , tran_type
                                , cardNumDisplay//des_acct   ---> save số dt thay cho acct
                                , benName
                                , benName// nap tien de 2 truong nay bang nhau
                                , txtDesc
                                , "" //bank_code
                                , "" //bank_name
                                , "" //bank_branch
                                , "" //bank_city
                                , "" //category_id
                                , "" //service_id
                                , "" //lastchange default = sysdate da xu ly o db
                                , cardMD5// bm1 LUU CARDMD5 voi tran_type = TOPUP PREPAID CARD
                                , ""// bm2
                                , ""// bm3
                                , ""// bm4
                                , ""// bm5
                                , ""// bm6
                                , ""// bm7
                                , Config.ChannelID//  ""// bm8
                                , ip // ""// bm9
                                , user_agent //""// bm10
                                );

                            if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            }
                            else
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                            }
                            //giai phong bo nho                            
                            ben = null;
                            dt = null;

                        }
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {
                        retStr = Config.CD_EB_TRANS_ERR_GENERAL;
                    }

                }
                else
                {
                    //not insert tran
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                }
            }
            else
            {
                //check befor tran
                retStr = check_before_trans;
            }

            return retStr;
        }
        #endregion

        #region AUTO DEBIT
        public string GET_AUTO_DEBIT(Hashtable hashTbl, string ip, string userAgent)
        {
            #region "CMD GET_AUTO_DEBIT"
            String retStr = Config.GET_AUTO_DEBIT;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String cardNum = Funcs.getValFromHashtbl(hashTbl, "CARD_MD5");

            try
            {
                CardAutoDebit.InquiryResType res = CardIntegration.getAutoDebit(custid, cardNum);

                if (res != null)
                {
                    if (res.RespSts.Sts.Equals("0") && !string.IsNullOrEmpty(res.cardMasking))
                    {
                        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{ERR_DESC}", "GET AUTO DEBIT SUCCESSFUL");
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{CARD_MD5}", (!string.IsNullOrEmpty(res.cardMasking) ? res.cardMasking : "_NULL_"));
                        retStr = retStr.Replace("{DEBIT_ACCT}", (!string.IsNullOrEmpty(res.casaAutoDebit) ? res.casaAutoDebit : "_NULL_"));
                        retStr = retStr.Replace("{DEBIT_AMOUNT}", (!string.IsNullOrEmpty(res.percentAutoDebit) ? res.percentAutoDebit : "_NULL_"));
                        retStr = retStr.Replace("{STATUS}", (!string.IsNullOrEmpty(res.autoDebitStatus) ? res.autoDebitStatus : "0"));
                    }
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("GET_AUTO_DEBIT|CIF: " + custid + "|NO DATA FOUND");
                    }

                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("GET_AUTO_DEBIT|CIF: " + custid + "|NO DATA FOUND");
                    return retStr;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("GET_AUTO_DEBIT: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            return retStr;
            #endregion "CMD GET_AUTO_DEBIT"
        }

        public string HANDLE_AUTO_DEBIT(Hashtable hashTbl, string ip, string user_agent)
        {

            string txdesc = "";
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string cardMd5 = Funcs.getValFromHashtbl(hashTbl, "CARD_MD5");
            string debit_acct = Funcs.getValFromHashtbl(hashTbl, "DEBIT_ACCT");
            string debit_amount = Funcs.getValFromHashtbl(hashTbl, "DEBIT_AMOUNT");
            string autoDebitStatus = Funcs.getValFromHashtbl(hashTbl, "AUTO_DEBIT_STATUS");
            string action = Funcs.getValFromHashtbl(hashTbl, "ACTION");
            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }
            #endregion
            // string pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "TRANPWD") + custid);
            string check_before_trans = "";
            string tran_type = Config.TRAN_TYPE_AUTO_DEBIT;

            //if (action.Equals(Config.AutoDebit.ACTION_REGISTER))
            //{
            //    txdesc = "DANG KI TRICH NO TU DONG";
            //} else if(action.Equals(Config.AutoDebit.ACTION_UPDATE))
            //{
            //    txdesc = "CHINH SUA TRICH NO TU DONG";
            //} else if (action.Equals(Config.AutoDebit.ACTION_CANCEL))
            //{
            //    txdesc = "HUY TRICH NO TU DONG";
            //} else
            //{
            //    return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI CHECK ACTION HANDLE_AUTO_DEBIT").Replace("{CIF_NO}", custid);
            //}


            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custid, debit_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI CustIdMatchScrAcct").Replace("{CIF_NO}", custid);
            }

            // KIEM TRA CARD THUOC CIF
            bool checkCard = CardIntegration.CheckCardBelongCif(custid, cardMd5);
            if (!checkCard)
            {
                return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI CheckCardBelongCif").Replace("{CIF_NO}", custid);
            }

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, debit_acct, debit_acct, 0, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, 0, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, 0, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion
            double eb_tran_id = 0;
            string retStr = "";

            if (check_before_trans == Config.ERR_CODE_DONE)
            //   if (1==1)
            {
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , debit_acct//src_acct
                    , cardMd5 //des_acct
                    , 0 //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , 0 //lcy_amount
                    , txdesc //txdesc
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
                    , debit_amount //bm1
                    , action //bm2
                    , action //bm3
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
                    , requestId //bm27
                    , ip// "" //bm28
                    , user_agent // ""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    Funcs.WriteLog("CUSTID:" + custid + "|HANDLE AUTO DEBIT:" + debit_acct
                        + "ACTION:" + tran_type + "INSERT TBL EB DONE --> CALL FUNCTION HANDLE AUTO DEBIT"
                        );
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    string coreRef = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

                    bool retWS = CardIntegration.HandleAutoDebit(custid, cardMd5, debit_acct, debit_amount, coreRef, action, txdesc);

                    Funcs.WriteLog("CUSTID:" + custid + "|HANDLE AUTO DEBIT:" + debit_acct
                       + "ACTION:" + tran_type + "CALL FUNCTION HANDLE AUTO DEBIT RET:"
                       + retWS.ToString()
                       );

                    if (retWS)
                    {
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, "", "", Config.ChannelID);

                        retStr = Config.HANDLE_AUTO_DEBIT;
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "HANDLE AUTO DEBIT IS COMPLETED");
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{REF_NO}", coreRef);
                        return retStr;

                    }
                    else
                    {
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                        retStr = Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "FAIL CALL ESB").Replace("{CIF_NO}", custid);
                        return retStr;
                    }

                }
                //KHONG INSERT DU LIEU DUOC VAO EBANK TRANSACTION 
                else
                {
                    retStr = Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "KHONG INSERT DU LIEU DUOC VAO EBANK TRANSACTION ").Replace("{CIF_NO}", custid);
                    return retStr;
                }
                //GIAI PHONG DU LIEU 
            }//end if check before tran
            else //invalid tranpwd
            {
                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "|HANDLE AUTO DEBIT INVALID TRANPWD END");
                return retStr;
            }

        }
        #endregion
        #region Unblock Card
        public string GET_UNBLOCK_CARD_DETAIL(Hashtable hashTbl, string ip, string user_agent)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string cardMD5 = Funcs.getValFromHashtbl(hashTbl, "CARD_MD5");
            Regex r = new Regex(@"&#\d{1,5};");

            string retStr = Config.PRE_UNBLOCK_CARD_DETAIL;

            try
            {
                UnblockCard.InquiryResType res = CardIntegration.GET_UNBLOCK_CARD_DETAIL(custid, cardMD5);

                if (res == null)
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_UNBLOCK_CARD_DETAIL EXCEPTION FROM ESB: null");
                    return Config.ERR_MSG_GENERAL;
                }
                else
                {
                    if (res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals("00"))
                    {
                        string strTemp = String.Empty;

                        foreach (var item in res.ListByTran)
                        {
                            strTemp = strTemp +
                           item.tranType +
                           Config.COL_REC_DLMT +
                           r.Replace(item.nameVN, Funcs.MatchEvaluator)
                           + Config.COL_REC_DLMT +
                           item.nameEN
                           + Config.COL_REC_DLMT +
                           item.status
                           + Config.COL_REC_DLMT +
                           item.expFrom
                           + Config.COL_REC_DLMT +
                           item.expTo
                           + Config.COL_REC_DLMT +
                           item.amount
                           + Config.ROW_REC_DLMT;
                        }

                        strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                        retStr = retStr.Replace("{RECORD}", strTemp);

                        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{ERR_DESC}", "SUCCESSFULL");
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{CARD_MD5}", cardMD5);
                        return retStr;
                    }
                    else
                    {
                        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace("{ERR_DESC}", "LOI KHONG RA THONG TIN");
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{RECORD}", "");

                        return retStr;
                    }
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("custid:" + custid + "|GET_UNBLOCK_CARD_DETAIL EXCEPTION FROM ESB: " + ex.ToString());

                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                retStr = retStr.Replace("{ERR_DESC}", "Lỗi không xác định$LOI KHONG XAC DINH");
                retStr = retStr.Replace("{CIF_NO}", custid);

                return retStr;
            }
        }
        public string UNBLOCK_CARD(Hashtable hashTbl, string ip, string user_agent)
        {
            string retStr = Config.ERR_MSG_FORMAT;

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string cardMD5 = Funcs.getValFromHashtbl(hashTbl, "CARD_MD5");
            string unblockType = Funcs.getValFromHashtbl(hashTbl, "UNBLOCK_TYPE");
            string validDate = Funcs.getValFromHashtbl(hashTbl, "VALID_DATE");
            string expDate = Funcs.getValFromHashtbl(hashTbl, "EXPIRE_DATE");
            double amount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
            string txtDesc = Funcs.getValFromHashtbl(hashTbl, "TXT_DESC");
            string status = Funcs.getValFromHashtbl(hashTbl, "STATUS");
            string action = Funcs.getValFromHashtbl(hashTbl, "ACTION");

            if (txtDesc == null || txtDesc == "") txtDesc = "MO CHAN THE QUOC TE";

            string benName = Funcs.getValFromHashtbl(hashTbl, "BEN_NAME");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string tran_type = Config.UNBLOCK_CARD;
            #region FOR TOKEN

            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }
            #endregion

            string channel_id = Config.ChannelID;

            double eb_tran_id = 0;

            string check_before_trans = "";
            string pos_cd = "";
            pos_cd = Config.HO_BR_CODE;

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, "", cardMD5, amount, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            //KIEM TRA SO THE THUOC CIF
            bool check = Auth.CheckCardBelongCIF(custid, cardMD5);
            if (!check)
            {
                Funcs.WriteLog("custid:" + custid + "| UNBLOCK CARD : ERROR CheckCardBelongCIF");
                return Config.ERR_MSG_GENERAL;
            }

            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();
                Funcs.WriteLog("custid:" + custid + "|BILL DES_ACCT:" + cardMD5
             + "BEGIN INSERT EB TRAN");

                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , cardMD5//src_acct
                    , "" //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txtDesc //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , DateTime.Now.ToString() //mkr dt
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
                    , amount // order_amount
                    , amount // order_dis
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
                    , benName // des name ten tai khoan thu huong
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , unblockType //bm1 --> category_id
                    , validDate + " 00:00:00" //bm2 --> service_id
                    , expDate + " 23:59:59" //bm3
                    , status //bm4
                    , amount.ToString() //bm5
                    , action //bm6
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
                    , requestId //bm27
                    , ip // "" //bm28
                    , user_agent //""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"

                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    Funcs.WriteLog("custid:" + custid + "|UNBLOCK CARD SRC_ACCT:" + cardMD5 + " BEGIN RESGIST");

                    UnblockCard.CreateResType res = CardIntegration.UNBLOCK_CARD(custid, cardMD5, unblockType, action, validDate + " 00:00:00", expDate + " 00:00:00", amount.ToString());

                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals("00"))
                    {
                        Funcs.WriteLog("custid:" + custid + "|UNBLOCK CARD SRC_ACCT:" + cardMD5 + " SUCCESSFULL");
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, "", DateTime.Now.ToString(), channel_id);

                        retStr = Config.PRE_UNBLOCK_CARD;

                        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{ERR_DESC}", Config.Message_Unblock_Card[0].Split('|')[1] + "$" + Config.Message_Unblock_Card[0].Split('|')[2]);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{CORE_REF}", "GD" + eb_tran_id.ToString());
                        retStr = retStr.Replace("{CORE_DATE}", DateTime.Now.ToString("dd/MM/yyyy"));

                        return retStr;

                    }

                    else if (res != null && res.RespSts != null && !res.RespSts.Sts.Equals("0"))
                    {
                        for (int i = 0; i < Config.Message_Unblock_Card.Length; i++)
                        {
                            if (res.errCode.Equals(Config.Message_Unblock_Card[i].Split('|')[0].ToString()))
                            {
                                Funcs.WriteLog("custid:" + custid + "|UNBLOCK CARD SRC_ACCT:" + cardMD5 + " FAIL" + res.errCode + "|" + res.errDesc);

                                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, "", DateTime.Now.ToString(), channel_id);
                                retStr = retStr.Replace("{0}", Config.ERR_CODE_GENERAL);
                                retStr = retStr.Replace("{1}", Config.Message_Unblock_Card[i].Split('|')[1] + "$" + Config.Message_Unblock_Card[i].Split('|')[2]);

                                return retStr;
                            }
                        }

                        return Config.ERR_MSG_GENERAL;
                    }
                    else
                    {
                        retStr = Config.CD_EB_TRANS_ERR_GENERAL;
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, "", DateTime.Now.ToString(), channel_id);
                        return retStr;
                    }

                }
                else
                {
                    //not insert tran
                    retStr = retStr.Replace("{0}", Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace("{1}", "CANT INSERT TO TBL_EB_TRANS");
                    return retStr;
                }
            }
            else
            {
                //check befor tran
                retStr = check_before_trans;
                return retStr;
            }
        }
        #endregion
    }
}