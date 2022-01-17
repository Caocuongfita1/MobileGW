using mobileGW.Service.Bussiness;
using mobileGW.Service.Framework;
using mobileGW.Service.DataAccess;
using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Collections;


/// <summary>
/// Summary description for Financial_Transfer
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class Financial_Transfer
    {
        public Financial_Transfer()
        {
            //
            //
        }
        public static string FUNDTRANSFER_INTRA(string custid, string src_acct, string pwd
            , string des_acct, string des_name, string des_nick_name, 
            string tran_type, string txdesc, 
            double amount, string save_to_benlist
            , string ip
            , string user_agent
            , Hashtable hashTbl
            )
        {
            string retStr = Config.FUNDTRANSFER_INTRA;
            string check_before_trans = "";
            double eb_tran_id = 0;

            string  core_txno_ref = "";
            string core_txdate_ref = "";

            string acctno = des_acct;
            string channel_id = "";

            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            //trong truong hop auth_method = 2 phai ma hoa md5
            if (typeOtp == 2)
            {
               
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA BEGIN");

            /*
            if (des_acct.IndexOf(Config.SHB_BIN) == 0)
            {
                CardInfoData cardinfo = new CardInfoData();
                CardTx cardtx = new CardTx();
                cardinfo = cardtx.GETDEFACCTNO_CARDNO(des_acct);
                cardtx = null;
                if (cardinfo != null)
                {
                    acctno = cardinfo.Tables[CardInfoData.TBLNAME].Rows[0][CardInfoData.ACCT_NO].ToString();
                    cardinfo.Dispose();
                }
                else
                {
                    retStr = Config.ERR_MSG_INVALID_CARD_NO;
                    return retStr;
                }
            }
            des_acct = acctno;
             */

            //linhtn fix 2017 02 07
            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }

            //tungdt8 fix 2020 07 22
            //KIEM TRA HAN MUC TAI KHOAN THAU CHI
            Utils util = new Utils();
            bool checkLimitThauChi = util.checkLimitThauChi(custid, Config.FUNDTRANSFER_INTRA, src_acct, acctno, amount);
            if (!checkLimitThauChi)
            {
                return Config.ERR_MSG_FORMAT.Replace("{0}",Config.ERR_CODE_LIMIT_THAU_CHI).Replace("{1}",Funcs.getConfigVal("LIMIT_THAU_CHI_DES")).Replace("{2}",custid);
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

                //  eb_tran = transfer.insTransferTx

                Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA BEGIN INSERT EB TRAN");

                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "TRANSFER" //mod_cd
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
                    , user_agent// ""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {

                    eb_tran_id = double.Parse( eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA BEGIN POST TO CORE");

                    retStr = Config.FUNDTRANSFER_INTRA;
                    //string result = tf.pstTransderTx(
                    string result = CoreIntegration.pstTransderTx(
                                  custid
                                  , eb_tran_id
                                  , src_acct
                                  , des_acct
                                  , amount
                                  , 0
                                  , txdesc
                                  , ref core_txno_ref
                                  , ref core_txdate_ref);
                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {
                        tf.uptTransferTx(eb_tran_id
                            , Config.TX_STATUS_DONE
                            , core_txno_ref
                            , core_txdate_ref
                            , Config.ChannelID);


                        //SAVE TO BEN LIST
                        if (save_to_benlist == "0")
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "FUNDTRANSFER INTRA IS COMPLETED TRAN_ID=" + eb_tran_id);
                            retStr = retStr.Replace( "{TRANID}", core_txno_ref);
                            retStr = retStr.Replace( "{TRAN_DATE}", core_txdate_ref); // case sensitive);

                        }
                        else if (save_to_benlist == "1")
                        {

                            Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA BEGIN SAVE TO BENLIST");

                            Beneficiarys ben = new Beneficiarys();
                            DataTable dt = new DataTable();
                            dt = ben.INSERT_BEN(
                                custid
                                , tran_type
                                , des_acct
                                , des_name
                                , des_nick_name //"XXXXXXXXXX"//des_nick_name
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
                                , ""// bm8
                                , ""// bm9
                                , ""// bm10
                                );

                            if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "INTRA TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id  + " SAVE TO BENLIST DONE");

                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                                Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA  SAVE TO BENLIST DONE");

                                //linhtn add new 20/11/2016
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
                                           , custid
                                           , ip
                                           , user_agent
                                           , Config.EB_ACTION_DONE
                                           , "THEM MOI CAP NHAT DANH SACH THU HUONG THANH CONG"
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
                                catch  (Exception ex)
                                {
                                    Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA  SAVE BENLIST ACTION TO TBL_EB_ACTION FAILED EX:" + ex.ToString());

                                }
                               
                                #endregion "INSERT_EB_ACTION"

                            }
                            else
                            {
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "INTRA TRANSFER IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);
                                Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA  SAVE TO BENLIST FAILED");

                            }
                            //giai phong bo nho                            
                            ben = null;
                            dt = null;
                        }
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {

                        tf.uptTransferTx(eb_tran_id
                         , Config.TX_STATUS_FAIL
                         , core_txno_ref
                         , core_txdate_ref
                         , channel_id);
                        //retStr = Config.CD_EB_TRANS_ERR_GENERAL;
                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA POST TO CORE FAILED");

                    }
                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    //retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    //retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA CAN NOT INSERT EB TRAN");

                }
            }
            else//AUTH FAILED
            {
                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA AUTH FAILED END");

            }
            Funcs.WriteLog("custid:" + custid + "|FUNDTRANSFER_INTRA END");
            return retStr;
        }

        //CMD#FUND_TRANSFER_SELF|CIF_NO#0310008705|SRC_ACCT#1000013376|DES_ACCT#1000010000|AMOUNT#10|TXDESC#DIEN GIAI GIAO DICH|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX
        public static string FUND_TRANSFER_SELF(Hashtable hashTbl, string ip, string user_agent)
        {
            string retStr = Config.FUNDTRANSFER_INTRA;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            string des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
                        
            string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");
            string tran_type = Config.TRAN_TYPE_SELF;
            double amount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
             double eb_tran_id = 0;
             string core_txno_ref = "";
             string core_txdate_ref = "";
            

            Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  BEGIN");

            //get Infor by CustId
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            DataSet ds = new DataSet();
            ds = da.GET_USER_BY_CIF(custid);
            DataTable dt = new DataTable();
            if (ds != null && ds.Tables[0] != null)
            {
                dt = ds.Tables[0];
            }
            else
            {
                return Config.ERR_MSG_GENERAL;
            }

            //tungdt8 fix 2020 07 22
            //KIEM TRA HAN MUC TAI KHOAN THAU CHI
            Utils util = new Utils();
            bool checkLimitThauChi = util.checkLimitThauChi(custid, Config.FUNDTRANSFER_INTRA, src_acct, des_acct, amount);
            if (!checkLimitThauChi)
            {
                return Config.ERR_MSG_FORMAT.Replace("{0}", Config.ERR_CODE_LIMIT_THAU_CHI).Replace("{1}", Funcs.getConfigVal("LIMIT_THAU_CHI_DES")).Replace("{2}", custid);
            }

            //check source acct.des acct belong to CIF ???
            if ((Auth.CustIdMatchScrAcct(custid,src_acct)) && (Auth.CustIdMatchScrAcct(custid,des_acct)))
            {
                bool isVirtualMoney = Auth.isVitualMoney(txdesc);
                Funcs.WriteLog("custid:" + custid + "|isVirtualMoney: " + isVirtualMoney);
                if (isVirtualMoney)
                {
                    string retErrVirtualMoney = string.Format("ERR_CODE#{0}|ERR_DESC_VI#{1}|ERR_DESC_EN#{2}"
                        , "90", LanguageConfig.ErrorVirtualMoneyVi, LanguageConfig.ErrorVirtualMoneyEn);
                    Funcs.WriteLog("mob_user:" + custid + "|retErrVirtualMoney: " + retErrVirtualMoney);
                    return retErrVirtualMoney;
                }
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //  eb_tran = transfer.insTransferTx

                Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  BEGIN INSERT EB TRAN");

                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "TRANSFER" //mod_cd
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
                    , Int32.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.AUTH_METHOD].ToString()) //auth_type
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
                    , "" //bm27
                    , ip //bm28 //ip
                    , user_agent//bm29  //user_agent             
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {

                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  BEGIN POST TO CORE");

                    retStr = Config.FUNDTRANSFER_INTRA;
                    //string result = tf.pstTransderTx(
                    string result = CoreIntegration.pstTransderTx(
                                  custid
                                  , eb_tran_id
                                  , src_acct
                                  , des_acct
                                  , amount
                                  , 0
                                  , txdesc
                                  , ref core_txno_ref
                                  , ref core_txdate_ref);
                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {
                        Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  END POST TO DONE");

                        Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  BEGIN UPATE EB TRAN");

                        tf.uptTransferTx(eb_tran_id
                            , Config.TX_STATUS_DONE
                            , core_txno_ref
                            , core_txdate_ref
                            , Config.ChannelID);


                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "FUND_TRANSFER_SELF IS COMPLETED TRAN_ID=" + eb_tran_id);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  POST TO CORE FAILED");

                        tf.uptTransferTx(eb_tran_id
                         , Config.TX_STATUS_FAIL
                         , core_txno_ref
                         , core_txdate_ref
                         , Config.ChannelID);
                        //retStr = Config.CD_EB_TRANS_ERR_GENERAL;

                        retStr = Config.ERR_MSG_GENERAL;


                    }
                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    //retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    //retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  COULD NOT INSERT EB TRAN");

                }
            }
            else // AUTH FAILED
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|FUND_TRANSFER_SELF  CASA NOT BELONG TO CIF");
            }
            return retStr;
        }

        /*
         REQ=CMD#DOMESTIC|CIF_NO#0310008705|SRC_ACCT#1000013376|DES_ACCT#1000010000|
         * AMOUNT#10|ACCT_NAME#NGUYEN VAN A|BANK_CODE#1234|BANK_NAME#VCB|
         * BANK_BRANCH#CN HA NOI|BANK_CITY#HANOI|CCYCD#VND|TXDESC#CKMOBILE|
         * TRANPWD#fksdfjf385738jsdfjsdf9#SAVE_TO_BENLIST=1|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

         */

        public static string DOMESTIC_TRANSFER(
            string custid, string src_acct, string pwd, 
            string des_acct, string des_name, string des_nick_name, string tran_type, string txdesc, 
            double amount, 
            string bank_code, string bank_name, string bank_branch, string bank_city, string bank_branch_name, string bank_city_name, string ccycd,
            string save_to_benlist, string ip, string user_agent, Hashtable hashTbl
        )
        {
            double fee_amount =0 ;
            string retStr = Config.DOMESTIC_TRANSFER;
            string check_before_trans = "";
            double eb_tran_id = 0;
            string core_txno_ref = "";
            string core_txdate_ref = "";
            string channel_id = Config.ChannelID;
            tran_type = Config.TRAN_TYPE_DOMESTIC;
            if(ccycd=="VND")
            {
                ccycd = "704";
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

            Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  BEGIN");


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

            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }


            if (check_before_trans == Config.ERR_CODE_DONE)
            {
                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //  eb_tran = transfer.insTransferTx

                Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  BEGIN INSERT EB TRAN");

                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "TRANSFER" //mod_cd
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
                    ,0 // tran pwd idx
                    ,"" //sms code
                    ,"" //sign data
                    ,"" //core err code
                    ,"" //core err desc
                    ,"" //core ref_no
                    ,"" //core txdate
                    , "" //core txtime
                    ,0 // order_amount
                    ,0 // order_dis
                    ,"" //order_id
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
                    , des_name // des name ten tai khoan thu huogn
                    , bank_code // bank code
                    , bank_name // ten ngan hang
                    , bank_city // ma thanh pho
                    , bank_branch // ma chi nhanh
                    , des_name // "" //bm1 //linhtn modify 2017 02 21 
                    , bank_city_name //"" //bm2
                    , bank_branch_name // "" //bm3
                    ,"" //bm4
                    ,"" //bm5
                    ,"" //bm6
                    ,"" //bm7
                    ,"" //bm8
                    ,"" //bm9
                    ,"" //bm10
                    ,"" //bm11
                    ,"" //bm12
                    ,"" //bm13
                    ,"" //bm14
                    ,"" //bm15
                    ,"" //bm16
                    ,"" //bm17
                    ,"" //bm18
                    ,"" //bm19
                    ,"" //bm20
                    ,"" //bm21
                    ,"" //bm22
                    ,"" //bm23
                    ,"" //bm24
                    ,"" //bm25
                    ,"" //bm26
                    ,requestId //bm27
                    , ip// "" //bm28
                    , user_agent // ""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && ( eb_tran.Rows.Count > 0))
                {
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  BEGIN POST TO CORE");

                    //retStr = Config.FUNDTRANSFER_INTRA;
                    string result = tf.PostDomesticTxnToCore(
                                  custid
                                  , eb_tran_id
                                  , src_acct
                                  , ccycd
                                  , des_acct
                                  , des_name
                                  , bank_code
                                  , bank_name
                                  , bank_city
                                  , bank_branch
                                  , bank_city_name
                                  , bank_branch_name
                                  , amount
                                  , fee_amount
                                  , txdesc
                                  , ref core_txno_ref
                                  , ref core_txdate_ref
                                 );
                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {
                        Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  POST TO CORE DONE");

                        Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  UPATE EB TRAN");

                        // cap nhat trang thai giao dich la thanh cong
                         tf.uptTransferTx(
                            eb_tran_id
                            , Config.TX_STATUS_DONE
                            , core_txno_ref
                            , core_txdate_ref
                            , channel_id
                             );
                        //SAVE TO BEN LIST
                        //khong can save to ben list
                        if (save_to_benlist == "0")
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "DOMESTIC TRANSFER IS COMPLETED TRAN_ID= " + eb_tran_id);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                        }
                        //can save to ben list
                        else if (save_to_benlist == "1")
                        {

                            Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  SAVE TO BENLIST =1");


                            // Goi ham save to BEN   

                            Beneficiarys ben = new Beneficiarys();
                            DataTable dt1 = new DataTable();
                            dt1 = ben.INSERT_BEN(
                                custid
                                , new Beneficiary().getTranTypePilot(custid, tran_type)
                                , des_acct
                                , des_name
                                , "" //des_nick_name (phan chuyen khoan khong co nhap ten viet tat kenh mobile)
                                , txdesc
                                , bank_code
                                , bank_name
                                , bank_branch
                                , bank_city
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
                                , Config.ChannelID// ""// bm8
                                , ip // ""// bm9
                                , user_agent // ""// bm10
                                );

                            if (dt1 != null && dt1.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "DOMESTIC TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id);
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                                Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  SAVE TO BENLIST DONE ACCTNO:" +des_acct);


                            }
                            else
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "FUNDTRANSFER INTRA IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);
                                Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  SAVE TO BENLIST FAILED ACCTNO:" + des_acct);

                            }
                            //giai phong bo nho                            
                            ben = null;
                            dt1 = null;

                        }
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {

                        Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  POST TO CORE FAILED");

                        tf.uptTransferTx(eb_tran_id
                         , Config.TX_STATUS_FAIL
                         , core_txno_ref
                         , core_txdate_ref
                         , channel_id);
                        //retStr = Config.CD_EB_TRANS_ERR_GENERAL;
                        retStr = Config.ERR_MSG_GENERAL;
                    }
                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    //retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);

                    //retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");

                    retStr = Config.ERR_MSG_GENERAL;

                    Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  COULD NOT INSERT EB TRAN");

                }
            }
            else // LOI AUTH, LIMIT, ....
            {
                retStr = check_before_trans;
            }

            Funcs.WriteLog("custid:" + custid + "|DOMESTIC_TRANSFER  END");

            return retStr;
        }
    }

}