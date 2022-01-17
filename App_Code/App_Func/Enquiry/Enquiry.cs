using System;
using System.Collections.Generic;
using System.Web;
using mobileGW.Service.Framework;
using mobileGW.Service.DataAccess;
using System.Data;
using System.Collections;
using System.Threading;
using System.Web.Script.Serialization;
using System.Text;
using iBanking.Common;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

/// <summary>
/// Summary description for Financial_Transfer
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class Enquiry
    {
        public Enquiry()
        {
            //
            //
        }
        /// <summary>
        /// 12. GET_ACCT_LIST_HOMESCREEN_N
        /// </summary>
        /// <param name="custid"></param>
        /// <returns></returns>
        public static string GET_ACCT_LIST_HOMESCREEN_N(String custid)
        {

            String retStr = Config.GET_ACCT_LIST_HOMESCREEN_N;

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_HOMESCREEN_N BEGIN");


            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Enquirys da = new Enquirys();
                DataSet ds = new DataSet();
                ds = da.GET_ACCT_LIST_HOMESCREEN_N(custid);

                //2. Gen reply message
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{CIF_NO}", custid);
                    retStr = retStr.Replace("{CASA_TOTAL}", ds.Tables[0].Rows[0]["CASA_TOTAL"].ToString());
                    retStr = retStr.Replace("{TIDE_TOTAL}", ds.Tables[0].Rows[0]["TIDE_TOTAL"].ToString());
                    retStr = retStr.Replace("{LOAN_TOTAL}", ds.Tables[0].Rows[0]["LOAN_TOTAL"].ToString());

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_HOMESCREEN_N END CASE DONE " );

                    return retStr;

                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_HOMESCREEN_N END CASE ELSE NO DATAFOUND");

                    return retStr;

                }

            }
            catch (Exception ex)
            {
                //Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_HOMESCREEN_N END EXCEPTION:" + ex.ToString());

                return retStr;
            }
        }

        #region "GET_ACCT_BALANCE_HOMESCREEN_N OLD"
        //public static string GET_ACCT_BALANCE_HOMESCREEN_N(string custid, string acctno)
        //{

        //    String retStr = Config.GET_ACCT_BALANCE_HOMESCREEN_N;
        //    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N BEGIN");

        //    try
        //    {


        //        //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
        //        Enquirys da = new Enquirys();
        //        DataSet ds = new DataSet();
        //        ds = da.GET_ACCT_BALANCE_HOMESCREEN_N(custid, acctno);

        //        //2. Gen reply message
        //        if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace("{CIF_NO}", custid);
        //            retStr = retStr.Replace("{ACCTNO}", ds.Tables[0].Rows[0][CASA_BALANCE.ACCTNO].ToString());
        //            retStr = retStr.Replace("{AVAIL_BALANCE}", ds.Tables[0].Rows[0][CASA_BALANCE.AVAI_BAL].ToString());
        //            retStr = retStr.Replace("{CURR_BALANCE}", ds.Tables[0].Rows[0][CASA_BALANCE.CUR_BAL].ToString());
        //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N END CASE DONE "
        //              + "RET:" + retStr);
        //            return retStr;
        //        }
        //        else
        //        {
        //            retStr = Config.ERR_MSG_GENERAL;
        //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N END CASE ELSE NO DATAFOUND");

        //            return retStr;
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;
        //        Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N END EX:" + ex.ToString());

        //        return retStr;
        //    }
        //}
        #region "GET_DETAIL_TRAN"
        public static string GET_DETAIL_TRAN(string custid, string refno, string channel) {
            String retStr = Config.GET_DETAIL_TRAN;
            Funcs.WriteLog("custid:" + custid + "|GET_DETAIL_TRAN BEGIN");
            try
            {
                //get all function that get allow user register notification
                var dynParams = new OracleDynamicParameters();
                dynParams.Add("OUT_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                dynParams.Add("pCUSTID", custid, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("pCORE_REF_NO", refno, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("pCHANNEL_ID", channel, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

                CasaDetailModel respModel = (CasaDetailModel)new ConnectionFactory(Config.gEBANKConnstr)
                                   .ExecuteData<CasaDetailModel>(CommandType.StoredProcedure, "pkg_tx_NEW.GET_TRAN_BY_CORE_REF_NO_NEW", dynParams).First();

                if (respModel != null && !String.IsNullOrEmpty(respModel.CUSTID))
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_DETAIL_TRAN Res: " + new JavaScriptSerializer().Serialize(respModel));

                    string tran_type_vi = "";
                    string tran_type_en = "";
                    string detail = "";
                    //if success to get data from db
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "SUCCESSFULL");

                    #region GETMESSAGE
                    switch (respModel.TRAN_TYPE)
                    {
                        case Config.TransType.TRAN_TYPE_INTRA:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }

                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("DES_ACCT", respModel.DES_ACCT)
                            + GetMessage("DES_NAME", respModel.DES_NAME)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("FEE_AMOUNT", respModel.FEE_AMOUNT + respModel.VAT_AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;

                            break;
                        case Config.TransType.TRAN_TYPE_ACQ_247AC:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en = item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("DES_ACCT", respModel.DES_ACCT)
                            + GetMessage("DES_NAME", respModel.DES_NAME)
                            + GetMessage("BANK_NAME", respModel.BANK_NAME)
                            + GetMessage("POS_CD", respModel.POS_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("AMOUNT", respModel.SUSPEND_AMOUNT, respModel.CCY_CD)
                            + GetMessage("FEE_AMOUNT", respModel.FEE_AMOUNT + respModel.VAT_AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                        case Config.Db.TransType.ACQ_247CARD:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("DES_ACCT", respModel.DES_ACCT)
                            + GetMessage("DES_NAME", respModel.DES_NAME)
                            + GetMessage("BANK_NAME", respModel.BANK_NAME)
                            + GetMessage("POS_CD", respModel.POS_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("AMOUNT", respModel.SUSPEND_AMOUNT, respModel.CCY_CD)
                            + GetMessage("FEE_AMOUNT", respModel.FEE_AMOUNT + respModel.VAT_AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                        case Config.TransType.TRAN_TYPE_DOMESTIC:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("DES_ACCT", respModel.DES_ACCT)
                            + GetMessage("DES_NAME", respModel.DES_NAME)
                            + GetMessage("BANK_NAME", respModel.BANK_NAME)
                            + GetMessage("POS_CD", respModel.POS_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("AMOUNT", respModel.SUSPEND_AMOUNT, respModel.CCY_CD)
                            + GetMessage("FEE_AMOUNT", respModel.FEE_AMOUNT + respModel.VAT_AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                        case Config.TransType.TRAN_TYPE_SELF:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD 
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("DES_ACCT", respModel.DES_ACCT)
                            + GetMessage("DES_NAME", respModel.DES_NAME)
                            + GetMessage("POS_CD", respModel.POS_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("FEE_AMOUNT", respModel.FEE_AMOUNT + respModel.VAT_AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;

                            break;
                        case Config.TransType.TRAN_TYPE_TOPUP_SHS:
                        case Config.TransType.TRAN_TYPE_TOPUP_SHBS:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                        case Config.TransType.TRAN_TYPE_CHARITY:
                            //String charityName = string.Empty;
                            //List<CHARITY> charityList = CharityDAO.GetCharityListByCode("%%");
                            //foreach (CHARITY item in charityList)
                            //{
                            //    if (item.ACCTNO.Equals(res.DES_ACCT))
                            //    {
                            //        if (Session[iBanking.Common.ConstantsCore.SystemConsts.LANG] != null
                            //                                       && iBanking.Common.ConstantsCore.Core.VN_LANG.Equals((string)Session[iBanking.Common.ConstantsCore.SystemConsts.LANG]))
                            //        {
                            //            charityName = item.ACCTNAME;
                            //        }
                            //        else
                            //        {
                            //            charityName = item.ACCTNAME_EN;
                            //        }
                            //    }
                            //}

                            //confirmModelViewPool.Add(new CustomKeyValue(Ibanking.to.ToUpper(), Ibanking.funCharity.ToUpper()));
                            //confirmModelViewPool.Add(new CustomKeyValue(Ibanking.funCharityNm.ToUpper(), charityName));
                            //confirmModelViewPool.Add(new CustomKeyValue(Ibanking.sourceAccount.ToUpper(), res.SRC_ACCT));
                            //confirmModelViewPool.Add(new CustomKeyValue(Ibanking.transferAmount.ToUpper(), Funcs.ConvertMoney(res.AMOUNT) + " " + res.CCY_CD));
                            //confirmModelViewPool.Add(new CustomKeyValue(Ibanking.transferContent.ToUpper(), res.TXDESC));
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("AMOUNT", respModel.SUSPEND_AMOUNT, respModel.CCY_CD)
                            + GetMessage("FEE_AMOUNT", respModel.FEE_AMOUNT + respModel.VAT_AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;

                        //Chieu` nhan tien chua la acc247, card 247, CITAD, INTRA
                        case Config.TransType.TRAN_TYPE_BILLING_BEN:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("ORDER_ID", respModel.ORDER_ID)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                        case Config.TransType.TRAN_TYPE_BILL_MOBILE:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("ORDER_ID", respModel.ORDER_ID)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;

                            break;
                        case Config.TransType.TRAN_TYPE_TOPUP_MOBILE:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("ORDER_ID", respModel.ORDER_ID)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                        case Config.TransType.TRAN_TYPE_TOPUP_OTHER:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("ORDER_ID", respModel.ORDER_ID)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("SRC_ACCT", respModel.SRC_ACCT)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;

                        case Config.TransType.TRAN_TYPE_PAYMENT_ONLINE:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("MERCHANT_CODE", respModel.MERCHANT_CODE)
                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                        default:
                            foreach (var item in Config.RET_TRAN_TYPE_NAME)
                            {
                                if (item.Split('|')[0].Equals(respModel.TRAN_TYPE))
                                {
                                    tran_type_vi = item.Split('|')[1].ToString();
                                    tran_type_en =  item.Split('|')[2].ToString();
                                }
                            }
                            detail = detail
                            + tran_type_vi
                            + Config.COL_REC_DLMT
                            + tran_type_en
                            + Config.COL_REC_DLMT
                            + "-" + Funcs.ConvertMoney(respModel.AMOUNT.ToString()) + " " + respModel.CCY_CD
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT

                            + GetMessage("AMOUNT", respModel.AMOUNT, respModel.CCY_CD)
                            + GetMessage("TXDESC", respModel.TXDESC)
                            + GetMessage("CORE_REF_NO", respModel.CORE_REF_NO)
                            + GetMessage("CORE_TXDATE", respModel.CORE_TXDATE)
                            ;
                            break;
                    }
                    #endregion

                    detail = detail.Remove(detail.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    
                    retStr = retStr.Replace("{DETAIL}", detail);

                    Funcs.WriteLog("custid:" + custid + "|GET_DETAIL_TRAN MESG: " + retStr);
                }
                else
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_DETAIL_TRAN EX: " + Config.ERR_CODE_GENERAL);
                    retStr = Config.ERR_MSG_GENERAL;
                }
                
            }
            catch (Exception ex) {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|GET_DETAIL_TRAN END EX:" + ex.ToString());
            }
            return retStr;
        }

        #endregion

        #endregion "GET_ACCT_BALANCE_HOMESCREEN_N OLD"

        
        public static string GET_ACCT_BALANCE_HOMESCREEN_N(string custid, string acctno)
        {

            String retStr = Config.GET_ACCT_BALANCE_HOMESCREEN_N;
            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N BEGIN");

            try
            {
                //step 1: Lay tai khoan mac dinh trong TBL_EB_USER_CHANNEL
                string default_acctno = string.Empty;
                default_acctno = new Enquirys().GET_MOB_ACCT_DEFAULT(custid, acctno);

                //step 2: lay so du tai khoan mac dinh (goi ham lay danh sach tai khoan tren ESB)
                
                AccList.AcctListInqResType resp = new AccList.AcctListInqResType();

                string[] custIdArr = new string[1] { custid };

                //lay list tai khoan CASA
                resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.prod_cd_CASA);

                //neu thanh cong, fill gia tri 
                if (resp != null && resp.AcctRec != null)
                {
                    //neu default acct la null

                    if (string.IsNullOrEmpty(default_acctno) || "ALL".Equals(default_acctno))
                    {
                        //lay tai khoan VND dau tien
                        foreach (AccList.AcctRecType item in resp.AcctRec)
                        {
                            if ( item.BankAcctId.AcctCur == Config.CCYCD_VND)
                            {
                                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                                retStr = retStr.Replace("{CIF_NO}", custid);

                                //retStr = retStr.Replace("{ACCTNO}", ds.Tables[0].Rows[0][CASA_BALANCE.ACCTNO].ToString());
                                retStr = retStr.Replace("{ACCTNO}", item.BankAcctId.AcctId.ToString());
                                retStr = retStr.Replace("{AVAIL_BALANCE}", item.AvailBal);
                                retStr = retStr.Replace("{CURR_BALANCE}", item.CurBal);
                                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N END CASE DONE "
                                  + "RET:" + retStr);
                                return retStr;
                            }
                        }
                    }
                    //neu co tai khoan mac dinh, lay so du cua tai khoan mac dinh
                    else 
                    {

                        foreach (AccList.AcctRecType item in resp.AcctRec)
                        {
                            
                            if (default_acctno.Equals(item.BankAcctId.AcctId.ToString())) //so tai khoan dung bang so tai khoan mac dinh thi add vao list
                            {
                                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                                retStr = retStr.Replace("{CIF_NO}", custid);

                                //retStr = retStr.Replace("{ACCTNO}", ds.Tables[0].Rows[0][CASA_BALANCE.ACCTNO].ToString());
                                retStr = retStr.Replace("{ACCTNO}", item.BankAcctId.AcctId.ToString());
                                retStr = retStr.Replace("{AVAIL_BALANCE}", item.AvailBal);
                                retStr = retStr.Replace("{CURR_BALANCE}", item.CurBal);
                                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N END CASE DONE "
                                  + "RET:" + retStr);
                                return retStr;
                            }

                        }
                    }
                    // cac loi khac thi return error
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N END CASE ELSE NO DATAFOUND");
                    return retStr;
                }
                

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_HOMESCREEN_N END EX:" +ex.ToString());

                return retStr;
            }
        }

        /*
        Request
"REQ=CMD# GET_ACCT_LIST_QRY_N|ACTIVE_CODE#123456|CIF_NO#0310008705|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

	
public static String GET_ACCT_LIST_QRY_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"	
			+ ROW_DLMT + "CASA_TOTAL" + COL_DLMT + "{CASA_TOTAL}"	
			+ ROW_DLMT + "TIDE_TOTAL" + COL_DLMT + "{TIDE_TOTAL}"	
			+ ROW_DLMT + "LOAN_TOTAL" + COL_DLMT + "{LOAN_TOTAL}"			
			+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
			;
			
RECORD LIST CAC TAI KHOAN: LOAI TAI KHOAN| SO TAI KHOAN| LOAI TIEN

public const String prod_cd_CASA = "001";
public const String prod_cd_TIDE = "002";
public const String prod_cd_LENDING = "003";
        */
        public static string GET_ACCT_LIST_QRY_N(Hashtable hashTbl)
        {

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

            //test tam
            //string custid = "0310008705";

            //retStr = Enquiry.GET_ACCT_LIST_QRY_N(acct_type, mob_user);
            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_QRY_N BEGIN");


            String retStr = Config.GET_ACCT_LIST_QRY_N;
            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N

                //Enquirys da = new Enquirys();
                DataSet ds = new DataSet();
                
                //ds = da.GET_ACCT_LIST_QRY_N(custid);

                //linhtn add new 2017 03 22
                

                AccList.AcctListInqResType resp = new AccList.AcctListInqResType();
                //FlexTide.AcctListInqResType resp = new FlexTide.AcctListInqResType();

                //string[] finSts = new string[2] { Config.FinancialStatus.NORMAL, Config.FinancialStatus.NO_CREDIT };
                string[] custIdArr = new string[1] { custid };

                //resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.TYPE_CASA_PRODUCT);

                //resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", "ALL");
                resp = CoreIntegration.getAcctTideOlInfoList(custid, "ALL", "CIF");

                //ds = da.GET_ACCT_LIST_QRY_N(custid);


                //ds format:
                // CASA_TOTAL | TIDE_TOTAL | LOAN_TOTAL | AC_TYPE| ACCTNO | CCYCD
                //2. Gen reply message
                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                if (resp != null && resp.AcctRec != null)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{CIF_NO}", custid);

                    //OLD
                    //retStr = retStr.Replace("{CASA_TOTAL}", ds.Tables[0].Rows[0]["CASA_TOTAL"].ToString());
                    //retStr = retStr.Replace("{TIDE_TOTAL}", ds.Tables[0].Rows[0]["TIDE_TOTAL"].ToString());
                    //retStr = retStr.Replace("{LOAN_TOTAL}", ds.Tables[0].Rows[0]["LOAN_TOTAL"].ToString());

                    retStr = retStr.Replace("{CASA_TOTAL}", "0");
                    retStr = retStr.Replace("{TIDE_TOTAL}", "0");
                    retStr = retStr.Replace("{LOAN_TOTAL}", "0");
                    //retStr = ret
                    //NEW FOR ESB NOT NEED

                    string strTemp = "";
                    string currentMonth = String.Empty;
                    string lastMonth = String.Empty;

                    //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)

                    if (DateTime.Now.Month == 1)
                    {
                        lastMonth = Funcs.getMonth(12);
                    }
                    else
                    {
                        lastMonth = Funcs.getMonth((DateTime.Now.Month - 1));
                    }

                    currentMonth = Funcs.getMonth(DateTime.Now.Month);

                    foreach (AccList.AcctRecType item in resp.AcctRec)

                    {
                        //OLD
                        //strTemp = strTemp +
                        //    ds.Tables[0].Rows[j]["ACCT_TYPE"].ToString() +
                        //    Config.COL_REC_DLMT +
                        //    ds.Tables[0].Rows[j]["ACCTNO"].ToString()
                        //    + Config.COL_REC_DLMT +
                        //    ds.Tables[0].Rows[j]["CCYCD"].ToString()
                        //    + Config.COL_REC_DLMT +
                        //    ds.Tables[0].Rows[j]["PROD_DESC"].ToString()
                        //     + Config.COL_REC_DLMT +
                        //    ds.Tables[0].Rows[j]["AVAIL_BAL"].ToString()
                        //     + Config.COL_REC_DLMT +
                        //    ds.Tables[0].Rows[j]["CCYCD"].ToString()
                        //+ Config.ROW_REC_DLMT;

                        //NEW FOR ESB

                     
                      //  if (item.BankAcctId.AcctCur == "VND") //chi add tk VND vao trong list
                        {
                           strTemp = strTemp +
                           item.BankAcctId.AcctType +
                           Config.COL_REC_DLMT +
                           item.BankAcctId.AcctId
                           + Config.COL_REC_DLMT +
                           item.BankAcctId.AcctCur
                           + Config.COL_REC_DLMT +
                            item.ProdDesc
                            + Config.COL_REC_DLMT +
                           item.AvailBal
                            + Config.COL_REC_DLMT +
                            item.BankAcctId.AcctCur
                            + Config.COL_REC_DLMT +
                            item.IsCoholder
                            + Config.COL_REC_DLMT +
                            item.combo
                            + Config.COL_REC_DLMT +
                            lastMonth
                            + Config.COL_REC_DLMT +
                            item.avgLastMonth
                            + Config.COL_REC_DLMT +
                            currentMonth
                            + Config.COL_REC_DLMT +
                            item.avgCurrent
                            + Config.COL_REC_DLMT +
                            item.openDate
                            + Config.ROW_REC_DLMT
                            ;
                        }
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_QRY_N END CASE DONE RET: "
                        + retStr);

                    return retStr;
                    
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_QRY_N END CASE ELSE ERROR RET: "
                   + retStr);

                    return retStr;

                }

            }
            catch (Exception ex)
            {
                retStr = Config.ERR_MSG_GENERAL;

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_QRY_N END CASE EX:"
                  + ex.ToString());
                return retStr;
            }
        }

        /*
        Request " REQ=CMD#GET_ACCT_CASA_INFO_N|CIF_NO#0310008705|ACCTNO#1000013376|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
Response
public static String GET_ACCT_CASA_INFO_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
			+ ROW_DLMT + "ACCT_CURR_BALANCE" + COL_DLMT + "{ACCT_CURR_BALANCE}"
			+ ROW_DLMT + "ACCT_AVAI_BALANCE" + COL_DLMT + "{ACCT_AVAI_BALANCE}"
			+ ROW_DLMT + "RECORD_BALANCE_HIS" + COL_DLMT + "{RECORD_BALANCE_HIS}"
			+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
			;	
			
RECORD_BALANCE_HIS: NGAY (DD/MM/YYYY) | BALANCE CUOI NGAY

RECORD_ACTIVITY: Default là trả về 5 giao dịch gần nhất

NGAY (DD/MM/YYYY) | AMOUNT (Định dạng + - số tiền, đã format ở server) | Diễn giải giao dịch (đã remove các ký tự đặc biệt)

12/01/2016^+15.000^chuyen khoan
$
11/01/2016^-10.000^chuyen khoan test
        */
        /// <summary>
        /// linhtn: update 15 jul 2016
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="acctno"></param>
        /// <returns></returns>
        public static string GET_ACCT_CASA_INFO_N(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
            string enquiry_type = Funcs.getValFromHashtbl(hashTbl, "ENQUIRY_TYPE");

            String retStr = Config.GET_ACCT_CASA_INFO_N;
            string strTemp = "";
            try
            {

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N BEGIN ACCTNO:" +  acctno );

                #region "1. Lấy số dư tk casa 30 ngày gần nhất"
                //Enquirys da = new Enquirys();
                //DataTable dt = new DataTable();
                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                    + "GET LAST 30 DAY BALANCE HIST BEGIN");

                //dt = da.CASA_BALANCE_HIST(custid, acctno);

                AcctBalHist.AcctBalHistInquiryResType resBal = CoreIntegration.GetBalanceForChartCasa( custid, acctno);


                //if (dt != null && dt.Rows.Count > 0)
                if (resBal != null && resBal.BalHistRec != null)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    //for (int j = 0; j < dt.Rows.Count; j++)
                    foreach (var item in resBal.BalHistRec)
                    {
                        strTemp = strTemp +
                             //(dt.Rows[j][CASA_TRAN.TXDATE] == DBNull.Value ? "_NULL" : dt.Rows[j][CASA_TRAN.TXDATE].ToString())
                             (item.TxnDt == "" ? "_NULL" : item.TxnDt)

                            //(dt.Rows[j][TBL_EB_BEN.ACCTNO] == DBNull.Value? "_NULL_":dt.Rows[j][TBL_EB_BEN.ACCTNO].ToString())

                            + Config.COL_REC_DLMT +
                            (item.BalAmt == "" ? "_NULL" : item.BalAmt)
                            + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD_BALANCE_HIST}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE_BALANCE_HIST}", Config.ERR_CODE_DONE);

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                    + "GET LAST 30 DAY BALANCE HIST DONE END");


                }
                else //KHONG CO DU LIEU
                {
                    //retStr = Config.ERR_MSG_GENERAL;

                    retStr = retStr.Replace("{ERR_CODE_BALANCE_HIST}", Config.ERR_CODE_GENERAL);
                    //return retStr;

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                        + "GET LAST 30 DAY BALANCE HIST ELSE NODATAFOUND END");
                }

               // Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N HET LAY SO DU 30 NGAY GAN NHAT ACCTNO:" + acctno);

                #endregion "1. Lấy số dư tk casa 30 ngày gần nhất"


                #region "2. Lấy thông tin chi tiết TK CASA"
                //2. Lấy thông tin chi tiết TK CASA

                //DataSet ds = new DataSet();

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                   + "GET GET_ACCT_BALANCE_LIST_N BEGIN");
                //ds = da.GET_ACCT_BALANCE_LIST_N(custid, acctno, "ALL");


                //linhtn fix 20160906 --> cho phep lay thong tin detail cua tai khoan dang dang o INACTIVE STATUS
                //ds = da.GET_ACCT_LIST_DETAIL(custid, acctno, "ALL");

                AccList.AcctListInqResType resp = new AccList.AcctListInqResType();

                string[] custIdArr = new string[1] { custid };

                resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.TYPE_CASA_PRODUCT);


                //2. Gen reply message
                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                if (resp != null && resp.AcctRec != null)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    string curBal = "0";
                    string avaiBal = "0";
                    string comboInfo = String.Empty;

                    //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    foreach (AccList.AcctRecType item in resp.AcctRec)
                    {
                        //retStr = retStr.Replace("{ACCT_CURR_BALANCE}", ds.Tables[0].Rows[j][CASA_BALANCE.CUR_BAL].ToString());
                        //retStr = retStr.Replace("{ACCT_AVAI_BALANCE}", ds.Tables[0].Rows[j][CASA_BALANCE.AVAI_BAL].ToString());
                        if (item.BankAcctId.AcctId.Equals(acctno))
                        {
                            curBal = item.CurBal;
                            avaiBal = item.AvailBal;
                            comboInfo = item.combo;

                            break;
                        } 
                    }
                    retStr = retStr.Replace("{ACCT_CURR_BALANCE}", curBal);
                    retStr = retStr.Replace("{ACCT_AVAI_BALANCE}", avaiBal);
                    retStr = retStr.Replace("{COMBO}", comboInfo);

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                 + "GET GET_ACCT_BALANCE_LIST_N DONE END|" + retStr);
                }

                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                 + "GET GET_ACCT_BALANCE_LIST_N END ELSE COULD NOT GET AVAIL BAL");
                    return retStr;
                }
                #endregion "2. Lấy thông tin chi tiết TK CASA"

                
                
                #region "3. Lấy thông tin 5 giao dịch gần nhất"
                //3. Lấy thông tin 5 giao dịch gần nhất
                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
               + "GET LAST5 TRAN BEGIN");

                //dt = da.GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N( custid,  acctno, enquiry_type, "", ""); //--> chuyen vao file config
                
                AcctHist.AcctHistInqResType res = CoreIntegration.GetAcctHist(custid, acctno, "CA", enquiry_type, "", "");


                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);


                //ds format:
                //NGAY(DD / MM / YYYY hh24:mm) | AMOUNT(Định dạng + -số tiền, đã format ở server) | Diễn giải giao dịch(đã remove các ký tự đặc biệt)
                // tra ve nho remove ky tu dac biet cua txdesc
                //2. Gen reply message
                //if (dt != null && dt.Rows.Count > 0)
                if (res != null && res.Transaction != null)
                {
                    StringBuilder strTempBuilder = new StringBuilder();
                    string tmpAmount = "0";
                    //for (int j = 0; j < dt.Rows.Count; j++)
                    foreach (var item in res.Transaction)
                    {

                        //tmpAmount = Double.Parse(dt.Rows[j][CASA_TRAN.DAMT].ToString()) > 0 ? 
                        //    "-" + String.Format("{0:#,###0}", dt.Rows[j][CASA_TRAN.DAMT]) : 
                        //    "+" + String.Format("{0:#,###0}", dt.Rows[j][CASA_TRAN.CAMT]);
                        
                        tmpAmount = Double.Parse(item.DbAmt.ToString()) > 0 ? "-" + String.Format("{0:#,###0}", Double.Parse(item.DbAmt.ToString()))
                        : "+" + String.Format("{0:#,###0}", Double.Parse(item.CrAmt.ToString()));

                        //strTemp = strTemp +
                        //    item.TxDt.ToString() + " " + //dt.Rows[j][CASA_TRAN.TXDATE].ToString() + " " +
                        //    item.TxTime.ToString() + //dt.Rows[j][CASA_TRAN.TXTIME].ToString() +
                        //    Config.COL_REC_DLMT + tmpAmount
                        //   //+ Config.COL_REC_DLMT + Funcs.NoHack(dt.Rows[j][CASA_TRAN.TXDESC] == DBNull.Value ? " " : dt.Rows[j][CASA_TRAN.TXDESC].ToString())

                        //   + Config.COL_REC_DLMT + item.TxDesc == "" ? " " : item.TxDesc

                        //   + Config.COL_REC_DLMT + item.RefNo.ToString() //+ Config.COL_REC_DLMT + dt.Rows[j][CASA_TRAN.REF_NO].ToString()
                        //   + Config.COL_REC_DLMT + item.Cur.ToString()  //+ Config.COL_REC_DLMT + dt.Rows[j][CASA_TRAN.CCYCD].ToString() 
                        //+ Config.ROW_REC_DLMT;
                        strTempBuilder.Append(item.TxDt.ToString())
                            .Append(Config.COL_REC_DLMT)
                            .Append(tmpAmount).Append(Config.COL_REC_DLMT).Append(string.IsNullOrEmpty(item.TxDesc) ? " " : item.TxDesc.Replace("#", "-").Replace("|","-"))
                            .Append(Config.COL_REC_DLMT).Append(item.RefNo.ToString())
                            .Append(Config.COL_REC_DLMT).Append(item.Cur.ToString())
                            .Append(Config.COL_REC_DLMT).Append(item.ChnlId.ToString())
                            .Append(Config.ROW_REC_DLMT);

                    }
                    strTemp = strTempBuilder.ToString().Remove(strTempBuilder.ToString().Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);                    
                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                        + "GET LAST5 TRAN DONE END|" + Funcs.getMaskingStr(retStr));

                }
                else // KHONG CO GIAO DICH
                {
                    //retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_MSG_NO_DATA_FOUND);

                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);

                    Funcs.WriteLog("custid:" + custid + "|G ET_ACCT_CASA_INFO_N  ACCTNO:" + acctno
                            + "GET LAST5 TRAN ELSE DONE END NODATAFOUND");
                }

                #endregion "3. Lấy thông tin 5 giao dịch gần nhất"
               // Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_INFO_N HET LAY 5 GIAO DICH GAN NHAT ACCTNO:" + acctno);

                return retStr;

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }
        /*
       Request " REQ=CMD#GET_ACCT_CASA_TRAN__BY_ENQ_TYPE_N|CIF_NO#0310008705|ACCTNO#1000013376|ENQUIRY_TYPE#THIS_MONTH|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
Response
public static String GET_ACCT_CASA_TRAN__BY_ENQ_TYPE_N= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
           + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
           + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
           + ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
           ;	


Trong đó: 

RECORD_ACTIVITY: Default là trả về 5 giao dịch gần nhất

NGAY (DD/MM/YYYY) | AMOUNT (Định dạng + - số tiền, đã format ở server) | Diễn giải giao dịch (đã remove các ký tự đặc biệt)



ENQUIRY_TYPE: 
Giao dịch trong ngày hôm nay: TODAY
Giao dịch trong tháng này: THIS_MONTH
Giao dịch trong tháng trước: LAST_MONTH
Giao dịch tuần này: THIS_WEEK
Giao dịch tuần trước: LAST_WEEK

       */


        #region "ACCT HIST OLD"
            ///// <summary>
            ///// linhtn 15 jul 2016
            ///// </summary>
            ///// <param name="acctno"></param>
            ///// <param name="enquiry_type"></param>
            ///// <returns></returns>
            //public static string GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N(Hashtable hashTbl)
            //{

            //    string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            //    string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
            //    string enquiry_type = Funcs.getValFromHashtbl(hashTbl, "ENQUIRY_TYPE");
            //    String retStr = Config.GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N;

            //    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N BEGIN ACCTNO:" + acctno
            //  + "|ENQ_TYPE:" + enquiry_type);
            //    try
            //    {
            //        //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
            //        Enquirys da = new Enquirys();
            //        DataTable dt = new DataTable();
            //        //dt = da.GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N(acctno, enquiry_type, "", "");
            //        dt = da.GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N(custid, acctno, enquiry_type, "", ""); //--> chuyen vao file config
            //        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);


            //        //ds format:
            //        //NGAY(DD / MM / YYYY hh24:mm) | AMOUNT(Định dạng + -số tiền, đã format ở server) | Diễn giải giao dịch(đã remove các ký tự đặc biệt)
            //        // tra ve nho remove ky tu dac biet cua txdesc
            //        //2. Gen reply message

            //        if (dt != null && dt.Rows.Count > 0)
            //        {

            //            string strTemp = "";
            //            string tmpAmount = "0";
            //            for (int j = 0; j < dt.Rows.Count; j++)
            //            {

            //                tmpAmount = Double.Parse(dt.Rows[j][CASA_TRAN.DAMT].ToString()) > 0 ? "-" + String.Format("{0:#,###0}", dt.Rows[j][CASA_TRAN.DAMT]) : "+" + String.Format("{0:#,###0}", dt.Rows[j][CASA_TRAN.CAMT]);


            //                strTemp = strTemp +
            //                    dt.Rows[j][CASA_TRAN.TXDATE].ToString() +
            //                    dt.Rows[j][CASA_TRAN.TXTIME].ToString() +
            //                    Config.COL_REC_DLMT + tmpAmount
            //                   + Config.COL_REC_DLMT +
            //                   Funcs.NoHack(dt.Rows[j][CASA_TRAN.TXDESC] == DBNull.Value ? " " : dt.Rows[j][CASA_TRAN.TXDESC].ToString())
            //                + Config.ROW_REC_DLMT;

            //            }
            //            strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

            //            retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
            //            //retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);
            //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N END ACCTNO:" + acctno
            //  + "|ENQ_TYPE:" + enquiry_type + "|CASE DONE");

            //        }
            //        else // KHONG CO GIAO DICH
            //        {
            //            //retStr = retStr.Replace("{ERR_CODE_RECORD}", Config.ERR);
            //            //retStr = Config.ERR_MSG_NO_DATA_FOUND;
            //            retStr = Config.ERR_MSG_GENERAL;

            //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N END ACCTNO:" + acctno
            //  + "|ENQ_TYPE:" + enquiry_type + "|CASE ELSE NODATAFOUND");
            //        }


            //        return retStr;
            //    }
            //    catch (Exception ex)
            //    {
            //        Funcs.WriteLog(ex.ToString());
            //        retStr = Config.ERR_MSG_GENERAL;
            //        return retStr;
            //    }
            //}
        #endregion "ACCT HIST OLD"

        /// <summary>
        /// linhtn 15 jul 2016
        /// </summary>
        /// <param name="acctno"></param>
        /// <param name="enquiry_type"></param>
        /// <returns></returns>
        public static string GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N( Hashtable hashTbl )
        {

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
            string enquiry_type = Funcs.getValFromHashtbl(hashTbl, "ENQUIRY_TYPE");
            String retStr = Config.GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N;

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N BEGIN ACCTNO:" + acctno
          + "|ENQ_TYPE:" + enquiry_type);
            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N

                //enquiry casa hist: acct_type = CA
                AcctHist.AcctHistInqResType res = CoreIntegration.GetAcctHist( custid, acctno, "CA", enquiry_type, "", ""  );

                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);


                //ds format:
                //NGAY(DD / MM / YYYY hh24:mm) | AMOUNT(Định dạng + -số tiền, đã format ở server) | Diễn giải giao dịch(đã remove các ký tự đặc biệt)
                // tra ve nho remove ky tu dac biet cua txdesc
                //2. Gen reply message

                if (res != null && res.Transaction != null)
                {

                    string strTemp = "";
                    string tmpAmount = "0";
                    
                    foreach (var item in res.Transaction)
                    {

                        // tmpAmount = Double.Parse(dt.Rows[j][CASA_TRAN.DAMT].ToString()) > 0 ? "-" + String.Format("{0:#,###0}", dt.Rows[j][CASA_TRAN.DAMT]) : "+" + String.Format("{0:#,###0}", dt.Rows[j][CASA_TRAN.CAMT]);

                        tmpAmount = Double.Parse( item.DbAmt.ToString()) > 0 ? "-" + String.Format("{0:#,###0}", Double.Parse(item.DbAmt.ToString())) 
                            : "+" + String.Format("{0:#,###0}", Double.Parse(item.CrAmt.ToString()));

                        strTemp = strTemp +
                            item.TxDt.ToString() + //dt.Rows[j][CASA_TRAN.TXDATE].ToString() +
                            item.TxTime.ToString() +
                            Config.COL_REC_DLMT + tmpAmount
                           + Config.COL_REC_DLMT +
                           //Funcs.NoHack(dt.Rows[j][CASA_TRAN.TXDESC]== DBNull.Value?" ": dt.Rows[j][CASA_TRAN.TXDESC].ToString())
                           (Funcs.NoHack(string.IsNullOrEmpty(item.TxDesc.ToString()) ? "" : item.TxDesc.ToString()))
                        + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    
                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    //retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);
                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N END ACCTNO:" + acctno
                                    + "|ENQ_TYPE:" + enquiry_type + "|CASE DONE");
                    
                }
                else // KHONG CO GIAO DICH
                {
                    //retStr = retStr.Replace("{ERR_CODE_RECORD}", Config.ERR);
                    //retStr = Config.ERR_MSG_NO_DATA_FOUND;
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N END ACCTNO:" + acctno
                    + "|ENQ_TYPE:" + enquiry_type + "|CASE ELSE NODATAFOUND");
                }
                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        /*
        requestBody = " REQ=CMD#GET_ACCT_TIDE_INFO_N|CIF_NO#0310008705|ACCTNO#1000013376|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
public static String GET_ACCT_TIDE_INFO_N= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
+ ROW_DLMT + "CCYCD" + COL_DLMT + {CCYCD}"
+ ROW_DLMT + "PRODUCT_CODE" + COL_DLMT + {PRODUCT_CODE}"
+ ROW_DLMT + "CURR_PRIN_AMT" + COL_DLMT + {CURR_PRIN_AMT}"

+ ROW_DLMT + "TENURE" + COL_DLMT + "{TENURE}"
+ ROW_DLMT + "UNIT_TENURE" + COL_DLMT + "{UNIT_TENURE}"
+ ROW_DLMT + "INT_RATE" + COL_DLMT + "{INT_RATE}"
+ ROW_DLMT + "MAT_DT" + COL_DLMT + "{MAT_DT}"
+ ROW_DLMT + "VAL_DT" + COL_DLMT + "{VAL_DT}"
+ ROW_DLMT + "CURR_MAT_AMT" + COL_DLMT + "{CURR_MAT_AMT}"
+ ROW_DLMT + "IS_SHOW_INTEREST" + COL_DLMT + "{IS_SHOW_INTEREST}"
+ ROW_DLMT + "ALLOW_TIDEWDL_ONLINE " + COL_DLMT + "{ALLOW_TIDEWDL_ONLINE}"
+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
;	

Trong đó:
PRODUCT_CODE: Mã sản phẩm → Server sẽ check nếu = 470 (tiết kiệm online) → trả về thêm biến “ALLOW_TIDEWDL” cho phép tất toán = 1
CCYCD: Loại tiền VND,….
CURR_PRIN_AMT: Gốc gửi
INT_RATE: Lãi suất
TENURE: kỳ hạn 
UNIT_TENURE:  đơn vị (M/W/D/Y)
MAT_DT: Ngày đến hạn
CURR_MAT_AMT: Lãi dự kiến

VAL_DT: Ngày mở (Nếu roll thì ngày ngày là ngày của kỳ mới)

IS_SHOW_INTEREST: Có hiển thị lãi suất hay không
ALLOW_TIDEWDL_ONLINE: Cho phép tất toán online hay không. Nếu = 1→ cho phép tất toán online (hiển thị thêm nút tất toán).
        */

        #region "GET_ACCT_TIDE_INFO_N OLD"
        //public static string GET_ACCT_TIDE_INFO_N(Hashtable hashTbl)
        //{
        //    string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");

        //    String retStr = Config.GET_ACCT_TIDE_INFO_N;

        //    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_TIDE_INFO_N BEGIN ACCTNO:" + acctno);

        //    try
        //    {
        //        //1. Call function: GET_ACCT_TIDE_INFO_N
        //        Enquirys da = new Enquirys();
        //        DataTable dt = new DataTable();

        //        dt = da.GET_ACCT_TIDE_INFO_N(custid, acctno, "%");

        //        //ds format:



        //        //2. Gen reply message
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace("{CIF_NO}", custid);
        //            retStr = retStr.Replace("{ACCTNO}", dt.Rows[0]["ACCTNO"].ToString());
        //            retStr = retStr.Replace("{CCYCD}", dt.Rows[0]["CCYCD"].ToString());
        //            retStr = retStr.Replace("{PROD_CD}", dt.Rows[0]["PROD_CD"].ToString());
        //            retStr = retStr.Replace("{CURR_PRIN_AMT}", dt.Rows[0]["CURR_PRIN_AMT"].ToString());
        //            retStr = retStr.Replace("{TENURE}", dt.Rows[0]["TENURE"].ToString());
        //            retStr = retStr.Replace("{UNIT_TENURE}", dt.Rows[0]["UNIT_TENURE"].ToString());
        //            retStr = retStr.Replace("{MAT_DT}", dt.Rows[0]["MAT_DT"].ToString());
        //            retStr = retStr.Replace("{VAL_DT}", dt.Rows[0]["VAL_DT"].ToString());
        //            retStr = retStr.Replace("{AUTO_REN_NO}", dt.Rows[0]["AUTO_REN_NO"].ToString());
        //            retStr = retStr.Replace("{CURR_MAT_AMT}", dt.Rows[0]["CURR_MAT_AMT"].ToString());
        //            retStr = retStr.Replace("{INT_RATE}", dt.Rows[0]["INT_RATE"].ToString());
        //            retStr = retStr.Replace("{INT_AMT}", dt.Rows[0]["INT_AMT"].ToString());
        //            retStr = retStr.Replace("{DEPOSIT_NO}", dt.Rows[0]["DEPOSIT_NO"].ToString());

        //            retStr = retStr.Replace("{IS_SHOW_INTEREST}", "1");


        //            if (dt.Rows[0]["PROD_CD"].ToString() == Config.PROD_CD_TIDE_ONLINE)
        //            {
        //                retStr = retStr.Replace("{ALLOW_TIDEWDL_ONLINE}", "1");
        //            }
        //            else
        //            {
        //                retStr = retStr.Replace("{ALLOW_TIDEWDL_ONLINE}", "0");

        //            }

        //            retStr = retStr.Replace("{INSTRUCTION}", dt.Rows[0]["INSTRUCTION"].ToString());

        //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_TIDE_INFO_N END CASE DONE ACCTNO:" + acctno);

        //            return retStr;

        //        }
        //        else
        //        {
        //            retStr = Config.ERR_MSG_NO_DATA_FOUND;

        //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_TIDE_INFO_N END CASE ELSE ACCTNO:" + acctno);

        //            return retStr;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;
        //        return retStr;
        //    }
        //}

        #endregion "GET_ACCT_TIDE_INFO_N OLD"

        public static string GET_ACCT_TIDE_INFO_N(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
            string[] listProdCode = Funcs.getConfigVal("TIDE_PRODUCTCODE_ALLOW_BUTTON").Split(',');
            String retStr = Config.GET_ACCT_TIDE_INFO_N;

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_TIDE_INFO_N BEGIN ACCTNO:" + acctno);

            try
            {
                //1. Call function: GET_ACCT_TIDE_INFO_N

                //AccList.AcctListInqResType resp = new AccList.AcctListInqResType();

                string[] custIdArr = new string[1] { custid };

                //lay danh sach tai khoan tien gui tiet kiem 
                //resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.TYPE_TIDE_PRODUCT);
                AccList.AcctListInqResType resp = new AccList.AcctListInqResType();
                resp = CoreIntegration.getAcctTideOlInfoList(custid, "002", "CIF");
                //2. Gen reply message
                //if (dt != null && dt.Rows.Count > 0)
                if (resp != null && resp.AcctRec != null)
                {
                    foreach (AccList.AcctRecType item in resp.AcctRec)
                    {
                        if (item.BankAcctId.AcctId.Equals(acctno))
                        {
                            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                            retStr = retStr.Replace("{CIF_NO}", custid);
                            retStr = retStr.Replace("{ACCTNO}", item.BankAcctId.AcctId);// dt.Rows[0]["ACCTNO"].ToString());
                            retStr = retStr.Replace("{CCYCD}", item.BankAcctId.AcctCur);// dt.Rows[0]["CCYCD"].ToString());
                            retStr = retStr.Replace("{PROD_CD}", item.ProdCD);//  dt.Rows[0]["PROD_CD"].ToString());
                            retStr = retStr.Replace("{CURR_PRIN_AMT}", item.AvailBal);// dt.Rows[0]["CURR_PRIN_AMT"].ToString());
                            retStr = retStr.Replace("{TENURE}", item.Tenure);// dt.Rows[0]["TENURE"].ToString());
                            retStr = retStr.Replace("{UNIT_TENURE}", item.UnitTenure);// dt.Rows[0]["UNIT_TENURE"].ToString());
                            retStr = retStr.Replace("{MAT_DT}", item.MatDt);// dt.Rows[0]["MAT_DT"].ToString());
                            retStr = retStr.Replace("{VAL_DT}", item.ValDt);// dt.Rows[0]["VAL_DT"].ToString());
                            retStr = retStr.Replace("{AUTO_REN_NO}", item.AutoRenNo);// dt.Rows[0]["AUTO_REN_NO"].ToString());
                            retStr = retStr.Replace("{CURR_MAT_AMT}", item.MatAmt);// dt.Rows[0]["CURR_MAT_AMT"].ToString());
                            retStr = retStr.Replace("{INT_RATE}", item.IntRate);// dt.Rows[0]["INT_RATE"].ToString());
                            retStr = retStr.Replace("{INT_AMT}", item.IntAmt);// dt.Rows[0]["INT_AMT"].ToString());
                            retStr = retStr.Replace("{DEPOSIT_NO}", item.DepositNo);// dt.Rows[0]["DEPOSIT_NO"].ToString());

                            retStr = retStr.Replace("{IS_SHOW_INTEREST}", "1");

                            //if (dt.Rows[0]["PROD_CD"].ToString() == Config.PROD_CD_TIDE_ONLINE)
                            if (listProdCode != null && Array.IndexOf(listProdCode, item.ProdCD) != -1)
                            {
                                retStr = retStr.Replace("{ALLOW_TIDEWDL_ONLINE}", "1");
                            }
                            else
                            {
                                retStr = retStr.Replace("{ALLOW_TIDEWDL_ONLINE}", "0");
                            }
                            //retStr = retStr.Replace("{INSTRUCTION}", dt.Rows[0]["INSTRUCTION"].ToString());
                            retStr = retStr.Replace("{INSTRUCTION}", item.Instruction);
                            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_TIDE_INFO_N END CASE DONE ACCTNO:" + acctno);

                            return retStr;
                        }
                    }

                    return Config.ERR_MSG_NO_DATA_FOUND;
                }
                else
                {
                    retStr = Config.ERR_MSG_NO_DATA_FOUND;
                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_TIDE_INFO_N END CASE ELSE ACCTNO:" + acctno);
                    return retStr;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        #region "GET_ACCT_LOAN_INFO_N OLD"
        //public static string GET_ACCT_LOAN_INFO_N(Hashtable hashTbl)
        //{

        //    string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");

        //    string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

        //    String retStr = Config.GET_ACCT_LOAN_INFO_N;

        //    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N BEGIN ACCTNO:" + acctno);

        //    try
        //    {
        //        //1. Call function: GET_ACCT_LOAN_INFO_N
        //        Enquirys da = new Enquirys();
        //        DataSet ds = new DataSet();
        //        ds = da.GET_ACCT_LOAN_INFO_N(custid, acctno);

        //        //ds format:
        //        /*
                 
        //public static String GET_ACCT_LOAN_INFO_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        //    + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        //    + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
        //    + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
        //    + ROW_DLMT + "CCYCD" + COL_DLMT + "{CCYCD}"
        //    + ROW_DLMT + "OBALANCE" + COL_DLMT + "{OBALANCE}"
        //    + ROW_DLMT + "OPENDT" + COL_DLMT + "{OPENDT}"
        //    + ROW_DLMT + "EXPDT" + COL_DLMT + "{EXPDT}"
        //    + ROW_DLMT + "NEXT_INT_DUE" + COL_DLMT + "{NEXT_INT_DUE}"
        //    + ROW_DLMT + "NEXT_PRIN_DUE" + COL_DLMT + "{NEXT_PRIN_DUE}"
        //    + ROW_DLMT + "NEXT_AMT_PRIN_DUE" + COL_DLMT + "{NEXT_AMT_PRIN_DUE}"
        //    + ROW_DLMT + "NEXT_AMT_INT_DUE" + COL_DLMT + "{NEXT_AMT_INT_DUE}"
        //    + ROW_DLMT + "SINT" + COL_DLMT + "{SINT}"
        //     + ROW_DLMT + "ERR_CODE_RECORD_ACTIVITY" + COL_DLMT + "{ERR_CODE_RECORD_ACTIVITY}";
        //    + ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}";

        //          //retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);

        //         */
        //        //2. lay thong tin tai khoan vay
        //        if (ds != null && ds.Tables[0].Rows.Count == 1)
        //        {
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace("{CIF_NO}", custid);
        //            retStr = retStr.Replace("{ACCTNO}", acctno);
        //            retStr = retStr.Replace("{CCYCD}", ds.Tables[0].Rows[0]["CCYCD"].ToString());
        //            retStr = retStr.Replace("{OBALANCE}", ds.Tables[0].Rows[0]["OBALANCE"].ToString());
        //            retStr = retStr.Replace("{OUT_STANDING}", ds.Tables[0].Rows[0]["OUT_STANDING"].ToString());
        //            retStr = retStr.Replace("{OPENDT}", ds.Tables[0].Rows[0]["OPENDT"].ToString());
        //            retStr = retStr.Replace("{EXPDT}", ds.Tables[0].Rows[0]["EXPDT"].ToString());
        //            retStr = retStr.Replace("{NEXT_INT_DUE}", ds.Tables[0].Rows[0]["NEXT_INT_DUE"].ToString());
        //            retStr = retStr.Replace("{NEXT_PRIN_DUE}", ds.Tables[0].Rows[0]["NEXT_PRIN_DUE"].ToString());
        //            retStr = retStr.Replace("{NEXT_AMT_PRIN_DUE}", ds.Tables[0].Rows[0]["NEXT_AMT_PRIN_DUE"].ToString());
        //            retStr = retStr.Replace("{NEXT_AMT_INT_DUE}", ds.Tables[0].Rows[0]["NEXT_AMT_INT_DUE"].ToString());
        //            retStr = retStr.Replace("{SINT}", ds.Tables[0].Rows[0]["SINT"].ToString());


        //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N  CASE DONE GET HEADER ACCTNO:" + acctno);

        //        }
        //        else
        //        {
        //            retStr = Config.ERR_MSG_GENERAL;

        //            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N END CASE ELSE HEADER ACCTNO:" + acctno);

        //            return retStr;

        //        }

        //        //3. lay list 5 giao dich gan nhat

        //        Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N BEGIN GET LAST5 ACCTNO:" + acctno);

        //        ds = null;
        //        ds = da.GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N(custid, acctno, Config.ENQ_TYPE_LAST5, "", "");

        //        string tmpAmount = "0";
        //        if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            string strTemp = "";
        //            // RECORD_ACTIVITY:

        //            //TXDATE^AMT^CCYCD^TXDESC

        //            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
        //            {
        //                //tmpAmount = String.Format("{0:#,###0}", double.Parse( ds.Tables[0].Rows[j]["AMT"].ToString()));

        //                tmpAmount = String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["AMT"]);
        //                strTemp = strTemp +
        //                    ds.Tables[0].Rows[j]["TXDATE"].ToString() +
        //                    Config.COL_REC_DLMT +
        //                   // ds.Tables[0].Rows[j]["AMT"].ToString()
        //                   tmpAmount
        //                    + Config.COL_REC_DLMT +
        //                    ds.Tables[0].Rows[j]["CCYCD"].ToString()
        //                     + Config.COL_REC_DLMT +

        //                       //Funcs.NoHack(dt.Rows[j][CASA_TRAN.TXDESC].ToString())

        //                       Funcs.NoHack(ds.Tables[0].Rows[j]["TXDESC"].ToString())
        //                + Config.ROW_REC_DLMT;

        //            }
        //            strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

        //            retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
        //            retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);

        //        }
        //        else
        //        {
        //            retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);
        //        }

        //        Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N END GET LAST5 ACCTNO:" + acctno);

        //        return retStr;

        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;
        //        return retStr;
        //    }
        //}

        # endregion "GET_ACCT_LOAN_INFO_N OLD"

        #region "LN ENQUIRY"

        /// <summary>
        /// Lấy thông tin chi tiết 1 tài khoản vay (custid, acctno)
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public static string GET_ACCT_LOAN_INFO_N(Hashtable hashTbl)
        {

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            String retStr = Config.GET_ACCT_LOAN_INFO_N;

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N BEGIN ACCTNO:" + acctno);

            try
            {
                //1. Call function: GET_ACCT_LOAN_INFO_N

                AcctInfo.AcctInfoInqResType res = CoreIntegration.GetAccInfoByAccNo(custid, "LN", acctno);

                //ds format:
                /*
                 
        public static String GET_ACCT_LOAN_INFO_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
            + ROW_DLMT + "CCYCD" + COL_DLMT + "{CCYCD}"
            + ROW_DLMT + "OBALANCE" + COL_DLMT + "{OBALANCE}"
            + ROW_DLMT + "OPENDT" + COL_DLMT + "{OPENDT}"
            + ROW_DLMT + "EXPDT" + COL_DLMT + "{EXPDT}"
            + ROW_DLMT + "NEXT_INT_DUE" + COL_DLMT + "{NEXT_INT_DUE}"
            + ROW_DLMT + "NEXT_PRIN_DUE" + COL_DLMT + "{NEXT_PRIN_DUE}"
            + ROW_DLMT + "NEXT_AMT_PRIN_DUE" + COL_DLMT + "{NEXT_AMT_PRIN_DUE}"
            + ROW_DLMT + "NEXT_AMT_INT_DUE" + COL_DLMT + "{NEXT_AMT_INT_DUE}"
            + ROW_DLMT + "SINT" + COL_DLMT + "{SINT}"
             + ROW_DLMT + "ERR_CODE_RECORD_ACTIVITY" + COL_DLMT + "{ERR_CODE_RECORD_ACTIVITY}";
            + ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}";

                  //retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);

                 */
                //2. lay thong tin tai khoan vay

                if (res != null && res.AcctRec != null)
                {
                    // neu co dung 1 ban ghi thi moi lay ra thong tin 
                    if (res.AcctRec.Length > 0 && res.AcctRec[0].LoanAcctRec != null&& res.AcctRec.Length ==1)
                    {
                        AcctInfo.LoanAcctRecType loanAcc = res.AcctRec[0].LoanAcctRec;
                       
                        retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{CIF_NO}", custid);
                        retStr = retStr.Replace("{ACCTNO}", acctno);
                        retStr = retStr.Replace("{CCYCD}", loanAcc.CcyCd);// ds.Tables[0].Rows[0]["CCYCD"].ToString());
                        retStr = retStr.Replace("{OBALANCE}", loanAcc.OrgBal); // ds.Tables[0].Rows[0]["OBALANCE"].ToString());
                        retStr = retStr.Replace("{OUT_STANDING}", loanAcc.OutStanding);//ds.Tables[0].Rows[0]["OUT_STANDING"].ToString());
                        retStr = retStr.Replace("{OPENDT}", loanAcc.OpenDt);// ds.Tables[0].Rows[0]["OPENDT"].ToString());
                        retStr = retStr.Replace("{EXPDT}", loanAcc.ExpDt);//  ds.Tables[0].Rows[0]["EXPDT"].ToString());
                        retStr = retStr.Replace("{NEXT_INT_DUE}", loanAcc.NextIntDue);// ds.Tables[0].Rows[0]["NEXT_INT_DUE"].ToString());
                        retStr = retStr.Replace("{NEXT_PRIN_DUE}", loanAcc.NextPrinDue);//ds.Tables[0].Rows[0]["NEXT_PRIN_DUE"].ToString());
                        retStr = retStr.Replace("{NEXT_AMT_PRIN_DUE}", loanAcc.NextPrinDueAmt);// ds.Tables[0].Rows[0]["NEXT_AMT_PRIN_DUE"].ToString());
                        retStr = retStr.Replace("{NEXT_AMT_INT_DUE}", loanAcc.NextIntDueAmt);// ds.Tables[0].Rows[0]["NEXT_AMT_INT_DUE"].ToString());
                        retStr = retStr.Replace("{SINT}", loanAcc.Sint);// ds.Tables[0].Rows[0]["SINT"].ToString());


                        Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N  CASE DONE GET HEADER ACCTNO:" + acctno);

                    }
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;

                        Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N END CASE ELSE HEADER ACCTNO:" + acctno);

                        return retStr;

                    }


                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;

                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N END CASE ELSE HEADER ACCTNO:" + acctno);

                    return retStr;

                }




                //3. lay list 5 giao dich gan nhat

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N BEGIN GET LAST5 ACCTNO:" + acctno);

                //ds = null;
                //ds = da.GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N(custid, acctno, Config.ENQ_TYPE_LAST5, "", "");

                AcctHist.AcctHistInqResType res1 = CoreIntegration.GetAcctHist(custid, acctno, "LN", Config.ENQ_TYPE_LAST5, "", "");
                
                string tmpAmount = "0";
                if (res1 != null && res1.Transaction != null)
                {
                    string strTemp = "";
                    // RECORD_ACTIVITY:

                    //TXDATE^AMT^CCYCD^TXDESC

                    foreach (var item in res1.Transaction)
                    {
                        //tmpAmount = String.Format("{0:#,###0}", double.Parse( ds.Tables[0].Rows[j]["AMT"].ToString()));

                        tmpAmount = String.Format("{0:#,###0}", double.Parse(item.CrAmt.ToString())); ;// ds.Tables[0].Rows[j]["AMT"]);
                        strTemp = strTemp +
                            item.TxDt.ToString() + // ds.Tables[0].Rows[j]["TXDATE"].ToString() +
                            Config.COL_REC_DLMT +
                           // ds.Tables[0].Rows[j]["AMT"].ToString()
                           tmpAmount
                            + Config.COL_REC_DLMT +
                            item.Cur.ToString() //ds.Tables[0].Rows[j]["CCYCD"].ToString()                               
                             + Config.COL_REC_DLMT +
                            Funcs.NoHack( item.TxDesc)   //Funcs.NoHack(ds.Tables[0].Rows[j]["TXDESC"].ToString())  
                        + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    
                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);

                }
                else
                {
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);
                }

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LOAN_INFO_N END GET LAST5 ACCTNO:" + acctno);

                return retStr;

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        #region "GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N OLD"
        //public static string GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N(Hashtable hashTbl)
        //{

        //    string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");

        //    string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

        //    string enquiry_type = Funcs.getValFromHashtbl(hashTbl, "ENQUIRY_TYPE");

        //    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N BEGIN:" + acctno);


        //    String retStr = Config.GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N;
        //    try
        //    {
        //        //1. Call function: GET_ACCT_LOAN_INFO_N
        //        Enquirys da = new Enquirys();
        //        DataSet ds = new DataSet();

        //        //3. lay list giao dich theo enquiry type
        //        ds = null;
        //        ds = da.GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N(custid, acctno, enquiry_type, "", "");


        //        string tmpAmount = "0";

        //        if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            string strTemp = "";
        //            // RECORD_ACTIVITY:

        //            //TXDATE^AMT^CCYCD^TXDESC

        //            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
        //            {
        //                //tmpAmount = String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["AMT"].ToString());
        //                tmpAmount = String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["AMT"]);


        //                strTemp = strTemp +
        //                    ds.Tables[0].Rows[j]["TXDATE"].ToString() +
        //                    Config.COL_REC_DLMT +
        //                     //ds.Tables[0].Rows[j]["AMT"].ToString()
        //                     tmpAmount
        //                    + Config.COL_REC_DLMT +
        //                    ds.Tables[0].Rows[j]["CCYCD"].ToString()
        //                     + Config.COL_REC_DLMT +
        //                       //ds.Tables[0].Rows[j]["TXDESC"].ToString()
        //                       Funcs.NoHack(ds.Tables[0].Rows[j]["TXDESC"].ToString())
        //                                        + Config.ROW_REC_DLMT;

        //            }
        //            strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
        //            retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);

        //        }
        //        else
        //        {
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
        //            retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);
        //        }

        //        Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N END:" + acctno);

        //        return retStr;

        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;
        //        return retStr;
        //    }
        //}

        #endregion "GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N OLD"

        public static string GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N(Hashtable hashTbl)
        {

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            string enquiry_type = Funcs.getValFromHashtbl(hashTbl, "ENQUIRY_TYPE");

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N BEGIN:" + acctno);


            String retStr = Config.GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N;
            try
            {
                //1. Call function: GET_ACCT_LOAN_INFO_N
                //enquiry loan hist: acct_type = LN
                AcctHist.AcctHistInqResType res = CoreIntegration.GetAcctHist(custid, acctno, "LN", enquiry_type, "", "");
                
                string tmpAmount = "0";

                if (res != null && res.Transaction != null)
                {
                    string strTemp = "";
                    // RECORD_ACTIVITY:

                    //TXDATE^AMT^CCYCD^TXDESC

                    foreach (var item in res.Transaction)
                    {
                        //tmpAmount = String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["AMT"].ToString());
                        //tmpAmount = String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["AMT"]);

                        tmpAmount = String.Format("{0:#,###0}", double.Parse(item.CrAmt));
                        
                        strTemp = strTemp +
                            //ds.Tables[0].Rows[j]["TXDATE"].ToString() +
                            item.TxDt +
                            Config.COL_REC_DLMT +
                            //ds.Tables[0].Rows[j]["AMT"].ToString()
                             tmpAmount 
                            + Config.COL_REC_DLMT +
                            //ds.Tables[0].Rows[j]["CCYCD"].ToString()
                            item.Cur
                             + Config.COL_REC_DLMT +
                               //ds.Tables[0].Rows[j]["TXDESC"].ToString()
                               //Funcs.NoHack(ds.Tables[0].Rows[j]["TXDESC"].ToString())
                               Funcs.NoHack( item.TxDesc)
                                                + Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD_ACTIVITY}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_DONE);
                }
                else
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_GENERAL);
                    retStr = retStr.Replace("{ERR_CODE_RECORD_ACTIVITY}", Config.ERR_CODE_GENERAL);
                }
                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N END:" + acctno);

                return retStr;

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        #region "GET_ACCT_LN_REPAYMENT_SCHEDULE OLD"
        //public static string GET_ACCT_LN_REPAYMENT_SCHEDULE(Hashtable hashTbl)
        //{

        //    string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");

        //    string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

        //    // string enquiry_type = Funcs.getValFromHashtbl(hashTbl, "ENQUIRY_TYPE");

        //    String retStr = Config.GET_ACCT_LN_REPAYMENT_SCHEDULE;

        //    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_REPAYMENT_SCHEDULE BEGIN:" + acctno);

        //    try
        //    {
        //        //1. Call function: GET_ACCT_LOAN_INFO_N
        //        Enquirys da = new Enquirys();
        //        DataSet ds = new DataSet();

        //        //3. lay list giao dich theo enquiry type
        //        ds = null;
        //        ds = da.listRepaymentSchedule(custid, acctno, Config.ENQ_TYPE_LAST5, "", "");

        //        if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            string strTemp = "";
        //            // RECORD_ACTIVITY:

        //            /*
        //             Ngày đến hạn ^ Gốc đến hạn ^ lãi đến hạn ^ khoản khác ^ Tổng đến hạn ^ Gốc còn lại

        //             EXPDT: Ngày đến hạn
        //             NEXT_INT_DUE: Ngày trả lãi kế tiếp (Chỉ hiển thị trường này)
        //             NEXT_AMT_PRIN_DUE: Gốc đến hạn
        //             NEXT_AMT_INT_DUE: Lãi đến hạn
        //             NEXT_AMT_OTHER_DUE: Khoản khác
        //             SINT: Tổng đến hạn
        //             OUTSTANDING_BALANCE: Dư nợ gốc (gốc còn lại)

        //              */

        //            //tmpAmount = String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["AMT"].ToString());

        //            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
        //            {
        //                strTemp = strTemp +
        //                    ds.Tables[0].Rows[j]["EXPDT"].ToString() + Config.COL_REC_DLMT +
        //                    String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["NEXT_AMT_PRIN_DUE"].ToString())
        //                    //ds.Tables[0].Rows[j]["NEXT_AMT_PRIN_DUE"].ToString() 
        //                    + Config.COL_REC_DLMT +
        //                      String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["NEXT_AMT_INT_DUE"].ToString())
        //                      //ds.Tables[0].Rows[j]["NEXT_AMT_INT_DUE"].ToString()                               
        //                      + Config.COL_REC_DLMT +
        //                      String.Format("{0:#,###0}", (ds.Tables[0].Rows[j]["NEXT_AMT_OTHER_DUE"] == DBNull.Value ? "0" : ds.Tables[0].Rows[j]["NEXT_AMT_OTHER_DUE"].ToString()))

        //                //ds.Tables[0].Rows[j]["NEXT_AMT_OTHER_DUE"].ToString() 
        //                + Config.COL_REC_DLMT +
        //                    String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["SINT"].ToString())
        //                //ds.Tables[0].Rows[j]["SINT"].ToString() 
        //                + Config.COL_REC_DLMT +
        //                String.Format("{0:#,###0}", ds.Tables[0].Rows[j]["OUTSTANDING_BALANCE"].ToString())
        //                //ds.Tables[0].Rows[j]["OUTSTANDING_BALANCE"].ToString()
        //                + Config.ROW_REC_DLMT;

        //            }
        //            strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
        //            retStr = retStr.Replace("{RECORD}", strTemp);
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

        //        }
        //        else
        //        {
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_MSG_NO_DATA_FOUND);
        //        }

        //        Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_REPAYMENT_SCHEDULE END:" + acctno);

        //        return retStr;

        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;
        //        return retStr;
        //    }
        //}
        #endregion "GET_ACCT_LN_REPAYMENT_SCHEDULE OLD"

        public static string GET_ACCT_LN_REPAYMENT_SCHEDULE(Hashtable hashTbl)
        {

            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string acctno = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            String retStr = Config.GET_ACCT_LN_REPAYMENT_SCHEDULE;

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_REPAYMENT_SCHEDULE BEGIN:" + acctno);

            try
            {
                AcctPaySched.AcctPaySchedInqResType res = CoreIntegration.getPaymentScheduleDate(custid, acctno, Config.ENQ_TYPE_LAST5, "", "");

                if (res != null && res.AcctPaySchRec != null)
                {
                    string strTemp = string.Empty;

                    foreach (var item in res.AcctPaySchRec)
                    {
                        strTemp = strTemp + item.ExpDt + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.NextAmtPrinDue) ? "0" : string.Format("{0:#,###0}", item.NextAmtPrinDue)) + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.NextAmtIntDue) ? "0" : string.Format("{0:#,###0}", item.NextAmtIntDue)) + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.NextAmtOtherDue) ? "0" : string.Format("{0:#,###0}", item.NextAmtOtherDue)) + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.SIntAmt) ? "0" : string.Format("{0:#,###0}", item.SIntAmt)) + Config.COL_REC_DLMT
                            + (string.IsNullOrEmpty(item.OutStandingBal) ? "0" : string.Format("{0:#,###0}",item.OutStandingBal)) + Config.COL_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                }
                else
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_MSG_NO_DATA_FOUND);
                }

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LN_REPAYMENT_SCHEDULE END:" + acctno);

                return retStr;

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }
        #endregion "LN ENQUIRY"



        /// <summary>
        /// linhtn: 20160807 lay danh sach tai khoan chuyen
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="acctno"></param>
        /// <param name="ccycd"></param>
        /// <returns></returns>

        /*
         //OLD: LAY DANH SACH TAI KHOAN CHUYEN: LAY THEO LOAI TIEN, TRANG THAI NORMAL & NO CREDIT

             public static string GET_ACCT_BALANCE_LIST_N(string custid, string acctno, string ccycd)
        {

            String retStr = Config.GET_ACCT_BALANCE_LIST_N;

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_LIST_N BEGIN:" + acctno);

            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Enquirys da = new Enquirys();
                DataSet ds = new DataSet();
                ds = da.GET_ACCT_BALANCE_LIST_N(custid, acctno, ccycd);

                //2. Gen reply message
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retStr = Config.GET_ACCT_LIST;
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{CIF_NO}", custid);

                    string strTemp = "";
                    //string tmpAmount = "0";
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {

                        // tmpAmount = Double.Parse(ds.Tables[0].Rows[j][CASA_TRAN.DAMT].ToString()) > 0 ? "-" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.DAMT]) : "+" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.CAMT]);

                        strTemp = strTemp +
                            ds.Tables[0].Rows[j][CASA_BALANCE.ACCTNO].ToString() + Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j][CASA_BALANCE.CCYCD].ToString() + Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j][CASA_BALANCE.PRODUCT_DESC].ToString() + Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j][CASA_BALANCE.CUR_BAL].ToString() + Config.COL_REC_DLMT +
                              ds.Tables[0].Rows[j][CASA_BALANCE.AVAI_BAL].ToString() + Config.COL_REC_DLMT +
                             Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                   
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    

                }

                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_LIST_N END:" + acctno);

                return retStr;


            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }
             */
        public static string GET_ACCT_BALANCE_LIST_N(string custid, string acctno, string ccycd, string overdraft)
        {

            String retStr = Config.GET_ACCT_BALANCE_LIST_N;

            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_LIST_N BEGIN:" + acctno);
            try
            {
                /*OLD*/

                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                /*
                Enquirys da = new Enquirys();
                DataSet ds = new DataSet();
                ds = da.GET_ACCT_LIST_RECEIVE(custid, acctno, ccycd);
                */

                DataSet ds = new DataSet();
                AccList.AcctListInqResType resp = new AccList.AcctListInqResType();
                CardList.CardListInqResType respCard = new CardList.CardListInqResType();
                //string[] finSts = new string[2] { Config.FinancialStatus.NORMAL, Config.FinancialStatus.NO_CREDIT };
                string[] custIdArr = new string[1] { custid };

                //LAY TAT CA CAC TAI KHOAN CASA CUA KHACH HANG
                resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.TYPE_CASA_PRODUCT);

				respCard = CardIntegration.getCardList(custid);

                //2. Gen reply message
                //OLD
                //if (ds != null && ds.Tables[0].Rows.Count > 0)

                if (resp != null && resp.AcctRec != null)
                {
                    retStr = Config.GET_ACCT_LIST_RECEIVE;
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{CIF_NO}", custid);

                    string strTemp = "";

                    //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)

                    foreach (AccList.AcctRecType item in resp.AcctRec)
                    {
                        // tmpAmount = Double.Parse(ds.Tables[0].Rows[j][CASA_TRAN.DAMT].ToString()) > 0 ? "-" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.DAMT]) : "+" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.CAMT]);
                        //CHI LAY TAI KHOAN CASA STATUS = ACTIVE && TRANG THAI TAI CHINH LA NORMAL HOAC NO DEBIT
                        //CHI LAY LOAI TIEN TAI KHOAN = CCYCD
                        if (((item.AcctFinSts == Config.FinancialStatus_NORMAL_CASA)
                                    || (item.AcctFinSts == Config.FinancialStatus_NO_CREDIT_CASA))
                                 && item.InactSt == Config.AccountStatus_ACTIVE_CASA
                                 && item.BankAcctId.AcctCur == Config.CCYCD_VND
                                 && !Config.IS_CO_HOLDER.Equals(item.IsCoholder)
                        )
                        {
                            //OLD
                            //strTemp = strTemp +
                            //ds.Tables[0].Rows[j][CASA_BALANCE.ACCTNO].ToString() + Config.COL_REC_DLMT +
                            //ds.Tables[0].Rows[j][CASA_BALANCE.CCYCD].ToString() + Config.COL_REC_DLMT +
                            //ds.Tables[0].Rows[j][CASA_BALANCE.PRODUCT_DESC].ToString() + Config.COL_REC_DLMT +
                            // ds.Tables[0].Rows[j][CASA_BALANCE.CUR_BAL].ToString() + Config.COL_REC_DLMT +
                            //  ds.Tables[0].Rows[j][CASA_BALANCE.AVAI_BAL].ToString() + Config.COL_REC_DLMT +
                            // Config.ROW_REC_DLMT;
								
							if ("1".Equals(overdraft) && (item.ProdCD == "125" || item.ProdCD == "126"))
                            {
                                continue;
                            }
								
                            strTemp = strTemp +
                            item.BankAcctId.AcctId //SO TAI KHOAN
                            + Config.COL_REC_DLMT +
                            item.BankAcctId.AcctCur //LOAI TIEN
                            + Config.COL_REC_DLMT +
                            item.ProdDesc  //TEN SAN PHAM
                            + Config.COL_REC_DLMT +
                            item.CurBal // SO DU HIEN TAI CUR BAL
                             + Config.COL_REC_DLMT +
                            item.AvailBal //SO DU KHA DUNG AVAI BAL                      
                             + Config.COL_REC_DLMT
                             + "CASA"
                             + Config.ROW_REC_DLMT;
                        }
                    }

                    if (respCard != null && respCard.CardRec != null)
                    {
                        foreach (CardList.CardListInqResTypeCardRec item in respCard.CardRec)
                        {
                            // tmpAmount = Double.Parse(ds.Tables[0].Rows[j][CASA_TRAN.DAMT].ToString()) > 0 ? "-" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.DAMT]) : "+" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.CAMT]);
                            //CHI LAY TAI KHOAN CASA STATUS = ACTIVE && TRANG THAI TAI CHINH LA NORMAL HOAC NO DEBIT
                            //CHI LAY LOAI TIEN TAI KHOAN = CCYCD
                            if (item.CardInfo.CardType.Equals("CREDIT") && item.CardInfo.CardSts.Equals("0"))
                            {
                                
                                strTemp = strTemp +
                                Funcs.MaskCardNo(item.CardInfo.CardId) //card num masking
                                + Config.COL_REC_DLMT
                                + "" //LOAI TIEN
                                + Config.COL_REC_DLMT
                                + item.CardInfo.MadAmt // SO DU HIEN TAI CUR BAL
                                 + Config.COL_REC_DLMT +
                                item.CardInfo.VailBal //SO DU KHA DUNG AVAI BAL            
                                 + Config.COL_REC_DLMT
                                 + item.CardInfo.CardId
                                 + Config.COL_REC_DLMT
                                 + "CREDIT"
                                 + Config.ROW_REC_DLMT;

                            }
                        }

                        Funcs.WriteLog("custid:" + custid + "|GET_CARDLIST_ END:" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(respCard)));
                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_LIST_N NO DATA FOUND:" + acctno);
                }
                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_BALANCE_LIST_N END:" + acctno);
                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        /// <summary>
        /// Linhtn: lay danh sach tai khoan nhan 
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="acctno"></param>
        /// <param name="ccycd"></param>
        /// <returns></returns>
        public static string GET_ACCT_LIST_RECEIVE(string custid, string acctno, string ccycd)
        {

            String retStr = Config.GET_ACCT_LIST_RECEIVE;
            Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_RECEIVE BEGIN:" + acctno);
            try
            {
                /*OLD*/               
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                /*
                Enquirys da = new Enquirys();
                DataSet ds = new DataSet();
                ds = da.GET_ACCT_LIST_RECEIVE(custid, acctno, ccycd);
                */

                DataSet ds = new DataSet();
                AccList.AcctListInqResType resp = new AccList.AcctListInqResType();

                //string[] finSts = new string[2] { Config.FinancialStatus.NORMAL, Config.FinancialStatus.NO_CREDIT };
                string[] custIdArr = new string[1] { custid};

                //LAY TAT CA CAC TAI KHOAN CASA CUA KHACH HANG
                resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.TYPE_CASA_PRODUCT);                

                //2. Gen reply message
                //OLD
                //if (ds != null && ds.Tables[0].Rows.Count > 0)

                if (resp != null && resp.AcctRec != null)
                {
                    retStr = Config.GET_ACCT_LIST_RECEIVE;
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{CIF_NO}", custid);

                    string strTemp = "";

                    //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)

                    foreach (AccList.AcctRecType item in resp.AcctRec)
                    {

                        // tmpAmount = Double.Parse(ds.Tables[0].Rows[j][CASA_TRAN.DAMT].ToString()) > 0 ? "-" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.DAMT]) : "+" + String.Format("{0:#,###0}", ds.Tables[0].Rows[j][CASA_TRAN.CAMT]);

                        //CHI LAY TAI KHOAN CASA STATUS = ACTIVE && TRANG THAI TAI CHINH LA NORMAL HOAC NO DEBIT
                        //CHI LAY LOAI TIEN TAI KHOAN = CCYCD
                        if (((item.AcctFinSts == Config.FinancialStatus_NORMAL_CASA)
                                    || (item.AcctFinSts == Config.FinancialStatus_NO_DEBIT_CASA))
                                 && item.InactSt == Config.AccountStatus_ACTIVE_CASA
                                 && item.BankAcctId.AcctCur == Config.CCYCD_VND
                        )
                        {
                            //OLD
                            //strTemp = strTemp +
                            //ds.Tables[0].Rows[j][CASA_BALANCE.ACCTNO].ToString() + Config.COL_REC_DLMT +
                            //ds.Tables[0].Rows[j][CASA_BALANCE.CCYCD].ToString() + Config.COL_REC_DLMT +
                            //ds.Tables[0].Rows[j][CASA_BALANCE.PRODUCT_DESC].ToString() + Config.COL_REC_DLMT +
                            // ds.Tables[0].Rows[j][CASA_BALANCE.CUR_BAL].ToString() + Config.COL_REC_DLMT +
                            //  ds.Tables[0].Rows[j][CASA_BALANCE.AVAI_BAL].ToString() + Config.COL_REC_DLMT +
                            // Config.ROW_REC_DLMT;

                          strTemp = strTemp +
                          item.BankAcctId.AcctId //SO TAI KHOAN
                          + Config.COL_REC_DLMT +
                          item.BankAcctId.AcctCur //LOAI TIEN
                          + Config.COL_REC_DLMT +
                          item.ProdDesc  //TEN SAN PHAM
                          + Config.COL_REC_DLMT +
                          item.CurBal // SO DU HIEN TAI CUR BAL
                           + Config.COL_REC_DLMT +
                          item.AvailBal //SO DU KHA DUNG AVAI BAL                      
                           + Config.ROW_REC_DLMT;
                        }                       
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_RECEIVE NO DATA FOUND:" + acctno);
                }
                Funcs.WriteLog("custid:" + custid + "|GET_ACCT_LIST_RECEIVE END:" + acctno);
                return retStr;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        #region "GET_BENNAME_FROM_CASA_ACCOUNT OLD"
        //public static string GET_BENNAME_FROM_CASA_ACCOUNT(string des_acct)
        //{

        //    String retStr = Config.GET_BENNAME_FROM_CASA_ACCOUNT;

        //    try
        //    {
        //        Enquirys da = new Enquirys();
        //        DataSet ds = new DataSet();
        //        ds = da.GET_BENNAME_FROM_CASA_ACCOUNT(des_acct);

        //        if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace(Config.ERR_DESC_VAL, "GET ACCTNAME SUCCESSFUL");
        //            retStr = retStr.Replace("{DES_ACCT}", des_acct);
        //            retStr = retStr.Replace("{DES_NAME}", ds.Tables[0].Rows[0]["CUSTNAME"].ToString());
        //        }
        //        else
        //        {
        //            retStr = Config.ERR_MSG_GENERAL;
        //            return retStr;
        //        }
        //        return retStr;
        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());
        //        retStr = Config.ERR_MSG_GENERAL;
        //        return retStr;
        //    }
        //}

        #endregion "GET_BENNAME_FROM_CASA_ACCOUNT OLD"
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="des_acct"></param>
        /// <returns></returns>
        public static string GET_BENNAME_FROM_CASA_ACCOUNT(string des_acct)
        {
            string retStr = Config.ERR_MSG_GENERAL;
            try
            {
                //Enquirys da = new Enquirys();
                //DataSet ds = new DataSet();
                //ds = da.GET_BENNAME_FROM_CASA_ACCOUNT(des_acct);
                //AccList.AcctListInqResType resp = new AccList.AcctListInqResType();
                //string[] custIdArr = new string[1] { custid };
                //resp = CoreIntegration.getAllAccountByConditions(custIdArr, "CIF", Config.TYPE_CASA_PRODUCT);

                string des_name = CoreIntegration.getCASAAccountNameInfo(des_acct, "*"); 

                if (des_name != string.Empty)
                {
                    retStr = Config.GET_BENNAME_FROM_CASA_ACCOUNT;
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "GET ACCTNAME SUCCESSFUL");
                    retStr = retStr.Replace("{DES_ACCT}", des_acct);
                    retStr = retStr.Replace("{DES_NAME}", des_name);// ds.Tables[0].Rows[0]["CUSTNAME"].ToString());
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
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        /*
        //anhnd2 06/2015
        // Ktra tai khoan chuyen va CIF co trung nhau khong.
        private bool CustIdMatchScrAcct(string cif, string ScrAcct)
        {
            bool result = false;
            try
            {
                BalanceData balData = new BalanceData();
                Balance balance = new Balance();

                balData = balance.getACCTLST_CUSTID(cif, Config.prod_cd_CASA);
                if ((balData != null) && (balData.Tables[0].Rows.Count > 0))
                {
                    for (int j = 0; j < balData.Tables[BalanceData.BALANCE_TABLE].Rows.Count; j++)
                    {
                        Funcs.WriteLog(balData.Tables[BalanceData.BALANCE_TABLE].Rows[j][BalanceData.ACCT_NO_FIELD].ToString());
                        if (ScrAcct == balData.Tables[BalanceData.BALANCE_TABLE].Rows[j][BalanceData.ACCT_NO_FIELD].ToString())   // TK chuyen co thuoc cif
                        {
                            //
                            result = true;
                            break;
                        }
                    }

                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return false;
            }

        }
        */

        public static string GetFieldName (string input, string lang)
        {
            string str = "";

            foreach (var item in Config.RET_FIELD_NAME)
            {
                if (item.Split('|')[0].Equals(input))
                {
                    if ("VI".Equals(lang.ToUpper()))
                    {
                        str = item.Split('|')[1];
                    }
                    else
                    {
                        str = item.Split('|')[2];
                    }
                }
                
            }

            return str;
        }

        public static string GetMessage(string key, string value)
        {
            string str = "";
            str = GetFieldName(key, "VI")
                            + Config.COL_REC_DLMT
                            + GetFieldName(key, "EN")
                            + Config.COL_REC_DLMT
                            + ( !string.IsNullOrEmpty(value) ? value : "_NULL_" )
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
            return str;
        }

        public static string GetMessage(string key, double value, string ccy_cd)
        {
            string str = "";
            str = GetFieldName(key, "VI")
                            + Config.COL_REC_DLMT
                            + GetFieldName(key, "EN")
                            + Config.COL_REC_DLMT
                            + Funcs.ConvertMoney(value.ToString()) + " " + ccy_cd
                            + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
            return str;
        }

    }
}