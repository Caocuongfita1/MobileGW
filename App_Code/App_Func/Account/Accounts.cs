using mobileGW.Service.DataAccess;
using mobileGW.Service.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Accounts
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class Accounts
    {
        public Accounts()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// lấy danh sách thẻ 
        /// linhtn 23 jul 2016
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public string GET_LIST_ACCT_NICE(Hashtable hashTbl, string ip, string user_agent)
        {
            #region "CMD GET_CARD_LIST"
            String retStr = Config.GET_ACCT_NICE_LIST;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String accountNo = Funcs.getValFromHashtbl(hashTbl, "ACCOUNT_NO");
            String pageIndex = Funcs.getValFromHashtbl(hashTbl, "PAGE_INDEX");
            String pageSize = Funcs.getValFromHashtbl(hashTbl, "PAGE_SIZE");
            String token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");
            String jwtTokenREQ = Funcs.getValFromHashtbl(hashTbl, "JWTOKEN");
            string jwtToken = String.Empty;

            AcctNiceQueryList modelGet = new AcctNiceQueryList();
            AcctNiceCheckLogin modelNice = new AcctNiceCheckLogin();
            String resCode = String.Empty;
            String resDescVn = String.Empty;
            String resDescEn = String.Empty;

            try
            {

                if (jwtTokenREQ.Equals("NOT_FOUND") || string.IsNullOrEmpty(jwtTokenREQ))
                {
                    try
                    {
                        modelNice.channelId = Config.ChannelID;
                        modelNice.custId = custid;
                        modelNice.ebToken = token;
                        modelNice.machine = user_agent;

                        string response = new AcctNiceDAO().Check_Ebank_Login(modelNice);

                        if (response != null && !String.IsNullOrEmpty(response))
                        {
                            Funcs.WriteLog("CIF_NO: " + custid + "|GET_JWTOKEN: SUCCESS");
                            jwtToken = response;
                        }
                        else
                        {
                            Funcs.WriteLog("CIF_NO: " + custid + "|GET_JWTOKEN: ERROR - response null");
                            return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
                        }

                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("CIF_NO: " + custid + "|GET_JWTOKEN: ERROR " + ex.Message.ToString());
                        return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
                    }
                }
                else
                {
                    Funcs.WriteLog("CIF_NO: " + custid + "|GET_JWTOKEN: SUCCESS");
                    jwtToken = jwtTokenREQ;
                }

                modelGet.accountNo = accountNo;
                modelGet.accountType = "NICE";
                modelGet.pageIndex = Double.Parse(pageIndex);
                modelGet.pageSize = 10;
                modelGet.channelId = Config.ChannelID;

                List<AcctNiceItemModel> resModel = new AcctNiceDAO().GetListAcctNices(custid,modelGet, jwtToken, ref resCode, ref resDescVn, ref resDescEn);

                if (resCode.Equals(Config.ERR_CODE_DONE))
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{CIF_NO}", custid);
                    retStr = retStr.Replace("{ERR_DESC}", "GET LIST ACCT NICE SUCCESSFUL");
                    retStr = retStr.Replace("{JWTOKEN}", jwtToken);

                    string strTemp = "";
                    foreach (var item in resModel)
                    {
                        strTemp = strTemp +
                          item.accountNo
                          + Config.COL_REC_DLMT
                          + item.currencyCode
                          + Config.COL_REC_DLMT
                          + item.currencyName
                          + Config.COL_REC_DLMT
                          + item.groupType
                          + Config.COL_REC_DLMT
                          + item.feeAmount
                          + Config.COL_REC_DLMT
                          + item.vatAmount
                          + Config.COL_REC_DLMT
                          + item.totalAmount
                          + Config.COL_REC_DLMT
                          + item.discountAmt
                          + Config.COL_REC_DLMT
                          + item.discountFeeAmt
                          + Config.COL_REC_DLMT
                          + item.discountVatAmt
                          + Config.ROW_REC_DLMT;
                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                }
                else if (resCode.Equals(Config.ERR_CODE_GENERAL))
                {
                    Funcs.WriteLog("CIF_NO: " + custid + "|GET_LIST_ACCT_NICE: ERROR ");

                    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(2)).Replace("{2}", custid);
                } 
                else if(!String.IsNullOrEmpty(resCode)) 
                {
                    Funcs.WriteLog("CIF_NO: " + custid + "|GET_LIST_ACCT_NICE: ERROR FROM CORE");

                    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", resCode).Replace("{1}", resDescVn + Config.COL_REC_DLMT + resDescEn).Replace("{2}", custid);
                }
                else
                {
                    Funcs.WriteLog("CIF_NO: " + custid + "|GET_LIST_ACCT_NICE: ERROR ");

                    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(2)).Replace("{2}", custid);
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO:" + custid + "GET_LIST_ACCT_NICE: " + ex.ToString());
                return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(2)).Replace("{2}", custid);
            }
            return retStr;
            #endregion "CMD GET_CARD_LIST"
        }

        /// <summary>
        /// lấy danh sách thẻ 
        /// linhtn 23 jul 2016
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public string REGIST_ACCT_NICE(Hashtable hashTbl, string ip, string user_agent)
        {
            #region "CMD GET_CARD_LIST"
            String retStr = Config.RES_ACCT_NICE;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String accountNo = Funcs.getValFromHashtbl(hashTbl, "ACCOUNT_NO");
            String token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");
            String jwToken = Funcs.getValFromHashtbl(hashTbl, "JWTOKEN");
            String amount = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");
            String src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            String refId = Funcs.getValFromHashtbl(hashTbl, "REFERRER_ID");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");

            string check_before_trans = "";
            string tran_type = Config.TRAN_TYPE_ACCT_NICE;
            string txdesc = "THANH TOAN PHI MO TAI KHOAN SO DEP";
            AcctNiceQueryList modelGet = new AcctNiceQueryList();
            AcctNiceCheckLogin modelNice = new AcctNiceCheckLogin();

            double totalAmount = 0;
            double vatAmount = 0;
            string groubType = String.Empty;
            double eb_tran_id = 0;

            string des_acct = Funcs.getConfigVal("ACCT_NICE_GL_SUSPEND");
            string core_txno_ref = String.Empty;
            string core_txdate_ref = String.Empty;
            string gl_vat = Funcs.getConfigVal("ACCT_NICE_GL_VAT");
            string custName = String.Empty;
            string email = String.Empty;
            string regbranch = String.Empty;
            String resCode = String.Empty;
            String resDescVn = String.Empty;
            String resDescEn = String.Empty;

            Funcs.WriteLog("custid:" + custid + "|REGIST_ACCT_NICE BEGIN");

            try
            {

                //string acctString = accountNo.Replace("%", "");

                //if (acctString.Length == 10 && (acctString.Substring(0, 1).Equals("0") || acctString.Substring(0, 1).Equals("1") || acctString.Substring(0, 1).Equals("9")))
                //{
                //    Funcs.WriteLog("CIF_NO: " + custid + "|GET_LIST_ACCT_NICE: ERROR. TAI KHOAN THUOC NHOM A0");
                //    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", "15").Replace("{1}", Funcs.gResult_Acct_Open_Arr(4)).Replace("{2}", custid);
                //}

                modelGet.accountNo = accountNo;
                modelGet.accountType = "SELF_SELECT";
                modelGet.pageIndex = 1;
                modelGet.pageSize = 1;
                modelGet.channelId = Config.ChannelID;

                List<AcctNiceItemModel> resModel = new AcctNiceDAO().GetListAcctNices(custid,modelGet, jwToken, ref resCode, ref resDescVn, ref resDescEn);

                if (resCode.Equals(Config.ERR_CODE_DONE))
                {
                    if (resModel[0].totalAmount != resModel[0].discountAmt)
                    {
                        totalAmount = resModel[0].discountFeeAmt;
                        accountNo = resModel[0].accountNo;
                        groubType = resModel[0].groupType;
                        vatAmount = resModel[0].discountVatAmt;
                    }
                    else
                    {
                        totalAmount = resModel[0].feeAmount;
                        accountNo = resModel[0].accountNo;
                        groubType = resModel[0].groupType;
                        vatAmount = resModel[0].vatAmount;
                    }
                }
                else if (resCode.Equals(Config.ERR_CODE_GENERAL))
                {
                    Funcs.WriteLog("CIF_NO: " + custid + "|GET_LIST_ACCT_NICE: ERROR ");

                    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(2)).Replace("{2}", custid);
                }
                else if (!String.IsNullOrEmpty(resCode))
                {
                    Funcs.WriteLog("CIF_NO: " + custid + "|GET_LIST_ACCT_NICE: ERROR FROM CORE");

                    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", resCode).Replace("{1}", resDescVn + Config.COL_REC_DLMT + resDescEn).Replace("{2}", custid);
                }
                else
                {
                    Funcs.WriteLog("CIF_NO: " + custid + "|GET_LIST_ACCT_NICE: ERROR ");

                    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(2)).Replace("{2}", custid);
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO:" + custid + "GET_LIST_ACCT_NICE: " + ex.ToString());
                return Config.ERR_MSG_GENERAL;
            }

            //check amount
            if (Double.Parse(amount) != (totalAmount + vatAmount))
            {
                Funcs.WriteLog("CIF_NO:" + custid + "LOI SO TIEN KHONG TRUNG VOI: ");
                return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
            }

            //check groub Type
            if (groubType.Equals("A0"))
            {
                Funcs.WriteLog("CIF_NO:" + custid + "ERROR: SO DEP THUOC NHOM A0 ");
                return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(3)).Replace("{2}", custid);
            }

            //check groub Type
            //if (accountNo.Substring(0, 1).Equals("0") || accountNo.Substring(0, 1).Equals("1") || accountNo.Substring(0, 1).Equals("9"))
            //{
            //    Funcs.WriteLog("CIF_NO:" + custid + "LOI SO TIEN KHONG TRUNG VOI: ");
            //    return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(4)).Replace("{2}", custid);
            //}

            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            if (!check)
            {
                Funcs.WriteLog("CIF_NO:" + custid + "KIEM TRA TAI KHOAN MO SO THUOC CIF: ERROR");
                return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
            }

            //get Infor by CustId
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            DataSet ds = new DataSet();
            ds = da.GET_USER_BY_CIF(custid);
            DataTable dt = new DataTable();

            if (ds != null && ds.Tables[0] != null)
            {
                custName = ds.Tables[0].Rows[0]["CUSTNAME"].ToString();
                email = ds.Tables[0].Rows[0]["EMAIL"].ToString();
                //regbranch = ds.Tables[0].Rows[0]["REG_BRANCH"].ToString();
            }
            else
            {
                Funcs.WriteLog("CIF_NO:" + custid + "LOI GET THONG TIN CIF");
                return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
            }

            string pos_cd = String.Empty;

            pos_cd = CoreIntegration.getPosCdByCif(custid, src_acct);

            if (string.IsNullOrEmpty(pos_cd))
            {
                Funcs.WriteLog("CIF_NO:" + custid + "LOI GET POS_CD");
                return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
            }

            regbranch = CoreIntegration.getPosCdByCif(custid, "");

            if (string.IsNullOrEmpty(regbranch))
            {
                Funcs.WriteLog("CIF_NO:" + custid + "LOI GET regbranch");
                return Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
            }

            #region FOR TOKEN
            int typeOtp = Int16.Parse(typeOtpStr);

            //trong truong hop auth_method = 2 phai ma hoa md5
            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, "", Double.Parse(amount), pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, Double.Parse(amount), custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, Double.Parse(amount), custid, pwd, requestId, typeOtp);
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

                Funcs.WriteLog("custid:" + custid + "|REGIST_ACCT_NICE BEGIN INSERT EB TRAN");

                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , string.Empty //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , Funcs.getConfigVal("ACCT_NICE_GL_SUSPEND") //des_acct
                    , totalAmount + vatAmount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , totalAmount + vatAmount //lcy_amount
                    , txdesc //txdesc
                    , Config.HO_BR_CODE //pos_cd
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
                    , totalAmount + vatAmount // order_amount
                    , totalAmount + vatAmount // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , totalAmount //suppend amount
                    , 0 //fee amount
                    , vatAmount //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , accountNo //bm1
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

                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                    Funcs.WriteLog("custid:" + custid + "|REGIST_ACCT_NICE BEGIN POST TO CORE");

                    string result = String.Empty;
                    

                    if (totalAmount == 0 && vatAmount == 0)
                    {
                        result = Config.gResult_INTELLECT_Arr[0];
                    }
                    else
                    {
                        result = Config.gResult_INTELLECT_Arr[0];

                        //result = CoreIntegration.postFINPOSTToCore(
                        //                                        custid
                        //                                        , tran_type
                        //                                        , eb_tran_id
                        //                                        , src_acct
                        //                                        , des_acct
                        //                                        , ""
                        //                                        , gl_vat
                        //                                        , totalAmount
                        //                                        , 0
                        //                                        , vatAmount
                        //                                        , txdesc
                        //                                        , pos_cd
                        //                                        , ref core_txno_ref
                        //                                        , ref core_txdate_ref
                        //                                        );
                    }

                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {
                        CoreHandleCIF.InquiryResType getCif = AccIntegration.getCifInfo(custid);

                        if (getCif != null && getCif.RespSts != null && getCif.RespSts.Sts.Equals("0"))
                        {
                            AcctNiceCreateModel modeCreate = new AcctNiceCreateModel();
                            string coreDT = DateTime.Now.ToString("dd/MM/yyyy");
                            core_txdate_ref = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now); 
                            
                            modeCreate.accountFinacialStatus = "0";
                            modeCreate.accountFinacialSubStatus = "0";
                            modeCreate.accountName = custName;
                            modeCreate.accountNameInLocalLanguage = "";
                            modeCreate.accountNo = accountNo;
                            modeCreate.accountStatus = "N";
                            modeCreate.action = "CREATE";
                            modeCreate.aprDT = DateTime.Now.ToString("yyyyMMddHHmmss");
                            modeCreate.aprID = "EBANK_APR";
                            modeCreate.block = "101";
                            modeCreate.branchCode = regbranch;
                            modeCreate.charge = totalAmount.ToString();
                            modeCreate.chargeTotal = (totalAmount + vatAmount).ToString();
                            modeCreate.chargeVAT = vatAmount.ToString();
                            modeCreate.cifNo = custid;
                            modeCreate.country = "VN";
                            modeCreate.createType = "NICE";
                            modeCreate.currencyCode = "704";
                            modeCreate.currentComboCode = "";
                            modeCreate.district = getCif.ListCifInfo[0].othersAddressDistrict;
                            modeCreate.mkrDT = DateTime.Now.ToString("yyyyMMddHHmmss");
                            modeCreate.mkrID = "EBANK_MKR";
                            modeCreate.newComboCode = "";
                            modeCreate.noteCode = "CS17";
                            modeCreate.noteDesc = "Mo tai khoan so dep online";
                            modeCreate.passportFutureDateLateSubmission = "";
                            modeCreate.passportOriginal1 = "";
                            modeCreate.productCode = "101";
                            modeCreate.residentPermFutureDateLateSubmission = "";
                            modeCreate.residentPermitCopy1 = "";
                            modeCreate.rmPrimaryCode = (String.IsNullOrEmpty(refId) ? "NO_RM" : refId);
                            modeCreate.rmPrimaryName = "_NULL_";
                            modeCreate.rmSecondaryCode = "_NULL_";
                            modeCreate.rmSecondaryName = "_NULL_";
                            modeCreate.schemeCode = "101VND";
                            modeCreate.securitiesCompany = "";
                            modeCreate.serviceTier = "ST0017";
                            modeCreate.statementCycle = "4";
                            modeCreate.statementDeliveryMode = "0";
                            modeCreate.street = getCif.ListCifInfo[0].othersAddressStreet;
                            modeCreate.town = getCif.ListCifInfo[0].othersAddressCity;
                            modeCreate.ward = getCif.ListCifInfo[0].othersAddressWard;
                            modeCreate.zipCode = getCif.ListCifInfo[0].othersAddressZipCode;
                            modeCreate.feeAcct = src_acct;
                            modeCreate.partnerBranchCode = Config.ChannelID;

                            string errCode = String.Empty;

                            bool isNew = new AcctNiceDAO().CreateAcctNice(custid,modeCreate, ref errCode, ref resDescVn, ref resDescEn,ref core_txno_ref, jwToken);

                            if (isNew)
                            {
                                tf.uptTransferTx(eb_tran_id
                                , Config.TX_STATUS_DONE
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID);

                                Funcs.WriteLog("custid:" + custid + "|SUCCESS REGIST ACCT NICE");

                                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                                retStr = retStr.Replace("{CIF_NO}", custid);
                                retStr = retStr.Replace("{ERR_DESC}", "SUCCESSFUL");
                                retStr = retStr.Replace("{REF_NO}", core_txno_ref);
                                retStr = retStr.Replace("{CORE_DT}", core_txdate_ref);
                                retStr = retStr.Replace("{ACCOUNT_NO}", accountNo);

                                try
                                {
                                    //sendMail
                                    string pathFile = String.Empty;
                                    string strContent = String.Empty;
                                    pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\REGIST_ACCT_NICE.html";
                                    strContent = Funcs.ReadAllFile(pathFile);

                                    strContent = strContent.Replace("P_CUSTNAME_P", custName);
                                    strContent = strContent.Replace("P_ACCOUNT_NICE_P", accountNo);
                                    strContent = strContent.Replace("P_AMOUNT_FEE_P", Funcs.ConvertMoney((totalAmount + vatAmount).ToString()).Trim());
                                    strContent = strContent.Replace("P_REF_ID_P", (String.IsNullOrEmpty(refId) ? "" : refId));
                                    strContent = strContent.Replace("P_REF_NO_P", core_txno_ref);
                                    strContent = strContent.Replace("P_TRAN_DATE_P", coreDT);
                                    strContent = strContent.Replace("P_CONTENT_P", txdesc);
                                    strContent = strContent.Replace("P_ACCOUNT_FEE_P", src_acct);

                                    bool sendMail = new PushNotification().sendPushEmail(custid, email, "SHB - XÁC NHẬN MỞ TÀI KHOẢN SỐ ĐẸP THÀNH CÔNG - " + accountNo, strContent, "EMAIL", eb_tran_id, core_txno_ref); 
                                    //sendPush
                                    string strContentPusht = "Quý khách đã mở tài khoản số đẹp thành công trên Ebank SHB, số tài khoản: " + accountNo + ". Trân trọng cảm ơn Quý khách. Nếu cần thêm thông tin, vui lòng liên hệ 1800.5888.56 (miễn phí 24/7) để được hỗ trợ.";

                                    bool sendPush = new PushNotification().sendPushEmail(custid, custid, "Thông báo mở tài khoản số đẹp", strContentPusht, "OTT", eb_tran_id, core_txno_ref);
                                }
                                catch (Exception ex)
                                {
                                    Funcs.WriteLog("custid:" + custid + "|REGIST ACCT NICE SAVING SEND MAIL FAIL" + ex.Message.ToString());
                                }

                            }
                            else
                            {
                                tf.uptTransferTx(eb_tran_id
                                 , Config.TX_STATUS_FAIL
                                 , core_txno_ref
                                 , core_txdate_ref
                                 , Config.ChannelID);

                                if (errCode.Equals(Config.ERR_CODE_GENERAL))
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|REGIST_ACCT_NICE: ERROR ");

                                    retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
                                }
                                else if (!String.IsNullOrEmpty(errCode))
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|REGIST_ACCT_NICE: ERROR FROM CORE");

                                    retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", errCode).Replace("{1}", resDescVn + Config.COL_REC_DLMT + resDescEn).Replace("{2}", custid);
                                }
                                else
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|REGIST_ACCT_NICE: ERROR ");

                                    retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);
                                }

                                Funcs.WriteLog("custid:" + custid + "|FAIL REGIST ACCT NICE. CREATE ACCT FAIL API");
                            }
                        }
                        else
                        {
                            tf.uptTransferTx(eb_tran_id
                             , Config.TX_STATUS_FAIL
                             , core_txno_ref
                             , core_txdate_ref
                             , Config.ChannelID);

                            retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);

                            Funcs.WriteLog("custid:" + custid + "|GET CIF INFO FAIL");
                        }
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {
                        tf.uptTransferTx(eb_tran_id
                         , Config.TX_STATUS_FAIL
                         , core_txno_ref
                         , core_txdate_ref
                         , Config.ChannelID);

                        retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);

                        Funcs.WriteLog("custid:" + custid + "|REGIST_ACCT_NICE POST TO CORE FAILED");
                    }
                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    retStr = Config.ERR_MSG_FORMAT_NEW.Replace("{0}", Config.ERR_CODE_GENERAL).Replace("{1}", Funcs.gResult_Acct_Open_Arr(1)).Replace("{2}", custid);

                    Funcs.WriteLog("custid:" + custid + "|REGIST_ACCT_NICE CAN NOT INSERT EB TRAN");

                }
            }
            else//AUTH FAILED
            {
                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "|REGIST_ACCT_NICE AUTH FAILED END");
            }


            Funcs.WriteLog("custid:" + custid + "|REGIST_ACCT_NICE END");

            return retStr;
            #endregion "CMD GET_CARD_LIST"
        }
    }
}