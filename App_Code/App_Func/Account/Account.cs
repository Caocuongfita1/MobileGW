
using mobileGW.Service.AppFuncs;
using mobileGW.Service.DataAccess;
using mobileGW.Service.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for Account
/// </summary>
public class Account
{
    public Account()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string HANDLE_AUTO_SAVING(Hashtable hashTbl, string ip, string userAgent)
    {
        string resp = Config.RESPONE_HANDLE_AUTO_SAVING;
        string txdesc = "AUTO SAVING";
        string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

        string legacyAc = Funcs.getValFromHashtbl(hashTbl, "LEGACY_AC");
        string depositType = Funcs.getValFromHashtbl(hashTbl, "DEPOSIT_TYPE");
        string minBal = Funcs.getValFromHashtbl(hashTbl, "MIN_BAL");
        string prinAmt = Funcs.getValFromHashtbl(hashTbl, "PRIN_AMT");
        string tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE");
        string tenureUnit = Funcs.getValFromHashtbl(hashTbl, "TENURE_UNIT");
        string matType = Funcs.getValFromHashtbl(hashTbl, "MAT_TYPE");
        string freqBooking = Funcs.getValFromHashtbl(hashTbl, "FREQ_BOOKING");
        string startDate = Funcs.getValFromHashtbl(hashTbl, "START_DATE");
        string action = Funcs.getValFromHashtbl(hashTbl, "ACTION");

        string resCode = Config.ERR_CODE_GENERAL;
        string resDesc = "HANDLE AUTO SAVING FAIL";
        string refNo = String.Empty;
        string coreDT = String.Empty;

        #region FOR TOKEN
        string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
        string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
        int typeOtp = Int16.Parse(typeOtpStr);

        if (typeOtp == 2)
        {
            pwd = Funcs.encryptMD5(pwd + custid);
        }
        #endregion

        string check_before_trans = "";
        string tran_type = Config.TRAN_TYPE_AUTO_SAVING;

        if (!action.Equals("REGIST") && !action.Equals("UPDATE") && !action.Equals("CANCEL"))
        {
            return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI CHECK ACTION HANDLE_AUTO_SAVING").Replace("{CIF_NO}", custid);
        }

        if (!action.Equals("CANCEL") && (Double.Parse(minBal) < Double.Parse(Funcs.getConfigVal("AUTO_SAVING_MIN_AMOUNT_PLACEHOLDER"))))
        {
            return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI CHECK MIN AMOUNT").Replace("{CIF_NO}", custid);
        }

        if (action.Equals("REGIST") || action.Equals("UPDATE"))
        {
            //string[] listCode = { "101", "121", "125", "126", "127", "135", "169", "171", "176", "183", "188", "140", "141" };
            string[] listCode = Funcs.getConfigVal("AUTO_SAVING_PRODCD_ACCT_VALID").Split(',');

            AccList.AcctListInqResType ret = new AccList.AcctListInqResType();

            string[] custIdArr = new string[1] { custid };

            ret = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.TYPE_CASA_PRODUCT);

            if (ret != null && ret.AcctRec != null)
            {
                foreach (var i in ret.AcctRec)
                {
                    string[] sourceAccountNmView = i.ProdDesc.Split('-');

                    if (i.BankAcctId.AcctId.Equals(legacyAc))
                    {
                        if (!listCode.ToList().Contains(sourceAccountNmView[0]))
                        {
                            return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", Funcs.GetError(Config.RES_AUTO_SAVING, "13")).Replace("{CIF_NO}", custid);
                        }

                        if (!i.AcctFinSts.Equals("NORMAL") && !i.AcctFinSts.Equals("NO_CREDIT"))
                        {
                            return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", Funcs.GetError(Config.RES_AUTO_SAVING, "11")).Replace("{CIF_NO}", custid);
                        }
                    }
                }
            }
        }

        if (!action.Equals("CANCEL") &&
            ((depositType.Equals("0") &&
                (Double.Parse(prinAmt) < Double.Parse(Funcs.getConfigVal("AUTO_SAVING_AMOUNT_PLACEHOLDER")))
            )
            ||
            (depositType.Equals("1") &&
                (Double.Parse(prinAmt) < Double.Parse(Funcs.getConfigVal("AUTO_SAVING_AMOUNT_FLEXIBLE_PLACEHOLDER")))
            ))
        )
        {
            return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI CHECK PRINT AMOUNT").Replace("{CIF_NO}", custid);
        }


        //KIEM TRA TAI KHOAN MO SO THUOC CIF
        bool check = Auth.CustIdMatchScrAcct(custid, legacyAc);
        if (!check)
        {
            return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI KIEM TRA TAI KHOAN MO SO THUOC CIF").Replace("{CIF_NO}", custid);
        }

        //get POS_CD
        string pos_cd = CoreIntegration.getPosCdByCif(custid, legacyAc);

        if (string.IsNullOrEmpty(pos_cd))
        {
            Funcs.WriteLog("CIF:" + custid + "|HANDLE_AUTO_SAVING COULD NOT GET POS_CD");

            return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI GET THONG IN POS_CD").Replace("{CIF_NO}", custid);
        }

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
        double eb_tran_id = 0;
        string retStr = "";

        if (check_before_trans == Config.ERR_CODE_DONE)
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
                , legacyAc//src_acct
                , "" //des_acct
                , Double.Parse(minBal) //amount
                , "VND" //ccy_cd
                , 1//convert rate
                , Double.Parse(prinAmt) //lcy_amount
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
                , depositType //bm7
                , "" //bm8
                , tenure //bm9
                , tenureUnit //bm10
                , matType //bm11
                , freqBooking //bm12
                , action //bm13
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
                , userAgent // ""//bm29                   
            );

            #endregion "insert TBL_EB_TRAN"
            //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
            if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
            {
                Funcs.WriteLog("CUSTID:" + custid + "|HANDLE AUTO SAVING:" + legacyAc
                    + "ACTION:" + tran_type + "INSERT TBL EB DONE --> CALL FUNCTION HANDLE AUTO SAVING"
                    );
                eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                AutoSavingModel model = new AutoSavingModel();
                string coreRef = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

                if (action.Equals("REGIST") || action.Equals("UPDATE"))
                {


                    model.REF_NO = coreRef;
                    model.CIF_NO = custid;
                    model.LEGACY_AC = legacyAc;
                    model.DEPOSIT_TYPE = Double.Parse(depositType);
                    model.CCY_CD = "704";
                    model.MIN_BAL = Double.Parse(minBal);
                    model.PRIN_AMT = Double.Parse(prinAmt);
                    model.TENURE = tenure;
                    model.TENURE_UNIT = tenureUnit;
                    model.MAT_TYPE = Double.Parse(matType);
                    model.FREQ_BOOKING =  (model.DEPOSIT_TYPE == 1 ? "D": freqBooking);

                    if (depositType.Equals("0"))
                    {
                        if (Funcs.CompeDateTime(Funcs.GetCoreDate("dd/MM/yyyy"), startDate))
                        {
                            model.START_DT = DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            model.START_DT = Funcs.GetCoreDate("dd/MM/yyyy");
                        }
                    }
                    else
                    {
                        model.START_DT = Funcs.GetCoreDate("dd/MM/yyyy");
                    }

                    model.LAST_BOOK_DT = DateTime.Now.ToString();
                    model.POS_CD = pos_cd;
                    model.SRC_REG = Config.ChannelID;
                    model.MKR_DT = DateTime.Now.ToString("dd/MM/yyyy");
                    model.MKR_ID = Config.ChannelIDVoucher;
                    model.AUTH_DT = DateTime.Now.ToString("dd/MM/yyyy");
                    model.AUTH_ID = Config.ChannelIDVoucher;

                    if (action.Equals("REGIST"))
                    {
                        TideAutoSaving.RegistResType res = CoreIntegration.REGIST_AUTO_SAVING(custid, model);

                        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals(Config.ERR_CODE_DONE))
                        {
                            resCode = Config.ERR_CODE_DONE;
                            resDesc = "SUCCESSFULL";
                            refNo = coreRef;
                        }
                        else
                        {
                            if (res != null && res.RespSts != null && !res.RespSts.Sts.Equals("0"))
                            {
                                resCode = Config.ERR_CODE_GENERAL;
                                resDesc = "HANDLE AUTO SAVING FAIL";
                                refNo = "";

                                for (int i = 0; i < Config.RES_AUTO_SAVING.Length; i++)
                                {
                                    if (Config.RES_AUTO_SAVING[i].Split('|')[0].ToString().Equals(res.errCode))
                                    {
                                        resCode = Config.ERR_CODE_GENERAL;
                                        resDesc = Funcs.GetError(Config.RES_AUTO_SAVING, res.errCode);
                                    }
                                }
                            }
                            else
                            {
                                resCode = Config.ERR_CODE_GENERAL;
                                resDesc = "HANDLE AUTO SAVING FAIL";
                                refNo = "";
                            }
                        }
                    }
                    else
                    {
                        TideAutoSaving.UpdateResType res = CoreIntegration.UPDATE_AUTO_SAVING(custid, model);

                        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals(Config.ERR_CODE_DONE))
                        {
                            resCode = Config.ERR_CODE_DONE;
                            resDesc = "SUCCESSFULL";
                            refNo = coreRef;
                        }
                        else
                        {
                            if (res != null && res.RespSts != null && !res.RespSts.Sts.Equals("0"))
                            {
                                resCode = Config.ERR_CODE_GENERAL;
                                resDesc = "HANDLE AUTO SAVING FAIL";
                                refNo = "";

                                for (int i = 0; i < Config.RES_AUTO_SAVING.Length; i++)
                                {
                                    if (Config.RES_AUTO_SAVING[i].Split('|')[0].ToString().Equals(res.errCode))
                                    {
                                        resCode = Config.ERR_CODE_GENERAL;
                                        resDesc = Funcs.GetError(Config.RES_AUTO_SAVING, res.errCode);
                                    }
                                }
                            }
                            else
                            {
                                resCode = Config.ERR_CODE_GENERAL;
                                resDesc = "HANDLE AUTO SAVING FAIL";
                                refNo = "";
                            }
                        }
                    }
                }
                else
                {

                    TideAutoSaving.GetDetailAutoSavingResType resInfo = CoreIntegration.GET_DETAIL_AUTO_SAVING(custid, legacyAc);

                    if (resInfo != null && resInfo.RespSts != null && resInfo.RespSts.Sts.Equals("0"))
                    {
                        TideAutoSaving.CancelResType res = CoreIntegration.CANCEL_AUTO_SAVING(custid, legacyAc);

                        if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0") && res.errCode.Equals(Config.ERR_CODE_DONE))
                        {
                            resCode = Config.ERR_CODE_DONE;
                            resDesc = "SUCCESSFULL";
                            refNo = coreRef;

                            model = new AutoSavingModel();
                            model.CIF_NO = resInfo.cifNo;
                            model.DEPOSIT_TYPE = Double.Parse(resInfo.depositType);
                            model.FREQ_BOOKING = resInfo.freqBooking;
                            model.LEGACY_AC = resInfo.legacyAc;
                            model.MAT_TYPE = Double.Parse(resInfo.matType);
                            model.MIN_BAL = Double.Parse(resInfo.minBal);
                            model.PRIN_AMT = Double.Parse(resInfo.prinAmt);
                            model.START_DT = DateTime.Parse(resInfo.startDt.ToString()).ToString("dd/MM/yyyy");
                            model.TENURE = resInfo.tenure;
                            model.TENURE_UNIT = resInfo.tenureUnit;
                        }
                        else
                        {
                            if (res != null && res.RespSts != null && !res.RespSts.Sts.Equals("0"))
                            {
                                resCode = Config.ERR_CODE_GENERAL;
                                resDesc = "HANDLE AUTO SAVING FAIL";
                                refNo = "";

                                for (int i = 0; i < Config.RES_AUTO_SAVING.Length; i++)
                                {
                                    if (Config.RES_AUTO_SAVING[i].Split('|')[0].ToString().Equals(res.errCode))
                                    {
                                        resCode = Config.ERR_CODE_GENERAL;
                                        resDesc = Funcs.GetError(Config.RES_AUTO_SAVING, res.errCode);
                                    }
                                }
                            }
                            else
                            {
                                resCode = Config.ERR_CODE_GENERAL;
                                resDesc = "HANDLE AUTO SAVING FAIL";
                                refNo = "";
                            }
                        }
                    }
                    else
                    {
                        return Config.ERR_MSG_GENERAL_ADDCIF.Replace("{ERR_DESC}", "LOI KIEM TRA THONG TIN DANG KY TRUOC KHI HUY").Replace("{CIF_NO}", custid);
                    }
                }

                if (resCode.Equals(Config.ERR_CODE_DONE))
                {
                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, refNo, DateTime.Now.ToString(), Config.ChannelID);
                    coreDT = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    //get Infor by CustId
                    TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                    DataSet ds = new DataSet();
                    ds = da.GET_USER_BY_CIF(custid);
                    DataTable dt = new DataTable();
                    if (ds != null && ds.Tables[0] != null)
                    {
                        string email = ds.Tables[0].Rows[0]["EMAIL"].ToString();

                        bool sendMail = sendMailAutoSaving(custid, ds.Tables[0].Rows[0]["CUSTNAME"].ToString(), email, model, action, refNo, coreDT, eb_tran_id);
                    }
                    else
                    {
                        Funcs.WriteLog("custid:" + custid + "|HANDLE AUTO SAVING SEND MAIL FAIL. GET INFO FAIL");
                    }

                }
                else
                {
                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                }

                resp = resp.Replace("{ERR_CODE}", resCode);
                resp = resp.Replace("{ERR_DESC}", resDesc);
                resp = resp.Replace("{CIF_NO}", custid);
                resp = resp.Replace("{REF_NO}", refNo);
                resp = resp.Replace("{CORE_DT}", coreDT);
                resp = resp.Replace("{START_DATE}", model.START_DT);

                return resp;
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
            Funcs.WriteLog("custid:" + custid + "|HANDLE AUTO SAVING INVALID TRANPWD END");
            return retStr;
        }
    }

    public string GET_LIST_AUTO_SAVING(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.RESPONE_GET_LIST_AUTO_SAVING;
        string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string depositType = Funcs.getValFromHashtbl(hashTbl, "DEPOSIT_TYPE");

        try
        {
            ListAutoSavingModel model = CoreIntegration.GET_LIST_AUTO_SAVING(custid, depositType);

            if (model != null && model.ErrCode.Equals(Config.ERR_CODE_DONE))
            {
                string strTemp = string.Empty;
                foreach (var item in model.ListAutoSaving)
                {
                    strTemp = strTemp
                        + item.LEGACY_AC
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(item.MIN_BAL.ToString()) ? "0" : item.MIN_BAL.ToString())
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(item.PRIN_AMT.ToString()) ? "0" : item.PRIN_AMT.ToString())
                        + Config.ROW_REC_DLMT;
                }

                strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{ERR_DESC}", "GET LIST AUTO SAVING SUCCESSFUL");
                retStr = retStr.Replace("{RECORD}", strTemp.ToString());

                Funcs.WriteLog("GET LIST AUTO SAVING SUCCESS: ");
            }
            else
            {
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{ERR_DESC}", "GET LIST AUTO SAVING FAIL");
                retStr = retStr.Replace("{RECORD}", "");
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GET LIST AUTO SAVING EXCEPTION: " + ex.ToString());
            retStr = Config.ERR_MSG_GENERAL;
        }

        return retStr;
    }

    public string GET_HIST_AUTO_SAVING(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.RESPONE_GET_LIST_AUTO_SAVING;
        string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string legacyAc = Funcs.getValFromHashtbl(hashTbl, "LEGACY_AC");
        string enqType = Funcs.getValFromHashtbl(hashTbl, "ENQ_TYPE");
        string fromDt = Funcs.getValFromHashtbl(hashTbl, "FROM_DATE");
        string toDt = Funcs.getValFromHashtbl(hashTbl, "TO_DATE");

        try
        {
            ListHistAutoSavingModel model = CoreIntegration.GET_HIST_AUTO_SAVING(custid, legacyAc, enqType, fromDt, toDt);

            if (model != null && model.ErrCode.Equals(Config.ERR_CODE_DONE))
            {
                string strTemp = string.Empty;
                foreach (var item in model.ListHistAutoSaving)
                {
                    DateTime openDate = DateTime.Parse(item.OPEN_DATE.ToString());
                    strTemp = strTemp
                        + openDate.ToString("dd/MM/yyyy")
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(item.ACCOUNT_NUMBER) ? "" : item.ACCOUNT_NUMBER)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(item.AMOUNT.ToString()) ? "0" : item.AMOUNT.ToString())
                        + Config.ROW_REC_DLMT;
                }

                strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{ERR_DESC}", "GET HIST AUTO SAVING SUCCESSFUL");
                retStr = retStr.Replace("{RECORD}", strTemp.ToString());

                Funcs.WriteLog("GET HIST AUTO SAVING SUCCESS: ");
            }
            else
            {
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{ERR_DESC}", "GET LIST AUTO SAVING FAIL");
                retStr = retStr.Replace("{RECORD}", "");
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GET HIST AUTO SAVING EXCEPTION: " + ex.ToString());
            retStr = Config.ERR_MSG_GENERAL;
        }

        return retStr;
    }

    public string GET_DETAIL_AUTO_SAVING(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.RESPONE_GET_LIST_AUTO_SAVING;
        string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string legacyAc = Funcs.getValFromHashtbl(hashTbl, "LEGACY_AC");

        try
        {
            TideAutoSaving.GetDetailAutoSavingResType res = CoreIntegration.GET_DETAIL_AUTO_SAVING(custid, legacyAc);

            if (res != null && res.RespSts != null && res.RespSts.Sts.Equals("0"))
            {
                string strTemp = string.Empty;

                strTemp = strTemp
                        + (string.IsNullOrEmpty(res.cifNo) ? "" : res.cifNo)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.legacyAc) ? "" : res.legacyAc)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.acName) ? "" : res.acName)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.depositType) ? "" : res.depositType)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.ccyCd) ? "" : res.ccyCd)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.minBal) ? "" : res.minBal)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.prinAmt) ? "" : res.prinAmt)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.tenure) ? "" : res.tenure)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.tenureUnit) ? "" : res.tenureUnit)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.matType) ? "" : res.matType)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.freqBooking) ? "" : res.freqBooking)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.startDt) ? "" : DateTime.Parse(res.startDt.ToString()).ToString("dd/MM/yyyy"))
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.lastBookDt) ? "" : DateTime.Parse(res.lastBookDt.ToString()).ToString("dd/MM/yyyy"))
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.posCd) ? "" : res.posCd)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.regSt) ? "" : res.regSt)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.srcReg) ? "" : res.srcReg)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.mkrId) ? "" : res.mkrId)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.mkrDt) ? "" : DateTime.Parse(res.mkrDt.ToString()).ToString("dd/MM/yyyy"))
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.authId) ? "" : res.authId)
                        + Config.COL_REC_DLMT
                        + (string.IsNullOrEmpty(res.authDt) ? "" : DateTime.Parse(res.authDt.ToString()).ToString("dd/MM/yyyy"))
                ;

                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{ERR_DESC}", "GET DETAIL AUTO SAVING SUCCESSFUL");
                retStr = retStr.Replace("{RECORD}", strTemp.ToString());

                Funcs.WriteLog("GET DETAIL AUTO SAVING SUCCESS: ");
            }
            else
            {
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                retStr = retStr.Replace("{CIF_NO}", custid);
                retStr = retStr.Replace("{ERR_DESC}", "GET DETAIL AUTO SAVING FAIL");
                retStr = retStr.Replace("{RECORD}", "");
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GET DETAIL AUTO SAVING EXCEPTION: " + ex.ToString());
            retStr = Config.ERR_MSG_GENERAL;
        }

        return retStr;
    }

    public bool sendMailAutoSaving(string custId, string custName, string email, AutoSavingModel modelSaving, string action, string refNo, string coreDT, double tranId)
    {
        string pathFile = String.Empty;
        string strContent = String.Empty;
        string title = String.Empty;

        if (action.Equals("REGIST") || action.Equals("UPDATE"))
        {
            if (modelSaving.DEPOSIT_TYPE == 0)
            {
                title = "DANG KY/THAY DOI DICH VU TIET KIEM TU DONG CO DINH";
                pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\REGIST_UPDATE_AUTO_FIXED.html";
                strContent = Funcs.ReadAllFile(pathFile);

                strContent = strContent.Replace("P_CUSTNAME_P", custName.Trim());
                strContent = strContent.Replace("P_ACCOUNTNUMBER_P", modelSaving.LEGACY_AC.Trim());
                strContent = strContent.Replace("P_AMOUNT_MIN_P", Funcs.ConvertMoney(modelSaving.MIN_BAL.ToString()).Trim());
                strContent = strContent.Replace("P_TRAN_DATE_P", coreDT);
                strContent = strContent.Replace("P_REF_NO_P", refNo.Trim());

                string nameEN = String.Empty;
                string nameVN = String.Empty;

                switch (modelSaving.FREQ_BOOKING.ToUpper().Trim())
                {
                    case "O":
                        {
                            nameEN = "One time";
                            nameVN = "Một lần";
                            break;
                        }
                    case "W":
                        {
                            nameEN = "Weekly";
                            nameVN = "Hàng tuần";
                            break;
                        }
                    case "M":
                        {
                            nameEN = "Monthly";
                            nameVN = "Hàng tháng";
                            break;
                        }
                }

                strContent = strContent.Replace("P_FREQ_BOOKING_VN_P", nameVN.Trim());
                strContent = strContent.Replace("P_FREQ_BOOKING_EN_P", nameEN.Trim());

                //String[] tenures = model.depositModel.tenureCb.Split('|');

                string TERM_VN = String.Empty;
                string TERM_EN = String.Empty;

                switch (modelSaving.TENURE_UNIT.ToUpper())
                {
                    case "D":
                        {
                            TERM_VN = "ngày";
                            TERM_EN = "day";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case "M":
                        {
                            TERM_VN = "tháng";
                            TERM_EN = "month";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case "Y":
                        {
                            TERM_VN = "năm";
                            TERM_EN = "year";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }


                strContent = strContent.Replace("P_TERM_VN_P", modelSaving.TENURE + " " + TERM_VN);
                strContent = strContent.Replace("P_TERM_EN_P", modelSaving.TENURE + " " + TERM_EN);

                strContent = strContent.Replace("P_AMOUNT_P", Funcs.ConvertMoney(modelSaving.PRIN_AMT.ToString()).Trim());

                string display_mat_type_VN = String.Empty;
                string display_mat_type_EN = String.Empty;

                switch (Int32.Parse(modelSaving.MAT_TYPE.ToString()))
                {
                    case 1:
                        {
                            display_mat_type_VN = "Chuyển gốc và lãi sang kỳ hạn mới";
                            display_mat_type_EN = "Principal & Interest Roll Over";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case 2:
                        {
                            display_mat_type_VN = "Chuyển gốc sang kỳ hạn mới, lãi trả vào TKTT";
                            display_mat_type_EN = "Principal Roll Over & Interest is credited to current account";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case 3:
                        {
                            display_mat_type_VN = "Không gửi tiếp, chuyển gốc và lãi trả vào TKTT";
                            display_mat_type_EN = "Do not roll ever, both principal and interest are credited to current account";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }

                strContent = strContent.Replace("P_MAT_TYPE_VN_P", display_mat_type_VN);
                strContent = strContent.Replace("P_MAT_TYPE_EN_P", display_mat_type_EN);
                strContent = strContent.Replace("P_START_DATE_P", modelSaving.START_DT.Trim());

            }
            else
            {
                title = "DANG KY/THAY DOI DICH VU TIET KIEM TU DONG LINH HOAT";
                pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\REGIST_UPDATE_FLEXIBLE.html";
                strContent = Funcs.ReadAllFile(pathFile);

                strContent = strContent.Replace("P_CUSTNAME_P", custName.Trim());
                strContent = strContent.Replace("P_ACCOUNTNUMBER_P", modelSaving.LEGACY_AC.Trim());
                strContent = strContent.Replace("P_AMOUNT_MIN_P", Funcs.ConvertMoney(modelSaving.MIN_BAL.ToString()).Trim());
                strContent = strContent.Replace("P_TRAN_DATE_P", coreDT);
                strContent = strContent.Replace("P_REF_NO_P", refNo.Trim());

                string nameEN = String.Empty;
                string nameVN = String.Empty;

                switch (modelSaving.FREQ_BOOKING.ToUpper().Trim())
                {
                    case "O":
                        {
                            nameEN = "One time";
                            nameVN = "Một lần";
                            break;
                        }
                    case "W":
                        {
                            nameEN = "Weekly";
                            nameVN = "Hàng tuần";
                            break;
                        }
                    case "M":
                        {
                            nameEN = "Monthly";
                            nameVN = "Hàng tháng";
                            break;
                        }
                }

                //String[] tenures = model.depositModel.tenureCb.Split('|');

                string TERM_VN = String.Empty;
                string TERM_EN = String.Empty;

                switch (modelSaving.TENURE_UNIT.ToUpper())
                {
                    case "D":
                        {
                            TERM_VN = "ngày";
                            TERM_EN = "day";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case "M":
                        {
                            TERM_VN = "tháng";
                            TERM_EN = "month";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case "Y":
                        {
                            TERM_VN = "năm";
                            TERM_EN = "year";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }


                strContent = strContent.Replace("P_TERM_VN_P", modelSaving.TENURE + " " + TERM_VN);
                strContent = strContent.Replace("P_TERM_EN_P", modelSaving.TENURE + " " + TERM_EN);

                strContent = strContent.Replace("P_AMOUNT_P", Funcs.ConvertMoney(modelSaving.PRIN_AMT.ToString()).Trim());

                string display_mat_type_VN = String.Empty;
                string display_mat_type_EN = String.Empty;

                switch (Int32.Parse(modelSaving.MAT_TYPE.ToString()))
                {
                    case 1:
                        {
                            display_mat_type_VN = "Chuyển gốc và lãi sang kỳ hạn mới";
                            display_mat_type_EN = "Principal & Interest Roll Over";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case 2:
                        {
                            display_mat_type_VN = "Chuyển gốc sang kỳ hạn mới, lãi trả vào TKTT";
                            display_mat_type_EN = "Principal Roll Over & Interest is credited to current account";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case 3:
                        {
                            display_mat_type_VN = "Không gửi tiếp, chuyển gốc và lãi trả vào TKTT";
                            display_mat_type_EN = "Do not roll ever, both principal and interest are credited to current account";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }

                strContent = strContent.Replace("P_MAT_TYPE_VN_P", display_mat_type_VN);
                strContent = strContent.Replace("P_MAT_TYPE_EN_P", display_mat_type_EN);
                strContent = strContent.Replace("P_START_DATE_P", modelSaving.START_DT.Trim());

            }
        }
        else
        {
            if (modelSaving.DEPOSIT_TYPE == 0)
            {
                title = "HUY DICH VU TIET KIEM CO DINH";
                pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\CANCEL_AUTO_FIXED.html";
                strContent = Funcs.ReadAllFile(pathFile);

                strContent = strContent.Replace("P_CUSTNAME_P", custName.Trim());
                strContent = strContent.Replace("P_ACCOUNTNUMBER_P", modelSaving.LEGACY_AC.Trim());
                strContent = strContent.Replace("P_AMOUNT_MIN_P", Funcs.ConvertMoney(modelSaving.MIN_BAL.ToString()).Trim());
                strContent = strContent.Replace("P_TRAN_DATE_P", coreDT);
                strContent = strContent.Replace("P_REF_NO_P", refNo.Trim());

                string nameEN = String.Empty;
                string nameVN = String.Empty;

                switch (modelSaving.FREQ_BOOKING.ToUpper().Trim())
                {
                    case "O":
                        {
                            nameEN = "One time";
                            nameVN = "Một lần";
                            break;
                        }
                    case "W":
                        {
                            nameEN = "Weekly";
                            nameVN = "Hàng tuần";
                            break;
                        }
                    case "M":
                        {
                            nameEN = "Monthly";
                            nameVN = "Hàng tháng";
                            break;
                        }
                }

                strContent = strContent.Replace("P_FREQ_BOOKING_VN_P", nameVN.Trim());
                strContent = strContent.Replace("P_FREQ_BOOKING_EN_P", nameEN.Trim());

                //String[] tenures = model.depositModel.tenureCb.Split('|');

                string TERM_VN = String.Empty;
                string TERM_EN = String.Empty;

                switch (modelSaving.TENURE_UNIT.ToUpper())
                {
                    case "D":
                        {
                            TERM_VN = "ngày";
                            TERM_EN = "day";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case "M":
                        {
                            TERM_VN = "tháng";
                            TERM_EN = "month";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case "Y":
                        {
                            TERM_VN = "năm";
                            TERM_EN = "year";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }


                strContent = strContent.Replace("P_TERM_VN_P", modelSaving.TENURE + " " + TERM_VN);
                strContent = strContent.Replace("P_TERM_EN_P", modelSaving.TENURE + " " + TERM_EN);

                strContent = strContent.Replace("P_AMOUNT_P", Funcs.ConvertMoney(modelSaving.PRIN_AMT.ToString()).Trim());

                string display_mat_type_VN = String.Empty;
                string display_mat_type_EN = String.Empty;

                switch (Int32.Parse(modelSaving.MAT_TYPE.ToString()))
                {
                    case 1:
                        {
                            display_mat_type_VN = "Chuyển gốc và lãi sang kỳ hạn mới";
                            display_mat_type_EN = "Principal & Interest Roll Over";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case 2:
                        {
                            display_mat_type_VN = "Chuyển gốc sang kỳ hạn mới, lãi trả vào TKTT";
                            display_mat_type_EN = "Principal Roll Over & Interest is credited to current account";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case 3:
                        {
                            display_mat_type_VN = "Không gửi tiếp, chuyển gốc và lãi trả vào TKTT";
                            display_mat_type_EN = "Do not roll ever, both principal and interest are credited to current account";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }

                strContent = strContent.Replace("P_MAT_TYPE_VN_P", display_mat_type_VN);
                strContent = strContent.Replace("P_MAT_TYPE_EN_P", display_mat_type_EN);
                strContent = strContent.Replace("P_START_DATE_P", modelSaving.START_DT.Trim());

            }
            else
            {
                title = "HUY DICH VU TIET KIEM LINH HOAT";
                pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\CANCEL_AUTO_FLEXIBLE.html";
                strContent = Funcs.ReadAllFile(pathFile);

                strContent = strContent.Replace("P_CUSTNAME_P", custName.Trim());
                strContent = strContent.Replace("P_ACCOUNTNUMBER_P", modelSaving.LEGACY_AC.Trim());
                strContent = strContent.Replace("P_AMOUNT_MIN_P", Funcs.ConvertMoney(modelSaving.MIN_BAL.ToString()).Trim());
                strContent = strContent.Replace("P_TRAN_DATE_P", coreDT);
                strContent = strContent.Replace("P_REF_NO_P", refNo.Trim());

                string nameEN = String.Empty;
                string nameVN = String.Empty;

                switch (modelSaving.FREQ_BOOKING.ToUpper().Trim())
                {
                    case "O":
                        {
                            nameEN = "One time";
                            nameVN = "Một lần";
                            break;
                        }
                    case "W":
                        {
                            nameEN = "Weekly";
                            nameVN = "Hàng tuần";
                            break;
                        }
                    case "M":
                        {
                            nameEN = "Monthly";
                            nameVN = "Hàng tháng";
                            break;
                        }
                }

                //String[] tenures = model.depositModel.tenureCb.Split('|');

                string TERM_VN = String.Empty;
                string TERM_EN = String.Empty;

                switch (modelSaving.TENURE_UNIT.ToUpper())
                {
                    case "D":
                        {
                            TERM_VN = "ngày";
                            TERM_EN = "day";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case "M":
                        {
                            TERM_VN = "tháng";
                            TERM_EN = "month";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case "Y":
                        {
                            TERM_VN = "năm";
                            TERM_EN = "year";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }


                strContent = strContent.Replace("P_TERM_VN_P", modelSaving.TENURE + " " + TERM_VN);
                strContent = strContent.Replace("P_TERM_EN_P", modelSaving.TENURE + " " + TERM_EN);

                strContent = strContent.Replace("P_AMOUNT_P", Funcs.ConvertMoney(modelSaving.PRIN_AMT.ToString()).Trim());

                string display_mat_type_VN = String.Empty;
                string display_mat_type_EN = String.Empty;

                switch (Int32.Parse(modelSaving.MAT_TYPE.ToString()))
                {
                    case 1:
                        {
                            display_mat_type_VN = "Chuyển gốc và lãi sang kỳ hạn mới";
                            display_mat_type_EN = "Principal & Interest Roll Over";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Roll;

                    case 2:
                        {
                            display_mat_type_VN = "Chuyển gốc sang kỳ hạn mới, lãi trả vào TKTT";
                            display_mat_type_EN = "Principal Roll Over & Interest is credited to current account";
                            break;
                        }
                    //display_mat_type = Ibanking.deposit_Roll_Casa;
                    case 3:
                        {
                            display_mat_type_VN = "Không gửi tiếp, chuyển gốc và lãi trả vào TKTT";
                            display_mat_type_EN = "Do not roll ever, both principal and interest are credited to current account";
                            break;
                        }
                        //display_mat_type = Ibanking.deposit_Casa_Casa;
                }

                strContent = strContent.Replace("P_MAT_TYPE_VN_P", display_mat_type_VN);
                strContent = strContent.Replace("P_MAT_TYPE_EN_P", display_mat_type_EN);
                strContent = strContent.Replace("P_START_DATE_P", modelSaving.START_DT.Trim());

            }
        }

        AlternateView view = AlternateView.CreateAlternateViewFromString(strContent, null, System.Net.Mime.MediaTypeNames.Text.Html);

        string mails = Funcs.getConfigVal("AUTO_SAVING_LISTMAI_PILOT");

        if (mails !=null && mails.Length > 0)
        {
            string[] listMail = mails.Split(',');

            if (listMail.ToList().Contains(email))
            {

                return new PushNotification().sendPushEmail(custId, email, title + " - " + custId, strContent, "EMAIL", tranId, refNo.Trim());
            }
            else
            {
                return true;
            }
        }
        else
        {
            return new PushNotification().sendPushEmail(custId, email, title + " - " + custId, strContent, "EMAIL", tranId, refNo.Trim());
        }
    }

}