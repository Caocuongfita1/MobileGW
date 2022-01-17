using System;
using System.Collections.Generic;
using System.Web;
using mobileGW.Service.Framework;
using mobileGW.Service.DataAccess;
using System.Data;
using System.Threading;

using System.Collections;
using System.Xml;
using System.Web.Script.Serialization;
using mobileGW.Service.DataAccess;
using mobileGW.Service.Models;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using mobileGW.Service.API;

/// <summary>
/// Summary description for Financial_Transfer
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class Payment
    {
        public Payment()
        {
            //
            //
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
    ((sender, certificate, chain, sslPolicyErrors) => true);
        }

        public string TOPUP(Hashtable hashTbl, string ip, string user_agent)
        {
            // Thread.Sleep(10000);
            string retStr = Config.TOPUP;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");

            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME");

            //gan lai des name
            des_name = (des_name == Config.NULL_VALUE ? "" : des_name);


            string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

            string type = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");

            string order_id = Funcs.getValFromHashtbl(hashTbl, "ORDER_ID");

            string partner_id = Funcs.getValFromHashtbl(hashTbl, "PARTNER_ID");
            string category_code = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_CODE");

            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");
            string service_code = Funcs.getValFromHashtbl(hashTbl, "SERVICE_CODE");

            string pinNum = Funcs.getValFromHashtbl(hashTbl, "PINNUM");
            double discountAmount = (Funcs.getValFromHashtbl(hashTbl, "AMOUNTVAL").Length > 0) ? double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNTVAL")) : 0;

            //TOPUP MOBILE PRICE là mệnh giá, topup khác PRICE = 0
            double price = double.Parse(Funcs.getValFromHashtbl(hashTbl, "PRICE"));

            string tran_type = "";
            if (type == "MOBILE")
            {
                tran_type = Config.TRAN_TYPE_TOPUP_MOBILE;
            }
            else
            {
                tran_type = Config.TRAN_TYPE_TOPUP_OTHER;
            }

            Funcs.WriteLog("custid:" + custid + "|TOPUP BEGIN   DES_ACCT:" + order_id);

            string des_acct = "";
            string check_before_trans = "";

            double amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            string pos_cd = "";
            pos_cd = Config.HO_BR_CODE;

            string core_txno_ref = "";
            string retPartnerWS = "";
            string core_txdate_ref = "";
            string channel_id = Config.ChannelID;

            double eb_tran_id = 0;
            bool checkVoucher = false;
            bool updateVoucher = true;
            //1. Tùy vào partner, type để lấy thông tin tài khoản đích: 
            // TK của vnpay topup và billing đang khác nhau



            if (partner_id == Config.PARTNER_ID_VNPAY)
            {
                des_acct = Config.ACCT_SUSPEND_VNPAY_TOPUP;
            }
            else if (partner_id == Config.PARTNER_ID_VAS)
            {
                /*

                    public static String SMLBILL_GL_SUSPEND = "9230387044";
                    public static String SMLBILL_GL_FEE = "9230387044";
                    public static String SMLBILL_GL_VAT = "9230387044";

                 */
                des_acct = Config.ACCT_SUSPEND_VAS;
                //gl_fee = Config.SMLBILL_GL_FEE;
                //gl_vat = Config.SMLBILL_GL_VAT;
            }
            //ONEPAY CHI CO BILLING
            //else if (partner_id == Config.PARTNER_ID_ONEPAY)
            //{
            //    des_acct = Config.ACCT_SUSPEND_ONEPAY;
            //}

            else if (partner_id == Config.PARTNER_ID_NLUONG)
            {
                des_acct = Config.ACCT_SUSPEND_NLUONG;
            }
            else
            {
                return retStr = Config.ERR_MSG_GENERAL;
            }


            save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            //insert vào bảng ebank transaction trạng thái chờ


            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            //linhtn fix 2017 02 07
            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }

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

            if (amount != price)
            {
                return Config.ERR_MSG_GENERAL;
            }

            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                if (!String.IsNullOrEmpty(pinNum))
                {
                    EbankVoucherCheckModel modelCheck = new EbankVoucherCheckModel();

                    modelCheck.PinNum = pinNum;
                    modelCheck.ChannelId = Config.ChannelIDVoucher;
                    modelCheck.CustomerId = custid;
                    modelCheck.TranAmount = amount;
                    modelCheck.TranType = tran_type;
                    modelCheck.DiscountAmount = discountAmount;

                    checkVoucher = EVoucher.CheckVoucher(modelCheck, custid);
                }
                else
                {
                    checkVoucher = true;
                }

                if (!checkVoucher)
                {
                    return Config.ERR_MSG_GENERAL;
                }

                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //for ngan luong
                NLBillPmt.NLBillPmtVerifyResType invoiceObj = null;

                Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id
                    + "BEGIN INSERT EB TRAN");

                double amountAfterDiscount = amount;

                if (!String.IsNullOrEmpty(pinNum))
                {
                    amountAfterDiscount = amount - discountAmount;
                }

                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amountAfterDiscount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amountAfterDiscount //lcy_amount
                    , txdesc + " " + order_id //txdesc: Linhtn fix 2017 02 06: theo comment cua anh ThangNC: dien giai them so dien thoai
                                              //, txdesc + " " + order_id //txdesc
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
                    , price // order_amount
                    , amountAfterDiscount // order_dis
                    , order_id //order_id
                    , partner_id //partner code
                    , category_code //category code
                    , service_code //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huong
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , category_id //bm1 --> category_id
                    , service_id //bm2 --> service_id
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
                    , pinNum //bm15
                    , discountAmount.ToString() //bm16
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
                    , ip//  "" //bm28
                    , user_agent// ""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    if (!String.IsNullOrEmpty(pinNum))
                    {
                        EbankVoucherUpdateModel modelUpdate = new EbankVoucherUpdateModel();

                        modelUpdate.ChannelId = Config.ChannelIDVoucher;
                        modelUpdate.CustomerId = custid;
                        modelUpdate.PinNum = pinNum;
                        modelUpdate.TranAmount = amount;
                        modelUpdate.TranRefNo = eb_tran_id.ToString();
                        modelUpdate.TranType = tran_type;
                        modelUpdate.DiscountAmount = discountAmount;

                        updateVoucher = EVoucher.UpdateVoucher(modelUpdate, custid);
                    }

                    if (!updateVoucher)
                    {
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                        return Config.ERR_MSG_GENERAL;
                    }

                    //retStr = Config.FUNDTRANSFER_INTRA;

                    //HẠCH TOÁN VÀO CORE BANKING, TÙY THEO TRAN TYPE, ĐỐI TÁC MÀ HẠCH TOÁN KHÁC NHAU
                    Payments pm = new Payments();

                    //xu ly them phan goi ws ngan luong de sinh ra ma hoa don
                    if (partner_id == Config.PARTNER_ID_NLUONG)
                    {

                        Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id + "CASE NLUONG GET INVOICE");

                        invoiceObj = new TopupBillingIntergrator().getNLInvoiceCode(order_id, amount.ToString(), custid);

                        Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id + "CASE NLUONG END GET INVOICE:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(invoiceObj)));

                        if (invoiceObj.RespSts != null && invoiceObj.RespSts.ErrInfo != null &&
                            invoiceObj.RespSts.ErrInfo.Length > 0 && invoiceObj.RespSts.ErrInfo[0] != null)
                        {
                            for (int i = 2; i < Config.RET_CODE_NLUONG.Length; i++)
                            {
                                if (Config.RET_CODE_NLUONG[i].Split('|')[0].Equals(invoiceObj.RespSts.ErrInfo[0].ErrCd))
                                {
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, string.Empty, string.Empty, Config.ChannelID);

                                    return Config.ERR_MSG_GENERAL;
                                }
                            }
                        }
                    }

                    Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id + "BEGIN POST TO CORE");

                    //string result = tf.postFINPOSTToCore
                    string result = CoreIntegration.postFINPOSTToCore
                    (custid
                    , tran_type
                    , eb_tran_id
                    , src_acct
                    , des_acct //gl suspend
                    , "" //gl fee
                    , ""// gl vat
                    , amountAfterDiscount  // suspend amount
                    , 0 // fee amount
                    , 0 //vat amount
                    //, txdesc
                    , txdesc + " " + order_id //txdesc: Linhtn fix 2017 02 06: theo comment cua anh ThangNC: dien giai them so dien thoai
                    , pos_cd
                    , ref core_txno_ref
                    , ref core_txdate_ref
                    );

                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {

                        Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id
                        + "BEGIN POST TO CORE DONE");
                        // GOI HAM TOPUP
                        string info_ref = "";
                        retPartnerWS = pm.postTopupToPartner(
                            eb_tran_id
                            , custid
                            , src_acct
                            , price
                            , order_id
                            , partner_id
                            , category_code
                            , service_code
                            //, txdesc
                            , txdesc + " " + order_id //txdesc: Linhtn fix 2017 02 06: theo comment cua anh ThangNC: dien giai them so dien thoai
                            , price.ToString()
                            , invoiceObj // extra info
                            , ref info_ref
                            );
                        Funcs.WriteLog("custid:" + custid + "TOPUP RET FROM PARTNER tran_id:" + eb_tran_id
                                        + "|RETWS:" + retPartnerWS);
                        // neu topup thanh cong
                        if (retPartnerWS == Config.ERR_CODE_DONE)
                        {
                            //xu ly tiep save to ben list o duoi

                        }
                        if (retPartnerWS == Config.ERR_CODE_TIMEOUT)
                        {
                            //TIMEOUT CUNG TINH LA STATUS EBANK TRAN = 1. + HAN MUC GIAO DICH
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                            return Config.ERR_MSG_TIMEOUT;

                        }
                        // endif neu topup thanh cong
                        if (retPartnerWS == Config.ERR_CODE_REVERT)// neu tra ve loi cu the thi revert                                    
                        {
                            //string revStr = tf.revTransderTx(eb_tran_id, txdesc);

                            Funcs.WriteLog("custid:" + custid + "TOPUP begin call revert finpost tran_id:" + eb_tran_id);

                            //string revStr = tf.revFinPost(eb_tran_id, txdesc);
                            string revStr = CoreIntegration.revFinPost(eb_tran_id, txdesc);


                            if (revStr == Config.gResult_INTELLECT_Arr[0])
                            {
                                Funcs.WriteLog("custid:" + custid + "TOPUP revert successfull tran_id:" + eb_tran_id);
                            }

                            else
                            {
                                Funcs.WriteLog("custid:" + custid + "TOPUP revert not succesfull tran_id:" + eb_tran_id);
                            }
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);

                            return Config.ERR_MSG_GENERAL;

                        }
                        if (retPartnerWS == Config.ERR_CODE_GENERAL)//                          
                        {
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                            return Config.ERR_MSG_GENERAL;
                        }

                        // CAP NHAT TRANG THAI VAO BANG TBL_EB_TRAN
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                        //SAVE TO BEN LIST
                        //khong can save to ben list
                        if (save_to_benlist != "1")
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "TOPUP IS COMPLETED TRAN_ID= " + eb_tran_id);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
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
                                , order_id//des_acct   ---> save số dt thay cho acct
                                , des_name
                                , des_name// nap tien de 2 truong nay bang nhau
                                , txdesc
                                , "" //bank_code
                                , "" //bank_name
                                , "" //bank_branch
                                , "" //bank_city
                                , category_id //category_id
                                , service_id //service_id
                                , "" //lastchange default = sysdate da xu ly o db
                                , ""// bm1
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
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "TOPUP IS COMPLETED TRAN_ID=" + eb_tran_id);
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                            }
                            else
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "TOPUP IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
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
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                }
            }
            else //Check before trans
            {
                retStr = check_before_trans;
            }

            Funcs.WriteLog("custid:" + custid + "|TOPUP END   DES_ACCT:" + order_id);

            return retStr;

        }


        public string TOPUPByCreditCard(Hashtable hashTbl, string ip, string user_agent)
        {
            // Thread.Sleep(10000);
            string retStr = Config.TOPUP;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");

            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME");

            //gan lai des name
            des_name = (des_name == Config.NULL_VALUE ? "" : des_name);


            string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

            string type = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");

            string order_id = Funcs.getValFromHashtbl(hashTbl, "ORDER_ID");

            string partner_id = Funcs.getValFromHashtbl(hashTbl, "PARTNER_ID");
            string category_code = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_CODE");

            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");
            string service_code = Funcs.getValFromHashtbl(hashTbl, "SERVICE_CODE");

            string pinNum = Funcs.getValFromHashtbl(hashTbl, "PINNUM");
            double discountAmount = (Funcs.getValFromHashtbl(hashTbl, "AMOUNTVAL").Length > 0) ? double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNTVAL")) : 0;

            //TOPUP MOBILE PRICE là mệnh giá, topup khác PRICE = 0
            double price = double.Parse(Funcs.getValFromHashtbl(hashTbl, "PRICE"));

            string tran_type = "";
            if (type == "MOBILE")
            {
                tran_type = Config.TRAN_TYPE_TOPUP_MOBILE;
            }
            else
            {
                tran_type = Config.TRAN_TYPE_TOPUP_OTHER;
            }

            Funcs.WriteLog("custid:" + custid + "|TOPUP BEGIN   DES_ACCT:" + order_id);

            string des_acct = "";
            string check_before_trans = "";

            double amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            string pos_cd = "";
            pos_cd = Config.HO_BR_CODE;

            string core_txno_ref = "";
            string retPartnerWS = "";
            string core_txdate_ref = "";
            string channel_id = Config.ChannelID;

            string ref_no_to_svfe = "";
            string set_date_to_svfe = "";

            double eb_tran_id = 0;
            bool checkVoucher = false;
            bool updateVoucher = true;
            //1. Tùy vào partner, type để lấy thông tin tài khoản đích: 
            // TK của vnpay topup và billing đang khác nhau



            if (partner_id == Config.PARTNER_ID_VNPAY)
            {
                des_acct = Config.ACCT_SUSPEND_VNPAY_TOPUP;
            }
            else if (partner_id == Config.PARTNER_ID_VAS)
            {
                /*

                    public static String SMLBILL_GL_SUSPEND = "9230387044";
                    public static String SMLBILL_GL_FEE = "9230387044";
                    public static String SMLBILL_GL_VAT = "9230387044";

                 */
                des_acct = Config.ACCT_SUSPEND_VAS;
                //gl_fee = Config.SMLBILL_GL_FEE;
                //gl_vat = Config.SMLBILL_GL_VAT;
            }
            //ONEPAY CHI CO BILLING
            //else if (partner_id == Config.PARTNER_ID_ONEPAY)
            //{
            //    des_acct = Config.ACCT_SUSPEND_ONEPAY;
            //}

            else if (partner_id == Config.PARTNER_ID_NLUONG)
            {
                des_acct = Config.ACCT_SUSPEND_NLUONG;
            }
            else
            {
                return retStr = Config.ERR_MSG_GENERAL;
            }

            pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            //insert vào bảng ebank transaction trạng thái chờ


            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            //linhtn fix 2017 02 07
            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            //if (src_acct.Length <= 10)
            //{
            //    bool check = Auth.CustIdMatchScrAcct(custid, src_acct);

            //    if (!check)
            //    {
            //        return Config.ERR_MSG_GENERAL;
            //    }

            //}


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

            if (amount != price)
            {
                return Config.ERR_MSG_GENERAL;
            }

            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                if (!String.IsNullOrEmpty(pinNum))
                {
                    EbankVoucherCheckModel modelCheck = new EbankVoucherCheckModel();

                    modelCheck.PinNum = pinNum;
                    modelCheck.ChannelId = Config.ChannelIDVoucher;
                    modelCheck.CustomerId = custid;
                    modelCheck.TranAmount = amount;
                    modelCheck.TranType = tran_type;
                    modelCheck.DiscountAmount = discountAmount;

                    checkVoucher = EVoucher.CheckVoucher(modelCheck, custid);
                }
                else
                {
                    checkVoucher = true;
                }

                if (!checkVoucher)
                {
                    return Config.ERR_MSG_GENERAL;
                }


                //for ngan luong
                NLBillPmt.NLBillPmtVerifyResType invoiceObj = null;

                Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id
                    + "BEGIN INSERT EB TRAN");

                double amountAfterDiscount = amount;

                if (!String.IsNullOrEmpty(pinNum))
                {
                    amountAfterDiscount = amount - discountAmount;
                }


                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amountAfterDiscount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amountAfterDiscount //lcy_amount
                    , txdesc + " " + order_id //txdesc: Linhtn fix 2017 02 06: theo comment cua anh ThangNC: dien giai them so dien thoai
                                              //, txdesc + " " + order_id //txdesc
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
                    , price // order_amount
                    , amountAfterDiscount // order_dis
                    , order_id //order_id
                    , partner_id //partner code
                    , category_code //category code
                    , service_code //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huong
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , category_id //bm1 --> category_id
                    , service_id //bm2 --> service_id
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
                    , pinNum //bm15
                    , discountAmount.ToString() //bm16
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
                    , ip//  "" //bm28
                    , user_agent// ""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    //retStr = Config.FUNDTRANSFER_INTRA;

                    if (!String.IsNullOrEmpty(pinNum))
                    {
                        EbankVoucherUpdateModel modelUpdate = new EbankVoucherUpdateModel();

                        modelUpdate.ChannelId = Config.ChannelIDVoucher;
                        modelUpdate.CustomerId = custid;
                        modelUpdate.PinNum = pinNum;
                        modelUpdate.TranAmount = amount;
                        modelUpdate.TranRefNo = eb_tran_id.ToString();
                        modelUpdate.TranType = tran_type;
                        modelUpdate.DiscountAmount = discountAmount;

                        updateVoucher = EVoucher.UpdateVoucher(modelUpdate, custid);
                    }

                    if (!updateVoucher)
                    {
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                        return Config.ERR_MSG_GENERAL;
                    }

                    //HẠCH TOÁN VÀO CORE BANKING, TÙY THEO TRAN TYPE, ĐỐI TÁC MÀ HẠCH TOÁN KHÁC NHAU
                    Payments pm = new Payments();

                    //xu ly them phan goi ws ngan luong de sinh ra ma hoa don
                    if (partner_id == Config.PARTNER_ID_NLUONG)
                    {

                        Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id + "CASE NLUONG GET INVOICE");

                        invoiceObj = new TopupBillingIntergrator().getNLInvoiceCode(order_id, amount.ToString(), custid);

                        Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id + "CASE NLUONG END GET INVOICE:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(invoiceObj)));

                        if (invoiceObj.RespSts != null && invoiceObj.RespSts.ErrInfo != null &&
                            invoiceObj.RespSts.ErrInfo.Length > 0 && invoiceObj.RespSts.ErrInfo[0] != null)
                        {
                            for (int i = 2; i < Config.RET_CODE_NLUONG.Length; i++)
                            {
                                if (Config.RET_CODE_NLUONG[i].Split('|')[0].Equals(invoiceObj.RespSts.ErrInfo[0].ErrCd))
                                {
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, string.Empty, string.Empty, Config.ChannelID);

                                    return Config.ERR_MSG_GENERAL;
                                }
                            }
                        }
                    }

                    Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id + "BEGIN POST TO CORE");
                    //chuyenlt1
                    //Doi voi TOPUP qua the Credit
                    //Goi ham Purcharse CardAPI => Goi FinPost ToCore

                    var ref_to_card_api = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

                    if (src_acct.Length > 10)
                    {
                        string tmpAmt = amountAfterDiscount.ToString() + "00";
                        tmpAmt = tmpAmt.ToString().PadLeft(12, '0');
                        CardAPI.PurchaseResType res = CardIntegration.purcharseCardAPI(
                                                        custid,
                                                        src_acct,
                                                        ref_to_card_api,
                                                        tmpAmt,
                                                        Funcs.getConfigVal("BILLING_TOPUP_MCC"),
                                                        Funcs.getConfigVal("BILLING_TOPUP_TID"),
                                                        Config.CardAPIConfig.ServiceCd,
                                                        Funcs.getConfigVal("BILLING_TOPUP_MID")
                                                        );
                        if (res != null && res.RespSts.Sts.Equals("0"))
                        {
                            ref_no_to_svfe = res.RefNo;
                            set_date_to_svfe = res.SettDate;
                            //string result = tf.postFINPOSTToCore

                            string src_acc_credit_gl = Funcs.GetAccGLCredit(src_acct);

                            if (string.IsNullOrEmpty(src_acc_credit_gl))
                            {
                                return Config.ERR_MSG_GENERAL;
                            }

                            string result = CoreIntegration.postFINPOSTToCore
                                                            (custid
                                                            , tran_type
                                                            , eb_tran_id
                                                            , src_acc_credit_gl
                                                            , des_acct //gl suspend
                                                            , "" //gl fee
                                                            , ""// gl vat
                                                            , amountAfterDiscount // suspend amount
                                                            , 0 // fee amount
                                                            , 0 //vat amount
                                                                //, txdesc
                                                            , txdesc + " " + order_id //txdesc: Linhtn fix 2017 02 06: theo comment cua anh ThangNC: dien giai them so dien thoai
                                                            , pos_cd
                                                            , ref core_txno_ref
                                                            , ref core_txdate_ref
                                                            );

                            //NEU HACH TOAN VAO CORE THANH CONG
                            if (result == Config.gResult_INTELLECT_Arr[0])
                            {
                                Funcs.WriteLog("custid:" + custid + "|TOPUP DES_ACCT:" + order_id + "BEGIN POST TO CORE DONE");
                                // GOI HAM TOPUP
                                string info_ref = "";
                                retPartnerWS = pm.postTopupToPartner(
                                    eb_tran_id
                                    , custid
                                    , ConfigurationManager.AppSettings.Get("ACCT_GL_CARD")
                                    , price //tren con that da sua amount to price
                                    , order_id
                                    , partner_id
                                    , category_code
                                    , service_code
                                    //, txdesc
                                    , txdesc + " " + order_id //txdesc: Linhtn fix 2017 02 06: theo comment cua anh ThangNC: dien giai them so dien thoai
                                    , price.ToString()
                                    , invoiceObj // extra info
                                    , ref info_ref
                                    );

                                Funcs.WriteLog("custid:" + custid + "TOPUP RET FROM PARTNER tran_id:" + eb_tran_id
                                                + "|RETWS:" + retPartnerWS);
                                // neu topup thanh cong
                                if (retPartnerWS == Config.ERR_CODE_DONE)
                                {
                                    //xu ly tiep save to ben list o duoi

                                }
                                if (retPartnerWS == Config.ERR_CODE_TIMEOUT)
                                {
                                    //TIMEOUT CUNG TINH LA STATUS EBANK TRAN = 1. + HAN MUC GIAO DICH
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                                    return Config.ERR_MSG_TIMEOUT;

                                }
                                // endif neu topup thanh cong
                                if (retPartnerWS == Config.ERR_CODE_REVERT)// neu tra ve loi cu the thi revert                                    
                                {
                                    //string revStr = tf.revTransderTx(eb_tran_id, txdesc);

                                    Funcs.WriteLog("custid:" + custid + "TOPUP begin call revert finpost tran_id:" + eb_tran_id);

                                    //string revStr = tf.revFinPost(eb_tran_id, txdesc);
                                    string revStr = CoreIntegration.revFinPost(eb_tran_id, txdesc);


                                    if (revStr == Config.gResult_INTELLECT_Arr[0])
                                    {
                                        Funcs.WriteLog("custid:" + custid + "TOPUP revert successfull tran_id:" + eb_tran_id);

                                        CardAPI.ReversalResType revertRes = CardIntegration.revertCardAPI(
                                                                    custid,
                                                                    src_acct,
                                                                    ref_to_card_api,
                                                                    tmpAmt,
                                                                    ref_no_to_svfe,
                                                                    set_date_to_svfe,
                                                                    Funcs.getConfigVal("BILLING_TOPUP_MCC"),
                                                                    Funcs.getConfigVal("BILLING_TOPUP_TID"),
                                                                    Config.CardAPIConfig.ServiceCd,
                                                                    Funcs.getConfigVal("BILLING_TOPUP_MID")
                                                                );

                                        if (revertRes != null && revertRes.RespSts.Sts.Equals("0"))
                                        {
                                            Funcs.WriteLog("custid:" + custid + "TOPUP revert to SVFE successfull ref_no_to_svfe:" + ref_no_to_svfe);
                                        }
                                        else
                                        {
                                            Funcs.WriteLog("custid:" + custid + "TOPUP revert to SVFE not successfull ref_no_to_svfe:" + ref_no_to_svfe);
                                        }
                                    }

                                    else
                                    {
                                        Funcs.WriteLog("custid:" + custid + "TOPUP revert not succesfull tran_id:" + eb_tran_id);
                                    }
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);

                                    return Config.ERR_MSG_GENERAL;

                                }
                                if (retPartnerWS == Config.ERR_CODE_GENERAL)//                          
                                {
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                                    return Config.ERR_MSG_GENERAL;
                                }

                                // CAP NHAT TRANG THAI VAO BANG TBL_EB_TRAN
                                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                                //SAVE TO BEN LIST
                                //khong can save to ben list
                                if (save_to_benlist != "1")
                                {
                                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "TOPUP IS COMPLETED TRAN_ID= " + eb_tran_id);
                                    retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                    retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
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
                                        , order_id//des_acct   ---> save số dt thay cho acct
                                        , des_name
                                        , des_name// nap tien de 2 truong nay bang nhau
                                        , txdesc
                                        , "" //bank_code
                                        , "" //bank_name
                                        , "" //bank_branch
                                        , "" //bank_city
                                        , category_id //category_id
                                        , service_id //service_id
                                        , "" //lastchange default = sysdate da xu ly o db
                                        , ""// bm1
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
                                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "TOPUP IS COMPLETED TRAN_ID=" + eb_tran_id);
                                        retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                        retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                    }
                                    else
                                    {
                                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "TOPUP IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);
                                        retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                        retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                    }
                                    //giai phong bo nho                            
                                    ben = null;
                                    dt = null;

                                }
                            }
                            //NEU HACH TOAN VAO CORE KHONG THANH CONG
                            //else
                            //{
                            //    CardAPI.ReversalResType revertRes = CardIntegration.revertCardAPI(
                            //                                        custid,
                            //                                        src_acct,
                            //                                        eb_tran_id.ToString(),
                            //                                        tmpAmt,
                            //                                        ref_no_to_svfe,
                            //                                        set_date_to_svfe,
                            //                                        Config.CardAPIConfig.MerchantType,
                            //                                        Config.CardAPIConfig.TermId,
                            //                                        Config.CardAPIConfig.ServiceCd
                            //                                    );

                            //    if (revertRes != null && revertRes.RespSts.Sts.Equals("0"))
                            //    {
                            //        Funcs.WriteLog("custid:" + custid + "TOPUP revert to SVFE successfull ref_no_to_svfe:" + ref_no_to_svfe);
                            //    }
                            //    else
                            //    {
                            //        Funcs.WriteLog("custid:" + custid + "TOPUP revert to SVFE not successfull ref_no_to_svfe:" + ref_no_to_svfe);
                            //    }

                            //    retStr = Config.ERR_MSG_GENERAL;
                            //}
                        }
                        else
                        {
                            retStr = res.RespSts.ErrInfo[0].ErrCd.ToString();
                            return retStr;
                        }

                    }

                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                }
            }
            else //Check before trans
            {
                retStr = check_before_trans;
            }

            Funcs.WriteLog("custid:" + custid + "|TOPUP END   DES_ACCT:" + order_id);

            return retStr;

        }


        /*
         public static String GET_CATEGORY_BY_TRAN_TYPE= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "TRAN_TYPE" + COL_DLMT + "{TRAN_TYPE}"
			+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

Cấu trúc của RECORD: 

CATEGORY_ID|CATEGORY_NAME|CATEGORY_NAME_EN|CATEGORY_DESC|BM1|BM2|BM3|BM4|BM5|BM6|BM7|BM8|BM9

             */
        // public DataSet GET_CATEGORY_BY_TRAN_TYPE(string tran_type)
        public string GET_CATEGORY_BY_TRAN_TYPE(Hashtable hashTbl)
        {
            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string retStr = Config.GET_CATEGORY_BY_TRAN_TYPE;

            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Payments da = new Payments();
                DataTable dt = new DataTable();
                dt = da.GET_CATEGORY_BY_TRAN_TYPE(tran_type);

                if (dt != null && dt.Rows.Count > 0)
                {

                    //CATEGORY_ID|CATEGORY_NAME|CATEGORY_NAME_EN|CATEGORY_DESC|BM1|BM2|BM3|BM4|BM5|BM6|BM7|BM8|BM9

                    string strTemp = "";
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        strTemp = strTemp +
                            (dt.Rows[j][PAY_CATEGORY.CATEGORY_ID] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_CATEGORY.CATEGORY_ID].ToString())
                            + Config.COL_REC_DLMT +
                            (dt.Rows[j][PAY_CATEGORY.CATEGORY_NAME] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_CATEGORY.CATEGORY_NAME].ToString())
                            + Config.COL_REC_DLMT +
                            (dt.Rows[j][PAY_CATEGORY.CATEGORY_NAME_EN] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_CATEGORY.CATEGORY_NAME_EN].ToString())
                            + Config.COL_REC_DLMT +
                            (dt.Rows[j][PAY_CATEGORY.CATEGORY_DESC] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_CATEGORY.CATEGORY_DESC].ToString())
                            + Config.COL_REC_DLMT +
                            (dt.Rows[j][PAY_CATEGORY.BM1] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_CATEGORY.BM1].ToString())
                               + Config.COL_REC_DLMT +
                            (dt.Rows[j][PAY_CATEGORY.BM2] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_CATEGORY.BM2].ToString())
                               + Config.COL_REC_DLMT +
                            (dt.Rows[j][PAY_CATEGORY.BM3] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_CATEGORY.BM3].ToString())
                               + Config.COL_REC_DLMT
                             + Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = Config.GET_CATEGORY_BY_TRAN_TYPE;
                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    return retStr;
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch (Exception ex)
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        /*
        
            "REQ=CMD#GET_SERVICES|CIF_NO#0310005018|CATEGORY#MOBILE|TRAN_TYPE#TOPUP_MOBILE|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

Response

public static String GET_SERVICES= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "TRAN_TYPE" + COL_DLMT + "{TRAN_TYPE}"
			+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";
Cấu trúc của RECORD: 

SERVICE_ID|SERVICE_NAME|SERVICE_NAME_EN|SERVICE_DESC|PAYCODE_LBL|PAYCODE_LBL_EN|BM1|BM2|BM3|BM4|BM5|BM6|BM7|BM8|BM9

Trong đó: 
PAYCODE_LBL: Mã hóa đơn/ Mã hợp đồng
PAYCODE_LBL_EN: tiếng Anh của PAYCODE_LBL
 
        */

        //public  string GET_SERVICES(string category_id, string tran_type)
        public string GET_SERVICES(Hashtable hashTbl)
        {
            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            String retStr = Config.GET_SERVICES;

            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Payments da = new Payments();
                DataTable dt = new DataTable();
                dt = da.GET_SERVICES(category_id, tran_type);

                //2. Gen reply message
                if (dt != null && dt.Rows.Count > 0)
                {

                    //SERVICE_ID|SERVICE_NAME|SERVICE_NAME_EN|SERVICE_DESC|PAYCODE_LBL|PAYCODE_LBL_EN|PARTNER_ID|SERVICE_CODE|CATEGORY_ID|CATEGORY_CODE|BM1|BM2|BM3|BM4|BM5|BM6|BM7|BM8|BM9

                    string strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_ID] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_NAME] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_NAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_NAME_EN] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_NAME_EN].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_DESC] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_DESC].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.PAYCODE_LBL] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.PAYCODE_LBL].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.PAYCODE_LBL_EN] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.PAYCODE_LBL_EN].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SER_PART.PARTNER_ID] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SER_PART.PARTNER_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_CODE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.CATEGORY_ID] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.CATEGORY_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SER_PART.CATEGORY_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SER_PART.CATEGORY_CODE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_TYPE] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_TYPE].ToString()) + Config.COL_REC_DLMT;

                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_PAYPRICE_TYPE] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_PAYPRICE_TYPE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM2] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM2].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM3] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM3].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM4] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM4].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM5] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM5].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM6] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM6].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM7] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM7].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM8] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM8].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM9] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM9].ToString())
                            + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = Config.GET_ACCT_LIST;
                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    return retStr;

                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch (Exception ex)
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        /*
public DataSet GET_LIST_MOBILE_PRICE(string category_id, string mobile_no)
        */
        public string GET_LIST_MOBILE_PRICE(string mobil_no)
        {

            String retStr = Config.GET_LIST_MOBILE_PRICE;
            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Payments da = new Payments();
                DataTable dt = new DataTable();
                dt = da.GET_LIST_MOBILE_PRICE(mobil_no);

                //ds format:
                // CASA_TOTAL | TIDE_TOTAL | LOAN_TOTAL | AC_TYPE| ACCTNO | CCYCD
                //2. Gen reply message
                if (dt != null && dt.Rows.Count > 0)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    retStr = retStr.Replace("{PARTNER_ID}", dt.Rows[0]["PARTNER_ID"].ToString());
                    retStr = retStr.Replace("{CATEGORY_ID}", dt.Rows[0]["CATEGORY_ID"].ToString());
                    retStr = retStr.Replace("{SERVICE_ID}", dt.Rows[0]["SERVICE_ID"].ToString());
                    retStr = retStr.Replace("{SERVICE_CODE}", dt.Rows[0]["SERVICE_CODE"].ToString());
                    retStr = retStr.Replace("{CATEGORY_CODE}", dt.Rows[0]["CATEGORY_CODE"].ToString());

                    string strTemp = "";
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        // (dt.Rows[j][ PAY_SERVICE.SERVICE_ID] ==  DBNull.Value ? "_NULL_": dt.Rows[j][ PAY_SERVICE.SERVICE_ID].ToString()) 

                        strTemp = strTemp +
                            (dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_VALUE] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_VALUE].ToString())
                            + Config.COL_REC_DLMT +

                            (dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_DISCOUNT] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_DISCOUNT].ToString())
                            + Config.COL_REC_DLMT

                        + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);

                    return retStr;

                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch (Exception ex)
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        public string GET_PRICE_TOPUP_OTHER(Hashtable hashTbl)
        {

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO"); //	

            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");

            String retStr = Config.GET_PRICE_TOPUP_OTHER;
            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Payments da = new Payments();
                DataTable dt = new DataTable();
                dt = da.GET_PRICE_TOPUP_OTHER(tran_type, category_id, service_id);

                //ds format:
                // CASA_TOTAL | TIDE_TOTAL | LOAN_TOTAL | AC_TYPE| ACCTNO | CCYCD
                //2. Gen reply message
                if (dt != null && dt.Rows.Count > 0)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    retStr = retStr.Replace("{PARTNER_ID}", dt.Rows[0]["PARTNER_ID"].ToString());
                    retStr = retStr.Replace("{CATEGORY_ID}", dt.Rows[0]["CATEGORY_ID"].ToString());
                    retStr = retStr.Replace("{SERVICE_ID}", dt.Rows[0]["SERVICE_ID"].ToString());
                    retStr = retStr.Replace("{SERVICE_CODE}", dt.Rows[0]["SERVICE_CODE"].ToString());
                    retStr = retStr.Replace("{CATEGORY_CODE}", dt.Rows[0]["CATEGORY_CODE"].ToString());

                    string strTemp = "";
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        // (dt.Rows[j][ PAY_SERVICE.SERVICE_ID] ==  DBNull.Value ? "_NULL_": dt.Rows[j][ PAY_SERVICE.SERVICE_ID].ToString()) 

                        strTemp = strTemp +
                            (dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_VALUE] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_VALUE].ToString())
                            + Config.COL_REC_DLMT +

                            (dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_DISCOUNT] == DBNull.Value ? "_NULL_" : dt.Rows[j][PAY_PRICE_DISCOUNT.PRICE_DISCOUNT].ToString())
                            + Config.COL_REC_DLMT

                        + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);

                    return retStr;

                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch (Exception ex)
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        #region "Billing"


        public string getBillInfo(Hashtable hashTbl)
        {
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            /*
             REQ=CMD#GET_BILL_INFO|CIF_NO#0310008705|BILL_ID#0979718888|CATEGORY#TELECOM|SERVICE#VNPT_HOME|TRANPWD#fksdfjf385738jsdfjsdf9|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

       
Response

public static String GET_BILL_INFO= "ERR_CODE" + COL_DLMT + "{ERR_CODE}" 
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
			+ ROW_DLMT + "BILL_ID" + COL_DLMT + "{BILL_ID}"
+ ROW_DLMT + "BILL_AMOUNT" + COL_DLMT + "{BILL_AMOUNT}"
+ ROW_DLMT + "BILL_INFO_EXT1" + COL_DLMT + "{BILL_INFO_EXT1}"
+ ROW_DLMT + "BILL_INFO_EXT2" + COL_DLMT + "{BILL_INFO_EXT2}"

			;        	
             */
            string retStr = Config.GET_BILL_INFO;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO"); //	
            string bill_id = Funcs.getValFromHashtbl(hashTbl, "BILL_ID"); //	
            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");
            string partner_id = Funcs.getValFromHashtbl(hashTbl, "PARTNER_ID"); ;
            string category_code = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_CODE");
            string service_code = Funcs.getValFromHashtbl(hashTbl, "SERVICE_CODE");
            string amount = "";
            string info_ext1 = "";
            string info_ext2 = "";
            string tran_type = "";
            Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO BEGIN|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id);

            //nếu category_code = 'MOBILE' --> goi ham lay service code
            // Cho tin hieu cua Cong TT
            if (category_id == "MOBILE")
            {

                tran_type = Config.TRAN_TYPE_BILL_MOBILE;
                Payments pms = new Payments();
                DataTable dt = new DataTable();

                dt = pms.getServicePartner_byBill(bill_id, tran_type);

                if (dt != null && dt.Rows.Count == 1)
                {
                    service_id = dt.Rows[0][PAY_SER_PART.SERVICE_ID].ToString();
                    service_code = dt.Rows[0][PAY_SER_PART.SERVICE_CODE].ToString();
                    category_code = dt.Rows[0][PAY_SER_PART.CATEGORY_CODE].ToString();
                    partner_id = dt.Rows[0][PAY_SER_PART.PARTNER_ID].ToString();
                }
                else
                {
                    Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id + "|KHONG LAY DUOC THONG TIN HOA DON");
                    return Config.ERR_MSG_GENERAL;
                }

                Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                    + "|FIND SER PART DONE"
                    + "|SERVICE_CODE:" + service_code
                    + "|CATEGORY_CODE:" + category_code
                    + "|PARTNER_ID:" + partner_id
               );
                //DOAN NAY DANG BI THUA
                retStr = retStr.Replace("{PARTNER_ID}", partner_id);
                retStr = retStr.Replace("{CATEGORY_ID}", category_id);
                retStr = retStr.Replace("{SERVICE_ID}", service_id);
                retStr = retStr.Replace("{CATEGORY_CODE}", category_code);
                retStr = retStr.Replace("{SERVICE_CODE}", service_code);
            }
            else if (category_id == Config.PARTNER_ID_ELECTRICITY)
            {
                bill_id = bill_id.ToUpper();
                DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(bill_id);
                if (dt.Rows.Count > 0)
                {
                    partner_id = dt.Rows[0]["PARTNER"].ToString();
                    service_code = dt.Rows[0]["SERVICE_CODE"].ToString();
                    category_code = dt.Rows[0]["CATEGORY_CODE"].ToString();
                    switch (dt.Rows[0]["PARTNER"].ToString())
                    {
                        case "PAYOO":
                            retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVNHN");
                            break;
                        case "VNPAY":
                            retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVN_ALL");
                            break;
                        case "EVNHN":
                            retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVNHN");
                            break;
                        case "EVNNPC":
                            retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVNNPC");
                            break;
                        case "EVNSPC":
                            retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVNSPC");
                            break;
                        default:
                            retStr = retStr.Replace("{SERVICE_ID}", "BILL_EVN_ALL");
                            break;
                    }

                    retStr = retStr.Replace("{PARTNER_ID}", partner_id);
                    retStr = retStr.Replace("{CATEGORY_CODE}", category_code);
                    retStr = retStr.Replace("{SERVICE_CODE}", service_code);
                }
                else
                {
                    retStr = retStr.Replace("{PARTNER_ID}", "_NULL_");
                }
            }

            Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
               + "BEGIN CALL WS" + partner_id);

            #region "CASE NAPAS VAS BILLING"
            if (partner_id == Config.PARTNER_ID_VAS)
            {
                try
                {
                    //CALL SMLBILL WS
                    NapasBillPmt.NAPASBillPmtInquiryResType res = new TopupBillingIntergrator().getBillInfoFromNapas(
                        new Payments().getNextTranId(), custid, category_id, service_id, bill_id, partner_id, service_code);

                    if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                           + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                        info_ext1 = string.IsNullOrEmpty(Funcs.NoHack(res.AdditionalData)) ? "--" : Funcs.NoHack(res.AdditionalData);
                        info_ext2 = "--";

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        try
                        {
                            if (Double.Parse(res.TxnAmt) < 0)
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                            }
                            else
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", res.TxnAmt);
                            }
                        }
                        catch (Exception)
                        {
                            retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        }
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                        retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }

            #endregion "CASE NAPAS VAS BILLING"

            #region "CASE VNPAY BILLING"
            else if (partner_id == Config.PARTNER_ID_VNPAY)
            {
                try
                {
                    string str = string.Empty;

                    VnPayBillPmt.VNPAYBillPmtInquiryResType res = new TopupBillingIntergrator().getBillInfoFromVnpay(
                        new Payments().getNextTranId(), custid, category_code, service_code, bill_id);

                    if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                        amount = "0";
                        info_ext1 = "--";
                        info_ext2 = "--";

                        if (!string.IsNullOrEmpty(res.AdditionalData))
                        {
                            String[] itemData = res.AdditionalData.Split('@');
                            try
                            {
                                //lay so tien
                                if (itemData.Length >= 4)
                                {
                                    amount = itemData[3].Trim();

                                    if (Double.Parse(amount) < 0)
                                    {
                                        amount = "0";
                                    }
                                }
                                //lay thong tin khach hang
                                if (itemData.Length >= 5 && itemData[4] != null)
                                {
                                    try
                                    {
                                        string[] valExt = itemData[4].Split('$');
                                        if (valExt[0] != null) info_ext1 = valExt[0];
                                        if (valExt[1] != null) info_ext2 = valExt[1];
                                    }
                                    catch (Exception ex)
                                    {
                                        Funcs.WriteLog(ex.ToString());
                                    }
                                }
                            }
                            catch (Exception ex) { }
                        }

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                        retStr = retStr.Replace("{BILL_AMOUNT}", amount);
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                        retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }

            #endregion "CASE VNPAY  BILLING"

            #region "CASE ONEPAY BILLING"
            else if (partner_id == Config.PARTNER_ID_ONEPAY)
            {
                try
                {
                    ONEPAYBillPmt.ONEPAYBillPmtInquiryResType res = new TopupBillingIntergrator().getBillInfoFromOnepay(
                        new Payments().getNextTranId(), custid, category_code, service_code, bill_id);

                    if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                        amount = res.EnqAmt;
                        try
                        {
                            if (Double.Parse(amount) < 0)
                            {
                                amount = "0";
                            }
                        }
                        catch (Exception ex)
                        {
                            amount = "0";
                        }

                        info_ext1 = "--";
                        info_ext2 = "--";
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                        retStr = retStr.Replace("{BILL_AMOUNT}", amount);
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                        retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }

            #endregion "CASE ONEPAY  BILLING"

            #region "CASE PAYOO BILLING"
            //linhtn: add new 2016 09 10
            else if (partner_id == Config.PARTNER_ID_PAYOO)
            {
                try
                {
                    PayooBillPmt.PayooBillPmtInquiryResType res = new TopupBillingIntergrator().getBillInfoFromPayoo(custid, bill_id);

                    if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                        string MoneyAmount = res.BillInfo.TxnAmt;
                        try
                        {
                            if (Double.Parse(MoneyAmount) < 0)
                            {
                                MoneyAmount = "0";
                            }
                        }
                        catch (Exception ex)
                        {
                            MoneyAmount = "0";
                        }
                        string CustomerName = res.BillInfo.CustName;
                        string BillId_payoo = res.BillInfo.BillId;
                        string Month_payoo = res.BillInfo.Month;

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                        retStr = retStr.Replace("{BILL_AMOUNT}", MoneyAmount);
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);

                        retStr = retStr.Replace("{BILL_INFO_EXT1}", CustomerName);
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", BillId_payoo + "*" + Month_payoo);
                        retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                        + "CALL WS" + partner_id + "|RET=" + res.RespSts.Sts);

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }

            #endregion "CASE PAYOO  BILLING"

            #region "CASE EVNHN BILLING"
            else if (partner_id == Config.PARTNER_ID_EVNHN)
            {

                return new EVNHN_().getBill(hashTbl);
            }
            #endregion

            #region "CASE HABECO BILLING"
            else if (partner_id == Config.PARTNER_ID_HABECO)
            {
                //HabecoBill.GetCustomerRespType res = new HabecoIntegration().GetCustomer(custid, bill_id);

                //if (res != null && res.ErrorCode.Equals("00"))
                //{
                //    //ma_kh$ten_kh$ten_kh2$dia_chi$dien_thoai$cn
                //    info_ext1 = (string.IsNullOrEmpty(res.ten_kh) ? "_NULL_" : res.ten_kh);

                //    info_ext2 = "--";

                //    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                //    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                //    retStr = retStr.Replace("{BILL_ID}", bill_id);
                //    retStr = retStr.Replace("{CIF_NO}", custid);
                //    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                //    retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                //    retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                //    retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                //    return retStr;

                //}
                //else
                //{
                //    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                //    retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON");
                //    retStr = retStr.Replace("{BILL_ID}", bill_id);
                //    retStr = retStr.Replace("{CIF_NO}", custid);
                //    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                //    retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                //    retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                //    retStr = retStr.Replace("{DESCRIPTION}", "");

                //    return retStr;
                //}
                HABECO.HabecoSAPGetCustomerRespType res = HabecoIntegration.HabecoSAPGetCustomer(bill_id);
                if (res != null && res.ETReturns[0].code.Equals("00"))
                {
                    info_ext1 = (string.IsNullOrEmpty(res.ESCustomer.tenKH) ? "_NULL_" : res.ESCustomer.tenKH);

                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN KHACH HANG THANH CONG");
                    retStr = retStr.Replace("{CIF_NO}", custid);
                    retStr = retStr.Replace("{BILL_ID}", res.ESCustomer.maKH);
                    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                    retStr = retStr.Replace("{PARTNER_ID}", Config.PARTNER_ID_HABECO);
                    retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                    retStr = retStr.Replace("{BILL_INFO_EXT2}", "null");
                    retStr = retStr.Replace("{CATEGORY_CODE}", "null");
                    retStr = retStr.Replace("{SERVICE_ID}", "null");
                    retStr = retStr.Replace("{SERVICE_CODE}", "null");
                    retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                    return retStr;
                }
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN KHACH HANG");
                    retStr = retStr.Replace("{CIF_NO}", custid);
                    retStr = retStr.Replace("{BILL_ID}", bill_id);
                    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                    retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                    retStr = retStr.Replace("{BILL_INFO_EXT2}", "null");
                    retStr = retStr.Replace("{CATEGORY_CODE}", "null");
                    retStr = retStr.Replace("{SERVICE_ID}", "null");
                    retStr = retStr.Replace("{SERVICE_CODE}", "null");
                    retStr = retStr.Replace("{DESCRIPTION}", "");
                    return retStr;
                }

            }
            #endregion

            #region "CASE SHBFC Billing"
            if (partner_id == Config.PARTNER_ID_SHBFC)
            {
                string desc = " THANH TOAN CHO HOP DONG SO " + bill_id;
                try
                {
                    SHBFC.GetLoanInforResType res = new SHBFCIntegration().getLoanInfo(custid, bill_id);

                    if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0") && res.statusCode.Equals("000"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                           + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                        info_ext1 = string.IsNullOrEmpty(Funcs.NoHack(res.LoanDetails.customerName)) ? "--" : Funcs.NoHack(res.LoanDetails.customerName);
                        info_ext2 = "--";


                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        try
                        {
                            if (Double.Parse(res.LoanDetails.totalAmt) < 0)
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                            }
                            else
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", res.LoanDetails.totalAmt);
                            }
                        }
                        catch (Exception)
                        {
                            retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        }
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                        retStr = retStr.Replace("{DESCRIPTION}", Funcs.NoHack(res.LoanDetails.customerName) + desc);

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL SHBFC" + partner_id + "retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog("CUSTID:" + custid + "| EXCEPTION GET LIST BILL QUAWACO " + ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }
			#endregion "CASE SHBFC BILLING"
			
            #region "CASE QUAWACO BILLING"
            if (partner_id == Config.PARTNER_ID_QUAWACO)
            {
                try
                {
                    QUAWACOBill.GetCustomerInfoRespType result = QUAWACOIntegration.GetCustomerInfo(custid,bill_id, "", "", "", "");

                    if (result == null || result.listCustInfo.Length < 1)
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "|retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                           + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(result)));

                        Funcs.WriteLog("CUSTID:" + custid + "| START GET LIST BILL QUAWACO " + bill_id);
                        try
                        {
                            QUAWACOBill.GetBillInfoByKHIDRespType listBill = QUAWACOIntegration.GetBillInfoByKHID(custid, bill_id);

                            double totalDebt = 0;

                            if (listBill != null)
                            {

                                if (listBill.errorCode.Equals("200") && listBill.listBills.Length > 0)
                                {
                                    foreach (var item in listBill.listBills)
                                    {
                                        totalDebt += Double.Parse(item.tongtien);
                                    }
                                }
                            }

                            info_ext1 = result.listCustInfo[0].tenkh;
                            info_ext2 = "--";

                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                            retStr = retStr.Replace("{BILL_ID}", bill_id);
                            retStr = retStr.Replace("{CIF_NO}", custid);
                            try
                            {
                                if (totalDebt <= 0)
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                                }
                                else
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", totalDebt.ToString());
                                }
                            }
                            catch (Exception)
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                            }

                            retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                            retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                            retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                            return retStr;
                        }
                        catch (Exception ex)
                        {
                            Funcs.WriteLog("CUSTID:" + custid + "| EXCEPTION GET LIST BILL QUAWACO " + ex.ToString());
                            return Config.ERR_MSG_GENERAL;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }

            #endregion "CASE QUAWACO BILLING"

            #region "CASE SOWACO BILLING"
            if (partner_id == Config.PARTNER_ID_SOWACO)
            {
                try
                {
                    SOWACO.GetCustomerInfoRespType result = SOWACOIntegration.GetCustomerInfo(custid, bill_id, "", "", "", "");

                    if (result == null || !result.errorCode.Equals("1"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "|retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                           + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(result)));

                        Funcs.WriteLog("CUSTID:" + custid + "| START GET LIST BILL SOWACO " + bill_id);
                        try
                        {
                            SOWACO.GetBillInfoRespType listBill = SOWACOIntegration.GetBillInfoByKHID(custid, bill_id);

                            double totalDebt = 0;

                            if (listBill != null && listBill.errorCode.Equals("1") && listBill.listBillInfos.Length > 0)
                            {
                                 foreach (var item in listBill.listBillInfos)
                                 {
                                    totalDebt += Double.Parse(item.tongTien);
                                 }
                            }

                            info_ext1 = result.customerInfo.tenKhachHang;
                            info_ext2 = "--";

                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                            retStr = retStr.Replace("{BILL_ID}", bill_id);
                            retStr = retStr.Replace("{CIF_NO}", custid);
                            try
                            {
                                if (totalDebt <= 0)
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                                }
                                else
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", totalDebt.ToString());
                                }
                            }
                            catch (Exception)
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                            }

                            retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                            retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                            retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN HD SOWASUCO " + bill_id);

                            return retStr;
                        }
                        catch (Exception ex)
                        {
                            Funcs.WriteLog("CUSTID:" + custid + "| EXCEPTION GET LIST BILL SOWACO " + ex.ToString());
                            return Config.ERR_MSG_GENERAL;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }

            #endregion "CASE SOWACO BILLING"

            #region "CASE DAWACO BILLING"
            if (partner_id == Config.PARTNER_ID_DAWACO)
            {
                try
                {
                    DAWACOBillPayments.InfoCustCheckRespType result = DAWACOIntegration.GetCustomerInfo(custid, bill_id, "", "", "", "");

                    if (result == null || !result.errorCode.Equals("00"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "|retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                           + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(result)));

                        Funcs.WriteLog("CUSTID:" + custid + "| START GET LIST BILL DAWACO " + bill_id);

                        try
                        {
                            DAWACOBillPayments.DebtCheckRespType listBill = DAWACOIntegration.GetBillInfoByKHID(custid, bill_id);

                            double totalDebt = 0;

                            if (listBill != null && listBill.errorCode.Equals("00") && listBill.customerData.Length > 0)
                            {
                                foreach (var item in listBill.customerData)
                                {
                                    totalDebt += Double.Parse(item.SO_TIEN);
                                }
                            }

                            info_ext1 = result.customerData[0].TEN_KHACH_HANG;
                            info_ext2 = "--";

                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                            retStr = retStr.Replace("{BILL_ID}", bill_id);
                            retStr = retStr.Replace("{CIF_NO}", custid);
                            try
                            {
                                if (totalDebt <= 0)
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                                }
                                else
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", totalDebt.ToString());
                                }
                            }
                            catch (Exception)
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                            }

                            retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                            retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                            retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                            return retStr;
                        }
                        catch (Exception ex)
                        {
                            Funcs.WriteLog("CUSTID:" + custid + "| EXCEPTION GET LIST BILL DAWACO " + ex.ToString());
                            return Config.ERR_MSG_GENERAL;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }

            #endregion "CASE DAWACO BILLING"

            #region "CASE EVNNPC BILLING"
            else if (partner_id == Config.PARTNER_ID_EVNNPC)
            {
                try
                {
                    EvnNPCBillPayment.QueryCustomerAddressRespType result = EVNNPCIntegration.QueryCustomerAddress(custid, bill_id, Funcs.GenESBMsgId(), "211801", "", "", "", "", bill_id.Substring(0, 6));

                    if (result == null || result.StatusCd == null || !result.StatusCd.Equals("0"))
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "|retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{BILL_ID}", bill_id);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                           + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(result)));

                        Funcs.WriteLog("CUSTID:" + custid + "| START GET LIST BILL EVNNPC " + bill_id);

                        try
                        {
                            EvnNPCBillPayment.QueryBillInfoRespType listBill = EVNNPCIntegration.QueryBillInfo(custid, bill_id, Funcs.GenESBMsgId(), "211801");

                            double totalDebt = 0;

                            if (listBill != null && listBill.StatusCd != null && listBill.StatusCd.Equals("0") && listBill.ListBillInfo.Length > 0)
                            {
                                foreach (var item in listBill.ListBillInfo)
                                {
                                    totalDebt += Double.Parse(String.IsNullOrEmpty(item.TotalAmount) ? "0": item.TotalAmount);
                                }
                            }

                            info_ext1 = result.ListCustomerAddress[0].TenKH;
                            info_ext2 = "--";

                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                            retStr = retStr.Replace("{BILL_ID}", bill_id);
                            retStr = retStr.Replace("{CIF_NO}", custid);
                            try
                            {
                                if (totalDebt <= 0)
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                                }
                                else
                                {
                                    retStr = retStr.Replace("{BILL_AMOUNT}", totalDebt.ToString());
                                }
                            }
                            catch (Exception)
                            {
                                retStr = retStr.Replace("{BILL_AMOUNT}", "0");
                            }

                            retStr = retStr.Replace("{BILL_INFO_EXT1}", info_ext1);
                            retStr = retStr.Replace("{BILL_INFO_EXT2}", info_ext2);
                            retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                            return retStr;
                        }
                        catch (Exception ex)
                        {
                            Funcs.WriteLog("CUSTID:" + custid + "| EXCEPTION GET LIST BILL EVNNPC " + ex.ToString());
                            return Config.ERR_MSG_GENERAL;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }
            #endregion

            #region "CASE EVNMN BILLING"
            else if (partner_id == Config.PARTNER_ID_EVNMN)
            {
                retStr = Config.GET_BILLS_INFO;
                try
                {
                    EVNMN.GetCustomerInfoResType result = EVNMNIntegration.GetCustomerInfo(Funcs.getConfigVal("BANK_ID"), bill_id);

                    retStr = retStr.Replace("{CIF_NO}", custid);
                    retStr = retStr.Replace("{PARTNER_ID}", Config.PARTNER_ID_EVNMN);

                    if (result == null || "".Equals(result.CustomerCode) || result.ListofBillInfo==null)
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                            + "END CALL WS" + partner_id + "|retXML:" + "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "KHONG LAY DUOC THONG TIN HOA DON QUY VE MA LOI 00");
                        retStr = retStr.Replace("{RECORD}", "");
                        retStr = retStr.Replace("{BILL_INFO_EXT1}", "--");
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{CUSTOMER_CODE}", "");
                        retStr = retStr.Replace("{DESCRIPTION}", "");

                        return retStr;
                    }
                    else
                    {
                        Funcs.WriteLog("CUSTID:" + custid + "|" + "GET_BILL_INFO|CATEGORY_ID:" + category_id + "|SERVICE_ID:" + service_id + "|BILL_ID:" + bill_id
                           + "END CALL WS" + partner_id + "retXML:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(result)));

                        retStr = retStr.Replace("{BILL_INFO_EXT1}", result.Name);
                        retStr = retStr.Replace("{BILL_INFO_EXT2}", "--");
                        retStr = retStr.Replace("{CUSTOMER_CODE}", result.CustomerCode);
                        string strTemp = "";

                        try
                        {
                           
                            foreach (var item in result.ListofBillInfo)
                            {
                                string ky = item.BillCode.Substring(4, 1);
                                string thang = item.BillCode.Substring(2, 2);
                                string ma = item.BillCode.Substring(6, 2);
                                string name = Config.EVNMNConfig.getName(ma);


                                strTemp = strTemp + item.BillCode
                                    + Config.COL_REC_DLMT + name
                                    + Config.COL_REC_DLMT + ky
                                    + Config.COL_REC_DLMT + item.Amount
                                    + Config.COL_REC_DLMT + thang + "/" + item.Year
                                    + Config.COL_REC_DLMT + item.SoHo
                                    + Config.ROW_REC_DLMT;
                            }
                            strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                            
                            retStr = retStr.Replace("{RECORD}", strTemp);
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "LAY THONG TIN HOA DON THANH CONG");
                            retStr = retStr.Replace("{DESCRIPTION}", "THANH TOAN");

                            return retStr;
                        }
                        catch (Exception ex)
                        {
                            Funcs.WriteLog("CUSTID:" + custid + "| EXCEPTION GET LIST BILL EVNMN " + ex.ToString());
                            return Config.ERR_MSG_GENERAL;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    return Config.ERR_MSG_GENERAL;
                }
            }
            #endregion

            #region "CASE OTHER"
            else
            {

                return Config.ERR_MSG_GENERAL;
            }
            #endregion "CASE OTHER"
        }

        public string BILL_PAYMENT(Hashtable hashTbl, string ip, string user_agent)
        {
            string des_acct = "";
            string retStr = Config.BILL_PAY;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");

            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME");

            //gan lai des name
            des_name = (des_name == Config.NULL_VALUE ? "" : des_name);


            string type = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");

            string order_id = Funcs.getValFromHashtbl(hashTbl, "BILL_ID");
            string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");
            if (txdesc == null || txdesc == "") txdesc = "THANH TOAN HOA DON " + order_id; //Funcs.getValFromHashtbl(hashTbl, "TXDESC"); khong truyen len                

            //string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC"); //theo mo ta la van truyen gia tri nay len

            string partner_id = Funcs.getValFromHashtbl(hashTbl, "PARTNER_ID");
            string category_code = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_CODE");

            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");
            string service_code = Funcs.getValFromHashtbl(hashTbl, "SERVICE_CODE");

            string bill_info_ext1 = Funcs.getValFromHashtbl(hashTbl, "BILL_INFO_EXT1");

            string bill_info_ext2 = Funcs.getValFromHashtbl(hashTbl, "BILL_INFO_EXT2");

            string mobileNo = Funcs.getValFromHashtbl(hashTbl, "MOBILE");

            string txtDescNew = txdesc + " " + order_id;

            //string tran_type = Config.TRAN_TYPE_BILL_OTHER;
            //if (type == "MOBILE")
            //{
            //    tran_type = Config.TRAN_TYPE_BILL_MOBILE;
            //}
            //else
            //{
            //    tran_type = Config.TRAN_TYPE_BILL_OTHER;
            //}

            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            //nếu category_code = 'MOBILE' --> goi ham lay service code
            // Cho tin hieu cua Cong TT
            if (category_id == "MOBILE")
            {
                tran_type = Config.TRAN_TYPE_BILL_MOBILE;
                Payments pms = new Payments();
                DataTable dt = new DataTable();

                dt = pms.getServicePartner_byBill(order_id, tran_type);

                if (dt != null && dt.Rows.Count == 1)
                {
                    service_id = dt.Rows[0][PAY_SER_PART.SERVICE_ID].ToString();
                    service_code = dt.Rows[0][PAY_SER_PART.SERVICE_CODE].ToString();
                    category_code = dt.Rows[0][PAY_SER_PART.CATEGORY_CODE].ToString();
                    partner_id = dt.Rows[0][PAY_SER_PART.PARTNER_ID].ToString();
                }

            }

            string gl_fee = "";
            string gl_vat = "";

            string check_before_trans = "";

            double amount = 0;

            try
            {
                amount = double.Parse(Funcs.NoHack(Funcs.getValFromHashtbl(hashTbl, "AMOUNT")));
            }
            catch (Exception ex)
            {
                return Config.ERR_MSG_GENERAL;
            }

            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

            if (partner_id.Equals(Config.PARTNER_ID_SHBFC) || service_id.Equals("BILL_SHBFC"))
            {
                if (amount < Double.Parse(Funcs.getConfigVal("MIN_TRAN_SHBFC")))
                {
                    Funcs.WriteLog("custid:" + custid + "AMOUNT ERROR: < " + Double.Parse(Funcs.getConfigVal("MIN_TRAN_SHBFC")));
                    return Config.ERR_MSG_FORMAT.Replace("{0}",Config.RET_CODE_SHBFC_BILLING[18].Split('|')[0]).Replace("{1}", Config.RET_CODE_SHBFC_BILLING[18].Split('|')[1] + "$" + Config.RET_CODE_SHBFC_BILLING[18].Split('|')[2]);
                }
                
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

            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            string pos_cd = "";
            pos_cd = Config.HO_BR_CODE;

            string core_txno_ref = "";
            string retPartnerWS = "";
            string core_txdate_ref = "";
            string channel_id = Config.ChannelID;

            double eb_tran_id = 0;

            //1. Tùy vào partner, type để lấy thông tin tài khoản đích: 


            if (partner_id == Config.PARTNER_ID_VNPAY)
            {
                des_acct = Config.ACCT_SUSPEND_VNPAY_BILL;
            }
            else if (partner_id == Config.PARTNER_ID_VAS)
            {
                /*

                    public static String SMLBILL_GL_SUSPEND = "9230387044";
                    public static String SMLBILL_GL_FEE = "9230387044";
                    public static String SMLBILL_GL_VAT = "9230387044";

                 */
                des_acct = Config.ACCT_SUSPEND_VAS;
                //gl_fee = Config.SMLBILL_GL_FEE;
                //gl_vat = Config.SMLBILL_GL_VAT;
            }
            else if (partner_id == Config.PARTNER_ID_ONEPAY)
            {
                des_acct = Config.ACCT_SUSPEND_ONEPAY;
            }

            //else if (partner_id == Config.PARTNER_ID_NLUONG)
            // {
            //     des_acct = Config.ACCT_SUSPEND_NLUONG;


            // }
            else if (partner_id == Config.PARTNER_ID_PAYOO)
            {
                des_acct = Config.ACCT_SUSPEND_PAYOO;

            }
            else if (partner_id == Config.PARTNER_ID_EVNHN)
            {
                DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(order_id);
                if (dt.Rows.Count > 0)
                {
                    partner_id = dt.Rows[0]["PARTNER"].ToString();
                    category_code = dt.Rows[0]["CATEGORY_CODE"].ToString();
                    service_code = dt.Rows[0]["SERVICE_CODE"].ToString();
                    des_acct = dt.Rows[0]["ACCOUNT_GL_SUSPEND"].ToString();
                }
            }
            else if (partner_id == Config.PARTNER_ID_EVNNPC)
            {
                DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(order_id);
                if (dt.Rows.Count > 0)
                {
                    partner_id = dt.Rows[0]["PARTNER"].ToString();
                    category_code = dt.Rows[0]["CATEGORY_CODE"].ToString();
                    service_code = dt.Rows[0]["SERVICE_CODE"].ToString();
                    des_acct = dt.Rows[0]["ACCOUNT_GL_SUSPEND"].ToString();
                }
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO EVNPC: billId: " + order_id);
                    return retStr = Config.ERR_MSG_GENERAL;
                }
            }
            else if (partner_id == Config.PARTNER_ID_EVNMN)
            {
                DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(order_id);
                if (dt.Rows.Count > 0)
                {
                    partner_id = dt.Rows[0]["PARTNER"].ToString();
                    category_code = dt.Rows[0]["CATEGORY_CODE"].ToString();
                    service_code = dt.Rows[0]["SERVICE_CODE"].ToString();
                    des_acct = dt.Rows[0]["ACCOUNT_GL_SUSPEND"].ToString();
                }
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO EVNPC: billId: " + order_id);
                    return retStr = Config.ERR_MSG_GENERAL;
                }

                // check thu tu thanh toan
                Funcs.WriteLog("CIF: " + custid + "|BILL:" + order_id + "| check thu tu bill");

                EVNMN.GetCustomerInfoResType listBill = EVNMNIntegration.GetCustomerInfo(Funcs.getConfigVal("BANK_ID"), order_id);


                if (listBill != null && listBill.ListofBillInfo != null && listBill.ListofBillInfo.Length > 0)
                {

                    string[] billArr = Array.FindAll(bill_info_ext1.Split('$'), s => s != "");
                    long[] amountArr = Array.ConvertAll(Array.FindAll(bill_info_ext2.Split('$'), s => s != ""), s => long.Parse(s));

                    long amountArrSum = 0;
                    Array.ForEach(amountArr, delegate (long i) { amountArrSum += i; });

                    if (amountArrSum != amount)
                    {
                        Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + order_id + " SO TIEN KHONG CHINH XAC");

                        var errorArr = Config.RET_CODE_EVNMN[2].Split('|');
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, errorArr[0]);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, errorArr[1]);
                        return retStr;
                    }


                    for (int i = 0; i < billArr.Length; i++)
                    {
                        var bill = listBill.ListofBillInfo[i];

                        if (!(billArr[i].Equals(bill.BillCode)))
                        {
                            Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + custid + " THU TU BILL KO CHINH XAC");

                            var errorArr = Config.RET_CODE_EVNMN[3].Split('|');
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, errorArr[0]);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, errorArr[1]);
                            return retStr;
                        }
                    }
                }
            }

            else if (partner_id == Config.PARTNER_ID_DAIICHI)
            {

                string[] Billling_list = bill_info_ext2.Split('^');
                des_acct = Funcs.getConfigVal("ACCT_SUSPEND_DAIICHI");

                if (Billling_list.Length > 0)
                {
                    List<PolicyDetailDLVN> billingList = DLVNSOA_.getBillingList(Billling_list);

                    txtDescNew = "MB." + order_id + "." + Funcs.RemoveVietnameseCharacterNoUpper(billingList[0].POLICY_OWNERNAME) + "." + mobileNo;
                }
                else
                {
                    txtDescNew = "MB." + order_id + "." + mobileNo;
                }

            }
            else if (partner_id == Config.PARTNER_ID_HABECO)
            {
                des_acct = Funcs.getConfigVal("ACCT_SUSPEND_HABECO");
            }
            else if (partner_id == Config.PARTNER_ID_SHBFC)
            {
                des_acct = Funcs.getConfigVal("ACCT_SUSPEND_SHBFC");
                txtDescNew = txdesc;
            }
            else if (partner_id == Config.PARTNER_ID_QUAWACO)
            {
                PayPartnerDAO da = new PayPartnerDAO();
                DataTable dt = new DataTable();
                dt = da.GetPartnerInfo(Config.PARTNER_ID_QUAWACO, order_id);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Funcs.WriteLog("custid:" + custid + "|GET ACCT_NO QUAWACO: billId: " + order_id);
                    des_acct = dt.Rows[0]["ACCOUNT_NO"].ToString();
                    Funcs.WriteLog("custid:" + custid + "|DONE GET ACCT_NO QUAWACO: billId: " + des_acct);
                }
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO QUAWACO: billId: " + order_id);
                    return retStr = Config.ERR_MSG_GENERAL;
                }
            }
            else if (partner_id == Config.PARTNER_ID_SOWACO)
            {
                txtDescNew = txdesc;

                try
                {
                    SOWACO.GetBillInfoRespType listBill = SOWACOIntegration.GetBillInfoByKHID(custid, order_id);

                    if (listBill != null && listBill.errorCode.Equals("1") && listBill.listBillInfos.Length > 0)
                    {
                         Funcs.WriteLog("custid:" + custid + "|GET ACCT_NO SOWACO: billId: " + order_id);
                         des_acct = listBill.listBillInfos[0].creditAccount.ToString();
                    }
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO SOWACO: billId: " + order_id + "|LOI LAY THONG TIN BILL INFO");
                        return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO SOWACO: billId: " + order_id + "|TRY CATCH GET BILL INFO");
                    return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                }
            }
            else if (partner_id == Config.PARTNER_ID_DAWACO)
            {
                txtDescNew = txdesc;

                try
                {
                    DAWACOBillPayments.DebtCheckRespType listBill = DAWACOIntegration.GetBillInfoByKHID(custid, order_id);

                    if (listBill != null && listBill.errorCode.Equals("00") && listBill.customerData.Length > 0)
                    {
                        Funcs.WriteLog("custid:" + custid + "|GET ACCT_NO DAWACO: billId: " + order_id);
                        des_acct = listBill.customerData[0].CREDIT_ACCOUNT.ToString();
                    }
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO DAWACO: billId: " + order_id + "|LOI LAY THONG TIN BILL INFO");
                        return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO DAWACO: billId: " + order_id + "|TRY CATCH GET BILL INFO");
                    return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                }
            }
            else
            {
                Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO : billId: " + order_id);
                return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
            }

            //linhtn fix 2017 02 07
            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }

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
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();
                Funcs.WriteLog("custid:" + custid + "|BILL DES_ACCT:" + order_id
             + "BEGIN INSERT EB TRAN");
                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "PAYMENT" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txtDescNew //txdesc
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
                    , order_id //order_id
                    , partner_id //partner code
                    , category_code //category code
                    , service_code //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huong
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , category_id //bm1 --> category_id
                    , service_id //bm2 --> service_id
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

                    Funcs.WriteLog("custid:" + custid + "BILL INSERT EB TRAN DONE tran_id:" + eb_tran_id.ToString());
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    //retStr = Config.FUNDTRANSFER_INTRA;

                    //HẠCH TOÁN VÀO CORE BANKING, TÙY THEO TRAN TYPE, ĐỐI TÁC MÀ HẠCH TOÁN KHÁC NHAU
                    Payments pm = new Payments();

                    Funcs.WriteLog("custid:" + custid + "|BILL DES_ACCT:" + order_id
                  + "BEGIN POST TO CORE");



                    //string result = tf.postFINPOSTToCore
                    string result = CoreIntegration.postFINPOSTToCore
                    (custid
                    , tran_type
                    , eb_tran_id
                    , src_acct
                    , des_acct //gl suspend
                    , "" //gl fee
                    , ""// gl vat
                    , amount // suspend amount
                    , 0 // fee amount
                    , 0 //vat amount
                    , txtDescNew
                    , pos_cd
                    , ref core_txno_ref
                    , ref core_txdate_ref
                    );

                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {

                        Funcs.WriteLog("custid:" + custid + "BILL POST TO CORE DONE tran_id:" + eb_tran_id.ToString());

                        string info_ref = "";
                        retPartnerWS = pm.postBillToPartner(
                            eb_tran_id
                            , custid
                            , src_acct
                            , amount
                            , order_id
                            , partner_id
                            , category_code
                            , service_code
                            , txdesc
                            , bill_info_ext1
                            , bill_info_ext2
                            , ref info_ref
                             , core_txno_ref
                            );

                        // neu bill  thanh cong
                        if (retPartnerWS == Config.ERR_CODE_DONE)
                        {
                            //xu ly tiep save to ben list o duoi

                        }
                        if (retPartnerWS == Config.ERR_CODE_TIMEOUT)
                        {
                            //TIMEOUT CUNG TINH LA STATUS EBANK TRAN = 1. + HAN MUC GIAO DICH
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                            return Config.ERR_MSG_TIMEOUT;

                        }
                        // endif neu topup thanh cong
                        if (retPartnerWS == Config.ERR_CODE_REVERT)// neu tra ve loi cu the thi revert                                    
                        {
                            //string revStr = tf.revTransderTx(eb_tran_id, txdesc);

                            string revStr = CoreIntegration.revFinPost(eb_tran_id, txdesc, core_txno_ref);
                            //string revStr = tf.revFinPost(eb_tran_id, txdesc);

                            if (revStr == Config.gResult_INTELLECT_Arr[0])
                            {
                                Funcs.WriteLog("custid:" + custid + "BILL revert successfull tran_id:" + eb_tran_id);
                            }

                            else
                            {
                                Funcs.WriteLog("custid:" + custid + "BILL revert not succesfull tran_id:" + eb_tran_id);
                            }
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);


                            //retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                            //retStr = retStr.Replace(Config.ERR_DESC_VAL, "");

                            if (partner_id.Equals(Config.PARTNER_ID_EVNMN) && info_ref != null && info_ref.Contains("02|"))
                            {
                                var errorArr = info_ref.Split('|');

                                retStr = retStr.Replace(Config.ERR_CODE_VAL, errorArr[0]);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, errorArr[1]);
                                return retStr;
                            }

                            return Config.ERR_MSG_GENERAL;

                        }
                        if (retPartnerWS == Config.ERR_CODE_GENERAL)//                          
                        {
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                            return Config.ERR_MSG_GENERAL;
                        }

                        // CAP NHAT TRANG THAI VAO BANG TBL_EB_TRAN
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                        //SAVE TO BEN LIST
                        //khong can save to ben list
                        if (save_to_benlist == "1")
                        {

                            // Goi ham save to BEN   
                            Funcs.WriteLog("custid:" + custid + "BILL SAVE TO BENLIST BEGIN:" + eb_tran_id.ToString()
                                + "ORDER_ID" + order_id);


                            Beneficiarys ben = new Beneficiarys();
                            DataTable dt = new DataTable();
                            dt = ben.INSERT_BEN(
                                custid
                                , tran_type
                                , order_id//des_acct   ---> save số dt thay cho acct
                                , des_name
                                , "" //des_name --linhtn fix 2017 01 17: form thanh toan hoa don khong can ten goi nho
                                , txdesc
                                , "" //bank_code
                                , "" //bank_name
                                , "" //bank_branch
                                , "" //bank_city
                                , category_id //category_id
                                , service_id //service_id
                                , "" //lastchange default = sysdate da xu ly o db
                                , ""// bm1
                                , ""// bm2
                                , ""// bm3
                                , ""// bm4
                                , ""// bm5
                                , ""// bm6
                                , ""// bm7
                                , Config.ChannelID// ""// bm8
                                , ip// ""// bm9
                                , user_agent // ""// bm10
                                );

                            if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "BILL IS COMPLETED TRAN_ID=" + eb_tran_id);

                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                                Funcs.WriteLog("custid:" + custid + "BILL SAVE TO BENLIST DONE:" + eb_tran_id.ToString());

                            }
                            else
                            {
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "BILL IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);

                                Funcs.WriteLog("custid:" + custid + "BILL SAVE TO BENLIST FALIED:" + eb_tran_id.ToString());
                            }
                            //giai phong bo nho                            
                            ben = null;
                            dt = null;

                        } // het else if else if (save_to_benlist == "1")
                        else
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "BILL IS COMPLETED TRAN_ID= " + eb_tran_id);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                        }
                        //can save to ben list

                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "BILL POST TO CORE FAILED tran_id:" + eb_tran_id.ToString());
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                        retStr = Config.CD_EB_TRANS_ERR_GENERAL;
                    }
                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                }
            }
            else
            {
                //retStr = Config.ERR_MSG_GENERAL;
                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "BILL CHECK PWD FAILED");
            }

            return retStr;
            //hạch toán vào core tùy theo partner

            //gọi sang đối tác để nạp tiền

        }

        #endregion "Billing"
        public string PaymentByCreditCard(Hashtable hashTbl, string ip, string user_agent)
        {
            string des_acct = "";
            string retStr = Config.BILL_PAY;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");

            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME");

            des_name = (des_name == Config.NULL_VALUE ? "" : des_name);

            string type = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");

            string order_id = Funcs.getValFromHashtbl(hashTbl, "BILL_ID");
            string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");
            if (txdesc == null || txdesc == "") txdesc = "THANH TOAN HOA DON " + order_id;

            string partner_id = Funcs.getValFromHashtbl(hashTbl, "PARTNER_ID");

            string category_code = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_CODE");

            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");
            string service_code = Funcs.getValFromHashtbl(hashTbl, "SERVICE_CODE");

            string bill_info_ext1 = Funcs.getValFromHashtbl(hashTbl, "BILL_INFO_EXT1");

            string bill_info_ext2 = Funcs.getValFromHashtbl(hashTbl, "BILL_INFO_EXT2");

            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            string mobileNo = Funcs.getValFromHashtbl(hashTbl, "MOBILE");

            string txtDescNew = txdesc + " " + order_id;

            if (category_id == "MOBILE")
            {
                tran_type = Config.TRAN_TYPE_BILL_MOBILE;
                Payments pms = new Payments();
                DataTable dt = new DataTable();

                dt = pms.getServicePartner_byBill(order_id, tran_type);

                if (dt != null && dt.Rows.Count == 1)
                {
                    service_id = dt.Rows[0][PAY_SER_PART.SERVICE_ID].ToString();
                    service_code = dt.Rows[0][PAY_SER_PART.SERVICE_CODE].ToString();
                    category_code = dt.Rows[0][PAY_SER_PART.CATEGORY_CODE].ToString();
                    partner_id = dt.Rows[0][PAY_SER_PART.PARTNER_ID].ToString();
                }

            }

            string gl_fee = "";
            string gl_vat = "";
            string check_before_trans = "";

            double amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "BILL_AMOUNT"));

            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

            if (partner_id.Equals(Config.PARTNER_ID_SHBFC) || service_id.Equals("BILL_SHBFC"))
            {
                if (amount < Double.Parse(Funcs.getConfigVal("MIN_TRAN_SHBFC")))
                {
                    Funcs.WriteLog("custid:" + custid + "AMOUNT ERROR: < " + Double.Parse(Funcs.getConfigVal("MIN_TRAN_SHBFC")));
                    return Config.ERR_MSG_FORMAT.Replace("{0}", Config.RET_CODE_SHBFC_BILLING[18].Split('|')[0]).Replace("{1}", Config.RET_CODE_SHBFC_BILLING[18].Split('|')[1] + "$" + Config.RET_CODE_SHBFC_BILLING[18].Split('|')[2]);
                }

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

            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            string pos_cd = "";

            pos_cd = Config.HO_BR_CODE;

            string core_txno_ref = "";
            string retPartnerWS = "";
            string core_txdate_ref = "";
            string channel_id = Config.ChannelID;

            string ref_no_to_svfe = "";
            string set_date_to_svfe = "";

            double eb_tran_id = 0;

            if (partner_id == Config.PARTNER_ID_VNPAY)
            {
                des_acct = Config.ACCT_SUSPEND_VNPAY_BILL;
            }
            else if (partner_id == Config.PARTNER_ID_VAS)
            {
                des_acct = Config.ACCT_SUSPEND_VAS;
            }
            else if (partner_id == Config.PARTNER_ID_ONEPAY)
            {
                des_acct = Config.ACCT_SUSPEND_ONEPAY;
            }
            else if (partner_id == Config.PARTNER_ID_PAYOO)
            {
                des_acct = Config.ACCT_SUSPEND_PAYOO;

            }
            else if (partner_id == Config.PARTNER_ID_EVNHN)
            {
                DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(order_id);
                if (dt.Rows.Count > 0)
                {
                    partner_id = dt.Rows[0]["PARTNER"].ToString();
                    category_code = dt.Rows[0]["CATEGORY_CODE"].ToString();
                    service_code = dt.Rows[0]["SERVICE_CODE"].ToString();
                    des_acct = dt.Rows[0]["ACCOUNT_GL_SUSPEND"].ToString();
                }
            }
            else if (partner_id == Config.PARTNER_ID_EVNNPC)
            {
                DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(order_id);
                if (dt.Rows.Count > 0)
                {
                    partner_id = dt.Rows[0]["PARTNER"].ToString();
                    category_code = dt.Rows[0]["CATEGORY_CODE"].ToString();
                    service_code = dt.Rows[0]["SERVICE_CODE"].ToString();
                    des_acct = dt.Rows[0]["ACCOUNT_GL_SUSPEND"].ToString();
                }
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO EVNPC: billId: " + order_id);
                    return retStr = Config.ERR_MSG_GENERAL;
                }
            }
            else if (partner_id == Config.PARTNER_ID_EVNMN)
            {
                DataTable dt = new EVNHNDAO().GET_PARTNER_ELECTRICITY(order_id);
                if (dt.Rows.Count > 0)
                {
                    partner_id = dt.Rows[0]["PARTNER"].ToString();
                    category_code = dt.Rows[0]["CATEGORY_CODE"].ToString();
                    service_code = dt.Rows[0]["SERVICE_CODE"].ToString();
                    des_acct = dt.Rows[0]["ACCOUNT_GL_SUSPEND"].ToString();
                }
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO EVNPC: billId: " + order_id);
                    return retStr = Config.ERR_MSG_GENERAL;
                }

                // check thu tu thanh toan
                Funcs.WriteLog("CIF: " + custid + "|BILL:" + order_id + "| check thu tu bill");

                EVNMN.GetCustomerInfoResType listBill = EVNMNIntegration.GetCustomerInfo(Funcs.getConfigVal("BANK_ID"), order_id);


                if (listBill != null && listBill.ListofBillInfo != null && listBill.ListofBillInfo.Length > 0)
                {

                    string[] billArr = Array.FindAll(bill_info_ext1.Split('$'), s => s != "");
                    long[] amountArr = Array.ConvertAll(Array.FindAll(bill_info_ext2.Split('$'), s => s != ""), s => long.Parse(s));

                    long amountArrSum = 0;
                    Array.ForEach(amountArr, delegate (long i) { amountArrSum += i; });

                    if (amountArrSum != amount)
                    {
                        Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + order_id + " SO TIEN KHONG CHINH XAC");

                        var errorArr = Config.RET_CODE_EVNMN[2].Split('|');
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, errorArr[0]);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, errorArr[1]);
                        return retStr;
                    }


                    for (int i = 0; i < billArr.Length; i++)
                    {
                        var bill = listBill.ListofBillInfo[i];

                        if (!(billArr[i].Equals(bill.BillCode)))
                        {
                            Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + custid + " THU TU BILL KO CHINH XAC");

                            var errorArr = Config.RET_CODE_EVNMN[3].Split('|');
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, errorArr[0]);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, errorArr[1]);
                            return retStr;
                        }
                    }
                }    
            }
            else if (partner_id == Config.PARTNER_ID_DAIICHI)
            {
                string[] Billling_list = bill_info_ext2.Split('^');
                des_acct = Funcs.getConfigVal("ACCT_SUSPEND_DAIICHI");

                if (Billling_list.Length > 0)
                {
                    List<PolicyDetailDLVN> billingList = DLVNSOA_.getBillingList(Billling_list);

                    txtDescNew = "MB." + order_id + "." + Funcs.RemoveVietnameseCharacterNoUpper(billingList[0].POLICY_OWNERNAME) + "." + mobileNo;
                }
                else
                {
                    txtDescNew = "MB." + order_id  + "." + mobileNo;
                }
                    
                
            }
            else if (partner_id == Config.PARTNER_ID_HABECO)
            {
                des_acct = Funcs.getConfigVal("ACCT_SUSPEND_HABECO");
            }
            else if (partner_id == Config.PARTNER_ID_SHBFC)
            {
                des_acct = Funcs.getConfigVal("ACCT_SUSPEND_SHBFC");
                txtDescNew = txdesc;
            }
            else if (partner_id == Config.PARTNER_ID_QUAWACO)
            {
                PayPartnerDAO da = new PayPartnerDAO();
                DataTable dt = new DataTable();
                dt = da.GetPartnerInfo(Config.PARTNER_ID_QUAWACO, order_id);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Funcs.WriteLog("custid:" + custid + "|GET ACCT_NO QUAWACO: billId: " + order_id);
                    des_acct = dt.Rows[0]["ACCOUNT_NO"].ToString();
                    Funcs.WriteLog("custid:" + custid + "|DONE GET ACCT_NO QUAWACO: billId: " + des_acct);
                }
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO QUAWACO: billId: " + order_id);
                    return retStr = Config.ERR_MSG_GENERAL;
                }
            }
            else if (partner_id == Config.PARTNER_ID_SOWACO)
            {
                txtDescNew = txdesc;

                try
                {
                    SOWACO.GetBillInfoRespType listBill = SOWACOIntegration.GetBillInfoByKHID(custid, order_id);

                    if (listBill != null && listBill.errorCode.Equals("1") && listBill.listBillInfos.Length > 0)
                    {
                        Funcs.WriteLog("custid:" + custid + "|GET ACCT_NO SOWACO: billId: " + order_id);
                        des_acct = listBill.listBillInfos[0].creditAccount.ToString();
                    }
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO SOWACO: billId: " + order_id + "|LOI LAY THONG TIN BILL INFO");
                        return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO SOWACO: billId: " + order_id + "|TRY CATCH GET BILL INFO");
                    return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                }
            }
            else if (partner_id == Config.PARTNER_ID_DAWACO)
            {
                txtDescNew = txdesc;

                try
                {
                    DAWACOBillPayments.DebtCheckRespType listBill = DAWACOIntegration.GetBillInfoByKHID(custid, order_id);

                    if (listBill != null && listBill.errorCode.Equals("00") && listBill.customerData.Length > 0)
                    {
                        Funcs.WriteLog("custid:" + custid + "|GET ACCT_NO DAWACO: billId: " + order_id);
                        des_acct = listBill.customerData[0].CREDIT_ACCOUNT.ToString();
                    }
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO DAWACO: billId: " + order_id + "|LOI LAY THONG TIN BILL INFO");
                        return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO DAWACO: billId: " + order_id + "|TRY CATCH GET BILL INFO");
                    return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);
                }
            }
            else
            {
                Funcs.WriteLog("custid:" + custid + "|ERROR GET ACCT_NO : billId: " + order_id);
                return retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", "KHONG LAY DUOC THONG TIN TAI KHOAN THU HO").Replace("{2}", custid);

            }

            //acct la so the da duoc ma hoa
            if (src_acct.Length > 10)
            {
                //bool check = Auth.CustIdMatchScrAcct(custid, src_acct);

                //if (!check)
                //{
                //    return Config.ERR_MSG_GENERAL;
                //}

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
                if (check_before_trans == Config.ERR_CODE_DONE)
                {
                    Transfers tf = new Transfers();
                    DataTable eb_tran = new DataTable();
                    Funcs.WriteLog("custid:" + custid + "|BILL DES_ACCT:" + order_id + "BEGIN INSERT EB TRAN");
                    //  eb_tran = transfer.insTransferTx
                    #region "insert TBL_EB_TRAN"
                    eb_tran = tf.insTransferTx(
                        Config.ChannelID
                        , "PAYMENT" //mod_cd
                        , tran_type //tran_type
                        , custid //custid
                        , src_acct//src_acct
                        , des_acct //des_acct
                        , amount //amount
                        , "VND" //ccy_cd
                        , 1//convert rate
                        , amount //lcy_amount
                        , txtDescNew //txdesc
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
                        , order_id //order_id
                        , partner_id //partner code
                        , category_code //category code
                        , service_code //service code
                        , "" //merchant code
                        , ""//suspend account
                        , ""//fee account
                        , ""//vat account
                        , 0 //suppend amount
                        , 0 //fee amount
                        , 0 //vat amount
                        , "" // des name ten tai khoan thu huong
                        , "" // bank code
                        , "" // ten ngan hang
                        , "" // ten thanh pho
                        , "" // ten chi nhanh
                        , category_id //bm1 --> category_id
                        , service_id //bm2 --> service_id
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

                    Funcs.WriteLog("insert TBL_EB_TRAN");
                    #endregion "insert TBL_EB_TRAN"
                    //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                    if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                    {

                        Funcs.WriteLog("custid:" + custid + "BILL INSERT EB TRAN DONE tran_id:" + eb_tran_id.ToString());
                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                        //retStr = Config.FUNDTRANSFER_INTRA;

                        //call Purchase Service
                        string tmpAmt = amount.ToString() + "00";
                        tmpAmt = tmpAmt.ToString().PadLeft(12, '0');
                        var ref_to_card_api = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

                        CardAPI.PurchaseResType res = CardIntegration.purcharseCardAPI(
                            custid,
                            src_acct,
                            ref_to_card_api,
                            tmpAmt,
                            Funcs.getConfigVal("BILLING_TOPUP_MCC"),
                            Funcs.getConfigVal("BILLING_TOPUP_TID"),
                            Config.CardAPIConfig.ServiceCd,
                            Funcs.getConfigVal("BILLING_TOPUP_MID")
                        );
                        //end call Purchase Service

                        if (res != null && res.RespSts.Sts.Equals("0"))
                        {
                            //HẠCH TOÁN VÀO CORE BANKING, TÙY THEO TRAN TYPE, ĐỐI TÁC MÀ HẠCH TOÁN KHÁC NHAU
                            Payments pm = new Payments();

                            Funcs.WriteLog("custid:" + custid + "|BILL DES_ACCT:" + order_id + "BEGIN POST TO CORE");

                            ref_no_to_svfe = res.RefNo;
                            set_date_to_svfe = res.SettDate;

                            string src_acc_credit_gl = Funcs.GetAccGLCredit(src_acct);

                            if (string.IsNullOrEmpty(src_acc_credit_gl))
                            {
                                return Config.ERR_MSG_GENERAL;
                            }

                            //string result = tf.postFINPOSTToCore
                            string result = CoreIntegration.postFINPOSTToCore
                                                (custid
                                                , tran_type
                                                , eb_tran_id
                                                , src_acc_credit_gl
                                                , des_acct //gl suspend
                                                , "" //gl fee
                                                , ""// gl vat
                                                , amount // suspend amount
                                                , 0 // fee amount
                                                , 0 //vat amount
                                                , txtDescNew
                                                , pos_cd
                                                , ref core_txno_ref
                                                , ref core_txdate_ref
                                                );

                            //Neu src_acct la Credit 
                            //Goi API PurchareAPI sau do goi FinPostCore
                            if (result == Config.gResult_INTELLECT_Arr[0])
                            {
                                //amount gui sang Card them 02 so 00 o cuoi truoc khi padleft
                                tmpAmt = amount.ToString() + "00";
                                tmpAmt = tmpAmt.ToString().PadLeft(12, '0');

                                Funcs.WriteLog("custid:" + custid + "BILL POST TO CORE DONE tran_id:" + eb_tran_id.ToString());

                                string info_refCard = "";

                                retPartnerWS = pm.postBillToPartner(
                                        eb_tran_id
                                        , custid
                                        , ConfigurationManager.AppSettings.Get("ACCT_GL_CARD")
                                        , amount
                                        , order_id
                                        , partner_id
                                        , category_code
                                        , service_code
                                        , txdesc
                                        , bill_info_ext1
                                        , bill_info_ext2
                                        , ref info_refCard
                                        , core_txno_ref
                                        );

                                // neu bill  thanh cong
                                if (retPartnerWS == Config.ERR_CODE_DONE)
                                {
                                    //xu ly tiep save to ben list o duoi

                                }
                                if (retPartnerWS == Config.ERR_CODE_TIMEOUT)
                                {
                                    //TIMEOUT CUNG TINH LA STATUS EBANK TRAN = 1. + HAN MUC GIAO DICH
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                                    return Config.ERR_MSG_TIMEOUT;

                                }
                                // endif neu topup thanh cong
                                if (retPartnerWS == Config.ERR_CODE_REVERT)// neu tra ve loi cu the thi revert                                    
                                {
                                    //string revStr = tf.revTransderTx(eb_tran_id, txdesc);


                                    //string revStr = tf.revFinPost(eb_tran_id, txdesc);
                                    string revStr = CoreIntegration.revFinPost(eb_tran_id, txdesc);



                                    if (revStr == Config.gResult_INTELLECT_Arr[0])
                                    {
                                        Funcs.WriteLog("custid:" + custid + "BILL revert successfull tran_id:" + eb_tran_id);

                                        //revert svfe
                                        CardAPI.ReversalResType revertRes = CardIntegration.revertCardAPI(
                                                                        custid,
                                                                        src_acct,
                                                                        ref_to_card_api,
                                                                        tmpAmt,
                                                                        ref_no_to_svfe,
                                                                        set_date_to_svfe,
                                                                        Funcs.getConfigVal("BILLING_TOPUP_MCC"),
                                                                        Funcs.getConfigVal("BILLING_TOPUP_TID"),
                                                                        Config.CardAPIConfig.ServiceCd,
                                                                        Funcs.getConfigVal("BILLING_TOPUP_MID")
                                                                    );

                                        if (revertRes != null && revertRes.RespSts.Sts.Equals("0"))
                                        {
                                            Funcs.WriteLog("custid:" + custid + "BILL revert to SVFE successfull ref_no_to_svfe:" + ref_no_to_svfe);
                                        }
                                        else
                                        {
                                            Funcs.WriteLog("custid:" + custid + "BILL revert to SVFE not successfull ref_no_to_svfe:" + ref_no_to_svfe);
                                        }
                                        //end revert svfe
                                    }
                                    else
                                    {
                                        Funcs.WriteLog("custid:" + custid + "BILL revert not succesfull tran_id:" + eb_tran_id);
                                    }


                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);

                                    //retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_GENERAL);
                                    //retStr = retStr.Replace(Config.ERR_DESC_VAL, "");

                                    if (partner_id.Equals(Config.PARTNER_ID_EVNMN) && info_refCard != null && info_refCard.Contains("02|"))
                                    {
                                        var errorArr = info_refCard.Split('|');

                                        retStr = retStr.Replace(Config.ERR_CODE_VAL, errorArr[0]);
                                        retStr = retStr.Replace(Config.ERR_DESC_VAL, errorArr[1]);
                                        return retStr;
                                    }

                                    return Config.ERR_MSG_GENERAL;

                                }
                                if (retPartnerWS == Config.ERR_CODE_GENERAL)//                          
                                {
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                                    return Config.ERR_MSG_GENERAL;
                                }

                                // CAP NHAT TRANG THAI VAO BANG TBL_EB_TRAN
                                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                                //SAVE TO BEN LIST
                                //khong can save to ben list
                                if (save_to_benlist == "1")
                                {

                                    // Goi ham save to BEN   
                                    Funcs.WriteLog("custid:" + custid + "BILL SAVE TO BENLIST BEGIN:" + eb_tran_id.ToString()
                                        + "ORDER_ID" + order_id);


                                    Beneficiarys ben = new Beneficiarys();
                                    DataTable dt = new DataTable();
                                    dt = ben.INSERT_BEN(
                                        custid
                                        , tran_type
                                        , order_id//des_acct   ---> save số dt thay cho acct
                                        , des_name
                                        , "" //des_name --linhtn fix 2017 01 17: form thanh toan hoa don khong can ten goi nho
                                        , txdesc
                                        , "" //bank_code
                                        , "" //bank_name
                                        , "" //bank_branch
                                        , "" //bank_city
                                        , category_id //category_id
                                        , service_id //service_id
                                        , "" //lastchange default = sysdate da xu ly o db
                                        , ""// bm1
                                        , ""// bm2
                                        , ""// bm3
                                        , ""// bm4
                                        , ""// bm5
                                        , ""// bm6
                                        , ""// bm7
                                        , Config.ChannelID// ""// bm8
                                        , ip// ""// bm9
                                        , user_agent // ""// bm10
                                        );

                                    if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                                    {
                                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "BILL IS COMPLETED TRAN_ID=" + eb_tran_id);

                                        retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                        retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                                        Funcs.WriteLog("custid:" + custid + "BILL SAVE TO BENLIST DONE:" + eb_tran_id.ToString());

                                    }
                                    else
                                    {
                                        retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                        retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "BILL IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);

                                        Funcs.WriteLog("custid:" + custid + "BILL SAVE TO BENLIST FALIED:" + eb_tran_id.ToString());
                                    }
                                    //giai phong bo nho                            
                                    ben = null;
                                    dt = null;

                                } // het else if else if (save_to_benlist == "1")
                                else
                                {
                                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "BILL IS COMPLETED TRAN_ID= " + eb_tran_id);
                                    retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                    retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                }

                            }
                            //else
                            //{
                            //    //revert the
                            //    CardAPI.ReversalResType revertRes = CardIntegration.revertCardAPI(
                            //                                            custid,
                            //                                            src_acct,
                            //                                            eb_tran_id.ToString(),
                            //                                            tmpAmt,
                            //                                            ref_no_to_svfe,
                            //                                            set_date_to_svfe,
                            //                                            Config.CardAPIConfig.MerchantType,
                            //                                            Config.CardAPIConfig.TermId,
                            //                                            Config.CardAPIConfig.ServiceCd
                            //                                        );

                            //    if (revertRes != null && revertRes.RespSts.Sts.Equals("0"))
                            //    {
                            //        Funcs.WriteLog("custid:" + custid + "BILL revert to SVFE successfull ref_no_to_svfe:" + ref_no_to_svfe);
                            //    }
                            //    else
                            //    {
                            //        Funcs.WriteLog("custid:" + custid + "BILL revert to SVFE not successfull ref_no_to_svfe:" + ref_no_to_svfe);
                            //    }

                            //    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                            //    Funcs.WriteLog("custid:" + custid + "BILL POST TO CORE FAILED tran_id:" + eb_tran_id.ToString());
                            //    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                            //    retStr = Config.ERR_MSG_GENERAL;
                            //}
                        }
                    }
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    //retStr = check_before_trans;
                    Funcs.WriteLog("custid:" + custid + "BILL CHECK PWD FAILED");
                }
            }

            return retStr;
        }

        public string GET_PAY_CREDIT_BY_SERVICEID(Hashtable hashTbl)
        {
            string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");

            String retStr = Config.ERR_MSG_GENERAL;

            Funcs.WriteLog("CIF: " + cifNo + "|CHECK PAY CREDIT BY SERVICE ID: REQ: "
                + "|category_id:" + category_id
                + "|tran_type:" + tran_type
                 + "|service_id:" + service_id
                );

            try
            {
                Payments da = new Payments();
                DataTable dt = new DataTable();
                dt = da.GET_PAY_CREDIT_BY_SERVICEID(category_id, tran_type, service_id);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Funcs.WriteLog("CIF: " + cifNo + "|CHECK PAY CREDIT BY SERVICE ID: RES: " + dt.Rows[0]["RET_CODE"].ToString());

                    if (dt.Rows[0]["RET_CODE"].Equals("E00"))
                        return Config.SUCCESS_MSG_GENERAL;

                    return retStr;
                }
                else
                {
                    Funcs.WriteLog("CIF: " + cifNo + "|CHECK PAY CREDIT BY SERVICE ID: RES: null");

                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF: " + cifNo + "|CHECK PAY CREDIT BY SERVICE ID: RES: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }
    }


}