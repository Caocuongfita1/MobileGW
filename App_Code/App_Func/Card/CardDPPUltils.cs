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
using CardDPP;

namespace mobileGW.Service.AppFuncs
{
    /// <summary>
    /// Summary description for Auth
    /// </summary>
    public class CardDPPUltils
    {
        public CardDPPUltils()
        {

        }

        public string RESGISTER_CARD_DPP(Hashtable hashTbl, string ip, string user_agent)
        {
            #region "CMD RESGISTER_CARD_DPP"
            String retStr = Config.RESPONE_RESGISTER_CARD_DPP;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String tranId = Funcs.getValFromHashtbl(hashTbl, "TRANS_ID");
            String cardNo = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
            String periodId = Funcs.getValFromHashtbl(hashTbl, "PERIOD_ID");
            String amount = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");
            String phone = Funcs.getValFromHashtbl(hashTbl, "PHONE");
            String email = Funcs.getValFromHashtbl(hashTbl, "EMAIL");
            String installmentPeriod = Funcs.getValFromHashtbl(hashTbl, "INSTALLMENTPERIOD");
            String amountMonth = Funcs.getValFromHashtbl(hashTbl, "AMOUNTMONTH");
            String conversionFee = Funcs.getValFromHashtbl(hashTbl, "CONVERSIONFEE");
            String conversionFeeAmount = Funcs.getValFromHashtbl(hashTbl, "CONVERSIONFEEAMOUNT");
            String interestRate = Funcs.getValFromHashtbl(hashTbl, "INTERESTRATE");

            string core_txno_ref = "";
            string retPartnerWS = "";
            string core_txdate_ref = "";

            string errCode = "";
            string errDesc = "";
            double eb_tran_id = 0;

            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");
            string check_before_trans = "";
            string tran_type = Config.TransType.TRAN_TYPE_CARDDPP;

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
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, "", "", Double.Parse(amount), pwd, tran_type);
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
                #region "insert TBL_EB_TRAN"
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , Config.TransType.TRAN_TYPE_CARDDPP //tran_type
                    , custid //custid
                    , cardNo//src_acct
                    , "" //des_acct
                    , Double.Parse(amount) //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , Double.Parse(amount) //lcy_amount
                    , "RESGISTER_CARD_DPP"
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
                    , Double.Parse(amount) // order_amount
                    , Double.Parse(amount) // order_dis
                    , tranId //order_id
                    , Funcs.getConfigVal("CARD_DPP_PARTNER") //partner code
                    , "" //category code
                    , "" //service code
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
                    , periodId //bm1 --> periodId
                    , "" //bm2 --> service_id
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , "" //bm7
                    , "" //bm8
                    , "" //bm9
                    , tranId //bm10
                    , periodId //bm11
                    , installmentPeriod //bm12
                    , amountMonth  //bm13
                    , conversionFee  //bm14
                    , conversionFeeAmount //bm15
                    , interestRate //bm16
                    , phone //bm17
                    , email //bm18
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

                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {
                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    core_txno_ref = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');
                    core_txdate_ref = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

                    try
                    {
                        CardDPP.RegisterTranResType res = new CardDPPIntegration().InsertCardInstallmentPeriod(custid, tranId, cardNo, periodId, amount, phone, email);

                        if (res != null && res.errorCode != null && res.RespSts.Sts != null && res.errorCode.Equals("00"))
                        {

                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, Config.ChannelID);

                            retStr = retStr.Replace("{ERR_CODE}", Config.RET_CARD_DPP_RES_CODE[0].Split('|')[0].ToString());
                            retStr = retStr.Replace("{ERR_DESC}", Config.RET_CARD_DPP_RES_CODE[0].Split('|')[1].ToString()
                                + Config.COL_REC_DLMT
                                + Config.RET_CARD_DPP_RES_CODE[0].Split('|')[2].ToString());
                            retStr = retStr.Replace("{REF_NO}", core_txno_ref);
                            retStr = retStr.Replace("{REF_DATE}", core_txdate_ref);

                            return retStr;
                        }
                        else
                        {
                            for (int i = 1; i < Config.RET_CARD_DPP_RES_CODE.Length; i++)
                            {
                                if (Config.RET_CARD_DPP_RES_CODE[i].Split('|')[0].Equals(res.errorCode))
                                {
                                    errCode = Config.RET_CARD_DPP_RES_CODE[i].Split('|')[0].ToString();
                                    errDesc = Config.RET_CARD_DPP_RES_CODE[i].Split('|')[1].ToString()
                                        + Config.COL_REC_DLMT
                                        + Config.RET_CARD_DPP_RES_CODE[i].Split('|')[2].ToString();
                                }
                            }

                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);

                            retStr = retStr.Replace("{ERR_CODE}", errCode);
                            retStr = retStr.Replace("{ERR_DESC}", errDesc);

                            Funcs.WriteLog("RESGISTER_CARD_DPP|CIF:" + custid + " ERROR");

                            return retStr;
                        }

                        errCode = Config.RET_CARD_DPP_RES_CODE[1].Split('|')[0].ToString();
                        errDesc = Config.RET_CARD_DPP_RES_CODE[1].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.RET_CARD_DPP_RES_CODE[1].Split('|')[2].ToString();

                        retStr = retStr.Replace("{ERR_CODE}", errCode);
                        retStr = retStr.Replace("{ERR_DESC}", errDesc);

                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);

                        return retStr;
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("CIF: " + custid + "|RESGISTER_CARD_DPP :" + ex.ToString());
                        errCode = Config.RET_CARD_DPP_RES_CODE[1].Split('|')[0].ToString();
                        errDesc = Config.RET_CARD_DPP_RES_CODE[1].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.RET_CARD_DPP_RES_CODE[1].Split('|')[2].ToString();

                        retStr = retStr.Replace("{ERR_CODE}", errCode);
                        retStr = retStr.Replace("{ERR_DESC}", errDesc);

                        return retStr;
                    }
                }
                else
                {
                    //insert trans fail
                    Funcs.WriteLog("CIF: " + custid + "|RESGISTER_CARD_DPP : INSERT TRANS FAIL");
                    errCode = Config.RET_CARD_DPP_RES_CODE[1].Split('|')[0].ToString();
                    errDesc = Config.RET_CARD_DPP_RES_CODE[1].Split('|')[1].ToString()
                        + Config.COL_REC_DLMT
                        + Config.RET_CARD_DPP_RES_CODE[1].Split('|')[2].ToString();

                    retStr = retStr.Replace("{ERR_CODE}", errCode);
                    retStr = retStr.Replace("{ERR_DESC}", errDesc);

                    return retStr;
                }
            }
            else
            {
                //retStr = Config.ERR_MSG_GENERAL;
                retStr = check_before_trans;
                Funcs.WriteLog("custid:" + custid + "BILL CHECK PWD FAILED");
            }

            return retStr;

            #endregion "CMD RESGISTER_CARD_DPP"
        }

        //getCardInstallmentPeriod
        public string GET_CARD_INSTALLMENT_PERIOD(Hashtable hashTbl)
        {
            #region "CMD GET_CARD_INSTALLMENT_PERIOD"
            String retStr = Config.RESPONE_GET_CARD_INSTALLMENT_PERIOD_CARD_DPP;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String tranId = Funcs.getValFromHashtbl(hashTbl, "TRANS_ID");
            String cardNo = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
            String amount = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");

            string errCode = "";
            string errDesc = "";
            string strTemp = "";
            string InterestAndFeeDetail = "";
            try
            {

                CardDPP.GetInterestAndFeeResType res = new CardDPPIntegration().GetCardInstallmentPeriod(custid, tranId, cardNo, amount);

                if (res != null && res.RespSts.Sts.Equals("0"))
                {
                    List<CardDPP.InterestAndFeeListType> lstInterestAndFee = new List<CardDPP.InterestAndFeeListType>(res.InterestAndFeeList);

                    if (lstInterestAndFee != null && lstInterestAndFee.Count > 0)
                    {
                        foreach (var item in lstInterestAndFee)
                        {
                            strTemp += (string.IsNullOrEmpty(item.amountTotal) ? "0" : item.amountTotal)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.periodId) ? "" : item.periodId)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.installmentPeriod) ? "" : item.installmentPeriod)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.amountMonth) ? "" : item.amountMonth)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.amountMonthEnd) ? "" : item.amountMonthEnd)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.conversionFee) ? "" : item.conversionFee)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.interestRate) ? "" : item.interestRate)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.installmentPeriodStart) ? "" : item.installmentPeriodStart)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.installmentPeriodEnd) ? "" : item.installmentPeriodEnd)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.conversionFeeAmount) ? "" : item.conversionFeeAmount)
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                        }

                        InterestAndFeeDetail = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    }

                    retStr = retStr.Replace("{ERR_CODE}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[0].ToString());
                    retStr = retStr.Replace("{ERR_DESC}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[1].ToString()
                        + Config.COL_REC_DLMT
                        + Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[2].ToString());

                    retStr = retStr.Replace("{INSTALLMENT_PERIOD}", InterestAndFeeDetail);

                    return retStr;
                }
                else
                {
                    for (int i = 1; i < Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length; i++)
                    {
                        if (Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].Equals(res.RespSts.ErrCd))
                        {
                            errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].ToString();
                            errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[1].ToString()
                                + Config.COL_REC_DLMT
                                + Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[2].ToString();
                        }
                    }

                    retStr = retStr.Replace("{ERR_CODE}", errCode);
                    retStr = retStr.Replace("{ERR_DESC}", errDesc);

                    Funcs.WriteLog("GET_CARD_INSTALLMENT_PERIOD|CIF:" + custid + " ERROR CODE: " + res.RespSts.ErrCd);

                    return retStr;
                }

                Funcs.WriteLog("GET_CARD_INSTALLMENT_PERIOD|CIF:" + custid + " ERROR");

                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF: " + custid + "|GET_CARD_INSTALLMENT_PERIOD :" + ex.ToString());
                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            #endregion "CMD GET_CARD_INSTALLMENT_PERIOD"
        }

        //getCardInstallmentPeriodByTran
        public string GET_CARD_INSTALLMENT_PERIOD_BY_TRAN(Hashtable hashTbl)
        {
            #region "CMD GET_CARD_INSTALLMENT_PERIOD_BY_TRAN"
            String retStr = Config.RESPONE_GET_CARD_INSTALLMENT_PERIOD_CARD_DPP;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String tranId = Funcs.getValFromHashtbl(hashTbl, "TRANS_ID");
            String cardNo = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");

            string errCode = "";
            string errDesc = "";
            string strTemp = "";

            try
            {

                CardDPP.GetCardInstallmentPeriodByTranResType res = new CardDPPIntegration().GetCardInstallmentPeriodByTran(custid, tranId, cardNo);

                if (res != null && res.RespSts.Sts.Equals("0"))
                {


                    strTemp += (string.IsNullOrEmpty(res.tranId) ? "" : res.tranId)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.amtOrg) ? "" : res.amtOrg)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.finDt) ? "" : res.finDt)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.periodId) ? "" : res.periodId)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.installmentPeriod) ? "" : res.installmentPeriod)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.amountMonth) ? "" : res.amountMonth)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.amountMonthEnd) ? "" : res.amountMonthEnd)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.conversionFee) ? "" : res.conversionFee)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.interestRate) ? "" : res.interestRate)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.installmentPeriodStart) ? "" : res.installmentPeriodStart)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.installmentPeriodEnd) ? "" : res.installmentPeriodEnd)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(res.conversionFeeAmount) ? "" : res.conversionFeeAmount);

                    retStr = retStr.Replace("{ERR_CODE}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[0].ToString());
                    retStr = retStr.Replace("{ERR_DESC}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[1].ToString()
                        + Config.COL_REC_DLMT
                        + Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[2].ToString());

                    retStr = retStr.Replace("{INSTALLMENT_PERIOD}", strTemp);

                    return retStr;
                }
                else
                {
                    for (int i = 1; i < Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length; i++)
                    {
                        if (Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].Equals(res.RespSts.ErrCd))
                        {
                            errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].ToString();
                            errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[1].ToString()
                                + Config.COL_REC_DLMT
                                + Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[2].ToString();
                        }
                    }

                    retStr = retStr.Replace("{ERR_CODE}", errCode);
                    retStr = retStr.Replace("{ERR_DESC}", errDesc);

                    Funcs.WriteLog("GET_CARD_INSTALLMENT_PERIOD_BY_TRAN|CIF:" + custid + " ERROR CODE: " + res.RespSts.ErrCd);

                    return retStr;
                }

                Funcs.WriteLog("GET_CARD_INSTALLMENT_PERIOD_BY_TRAN|CIF:" + custid + " ERROR");

                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF: " + custid + "|GET_CARD_INSTALLMENT_PERIOD_BY_TRAN :" + ex.ToString());
                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            #endregion "CMD GET_CARD_INSTALLMENT_PERIOD_BY_TRAN"
        }

        //getCardInstallmentSchedule
        public string GET_CARD_INSTALLMENT_SCHEDULE(Hashtable hashTbl)
        {
            #region "CMD GET_CARD_INSTALLMENT_SCHEDULE"
            String retStr = Config.RESPONE_GET_CARD_INSTALLMENT_SCHEDULE_CARD_DPP;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String tranId = Funcs.getValFromHashtbl(hashTbl, "TRANS_ID");
            String cardNo = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
            String periodId = Funcs.getValFromHashtbl(hashTbl, "PERIOD_ID");
            String amount = Funcs.getValFromHashtbl(hashTbl, "AMOUNT");

            string errCode = "";
            string errDesc = "";
            string strTemp = "";
            string InstallmentScheduleDetail = "";
            try
            {

                CardDPP.GetScheduleListResType res = new CardDPPIntegration().GetCardInstallmentSchedule(custid, tranId, cardNo, periodId, amount);

                if (res != null && res.RespSts.Sts.Equals("0"))
                {
                    List<CardDPP.ScheduleListType> lstSchedule = new List<CardDPP.ScheduleListType>(res.ScheduleList);

                    if (lstSchedule != null && lstSchedule.Count > 0)
                    {
                        foreach (var item in lstSchedule)
                        {
                            strTemp += (string.IsNullOrEmpty(item.periodId) ? "" : item.periodId)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.paymentDate) ? "" : item.paymentDate)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.paymentAmount) ? "" : item.paymentAmount)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.outstandingPrincipal) ? "" : item.outstandingPrincipal)
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                        }

                        InstallmentScheduleDetail = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    }

                    retStr = retStr.Replace("{ERR_CODE}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[0].ToString());
                    retStr = retStr.Replace("{ERR_DESC}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[1].ToString()
                        + Config.COL_REC_DLMT
                        + Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[2].ToString());

                    retStr = retStr.Replace("{INSTALLMENT_SCHEDULE}", InstallmentScheduleDetail);

                    return retStr;
                }
                else
                {
                    for (int i = 1; i < Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length; i++)
                    {
                        if (Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].Equals(res.RespSts.ErrCd))
                        {
                            errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].ToString();
                            errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[1].ToString()
                                + Config.COL_REC_DLMT
                                + Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[2].ToString();
                        }
                    }

                    retStr = retStr.Replace("{ERR_CODE}", errCode);
                    retStr = retStr.Replace("{ERR_DESC}", errDesc);

                    Funcs.WriteLog("GET_CARD_INSTALLMENT_SCHEDULE|CIF:" + custid + " ERROR CODE: " + res.RespSts.ErrCd);

                    return retStr;
                }

                Funcs.WriteLog("GET_CARD_INSTALLMENT_SCHEDULE|CIF:" + custid + " ERROR");

                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF: " + custid + "|GET_CARD_INSTALLMENT_SCHEDULE :" + ex.ToString());
                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            #endregion "CMD GET_CARD_INSTALLMENT_SCHEDULE"
        }

        //getCardInstallmentPeriodByTran
        public string GET_CARD_HIS(Hashtable hashTbl)
        {
            #region "CMD GET_CARD_HIS"
            String retStr = Config.RESPONE_GET_CARD_HIS_CARD_DPP;
            String custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            String tranId = Funcs.getValFromHashtbl(hashTbl, "TRANS_ID");
            String cardNo = Funcs.getValFromHashtbl(hashTbl, "CARD_NO");
            String enqType = Funcs.getValFromHashtbl(hashTbl, "ENQTYPE");
            String fromDate = Funcs.getValFromHashtbl(hashTbl, "FROMDATE");
            String toDate = Funcs.getValFromHashtbl(hashTbl, "TODATE");

            string errCode = "";
            string errDesc = "";
            string strTemp = "";
            string cardHist = "";

            try
            {

                CardDPP.GetTransactionListResType res = new CardDPPIntegration().GetCardHist(custid, cardNo, enqType, fromDate, toDate);

                if (res != null && res.RespSts.Sts.Equals("0"))
                {
                    List<CardDPP.TransactionListType> lstTransaction = new List<CardDPP.TransactionListType>(res.TransactionList);

                    if (lstTransaction != null && lstTransaction.Count > 0)
                    {
                        foreach (var item in lstTransaction)
                        {

                            strTemp += (string.IsNullOrEmpty(item.tranId) ? "" : item.tranId)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.status) ? "" : item.status)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.txDt) ? "" : item.txDt)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.txTime) ? "" : item.txTime)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.txDesc) ? "" : item.txDesc)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.txAmt) ? "" : item.txAmt)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.mccCd) ? "" : item.mccCd)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.amtOrg) ? "" : item.amtOrg)
                            + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.finDt) ? "" : item.finDt)
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                        }

                        cardHist = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                        retStr = retStr.Replace("{ERR_CODE}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[0].ToString());
                        retStr = retStr.Replace("{ERR_DESC}", Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[1].ToString()
                            + Config.COL_REC_DLMT
                            + Config.RET_GET_CARD_INSTALLMENT_PERIOD[0].Split('|')[2].ToString());

                        retStr = retStr.Replace("{CARD_HIS}", cardHist);
                    }

                    return retStr;
                }
                else
                {
                    for (int i = 1; i < Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length; i++)
                    {
                        if (Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].Equals(res.RespSts.ErrCd))
                        {
                            errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[0].ToString();
                            errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[1].ToString()
                                + Config.COL_REC_DLMT
                                + Config.RET_GET_CARD_INSTALLMENT_PERIOD[i].Split('|')[2].ToString();
                        }
                    }

                    retStr = retStr.Replace("{ERR_CODE}", errCode);
                    retStr = retStr.Replace("{ERR_DESC}", errDesc);

                    Funcs.WriteLog("GET_CARD_HIS|CIF:" + custid + " ERROR CODE: " + res.RespSts.ErrCd);

                    return retStr;
                }

                Funcs.WriteLog("GET_CARD_HIS|CIF:" + custid + " ERROR");

                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF: " + custid + "|GET_CARD_HIS :" + ex.ToString());
                errCode = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[0].ToString();
                errDesc = Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[1].ToString()
                    + Config.COL_REC_DLMT
                    + Config.RET_GET_CARD_INSTALLMENT_PERIOD[Config.RET_GET_CARD_INSTALLMENT_PERIOD.Length - 1].Split('|')[2].ToString();

                retStr = retStr.Replace("{ERR_CODE}", errCode);
                retStr = retStr.Replace("{ERR_DESC}", errDesc);

                return retStr;
            }
            #endregion "CMD GET_CARD_HIS"
        }
    }
}