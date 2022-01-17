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
using System.Threading;
using System.Collections.Generic;
using mobileGW.Service.AppFuncs;
using mobileGW.Service.Framework;
using mobileGW.Service.DataAccess;
using System.Linq;
//using mobileGW.Service.Bussiness;

public partial class Process_new : System.Web.UI.Page
{
    private static log4net.ILog log4Net = log4net.LogManager.GetLogger(typeof(Process_new));
    protected void Page_Load(object sender, EventArgs e)
    {
        log4net.Config.XmlConfigurator.Configure();

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
        {
            string msg = "";
            string mob_user = "";
            string ip = "";
            string user_agent = "";
            string requestBody = "";
            string activeCode = "";
            string acct_no = "";
            string src_acct = "";
            string des_acct = "";
            string amount = "";
            string txdesc = "";
            string pwd = "";
            string save_to_benlist = "0";
            string ccycd = "";
            string type = "";
            string mob_mobil_no;
            string des_name = "";
            string des_nick_name = "";
            string bank_code = "";
            string bank_name = "";
            string bank_branch = "";
            string bank_city = "";
            try
            {
                //Funcs.WriteLog("Begin check request"); ;

                string theRequestString = Page.Request.Form[0].ToString();

                Funcs.WriteLog("Begin check request - Request string with masking:" + Funcs.MaskingString(theRequestString));

                requestBody = Page.Request.Form[0];
                //requestBody = Page.Request.Params.AllKeys[0];
                msg = requestBody;

                //Funcs.WriteLog("msg decode: " + msg);
                msg = msg.Replace("REQ=", "");

                user_agent = Request.Headers["User-Agent"];
                Funcs.WriteLog("User Agent|" + user_agent);

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                Response.Write(Config.ERR_MSG_GENERAL);
                //Response.Write( Funcs.Encrypt2( Config.ERR_MSG_GENERAL, activeCode));
                //Response.Write(Funcs.ReadAllFile(AppDomain.CurrentDomain.BaseDirectory +"/Default.html"));
                return;
            }

            //lay thêm IP
            /*
            linhtn fix session
            */
            try
            {
                ip = Request.UserHostAddress;
            }
            catch (Exception ex)
            {
                ip = string.Empty;
                Funcs.WriteLog(ex.ToString());
            }


            string cmd = "";
            string retStr = "";

            //Funcs.WriteLog("INCOME MESSAGE :" + msg);
            Hashtable hashTbl = Funcs.stringToHashtbl(msg);
            cmd = Funcs.getValFromHashtbl(hashTbl, "CMD");

            // neu cmd = login hoac active thi skip check token
            int cmd_check = Array.IndexOf(Config.CMD_IGNORE_CHECK, cmd);
            if (cmd_check < 0)
            //if (false)
            {

                Funcs.WriteLog("BEGIN CHECK TOKEN");
                string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");
                if (!token.Equals("NOT_FOUND"))
                {
                    //linhtn add new 2017 02 07
                    //masking token: last 16 characters
                    string token_mask = "";
                    try
                    {
                        token_mask = token.Substring(16);
                    }
                    catch
                    {

                    }

                    //CHECK TOKEN HERE and return msg if not valid
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

                    Funcs.WriteLog("CHECK TOKEN CIF_NO:" + mob_user
                        + "|TOKEN:" + token_mask);

                    string check_result = Auth.CHECK_TOKEN(mob_user, token);

                    Funcs.WriteLog("CHECK TOKEN CIF_NO:" + mob_user
                        + "|TOKEN:" + token_mask
                        + "|RESULT:" + check_result);

                    if (check_result != Config.ERR_CODE_DONE)
                    {
                        Funcs.WriteLog("CHECK TOKEN CIF_NO:" + mob_user
                      + "|TOKEN:" + token_mask
                      + "|INVALID TOKEN");

                        cmd = "";
                        retStr = Config.ERR_MSG_INVALID_TOKEN;
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "TOKEN INVALID");

                        retStr = Config.BEGIN_TAG + retStr + Config.END_TAG;

                        Funcs.WriteLog("ORG RESPONSE|" + retStr);
                        //Funcs.WriteLog( "ENCRYPT RESPONSE|" + Funcs.Encrypt2( retStr, activeCode));					   
                        //Response.Write( Funcs.Encrypt2( retStr, activeCode) );
                        Response.Write(retStr);
                        return;
                    }
                }
            }

            //Buoc 2: Tach xau
            switch (cmd)
            {
                case "GET_CUR_VER":  //DONE
                    #region "GET_CUR_VER"
                    Funcs.WriteLog("begin get current version");
                    type = Funcs.getValFromHashtbl(hashTbl, "TYPE");
                    Utils GET_CUR_VER = new Utils();
                    retStr = GET_CUR_VER.GET_CUR_VER(type);
                    Funcs.WriteLog("end get current version");
                    break;
                #endregion "GET_CUR_VER"

                case "CHANGE_PWD":  //DONE
                    #region "CMD CHANGE PWD"
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    string curPass = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "CUR_PWD") + mob_user);
                    string newPass = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "NEW_PWD") + mob_user);

                    retStr = Auth.CHANGE_PWD(mob_user, curPass, newPass, ip, user_agent);

                    break;
                #endregion "CMD CHANGE PWD"

                case "ACTIVE_MOB":  //DONE
                    #region "CMD ACTIVE_MOB"

                    mob_mobil_no = Funcs.getValFromHashtbl(hashTbl, "MOBILE_NO");
                    //string mob_active_code = Funcs.getValFromHashtbl(hashTbl, "MOB_ACTIVE_CODE");
                    pwd = Funcs.getValFromHashtbl(hashTbl, "PWD");
                    retStr = Auth.ACTIVE_MOB(mob_mobil_no, pwd, ip, user_agent);

                    break;
                #endregion "CMD ACTIVE_MOB"

                case "ACTIVE_MOB_CONFIRM":  //DONE
                    #region "CMD ACTIVE_MOB_CONFIRM"
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "MOBILE_NO");
                    string mob_active_code = Funcs.getValFromHashtbl(hashTbl, "MOB_ACTIVE_CODE");

                    retStr = Auth.ACTIVE_MOB_CONFIRM(mob_user, mob_active_code, ip, user_agent);
                    break;
                #endregion "CMD ACTIVE_MOB_CONFIRM"

                case "CHECK_LOGIN": //DONE
                    #region "CMD CHECK LOGIN"
                    //retStr = Config.CHECK_LOGIN;
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "PWD") + mob_user);
                    activeCode = Funcs.getValFromHashtbl(hashTbl, "ACTIVE_CODE");
                    retStr = Auth.CHECK_LOGIN(mob_user, pwd, activeCode, ip, user_agent);
                    break;
                #endregion "CMD CHECK LOGIN"

                case "LOGIN_FP": //DONE
                    #region "CMD LOGIN_FP"
                    //retStr = Config.CHECK_LOGIN;
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "PWD") + mob_user);
                    activeCode = Funcs.getValFromHashtbl(hashTbl, "ACTIVE_CODE");
                    retStr = Auth.LOGIN_FP(hashTbl, ip, user_agent);
                    break;
                #endregion "CMD LOGIN_FP"

                case "LOGOUT": //DONE
                    #region "CMD LOGOUT"
                    //retStr = Config.CHECK_LOGIN;
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    retStr = Auth.LOGOUT(hashTbl, ip, user_agent);
                    break;
                #endregion "CMD LOGOUT"

                case "SET_FINGER_PRINT": //DONE
                    #region "CMD SET_FINGER_PRINT"
                    //retStr = Config.CHECK_LOGIN;
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "PWD") + mob_user);
                    activeCode = Funcs.getValFromHashtbl(hashTbl, "ACTIVE_CODE");
                    retStr = Utils.SET_FP(hashTbl, ip, user_agent);
                    break;
                #endregion "CMD SET_FINGER_PRINT"

                #region "13. ENQUIRY CASA TIDE LENDING"

                //linhtn check 2017 05 04: khong dung ham nay
                case "GET_ACCT_LIST_HOMESCREEN_N":
                    #region "CMD GET_ACCT_LIST_HOMESCREEN_N"
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    retStr = Enquiry.GET_ACCT_LIST_HOMESCREEN_N(mob_user);
                    break;
                #endregion "CMD GET_ACCT_LIST_HOMESCREEN_N"

                case "GET_ACCT_BALANCE_HOMESCREEN_N":
                    #region "CMD GET_ACCT_BALANCE_HOMESCREEN_N"
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCT_NO");
                    retStr = Enquiry.GET_ACCT_BALANCE_HOMESCREEN_N(mob_user, acct_no);
                    break;
                #endregion "CMD GET_ACCT_BALANCE_HOMESCREEN_N"

                case "GET_ACCT_LIST_QRY_N":
                    #region "CMD GET_ACCT_LIST_QRY_N"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    //string acct_type = Funcs.getValFromHashtbl(hashTbl, "TYPE");
                    retStr = Enquiry.GET_ACCT_LIST_QRY_N(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_LIST_QRY_N"

                case "GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N":
                    #region "CMD GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    //acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
                    retStr = Enquiry.GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N"

                case "GET_ACCT_CASA_INFO_N":
                    #region "CMD GET_ACCT_CASA_INFO_N"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    //acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
                    retStr = Enquiry.GET_ACCT_CASA_INFO_N(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_CASA_INFO_N"

                case "GET_ACCT_TIDE_INFO_N":
                    #region "CMD GET_ACCT_TIDE_INFO_N"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    //acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
                    retStr = Enquiry.GET_ACCT_TIDE_INFO_N(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_TIDE_INFO_N"

                case "GET_ACCT_LOAN_INFO_N":
                    #region "CMD GET_ACCT_LOAN_INFO_N"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    //acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
                    retStr = Enquiry.GET_ACCT_LOAN_INFO_N(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_LOAN_INFO_N"

                case "GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N":
                    #region "CMD GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    //acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
                    retStr = Enquiry.GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N"

                case "GET_ACCT_LN_REPAYMENT_SCHEDULE":
                    #region "CMD GET_ACCT_LN_REPAYMENT_SCHEDULE"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    //acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
                    retStr = Enquiry.GET_ACCT_LN_REPAYMENT_SCHEDULE(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_LN_REPAYMENT_SCHEDULE"

                #endregion "13. ENQUIRY CASA TIDE LENDING"

                case "GET_BENLIST_BY_TRAN_TYPE":    //DONE
                    #region "CMD GET_BENLIST_BY_TRAN_TYPE"
                    Beneficiary ben = new Beneficiary();
                    retStr = ben.GET_BENLIST_BY_TRAN_TYPE(hashTbl);
                    break;
                #endregion "CMD GET_ACCT_TIDE_INFO_N"

                case "GET_BENNAME_FROM_CASA_ACCOUNT":   //DONE
                    #region "GET_BENNAME_FROM_CASA_ACCOUNT"
                    des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
                    retStr = Enquiry.GET_BENNAME_FROM_CASA_ACCOUNT(des_acct);
                    break;
                #endregion "GET_BENNAME_FROM_CASA_ACCOUNT"

                case "GET_ACCT_BALANCE_LIST_N":
                    #region "GET_ACCT_BALANCE_LIST_N"
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCT_NO");
                    ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD");
                    string overdraft = Funcs.getValFromHashtbl(hashTbl, "OVERDRAFT");
                    retStr = Enquiry.GET_ACCT_BALANCE_LIST_N(mob_user, acct_no, ccycd, overdraft);
                    break;
                #endregion "GET_ACCT_BALANCE_LIST_N"

                case "GET_ACCT_LIST_RECEIVE":
                    #region "GET_ACCT_LIST_RECEIVE"
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    acct_no = Funcs.getValFromHashtbl(hashTbl, "ACCT_NO");
                    ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD");
                    retStr = Enquiry.GET_ACCT_LIST_RECEIVE(mob_user, acct_no, ccycd);
                    break;
                #endregion "GET_ACCT_LIST_RECEIVE"

                case "FUNDTRANSFER_INTRA":      //DONE
                    #region "CMD FUNDTRANSFER_INTRA"
                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
                    des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
                    des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME");
                    des_nick_name = Funcs.getValFromHashtbl(hashTbl, "DES_NICK_NAME");

                    des_name = (des_name == Config.NULL_VALUE ? "" : des_name);
                    des_nick_name = (des_nick_name == Config.NULL_VALUE ? "" : des_nick_name);

                    txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");
                    type = Funcs.getValFromHashtbl(hashTbl, "TYPE");
                    amount = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");
                    pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
                    save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");
                    retStr = Financial_Transfer.FUNDTRANSFER_INTRA(
                        mob_user
                        , src_acct
                        , pwd
                        , des_acct
                        , des_name
                        , des_nick_name
                        , type
                        , txdesc
                        , double.Parse(amount)
                        , save_to_benlist
                        , ip
                        , user_agent
                        , hashTbl
                        );
                    break;
                #endregion "CMD FUNDTRANSFER_INTRA"

                case "FUND_TRANSFER_SELF":       //DONE
                    #region "CMD FUND_TRANSFER_SELF"
                    retStr = Financial_Transfer.FUND_TRANSFER_SELF(hashTbl, ip, user_agent);
                    break;
                #endregion "CMD FUND_TRANSFER_SELF"

                case "GET_CHARITY_LIST":    //DONE
                    #region "CMD GET_CHARITY_LIST"
                    Utils GET_CHARITY_LIST = new Utils();
                    retStr = GET_CHARITY_LIST.GET_CHARITY_LIST(hashTbl);
                    break;
                #endregion "CMD GET_CHARITY_LIST "

                case "CHARITY_TRANSFER":    //DONE
                    #region "CMD CHARITY_TRANSFER"
                    Utils ut = new Utils();
                    retStr = ut.CHARITY_TRANSFER(hashTbl, ip, user_agent);
                    //giai phong bo nho
                    ut = null;
                    break;
                #endregion "CMD CHARITY_TRANSFER "

                case "HANDLE_BENLIST":      //DONE
                    #region "CMD HANDLE_BENLIST"
                    String action = Funcs.getValFromHashtbl(hashTbl, "ACTION");
                    Beneficiary HANDLE_BENLIST = new Beneficiary();
                    if (action == "ADDNEW")
                        retStr = HANDLE_BENLIST.INSERT_BEN(hashTbl, ip, user_agent);
                    else if (action == "DELETE")
                        retStr = HANDLE_BENLIST.DELETE_BEN(hashTbl);
                    else if (action == "UPDATE")
                        retStr = HANDLE_BENLIST.UPDATE_BEN(hashTbl, ip, user_agent);
                    break;
                #endregion "CMD HANDLE_BENLIST"

                case "DOMESTIC":            //DONE
                    #region "CMD DOMESTIC"

                    mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
                    des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
                    des_name = Funcs.getValFromHashtbl(hashTbl, "ACCT_NAME");
                    txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");
                    type = Funcs.getValFromHashtbl(hashTbl, "TYPE");
                    amount = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");
                    pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

                    bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");
                    bank_name = Funcs.getValFromHashtbl(hashTbl, "BANK_NAME");

                    bank_branch = Funcs.getValFromHashtbl(hashTbl, "BANK_BRANCH");
                    bank_city = Funcs.getValFromHashtbl(hashTbl, "BANK_CITY");

                    string bank_branch_name = Funcs.getValFromHashtbl(hashTbl, "BANK_BRANCH_NAME");
                    string bank_city_name = Funcs.getValFromHashtbl(hashTbl, "BANK_CITY_NAME");


                    ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD");

                    save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");
                    retStr = Financial_Transfer.DOMESTIC_TRANSFER(
                        mob_user
                        , src_acct
                        , pwd
                        , des_acct
                        , des_name
                        , "*"
                        , type
                        , txdesc
                        , double.Parse(amount)
                        , bank_code
                        , bank_name
                        , bank_branch
                        , bank_city
                        , bank_branch_name
                        , bank_city_name
                        , ccycd
                        , save_to_benlist
                        , ip
                        , user_agent
                        , hashTbl
                        );

                    break;
                #endregion "CMD DOMESTIC"

                case "GET_BANK_CODE_CITAD": //DONE
                    #region "GET_BANK_CODE_CITAD"
                    //mob_user = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    Utils GET_BANK_CODE_CITAD = new Utils();
                    retStr = GET_BANK_CODE_CITAD.GET_BANK_CODE_CITAD();
                    break;
                #endregion "GET_BANK_CODE_CITAD"

                case "GET_CITY_CITAD":      //DONE
                    #region "GET_CITY_CITAD"
                    bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");
                    Utils GET_CITY_CITAD = new Utils();
                    retStr = GET_CITY_CITAD.GET_CITY_CITAD(bank_code);
                    break;
                #endregion "GET_CITY_CITAD"

                case "GET_BANK_BRANCH_CITAD":  //DONE
                    #region "GET_BANK_BRANCH_CITAD"
                    bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");
                    bank_city = Funcs.getValFromHashtbl(hashTbl, "CITY_CODE");
                    Utils GET_BANK_BRANCH_CITAD = new Utils();
                    retStr = GET_BANK_BRANCH_CITAD.GET_BANK_BRANCH_CITAD(bank_code, bank_city);
                    break;
                #endregion "GET_CITY_CITAD"

                case "GET_LIST_MOBILE_PRICE":  //DONE
                    #region "GET_LIST_MOBILE_PRICE"
                    mob_mobil_no = Funcs.getValFromHashtbl(hashTbl, "MOBILE_NO");
                    Payment GET_LIST_MOBILE_PRICE = new Payment();
                    retStr = GET_LIST_MOBILE_PRICE.GET_LIST_MOBILE_PRICE(mob_mobil_no);
                    break;
                #endregion "GET_LIST_MOBILE_PRICE"

                case "GET_PRICE_TOPUP_OTHER":  //DONE

                    Payment GET_PRICE_TOPUP_OTHER = new Payment();
                    retStr = GET_PRICE_TOPUP_OTHER.GET_PRICE_TOPUP_OTHER(hashTbl);
                    break;

                case "TOPUP":
                    #region "TOPUP"
                    Payment TOPUP = new Payment();

                    src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
                    if (src_acct.Length > 10)
                    {
                        retStr = TOPUP.TOPUPByCreditCard(hashTbl, ip, user_agent);
                    }
                    else
                    {
                        retStr = TOPUP.TOPUP(hashTbl, ip, user_agent);
                    }

                    //release
                    TOPUP = null;
                    break;
                #endregion "GET_LIST_MOBILE_PRICE"

                case "TRANSFER_247":
                    #region "TRANSFER_247"
                    Transfer247 TRANSFER_247 = new Transfer247();
                    retStr = TRANSFER_247.TRANSFER247(hashTbl, ip, user_agent);
                    //release
                    TRANSFER_247 = null;
                    break;
                #endregion "TRANSFER_247"

                case "GET_CATEGORY_BY_TRAN_TYPE":
                    #region "GET_CATEGORY_BY_TRAN_TYPE"

                    Payment GET_CATEGORY_BY_TRAN_TYPE = new Payment();
                    retStr = GET_CATEGORY_BY_TRAN_TYPE.GET_CATEGORY_BY_TRAN_TYPE(hashTbl);

                    //release
                    GET_CATEGORY_BY_TRAN_TYPE = null;
                    break;
                #endregion "GET_LIST_MOBILE_PRICE"k

                case "GET_SERVICES":
                    #region "GET_SERVICES"
                    Payment GET_SERVICES = new Payment();
                    retStr = GET_SERVICES.GET_SERVICES(hashTbl);
                    break;
                #endregion "GET_SERVICES"

                case "GET_BANK_CODE_247_AC2AC":            //DONE
                    #region "GET_BANK_CODE_247_AC2AC"
                    Utils GET_BANK_CODE_247_AC2AC = new Utils();
                    retStr = GET_BANK_CODE_247_AC2AC.GET_BANK_CODE_247_AC2AC(hashTbl);
                    break;
                #endregion "GET_LIST_MOBILE_PRICE"

                case "GET_247_ACCT_HOLDER":            //DONE
                    #region "GET_247_ACCT_HOLDER"
                    //GET_CARD_HOLDER_NAME_SMLGW
                    Transfer247 GET_247_ACCT_HOLDER = new Transfer247();
                    retStr = GET_247_ACCT_HOLDER.GET_247_ACCT_HOLDER(hashTbl);
                    break;
                #endregion "GET_247_ACCT_HOLDER"

                case "GET_247_ACCT_HOLDER_NEW":            //DONE
                    #region "GET_247_ACCT_HOLDER_NEW"
                    //GET_CARD_HOLDER_NAME_SMLGW
                    Transfer247 GET_247_ACCT_HOLDER_NEW = new Transfer247();
                    retStr = GET_247_ACCT_HOLDER_NEW.GET_247_ACCT_HOLDER_NEW(hashTbl);
                    break;
                #endregion "GET_247_ACCT_HOLDER_NEW"


                case "GET_247_CARD_HOLDER":                   //DONE
                    #region "GET_247_CARD_HOLDER"
                    //GET_CARD_HOLDER_NAME_SMLGW
                    Transfer247 GET_247_CARD_HOLDER = new Transfer247();
                    retStr = GET_247_CARD_HOLDER.GET_247_CARD_HOLDER(hashTbl);
                    break;
                #endregion "GET_247_ACCT_HOLDER"

                case "GET_STOCK_BRANCH_LIST":             //DONE
                    #region "GET_STOCK_BRANCH_LIST"
                    Utils GET_STOCK_BRANCH_LIST = new Utils();
                    retStr = GET_STOCK_BRANCH_LIST.GET_STOCK_BRANCH_LIST(hashTbl);
                    break;
                #endregion "GET_STOCK_BRANCH_LIST"

                case "STOCK_TRANSFER":                    //DONE
                    #region "STOCK_TRANSFER"
                    Utils STOCK_TRANSFER = new Utils();
                    retStr = STOCK_TRANSFER.STOCK_TRANSFER(hashTbl, ip, user_agent);
                    break;
                #endregion "STOCK_TRANSFER"

                case "GET_FEE_BY_TRAN_TYPE":            //DONE
                    #region "GET_FEE_BY_TRAN_TYPE"
                    Utils GET_FEE_BY_TRAN_TYPE = new Utils();
                    retStr = GET_FEE_BY_TRAN_TYPE.GET_FEE_BY_TRAN_TYPE(hashTbl);
                    break;
                #endregion "GET_FEE_BY_TRAN_TYPE"

                case "GET_ACCT_TIDE_OL_INFO_LIST":
                    #region "GET_ACCT_TIDE_OL_INFO_LIST"
                    Utils GET_ACCT_TIDE_OL_INFO_LIST = new Utils();
                    retStr = GET_ACCT_TIDE_OL_INFO_LIST.GET_ACCT_TIDE_OL_INFO_LIST(hashTbl);
                    break;
                #endregion "GET_ACCT_TIDE_OL_INFO_LIST"

                case "GET_TIDE_INTEREST_RATE":
                    #region "GET_TIDE_INTEREST_RATE"
                    Utils GET_TIDE_INTEREST_RATE = new Utils();
                    retStr = GET_TIDE_INTEREST_RATE.GET_TIDE_INTEREST_RATE(hashTbl);
                    break;
                #endregion "GET_TIDE_INTEREST_RATE"

                case "TIDEBOOKING":
               #region "TIDEBOOKING"
                    Utils TIDEBOOKING = new Utils();
                    retStr = TIDEBOOKING.TIDEBOOKING(hashTbl, ip, user_agent);
                    //relase
                    TIDEBOOKING = null;
                    break;
                #endregion "TIDEBOOKING"

                case "TIDEWDL":
                    #region "TIDEWDL"
                    Utils TIDEWDL = new Utils();
                    retStr = TIDEWDL.TIDEWDL(hashTbl, ip, user_agent);
                    //relase
                    TIDEWDL = null;
                    break;
                #endregion "TIDEWDL"

                case "BILLING":
                    #region "BILLING"
                    Payment BILLING = new Payment();
                    string src_acc = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
                    if (src_acc.Length > 10)
                    {
                        retStr = BILLING.PaymentByCreditCard(hashTbl, ip, user_agent);
                    }
                    else
                    {
                        retStr = BILLING.BILL_PAYMENT(hashTbl, ip, user_agent);
                    }

                    //release
                    BILLING = null;
                    break;
                #endregion "BILLING"

                case "GET_BILL_INFO":
                    #region "GET_BILL_INFO"
                    Payment GET_BILL_INFO = new Payment();
                    retStr = GET_BILL_INFO.getBillInfo(hashTbl);
                    break;
                #endregion "GET_BILL_INFO"
                case "GET_LIMIT_TRANSACTION_AMOUNT":
                    Utils utils = new Utils();
                    string cif = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    string channel_id = Funcs.getValFromHashtbl(hashTbl, "CHANNEL");
                    string transtype = Funcs.getValFromHashtbl(hashTbl, "TRANS_TYPE");//DOMESTIC_ACC
                    retStr = utils.GetLimitTransactionBy(cif, channel_id, transtype);
                    break;
                case "GET_LIMIT_TRANSACTION_AMOUNT_V2":
                    Utils GET_LIMIT_TRANSACTION_AMOUNT_V2 = new Utils();
                    cif = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    channel_id = Funcs.getValFromHashtbl(hashTbl, "CHANNEL");
                    transtype = Funcs.getValFromHashtbl(hashTbl, "TRANS_TYPE");//DOMESTIC_ACC
                    retStr = GET_LIMIT_TRANSACTION_AMOUNT_V2.GetLimitTransactionByV2(cif, channel_id, transtype);
                    break;

                case "SETTING":
                    #region "SETTING"

                    retStr = Utils.SETTING(hashTbl);

                    break;
                #endregion "SETTING"


                //TUANNM10 - Dang ky dich vu MB tren MB
                #region "SETTING - Dang ky MB tren MB"
                //get thong tin
                case "GET_INFO_REG_MOB":
                    Utils _utils = new Utils();
                    retStr = _utils.GET_INFO_REG_MOB(hashTbl);
                    break;

                //check dang nhap
                case "CHECK_LOGIN_SM":
                    Funcs.WriteLog("START CHECK_LOGIN_SM: ");
                    Utils utils1 = new Utils();
                    string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    pwd = (Funcs.getValFromHashtbl(hashTbl, "PWD"));
                    retStr = utils1.CHECK_LOGIN_SM(custid, pwd);
                    break;


                //Confirm dang ky dich vu
                case "CONFIRM_REGISTER_MGOLD":
                    Utils utils2 = new Utils();
                    string _custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    string _pwd = Funcs.getValFromHashtbl(hashTbl, "PWD");
                    string _MOBILE_NO_REGISTER = Funcs.getValFromHashtbl(hashTbl, "MOBILE_NO_REGISTER");
                    retStr = utils2.GET_OTP(_custid, _pwd, _MOBILE_NO_REGISTER);
                    break;

                //Dang ky dich vu
                case "REGISTER_MGOLD":
                    Utils utils3 = new Utils();
                    string CIF_NO = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    string TRANID = Funcs.getValFromHashtbl(hashTbl, "TRANID");
                    string ACTIVE_CODE = Funcs.getValFromHashtbl(hashTbl, "ACTIVE_CODE");
                    string MOBILE_NO_REGISTER = Funcs.getValFromHashtbl(hashTbl, "MOBILE_NO_REGISTER");
                    string OLD_AUTH_METHOD = "0";
                    //string AUTH_METHOD_NAME = "MGOLD";
                    string REQUEST_ID = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
                    string OLD_MOBIL_NO = Funcs.getValFromHashtbl(hashTbl, "OLD_MOBIL_NO");
                    //string AUTH_METHOD = "2";

                    retStr = utils3.REGISTER_MGOLD(CIF_NO, MOBILE_NO_REGISTER, OLD_AUTH_METHOD, OLD_MOBIL_NO,
                       ACTIVE_CODE, Double.Parse(TRANID), REQUEST_ID);
                    break;

                #endregion



                #region "21. CARD"
                // lay danh sach the
                case "GET_CARD_LIST":
                    CardUltils cdlist = new CardUltils();
                    retStr = cdlist.GET_CARD_LIST(hashTbl);
                    cdlist = null;
                    break;

                // lay thong tin the debit + 5 giao dich gan nhat
                case "GET_DEBIT_CARD_INFO":
                    retStr = CardUltils.GET_DEBIT_CARD_INFO(hashTbl);
                    break;


                // lay thong tin the credit + 5 giao dich gan nhat
                case "GET_CREDIT_CARD_INFO":
                    retStr = CardUltils.GET_CREDIT_CARD_INFO(hashTbl);
                    break;

                //Khoa mo the
                case "HANDLE_CARD":
                    retStr = CardUltils.HANDLE_CARD(hashTbl, ip, user_agent);
                    break;

                //liet ke giao dich the theo loai giao dich --> sua lai store de lay cho nhanh va chinh xac
                case "GET_CARD_TRAN_BY_ENQ_TYPE":
                    retStr = CardUltils.GET_CARD_TRAN_BY_ENQ_TYPE(hashTbl);
                    break;

                case "GET_CARD_NAME_BY_ACCTNO":
                    retStr = CardUltils.GET_CARD_NAME_BY_ACCTNO(hashTbl);
                    break;

                case "CREDIT_CARD_PAYMENT":
                    retStr = CardUltils.CREDIT_CARD_PAYMENT(hashTbl, ip, user_agent);
                    break;

                case "GET_LIMIT_CREDIT_CARD":
                    #region "GET_LIMIT_CREDIT_CARD"
                    CardUltils cardu = new CardUltils();
                    retStr = cardu.GET_LIMIT_CREDIT_CARD(hashTbl);
                    break;
                #endregion "GET_LIMIT_CREDIT_CARD"

                case "SET_LIMIT_CREDIT_CARD":
                    #region "SET_LIMIT_CREDIT_CARD"
                    CardUltils SET_LIMIT_CREDIT_CARD = new CardUltils();
                    retStr = SET_LIMIT_CREDIT_CARD.SET_LIMIT_CREDIT_CARD(hashTbl, ip, user_agent);
                    break;
                #endregion "SET_LIMIT_CREDIT_CARD"

                #endregion "CARD"

                #region "OTHER"
                case "GET_INBOX_OUTBOX":
                    retStr = Utils.GET_INBOX_OUTBOX(hashTbl);
                    break;

                case "SEND_MAIL_OUTBOX":
                    retStr = Utils.SEND_MAIL_OUTBOX(hashTbl);
                    break;

                case "DELETE_MAIL":
                    retStr = Utils.DELETE_MAIL(hashTbl);
                    break;

                case "CHECK_REF_INVITE":
                    retStr = Utils.CHECK_REF_INVITE(hashTbl);
                    break;

                case "UPDATE_REF_INVITE":
                    retStr = Utils.UPDATE_REF_INVITE(hashTbl);
                    break;

                #endregion "OTHER"

                #region "LOCATION"
                case "GET_LOCATION_LIST":
                    retStr = Utils.GET_LOCATION_LIST(hashTbl);
                    break;

                case "GET_LOCATION_LIST_BY_ADDRESS":
                    retStr = Utils.GET_LOCATION_LIST_BY_ADDRESS(hashTbl);
                    break;

                #endregion "LOCATION"

                #region "FX RATE & TIDE RATE"

                case "GET_FX_RATE":
                    retStr = Utils.GET_FX_RATE(hashTbl);
                    break;

                case "GET_TIDE_RATE":
                    retStr = Utils.GET_TIDE_RATE(hashTbl);
                    break;

                #endregion "FX RATE & TIDE RATE"


                //vnpay qr code
                case "QR_PAYMENT":

                    #region QR_PAYMENT
                    src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");

                    if (src_acct.Length > 10)
                    {
                        retStr = new QRPayment().qrPaymentByCredit(hashTbl, ip, user_agent);
                    }
                    else
                    {
                        retStr = new QRPayment().qrPayment(hashTbl, ip, user_agent);
                    }
                    #endregion

                    
                    break;


                //REQ = CMD#GET_QR_DISCOUNT|CIF_NO#{CIF_NO}|TOKEN={TOKEN}|MOBILE#{MOBILE}|ITEM#{ITEM}|PAYTYPE#{PAYTYPE}|VOUCHER#{VOUCHER}
                case "GET_QR_DISCOUNT":
                    retStr = new QRPayment().qrDiscount(hashTbl, ip, user_agent);
                    break;

                //tungdt8
                //CMD#GET_BILL_DAIICHI|CIF_NO#{CIF_NO}|TOKEN={TOKEN}|POLICYNUMBER#{POLICYNUMBER}
                case "GET_BILL_DAIICHI":
                    retStr = new DLVNSOA_().getBillDaiichi(hashTbl, ip, user_agent);
                    break;
                #region Change_PWD
                //CMD#CHANGE_PWD_VERIFY|MOBILE#{MOBILE}|PASSNO#{PASSNO}|LEGACYAC#{LEGACYAC}
                case "CHANGE_PWD_VERIFY":
                    retStr = new ChangePassword().Verify(hashTbl, ip, user_agent);
                    break;
                //CMD#CHANGE_PWD_CONFIRM|CIF_NO#{CIF_NO}|TRAN_ID#{TRAN_ID}|OTP#{OTP}
                case "CHANGE_PWD_CONFIRM":
                    retStr = new ChangePassword().Confirm(hashTbl, ip, user_agent);
                    break;
                #endregion

                #region API Push Notification
                case "REGISTER_PUSH_NOTIFICATION":
                    retStr = new PushNotification().Register(hashTbl, ip, user_agent);
                    break;
                case "LIST_FUNCTION_PUSH":
                    retStr = new PushNotification().GetListFuncNotification(hashTbl, ip, user_agent);
                    break;
                case "UPDATE_FUNCTION_PUSH":
                    retStr = new PushNotification().UpdateListFuncNotification(hashTbl, ip, user_agent);
                    break;
                case "LIST_NEWS":
                    retStr = new PushNotification().GetListNewsNotification(hashTbl, ip, user_agent);
                    break;
                case "VIEW_NEWS":
                    retStr = new PushNotification().GetDetailNewsNotification(hashTbl, ip, user_agent);
                    break;
                case "SET_UNREAD_NEWS":
                    retStr = new PushNotification().SetUnreadNews(hashTbl, ip, user_agent);
                    break;
                case "GET_UNREAD_NEWS":
                    retStr = new PushNotification().GetUnreadNews(hashTbl, ip, user_agent);
                    break;
                #endregion

                #region TokenOTP
                //CMD#GET_ACTIVATION_CODE|CIF_NO#{CIF_NO}|TOKEN#{TOKEN}|OPTION#{OPTION}
                case "GET_ACTIVATION_CODE":
                    retStr = new TokenOTPFunc().GetActivationCode(hashTbl, ip, user_agent);
                    break;
                //CMD#SYNCHRONIZE_OTP|CIF_NO#{CIF_NO}|TOKEN#{TOKEN}|OPERATORID#{OPERATORID}|OTP1#{OTP1}|OTP2#{OTP2}
                case "SYNCHRONIZE_OTP":
                    retStr = new TokenOTPFunc().SynchronizeOTP(hashTbl, ip, user_agent);
                    break;
                case "GET_OTP": //DUNG CHO AUTH_METHOD = 4 HOAC 5
                    retStr = new TokenOTPFunc().GetOtp(hashTbl, ip, user_agent);
                    break;
                case "GET_INFO_TOKEN_POPUP":
                    retStr = new TokenOTPFunc().GetInfoTokenPopup(hashTbl, ip, user_agent);
                    break;
                case "UPDATE_STATUS_SHOW_TOKEN_POPUP":
                    retStr = new TokenOTPFunc().UpdateStatusShowTokenPoup(hashTbl, ip, user_agent);
                    break;
                #endregion
                #region Chan Nguoi Nguoc NgoaiNationality
                case "CHECK_NATIONALITY":
                    string custid_check = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                    retStr = AccIntegration.checkNationality(custid_check);
                    break;
                #endregion

                #region ELOUNGE
                case "GET_VOUCHER_LIST":
                    retStr = EVoucher.GetVoucherList(hashTbl, ip, user_agent);
                    break;
                #endregion

                #region PAY_CREDIT_CHECK
                case "CHECK_PAY_CREDIT_BY_SERVICEID":
                    Payment CHECK_BY_SERVICEID = new Payment();
                    retStr = CHECK_BY_SERVICEID.GET_PAY_CREDIT_BY_SERVICEID(hashTbl);
                    break;
                #endregion

                #region CARD_DPP Register
                case "RESGISTER_CARD_DPP":
                    retStr =  new CardDPPUltils().RESGISTER_CARD_DPP(hashTbl, ip, user_agent);
                    break;
                case "GET_CARD_INSTALLMENT_PERIOD":
                    retStr = new CardDPPUltils().GET_CARD_INSTALLMENT_PERIOD(hashTbl);
                    break;
                case "GET_CARD_INSTALLMENT_PERIOD_BY_TRAN":
                    retStr = new CardDPPUltils().GET_CARD_INSTALLMENT_PERIOD_BY_TRAN(hashTbl);
                    break;
                case "GET_CARD_INSTALLMENT_SCHEDULE":
                    retStr = new CardDPPUltils().GET_CARD_INSTALLMENT_SCHEDULE(hashTbl);
                    break;
                #endregion

                #region Rut Goc Linh Hoat
                case "GET_EOD_STATUS":
                    retStr = CoreIntegration.getEODStatus(hashTbl);
                    break;
                case "GET_FLEX_TIDE_RATE":
                    retStr = CoreIntegration.flexTideRate(hashTbl);
                    break;
                case "GET_FLEX_TIDE_DETAIL":
                    retStr = CoreIntegration.getAcctTideDetail(hashTbl);
                    break;
                case "FLEXTIDEBOOKING":
                    #region "FLEXTIDEBOOKING"
                    Utils FLEXTIDEBOOKING = new Utils();
                    retStr = FLEXTIDEBOOKING.FLEXTIDEBOOKING(hashTbl, ip, user_agent);
                    //relase
                    TIDEBOOKING = null;
                    break;
                #endregion "FLEXTIDEBOOKING"
                case "FLEXTIDEWDL":
                    Utils flexTide = new Utils();
                    retStr = flexTide.FLEXTIDEWDL(hashTbl, ip, user_agent);
                    //relase
                    flexTide = null;
                    break;

                #endregion
                #region CARD VERIFY
                case "VERIFY_CARD_NUM":
                    retStr = CardUltils.verifyCardNum(hashTbl);
                    break;
                case "SET_PIN_CARD":
                    retStr = CardUltils.setPinCard(hashTbl, ip, user_agent);
                    break;
                #endregion
                #region GET INFO COMBO
                case "COMBO_INFO":
                    retStr = AccIntegration.GetComboInfo(hashTbl, ip, user_agent);
                    break;
                #endregion
                #region TOPUP PRE-PAID CARD
                case "GET_PREPAID_CARD_DETAIL":
                    retStr = new CardUltils().GET_PREPAID_CARD_DETAIL(hashTbl);
                    break;
                case "DO_TOPUP_PREPAID_CARD":
                    retStr = new CardUltils().DO_TOPUP_PREPAID_CARD(hashTbl, ip, user_agent);
					break;
				#endregion
                #region Dang ky trich no tu dong
                case "GET_AUTO_DEBIT":
                    retStr = new CardUltils().GET_AUTO_DEBIT(hashTbl, ip, user_agent);
                    break;
                case "HANDLE_AUTO_DEBIT":
                    retStr = new CardUltils().HANDLE_AUTO_DEBIT(hashTbl, ip, user_agent);
                    break;
                #endregion

                #region Auto Saving
                case "HANDLE_AUTO_SAVING":
                    retStr = new Account().HANDLE_AUTO_SAVING(hashTbl, ip, user_agent);
                    break;
                case "GET_LIST_AUTO_SAVING":
                    retStr = new Account().GET_LIST_AUTO_SAVING(hashTbl, ip, user_agent);
                    break;
                case "GET_HIS_AUTO_SAVING":
                    retStr = new Account().GET_HIST_AUTO_SAVING(hashTbl, ip, user_agent);
                    break;
                case "GET_DETAIL_AUTO_SAVING":
                    retStr = new Account().GET_DETAIL_AUTO_SAVING(hashTbl, ip, user_agent);
                    break;
                #endregion

                #region Card International
                case "GET_DETAIL_UNBLOCK_CARD":
                    retStr = new CardUltils().GET_UNBLOCK_CARD_DETAIL(hashTbl, ip, user_agent);
                    break;
                case "UNBLOCK_CARD":
                    retStr = new CardUltils().UNBLOCK_CARD(hashTbl, ip, user_agent);
                    break;
                #endregion

                #region ACCT NICE
                case "GET_LIST_ACCT_NICE":
                    retStr = new Accounts().GET_LIST_ACCT_NICE(hashTbl, ip, user_agent);
                    break;
                case "REGIST_ACCT_NICE":
                    retStr = new Accounts().REGIST_ACCT_NICE(hashTbl, ip, user_agent);
                    break;
                #endregion

                #region GIVE GIFT
                case "GET_GIFT_TYPE":
                    try
                    {
                        retStr = new GiveGift().GET_GIFT_TYPE(hashTbl, ip, user_agent);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("ERROR EXCEPTION: " + ex.ToString());
                        retStr = Config.ERR_MSG_GENERAL;
                        break;
                    }
                case "GET_GIFT_TEMPLACE":
                    try
                    {
                        retStr = new GiveGift().GET_GIFT_TEMPLACE(hashTbl, ip, user_agent);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("ERROR EXCEPTION: " + ex.ToString());
                        retStr = Config.ERR_MSG_GENERAL;
                        break;
                    }
                case "GIVE_GIFT":
                    try
                    {
                        retStr = new GiveGift().GIVE_GIFT(hashTbl, ip, user_agent);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("ERROR EXCEPTION: " + ex.ToString());
                        retStr = Config.ERR_MSG_GENERAL;
                        break;
                    }
                case "OPEN_GIFT":
                    try
                    {
                        retStr = new GiveGift().OPEN_GIFT(hashTbl, ip, user_agent);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("ERROR EXCEPTION: " + ex.ToString());
                        retStr = Config.ERR_MSG_GENERAL;
                        break;
                    }
                case "GIFT_HISTORY":
                    try
                    {
                        retStr = new GiveGift().GIFT_HISTORY(hashTbl, ip, user_agent);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("ERROR EXCEPTION: " + ex.ToString());
                        retStr = Config.ERR_MSG_GENERAL;
                        break;
                    }
                #endregion
                default:
                    retStr = Config.ERR_MSG_GENERAL;
                    break;
            }

            //Funcs.WriteLog( Config.BEGIN_TAG + retStr + Config.END_TAG);
            //Response.Write( Config.BEGIN_TAG + retStr + Config.END_TAG);
            retStr = Config.BEGIN_TAG + retStr + Config.END_TAG;

            // Funcs.WriteLog("ORG RESPONSE|" + retStr);

            Funcs.WriteLog("ORG RESPONSE WITH MASKING|" + Funcs.MaskingString(retStr));

            //Funcs.WriteLog( "ENCRYPT RESPONSE|" + Funcs.Encrypt2( retStr, activeCode));					   
            //Response.Write( Funcs.Encrypt2( retStr, activeCode) );

            Response.Write(retStr);
        }

    }

    /* private bool checkMinBalOfStaff(string acctno, double amount)
     {
         bool ret = false;
         double avaiableBalance = 0;
         Balance bl = new Balance();
         BalanceData blda = new BalanceData();
         blda = bl.getBALANCE_ACCTNO( acctno, Config.prod_cd_CASA);
         if ( ( blda != null)  && ( blda.Tables[0].Rows.Count > 0 ))
         {
             avaiableBalance = Double.Parse(blda.Tables[BalanceData.BALANCE_TABLE].Rows[0][BalanceData.ACCT_RBAL_FIELD].ToString());

             //if (amount + Config.LIMIT_MIN_BALANCE_STAFF > avaiableBalance)
             if (amount  > avaiableBalance)
             {
                 return false;
             }
             else
             {
                 return true;
             }
         }
         else 
         {
             ret = false;
         }

         //giai phong bo nho

         bl = null; 
         if ( blda != null) blda.Dispose();
                                                
         return ret; 
     }
     */
    private bool checkRequest(string str)
    {
        //			int i = str.IndexOf("MIDP");
        //			int j = str.IndexOf("CLDC");
        //			if ( ( str.IndexOf("MIDP") < 0) && ( str.IndexOf("CLDC") < 0))
        //			{
        //				return false;
        //			}
        //			else
        //			{
        //			return true;
        //			}
        return true;
    }

    public string ShowTenure(string Tenure, string Tenure_Symbol)
    {
        switch (Tenure_Symbol)
        {
            case "Y": return Tenure + " năm";
            case "M": return Tenure + " tháng";
            case "D": return Tenure + " ngày";
            case "W": return Tenure + " tuần";
            default: return "";
        }
    }

    public string ShowBalance(double Balance, string CCY)
    {
        try
        {
            if (CCY == "VND")
            {
                return String.Format("{0:#,###0}", Balance);
            }
            else
            {
                return String.Format("{0:#,###0.##}", Balance);
            }
        }
        catch
        {
            return "";
        }
    }

}
