using System;
using System.Collections.Generic;
using System.Configuration;

namespace mobileGW.Service.Framework
{
    /// <summary>
    /// Summary description for Config.
    /// </summary>
    public class Config
    {

        #region "For ESB config"

        /// <summary>
        /// linhtn add new 2017 03 23
        /// </summary>
        public static string FinancialStatus_NORMAL_CASA = "NORMAL";
        public static string FinancialStatus_NO_DEBIT_CASA = "NO_DEBIT";
        public static string FinancialStatus_NO_CREDIT_CASA = "NO_CREDIT";
        public static string FinancialStatus_BLOCKED_CASA = "BLOCKED";

        //DECODE(C.INACT_ST,'Y','INACTIVE','ACTIVE') INACT_ST --Account Status Change
        public static string AccountStatus_ACTIVE_CASA = "ACTIVE";
        public static string AccountStatus_IN_ACTIVE_CASA = "INACTIVE";

        public static string IS_CO_HOLDER = "1";
        public static string CCYCD_VND = "VND";


        public static string SharedKeyMD5 = "A2C75E5C3A4C0E23D01871A7DB4A8D9E";

        #endregion "For ESB config"

        public static String[] gResult_Setting_SHB_Mobile_Arr = new String[]{
                                                                    "E00|Cập nhật thành công.|Your info changed sucessfully",
                                                                    "E99|Cập nhật không thành công.|Change info unsucessfully",
                                                                    "E90|Số điện thoại đã đăng ký|Error in register mobile number. Please enter mobile number field again",
                                                                    "E99999|Có lỗi trên đường truyền|General error"
                                                                };

        public const decimal MOB_REV_AUTH_METHOD = 0; //HUY DICH VU SHB MOBILE
        public const decimal MOB_REG_AUTH_METHOD = 2; //DANG KY DICH VU SHB MOBILE
        public class Email
        {
            public static string IPMailServer = ConfigurationManager.AppSettings.Get("MAIL_SERVER_IP");
            public static int mailServerPort = Int32.Parse(ConfigurationManager.AppSettings.Get("MAIL_SERVER_PORT"));
            public static string username = ConfigurationManager.AppSettings.Get("MAIL_SERVER_USER");
            public static string password = ConfigurationManager.AppSettings.Get("MAIL_SERVER_PASS");
            public static string domain = ConfigurationManager.AppSettings.Get("MAIL_SERVER_DOMAIN");
            public static string fromEmail = ConfigurationManager.AppSettings.Get("MAIL_SERVER_FROM");
            public static string senderEmail = ConfigurationManager.AppSettings.Get("MAIL_SERVER_FROM_SENDER");
        }


        #region "REPLY MESSAGE TEMPLATE"
        #region "Other"
        public static String[] CMD_IGNORE_CHECK = new String[] {
            "CHECK_LOGIN","SET_FINGER_PRINT","LOGIN_FP"
            , "ACTIVE_MOB", "ACTIVE_MOB_CONFIRM", "GET_CUR_VER"
            , "GET_LOCATION_LIST"
            , "GET_LOCATION_LIST_BY_ADDRESS"
            , "GET_FX_RATE"
            , "GET_TIDE_RATE"
            ,"CHANGE_PWD_VERIFY"
            ,"GET_DETAIL_TRAN"
            , "SYNCHRONIZE_OTP"
            , "GET_ACTIVATION_CODE"
            , "GET_INFO_TOKEN_POPUP"
            , "UPDATE_STATUS_SHOW_TOKEN_POPUP"
            , "GET_UNREAD_NEWS"
            ,"BILLING"
            ,"FUNDTRANSFER_INTRA"
            ,"GET_OTP"
            ,"GET_ACCT_LIST_QRY_N"
            ,"GET_BENNAME_FROM_CASA_ACCOUNT"

            /*,"REGISTER_PUSH_NOTIFICATION"
            ,"LIST_FUNCTION_PUSH"
            ,"UPDATE_FUNCTION_PUSH"
            ,"LIST_NEWS"
            ,"VIEW_NEWS"*/
        };

        public static String COL_DLMT = "#"; //Dau ngan cach NAME va VALUE: (Ex: NAME COL_DLMT VALUE). ALT 171
        public static String ROW_DLMT = "|"; //Dau ngan cach cac truong (Ex: NAME1 COL_DLMT VALUE1 ROW_DLMT NAME2 COL_DLMT VALUE2). ALT 181
        public static String COL_REC_DLMT = "$"; //Dau ngan cach cac truong trong 1 RECORD . ALT 201
        public static String ROW_REC_DLMT = "^"; //Dau ngan cach cac RECORD trong truong hop tra ve nhieu ban ghi. ALT 191

        public static Double LIMIT_MIN_BALANCE_STAFF = 50000; // TOI THIEU NHAN VIEN PHAI DE LAI 50K.
        public static String ERR_CODE_VAL = "{ERR_CODE}";
        public static String ERR_DESC_VAL = "{ERR_DESC}";

        public const String TYPE_CASA_PRODUCT = "001";
        public const String TYPE_TIDE_PRODUCT = "002";
        public const String TYPE_LOAN_PRODUCT = "003";

        //anhnd2
        public const String CD_EB_TRANS_DONE = "0000";
        public const String CD_EB_TRANS_DONE_BEN_FAILED = "1001";
        public const String CD_EB_TRANS_DONE_BEN_EXIST = "1002";
        public const String CD_EB_TRANS_TIME_OUT = "1068";
        public const String CD_EB_TRANS_ERR_GENERAL = "9999";



        public static String ERR_CODE_DONE = "00";
        public static String StatusCode_Success = "000";

        //linhtn 16 jul 2016

        public static String ERR_CODE_REVERT = "RV"; //


        public static String ERR_CODE_ACTIVE_CODE_EXPIRE = "97";
        public static String NOT_FOUND = "NOT_FOUND";
        public static String ERR_CODE_INVALID_LOGIN = "99";
        public static String ERR_CODE_GENERAL = "99";
        public static String ERR_CODE_QR_NOT_PAYMENT = "01";
        public static String ERR_CODE_INVALID_ACTIVE_CODE = "98";
        public static String ERR_CODE_TIMEOUT = "08";
        public static String ERR_CODE_TOKEN_INVALID = "77";
        public static String ERR_CODE_CIF_NOT_REG_TOKEN = "78";
        public static String ERR_CODE_LIMIT_THAU_CHI = "79";

        #region TOKEN OTP
        public static String TypeMToken = "5";
        public static String NameMToken = "mToken";
        public static String ERR_TOKEN_MSG_INVALID_TRANPWD = "ERR_CODE" + COL_DLMT + "51" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "INVALID OTP";
        #endregion

        public static String ERR_MSG_VIOLATE_MAX_TIDE_OL = "ERR_CODE" + COL_DLMT + "2000" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "VUOT QUA SO LUONG SO TIET KIEM ONLINE CHO PHEP BOOK";

        //anhnd2 03/07/2016 


        public static String SUCCESS_MSG_GENERAL = "ERR_CODE" + COL_DLMT + "00" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "SUCCESSFUL";


        public static String ERR_MSG_GENERAL = "ERR_CODE" + COL_DLMT + "99" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "LOI KHONG XAC DINH";
        public static String ERR_MSG_GENERAL_ADDCIF = "ERR_CODE" + COL_DLMT + "99" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}" + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}";
        public static String ERR_MSG_OVER_OTP = "ERR_CODE" + COL_DLMT + "10" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "OVER OTP";
        //anhnd2 02/12/2015 them err_code 
        public static String ERR_MSG_TIMEOUT = "ERR_CODE" + COL_DLMT + "08" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "TIME OUT HOAC EXCEPTION DAU SML";

        //DAU 8: DUNG BAO LOI VE PHAN POSTPAID
        public static String ERR_MSG_POSTPAID_PARTIAL_AMOUNT = "ERR_CODE" + COL_DLMT + "80" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "HOA DON KHONG CHO THANH TOAN 1 PHAN";

        public static String ERR_MSG_NO_DATA_FOUND = "ERR_CODE" + COL_DLMT + "00" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "NO DATA FOUND" +
                                                    ROW_DLMT + "RECORD" + COL_DLMT + "{NULL}";

        public static String ERR_MSG_INVALID_TRANPWD = "ERR_CODE" + COL_DLMT + "11" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "INVALID TRANPWD";

        public static String ERR_MSG_INVALID_CURRENT_PWD = "ERR_CODE" + COL_DLMT + "12" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "INVALID CURRENT PASS";
        public static String ERR_CODE_INVALID_CURRENT_PWD = "12";

        public static String ERR_MSG_VIOLATE_LIMIT = "ERR_CODE" + COL_DLMT + "13" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "VIOLATE LIMIT";

        public static String ERR_MSG_VIOLATE_LIMIT_PERTRAN = "ERR_CODE" + COL_DLMT + "1301" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "VIOLATE LIMIT PERTRAN";

        public static String ERR_MSG_INVALID_SECURE_CODE = "ERR_CODE" + COL_DLMT + "14" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "INVALID SECURE CODE";

        public static String ERR_MSG_INVALID_MOBILE_NO = "ERR_CODE" + COL_DLMT + "16" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";

        public static String ERR_MSG_INVALID_CARD_NO = "ERR_CODE" + COL_DLMT + "17" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";

        public static String ERR_MSG_INVALID_DES_ACCTNO = "ERR_CODE" + COL_DLMT + "18" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";

        public static String ERR_MSG_INVALID_ACTIVE_CODE = "ERR_CODE" + COL_DLMT + "19" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";

        public static String ERR_MSG_INVALID_TOKEN = "ERR_CODE" + COL_DLMT + "77" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";

        public static String ERR_MSG_INVALID_TOKEN_FP = "ERR_CODE" + COL_DLMT + "27" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";

        public static String INVALID_BILL_CODE = "15";

        public static String ERR_MSG_FORMAT = "ERR_CODE" + COL_DLMT + "{0}" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{1}";
        public static String ERR_MSG_FORMAT_NEW = "ERR_CODE" + COL_DLMT + "{0}" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{1}" + ROW_DLMT + "CIF_NO" + COL_DLMT + "{2}";
        public static String ERR_MSG_AMOUNT_LESS_FEE = "ERR_CODE" + COL_DLMT + "28" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "AMOUNT LESS THAN TOTAL FEE";
        public static String BEGIN_TAG = "<MSG>";
        public static String END_TAG = "</MSG>";
        #endregion "other"


        #region "STOCK"
        public static String GET_STOCK_BRANCH_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";
        #endregion "STOCK"

        #region "TIDE ONLINE"
        public static String GET_ACCT_TIDE_OL_INFO_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "TOTAL_AMOUNT" + COL_DLMT + "{TOTAL_AMOUNT}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
;
        #endregion"TIDE ONLINE"


        #region "CHARITY"
        public static String GET_CHARITY_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";
        #endregion "CHARITY"


        #region "Payment"
        public static String GET_CATEGORY_BY_TRAN_TYPE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
                + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
                + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_SERVICES = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_LIST_MOBILE_PRICE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
        + ROW_DLMT + "PARTNER_ID" + COL_DLMT + "{PARTNER_ID}"
        + ROW_DLMT + "CATEGORY_ID" + COL_DLMT + "{CATEGORY_ID}"
        + ROW_DLMT + "SERVICE_ID" + COL_DLMT + "{SERVICE_ID}"
        + ROW_DLMT + "CATEGORY_CODE" + COL_DLMT + "{CATEGORY_CODE}"
        + ROW_DLMT + "SERVICE_CODE" + COL_DLMT + "{SERVICE_CODE}"
       + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";


        public static String GET_PRICE_TOPUP_OTHER = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
   + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
   + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
   + ROW_DLMT + "PARTNER_ID" + COL_DLMT + "{PARTNER_ID}"
   + ROW_DLMT + "CATEGORY_ID" + COL_DLMT + "{CATEGORY_ID}"
   + ROW_DLMT + "SERVICE_ID" + COL_DLMT + "{SERVICE_ID}"
   + ROW_DLMT + "CATEGORY_CODE" + COL_DLMT + "{CATEGORY_CODE}"
   + ROW_DLMT + "SERVICE_CODE" + COL_DLMT + "{SERVICE_CODE}"
  + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";


        public static String GET_BILL_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
             + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
             + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
             + ROW_DLMT + "BILL_ID" + COL_DLMT + "{BILL_ID}"
 + ROW_DLMT + "BILL_AMOUNT" + COL_DLMT + "{BILL_AMOUNT}"
 + ROW_DLMT + "BILL_INFO_EXT1" + COL_DLMT + "{BILL_INFO_EXT1}"
 + ROW_DLMT + "BILL_INFO_EXT2" + COL_DLMT + "{BILL_INFO_EXT2}"
 + ROW_DLMT + "PARTNER_ID" + COL_DLMT + "{PARTNER_ID}"
            + ROW_DLMT + "CATEGORY_CODE" + COL_DLMT + "{CATEGORY_CODE}"
            + ROW_DLMT + "SERVICE_CODE" + COL_DLMT + "{SERVICE_CODE}"
            + ROW_DLMT + "SERVICE_ID" + COL_DLMT + "{SERVICE_ID}"
            + ROW_DLMT + "DESCRIPTION" + COL_DLMT + "{DESCRIPTION}"
             ;
       public static String GET_BILLS_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
             + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
             + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
             + ROW_DLMT + "CUSTOMER_CODE" + COL_DLMT + "{CUSTOMER_CODE}"
             + ROW_DLMT + "BILL_INFO_EXT1" + COL_DLMT + "{BILL_INFO_EXT1}"
             + ROW_DLMT + "BILL_INFO_EXT2" + COL_DLMT + "{BILL_INFO_EXT2}"
             + ROW_DLMT + "PARTNER_ID" + COL_DLMT + "{PARTNER_ID}"
             + ROW_DLMT + "DESCRIPTION" + COL_DLMT + "{DESCRIPTION}"
             + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        #endregion "Payment"

        #region "UTIL"

        public static String GET_BEN_LIST_CUSTID_TRANTYPE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "TRAN_TYPE" + COL_DLMT + "{TRAN_TYPE}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String HANDLE_BENLIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "TRAN_TYPE" + COL_DLMT + "{TRAN_TYPE}";

        public static String GET_BANK_CODE_CITAD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_CITY_CITAD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
          + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_BANK_BRANCH_CITAD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
  + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_BANK_CODE_247_AC2AC = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
  + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
  + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_247_ACCT_HOLDER = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
  + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
  + ROW_DLMT + "ACCT_NO" + COL_DLMT + "{ACCT_NO}"
  + ROW_DLMT + "BEN_NAME" + COL_DLMT + "{BEN_NAME}";

        public static String GET_247_CARD_HOLDER = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
  + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
  + ROW_DLMT + "CARD_NO" + COL_DLMT + "{CARD_NO}"
  + ROW_DLMT + "BEN_NAME" + COL_DLMT + "{BEN_NAME}"
   + ROW_DLMT + "BANK_NAME" + COL_DLMT + "{BANK_NAME}";

        public static String GET_TIDE_INTEREST_RATE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
   + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
   + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
   + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        //Trong đó:
        // RECORD: CCYCD^TENURE^UNIT_TENURE^INT_RATE


        public static String TIDEBOOKING = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
     + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
     + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
     + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
     + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
     + ROW_DLMT + "TIDE_ACCT" + COL_DLMT + "{TIDE_ACCT}"
     + ROW_DLMT + "MAT_DT" + COL_DLMT + "{MAT_DT}"
     + ROW_DLMT + "INT_RATE" + COL_DLMT + "{INT_RATE}"
     + ROW_DLMT + "INT_AMT" + COL_DLMT + "{INT_AMT}"
     + ROW_DLMT + "VAL_DT" + COL_DLMT + "{VAL_DT}"
     ;

        public static String TIDEWDL = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
                    + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
                    + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
                    + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
                    + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
                    + ROW_DLMT + "INT_AMT" + COL_DLMT + "{INT_AMT}"
            ;

        //linhtn 20160720
        public static String GETGET_FEE_BY_TRAN_TYPE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "TRAN_TYPE" + COL_DLMT + "{TRAN_TYPE}"
           + ROW_DLMT + "TOTAL_FEE" + COL_DLMT + "{TOTAL_FEE}";


        public static String SETTING = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
                 + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
                 + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"

         ;

        public static String CHECK_REF_INVITE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";

        public static String UPDATE_REF_INVITE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
          + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";



        public static String GET_INBOX_OUTBOX = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String SEND_MAIL_OUTBOX = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            ;
        public static String DELETE_MAIL = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
                    + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}";




        #endregion "UTIL"

        #region "Enquiry"

        public static string ENQ_TYPE_LAST5 = "LAST5";
        public static string ENQ_TYPE_TODAY = "TODAY";
        public static string ENQ_TYPE_THIS_MONTH = "THIS_MONTH";
        public static string LAST_MONTH = "LAST_MONTH";
        public static string ENQ_TYPE_THIS_WEEK = "THIS_WEEK";
        public static string ENQ_TYPE_LAST_WEEK = "LAST_WEEK";
        public static string ENQ_TYPE_FR_2_TO_DATE = "FR_2_TO_DATE";


        public static string OUT_CUR = "OUT_CUR";

        public static String GET_ACCT_LIST_HOMESCREEN_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "CASA_TOTAL" + COL_DLMT + "{CASA_TOTAL}"
            + ROW_DLMT + "TIDE_TOTAL" + COL_DLMT + "{TIDE_TOTAL}"
            + ROW_DLMT + "LOAN_TOTAL" + COL_DLMT + "{LOAN_TOTAL}";

        public static String GET_ACCT_BALANCE_HOMESCREEN_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
            + ROW_DLMT + "AVAIL_BALANCE" + COL_DLMT + "{AVAIL_BALANCE}"
            + ROW_DLMT + "CURR_BALANCE" + COL_DLMT + "{CURR_BALANCE}";

        public static String GET_DETAIL_TRAN = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "DETAIL" + COL_DLMT + "{DETAIL}"
            ;

        public static String GET_ACCT_LIST_QRY_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "CASA_TOTAL" + COL_DLMT + "{CASA_TOTAL}"
            + ROW_DLMT + "TIDE_TOTAL" + COL_DLMT + "{TIDE_TOTAL}"
            + ROW_DLMT + "LOAN_TOTAL" + COL_DLMT + "{LOAN_TOTAL}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;


        public static String GET_ACCT_CASA_INFO_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "ACCT_CURR_BALANCE" + COL_DLMT + "{ACCT_CURR_BALANCE}"
            + ROW_DLMT + "ACCT_AVAI_BALANCE" + COL_DLMT + "{ACCT_AVAI_BALANCE}"
            + ROW_DLMT + "ERR_CODE_BALANCE_HIST" + COL_DLMT + "{ERR_CODE_BALANCE_HIST}"
            + ROW_DLMT + "ERR_CODE_RECORD_ACTIVITY" + COL_DLMT + "{ERR_CODE_RECORD_ACTIVITY}"
            + ROW_DLMT + "RECORD_BALANCE_HIST" + COL_DLMT + "{RECORD_BALANCE_HIST}"
            + ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
            + ROW_DLMT + "COMBO" + COL_DLMT + "{COMBO}"
            ;

        public static String GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
            ;


        public static String GET_ACCT_TIDE_INFO_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
+ ROW_DLMT + "CCYCD" + COL_DLMT + "{CCYCD}"
+ ROW_DLMT + "PRODUCT_CODE" + COL_DLMT + "{PRODUCT_CODE}"
+ ROW_DLMT + "CURR_PRIN_AMT" + COL_DLMT + "{CURR_PRIN_AMT}"

+ ROW_DLMT + "TENURE" + COL_DLMT + "{TENURE}"
+ ROW_DLMT + "UNIT_TENURE" + COL_DLMT + "{UNIT_TENURE}"
+ ROW_DLMT + "INT_RATE" + COL_DLMT + "{INT_RATE}"
+ ROW_DLMT + "INT_AMT" + COL_DLMT + "{INT_AMT}"
+ ROW_DLMT + "MAT_DT" + COL_DLMT + "{MAT_DT}"
+ ROW_DLMT + "VAL_DT" + COL_DLMT + "{VAL_DT}"
+ ROW_DLMT + "AUTO_REN_NO" + COL_DLMT + "{AUTO_REN_NO}"
+ ROW_DLMT + "CURR_MAT_AMT" + COL_DLMT + "{CURR_MAT_AMT}"
+ ROW_DLMT + "IS_SHOW_INTEREST" + COL_DLMT + "{IS_SHOW_INTEREST}"
+ ROW_DLMT + "ALLOW_TIDEWDL_ONLINE" + COL_DLMT + "{ALLOW_TIDEWDL_ONLINE}"
+ ROW_DLMT + "INSTRUCTION" + COL_DLMT + "{INSTRUCTION}"
+ ROW_DLMT + "DEPOSIT_NO" + COL_DLMT + "{DEPOSIT_NO}"
;


        public static String GET_ACCT_LOAN_INFO_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
+ ROW_DLMT + "CCYCD" + COL_DLMT + "{CCYCD}"
 + ROW_DLMT + "OBALANCE" + COL_DLMT + "{OBALANCE}"
 + ROW_DLMT + "OUT_STANDING" + COL_DLMT + "{OUT_STANDING}"
+ ROW_DLMT + "OPENDT" + COL_DLMT + "{OPENDT}"
+ ROW_DLMT + "EXPDT" + COL_DLMT + "{EXPDT}"
+ ROW_DLMT + "NEXT_INT_DUE" + COL_DLMT + "{NEXT_INT_DUE}"
+ ROW_DLMT + "NEXT_PRIN_DUE" + COL_DLMT + "{NEXT_PRIN_DUE}"
+ ROW_DLMT + "NEXT_AMT_PRIN_DUE" + COL_DLMT + "{NEXT_AMT_PRIN_DUE}"
+ ROW_DLMT + "NEXT_AMT_INT_DUE" + COL_DLMT + "{NEXT_AMT_INT_DUE}"
+ ROW_DLMT + "SINT" + COL_DLMT + "{SINT}"
+ ROW_DLMT + "ERR_CODE_RECORD_ACTIVITY" + COL_DLMT + "{ERR_CODE_RECORD_ACTIVITY}"
+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}";


        public static String GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "ERR_CODE_RECORD_ACTIVITY" + COL_DLMT + "{ERR_CODE_RECORD_ACTIVITY}"
+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
            ;

        public static String GET_ACCT_LN_REPAYMENT_SCHEDULE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;




        public static String GET_ACCT_BALANCE_LIST_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_ACCT_LIST_RECEIVE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
        + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_BENNAME_FROM_CASA_ACCOUNT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "DES_ACCT" + COL_DLMT + "{DES_ACCT}"
+ ROW_DLMT + "DES_NAME" + COL_DLMT + "{DES_NAME}";




        public static String GET_LOCATION_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "TYPE" + COL_DLMT + "{TYPE}"
            + ROW_DLMT + "CUR_LAT" + COL_DLMT + "{CUR_LAT}"
            + ROW_DLMT + "CUR_LONG" + COL_DLMT + "{CUR_LONG}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";


        public static String GET_CARD_TRAN_BY_ENQ_TYPE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}";


        public static String GET_DEBIT_CARD_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
//+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
//+ ROW_DLMT + "BALANCE" + COL_DLMT + "{BALANCE}"
//+ ROW_DLMT + "OPEN_DATE" + COL_DLMT + "{OPEN_DATE}"
//+ ROW_DLMT + "STATUS" + COL_DLMT + "{STATUS}"
+ ROW_DLMT + "ERR_CODE_RECORD_ACTIVITY" + COL_DLMT + "{ERR_CODE_RECORD_ACTIVITY}"
+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}";


        public static String GET_FX_RATE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String GET_TIDE_RATE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";


        public static String RES_CHECK_NATIONALITY = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "RET_CODE" + COL_DLMT + "{RET_CODE}";

        #endregion "Enquiry"

        #region ResponseMsg Habeco
        public static String RES_HABECO_GETCUSTOMER = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CUSTOMER_INFO" + COL_DLMT + "{CUSTOMER_INFO}";
        #endregion

        #region "TRAN_TYPE"

        public const string TRAN_TYPE_SELF = "SELF";
        public const string TRAN_TYPE_INTRA = "INTRA";
        public const string TRAN_TYPE_TOPUP_MOBILE = "TOPUP_MOBILE";
        public const string TRAN_TYPE_TOPUP_OTHER = "TOPUP_OTHER";
        public const string TRAN_TYPE_BILL_MOBILE = "BILL_MOBILE";
        public const string TRAN_TYPE_BILL_OTHER = "BILL_OTHER";
        public const string TRAN_TYPE_ECOM = "ECOM";
        public const string TRAN_TYPE_BATCH = "BATCH";
        public const string TRAN_TYPE_DOMESTIC = "DOMESTIC";
        //add new
        public const string TRAN_TYPE_DOMESTIC_ACC = "DOMESTIC_ACC";
        public const string TRAN_TYPE_BEN_247AC = "BEN_247AC";
        public const string TRAN_TYPE_BEN_247CARD = "BEN_247CARD";
        public const string TRAN_TYPE_ACQ_247AC = "ACQ_247AC";
        public const string TRAN_TYPE_ACQ_247CARD = "ACQ_247CARD";
        public const string TRAN_TYPE_CREDIT_PAYMENT = "CREDIT_PAYMENT";
        public const string TRAN_TYPE_TOPUP_SHS = "TOPUP_SHS";
        public const string TRAN_TYPE_TOPUP_SHBS = "TOPUP_SHBS";
        public const string TRAN_TYPE_TIDEBOOKING = "TIDEBOOKING";
        public const string TRAN_TYPE_TIDEWDL = "TIDEWDL";
        public const string TRAN_TYPE_CHARITY = "CHARITY";
        public const string TRAN_TYPE_MANCITY = "MANCITY";
        public const string TRAN_TYPE_OPEN_CARD_NORM = "OPEN_CARD_NORM";
        public const string TRAN_TYPE_CLOSE_CARD_NORM = "CLOSE_CARD_NORM";
        public const string TRAN_TYPE_SET_PUR_LIMIT = "SET_PUR_LIMIT";
        public const string TRAN_TYPE_SET_PUR_LIMIT_INTER = "SET_PUR_LIMIT_INTER";
        public const string TRAN_TYPE_CLOSE_CARD_INTER = "CLOSE_CARD_INTER";
        public const string TRAN_TYPE_OPEN_CARD_INTER = "OPEN_CARD_INTER";
        public const string TRAN_TYPE_REG_EBANK_ONLINE = "REG_EBANK_ONLINE";
        public const string TRAN_TYPE_SET_LIMIT_CREDIT_CARD = "SET_LIMIT_CREDIT_CARD";
        public const string TRAN_TYPE_AUTO_DEBIT = "AUTO_DEBIT_CARD";
        public const string TRAN_TYPE_AUTO_SAVING = "TIDE_AUTO_SAVING";
        public const string TRAN_TYPE_ACCT_NICE = "CREATE_ACCT_NICE";
        public const string TRAN_TYPE_GIVE_GIFT = "GIVE_GIFT";
        public const string TRAN_TYPE_RECEIVE_GIFT = "RECEIVE_GIFT";
        //add new
        public const string TRAN_TYPE_QR_PAYMENT = "QR_PAYMENT";


        public const string EB_ACTION_CHANGE_PWD = "CHANGE_PWD";
        public const string EB_ACTION_GET_ACTIVE_CODE = "GET_ACTIVE_CODE";
        public const string EB_ACTION_ACTIVE_CONFIRM = "ACTIVE_CONFIRM";
        public const string EB_ACTION_UPDATE_BENLIST = "INSERT_UPDATE_BENLIST";

        public const string EB_ACTION_LOGIN = "LOGIN";
        public const string EB_ACTION_LOGIN_FP = "LOGIN_FP";

        #endregion "TRAN_TYPE"


        //CMD#GET_DES_NAME|CIF_NO#0310008705
        public static String GET_DES_NAME = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "DES_ACCT" + COL_DLMT + "{DES_ACCT}"
            + ROW_DLMT + "DES_NAME" + COL_DLMT + "{DES_NAME}"
            ;


        public static String GET_ACCT_BALANCE_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;

        public static String GET_CUR_VER = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CUR_VER" + COL_DLMT + "{CUR_VER}"
            + ROW_DLMT + "COUNT_IMG" + COL_DLMT + "{COUNT_IMG}"
            ;

        public static String GET_LIMIT_TRANSACTION_AMOUNT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "LIMIT_247AC" + COL_DLMT + "{LIMIT_247AC}"
            + ROW_DLMT + "LIMIT_247CARD" + COL_DLMT + "{LIMIT_247CARD}"
            + ROW_DLMT + "LIMIT_DOMESTIC" + COL_DLMT + "{LIMIT_DOMESTIC}"
            + ROW_DLMT + "REMAIN_LIMIT_PER_DAY" + COL_DLMT + "{REMAIN_LIMIT_PER_DAY}"
            ;
        public static String GET_LIMIT_TRANSACTION_AMOUNT_V2 = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "LIMIT_247AC" + COL_DLMT + "{LIMIT_247AC}"
            + ROW_DLMT + "LIMIT_247CARD" + COL_DLMT + "{LIMIT_247CARD}"
            + ROW_DLMT + "LIMIT_DOMESTIC" + COL_DLMT + "{LIMIT_DOMESTIC}"
            + ROW_DLMT + "REMAIN_LIMIT_PER_DAY" + COL_DLMT + "{REMAIN_LIMIT_PER_DAY}"
            + ROW_DLMT + "REG_USER" + COL_DLMT + "{REG_USER}"
            + ROW_DLMT + "LIMIT_AMT_MONTH" + COL_DLMT + "{LIMIT_AMT_MONTH}"
            ;


        public static String BILL_PAY = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
            + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}";


        public static String TOPUP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
            + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}";



        public static String FUNDTRANSFER_INTRA = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
              + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
            ;

        public static String DOMESTIC_TRANSFER = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
            + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
            ;

        public static String TRANSFER_247 = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
        + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
        + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
            ;

        public static String CHANGE_PWD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            ;

        public static String CHECK_LOGIN = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "PKG_TYPE" + COL_DLMT + "{PKG_TYPE}"
            + ROW_DLMT + "DEFAULT_ACCT" + COL_DLMT + "{DEFAULT_ACCT}"
            + ROW_DLMT + "DEFAULT_LANG" + COL_DLMT + "{DEFAULT_LANG}"
            + ROW_DLMT + "AVATAR" + COL_DLMT + "{AVATAR}"
            + ROW_DLMT + "CUST_NAME" + COL_DLMT + "{CUST_NAME}"
            + ROW_DLMT + "REQ_PWD_CHANGE" + COL_DLMT + "{REQ_PWD_CHANGE}"
             + ROW_DLMT + "AUTH_METHOD" + COL_DLMT + "{AUTH_METHOD}"
            //anhnd2  05/01/2017 Them sdt tra ve.
            + ROW_DLMT + "AUTH_INFO_EXT1" + COL_DLMT + "{AUTH_INFO_EXT1}"
            + ROW_DLMT + "TOKEN" + COL_DLMT + "{TOKEN}"
            + ROW_DLMT + "FP_TOKEN" + COL_DLMT + "{FP_TOKEN}"
            + ROW_DLMT + "DATE_EXP_PWD" + COL_DLMT + "{DATE_EXP_PWD}"
            + ROW_DLMT + "EMAIL" + COL_DLMT + "{EMAIL}"
            ;


        public static String ACTIVE_MOB = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "CUST_NAME" + COL_DLMT + "{CUST_NAME}"
            + ROW_DLMT + "DEFAULT_ACCT" + COL_DLMT + "{DEFAULT_ACCT}"
            + ROW_DLMT + "DEFAULT_LANG" + COL_DLMT + "{DEFAULT_LANG}"
            + ROW_DLMT + "AVATAR" + COL_DLMT + "{AVATAR}"
            + ROW_DLMT + "AUTH_METHOD" + COL_DLMT + "{AUTH_METHOD}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            ;

        public static String ACTIVE_MOB_CONFIRM = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "REQ_PWD_CHANGE" + COL_DLMT + "{REQ_PWD_CHANGE}"
           + ROW_DLMT + "TOKEN" + COL_DLMT + "{TOKEN}"
           + ROW_DLMT + "EMAIL" + COL_DLMT + "{EMAIL}"
           ;

        public static String GET_ACCT_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;
        public static String GET_ACCT_BALANCE_CASA = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
            + ROW_DLMT + "AVAIL_BALANCE" + COL_DLMT + "{AVAIL_BALANCE}"
            + ROW_DLMT + "CURR_BALANCE" + COL_DLMT + "{CURR_BALANCE}";

        public static String GET_ACCT_BALANCE_TIDE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
            + ROW_DLMT + "BALANCE" + COL_DLMT + "{BALANCE}";

        public static String GET_ACCT_BALANCE_LOAN = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
            + ROW_DLMT + "ORG_AMOUNT" + COL_DLMT + "{ORG_AMOUNT}"
            + ROW_DLMT + "OUTSTANDING_AMOUNT" + COL_DLMT + "{OUTSTANDING_AMOUNT}";

        //CMD#GET_ACCT_ENQUIRY|CIF_NO#0310008705|ACCTNO#1000013376|TYPE#001
        public static String GET_ACCT_ENQUIRY = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";



        //CMD#GET_PRICE|CATID#PREPAID|PROVIDER_ID#VTEL|PARTNER#PAYNET
        public static String GET_PRICE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;


        public static String SET_FP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "FP_TOKEN" + COL_DLMT + "{FP_TOKEN_VALUE}"
           ;

        public static String LOGOUT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
          + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
          ;

        //CMD#GET_CARD_LIST|CIF_NO#0310008705
        //CHST0, CHST17 --> BINH THUONG
        //CHST6, CHST7, CHST12  --> TAM KHOA
        //KHAC  --> CHUA KICH HOAT
        public static String GET_CARD_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        //CMD#GET_CARD_INFO|CIF_NO#0310008705|CARD_NO
        //CHST0, CHST17 --> BINH THUONG
        //CHST6, CHST7, CHST12  --> TAM KHOA
        //KHAC  --> CHUA KICH HOAT
        public static String GET_CARD_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "CARD_NO" + COL_DLMT + "{CARD_NO}"
            + ROW_DLMT + "HOLDER_NAME" + COL_DLMT + "{HOLDER_NAME}"
            + ROW_DLMT + "CARD_TYPE" + COL_DLMT + "{CARD_TYPE}"
            + ROW_DLMT + "ISSUE_DATE" + COL_DLMT + "{ISSUE_DATE}"
            + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
            + ROW_DLMT + "STATUS" + COL_DLMT + "{STATUS}"
            ;



        //CMD#GET_FXRATE|CIF_NO#0310008705
        public static String GET_FXRATE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";


        //CMD#BILL_ENQUIRY|CATEGORY_CODE#400000|PROVIDER_CODE#198000|PARTNER#VNPAY|BIIL_CODE#24242424
        //string  retXML= myWS.SHB_OneBill_Query( txtCode.Text, Session["CategoryCode"].ToString() ,  Session["ProviderCode"].ToString());
        public static String BILL_ENQUIRY = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "BILL_CODE" + COL_DLMT + "{BILL_CODE}"
            + ROW_DLMT + "BILL_AMOUNT" + COL_DLMT + "{BILL_AMOUNT}"
            + ROW_DLMT + "CHECK_AMOUNT" + COL_DLMT + "{CHECK_AMOUNT}"
            ;

        //CMD#GET_CARD_HOLDER_NAME_SMLGW|CARD_NO#9704XXXXXXXXXXXXXXX
        //string  retXML= myWS.SHB_OneBill_Query( txtCode.Text, Session["CategoryCode"].ToString() ,  Session["ProviderCode"].ToString());
        public static String GET_CARD_HOLDER_NAME_SMLGW = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CARD_HOLDER_NAME" + COL_DLMT + "{CARD_HOLDER_NAME}";


        public static String QR_PAYMENT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
           + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
           + ROW_DLMT + "PAYMENT_CODE" + COL_DLMT + "{PAYMENT_CODE}";

        public static String RESPONE_GET_QR_DISCOUNT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC_VI" + COL_DLMT + "{ERR_DESC_VI}"
            + ROW_DLMT + "ERR_DESC_EN" + COL_DLMT + "{ERR_DESC_EN}"
            + ROW_DLMT + "VALUE" + COL_DLMT + "{VALUE}";
        public static String RESPONE_GET_BILL_DAIICHI = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "POLICY_DETAIL" + COL_DLMT + "{POLICY_DETAIL}";

        public static String RESPONE_GET_BILL_DAIICHI_EOD = "31";

        public static String RESPONE_RESSET_PASSWORD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "TRAN_ID" + COL_DLMT + "{TRAN_ID}"
            + ROW_DLMT + "AUTH_METHOD" + COL_DLMT + "{AUTH_METHOD}"
             + ROW_DLMT + "REQUEST_ID" + COL_DLMT + "{REQUEST_ID}"
             + ROW_DLMT + "CHALLENGE_CODE" + COL_DLMT + "{CHALLENGE_CODE}";
        public static String RESPONE_RESSET_PASSWORD_CONFIRM = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "EMAIL" + COL_DLMT + "{EMAIL}"
           + ROW_DLMT + "MOBILE" + COL_DLMT + "{MOBILE}";

        public static String RESPONE_COMBO_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "DESCRIPTION_VN" + COL_DLMT + "{DESCRIPTION_VN}"
           + ROW_DLMT + "DESCRIPTION_ENG" + COL_DLMT + "{DESCRIPTION_ENG}"
           + ROW_DLMT + "CONTENT_VN" + COL_DLMT + "{CONTENT_VN}"
           + ROW_DLMT + "CONTENT_ENG" + COL_DLMT + "{CONTENT_ENG}"
       ;

        #region Rest AUTO SAVING
        public static String RESPONE_GET_LIST_AUTO_SAVING = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
        ;
        public static String RESPONE_HANDLE_AUTO_SAVING = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "REF_NO" + COL_DLMT + "{REF_NO}"
           + ROW_DLMT + "CORE_DT" + COL_DLMT + "{CORE_DT}"
           + ROW_DLMT + "START_DATE" + COL_DLMT + "{START_DATE}"
        ;
        public static String[] RES_AUTO_SAVING = new String[] {
                                "00|Successfully|Successfully",
                                "99|Lỗi không xác định.|An unknown error.",
                                "01|Cập nhật không thành công.|Change info unsucessfully",
                                "02|Cập nhật không thành công.|Change info unsucessfully",
                                "03|Cập nhật không thành công.|Change info unsucessfully",
                                "04|Cập nhật không thành công.|Change info unsucessfully",
                                "05|Cập nhật không thành công.|Change info unsucessfully",
                                "06|Cập nhật không thành công.|Change info unsucessfully",
                                "07|Cập nhật không thành công.|Change info unsucessfully",
                                "08|Cập nhật không thành công.|Change info unsucessfully",
                                "09|Cập nhật không thành công.|Change info unsucessfully",
                                "10|Tài khoản nguồn không tồn tại.|Source  account is invalid",
                                "11|Trạng thái tài khoản không đủ điều kiện  để sử dụng dịch vụ, vui lòng kiểm tra lại.|The account status is not eligible to use this product. Please check again",
                                "12|Tài khoản nguồn đã được sử dụng để đăng ký dịch vụ tiết kiệm tự động.|Source account already exists",
                                "13|Tài khoản nguồn không hợp lệ|Source account is invalid"
        };

        #endregion

        #endregion "REPLY MESSAGE TEMPLATE"

        #region "TokenOPT Response string"
        public static String RESPONE_GET_ACTIVATION_CODE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "ACTIVATION_CODE" + COL_DLMT + "{ACTIVATION_CODE}"
           + ROW_DLMT + "AUTH_METHOD" + COL_DLMT + "{AUTH_METHOD}";
        public static String RESPONE_CREATE_TRANSACTION = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "USERID" + COL_DLMT + "{USERID}"
           + ROW_DLMT + "TRANSACTIONID" + COL_DLMT + "{TRANSACTIONID}"
           + ROW_DLMT + "CHALLENGE" + COL_DLMT + "{CHALLENGE}"
            ;
        public static String GET_OTP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "REQUEST_ID" + COL_DLMT + "{REQUEST_ID}"
            + ROW_DLMT + "TYPE_OTP" + COL_DLMT + "{TYPE_OTP}"
            + ROW_DLMT + "CHALLENGE_CODE" + COL_DLMT + "{CHALLENGE_CODE}";
        public const String gFormatSmsOtp = "Ma xac thuc OTP cua giao dich {0} LA {1}. Quy khach hay nhap vao ung dung MOBILE BANKING de hoan tat giao dich. Hotline ho tro *6688.";

        public static String ERR_NOT_REGIS_MTOKEN = "ERR_CODE" + COL_DLMT + "98" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "SO CIF CHUA DANG KI GOI MTOKEN";
        public static String ERR_LOGIN_MTOKEN = "ERR_CODE" + COL_DLMT + "97" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "MAT KHAU KHONG DUNG";
        public static String RESPONSE_MSG_TOKEN_POPUP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CONTENT_VN" + COL_DLMT + "{CONTENT_VN}"
            + ROW_DLMT + "CONTENT_EN" + COL_DLMT + "{CONTENT_EN}"
            + ROW_DLMT + "ISHOWPOPUP" + COL_DLMT + "{ISHOWPOPUP}"
            + ROW_DLMT + "STATE" + COL_DLMT + "{STATE}";

        #endregion

        #region "Elounge Response string"
        public static String RESPONE_GET_VOUCHER_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF" + COL_DLMT + "{CIF}"
           + ROW_DLMT + "LIST_VOUCHER" + COL_DLMT + "{LIST_VOUCHER}";
        #endregion

        #region "Database connection"

        //******************************************  Database Connection Param
        //******************************************  -------------------------	
        public static String gMOBILConnstr = Funcs.getConn("MOBILDB_SMSUSER");//"User Id=smsuser;Data Source=MOBIL_TEST;Password=ebank;Min Pool Size=0;Max Pool Size=1000; Connection Lifetime=30;Connection Timeout=30;Incr Pool Size=3;Decr Pool Size=1;";
        public static String gEBANKConnstr = Funcs.getConn("EBANKDB_EBANK");// "User Id=ebank;Data Source=ebank_test;Password=EBANK;Min Pool Size=0;Max Pool Size=1000; Connection Lifetime=30;Connection Timeout=30;Incr Pool Size=3;Decr Pool Size=1;Validate Connection=true;";
        public static String gCARDCnnstr = Funcs.getConn("CARD_SVBODB_EBANK");//"User Id=EBANK;Data Source=UAT_SVBO;Password=EBANKSHB;Min Pool Size=0;Max Pool Size=1000; Connection Lifetime=30;Connection Timeout=30;Incr Pool Size=3;Decr Pool Size=1;";
        public static String gINTELLECTConnstr = Funcs.getConn("INTELLECTDB_EBANK");//"User Id=ebank;Data Source=INTELLECT_TEST;Password=ebank;Min Pool Size=1;Max Pool Size=1000; Connection Lifetime=30;Connection Timeout=30;Incr Pool Size=3;Decr Pool Size=1;";

        #endregion "Database connection"


        #region CARD_DPP
        public static String[] RET_CARD_DPP_RES_CODE = new String[]{
                                                                     "00|Giao dịch thành công|Transaction sucessfully",
                                                                    "99|Giao dịch đang chờ xử lý. Vui lòng liên hệ Hotline 1800 58 88 56 để được trợ giúp|The transaction is processing. Please contact Hotline 1800 58 88 56 for assistance.",
                                                                    "20001|Giao dịch đang chờ xử lý (Mã 20001). Vui lòng liên hệ Hotline 1800 58 88 56 để được trợ giúp.|The transaction is processing (Code 20001). Please contact Hotline 1800 58 88 56 for assistance.",// There is an error during convert CMTP136
                                                                    "20000|Giao dịch đang chờ xử lý (Mã 20000). Vui lòng liên hệ Hotline 1800 58 88 56 để được trợ giúp.|The transaction is processing (Code 20000). Please contact Hotline 1800 58 88 56 for assistance.", // There is an error during insert transaction'
                                                                    "20003|Giao dịch đang chờ xử lý (Mã 20003). Vui lòng liên hệ Hotline 1800 58 88 56 để được trợ giúp.|The transaction is processing (Code 20003). Please contact Hotline 1800 58 88 56 for assistance.", //There is an error during insert DPP BTRT35 into CARD_BTRT35_CHECKER
                                                                    "20004|Giao dịch đang chờ xử lý (Mã 20004). Vui lòng liên hệ Hotline 1800 58 88 56 để được trợ giúp.|The transaction is processing (Code 20004). Please contact Hotline 1800 58 88 56 for assistance.", // There is an error during generate file DPP BTRT35
                                                                    "20005|Giao dịch đang chờ xử lý (Mã 20005). Vui lòng liên hệ Hotline 1800 58 88 56 để được trợ giúp.|The transaction is processing (Code 20005). Please contact Hotline 1800 58 88 56 for assistance."  // There is an error duringprocess DPP BTRT35
                                                             };
    public static String[] RET_GET_CARD_INSTALLMENT_PERIOD = new String[]{
                                                                    "00|THANH CONG|SUCCESSFULL",
                                                                    "99|LOI KHONG XAC DINH|FAIL"
                                                             };
        public static String RESPONE_RESGISTER_CARD_DPP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "REF_NO" + COL_DLMT + "{REF_NO}"
           + ROW_DLMT + "REF_DATE" + COL_DLMT + "{REF_DATE}";
        public static String RESPONE_GET_CARD_INSTALLMENT_PERIOD_CARD_DPP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "INSTALLMENT_PERIOD" + COL_DLMT + "{INSTALLMENT_PERIOD}";
        public static String RESPONE_GET_CARD_INSTALLMENT_SCHEDULE_CARD_DPP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "INSTALLMENT_SCHEDULE" + COL_DLMT + "{INSTALLMENT_SCHEDULE}";
        public static String RESPONE_GET_CARD_HIS_CARD_DPP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CARD_HIS" + COL_DLMT + "{CARD_HIS}";
        #endregion

        #region "System param"



        public static string NULL_VALUE = "_NULL_";

        //******************************************  System Param
        //******************************************  -----------------
        public static string SHB_BIN = "970443";
        //anhnd2 PCI_DSS 26.01.2015
        //Them CARDAPPSchema.
        public static String gCARDAPPSchema = "ITCARDAPP.";
        //
        public static String gEBANKSchema = Funcs.getConfigVal("EBANK_RETAIL_SCHEMA"); // "EBANK_RETAIL.";

        public static String gSMSUSERSchema = "SMSUSER.";

        public static string defaultBR = "BR0001";

        public static string gSMS_CENTER = "6089";
        public static string gSMS_VN_PREFIX = "84";

        /*KHONG DUNG CAC THONG TIN NAY
        public static string gSMTP_SERVER_IP = "172.16.0.15";
        public static string gSMTP_SERVER_PORT = "25";
        public static string gSMTP_SERVER_SENDUSING = "2";
        public static string gSMTP_SERVER_AUTHENTICATE = "1";
        public static string gSMTP_SENDUSER = "SHBHO/ibanking";
        public static string gSMTP_SENDPASSWORD = "123456";

        public static string gSHB_FROM_EMAIL = "ibanking@shb.com.vn";
        public static string gSHB_FEEDBACK_EMAIL = "thuy.ntn@shb.com.vn";
        public static string gSHB_FEEDBACK_EMAIL_CC = "minh.ha1@shb.com.vn";
        */

        //------------------- INTERBANK FEE

        public static double interbank_feePercent = 0.01;
        public static double interbank_minFee = 9000;
        public static double interbank_maxFee = 100000;
        public static string securityAcct_PRD = ",135,136,137,";
        public static string runSchema = "EBANK.";
        public const int waitTime = 300;
        public const int LenOfCode = 6;
        public const String SystemStatus_SysVarName = "EBANK_SYS_STS";
        public const String AccessCount_SysVarName = "EBANK_ACCESS_CNT";
        public const String AMOUNT_LIMIT_SYSVAR = "AMOUNT_LIMIT_SYSVAR";
        public const String AMOUNT_LIMIT_SMS_SYSVAR = "AMOUNT_LIMIT_SMS_SYSVAR";


        //han muc chuyen khoan 247 theo 1 giao dich la 50. trieu
        public const double AMOUNT_LIMIT_247_PERTRAN = 50000000;


        public static string gINTEGRATOR_NVP_PATH = Funcs.getConfigVal("INTEGRATOR_NVP_PATH");
        public static string gINTEGRATOR_SERVER_IP = Funcs.getConfigVal("INTEGRATOR_SERVER_IP");
        public static string gINTEGRATOR_SERVER_PORT = Funcs.getConfigVal("INTEGRATOR_SERVER_PORT");
        public static String gLogFile = Funcs.getConfigVal("LOG_FILE");
        public static String gLogFile_IFACE = Funcs.getConfigVal("INTEGRATOR_LOG");

        public const int TX_STATUS_START = 0;
        public const int TX_STATUS_DONE = 1;
        public const int TX_STATUS_FAIL = 2;
        public const int TX_STATUS_WAIT = 3;

        public const int INV_STATUS_START = 0;
        public const int INV_STATUS_DONE = 1;
        public const int INV_STATUS_CANCEL = 2;

        //public const int INV_STATUS_WAIT = 3;

        public const Double feeAmount_SC_VND = 0;
        public const Double feeAmount_DC_VND = 0;
        public const Double feeAmount_SC_OTHER = 0;
        public const Double feeAmount_DC_OTHER = 0;


        public const String txType_PREPAID = "PREPAID";
        public const String txType_POSTPAID = "POSTPAID";
        public const String txType_TOPUP = "TOPUP";
        public const String txType_INTRA = "INTRA";
        public const String txType_SELF = "SELF";
        public const String txType_PAYMENT = "PAYMENT";
        public const String txType_INTER = "INTER";
        public const String txType_SI = "AS";
        public const String txType_SW = "SW";
        public const String txType_BATCH = "BATCH";

        public const String txType_CARDOPEN = "OPEN";
        public const String txType_CARDCLOSE = "CLOSE";


        // DEFAULT kênh mobile banking dùng gói chuyển khoản

        public const int AUTH_MODE_MOB_GOLD = 2;

        //GIFT CONFIG
        public const String GIFT_STATUS_START = "PENDING";
        public const String GIFT_STATUS_DONE = "DONE";
        public const String GIFT_STATUS_FAIL = "FAIL";
        public const String GIFT_STATUS_WAIT = "WAITTING";
        public const String GIFT_STATUS_EXP = "EXPIRED";
        #endregion "System param"

        #region "Partner WS URL"
        public static string URL_WS_VNPAY = Funcs.getConfigVal("URL_WS_VNPAY");
        public static string URL_WS_ONEPAY_TOPUP = Funcs.getConfigVal("URL_WS_ONEPAY_TOPUP");
        public static string URL_WS_NLUONG_TOPUP = Funcs.getConfigVal("URL_WS_NLUONG_TOPUP");

        public static string URL_WS_NAPAS_VAS_TOPUP = Funcs.getConfigVal("URL_WS_NAPAS_VAS_TOPUP");

        // NAPAS VAS TOPUP VA BILL là 2 service khác nhau
        public static string URL_WS_NAPAS_VAS_BILL = Funcs.getConfigVal("URL_WS_NAPAS_VAS_BILL");

        public static string URL_WS_NAPAS_247 = Funcs.getConfigVal("URL_WS_NAPAS_247");

        public static string URL_WS_PAYOO = Funcs.getConfigVal("URL_WS_PAYOO");


        public static string URL_WS_CARD_SHB = Funcs.getConfigVal("URL_WS_CARD_SHB");




        #endregion "Partner WS URL"

        #region "Timeout config"
        //TIMEOUT CORE BANKING

        public const int TIMEOUT_WITH_CORE = 60000;

        public const int TIMEOUT_WITH_VAS = 60000;

        public const int TIMEOUT_WITH_VNPAY = 80000;

        public const int TIMEOUT_WITH_ONEPAY = 60000;

        public const int TIMEOUT_WITH_PAYOO = 120000;

        public const int TIMEOUT_WITH_NLUONG = 60000;

        public const int TIMEOUT_WITH_NAPAS_247 = 60000;

        public const int TIMEOUT_WITH_CARD_SHB = 60000;

        public const int TIMEOUT_WITH_API = 30000;

        #endregion "Timeout config"


        #region "Core banking Config"

        public const int LIMIT_TOTAL_TIDE = 99;

        public const String PROD_CD_TIDE_ONLINE = "500"; //470
        public const String PROD_CD_FLEX_TIDE_ONLINE = "523";
        public const String PROD_CD_TIDE_ONLINE_OLD = "470";

        public static String HO_BR_CODE = "110000";

        public const String prod_cd_CASA = "001";
        public const String prod_cd_TIDE = "002";
        public const String prod_cd_LENDING = "003";

        public static String[] gResult_IBANKING_Arr = new String[]{
                                                                    "E00|Giao dịch thành công, xin cảm ơn quý khách.|Transaction sucessfully.",					//0
																	"E99|Có lỗi trong quá trình giao dịch (đường truyền Internet có thể bị gián đoạn).|General error.",				//1
																	"E01|Mã bảo vệ không chính xác. Quý khách vui lòng kiểm tra lại.|CAPTCHAR is invalid.",				//2
																	"E02|Số tiền giao dịch không hợp lệ (số tiền nhỏ hơn 1 hoặc lớn hơn số dư tài khoản).|Invalid amount.",							//3
																    "E03|Mật khẩu giao dịch không hợp lệ. Quý khách vui lòng kiểm tra lại.|Invalid transaction password.", //4
																	"E04|Mã xác thực eSecure không hợp lệ. Quý khách vui lòng kiểm tra lại.|Invalid SMS code.",				//5
																	"E05|Số lần giao dịch vượt quá giới hạn trong ngày.|Invalid txcount.",//6
																	"E06|Số tiền giao dịch vượt quá giới hạn trong ngày.|Invalid amount.",//7
																    "E07|Tài khoản không hợp lệ (tài khoản không tồn tại,không thuộc quyền sở hữu của quý khách hoặc tài khoản chuyển và nhận trùng nhau).|Invalid account."						//8
																};

        public static String ERR_CODE_TIMEOUT_WHEN_POST_TO_CORE = "-01";

        //****************************************** Intellect Connection Param
        //****************************************** --------------------------

        public static String ChannelID = "MOB";

        public static String ChannelIDVoucher = "EBANK";

        public static String EB_ACTION_DONE = "DONE";

        public static String EB_ACTION_FAILED = "FAILED";

        //add new
        public const string MODE_CORETIME = "INTER";

        public static String InterfaceID = "MOBBNK";

        public static String TRANFER_FEE_ACQ = "FEE_ACQ";
        public static String TRANFER_FEE_BEN = "FEE_BEN";

        public static String TOPUP_PREPAID_CARD = "TOPUP_PREPAID_CARD";

        public static String UNBLOCK_CARD = "UNBLOCK_CARD";

        public static String refFormat = "MOB";
        public static String refFormat_Book = "MOB";
        public static String refFormatSHBFC = "THO";

        public static String[] gResult_INTELLECT_Arr = new String[]{
                                                                    "00000|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction successfull.",
                                                                    "99999|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn).|General error.",
                                                                    "057|Số tài khoản không hợp lệ (tài khoản không tồn tại hoặc không thuộc quyền sở hữu của quý khách).|Invalid account number.",
                                                                    "002|Tài khoản của quý khách đã bị tạm treo.|Override - Account is Blocked.",
                                                                    "03|Tài khoản của quý khách đang ở trạng thái không cho phép ghi có.|Override - Account in No Credit Status.",
                                                                    "04|Tài khoản của quý khách đang ở trạng thái không cho phép ghi nợ.|Override - Account in No Debit Status.",
                                                                    "005|Tài khoản quý khách không đủ số dư để thực hiện giao dịch.|Override - Insufficent Fund.",
                                                                    "08|Ngày hiệu lực là ngày nghỉ. Quý khách vui lòng chọn ngày khác.|Override - Value Date is Holiday.",
                                                                    "11|Ngày hiệu lực không được nhỏ hơn ngày mở tài khoản. Quý khách vui lòng chọn ngày khác.|Override - Value Date is less than Account Open Date.",
                                                                    "12|Tài khoản đang ở trạng thái ngủ.|Override - Inactive Status.",
                                                                    "022|Tài khoản ghi có và tài khoản ghi nợ không thể trùng nhau.|Debit and Credit account no.can not be same.",
                                                                    "024|Số tài khoản không tồn tại. Quý khách vui lòng kiểm tra lại.|Account Number does Not exists.",
                                                                    "030|Số tiền phong không thể nhỏ hơn 1. Quý khách vui lòng thực hiện lại.|Ear Mark amount can not be lesser than 1.",
                                                                    "033|Tài khoản đang ở trạng thái ngủ.|Account is in Inactive Status - Dormant Account.",
                                                                    "RTE20007|Ngày thực hiện phải lớn hơn ngày hoạt động ngân hàng|Start date must greater then business date.",
                                                                    "80|Invalid date.",
                                                                    "06|Error."
                                                                };
        public static String BK = "12345a@";

        #endregion "Core banking Config"

        #region "Partner account"

        public const String ACCT_SUSPEND_VNPAY_TOPUP = "1001010001";
        public const String ACCT_SUSPEND_VNPAY_BILL = "1001010072";
        public const String ACCT_SUSPEND_VNPAY_QR_PAYMENT = "1010541817";
        public const String ACCT_SUSPEND_SHB_QR_PAYMENT = "9231217045";
        public const String ACCT_SUSPEND_NLUONG = "1002416484";

        public const String ACCT_SUSPEND_ONEPAY = "1001404549";


        //PAYOO. LINHTN ADD NEW 21/09/2016
        //PAYOO. LINHTN ADD NEW 05/10/2016
        public const String ACCT_SUSPEND_PAYOO = "1005674346";


        //NAPAS TOPUP BILLING


        public static String ACCT_SUSPEND_VAS = "9230387044";
        public static String ACCT_FEE_VAS = "9230387044";
        public static String ACCT_VAT_VAS = "9230387044";



        //NASPAS CHUYEN TIEN QUA THE
        public static String ACCT_SUSPEND_NAPAS_247_CARD = "9230097046";
        public static String ACCT_FEE_NAPAS_247_CARD = "9231167043";
        public static String ACCT_VAT_NAPAS_247_CARD = "9231167043";

        //NASPAS CHUYEN TIEN QUA TAI KHOAN
        public static String ACCT_SUSPEND_NAPAS_247ACCT = "9230097046";
        public static String ACCT_FEE_NAPAS_247ACCT = "9231167043";
        public static String ACCT_VAT_NAPAS_247ACCT = "9231167043";


        #endregion "Partner account"

        #region "Partner return code"

        public static String[] RET_CODE_NAPAS_TOPUP = new String[]{
                                                                 "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction sucessfully",
                                                                 "68|Giao dịch đang được xử lý|Transaction is processing",
                                                                 "99|Giao dịch không thành công. Có lỗi trên đường truyền hoặc nhà cung cấp tạm ngưng dịch vụ.|Transaction failed",
                                                                 "01|Không tìm thấy thông tin khách hàng.|Can't not find result.",
                                                                 "08|Mã hóa đơn đã hết hạn|Bill code expire",
                                                                 "13|Mệnh giá không hợp lệ|Invalid price",
                                                                 "20|Mã hoá đơn không tồn tại|Bill code not exist",
                                                                 "21|Mã hóa đơn đã được thanh toán|Bill code have been paid",
                                                                 "40|Số điện thoại chưa được hỗ trợ|Not support this mobile",
                                                                 "05|Giao dịch không thành công|Can not process",
                                                                 "96|Lỗi hệ thống. Xin quý khách vui lòng thực hiện lại.|Error system. Please try again."
                                                             };



        public static String[] RET_CODE_VNPAY = new String[]{
                                                                   "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "08|Giao dịch đang được xử lý|Transaction is processing",
                                                                   "90|System trace trùng lặp|Error system. Please try again.",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|General error",
                                                                   "12|Số điện thoại không hợp lệ.|Invalid mobile number",
                                                                   "22|Số điện thoại không hợp lệ.|Invalid mobile number",
                                                                   "89|Hóa đơn không cho thanh toán một phần|Invalid amount",
                                                                   "50|Mã hóa đơn không hợp lệ|Invalid mobile number",
                                                                   "01|Hệ thống nhà cung cấp đang bảo trì|The system is being maintenance",
                                                                   "05|Hệ thống tạm ngừng phục vụ do lỗi đường truyền giữa VNPAY tới Ngân hàng hoặc VNPAY tới nhà cung cấp|Error system. Please try again.",
                                                                   "07|Dữ liệu mã hóa không hợp lệ (không giải mã được)|Error system. Please try again.",
                                                                   "50|Mã khách hàng không tồn tại|Error system. Please try again.",
                                                                   "80|Không tìm thấy mã đối tác|Error system. Please try again.",
                                                                   "81|Không tìm thấy mã đối tác|Error system. Please try again.",
                                                                   "82|Không tìm thấy nhà cung cấp|Error system. Please try again.",
                                                                   "84|Không tìm thấy dịch vụ|Error system. Please try again.",
                                                                   "96|Hệ thống tạm ngừng phục vụ|Error system. Please try again.",
                                                                   "11|Thuê bao được nạp tiền là thuê bao trả sau|Error system. Please try again.",
                                                                   "13|Mệnh giá tiền không hợp lệ|Invalid Amount",
                                                                   "83|Mã nhà cung cấp không được hỗ trợ|Partner code not support",
                                                                   "85|Dịch vụ không được hỗ trợ|Service not support",
                                                                   "86|Dịch vụ và nhà cung cấp không được hỗ trợ|Partner or service not support",
                                                                   "87|Message sai định dạng|Invalid Message",
                                                                   "88|Mã xử lý không được hỗ trợ|Error system. Please try again.",
                                                                   "91|Message không được hỗ trợ|Not support message",
                                                                   "17|Nhà mạng hoặc số thuê bao không hợp lệ (MNP Mạng Viettel)|Nhà mạng hoặc số thuê bao không hợp lệ (MNP Mạng Viettel)"
                                                               };

        public static String[] RET_CODE_QR_VNPAY = new String[]
        {
            "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
            "08|Giao dịch đang được xử lý|Transaction is processing",
            "88|Giao dịch đang được xử lý|Transaction is processing",
            "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|General error",
            "01|Sai định dạng dữ liệu|Data is not in format",
            "02|Ip address is denied|Ip address is denied",
            "03|Bank code is not exist|Bank code is not exist",
            "04|Mobile is invalid|Mobile is invalid",
            "05|Post data to merchant failed|Post data to merchant failed",
            "06|Pay date is not in format|Pay date is not in format",
            "07|Merchant is not exist|Merchant is not exist",
            "08|Merchant don't response|Merchant don't response",
            "11|Merchant is not active|Merchant is not active",
            "12|False checkSum|False checkSum",
            "13|Transaction already confirmed|Transaction already confirmed",
            "14|Invalid Transaction|Invalid Transaction",
            "15|Transaction not found|Transaction not found",
            "16|Invalid Amount|Invalid Amount",
            "17|Transaction Expired|Transaction Expired",
            "18|Invalid ResponseCode|Invalid ResponseCode",
            "19|Account is locked|Account is locked",
            "20|Mobile is locked|Mobile is locked",
            "21|Terminal is invalid|Terminal is invalid",
            "23|Transaction duplicate|Transaction duplicate",
            "24|Terminal is inactive|Terminal is inactive",
            "25|MessageType is invalid|MessageType is invalid",
            "26|Thiếu hàng trong đơn hàng|Not enough product in order",
            "27|Hết toàn bộ hàng|Out of stock in order",
            "29|Qrcode is not active|Qrcode is not active",
            "71|Merchant or terminal is not approved|Merchant or terminal is not approved",
            "89|Số tiền gạch nợ không hợp lệ|Số tiền gạch nợ không hợp lệ",
            "96|System is maintaining|System is maintaining",
            "76|Ngân hàng không hỗ trợ|This bank is not support"
        };

        public static String[] RET_CODE_ONEPAY = new String[]{
                                                                    "000000|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction sucessfully",
                                                                    "08|Giao dịch đang được xử lý|Transaction is processing",
                                                                    "100007|Có lỗi trên đường truyền hoặc nhà cung cấp tạm ngưng dịch vụ.|Invalid mobile number",
                                                                    "100003|Hóa đơn không cho phép thanh toán một phần hoặc số tiền thanh toán lớn hơn số tiền hóa đơn|Invalid amout",
                                                                    "100990|Loi duong truyen.|Invalid mobile number"
                                                                };

        public static String[] RET_CODE_NLUONG = new String[]{
                                                                    "E00|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction sucessfully.",
                                                                    "08|Giao dịch đang được xử lý|Transaction is processing",
                                                                    "E99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn).|General error.",
                                                                    "E98|Sai mat khau goi WS.|Invalid username and password for WS.",
                                                                    "E01|Số tiền nạp không hợp lệ.|Invalid topup amount",
                                                                    "E02|Email/tài khoản không tồn tại.|Invalid topup email",
                                                                };



        public static String[] RET_CODE_PAYOO = new String[]{

                                                                   "0|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "68|Giao dịch đang được xử lý|Transaction is processing",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|General error",
                                                                   "-1|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn)|General error"
                                                                };
        public static String[] RET_CODE_HABECO = new String[]{

                                                                   "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "68|Giao dịch đang được xử lý|Transaction is processing",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|General error",
                                                                   "-1|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn)|General error"
                                                                };

        public static String[] RET_CODE_SHBFC_BILLING = new String[]{
                                                                 "000|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction sucessfully",
                                                                "99|Giao dịch không thành công. Có lỗi trên đường truyền hoặc nhà cung cấp tạm ngưng dịch vụ.|Transaction failed",
                                                                "001|Giao dịch đang được xử lý|Transaction is processing",
                                                                "003|Giao dịch đang được xử lý|Transaction is processing",
                                                                "004|Giao dịch đang được xử lý|Transaction is processing",
                                                                "006|Giao dịch đang được xử lý|Transaction is processing",
                                                                "100|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101000|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101001|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101500|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101501|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101503|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101504|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101505|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101506|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101507|Giao dịch đang được xử lý|Transaction is processing",
                                                                "101508|Giao dịch đang được xử lý|Transaction is processing",
                                                                "002|Vui lòng nhập số tiền thanh toán >= 12,000 NVD|Please enter an amount greater than or equal 12.000 VND",
                                                                "105|REF No đã tồn tại. Không thể chuyển tiếp được.|REF No already exists. Cannot be forwarded.",
                                                                "109|Lỗi ngoại lệ|Exception error",
                                                                "007|Số tài khoản hoặc hợp đồng không hợp lệ|Invalid account or contract number",
                                                                "008|Hợp đồng đã hủy, không thể thanh toán được|The contract has been canceled, cannot be paid",
                                                                "009|Hợp đồng đã tất toán không thể thanh toán được|Settled contract cannot be paid",
                                                                "103|Hợp đồng không hợp lệ|Contract is not valid"
                                                             };

        public static String[] RET_CODE_QUAWACO = new String[]{
                                                                   "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "68|Giao dịch đang được xử lý...|Transaction is processing...",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|There is an error"
                                                                };
        public static String[] RET_CODE_SOWACO = new String[]{
                                                                   "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "68|Giao dịch đang được xử lý...|Transaction is processing...",
                                                                   "2|Giao dịch không thành công. Có lỗi trên đường truyền hoặc nhà cung cấp tạm ngưng dịch vụ.|Transaction failed",
                                                                   "3|Giao dịch không thành công. Có lỗi trên đường truyền hoặc nhà cung cấp tạm ngưng dịch vụ.|Transaction failed",
                                                                   "4|Không tìm thấy hóa đơn thanh toán|Không tìm thấy hóa đơn thanh toán",
                                                                   "5|Hóa đơn này đã được thanh toán|Hóa đơn này đã được thanh toán",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|There is an error"
                                                                };

        public static String[] RET_CODE_DAWACO = new String[]{
                                                                   "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|There is an error",
                                                                   "01|AgentCode không hợp lệ|There is an error",
                                                                    "02|Mật khẩu không chính xác|There is an error",
                                                                    "03|IP không hợp lệ|There is an error",
                                                                    "04|Kết nối không thành công|There is an error",
                                                                    "05|Hình thức thanh toán không hợp lệ (TM-0 hay CK-1)|There is an error",
                                                                    "06|Ngày thanh toán không hợp lệ|There is an error",
                                                                    "07|Lỗi chưa xác định|There is an error",
                                                                    "08|Khách hàng không tồn tại|There is an error",
                                                                    "09|Mã khách hàng không hợp lệ (9 ký tự)|There is an error",
                                                                    "10|Khách hàng hiện tại có nợ|There is an error",
                                                                    "11|Khách hàng không có thanh toán để hủy|There is an error",
                                                                    "12|Dữ liệu đầu vào không hợp lệ|There is an error",
                                                                    "13|Mã giao dịch không hợp lệ (14 ký tự)|There is an error",
                                                                    "14|Hình thức thanh toán Dawaco và Đơn vị thu hộ không khớp|There is an error",
                                                                    "15|Ngày hủy thanh toán và thanh toán không cùng một ngày|There is an error",
                                                                    "16|Độ dài chuỗi không đúng theo qui định|There is an error",
                                                                    "18|Trùng mã giao dịch (TRANS_ID) hoặc thanh toán trùng lặp|There is an error",
                                                                    "19|Định danh không chính xác|There is an error",
                                                                    "20|Tổng số tiền (nợ/hủy nợ) không khớp|There is an error",
                                                                    "22|Đang bảo trì hệ thống|There is an error",
                                                                    "27|Hóa đơn này chưa bàn giao|There is an error",
                                                                    "28|Từ chối truy cập dịch vụ|There is an error",
                                                                    "29|Từ chối hủy thanh toán do tiền nợ tại Dawaco đã thay đổi.|There is an error",
                                                                    "30|Hóa đơn không tồn tại|There is an error",
                                                                    "31|Request query timeout|There is an error",
                                                                    "32|Khách hàng đã đăng ký dịch vụ trích nợ tự động|There is an error",
                                                                    "33|Khách hàng này chưa đăng ký dịch vụ trích nợ tự động|There is an error",
                                                                    "34|Đơn vị đã thanh toán thành công|There is an error",
                                                                    "35|Không tồn tại thanh toán hoặc đơn vị khác đã thanh toán|There is an error"
                                                                };
        public static String[] RET_CODE_EVNPC = new String[]{
                                                                   "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|There is an error",
                                                                    "01|Số tiền không đúng | The balance is invalid"
                                                                };
        public static String[] RET_CODE_EVNMN = new String[]{
                                                                   "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách|Transaction sucessfully",
                                                                   "99|Có lỗi trong quá trình giao dich (có thể đường truyền Internet bị gián đoạn)|There is an error",
                                                                   "01|Số tiền không đúng | The balance is invalid",
                                                                   "02|Yêu cầu thanh toán đúng thứ tự Hóa đơn từ trên xuống | Payment suggestions in the correct order Invoices from top to bottom"
                                                                };
        #endregion "Partner return code"
        #region  "Config Partner ID"

        public static String PARTNER_ID_NLUONG = "NLUONG";
        public static String PARTNER_ID_VNPAY = "VNPAY";
        public static String PARTNER_ID_VAS = "VAS"; //SML TOUP BILLING
        public static String PARTNER_ID_ONEPAY = "ONEPAY"; //ONEPAY POSTPAID

        public static String PARTNER_ID_PAYOO = "PAYOO"; //ONEPAY POSTPAID
        public static String PARTNER_ID_EVNHN = "EVNHN"; //ONEPAY POSTPAID
        public static String PARTNER_ID_ELECTRICITY = "ELECTRICITY"; //ELECTRICITY
        public static String PARTNER_ID_DAIICHI = "DAI_ICHI"; //DAI_ICHI
        public static String PARTNER_ID_HABECO = "HABECO"; //HABECO
        public static String PARTNER_ID_SHBFC = "SHBFC"; //SHBFC
        public static String PARTNER_ID_QUAWACO = "QUAWACO";
        public static String PARTNER_ID_SOWACO = "SLAWACO";
        public static String PARTNER_ID_DAWACO = "DAWACO";
        public static String PARTNER_ID_EVNNPC = "EVNNPC";
        public static String PARTNER_ID_EVNMN = "EVNSPC";
        public static String PARTNER_ID_HABECOBANK = "HABECOBANK";

        #endregion "Config Parner ID"

        #region "Config for VNPAY"
        public static string gGateway_VNPAY_URL = Funcs.getConfigVal("URL_WS_VNPAY");
        public static String[] gPrice_Arr = new String[] { "10000", "20000", "30000", "50000", "100000", "200000", "300000", "500000" };//0
        public static String[] gPrice_Arr_SML = new String[] { "30,000", "50,000", "100,000", "200,000", "300,000", "500,000" };//0
        public static String gVNPAY_ARR_GETPRICE = "000003|000000000000|[LOCAL_DATE]|000000|[SEND_TIME]|[SEND_DATE]|6014|970443|     SHB|[PROVIDER_CODE]|          ";

        public static String gVNPAY_ARR_0220_000000 = "000000|[AMOUNT]|[LOCAL_DATE]|[TRACE_ID]|[SEND_TIME]|[SEND_DATE]|6014|970443|     SHB|[EXT_DATA]|[ACCTNO]";

        public static String gVNPAY_ARR_0220_000001 = "000001|[AMOUNT]|[LOCAL_DATE]|[TRACE_ID]|[SEND_TIME]|[SEND_DATE]|6014|970443|     SHB|[EXT_DATA]|0000000000";

        public static String gVNPAY_ARR_0220_000002 = "000002|000000000000|[LOCAL_DATE]|000000|[SEND_TIME]|[SEND_DATE]|6014|970443|     SHB|[EXT_DATA]|0000000000";

        public static String gVNPAY_BITMAP_0220_000003 = "3,4,7,11,12,13,18,32,41,48,102";
        public static String gVNPAY_BITMAP_0220_000000 = "3,4,7,11,12,13,18,32,41,48,102";
        public static String gVNPAY_BITMAP_0220_000001 = "3,4,7,11,12,13,18,32,41,48,102";
        public static String gVNPAY_BITMAP_0220_000002 = "3,4,7,11,12,13,18,32,41,48,102";
        public static String gVNPAY_TRAN_CAT_0220 = "0220";
        public static String gVNPAY_TRAN_TYPE_000003 = "000003";
        public static String gVNPAY_TRAN_TYPE_000000 = "000000";
        public static String gVNPAY_TRAN_TYPE_000001 = "000001";
        public static String gVNPAY_TRAN_TYPE_000002 = "000002";

        public static String gSHB_CHANNEL_NET = "6014";
        public static String gSHB_BIN = "970443";

        public static String gVNPAY_SHB_USER = "SHB";
        public static String gVNPAY_SHB_PASSWORD = "SHB";




        #endregion "Config for VNPAY"

        #region "Config for ONEPAY"
        // TEST ONEPAY		
        public static String payment_ONEPAY_Partner = "ONEPAY";




        // THONG TIN TAI KHOAN ONEPAY
        // CIF_NO: 0100635883
        // ACCT: 1001404549

        public static string BANK_KEY = "A2C75E5C3A4C0E23D01871A7DB5B7C8F"; // BANK_KEY: ONEPAY - SHB

        #endregion "Config for ONEPAY"


        #region "Config for NAPAS VAS TOPUP BILLING"

        public static Double SMLBILL_FEE_AMT = 0;
        public static Double SMLBILL_VAT_AMT = 0;



        #endregion "Config for NAPAS VAS TOPUP BILLING"

        #region "Config for NLUONG"

        public static String payment_NLUONG_gwUsername = "shb@shb";
        public static String payment_NLUONG_gwPassword = "shb@shb";


        #endregion "Config for NLUONG"

        public static string gGateway_SLMGW_URL = Funcs.getConfigVal("URL_WS_NAPAS_247");
        #region "Config for NAPAS 247 TO CARD"

        public static Double SML_FEE_AMT = 9000;
        public static Double SML_VAT_AMT = 900;


        public static Double SML_LIMIT_PER_TRAN = 50000000;


        public static String[] gResult_SML_Arr = new String[]{
                                                                "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction sucessfully",
                                                                "99|Giao dịch không thành công. Có lỗi trên đường truyền hoặc nhà cung cấp tạm ngưng dịch vụ.|Transaction failed",
                                                                "68|Giao dịch đã được gửi đến Ngân hàng thụ hưởng. Trường hợp người nhận chưa nhận được tiền, đề nghị quý khách không thực hiện lại giao dịch và vui lòng liên hệ với Ngân hàng để kiểm tra.|Transaction failed",
                                                                "01|Chủ thẻ nên liên hệ với Ngân hàng phát hành thẻ|Refer to card issuer",
                                                                "02|Từ chối (decline)|Refer to issuing bank",
                                                                "03|Từ chối (decline)|Refer to issuing bank",
                                                                "04|Thẻ của bạn đã bị thu hồi|Pick'-up",
                                                                "05|Từ chối (decline)|Refer to issuing bank",
                                                                "06|Từ chối (decline)|Invalid transaction terminal",
                                                                "07|Từ chối (decline)|Refer to issuing bank",
                                                                "08|Từ chối (decline)|Issuer Time out",
                                                                "09|Từ chối (decline)|No Original",
                                                                "10|Từ chối (decline)|Unable to Reverse",
                                                                "11|Từ chối (decline)|Refer to issuing bank",
                                                                "12|Từ chối (decline)|Refer to issuing bank",
                                                                "13|Từ chối (decline)|Refer to issuing bank",
                                                                "14|Từ chối (decline)|Refer to issuing bank",
                                                                "15|Không tìm thấy ngân hàng phát hành|No such Issuer",
                                                                "16|Từ chối (decline)|Refer to issuing bank",
                                                                "17|Từ chối (decline)|Invalid capture date",
                                                                "18|Từ chối (decline)|Refer to issuing bank",
                                                                "19|Từ chối (decline)|System error re'-enter",
                                                                "20|Từ chối (decline)|No From Account",
                                                                "21|Từ chối (decline)|No To Account",
                                                                "22|Từ chối (decline)|No Checking Account",
                                                                "23|Từ chối (decline)|No Saving Account",
                                                                "24|Từ chối (decline)|No Credit Account",
                                                                "25|Từ chối (decline)|Refer to issuing bank",
                                                                "26|Từ chối (decline)|Refer to issuing bank",
                                                                "27|Từ chối (decline)|Refer to issuing bank",
                                                                "28|Từ chối (decline)|Refer to issuing bank",
                                                                "29|Từ chối (decline)|Refer to issuing bank",
                                                                "30|Từ chối (decline)|Refer to issuing bank",
                                                                "31|Từ chối (decline)|Refer to issuing bank",
                                                                "32|Từ chối (decline)|Refer to issuing bank",
                                                                "33|Từ chối (decline)|Refer to issuing bank",
                                                                "34|Thẻ giả mạo, thu hồi thẻ|Suspected Fraud",
                                                                "35|Từ chối (decline)|Refer to issuing bank",
                                                                "36|Từ chối (decline)|Refer to issuing bank",
                                                                "37|Từ chối (decline)|Refer to issuing bank",
                                                                "38|Từ chối (decline)|Refer to issuing bank",
                                                                "39|Từ chối (decline)|Refer to issuing bank",
                                                                "40|Từ chối (decline)|Refer to issuing bank",
                                                                "41|Thẻ đã được khách hàng thông báo bị mất, thu hồi thẻ.|Lost Card",
                                                                "42|Từ chối (decline)|Special Pickup",
                                                                "43|Thẻ đã được khách hàng báo là bị mất cắp, thu hồi thẻ.|Stolen Card",
                                                                "44|Từ chối (decline)|Refer to issuing bank",
                                                                "45|Từ chối (decline)|Refer to issuing bank",
                                                                "46|Từ chối (decline)|Refer to issuing bank",
                                                                "47|Từ chối (decline)|Refer to issuing bank",
                                                                "48|Từ chối (decline)|Refer to issuing bank",
                                                                "49|Từ chối (decline)|Refer to issuing bank",
                                                                "50|Từ chối (decline)|Refer to issuing bank",
                                                                "51|Số dư tài khoản không đủ|Insufficient Balance",
                                                                "52|Từ chối (decline)|Refer to issuing bank",
                                                                "53|Từ chối (decline)|Refer to issuing bank",
                                                                "54|Từ chối (decline)|Refer to issuing bank",
                                                                "55|Sai PIN/OTP|Incorrect PIN",
                                                                "56|Từ chối (decline)|Refer to issuing bank",
                                                                "57|Thẻ không thể thực hiện giao dịch này.|Transaction not permitted to cardholder",
                                                                "58|Ngân hàng phát hành không cho phép thẻ này được thực hiện tại thiết bị hiện tại|Transaction not permitted to terminal",
                                                                "59|Từ chối (decline)|Refer to issuing bank",
                                                                "60|Từ chối (decline)|Refer to issuing bank",
                                                                "61|Từ chối (decline)|Refer to issuing bank",
                                                                "62|Thẻ đang giao dịch tại khu vực không được cho phép|Restricted Card",
                                                                "63|Từ chối (decline)|Refer to issuing bank",
                                                                "64|Từ chối (decline)|Refer to issuing bank",
                                                                "65|Từ chối (decline)|Refer to issuing bank",
                                                                "66|Từ chối (decline)|Exceeds Acquirer Limit",
                                                                "67|Từ chối (decline)|Refer to issuing bank",
                                                                "69|Từ chối (decline)|Refer to issuing bank",
                                                                "70|Từ chối (decline)|Refer to issuing bank",
                                                                "71|Từ chối (decline)|Refer to issuing bank",
                                                                "72|Từ chối (decline)|Refer to issuing bank",
                                                                "73|Từ chối (decline)|Refer to issuing bank",
                                                                "74|Từ chối (decline)|Refer to issuing bank",
                                                                "75|Vượt quá số lần cho phép nhập sai PIN/OTP|Allowable number of PIN tries exceeded",
                                                                "76|Từ chối (decline)|Refer to issuing bank",
                                                                "77|Từ chối (decline)|Refer to issuing bank",
                                                                "78|Từ chối (decline)|Refer to issuing bank",
                                                                "79|Từ chối (decline)|Key validation Error",
                                                                "80|Từ chối (decline)|Pin Length Error",
                                                                "81|Từ chối (decline)|Invalid Pin Block",
                                                                "82|Từ chối (decline)|Invalid CVV",
                                                                "83|Từ chối (decline)|Counter Sync Error",
                                                                "84|Xác thực giá trị ARQC lỗi|ARQC validation error",
                                                                "85|NHPH từ chối No CVM, yêu cầu nhập PIN|No CVM Threshold exceeded, enter PIN",
                                                                "86|Từ chối (decline)|Refer to issuing bank",
                                                                "87|Từ chối (decline)|PIN Key Error",
                                                                "88|Từ chối (decline)|MAC Sync Error",
                                                                "89|Từ chối (decline)|Refer to issuing bank",
                                                                "90|Từ chối (decline)|Refer to issuing bank",
                                                                "91|Từ chối (decline)|Refer to issuing bank",
                                                                "92|Từ chối (decline)|Refer to issuing bank",
                                                                "93|Từ chối (decline)|Invalid Acquirer",
                                                                "94|Từ chối (decline)|Refer to issuing bank",
                                                                "95|Từ chối (decline)|Refer to issuing bank",
                                                                "96|Từ chối (decline)|Refer to issuing bank",
                                                                "97|Từ chối (decline)|Refer to issuing bank",
                                                                "98|Từ chối (decline)|Duplicate Reversal"
                                                             };
        #endregion "Config for NAPAS 247 TO CARD"


        #region ConfigDAIICHI_Resp
        public static String[] gResult_DaiiCHI_Arr = new String[]{
                                                                 "DLVN0|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction sucessfully",
                                                                "99|Không tìm thấy thông tin hóa đơn hoặc hoá đơn đã được thanh toán|Cannot find bill information",
                                                                "DLVN1|Kết nối đến DLVN không thành công|Kết nối đến DLVN không thành công",
                                                                "DLVN2|Kết nối đến DLVN không thành công|Kết nối đến DLVN không thành công",
                                                                "DLVN3|Kết nối đến DLVN không thành công|Kết nối đến DLVN không thành công",
                                                                "DLVN4|Tài khoản không hợp lệ.|Tài khoản không hợp lệ.",
                                                                "DLVN5|Số HĐ không được để trống.|Số HĐ không được để trống.",
                                                                "DLVN6|Mã số khách hàng không được để trống.|Mã số khách hàng không được để trống.",
                                                                "DLVN8|Không tìm thấy số hợp đồng này.|Không tìm thấy số hợp đồng này.",
                                                                "DLVN11|Lỗi hệ thống.|Lỗi hệ thống.",
                                                                "DLVN12|Không có thông tin về phí của hợp đồng này.|Không có thông tin về phí của hợp đồng này.",
                                                                "DLVN13|Vui lòng cung cấp thông tin về tài khoản đăng nhập khi thực hiện chức năng thanh toán.|Vui lòng cung cấp thông tin về tài khoản đăng nhập khi thực hiện chức năng thanh toán.",
                                                                "DLVN14|Vui lòng cung cấp thông tin về mật khẩu đăng nhập khi thực hiện chức năng thanh toán.|Vui lòng cung cấp thông tin về mật khẩu đăng nhập khi thực hiện chức năng thanh toán.",
                                                                "DLVN15|Vui lòng cung cấp thông tin về số DLVN_Ref khi thực hiện chức năng thanh toán.|Vui lòng cung cấp thông tin về số DLVN_Ref khi thực hiện chức năng thanh toán.",
                                                                "DLVN16|Vui lòng cung cấp thông tin về số HĐ khi thực hiện chức năng thanh toán.|Vui lòng cung cấp thông tin về số HĐ khi thực hiện chức năng thanh toán.",
                                                                "DLVN17|Vui lòng cung cấp thông tin về số tiền phí đóng vào khi thực hiện chức năng thanh toán.|Vui lòng cung cấp thông tin về số tiền phí đóng vào khi thực hiện chức năng thanh toán.",
                                                                "DLVN18|Amount can not be equal zero in payment function.|Amount can not be equal zero in payment function.",
                                                                "DLVN19|Vui lòng cung cấp thông tin về ngày thực hiện thanh toán phí khi thực hiện chức năng thanh toán|Vui lòng cung cấp thông tin về ngày thực hiện thanh toán phí khi thực hiện chức năng thanh toán",
                                                                "DLVN20|Vui lòng cung cấp thông tin về định kỳ đóng phí khi thực hiện chức năng thanh toán.|Vui lòng cung cấp thông tin về định kỳ đóng phí khi thực hiện chức năng thanh toán.",
                                                                "DLVN21|Vui lòng cung cấp thông tin về tên chủ hợp đồng khi thực hiện chức năng thanh toán |Vui lòng cung cấp thông tin về tên chủ hợp đồng khi thực hiện chức năng thanh toán",
                                                                "DLVN22|Vui lòng cung cấp thông tin về người đóng phí khi thực hiện chức năng thanh toán|Vui lòng cung cấp thông tin về người đóng phí khi thực hiện chức năng thanh toán",
                                                                "DLVN23|Vui lòng cung cấp thông tin về địa chỉ của người đóng phí khi thực hiện chức năng thanh toán.|Vui lòng cung cấp thông tin về địa chỉ của người đóng phí khi thực hiện chức năng thanh toán.",
                                                                "DLVN24|Vui lòng cung cấp thông tin về Số giao dịch của hệ thống Đối tác.|Vui lòng cung cấp thông tin về Số giao dịch của hệ thống Đối tác.",
                                                                "DLVN26|Không tìm thấy thông tin về Số giao dịch của hệ thống Đối tác.|Không tìm thấy thông tin về Số giao dịch của hệ thống Đối tác.",
                                                                "DLVN31|Hệ thống đang xử lý dữ liệu cuối ngày, Quý khách vui lòng thực hiện lại giao dịch sau thời gian này|The system is processing data at the end of the day, please do the transaction again after this time"
                                                             };
        #endregion

        //18/05/2015 CK nhannh qua tai khoan SML
        #region "Config for NAPAS 247 TO ACCT"


        //anhnd2 thêm. 
        public static string gGateway_SLMGW_ACCT_URL = Funcs.getConfigVal("SMLGWIBTACC.SMLWSGW");

        public static Double SML_ACCT_FEE_AMT = 9000;
        public static Double SML_ACCT_VAT_AMT = 900;
        public static Double SML_ACCT_LIMIT_PER_TRAN = 50000000;


        //CMD#GET_SML_TRANSFER_BANK_LIST|CIF_NO#  
        public static string GET_SML_TRANSFER_BANK_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        //CMD#GET_ACCOUNT_INFO_SMLGW|BANK_CODE#658568|ACCT_DES#23443543434
        public static String GET_ACCOUNT_INFO_SMLGW = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "ACCOUNT_OWNER_NAME" + COL_DLMT + "{ACCOUNT_OWNER_NAME}";

        //CMD#GET_SML_ACCT_BEN|CIF_NO#0310003896
        public static String GET_SML_ACCT_BEN = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";
        //CMD#FUNDTRANSFER_SML_ACCT_FULL|ACTIVE_CODE#ABC|CIF_NO#0310008705|SRC_ACCT#1000013376|BANK_DES#BANK1"|DES_ACCT#1000010000|AMOUNT#1000|TXDESC#CK SML TEST|TRANPWD#fksdfjf385738jsdfjsdf9";
        public static String FUNDTRANSFER_SML_ACCT_FULL = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
        + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}";


        public static String[] gResult_SML_Acct_Arr = new String[]{
                                                                 "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction sucessfully",
                                                                 "99|Giao dịch không thành công. Có lỗi trên đường truyền hoặc nhà cung cấp tạm ngưng dịch vụ.|Transaction failed",
                                                                 "68|Giao dịch đang được xử lý tại ngân hàng thụ hưởng, quý khách vui lòng kiểm tra với người nhận.|Transaction failed",
                                                                 "14|Số tài khoản đích không đúng|Invalid acctno",
                                                                 "41|Tài khoản đích không hợp lệ|Invalid card",
                                                                 "54|Tài khoản đích hết hạn sử dụng|Card expire",
                                                                 "61|Số tiền chuyển khoản vượt quá hạn mức một giao dịch|Violate limit amount",
                                                                 "91|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",																								 
																 //"91|Ngân hàng thụ hưởng không có trong danh sách|Bank ben not in accepted list",												
																 "01|Thông tin tài khoản đích không hợp lệ|Invalid account",
                                                                 "05|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "96|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "12|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "57|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "94|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "75|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "40|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "21|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "27|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "30|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "31|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed",
                                                                 "39|Giao dịch không thành công. Xin quý khách vui lòng thực hiện lại.|Transaction failed"
                                                             };

        #endregion "Config for NAPAS 247 TO ACCT"

        #region CONFIG CASA_DETAIL
        public static String[] RET_TRAN_TYPE_NAME = new String[]{
                                                                    "SELF|Thông tin giao dịch|Transaction Information",
                                                                    "ACQ_247CARD|Chuyển tiền liên ngân hàng|Interbank money transfer",
                                                                    "CREDIT_PAYMENT|Thanh toán dư nợ thẻ tín dụng|Payment Credit Card",
                                                                    "BATCH|Thông tin giao dịch|Transaction Information",
                                                                    "CREDIT_CARD|Thông tin giao dịch|Transaction Information",
                                                                    "ACT_CARD|Thông tin giao dịch|Transaction Information",
                                                                    "BILL_MOBILE_OTHER|Thanh toán|Billing",
                                                                    "TOPUP_MOBILE|Nạp Tiền|Topup",
                                                                    "BILL_MOBILE|Thanh toán|Billing",
                                                                    "TATTOAN_ONLINE|Thông tin giao dịch|Transaction Information",
                                                                    "BEN_247CARD|Chuyển tiền liên ngân hàng|Interbank money transfer",
                                                                    "CashIn|Thông tin giao dịch|Transaction Information",
                                                                    "RESET PASSWORD|Thay đổi mật khẩu|Reset Password",
                                                                    "BILLING|Thanh toán|Billing",
                                                                    "TAICAP_HANMUC|Thông tin giao dịch|Transaction Information",
                                                                    "ONEPAY_ECOM_REFUND|Thông tin giao dịch|Transaction Information",
                                                                    "MOB_REG_MOB|Thông tin giao dịch|Transaction Information",
                                                                    "BILL_OTHER|Thanh toán hóa đơn|Billing",
                                                                    "TOPUP_SHS|Thông tin giao dịch|Transaction Information",
                                                                    "NET_REG_MOB|Thông tin giao dịch|Transaction Information",
                                                                    "PAYMENT_ONLINE|Thanh toán trực tuyến|Payment Online",
                                                                    "TIDEBOOKING|Đặt Phòng|Tide Booking",
                                                                    "TUITION_FEE|Thông tin giao dịch|Transaction Information",
                                                                    "SI_BOOKING|Thông tin giao dịch|Transaction Information",
                                                                    "TOPUP_OTHER|Nạp Tiền|Topup",
                                                                    "ACQ_247AC|Chuyển tiền liên ngân hàng|Interbank money transfer",
                                                                    "TOPUP_SHBS|Nạp Tiền|Topup",
                                                                    "CLOSE_CARD|Đóng thẻ|Close Card",
                                                                    "AS_BOOKING|Thông tin giao dịch|Transaction Information",
                                                                    "QR_PAYMENT|Thông tin giao dịch|Transaction Information",
                                                                    "BEN_247AC|Chuyển tiền liên ngân hàng|Interbank money transfer",
                                                                    "CHARITY|Thông tin giao dịch|Transaction Information",
                                                                    "THAUCHI_ONLINE|Thấu chi online|Overdraft Online",
                                                                    "CREDIT_CARD_OTHER|Thanh toán dư nợ thẻ tín dụng|Payment Credit Card",
                                                                    "SCHOOL_FEE|Thu học phí|School Fee",
                                                                    "INTRA|Chuyển tiền trong SHB|Domestic Tranfer",
                                                                    "DOMESTIC|Chuyển tiền trong SHB|Domestic Tranfer"
                                                             };
        public static String[] RET_FIELD_NAME = new String[]{
                                                                    "DES_ACCT|Tài khoản nhận|Payee Account",
                                                                    "DES_NAME|Tên tài khoản nhận|Payee Account Name",
                                                                    "BANK_NAME|Ngân hàng nhận|BeneficiaryBank",
                                                                    "POS_CD|PGD thực hiện|Post",
                                                                    "SRC_ACCT|Tài khoản chuyển|From Account",
                                                                    "AMOUNT|Số tiền|Amount",
                                                                    "FEE_AMOUNT|Phí|Fee",
                                                                    "TXDESC|Nội dung giao dịch|Remark",
                                                                    "CORE_REF_NO|Mã giao dịch|Transaction Code",
                                                                    "CORE_TXDATE|Ngày giao dịch|Transaction Date",
                                                                    "ORDER_ID|Mã đơn hàng|Bill Code"
                                                             };
        #endregion

        //        Nhận tiền chuyển khoản
        //Nộp tiền tại quầy
        //Nhập lãi tiền gửi thanh toán vào gốc
        //Tất toán tài khoản tiết kiệm 
        //------Đối với giao dịch ghi nợ -------------------------------------
        //Trả lãi tiết kiệm cho tài khoản tiết kiệm
        //Chuyển tiền trong SHB
        //Chuyển tiền liên ngân hàng
        //Thu phí CK LNH CITAD
        //Rút tiền tại quầy
        //Rút tiền tại ATM
        //Thanh toán tại POS/mPOS
        //Chuyển tiền chứng khoán
        //Thanh toán hóa đơn/ Nạp tiền
        //Thanh toán dư nợ thẻ tín dụng
        //Mua hàng online
        //Chuyển tiền từ thiện
        //Thu nợ tự động tài khoản vay
        //Gửi tiết kiệm online

        #region CONFIG_VERIFY_CARD_SET_PIN
        public static string PIN_CARD_TYPE = "ACTIVE_CARD";
        public static String[] gResult_PIN_VERIFY_Arr = new String[]{
                                                                "00|Số thẻ hợp lệ.|Card num valid",
                                                                "99|Lỗi không xác định.|Error Server.",
                                                                "01|Số thẻ không hợp lệ.|Card num invalid",
                                                                "02|Sai quá 3 lần.|Limit verify",
                                                                "03|Thẻ đang ở trạng thái bị khóa.|Thẻ đang ở trạng thái bị khóa.",
                                                                "05|Không dùng cho thẻ chi trả lương.|Không dùng cho thẻ chi trả lương.",
                                                                "06|Giao dịch không thành công. Vui lòng liên hệ Hotline 024 62754332 để được hỗ trợ.|Transaction unsuccessfully. Please contact Hotline 024 62754332 for support.",
                                                                "08|Thẻ đã bị khóa do nhập sai mã PIN 3 lần liên tiếp. Quý khách vui lòng liên hệ Hotline 024 62754332 để được hỗ trợ.|The card is locked due to entering PIN code incorrectly 3 times. Please contact Hotline 024 62754332 for support.",
                                                                "10|Thông tin thẻ không chính xác.|Thông tin thẻ không chính xác."
                                                             };
        public static String[] gResult_SET_PIN_Arr = new String[]{
                                                                "00|Thành công.|Successfull",
                                                                "99|Lỗi không xác định.|Error Server.",
                                                                "10| Lỗi kết nối đến HSM – Encrypt PIN.| Lỗi kết nối đến HSM – Encrypt PIN",
                                                                "11|Không tìm thấy thông tin khách hàng.|Không tìm thấy thông tin khách hàng.",
                                                                "12|Trạng thái hiện tại không cho phép cập nhật PIN.|Trạng thái hiện tại không cho phép cập nhật PIN."
                                                             };
        public static String ERR_MSG_GENERAL_VERIFY_PIN = "ERR_CODE" + COL_DLMT + "29" + ROW_DLMT + "ERR_DESC" + COL_DLMT + "Mã PIN không trùng nhau.|PIN does not match.";
        public static string PIN_TRANS_TYPE = "SET_PIN";

        #endregion

        #region CREDIT CARD TRANSFER
        // creadit card transfer        
        public static string key_credit_card_gw = "186a6b3ba32c1168bbd953bc14e4f4c2";
        public static string gGateway_CREDIT_CARD_GW_URL = Funcs.getConfigVal("SHBCardGW.Service");
        //public const string CREADIT_CARD_GL_SUSPEND = "9180297048";
        public const string CREDIT_CARD_GL_SUSPEND = "9230617044";
        public const string txType_CREADIT = "CREDIT";

        public static String CREDIT_CARD_PAYMENT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
               + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
               + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
               + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
                + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
               ;
        public static String GET_CARD_NAME_BY_ACCTNO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
                 + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
                 + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
                  + ROW_DLMT + "CARD_NAME" + COL_DLMT + "{CARD_NAME}"
                 ;

        public static String GET_LIMIT_CREDIT_CARD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "CARD_NO" + COL_DLMT + "{CARD_NO}"
           + ROW_DLMT + "CARD_NO" + COL_DLMT + "{CARD_NO}"
            + ROW_DLMT + "ECOMMERCE_LIMIT" + COL_DLMT + "{ECOMMERCE_LIMIT}"
            + ROW_DLMT + "ECOMMERCE_MAX" + COL_DLMT + "{ECOMMERCE_MAX}"
            + ROW_DLMT + "ECOMMERCE_TYPE" + COL_DLMT + "{ECOMMERCE_TYPE}"
            + ROW_DLMT + "PURCHASE_DAILY_LIMIT" + COL_DLMT + "{PURCHASE_DAILY_LIMIT}"
            + ROW_DLMT + "PURCHASE_DAILY_MAX" + COL_DLMT + "{PURCHASE_DAILY_MAX}"
            + ROW_DLMT + "PURCHASE_DAILY_TYPE" + COL_DLMT + "{PURCHASE_DAILY_TYPE}"
            + ROW_DLMT + "CASH_DAILY_LIMIT" + COL_DLMT + "{CASH_DAILY_LIMIT}"
            + ROW_DLMT + "CASH_DAILY_MAX" + COL_DLMT + "{CASH_DAILY_MAX}"
            + ROW_DLMT + "CASH_DAILY_TYPE" + COL_DLMT + "{CASH_DAILY_TYPE}"
            + ROW_DLMT + "PURCHASE_MONTHLY_LIMIT" + COL_DLMT + "{PURCHASE_MONTHLY_LIMIT}"
            + ROW_DLMT + "PURCHASE_MONTHLY_MAX" + COL_DLMT + "{PURCHASE_MONTHLY_MAX}"
            + ROW_DLMT + "PURCHASE_MONTHLY_TYPE" + COL_DLMT + "{PURCHASE_MONTHLY_TYPE}";




        public static String SET_LIMIT_CREDIT_CARD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
            + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}";


        public static string GET_LIST_CREDIT_CARD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";


        public static string GET_CREDIT_CARD_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CARD_HOLDER_NAME" + COL_DLMT + "{CARD_HOLDER_NAME}"
            + ROW_DLMT + "STATUS" + COL_DLMT + "{STATUS}"
            + ROW_DLMT + "TYPE" + COL_DLMT + "{TYPE}"
            + ROW_DLMT + "MAD" + COL_DLMT + "{MAD}"
            + ROW_DLMT + "AMOUNT" + COL_DLMT + "{Amount}";


        public static string GET_CREDIT_CARD_ACCT_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CARD_HOLDER_NAME" + COL_DLMT + "{CARD_HOLDER_NAME}"
            + ROW_DLMT + "STATUS" + COL_DLMT + "{STATUS}"
            + ROW_DLMT + "TYPE" + COL_DLMT + "{TYPE}"
            + ROW_DLMT + "MAD" + COL_DLMT + "{MAD}"
            + ROW_DLMT + "AMOUNT" + COL_DLMT + "{Amount}";

        public static String HANDLE_CARD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            ;

        #endregion
        #region Rut Goc Linh Hoat
        public static String GET_FLEX_TIDE_RATE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";
        public static String[] gResult_Flex_Tide_Booking_Arr = new String[]{
                                                                    "00000|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction successfull.",
                                                                    "99999|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn).|General error.",
                                                                    "01|Số tài khoản không hợp lệ (tài khoản không tồn tại hoặc không thuộc quyền sở hữu của quý khách).|Invalid account number.",
                                                                    "02|Lỗi xác thực Mac.|Invalid Mac.",
                                                                    "03|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn).|General error.",
                                                                    "04|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn).|General error.",
                                                                    "05|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn).|General error.",
                                                                    "99|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn).|General error.",
                                                                };
        public static String FLEXTIDEBOOKING = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
         + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
         + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
         + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
         + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
         + ROW_DLMT + "RESTRANSID" + COL_DLMT + "{RESTRANSID}"
        + ROW_DLMT + "DEPOSITNOPARENTTIDE" + COL_DLMT + "{DEPOSITNOPARENTTIDE}"
        + ROW_DLMT + "ACCCOUNTNOPARENTTIDE" + COL_DLMT + "{ACCCOUNTNOPARENTTIDE}"
        + ROW_DLMT + "NUMOFPARENTTIDE" + COL_DLMT + "{NUMOFPARENTTIDE}"
        + ROW_DLMT + "NUMOFCHILDTIDESUCCESS" + COL_DLMT + "{NUMOFCHILDTIDESUCCESS}"
        + ROW_DLMT + "VALDATE" + COL_DLMT + "{VALDATE}"
        + ROW_DLMT + "MATDATE" + COL_DLMT + "{MATDATE}"
        + ROW_DLMT + "TENURE" + COL_DLMT + "{TENURE}"
        + ROW_DLMT + "TENUREUNIT" + COL_DLMT + "{TENUREUNIT}"
        + ROW_DLMT + "ORGAMOUNTCHILD" + COL_DLMT + "{ORGAMOUNTCHILD}"
        + ROW_DLMT + "INTERESTAMOUNTCHILD" + COL_DLMT + "{INTERESTAMOUNTCHILD}"
        + ROW_DLMT + "INTERESTAMOUNTPARENT" + COL_DLMT + "{INTERESTAMOUNTPARENT}"
        + ROW_DLMT + "TOTALAMOUNTSUCCESS" + COL_DLMT + "{TOTALAMOUNTSUCCESS}"
         ;
        public static String GET_FLEX_TIDE_DETAIL = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "SAVINGNO" + COL_DLMT + "{SAVINGNO}"
            + ROW_DLMT + "CCYCD" + COL_DLMT + "{CCYCD}"
            + ROW_DLMT + "PRINAMT" + COL_DLMT + "{PRINAMT}"
            + ROW_DLMT + "MATAMT" + COL_DLMT + "{MATAMT}"
            + ROW_DLMT + "INTAMT" + COL_DLMT + "{INTAMT}"
            + ROW_DLMT + "TENURE" + COL_DLMT + "{TENURE}"
            + ROW_DLMT + "UNITTENUREEN" + COL_DLMT + "{UNITTENUREEN}"
            + ROW_DLMT + "UNITTENUREVN" + COL_DLMT + "{UNITTENUREVN}"
            + ROW_DLMT + "INTRATE" + COL_DLMT + "{INTRATE}"
            + ROW_DLMT + "VALDT" + COL_DLMT + "{VALDT}"
            + ROW_DLMT + "MATDT" + COL_DLMT + "{MATDT}"
            + ROW_DLMT + "AUTORENNO" + COL_DLMT + "{AUTORENNO}"
            + ROW_DLMT + "INSTRUCTION" + COL_DLMT + "{INSTRUCTION}"
            + ROW_DLMT + "POSCD" + COL_DLMT + "{POSCD}"
            + ROW_DLMT + "POSDES" + COL_DLMT + "{POSDES}"
            + ROW_DLMT + "UNITTENURE" + COL_DLMT + "{UNITTENURE}"
            + ROW_DLMT + "ISPARENT" + COL_DLMT + "{ISPARENT}"
            + ROW_DLMT + "PARENTDEPOSITNO" + COL_DLMT + "{PARENTDEPOSITNO}"
            + ROW_DLMT + "PARENTACCOUNTNO" + COL_DLMT + "{PARENTACCOUNTNO}"
            + ROW_DLMT + "TOTALINTERESTAMOUNTORG" + COL_DLMT + "{TOTALINTERESTAMOUNTORG}"
            + ROW_DLMT + "TOTALPRINCIPLEAMOUNTREMAIN" + COL_DLMT + "{TOTALPRINCIPLEAMOUNTREMAIN}"
            + ROW_DLMT + "TOTALINTERESTAMOUNTREMAIN" + COL_DLMT + "{TOTALINTERESTAMOUNTREMAIN}"
            + ROW_DLMT + "TOTALMATAMOUNTORG" + COL_DLMT + "{TOTALMATAMOUNTORG}"
            + ROW_DLMT + "TOTALMATAMOUNTREMAIN" + COL_DLMT + "{TOTALMATAMOUNTREMAIN}"
            + ROW_DLMT + "TOTALPRINCIPLEAMOUNTORG" + COL_DLMT + "{TOTALPRINCIPLEAMOUNTORG}"
            + ROW_DLMT + "LISTCHILDDEPOSITS" + COL_DLMT + "{LISTCHILDDEPOSITS}";

        public static String FLEXTIDEWDL = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
                    + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
                    + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
                    + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
                    + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
                    + ROW_DLMT + "PARENTACCTNO" + COL_DLMT + "{PARENTACCTNO}"
                    + ROW_DLMT + "CURRPRINAMT" + COL_DLMT + "{CURRPRINAMT}"
                    + ROW_DLMT + "CURRMATAMT" + COL_DLMT + "{CURRMATAMT}"
                    + ROW_DLMT + "INTAMOUNT" + COL_DLMT + "{INTAMOUNT}"
                    + ROW_DLMT + "TENURE" + COL_DLMT + "{TENURE}"
                    + ROW_DLMT + "UNITTENURE" + COL_DLMT + "{UNITTENURE}"
                    + ROW_DLMT + "UNITTENUREEN" + COL_DLMT + "{UNITTENUREEN}"
                    + ROW_DLMT + "UNITTENUREVN" + COL_DLMT + "{UNITTENUREVN}";
        #endregion

        #region TOPUP PRE-PAID CARD
        public static String[] RES_PRE_PAID_CARD_INFO_DETAIL = new String[] {
                                "00|Successfully|Successfully",
                                "99|Lỗi không xác định.|An unknown error.",
                                "01|Thẻ đang bị khóa hoặc đã hết hạn. Vui lòng chọn thẻ khác hoặc LH Hotline 1900xxxx.|Locked card or expired card. Please select another card or contact Hotline 1900xxxx.",
                                "-1|Số thẻ không đúng. Vui lòng kiểm tra lại.|Incorrect card number. Please check again."
        };

        public static String[] RES_PRE_PAID_TOPUP = new String[] {
            "00|Giao dịch thành công, xin cảm ơn quý khách.|Transaction sucessfully.",
            "68|Giao dịch đang chờ xử lý.|Transaction pending.",
            "99|Có lỗi trong quá trình giao dịch (đường truyền Internet có thể bị gián đoạn).|General error.",
            "01|Thông tin thẻ không chính xác.|Card Num Invalid",
            "02|Vượt hạn mức có thể nạp vào thẻ.|Limit Amount ",
            "03|Invalid Merchant ...|Invalid Merchant ...",
            "04|Hot Card - Advise Cardholder To Contact Office|Hot Card - Advise Cardholder To Contact Office",
            "05|Do not honor transaction|Do not honor transaction",
            "06|Error|Error",
            "08|Approve if Customer has Identification|Approve if Customer has Identification",
            "12|Transaction Type Not Supported By Institution ...|Transaction Type Not Supported By Institution ...",
            "13|Cannot Process Amount|Cannot Process Amount",
            "14|Primary account number contains invalid characters|Primary account number contains invalid characters",
            "15|Issuer Inoperative|Issuer Inoperative",
            "17|CANCEL key was pressed|CANCEL key was pressed",
            "30|Message received was in wrong format|Message received was in wrong format",
            "31|Issuer Inoperative|Issuer Inoperative",
            "32|ATM performed a partial dispense|ATM performed a partial dispense",
            "33|Card Has Expired, Capture it|Card Has Expired, Capture it",
            "34|Fraud is suspected|Fraud is suspected",
            "36|Account Restricted, Capture Card|Account Restricted, Capture Card",
            "37|Call Security - Capture Card|Call Security - Capture Card",
            "38|Invalid PIN - Capture Card ...|Invalid PIN - Capture Card ...",
            "41|Card restrictions are in effect|Card restrictions are in effect",
            "43|This Card Has Been Stolen|This Card Has Been Stolen",
            "51|Insufficient Funds ...|Insufficient Funds ...",
            "54|Card Has Expired|Card Has Expired",
            "55|Invalid PIN ...|Invalid PIN ...",
            "57|NOT PERMITTED|NOT PERMITTED",
            "58|NOT PERMITTED|NOT PERMITTED",
            "61|Withdrawal Limit Reached - Retry|Withdrawal Limit Reached - Retry",
            "62|Card restrictions are in effect|Card restrictions are in effect",
            "63|Call acquirer security|Call acquirer security",
            "64|Cannot Process Amount|Cannot Process Amount",
            "65|Limit on total number of transactions per cycle exceeded|Limit on total number of transactions per cycle exceeded",
            "66|Call Security - Capture Card|Call Security - Capture Card",
            "67|Pick up card, special condition|Pick up card, special condition",
            "75|Excessive PIN failures, do not capture|Excessive PIN failures, do not capture",
            "76|Wrong PIN, Excessive PIN Failures...|Wrong PIN, Excessive PIN Failures...",
            "77|NOT PERMITTED|NOT PERMITTED",
            "80|Unable To Process Transaction - Retry ...|Unable To Process Transaction - Retry ...",
            "81|NOT PERMITTED|NOT PERMITTED",
            "82|Unable To Process Transaction - Retry ...|Unable To Process Transaction - Retry ...",
            "83|Issuer Inoperative|Issuer Inoperative",
            "85|Successful Txn|Successful Txn",
            "86|Unable To Process Transaction - Retry ...|Unable To Process Transaction - Retry ...",
            "88|Error during PIN processing|Error during PIN processing",
            "89|Unable To Process Transaction - Retry ...|Unable To Process Transaction - Retry ...",
            "91|Issuer Inoperative|Issuer Inoperative",
            "92|Issuer Inoperative|Issuer Inoperative",
            "94|Duplicate transaction|Duplicate transaction",
            "95|Error reconciling|Error reconciling",
            "96|Error|Error",
            "-1|Thông tin thẻ không chính xác.|Card Num invalid.",
        };

        public static string[] Message_Unblock_Card = new string[]
            {
                "00|Cập nhật thành công.|Your info changed sucessfully",
                "99|Cập nhật không thành công.|Change info unsucessfully",
                "9999|Có lỗi trên đường truyền|General error",
                "20020|Ngày hiệu lực phải lớn hơn ngày hiện tại.|Valid date should be greater than Current date.",
                "20021|Ngày hiệu lực phải lớn hơn ngày hết hạn|Valid date should be less than Expired date.",
                "20022|Thời gian hiệu lực không được quá 15 ngày|Valid date should be greater than 15 days.",
                "20023|Chưa đăng kí dịch vụ để hủy|No valid service to cancel.",
                "20024|Đã đăng kí dịch vụ tương tự|The same service is active."
            };

        public static String PRE_PAID_CARD_INFO = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "CARD_NUM" + COL_DLMT + "{CARD_NUM}"
            + ROW_DLMT + "CARD_MD5" + COL_DLMT + "{CARD_MD5}"
            + ROW_DLMT + "EMBOSS_NAME" + COL_DLMT + "{EMBOSS_NAME}"
            + ROW_DLMT + "CARD_TYPE_CODE" + COL_DLMT + "{CARD_TYPE_CODE}"
            + ROW_DLMT + "CARD_TYPE_DESC" + COL_DLMT + "{CARD_TYPE_DESC}"
            + ROW_DLMT + "CARD_STATUS" + COL_DLMT + "{CARD_STATUS}"
            + ROW_DLMT + "AVAL_BALANCE" + COL_DLMT + "{AVAL_BALANCE}"
            + ROW_DLMT + "CURRENCY" + COL_DLMT + "{CURRENCY}"
            + ROW_DLMT + "TOPUP_LIMIT_AMOUNT" + COL_DLMT + "{TOPUP_LIMIT_AMOUNT}"
            + ROW_DLMT + "BM1" + COL_DLMT + "{BM1}"
            + ROW_DLMT + "BM2" + COL_DLMT + "{BM2}"
            + ROW_DLMT + "BM3" + COL_DLMT + "{BM3}"
            + ROW_DLMT + "BM4" + COL_DLMT + "{BM4}"
            + ROW_DLMT + "BM5" + COL_DLMT + "{BM5}"
            ;

        public static String PRE_PAID_TOPUP = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
            + ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
            + ROW_DLMT + "TRAN_TYPE" + COL_DLMT + "{TRAN_TYPE}"
            + ROW_DLMT + "CARD_NUM" + COL_DLMT + "{CARD_NUM}"
            + ROW_DLMT + "EMBOSS_NAME" + COL_DLMT + "{EMBOSS_NAME}"
            + ROW_DLMT + "BEN_NAME" + COL_DLMT + "{BEN_NAME}"
            + ROW_DLMT + "CARD_TYPE_CODE" + COL_DLMT + "{CARD_TYPE_CODE}"
            + ROW_DLMT + "CARD_TYPE_DESC" + COL_DLMT + "{CARD_TYPE_DESC}"
            + ROW_DLMT + "AVAL_BALANCE" + COL_DLMT + "{AVAL_BALANCE}"
            + ROW_DLMT + "CURRENCY" + COL_DLMT + "{CURRENCY}"
            + ROW_DLMT + "TXT_DESC" + COL_DLMT + "{TXT_DESC}"
            + ROW_DLMT + "BM1" + COL_DLMT + "{BM1}"
            + ROW_DLMT + "BM2" + COL_DLMT + "{BM2}"
            + ROW_DLMT + "BM3" + COL_DLMT + "{BM3}"
            + ROW_DLMT + "BM4" + COL_DLMT + "{BM4}"
            + ROW_DLMT + "BM5" + COL_DLMT + "{BM5}"
            ;

        public static String PRE_UNBLOCK_CARD_DETAIL = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "CARD_MD5" + COL_DLMT + "{CARD_MD5}"
           + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
           ;

        public static String PRE_UNBLOCK_CARD = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "CORE_REF" + COL_DLMT + "{CORE_REF}"
           + ROW_DLMT + "CORE_DATE" + COL_DLMT + "{CORE_DATE}"
           ;
        #endregion
        #region Dang ky tra gop

        public class AutoDebit
        {
            public const string ACTION_REGISTER = "REGISTER";
            public const string ACTION_UPDATE = "UPDATE";
            public const string ACTION_CANCEL = "CANCEL";
            public const string DEBIT_AMOUT_MAX = "MAXIMUM";
            public const string DEBIT_AMOUT_MIN = "MAXIMUM";
        }

       

        public static String GET_AUTO_DEBIT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "CARD_MD5" + COL_DLMT + "{CARD_MD5}"
            + ROW_DLMT + "DEBIT_ACCT" + COL_DLMT + "{DEBIT_ACCT}"
            + ROW_DLMT + "DEBIT_AMOUNT" + COL_DLMT + "{DEBIT_AMOUNT}"
            + ROW_DLMT + "STATUS" + COL_DLMT + "{STATUS}";
        public static String HANDLE_AUTO_DEBIT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "REF_NO" + COL_DLMT + "{REF_NO}"
            ;
        #endregion

        #region ACCT NICE
        public static String GET_ACCT_NICE_LIST = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "JWTOKEN" + COL_DLMT + "{JWTOKEN}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

        public static String RES_ACCT_NICE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "REF_NO" + COL_DLMT + "{REF_NO}"
            + ROW_DLMT + "CORE_DT" + COL_DLMT + "{CORE_DT}"
            + ROW_DLMT + "ACCOUNT_NO" + COL_DLMT + "{ACCOUNT_NO}";

        public static String[] gResult_Acct_Open_Arr = new String[]{
                                                                    "00|Giao dịch thành công. Xin chân thành cảm ơn quý khách.|Transaction successfull.",
                                                                    "99|Có lỗi trong quá trình giao dịch (có thể đường truyền Internet bị gián đoạn).|There is an error.",
                                                                    "01|Số tài khoản Quý khách chọn không khả dụng hoặc không phù hợp. Vui lòng nhập số tài khoản khác.|This account number is no longer available or not suitable. Please enter another account number",
                                                                    "02|Số tài khoản Quý khách chọn cần được đăng ký trực tiếp tại điểm giao dịch của SHB. Vui lòng tới điểm giao dịch gần nhất để được hỗ trợ.|This requested account number must be registered at SHB Branches. Please contact the neareast SHB Branch for assistance.",
                                                                    "03|Quý khách vui lòng không nhập các số tài khoản bắt đầu bằng các số 0, 1 và 9.|Please do not enter account numbers starting with the numbers 0, 1 and 9",
                                                                    "04|Số dư tài khoản nhỏ hơn tổng số tiền giao dịch (số tiền + phí giao dịch).|Failed due to insufficient funds.",
                                                                    "05|Nếu Quý khách vẫn chưa tìm được số tài khoản mong muốn, vui lòng nhập nhiều ký tự hơn để có kết quả chính xác nhất|If you have not found the requested account number, please enter more characters to get the most accurate results.",
                                                                    "06|Quý khách vui lòng nhập tối thiểu 4 ký tự|Please enter at least 4 numbers"
                                                                };
		#endregion

        #region GIVE GIFT
        public static String GET_GIFT_TYPE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;
        public static String GET_GIFT_TEMPLACE = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "GIFT_TYPE" + COL_DLMT + "{GIFT_TYPE}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;
        public static String GET_HISTORY_GIFT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
            ;
        public static String GET_GIFT_DETAIL = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "CUSTOMER_NAME" + COL_DLMT + "{CUSTOMER_NAME}"
            + ROW_DLMT + "SRC_ACCT" + COL_DLMT + "{SRC_ACCT}"
            + ROW_DLMT + "AMOUNT" + COL_DLMT + "{AMOUNT}"
            + ROW_DLMT + "MESSAGE" + COL_DLMT + "{MESSAGE}"
            + ROW_DLMT + "REF_NO" + COL_DLMT + "{REF_NO}"
            + ROW_DLMT + "CORE_DT" + COL_DLMT + "{CORE_DT}"
            + ROW_DLMT + "STATUS" + COL_DLMT + "{STATUS}"
            + ROW_DLMT + "CORE_DT_OPEN" + COL_DLMT + "{CORE_DT_OPEN}"
            + ROW_DLMT + "URL" + COL_DLMT + "{URL}"
            + ROW_DLMT + "POSITION" + COL_DLMT + "{POSITION}"
            + ROW_DLMT + "COLOR" + COL_DLMT + "{COLOR}"
            ;
        public static String RES_GIVE_GIFT = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "REF_NO" + COL_DLMT + "{REF_NO}"
            + ROW_DLMT + "CORE_DT" + COL_DLMT + "{CORE_DT}"
            ;
        #endregion

        public const String Delimiter = "|";
        public const String cbNoData_text = "--Không có--";
        public const String en_cbNoData_text = "--No data--";
        public static String gService = "SHB MOBILEBANKING\r\n";
        public static String gSMSCodeMsg = gService + "Ma xac thuc sms cua giao dich so [TRANO] la [SMSCODE]. Quy khach hay dien vao dtdd de hoan tat giao dich trong vong " + waitTime.ToString() + " giay.";
        public static String gSMSCodeMsg_Prepaid = gService + "THE NAP TIEN [PROVIDER] [PRICE]:\r\nMA SO NAP TIEN:[CODE]\r\nNGAY HET HAN:[EXPDATE]\r\nSERIAL:[SERIAL]\r\nXIN CAM ON QUY KHACH!";
        public static String gSMSCodeMsg_Prepaid_Extra = gService + "THE NAP TIEN [PROVIDER] [PRICE]:\r\nMA SO:[CODE]\r\nHET HAN:[EXPDATE]\r\nSERIAL:[SERIAL]\r\nACCOUNT:[ACCOUNT]\r\nPASSWORD:[PASSWORD]\r\nPERIOD:[PERIOD]";


        /// 03Jul2016
        ///anhnd2
        ///template SMS gui active code.
        ///MA KICH HOAT DỊCH VỤ SHB MOBILE LA843324. QUY KHACH HAY NHẠP VAO UNG DUNG DE HOAN TAT. HOTLINE HO TRO 1800588856.
        ///
        //public const String gSMSCodeMsg_Active = "[ACTIVE_CODE] LA MA KICH HOAT CUA QUY KHACH";
        public const String gSMSCodeMsg_Active = "Ma kich hoat dich vu SHB MOBILE la [ACTIVE_CODE]. Quy khach tuyet doi khong cung cap cho bat ky ai. Vui long nhap vao ung dung de hoan tat. Hotline ho tro *6688";
        public const String gSMSCodeMsg_RessetPassword = "Ma kich hoat dich vu SHB MOBILE la [ACTIVE_CODE]. Quy khach hay tuyet doi khong cung cap cho bat ky ai. Vui long nhap vao ung dung de hoan tat. Hotline ho tro *6688";
        public const String gSMSCodeMsg_Active_Token = "Ma kich hoat dich vu SMART OTP la [ACTIVE_CODE]. Quy khach tuyet doi khong cung cap cho bat ky ai.Vui long nhap vao ung dung de hoan tat.Hotline ho tro *6688";

        public const String gSMSCodeMsg_ChangePassWord = "Yeu cau cap lai mat khau dich vu SHB Mobile thanh cong. LH ngay Hotline *6688 neu Quy khach khong thuc hien giao dich nay. Mat khau:[PASSWORD]";



        public class PayooConfig
        {
            public static string ServiceId = "DIEN";
            public static string ProviderId = "EVNHN";
            public static string UserId = "DEMO";
            public static string AgentId = "6577";
        }

        public class EVNMNConfig
        {
            public static string getName(string code)
            {
                string result = "";
                switch (code)
                {
                    case "TD":
                        result = "Tiền điện";
                        break;
                    case "VC":
                        result = "Tiền công suất phản kháng";
                        break;
                    case "DC":
                        result = "Tiền đóng cắt";
                        break;
                    case "CD":
                        result = "Cấp điện";
                        break;
                    case "DD":
                        result = "Di dời thiết bị đo đếm";
                        break;
                    case "TC":
                        result = "Nâng công suất trạm";
                        break;
                }
                return result;
            }
        }

        //tungdt8
        public class ElectricityEvnHN
        {
            public const string MA_DV = "300000";
            public const string BANK_CD = "970424";
            public const string PROVIDER_CD = "970405";
        }

        //chuyenlt1
        //Config CARDAPI 
        public class CardAPIConfig
        {
            public static string MerchantType = "6010";
            public static string TermId = "00999022";
            public static string ServiceCd = "000000";
        }
        //chuyenlt1 config KEY dung Purcharse
        public static string ShareKey = "7235CFDE06BD96B8D4811B655C36594D";
        public  String USERNAMEDAICHI = Funcs.getConfigVal("USERNAME_DAIICHI"); //product  "shb"
        public  String PWDDAICHI = Funcs.getConfigVal("PASSWORD_DAIICHI"); //product "PrmSvrP@ss.SHB@nk.dlvn"
        public  String ACCT_SUSPEND_DAIICHI = Funcs.getConfigVal("ACCT_SUSPEND_DAIICHI"); //product 1010502953
        public const String DESCRIPTION_DAIICHI = "Thanh toán hóa đơn bảo hiểm";
        public Config()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region PUSH NOTIFICATION

        public static String RESPONE_TOTAL_UNREAD_NEWS = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
          + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
          + ROW_DLMT + "TOTAL_RECORD" + COL_DLMT + "{TOTAL_RECORD}";

        public static String RESPONE_GET_LIST_FUNCTION_PUSH = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "STATUS_ALL" + COL_DLMT + "{STATUS_ALL}"
           + ROW_DLMT + "LIST_FUNCTION" + COL_DLMT + "{LIST_FUNCTION}";

        public static String RESPONE_GET_LIST_NEWS = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
          + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
          + ROW_DLMT + "LIST_NEWS" + COL_DLMT + "{LIST_NEWS}";

        public static String RESPONE_GET_DETAIL_NEWS = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
         + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
         + ROW_DLMT + "NEWS_CONTENT" + COL_DLMT + "{NEWS_CONTENT}"
             + ROW_DLMT + "NEWS_INSIDE_LINK" + COL_DLMT + "{NEWS_INSIDE_LINK}"
            ;

        #endregion
        public static String ChangeSM = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "AUTH_INFO_EXT1" + COL_DLMT + "{AUTH_INFO_EXT1}"
           + ROW_DLMT + "AUTH_METHOD" + COL_DLMT + "{AUTH_METHOD}"
           + ROW_DLMT + "AUTH_METHOD_NAME" + COL_DLMT + "{AUTH_METHOD_NAME}"
           + ROW_DLMT + "CUSTNAME" + COL_DLMT + "{CUSTNAME}"
           + ROW_DLMT + "EMAIL" + COL_DLMT + "{EMAIL}"
           + ROW_DLMT + "OLD_AUTH_METHOD" + COL_DLMT + "{OLD_AUTH_METHOD}"
           + ROW_DLMT + "OLD_MOBIL_NO" + COL_DLMT + "{OLD_MOBIL_NO}";


        public static String VAL_LOGIN_NET = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "AUTH_METHOD" + COL_DLMT + "{AUTH_METHOD}"
           + ROW_DLMT + "USERNAME" + COL_DLMT + "{USERNAME}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "CHECK_REG_MOB" + COL_DLMT + "{CHECK_REG_MOB}";

        public static String GET_OTP_REG = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "TRAN_ID" + COL_DLMT + "{TRAN_ID}"
            + ROW_DLMT + "SMS_CODE" + COL_DLMT + "{SMS_CODE}"
            + ROW_DLMT + "MOBILE_NO" + COL_DLMT + "{MOBILE_NO}"
            + ROW_DLMT + "REQUEST_ID" + COL_DLMT + "{REQUEST_ID}"
            + ROW_DLMT + "CHALLENGE_CODE" + COL_DLMT + "{CHALLENGE_CODE}";

        public static String CONFIRM_REG = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "TRAN_ID" + COL_DLMT + "{TRAN_ID}";

        public class TransType
        {
            public const string TRAN_TYPE_SELF = "SELF";
            public const string TRAN_TYPE_INTRA = "INTRA";
            public const string TRAN_TYPE_TOPUP_MOBILE = "TOPUP_MOBILE";
            public const string TRAN_TYPE_TOPUP_OTHER = "TOPUP_OTHER";
            public const string TRAN_TYPE_BILLING = "BILLING";
            public const string TRAN_TYPE_BILLING_BEN = "BILL_OTHER";
            public const string TRAN_TYPE_BILL_ELECTRICITY = "BILL_ELECTRICITY";
            public const string TRAN_TYPE_BILL_MOBILE = "BILL_MOBILE";
            public const string TRAN_TYPE_BILL_MOB_OTHER = "BILL_MOB_OTHER";
            public const string TRAN_TYPE_BILL_MOBILE_OTHER = "BILL_MOBILE_OTHER";
            public const string TRAN_TYPE_ECOM = "ECOM";
            public const string TRAN_TYPE_BATCH = "BATCH";
            public const string TRAN_TYPE_DOMESTIC = "DOMESTIC";
            public const string TRAN_TYPE_BEN_247AC = "BEN_247AC";
            public const string TRAN_TYPE_BEN_247CARD = "BEN_247CARD";
            public const string TRAN_TYPE_ACQ_247AC = "ACQ_247AC";
            public const string TRAN_TYPE_ACQ_247CARD = "ACQ_247CARD";
            public const string TRAN_TYPE_CREDIT_PAYMENT = "CREDIT_PAYMENT";
            public const string TRAN_TYPE_TOPUP_SHS = "TOPUP_SHS";
            public const string TRAN_TYPE_TOPUP_SHBS = "TOPUP_SHBS";
            public const string TRAN_TYPE_TIDEBOOKING = "TIDEBOOKING";
            public const string TRAN_TYPE_TIDEWDL = "TIDEWDL";
            public const string TRAN_TYPE_CHARITY = "CHARITY";
            public const string TRAN_TYPE_MANCITY = "MANCITY";
            public const string TRAN_TYPE_OPEN_CARD_NORM = "OPEN_CARD_NORM";
            public const string TRAN_TYPE_CLOSE_CARD_NORM = "CLOSE_CARD_NORM";
            public const string TRAN_TYPE_SET_PUR_LIMIT = "SET_PUR_LIMIT";
            public const string TRAN_TYPE_SET_PUR_LIMIT_INTER = "SET_PUR_LIMIT_INTER";
            public const string TRAN_TYPE_CLOSE_CARD_INTER = "CLOSE_CARD_INTER";
            public const string TRAN_TYPE_OPEN_CARD_INTER = "OPEN_CARD_INTER";
            public const string TRAN_TYPE_REG_EBANK_ONLINE = "REG_EBANK_ONLINE";
            public const string TRAN_TYPE_TOPUP_SHS_AND_SHBS = "TOPUP_SHS_AND_SHBS";
            public const string TRAN_TYPE_CREDIT_CARD = "CREDIT_CARD";
            public const string TRAN_TYPE_CREDIT_CARD_OTHER = "CREDIT_CARD_OTHER";
            public const string TRAN_TYPE_SET_LIMIT_CREDIT_CARD = "SET_LIMIT_CREDIT_CARD";
            public const string TRAN_TYPE_AS_BOOKING = "SI_BOOKING";
            public const string TRAN_TYPE_PAYMENT_ONLINE = "PAYMENT_ONLINE";
            public const string TRAN_TYPE_THAUCHI_ONLINE = "THAUCHI_ONLINE";
            public const string TRAN_TYPE_TATTOAN_ONLINE = "TATTOAN_ONLINE";
            public const string TRAN_TYPE_TAICAPHANMUC = "TAICAP_HANMUC";



            //setting
            public const string TRANTYPE_SETTING_CUSTOMER_INFO = "NET_CHANGE_USERNAME";
            public const string TRANTYPE_SETTING_CHANGE_PASS = "NET_CHANGE_PWD";
            public const string TRANTYPE_SETTING_SHB_MOB = "NET_REG_MOB";

            //action
            public const string TRANTYPE_ACTION_LOGIN = "LOGIN";
            public const string TRANTYPE_ACTION_CHANGE_PWD = "CHANGE_PWD";

            public const string TRAN_TYPE_CARDDPP = "CARDDPP";
        }

        public class Db
        {
            public class TransType
            {
                public const string INTRA = "INTRA";
                public const string SELF = "SELF";
                public const String ACQ_247AC = "ACQ_247AC"; // chuyen khoan lien ngan hang nhanh qua so tk
                public const String DOMESTIC = "DOMESTIC";  //chuyen lien ngan hang qua citad
                public const String TRAN_TYPE_CITAD = "CITAD";
                public const String ACQ_247CARD = "ACQ_247CARD";
                public const string TRAN_TYPE_TOPUP_SHS = "TOPUP_SHS";
                public const string TRAN_TYPE_TOPUP_SHBS = "TOPUP_SHBS";
                public const string TRAN_TYPE_STOCK = "STOCK";
                public const string TRAN_TYPE_DOMESTIC_FULL = "DOMESTIC_FULL";

                public const string TRAN_TYPE_DOMESTIC_ACC = "DOMESTIC_ACC"; // gop chuyen khoan qua citad và nhanh qua số tài khoản

                public const string TRAN_TYPE_CHARITY = "CHARITY";
            }
        }
    }
}