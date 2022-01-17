using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;
using mobileGW.Service.Bussiness;
using mobileGW.Service.DataAccess;
using System.Collections.Generic;
using System.Xml;
using mobileGW.Service.Models;
using System.Globalization;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Utility
/// </summary>


namespace mobileGW.Service.AppFuncs
{
    public class Payments
    {
        private OracleCommand dsCmd;
        private OracleDataAdapter dsApt;
        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (dsApt != null)
            {
                if (dsApt.SelectCommand != null)
                {
                    if (dsApt.SelectCommand.Connection != null)
                    {
                        dsApt.SelectCommand.Connection.Dispose();
                    }
                    dsApt.SelectCommand.Dispose();
                }
                dsApt.Dispose();
                dsApt = null;
            }
        }

        #endregion
        public Payments()
        {
            //
            dsApt = new OracleDataAdapter();
            //
        }


        /// <summary>
        /// TRAN_TYPE: TOPUP_MOBILE, TOPUP_OTHER, BILL_MOBILE, BILL_OTHER
        /// 2 TRAN_TYPE tong hop: TOPUP_MOBILE_OTHER, BILL_MOBILE_OTHER
        /// </summary>
        /// <param name="tran_type"></param>
        /// <returns></returns>
        public DataTable GET_CATEGORY_BY_TRAN_TYPE(string tran_type)
        {
            try
            {
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_CAT_LIST", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("tran_type", OracleDbType.Varchar2, tran_type, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Linhtn: 09 jul 2016
        /// Get list services by category id
        /// Trả luôn về partner_code, category_code, service_code trong dataset
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns></returns>
        public DataTable GET_SERVICES(string category_id, string tran_type)
        {

            try
            {
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_SERVICES ", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("category_id", OracleDbType.Varchar2, category_id, ParameterDirection.Input);
                dsCmd.Parameters.Add("tran_type", OracleDbType.Varchar2, tran_type, ParameterDirection.Input);

                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        /// <summary>
        /// Từ category = TOPUP_MOBILE, mobile_number --> list price: price|price_discount
        /// Lấy giá lấy luôn thêm PARTNER, CATEGORY_CODE, PROVIDER_CODE để định tuyến
        /// </summary>
        /// <param name="category_id"></param>
        /// <param name="mobile_no"></param>
        /// <returns></returns>
        public DataTable GET_LIST_MOBILE_PRICE(string mobile_no)
        {

            try
            {
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_LIST_MOBILE_PRICE ", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("mobile_no", OracleDbType.Varchar2, mobile_no, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        /// <summary>
        /// Lay menh gia loai nap tien khac linhtn 26jul2016
        /// </summary>
        /// <param name="tran_type"></param>
        /// <param name="category_id"></param>
        /// <param name="service_id"></param>
        /// <returns></returns>
        public DataTable GET_PRICE_TOPUP_OTHER(string tran_type, string category_id, string service_id)
        {

            try
            {
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_PRICE_TOPUP_OTHER ", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("tran_type", OracleDbType.Varchar2, tran_type, ParameterDirection.Input);
                dsCmd.Parameters.Add("category_id", OracleDbType.Varchar2, category_id, ParameterDirection.Input);
                dsCmd.Parameters.Add("service_id", OracleDbType.Varchar2, service_id, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }


        }


        public DataTable getServicePartner_byBill(string bill_id, string tran_type)
        {
            try
            {

                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GETSERVICE_MOBI", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("V_MOBILE", OracleDbType.Varchar2, bill_id, ParameterDirection.Input);
                dsCmd.Parameters.Add("V_TYPE", OracleDbType.Varchar2, tran_type, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        public string getNextTranId()
        {
            try
            {

                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "pkg_payment_new.GET_TRAN_ID", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("V_TYPE", OracleDbType.Varchar2, ParameterDirection.Input);
                dsApt.SelectCommand = dsCmd;
                dsCmd.Parameters.Add("MY_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.Parameters["V_TYPE"].Value = "NOTQRY";

                dsApt.Fill(ds);
                if ((ds.Tables[0] != null) && (ds.Tables[0].Rows.Count > 0))
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("*[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tranid"></param>
        /// <param name="custid"></param>
        /// <param name="src_acct"></param>
        /// <param name="amount"></param>
        /// <param name="order_id"> so dien thoai, tai khoan, ....</param>
        /// <param name="partner_id"></param>
        /// <param name="category_code"></param>
        /// <param name="service_code"></param>
        /// <param name="v_txdesc"></param>
        /// <param name="price"></param>
        /// <param name="extra_info"> trường hơp topup nganluong phai truyen invoice_code chu khong phai order id</param>
        /// <param name="info_ref"></param>
        /// <returns></returns>
        public String postTopupToPartner(
            double tranid
            , string custid //for log only
            , string src_acct
            , double amount
            , string order_id
            , string partner_id
            , string category_code
            , string service_code
            , string v_txdesc
            , string price
            , object extra_info
            , ref string info_ref
        )
        {
            string ret = string.Empty;
            string retws = string.Empty;
            string finalRet = string.Empty;

            Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|TOPUP BEGIN SWITCH PARTNER"
                + "|ORDER_ID:" + order_id
                + "|CATEGORY_CODE:" + category_code
                + "|SERVICE_CODE:" + service_code
                 + "|AMOUNT:" + amount.ToString()
                );

            #region "NAPAS VAS"
            if (partner_id == Config.PARTNER_ID_VAS)
            {
                Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|begin call ws");
                try
                {
                    ret = new TopupBillingIntergrator().postMessagePaymentToNapas(custid, src_acct, amount, tranid, service_code, order_id);
                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|parse retws ret_code:" + ret);

                    if (ret == Config.RET_CODE_NAPAS_TOPUP[0].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = ret; //ket qua tra ve
                        return finalRet;
                    }
                    else if (ret == Config.RET_CODE_NAPAS_TOPUP[1].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|PROCESS CASE TIMEOUT");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = ret; //ket qua tra ve
                        return finalRet;
                    }
                    else
                    {

                        //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
                        for (int i = 3; i < Config.RET_CODE_NAPAS_TOPUP.Length; i++)
                        {
                            info_ref = ret;
                            if (ret == Config.RET_CODE_NAPAS_TOPUP[i].Split('|')[0])
                            {
                                info_ref = Config.RET_CODE_NAPAS_TOPUP[i];

                                finalRet = Config.ERR_CODE_REVERT;
                                return finalRet;
                            }
                        }

                        // khong co trong bang ma loi thi ko revert
                        info_ref = Config.RET_CODE_NAPAS_TOPUP[2]; //LOI CHUNG CHUNG
                        finalRet = Config.ERR_CODE_GENERAL;
                        return finalRet;

                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                //TRA VE MA LOI TIMEOUT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|exception:" + ex.ToString());
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    return finalRet;
                }


            } //endif VAS
            #endregion "NAPAS VAS"

            #region "VNPAY TOPUP"
            else if (partner_id == Config.PARTNER_ID_VNPAY)
            {
                try
                {
                    retws = string.Empty;
                    finalRet = string.Empty;

                    string retVnpay = new TopupBillingIntergrator().postMessagePaymentToVNPay(custid, string.Empty, string.Empty, order_id, tranid, amount, Config.gVNPAY_TRAN_TYPE_000000);
                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|parse retws ret_code:" + retVnpay);

                    if (retVnpay == Config.RET_CODE_VNPAY[0].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = retVnpay; //ket qua tra ve
                        return finalRet;
                    }
                    else if (retVnpay == Config.RET_CODE_VNPAY[1].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|PROCESS CASE TIMEOUT");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = retVnpay; //ket qua tra ve
                        return finalRet;
                    }
                    else
                    {
                        //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
                        for (int i = 3; i < Config.RET_CODE_VNPAY.Length; i++)
                        {
                            info_ref = retVnpay;
                            if (retVnpay.Equals(Config.RET_CODE_VNPAY[i].Split('|')[0]))
                            {
                                info_ref = Config.RET_CODE_VNPAY[i];

                                finalRet = Config.ERR_CODE_REVERT;

                            }
                        }

                        if (String.IsNullOrEmpty(finalRet))
                        {
                            info_ref = Config.RET_CODE_VNPAY[2];
                            return Config.ERR_CODE_GENERAL;
                        }
                        else
                        {
                            return finalRet;
                        }
                    }
                }
                //EXCEPTION KHONG REVERT 
                //TRA MA LOI = TIMEOUT
                catch (Exception ex)
                {
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|EX " + ex.ToString());
                    return finalRet;
                }
            } //endif VNPAY
            #endregion "VNPAY TOPUP"

            #region ONEPAY TOPUP
            // if ONEPAY
            //Linhtn: add new 20160908
            //onepay cung ho tro ca TOPUP
            //phan biet TOPUP - BILLING BANG PROVIDER_CODE
            else if (partner_id == Config.PARTNER_ID_ONEPAY)
            {
                return finalRet;
                //try
                //{
                //    retws = "";
                //    finalRet = "";
                //    ret = "";
                //    onepay.OnepayPaymentSS myGW = new onepay.OnepayPaymentSS();
                //    myGW.Url = Config.URL_WS_ONEPAY_TOPUP;
                //    myGW.Timeout = Config.TIMEOUT_WITH_ONEPAY;
                //    string retXML = "";
                //    //goi ws onepay
                //    retXML = myGW.SHB_OneBill_Payment(order_id
                //        , category_code
                //        , amount, tranid.ToString(), "6014", "MOB", "SHB", "BILL PAY", service_code);
                //    myGW.Dispose();
                //    XmlDocument XmlDoc = new XmlDocument();
                //    XmlDoc.LoadXml(retXML);
                //    //public string funcPOSTPAID(String Provider, String vCustCode ,Double vPrice, Double TranId, ref string Parm, String partner_code, string category_code)
                //    ret = XmlDoc.GetElementsByTagName("ResponseCode")[0].InnerText;
                //    if (ret == Config.RET_CODE_ONEPAY[0].Split('|')[0])
                //    {
                //        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");
                //        finalRet = Config.ERR_CODE_DONE;
                //        info_ref = Config.RET_CODE_ONEPAY[0]; //ket qua tra ve
                //        return finalRet;
                //    }
                //    else if (ret == Config.RET_CODE_ONEPAY[1].Split('|')[0])
                //    {
                //        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE TIMEOUT");
                //        finalRet = Config.ERR_CODE_TIMEOUT;
                //        info_ref = Config.RET_CODE_ONEPAY[1]; //ket qua tra ve
                //        return finalRet;
                //    }
                //    else
                //    {
                //        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE OTHER ERROR");
                //        //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
                //        for (int i = 3; i < Config.RET_CODE_ONEPAY.Length; i++)
                //        {
                //            info_ref = ret;
                //            if (ret == Config.RET_CODE_ONEPAY[i].Split('|')[0])
                //            {
                //                info_ref = Config.RET_CODE_ONEPAY[i];
                //                finalRet = Config.ERR_CODE_REVERT;
                //                return finalRet;
                //            }
                //        }
                //        // khong co trong bang ma loi thi ko revert
                //        info_ref = Config.RET_CODE_ONEPAY[1];
                //        finalRet = Config.ERR_CODE_GENERAL;
                //        return finalRet;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    finalRet = Config.ERR_CODE_GENERAL;
                //    Funcs.WriteLog(ex.ToString());
                //    return finalRet;
                //}
            } //endif ONEPAY
            #endregion

            #region "NLUONG TOPUP"
            else if (partner_id == Config.PARTNER_ID_NLUONG)
            {
                Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|begin call ws");

                try
                {
                    //SMLBILLGW.Service myWS = new SMLBILLGW.Service();
                    //myWS.Url = Config.URL_WS_NAPAS_VAS_TOPUP;
                    //myWS.Timeout = Config.TIMEOUT_WITH_VAS;

                    //Nganluong.Services myGW = new Nganluong.Services();

                    //myGW.Url = Config.URL_WS_NLUONG_TOPUP;
                    //myGW.Timeout = Config.TIMEOUT_WITH_NLUONG;

                    //// xu ly rieng ngan luong se phai truyen invoice code
                    //retws = myGW.SHBAPP_UPTBILL(Config.payment_NLUONG_gwUsername, Config.payment_NLUONG_gwPassword
                    //    , extra_info, Config.INV_STATUS_DONE, (decimal) tranid);
                    ////giai phong bo nho 
                    //myGW.Dispose();

                    if (extra_info == null)
                    {
                        return Config.ERR_CODE_GENERAL;
                    }

                    NLBillPmt.NLBillPmtVerifyResType invoiceObject = null;
                    if (extra_info != null)
                        invoiceObject = (NLBillPmt.NLBillPmtVerifyResType)extra_info;

                    retws = new TopupBillingIntergrator().postMessagePaymentToNganLuong(custid, invoiceObject, Config.INV_STATUS_DONE, tranid.ToString());

                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|end call ws|retws:" + retws);
                    ret = retws;

                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|parse retws ret_code:" + ret);
                    if (ret == Config.RET_CODE_NLUONG[0].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = ret; //ket qua tra ve
                        return finalRet;
                    }
                    else if (ret == Config.RET_CODE_NLUONG[1].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|PROCESS CASE TIMEOUT");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = ret; //ket qua tra ve
                        return finalRet;
                    }
                    else
                    {
                        //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
                        for (int i = 3; i < Config.RET_CODE_NLUONG.Length; i++)
                        {
                            info_ref = ret;
                            if (ret == Config.RET_CODE_NLUONG[i].Split('|')[0])
                            {
                                info_ref = Config.RET_CODE_NLUONG[i];

                                finalRet = Config.ERR_CODE_REVERT;
                                return finalRet;
                            }
                        }
                        // khong co trong bang ma loi thi ko revert
                        info_ref = Config.RET_CODE_NLUONG[2]; //LOI CHUNG CHUNG
                        finalRet = Config.ERR_CODE_GENERAL;
                        return finalRet;
                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|exception:" + ex.ToString());

                    finalRet = Config.ERR_CODE_TIMEOUT;
                    return finalRet;
                }
            }//endif NLUONG
            #endregion "NLUONG TOPUP"
            else
            {
                return Config.ERR_CODE_GENERAL;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tranid"></param>
        /// <param name="custid"></param>
        /// <param name="src_acct"></param>
        /// <param name="amount"></param>
        /// <param name="order_id"></param>
        /// <param name="partner_id"></param>
        /// <param name="category_code"></param>
        /// <param name="service_code"></param>
        /// <param name="v_txdesc"></param>
        /// <param name="price"></param>
        /// <param name="info_ref"></param>
        /// <returns></returns>
        public String postBillToPartner(
            double tranid
            , string custid //for log only
            , string src_acct
            , double amount //bill_amount
            , string order_id //bill_id
            , string partner_id
            , string category_code
            , string service_code
            , string v_txdesc
            , string bill_info_ext1
            , string bill_info_ext2
            , ref string info_ref
            , string coreTxNo
        )
        {
            string ret = "";
            string retws = "";
            string finalRet = "";

            #region "NAPAS VAS"

            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|BILL BEGIN SWITCH PARTNER"
                 + "|ORDER_ID:" + order_id
                + "|CATEGORY_CODE:" + category_code
                + "|SERVICE_CODE:" + service_code
                 + "|AMOUNT:" + amount.ToString()
                );


            if (partner_id == Config.PARTNER_ID_VAS)
            {

                Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|begin call ws");

                try
                {
                    ret = string.Empty;
                    ret = new TopupBillingIntergrator().postMessagePaymentToNapas(custid, src_acct, amount, tranid, service_code, order_id);

                    Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|parse retws ret_code:" + ret);

                    //BILL VA TIMEOUT
                    if (ret == Config.RET_CODE_NAPAS_TOPUP[0].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = Config.RET_CODE_NAPAS_TOPUP[0]; //ket qua tra ve
                        return finalRet;
                    }
                    else if (ret == Config.RET_CODE_NAPAS_TOPUP[1].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE TIMEOUT");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = Config.RET_CODE_NAPAS_TOPUP[1]; //ket qua tra ve
                        return finalRet;
                    }
                    else // neu co ma loi cu the <> DONE, TIMEOUT, 99--> SE HACH TOAN REVERT
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE OTHER WS ERROR");

                        //Neu ma loi tra ve la ma loi cu the --> revert, DUNG CHUNG VOI TOPUP
                        for (int i = 3; i < Config.RET_CODE_NAPAS_TOPUP.Length; i++)
                        {
                            info_ref = ret;
                            if (ret == Config.RET_CODE_NAPAS_TOPUP[i].Split('|')[0])
                            {
                                info_ref = Config.RET_CODE_NAPAS_TOPUP[i];

                                finalRet = Config.ERR_CODE_REVERT;
                                return finalRet;
                            }
                        }

                        finalRet = Config.ERR_CODE_GENERAL;
                        info_ref = Config.RET_CODE_NAPAS_TOPUP[2];
                        return finalRet;
                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|exception:" + ex.ToString());
                    info_ref = Config.RET_CODE_NAPAS_TOPUP[2];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    return finalRet;
                }


            } //endif VAS

            #endregion "NAPAS VAS"

            #region "VNPAY BILL"
            else if (partner_id == Config.PARTNER_ID_VNPAY)
            {
                try
                {
                    ret = string.Empty;
                    retws = string.Empty;
                    finalRet = string.Empty;

                    string retStr = new TopupBillingIntergrator().postMessagePaymentToVNPay(custid, service_code, category_code, order_id, tranid, amount, Config.gVNPAY_TRAN_TYPE_000001);
                    ret = retStr;

                    if (ret == Config.RET_CODE_VNPAY[0].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = Config.RET_CODE_VNPAY[0]; //ket qua tra ve
                        return finalRet;
                    }
                    else if (ret == Config.RET_CODE_VNPAY[1].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE TIMEOUT");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = Config.RET_CODE_VNPAY[1]; //ket qua tra ve
                        return finalRet;
                    }
                    else
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE OTHER ERROR");

                        //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
                        for (int i = 3; i < Config.RET_CODE_VNPAY.Length; i++)
                        {
                            info_ref = ret;
                            if (ret == Config.RET_CODE_VNPAY[i].Split('|')[0])
                            {
                                info_ref = Config.RET_CODE_VNPAY[i];

                                finalRet = Config.ERR_CODE_REVERT;
                                return finalRet;

                            }
                        }

                        if (String.IsNullOrEmpty(finalRet))
                        {
                            info_ref = Config.RET_CODE_VNPAY[2];
                            return Config.ERR_CODE_GENERAL;
                        }
                        else
                        {
                            return finalRet;
                        }

                    }
                }
                //EXCEPTION KHONG REVERT
                //GIU LAI TIEN TRA MA LOI = TIMEOUT
                catch (Exception ex)
                {

                    finalRet = Config.ERR_CODE_TIMEOUT;
                    Funcs.WriteLog("CIF: " + custid + "EXCEPTION CALL PAYMENT VNPAY: " + ex.ToString());
                    return finalRet;
                }


            } //endif VNPAY
            #endregion "VNPAY BILL"

            #region "ONEPAY BILL"
            else if (partner_id == Config.PARTNER_ID_ONEPAY)
            {
                try
                {
                    retws = string.Empty;
                    finalRet = string.Empty;
                    ret = string.Empty;

                    ret = new TopupBillingIntergrator().postMessagePaymentToOnepay(custid, order_id, category_code, amount,
                        tranid.ToString(), "6014", "MOB", "SHB", "BILL PAY", service_code);

                    if (ret == Config.RET_CODE_ONEPAY[0].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = Config.RET_CODE_ONEPAY[0]; //ket qua tra ve
                        return finalRet;
                    }
                    else if (ret == Config.RET_CODE_ONEPAY[1].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE TIMEOUT");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = Config.RET_CODE_ONEPAY[1]; //ket qua tra ve
                        return finalRet;
                    }
                    else
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE OTHER ERROR");

                        //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
                        for (int i = 3; i < Config.RET_CODE_ONEPAY.Length; i++)
                        {
                            info_ref = ret;
                            if (ret == Config.RET_CODE_ONEPAY[i].Split('|')[0])
                            {
                                info_ref = Config.RET_CODE_ONEPAY[i];

                                finalRet = Config.ERR_CODE_REVERT;
                                return finalRet;

                            }
                        }

                        // khong co trong bang ma loi thi ko revert
                        info_ref = Config.RET_CODE_ONEPAY[2];
                        finalRet = Config.ERR_CODE_GENERAL;
                        return finalRet;
                    }
                }
                //EXCEPTION KHONG REVERT 
                //TRA MA LOI = TIMEOUT
                catch (Exception ex)
                {
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    Funcs.WriteLog("CIF: " + custid + " EXCEPTION CALL BILL PAYMENT TO ONEPAY: " + ex.ToString());
                    return finalRet;
                }

            } //endif ONEPAY
            #endregion "ONEPAY BILL"

            //linhtn addnew 2016 09 12
            #region "PAYOO BILL"
            else if (partner_id == Config.PARTNER_ID_PAYOO)
            {
                try
                {
                    retws = string.Empty;
                    finalRet = string.Empty;
                    ret = string.Empty;

                    string BillId_payoo = bill_info_ext2.Split('*')[0];
                    string Month_payoo = bill_info_ext2.Split('*')[1];

                    ret = new TopupBillingIntergrator().postMessagePaymentToPayoo(tranid.ToString(), BillId_payoo,
                        Decimal.Parse(amount.ToString()), order_id, bill_info_ext1, Month_payoo, src_acct, custid);

                    if (ret == Config.RET_CODE_PAYOO[0].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = Config.RET_CODE_PAYOO[0]; //ket qua tra ve
                        return finalRet;
                    }
                    else if (ret == Config.RET_CODE_PAYOO[1].Split('|')[0])
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE TIMEOUT");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = Config.RET_CODE_PAYOO[1]; //ket qua tra ve
                        return finalRet;
                    }
                    else
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE OTHER ERROR");

                        //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
                        for (int i = 3; i < Config.RET_CODE_PAYOO.Length; i++)
                        {
                            info_ref = ret;
                            if (ret == Config.RET_CODE_PAYOO[i].Split('|')[0])
                            {
                                info_ref = Config.RET_CODE_PAYOO[i];

                                finalRet = Config.ERR_CODE_REVERT;
                                return finalRet;
                            }
                        }

                        // khong co trong bang ma loi thi ko revert
                        info_ref = Config.RET_CODE_ONEPAY[1];
                        finalRet = Config.ERR_CODE_GENERAL;
                        return finalRet;
                    }
                }
                //EXCEPTION KHONG REVERT
                //TRA VE MA LOI = TIMEOUT
                catch (Exception ex)
                {
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    Funcs.WriteLog("CIF: " + custid + " EXCEPTION CALL BILL PAYMENT TO PAYOO:" + ex.ToString());
                    return finalRet;
                }

            } //endif ONEPAY
            #endregion "PAYOO BILL"

            //TUNGDT8 addnew 2018 10 15
            #region "EVNHN BILL"
            else if (partner_id == Config.PARTNER_ID_EVNHN)
            {
                try
                {
                    retws = string.Empty;
                    finalRet = string.Empty;
                    ret = string.Empty;

                    string[] Billling_list = bill_info_ext2.Split('^');



                    if (Billling_list.Length > 0)
                    {

                        List<EVNHN_.billList> billingList = EVNHN_.getBillingList(Billling_list);

                        foreach (EVNHN_.billList item in billingList)
                        {
                            ret = new EVNHN_().doPayment(tranid, order_id, service_code, item.debtAmount, Config.ChannelID, "", "", Config.ElectricityEvnHN.BANK_CD, item.billingCd, "", Config.ElectricityEvnHN.PROVIDER_CD);

                            if (ret == Config.ERR_CODE_DONE)
                            {
                                Funcs.WriteLog("Customer_CD: " + order_id + "|BILL_CD:" + item.billingCd + "|debtAmount:" + item.debtAmount + "|PROCESS CASE DONE");
                                finalRet = Config.ERR_CODE_DONE;
                            }
                            else
                            {
                                Funcs.WriteLog("Customer_CD: " + order_id + "|PROCESS CASE OTHER ERROR");
                                finalRet = ret;
                                return finalRet;
                            }
                        }
                    }

                    if (finalRet == Config.ERR_CODE_DONE)
                    {
                        Funcs.WriteLog("Customer_CD: " + order_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        return finalRet;
                    }
                    else
                    {
                        finalRet = Config.ERR_CODE_GENERAL;
                        return finalRet;
                    }
                }

                //EXCEPTION KHONG REVERT
                //TRA VE MA LOI = TIMEOUT
                catch (Exception ex)
                {
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    Funcs.WriteLog("Customer_CD: " + order_id + " EXCEPTION CALL BILL PAYMENT TO EVNHN:" + ex.ToString());
                    return finalRet;
                }
            } //endif EVNHN
            #endregion "EVNHN BILL"

            #region "DAIICHI BILL"
            else if (partner_id == Config.PARTNER_ID_DAIICHI)
            {
                try
                {
                    retws = string.Empty;
                    finalRet = string.Empty;
                    ret = string.Empty;

                    string[] Billling_list = bill_info_ext2.Split('^');

                    if (Billling_list.Length > 0)
                    {

                        List<PolicyDetailDLVN> billingList = DLVNSOA_.getBillingList(Billling_list);
                        //bill_info_ext1 = bill_info_ext1.Remove(bill_info_ext1.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                        if (bill_info_ext1.Substring(bill_info_ext1.Length - Config.ROW_REC_DLMT.Length).Equals(Config.ROW_REC_DLMT))
                        {
                            bill_info_ext1 = bill_info_ext1.Remove(bill_info_ext1.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                        }

                        string[] checkInsurance_list = bill_info_ext1.Split('^');
                        if (billingList.Count == checkInsurance_list.Length)
                        {
                            for (int i = 0; i < checkInsurance_list.Length; i++)
                            {
                                if (checkInsurance_list[i].Split('$')[0].Equals(billingList[i].PREM_TYPE))
                                {
                                    billingList[i].AMOUNT_TRANSFER = checkInsurance_list[i].Split('$')[2].ToString().Trim();
                                    billingList[i].CHECK_INSURANCE = checkInsurance_list[i].Split('$')[1].ToString().Trim();
                                }

                            }
                        }

                        foreach (PolicyDetailDLVN item in billingList)
                        {
                            DLVNSOA_.ins_payment_daichi("BH" + item.DLVN_REF,
                                item.ID_NUMBER,
                                "",
                                "BH",
                                "DLVN",
                                item.POLICY_NUMBER,
                                "",
                                DateTime.Now.ToString(),
                                DateTime.Now.ToString(),
                                DateTime.Now.ToString(),
                                DateTime.Now.ToString(),
                                item.FREQUENCE_PREMIUM,
                                "0",
                                "0",
                                item.AMOUNT,
                                "0",
                                "0",
                                item.AMOUNT,
                                item.AMOUNT_TRANSFER,
                                "0",
                                "VND",
                                Config.DESCRIPTION_DAIICHI,
                                Config.DESCRIPTION_DAIICHI,
                                "N",
                                item.DLVN_REF,
                                item.POLICY_OWNERNAME,
                                "",
                                "",
                                item.LI_NUMBER,
                                "1",
                                "",
                                DateTime.Now.ToString(),
                                "",
                                DateTime.Now.ToString(),
                                Config.DESCRIPTION_DAIICHI,
                                "",
                                item.LI_NAME,
                                "",
                                item.ADDRESS,
                                item.PHONE_NUMBER,
                                "",
                                "",
                                "",
                                Convert.ToString(tranid),
                                item.CHECK_INSURANCE);

                            if (item.CHECK_INSURANCE.Equals("0"))
                            {
                                ret = new DLVNSOA_().doPayment(item.POLICY_NUMBER,
                                item.DLVN_REF,
                                item.AMOUNT_TRANSFER,
                                item.AMOUNT,
                                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss t"),
                                item.FREQUENCE_PREMIUM,
                                item.PREM_TYPE,
                                item.POLICY_OWNERNAME,
                                item.ADDRESS,
                                item.PHONE_NUMBER,
                                Config.DESCRIPTION_DAIICHI,
                                coreTxNo,
                                "MOB");

                                if (ret == Config.ERR_CODE_DONE)
                                {
                                    Funcs.WriteLog("Customer_CD: " + order_id + "|BILL_CD:" + item.ID_NUMBER + "|debtAmount:" + item.AMOUNT + "|PROCESS CASE DONE");
                                    finalRet = Config.ERR_CODE_DONE;
                                }
                                else if (ret == Config.ERR_CODE_REVERT)
                                {
                                    Funcs.WriteLog("Customer_CD: " + order_id + "|BILL_CD:" + item.ID_NUMBER + "|debtAmount:" + item.AMOUNT + "|PROCESS CASE REVERT");
                                    finalRet = Config.ERR_CODE_REVERT;
                                    return finalRet;
                                }
                                else
                                {
                                    Funcs.WriteLog("Customer_CD: " + order_id + "|PROCESS CASE OTHER ERROR");
                                    finalRet = ret;
                                    return finalRet;
                                }

                            }
                        }
                    }

                    if (finalRet == Config.ERR_CODE_DONE)
                    {
                        Funcs.WriteLog("Customer_CD: " + order_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        return finalRet;
                    }
                    else
                    {
                        finalRet = Config.ERR_CODE_GENERAL;
                        return finalRet;
                    }
                }

                //EXCEPTION KHONG REVERT
                //TRA VE MA LOI = TIMEOUT
                catch (Exception ex)
                {
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    Funcs.WriteLog("Customer_CD: " + order_id + " EXCEPTION CALL BILL PAYMENT TO DAIICHI:" + ex.ToString());
                    return finalRet;
                }
            } //endif EVNHN
            #endregion "DAIICHI BILL"

            //#region "HABECO BILL"
            //else if (partner_id == Config.PARTNER_ID_HABECO)
            //{
            //    try
            //    {
            //        retws = string.Empty;
            //        finalRet = string.Empty;
            //        ret = string.Empty;

            //        //string BillId_payoo = bill_info_ext2.Split('*')[0];
            //        //string Month_payoo = bill_info_ext2.Split('*')[1];

            //        ret = new HabecoIntegration().AddCreditAdvice(custid, order_id, tranid.ToString(), v_txdesc, amount.ToString());

            //        if (ret == Config.RET_CODE_HABECO[0].Split('|')[0])
            //        {
            //            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");

            //            finalRet = Config.ERR_CODE_DONE;
            //            info_ref = Config.RET_CODE_HABECO[0]; //ket qua tra ve
            //            return finalRet;
            //        }
            //        else if (ret == Config.RET_CODE_HABECO[1].Split('|')[0])
            //        {
            //            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE TIMEOUT");

            //            finalRet = Config.ERR_CODE_TIMEOUT;
            //            info_ref = Config.RET_CODE_HABECO[1]; //ket qua tra ve
            //            return finalRet;
            //        }
            //        else
            //        {
            //            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE OTHER ERROR");

            //            //Neu ma loi tra ve la ma loi cu the <> DONE, TIMEOUT, 99 --> revert
            //            for (int i = 3; i < Config.RET_CODE_HABECO.Length; i++)
            //            {
            //                info_ref = ret;
            //                if (ret == Config.RET_CODE_HABECO[i].Split('|')[0])
            //                {
            //                    info_ref = Config.RET_CODE_HABECO[i];

            //                    finalRet = Config.ERR_CODE_REVERT;
            //                    return finalRet;
            //                }
            //            }

            //            // khong co trong bang ma loi thi ko revert
            //            info_ref = Config.RET_CODE_HABECO[1];
            //            finalRet = Config.ERR_CODE_GENERAL;
            //            return finalRet;
            //        }
            //    }
            //    //EXCEPTION KHONG REVERT
            //    //TRA VE MA LOI = TIMEOUT
            //    catch (Exception ex)
            //    {
            //        finalRet = Config.ERR_CODE_TIMEOUT;
            //        Funcs.WriteLog("CIF: " + custid + " EXCEPTION CALL BILL PAYMENT TO PAYOO:" + ex.ToString());
            //        return finalRet;
            //    }

            //} //endif ONEPAY
            //#endregion "PAYOO BILL"

            //CUONGDC Add new 2021 11 23
            else if (partner_id == Config.PARTNER_ID_HABECO)
            {
                try
                {
                    retws = string.Empty;
                    finalRet = string.Empty;

                    HABECO.HabecoSAPGetCustomerRespType getCustomer = HabecoIntegration.HabecoSAPGetCustomer(order_id);
                    if (getCustomer.ETReturns[0].code == "00")
                    {
                        HABECO.HabecoSAPBankThuRespType res = HabecoIntegration.HabecoSAPITInputBankThu(order_id, bill_info_ext1, amount.ToString());

                        if (res.ETReturns[0].code == "00")
                        {
                            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");
                            finalRet = Config.ERR_CODE_DONE;
                            info_ref = Config.RET_CODE_HABECO[0]; //ket qua tra ve
                            return finalRet;
                        }
                        else
                        {
                            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE ERROR HabecoSAPITInputBankThu");

                            finalRet = Config.ERR_CODE_TIMEOUT;
                            info_ref = Config.RET_CODE_HABECO[2]; //ket qua tra ve
                            return finalRet;
                        }
                    }
                    else
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE ERROR HabecoSAPITInputBankThu");

                        finalRet = Config.ERR_CODE_TIMEOUT;
                        info_ref = Config.RET_CODE_HABECO[2]; //ket qua tra ve
                        return finalRet;
                    }
                }
                catch (Exception ex)
                {
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    Funcs.WriteLog("CIF: " + custid + " EXCEPTION CALL BILL PAYMENT TO HABECO:" + ex.ToString());
                    return finalRet;
                }

            }

            #region "SHBFC BILL"

            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|BILL BEGIN SWITCH PARTNER"
                 + "|ORDER_ID:" + order_id
                + "|CATEGORY_CODE:" + category_code
                + "|SERVICE_CODE:" + service_code
                 + "|AMOUNT:" + amount.ToString()
                );


            if (partner_id == Config.PARTNER_ID_SHBFC)
            {

                Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|begin call SHBFC");

                try
                {
                    ret = string.Empty;
                    Utility ul = new Utility();
                    DataSet ds = ul.getCoreDate();

                    string vldate = string.Empty;
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        vldate = DateTime.ParseExact(ds.Tables[0].Rows[0]["CORE_DT"].ToString(), "yyyyMMdd", null).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    }

                    ret = new SHBFCIntegration().DebtPayment(custid,
                        order_id,
                        amount.ToString(),
                        "MBA",
                        coreTxNo, //refNo
                        DateTime.Now.ToString("yyyyMMddHHmmss"), //orgTranDate
                        vldate, //valdate
                        v_txdesc, //cur_marks
                        src_acct
                     );

                    Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|parse retws ret_code:" + ret);

                    //BILL VA TIMEOUT
                    if (ret == Config.ERR_CODE_DONE)
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE DONE");

                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = Config.RET_CODE_SHBFC_BILLING[0]; //ket qua tra ve
                        return finalRet;
                    }
                    else // neu co ma loi cu the <> DONE, TIMEOUT, 99--> SE HACH TOAN REVERT
                    {
                        Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|PROCESS CASE OTHER SHBFC ERROR");

                        for (int i = 18; i < Config.RET_CODE_SHBFC_BILLING.Length; i++)
                        {
                            if (ret.Equals(Config.RET_CODE_SHBFC_BILLING[i].Split('|')[0]))
                            {
                                info_ref = Config.RET_CODE_SHBFC_BILLING[i];
                                finalRet = Config.ERR_CODE_REVERT;
                                return Config.ERR_CODE_REVERT;
                            }
                        }

                        for (int j = 0; j < Config.RET_CODE_SHBFC_BILLING.Length; j++)
                        {
                            if (ret.Equals(Config.RET_CODE_SHBFC_BILLING[j].Split('|')[0]))
                            {
                                info_ref = Config.RET_CODE_SHBFC_BILLING[j];
                                finalRet = Config.ERR_CODE_GENERAL;
                                return Config.ERR_CODE_GENERAL;
                            }
                        }

                        finalRet = Config.ERR_CODE_GENERAL;
                        info_ref = Config.RET_CODE_SHBFC_BILLING[1];
                        return finalRet;
                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|BILLING:" + partner_id + "|exception:" + ex.ToString());
                    info_ref = Config.RET_CODE_SHBFC_BILLING[2];
                    finalRet = Config.ERR_CODE_GENERAL;
                    return finalRet;
                }


            } //endif SHBFC

            #endregion "SHBFC BILL"

            #region "QUAWACO BILL"

            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|BILL BEGIN SWITCH PARTNER"
                 + "|ORDER_ID:" + order_id
                + "|CATEGORY_CODE:" + category_code
                + "|SERVICE_CODE:" + service_code
                 + "|AMOUNT:" + amount.ToString()
                );


            if (partner_id == Config.PARTNER_ID_QUAWACO)
            {

                Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|begin call ws");

                try
                {
                    ret = string.Empty;
                    //ret = QUAWACOIntegration.DoPayment(custid, src_acct, amount, tranid, service_code, order_id);

                    Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|parse retws ret_code:" + ret);

                    int totalBill = 0;
                    int totalBillSuccess = 0;
                    try
                    {
                        QUAWACOBill.GetBillInfoByKHIDRespType listBill = QUAWACOIntegration.GetBillInfoByKHID(custid, order_id);

                        if (listBill != null)
                        {
                            if (listBill.errorCode.Equals("200") && listBill.listBills.Length > 0)
                            {
                                totalBill = listBill.listBills.Length;

                                double totalDebt = 0;

                                foreach (var i in listBill.listBills)
                                {
                                    totalDebt += Double.Parse(i.tongtien);
                                }

                                if (totalDebt != amount)
                                {
                                    Funcs.WriteLog("CIF: " + custid + "SO TIEN KHONG HOP LE:");

                                    finalRet = Config.ERR_CODE_REVERT;
                                    info_ref = Config.RET_CODE_QUAWACO[2]; //ket qua tra ve
                                    return finalRet;

                                }

                                foreach (var item in listBill.listBills)
                                {
                                    try
                                    {
                                        QUAWACOBill.DoPaymentRespType res = QUAWACOIntegration.DoPayment(custid, order_id, item.idkh + item.thang + item.nam, Funcs.getConfigVal("TRAN_TYPE_QUAWACO"), item.tongtien, item.thang, item.nam);

                                        Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO QUAWACO: ");

                                        if (res != null && res.errorCode.Equals("200"))
                                        {
                                            totalBillSuccess++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Funcs.WriteLog("CIF: " + custid + "EXCEPTION BILL TO QUAWACO:" + ex.ToString());
                                    }
                                }

                                if (totalBill == totalBillSuccess)
                                {
                                    Funcs.WriteLog("CIF: " + custid + "RESPONSE BILL TO QUAWACO: SO LUONG THANH TOAN THANH CONG BANG NHAU");

                                    finalRet = Config.ERR_CODE_DONE;
                                    info_ref = Config.RET_CODE_QUAWACO[0]; //ket qua tra ve
                                    return finalRet;
                                }
                                if (totalBillSuccess == 0)
                                {
                                    Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO QUAWACO: KHONG THANH TOAN DUOC KY NAO");

                                    finalRet = Config.ERR_CODE_REVERT;
                                    info_ref = Config.RET_CODE_QUAWACO[2]; //ket qua tra ve
                                    return finalRet;
                                }
                                else
                                {
                                    Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO QUAWACO: SO LUONG THANH TOAN THANH CONG KHONG BANG NHAU");

                                    finalRet = Config.ERR_CODE_GENERAL;
                                    info_ref = Config.RET_CODE_QUAWACO[1]; //ket qua tra ve
                                    return finalRet;
                                }
                            }
                            else
                            {
                                Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO QUAWACO: KHONG CO BILL NO");

                                finalRet = Config.ERR_CODE_REVERT;
                                info_ref = Config.RET_CODE_QUAWACO[2]; //ket qua tra ve
                                return finalRet;
                            }
                        }
                        else
                        {
                            Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO QUAWACO: KHONG CO BILL NO");
                            //message = Config.PartnerAccount.ReturnCode.RET_CODE_QUAWACO[2];
                            //return Config.PartnerAccount.ERR_CODE_REVERT;
                            finalRet = Config.ERR_CODE_REVERT;
                            info_ref = Config.RET_CODE_QUAWACO[2]; //ket qua tra ve
                            return finalRet;
                        }
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("CIF: " + custid + "|ERROR postBillingToPartnerQUAWACO: " + ex.Message);
                        finalRet = Config.ERR_CODE_GENERAL;
                        info_ref = Config.RET_CODE_QUAWACO[1]; //ket qua tra ve
                        return finalRet;
                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|TOPUP:" + partner_id + "|exception:" + ex.ToString());
                    info_ref = Config.RET_CODE_NAPAS_TOPUP[2];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    return finalRet;
                }


            } //endif VAS

            #endregion "QUAWACO BILL"

            #region "SLAWACO BILL"

            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|BILL BEGIN SWITCH PARTNER"
                 + "|ORDER_ID:" + order_id
                + "|CATEGORY_CODE:" + category_code
                + "|SERVICE_CODE:" + service_code
                 + "|AMOUNT:" + amount.ToString()
                );


            if (partner_id == Config.PARTNER_ID_SOWACO)
            {

                Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|begin call ws");

                try
                {
                    ret = string.Empty;

                    Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|parse retws ret_code:" + ret);

                    int totalBill = 0;
                    int totalBillSuccess = 0;
                    try
                    {
                        SOWACO.GetBillInfoRespType listBill = SOWACOIntegration.GetBillInfoByKHID(custid, order_id);

                        if (listBill != null)
                        {
                            if (listBill.errorCode.Equals("1") && listBill.listBillInfos.Length > 0)
                            {
                                totalBill = listBill.listBillInfos.Length;
                                List<string> listSuccess = new List<string>();
                                double totalDebt = 0;

                                foreach (var i in listBill.listBillInfos)
                                {
                                    totalDebt += Double.Parse(i.tongTien);
                                }

                                if (totalDebt != amount)
                                {
                                    Funcs.WriteLog("CIF: " + custid + "SO TIEN KHONG HOP LE:");

                                    finalRet = Config.ERR_CODE_REVERT;
                                    info_ref = Config.RET_CODE_SOWACO[6]; //ket qua tra ve
                                    return finalRet;

                                }

                                foreach (var item in listBill.listBillInfos)
                                {
                                    try
                                    {
                                        SOWACO.DoPaymentRespType res = SOWACOIntegration.DoPayment(custid, item.idHoaDon);

                                        Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO SLAWACO: ");

                                        if (res != null && res.errorCode.Equals("1"))
                                        {
                                            totalBillSuccess++;
                                            listSuccess.Add(item.idHoaDon);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Funcs.WriteLog("CIF: " + custid + "EXCEPTION BILL TO SLAWACO:" + ex.ToString());
                                        break;
                                    }
                                }

                                if (totalBill == totalBillSuccess)
                                {
                                    Funcs.WriteLog("CIF: " + custid + "RESPONSE BILL TO SLAWACO: SO LUONG THANH TOAN THANH CONG BANG NHAU");

                                    finalRet = Config.ERR_CODE_DONE;
                                    info_ref = Config.RET_CODE_SOWACO[0]; //ket qua tra ve
                                    return finalRet;
                                }

                                if (totalBillSuccess == 0)
                                {
                                    Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO SLAWACO: KHONG THANH TOAN DUOC KY NAO");

                                    finalRet = Config.ERR_CODE_REVERT;
                                    info_ref = Config.RET_CODE_SOWACO[6]; //ket qua tra ve
                                    return finalRet;
                                }
                                else
                                {
                                    Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO SLAWACO: SO LUONG THANH TOAN THANH CONG KHONG BANG NHAU");

                                    int refundSuccess = 0;

                                    foreach (var item in listSuccess)
                                    {
                                        try
                                        {
                                            SOWACO.CancelPaymentRespType rets = SOWACOIntegration.UNDoPayment(custid, item);

                                            Funcs.WriteLog("CIF: " + custid + "|RESPONSE UnPayment BILL TO SLAWACO: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(rets)));

                                            if (rets != null && rets.errorCode.Equals("1"))
                                            {
                                                refundSuccess++;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Funcs.WriteLog("EXCEPTION BILL TO SLAWACO:" + custid + " :" + ex.ToString());
                                        }
                                    }

                                    if (refundSuccess == 0)
                                    {
                                        Funcs.WriteLog("RESPONSE REVERT BILL TO SLAWACO " + custid + " KHONG HUY BAO CO DUOC HOA DON NAO");

                                        finalRet = Config.ERR_CODE_GENERAL;
                                        info_ref = Config.RET_CODE_SOWACO[1]; //ket qua tra ve
                                        return finalRet;
                                    }
                                    else if (refundSuccess == listSuccess.Count)
                                    {
                                        Funcs.WriteLog("RESPONSE REVERT BILL TO SLAWACO " + custid + " HUY BAO CO THANH CONG TOAN BO");

                                        finalRet = Config.ERR_CODE_REVERT;
                                        info_ref = Config.RET_CODE_SOWACO[6]; //ket qua tra ve
                                        return finalRet;
                                    }
                                    else
                                    {
                                        Funcs.WriteLog("RESPONSE REVERT BILL TO SLAWACO " + custid + " HUY BAO CO DUOC MOT PHAN");
                                        finalRet = Config.ERR_CODE_GENERAL;
                                        info_ref = Config.RET_CODE_SOWACO[1]; //ket qua tra ve
                                        return finalRet;
                                    }
                                }
                            }
                            else
                            {
                                Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO SLAWACO: KHONG CO BILL NO");

                                finalRet = Config.ERR_CODE_REVERT;
                                info_ref = Config.RET_CODE_SOWACO[6]; //ket qua tra ve
                                return finalRet;
                            }
                        }
                        else
                        {
                            Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO SLAWACO: KHONG CO BILL NO");

                            finalRet = Config.ERR_CODE_REVERT;
                            info_ref = Config.RET_CODE_SOWACO[6]; //ket qua tra ve
                            return finalRet;
                        }
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("CIF: " + custid + "|ERROR postBillingToPartnerSLAWACO: " + ex.Message);
                        finalRet = Config.ERR_CODE_GENERAL;
                        info_ref = Config.RET_CODE_SOWACO[1]; //ket qua tra ve
                        return finalRet;
                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|BILLING:" + partner_id + "|exception:" + ex.ToString());
                    info_ref = Config.RET_CODE_SOWACO[1];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    return finalRet;
                }


            } //endif VAS

            #endregion "SLAWACO BILL"

            #region "DAWACO BILL"

            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|BILL BEGIN SWITCH PARTNER"
                 + "|ORDER_ID:" + order_id
                + "|CATEGORY_CODE:" + category_code
                + "|SERVICE_CODE:" + service_code
                 + "|AMOUNT:" + amount.ToString()
                );


            if (partner_id == Config.PARTNER_ID_DAWACO)
            {

                Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|begin call ws");

                try
                {
                    ret = string.Empty;

                    Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|parse retws ret_code:" + ret);

                    try
                    {
                        DAWACOBillPayments.DebtCheckRespType listBill = DAWACOIntegration.GetBillInfoByKHID(custid, order_id);

                        if (listBill != null)
                        {
                            if (listBill.errorCode.Equals("00") && listBill.customerData.Length > 0)
                            {

                                DAWACOBillPayments.DebtPaymentRespType retPay = DAWACOIntegration.DoPayment(custid, order_id, amount.ToString(), tranid.ToString());

                                if (retPay != null && retPay.errorCode.Equals("00"))
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "RESPONSE BILL TO DAWACOBillPayments " + custid + " SUCCESS");

                                    finalRet = Config.ERR_CODE_DONE;
                                    info_ref = Config.RET_CODE_DAWACO[0]; //ket qua tra ve
                                    return finalRet;

                                }
                                else
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "RESPONSE BILL TO DAWACOBillPayments " + custid + " FAIL");

                                    finalRet = Config.ERR_CODE_REVERT;
                                    info_ref = Config.RET_CODE_DAWACO[1]; //ket qua tra ve
                                    return finalRet;
                                }
                            }
                            else
                            {
                                Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO DAWACOBillPayments: KHONG CO BILL NO");

                                finalRet = Config.ERR_CODE_REVERT;
                                info_ref = Config.RET_CODE_DAWACO[1]; //ket qua tra ve
                                return finalRet;
                            }
                        }
                        else
                        {
                            Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO DAWACOBillPayments: KHONG CO BILL NO");

                            finalRet = Config.ERR_CODE_REVERT;
                            info_ref = Config.RET_CODE_DAWACO[1]; //ket qua tra ve
                            return finalRet;
                        }
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("CIF: " + custid + "|ERROR postBillingToPartnerDAWACO: " + ex.Message);
                        finalRet = Config.ERR_CODE_GENERAL;
                        info_ref = Config.RET_CODE_DAWACO[1]; //ket qua tra ve
                        return finalRet;
                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|BILLING:" + partner_id + "|exception:" + ex.ToString());
                    info_ref = Config.RET_CODE_DAWACO[1];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    return finalRet;
                }


            } //endif VAS

            #endregion "DAWACO BILL"

            #region "EVNNPC BILL"

            Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|BILL BEGIN SWITCH PARTNER"
                 + "|ORDER_ID:" + order_id
                + "|CATEGORY_CODE:" + category_code
                + "|SERVICE_CODE:" + service_code
                 + "|AMOUNT:" + amount.ToString()
                );


            if (partner_id == Config.PARTNER_ID_EVNNPC)
            {

                Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|begin call ws");

                try
                {
                    ret = string.Empty;

                    Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|parse retws ret_code:" + ret);

                    try
                    {
                        EvnNPCBillPayment.QueryBillInfoRespType listBill = EVNNPCIntegration.QueryBillInfo(custid, order_id, Funcs.GenESBMsgId(), "211801");

                        double totalDebt = 0;
                        double totalItem = 0;

                        if (listBill != null)
                        {
                            if (listBill.StatusCd != null && listBill.StatusCd.Equals("0") && listBill.ListBillInfo.Length > 0)
                            {
                                totalItem = listBill.ListBillInfo.Length;

                                foreach (var item in listBill.ListBillInfo)
                                {
                                    totalDebt += Double.Parse(String.IsNullOrEmpty(item.TotalAmount) ? "0" : item.TotalAmount);
                                }

                                if (totalDebt != amount)
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC " + custid + " SO TIEN KHONG CHINH XAC");

                                    finalRet = Config.ERR_CODE_REVERT;
                                    info_ref = Config.RET_CODE_EVNPC[2]; //ket qua tra ve
                                    return finalRet;
                                }

                                int count = 0;

                                foreach (var item in listBill.ListBillInfo)
                                {
                                    EvnNPCBillPayment.TransConfirmV2RespType res = EVNNPCIntegration.TransConfirmV2(custid, order_id, Funcs.GenESBMsgId(), "211801", item.IdHD, order_id.Substring(0, 6), "1", item.TotalAmount, order_id.Substring(0, 6), "0");

                                    if (res != null && res.StatusCd != null && res.StatusCd.Equals("0"))
                                    {
                                        Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC " + custid + "|IdHD :  " + item.IdHD + " SUCCESS");
                                        count++;
                                    }
                                    else
                                    {
                                        Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC " + custid + "|IdHD :  " + item.IdHD + " FAIL");
                                        finalRet = Config.ERR_CODE_GENERAL;
                                        info_ref = Config.RET_CODE_EVNPC[1];
                                        return finalRet;
                                    }
                                }

                                if (count == totalItem)
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC " + custid + "| SUCCESS");
                                    finalRet = Config.ERR_CODE_DONE;
                                    info_ref = Config.RET_CODE_EVNPC[0];
                                    return finalRet;
                                }
                                else
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC " + custid + "| FAIL");
                                    finalRet = Config.ERR_CODE_GENERAL;
                                    info_ref = Config.RET_CODE_EVNPC[1];
                                    return finalRet;
                                }
                            }
                            else
                            {
                                Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC: KHONG CO BILL NO");

                                finalRet = Config.ERR_CODE_REVERT;
                                info_ref = Config.RET_CODE_EVNPC[1]; //ket qua tra ve
                                return finalRet;
                            }
                        }
                        else
                        {
                            Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC: KHONG CO BILL NO");

                            finalRet = Config.ERR_CODE_REVERT;
                            info_ref = Config.RET_CODE_EVNPC[1]; //ket qua tra ve
                            return finalRet;
                        }
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("CIF: " + custid + "|ERROR postBillingToPartnerEVNNPC: " + ex.Message);
                        finalRet = Config.ERR_CODE_GENERAL;
                        info_ref = Config.RET_CODE_EVNPC[1]; //ket qua tra ve
                        return finalRet;
                    }
                }
                //NEU EXCEPTION KHONG HACH TOAN REVERT
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|BILLING:" + partner_id + "|exception:" + ex.ToString());
                    info_ref = Config.RET_CODE_EVNPC[1];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    return finalRet;
                }


            } //endif VAS

            #endregion "EVNNPC BILL"

            #region "EVNMN BILL"

            //Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|BILL BEGIN SWITCH PARTNER"
            //     + "|ORDER_ID:" + order_id
            //    + "|CATEGORY_CODE:" + category_code
            //    + "|SERVICE_CODE:" + service_code
            //     + "|AMOUNT:" + amount.ToString()
            //    );


            if (partner_id == Config.PARTNER_ID_EVNMN)
            {

                Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|begin call ws");

                try
                {
                    ret = string.Empty;

                    Funcs.WriteLog("CIF: " + custid + "|BILL:" + partner_id + "|parse retws ret_code:" + ret);

                    EVNMN.GetCustomerInfoResType listBill = EVNMNIntegration.GetCustomerInfo(Funcs.getConfigVal("BANK_ID"), order_id);

                    double totalDebt = 0;
                    double totalItem = 0;

                    if (listBill != null && listBill.ListofBillInfo != null && listBill.ListofBillInfo.Length > 0)
                    {
                        totalItem = listBill.ListofBillInfo.Length;

                        foreach (var item in listBill.ListofBillInfo)
                        {
                            totalDebt += Double.Parse(String.IsNullOrEmpty(item.Amount) ? "0" : item.Amount);
                        }

                        if (totalDebt < amount)
                        {
                            Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC " + custid + " SO TIEN KHONG CHINH XAC");

                            finalRet = Config.ERR_CODE_REVERT;
                            info_ref = Config.RET_CODE_EVNMN[2]; //ket qua tra ve
                            return finalRet;
                        }

                        int count = 0;
                        string[] billArr = Array.FindAll(bill_info_ext1.Split('$'), s => s != "");
                        long[] amountArr = Array.ConvertAll(Array.FindAll(bill_info_ext2.Split('$'), s => s != ""), s => long.Parse(s));

                        var successBills = new List<string>();
                        var succesAmounts = new List<long>();
                        var successTransactionCodes = new List<string>();

                        long amountArrSum = 0;
                        Array.ForEach(amountArr, delegate (long i) { amountArrSum += i; });

                        if (amountArrSum != amount)
                        {
                            Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + custid + " SO TIEN KHONG CHINH XAC");

                            finalRet = Config.ERR_CODE_REVERT;
                            info_ref = Config.RET_CODE_EVNMN[2]; //ket qua tra ve
                            return finalRet;
                        }


                        for (int i = 0; i < billArr.Length; i++)
                        {
                            var bill = listBill.ListofBillInfo[i];
                            //var bill = Array.Find(listBill.ListofBillInfo, s => billArr[i].Equals(s.BillCode));

                            if (!(billArr[i].Equals(bill.BillCode)))
                            {
                                Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + custid + " THU TU BILL KO CHINH XAC");

                                finalRet = Config.ERR_CODE_REVERT;
                                info_ref = Config.RET_CODE_EVNMN[3]; //ket qua tra ve
                                return finalRet;
                            }

                            string transactionCode = Utils.RandomString("MO", 10) + bill.HoaDonID;
                            string[] transactionArr = { transactionCode };
                            long[] amountPayArr = { amountArr[i] };
                            string[] billPayArr = { billArr[i] };

                            EVNMN.PayBillsByCustomerCodeResType res = EVNMNIntegration.PayBillsByCustomerCode(custid, order_id, Funcs.getConfigVal("BANK_ID"), "", billPayArr, amountPayArr, transactionArr, "", "");

                            if (res != null && "0".Equals(res.Status))
                            {
                                Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + custid + "|IdHD :  " + order_id + " SUCCESS");
                                successBills.Add(billArr[i]);
                                succesAmounts.Add(amountArr[i]);
                                successTransactionCodes.Add(transactionCode);
                                count++;
                            }
                            else
                            {
                                if (res != null && ("1".Equals(res.Status) || "3".Equals(res.Status) || "4".Equals(res.Status)))  // revert
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + custid + "|IdHD :  " + order_id + " FAIL-REVERT");

                                    try
                                    {
                                        EVNMN.CancelBillsByCustomerCodeResType cancelResponse = EVNMNIntegration.CancelBills(custid, order_id, Funcs.getConfigVal("BANK_ID"), billPayArr, amountPayArr, transactionArr);
                                    }
                                    catch (Exception ex)
                                    {
                                        Funcs.WriteLog("CIF: " + custid + "|ERROR CANCEL_BillingToPartnerEVNMN: " + ex.Message);
                                        return finalRet;
                                    }

                                    finalRet = Config.ERR_CODE_REVERT;
                                    info_ref = Config.RET_CODE_EVNMN[1]; //ket qua tra ve

                                }
                                else
                                {
                                    Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN " + custid + "|IdHD :  " + order_id + " FAIL");
                                    finalRet = Config.ERR_CODE_GENERAL;
                                    info_ref = Config.RET_CODE_EVNMN[1];

                                }
                                return finalRet;
                            }
                        }
                        Funcs.WriteLog("CIF_NO: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNNPC " + custid + "| SUCCESS");
                        finalRet = Config.ERR_CODE_DONE;
                        info_ref = Config.RET_CODE_EVNPC[0];
                        return finalRet;

                    }
                    else
                    {
                        Funcs.WriteLog("CIF: " + custid + "|RESPONSE BILL TO postBillingToPartnerEVNMN: KHONG CO BILL NO");
                        finalRet = Config.ERR_CODE_REVERT;
                        info_ref = Config.RET_CODE_EVNMN[1]; //ket qua tra ve
                        return finalRet;
                    }
                }
                catch (Exception ex)
                {
                    Funcs.WriteLog("CIF: " + custid + "|BILLING:" + partner_id + "|exception:" + ex.ToString());
                    finalRet = Config.ERR_CODE_GENERAL;
                    info_ref = Config.RET_CODE_EVNMN[1]; //ket qua tra ve
                    return finalRet;
                }
            } //endif EVNMN

            #endregion "EVNMN BILL"

            else
            {
                return Config.ERR_CODE_GENERAL;
            }
        }

        public String postTopupToCore(
            double tran_id
            , string custid //for log only
            , string src_acct
            , string suspend_acct
            , string fee_acct
            , string vat_acct
            , double suspend_amt
            , double fee_amt
            , double vat_amt
            , string txdesc
            , string pos_cd
            , ref string core_txno_ref
        )
        {
            mobileGW.SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
            myWS.Timeout = Config.TIMEOUT_WITH_CORE;
            core_txno_ref = "";
            core_txno_ref = Config.refFormat + tran_id.ToString().PadLeft(9, '0');

            try
            {
                string postinfo = pos_cd + "|"
                    + src_acct + "|D|"
                    + (suspend_amt).ToString() + "|VND~"
                    + pos_cd + "|"
                    + suspend_acct + "|C|"
                    + suspend_amt.ToString() + "|VND";

                string retStr = myWS.FINANCIAL_POSTING(
                    Config.ChannelID,
                    Config.InterfaceID,
                    pos_cd,
                    postinfo,
                    txdesc,
                    "",
                    core_txno_ref
                    );
                DataSet ds = Funcs.Node2Ds(retStr, "HEADER");

                if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
                    == Config.gResult_INTELLECT_Arr[0].Split('|')[0])
                {
                    //postTopupToPartner(tran_id, custid, src_acct, suspend_amt, order_id, partner_id, category_code, service_code, txdesc, price, info_ref);

                    //               uptTransferTx(TranId, Config.TX_STATUS_DONE, core_txno, TxnDate, Config.ChannelID);
                    Transfers tf = new Transfers();

                    tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno_ref, String.Format("{0:dd/MM/yyyy}", DateTime.Now), Config.ChannelID);
                    tf = null;
                    if (ds != null) ds.Dispose();
                    return Config.gResult_INTELLECT_Arr[0];
                }

                else //neu tra ve loi cu the revert
                {
                    // revert transaction
                    //...
                    //myWS.FINANCIALPOSTING_REVERT(
                    //    Config.ChannelID,
                    //    Config.InterfaceID,
                    //    Config.refFormat + tran_id.ToString().PadLeft(9, '0'),
                    //    "0"
                    //    );

                    Transfers tf = new Transfers();
                    tf.uptTransferTx(tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                    tf = null;
                    if (ds != null) ds.Dispose();
                    return Config.gResult_INTELLECT_Arr[1];
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("POST TOPUP TO CORE EX" + ex.ToString());
                // revert transaction
                // khong can revert
                //myWS.FINANCIALPOSTING_REVERT(
                //    Config.ChannelID,
                //    Config.InterfaceID,
                //    Config.refFormat + v_tran_id.ToString().PadLeft(9, '0'),
                //    "0"
                //    );
                //UPDATE_CARD_2_CARD(v_tran_id, Config.TX_STATUS_FAIL, "", "");
                Transfers tf = new Transfers();
                tf.uptTransferTx(tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                tf = null;
                return Config.gResult_INTELLECT_Arr[1];
            }
            finally
            {
                myWS.Dispose();
            }
        }
        public DataTable GET_PAY_CREDIT_BY_SERVICEID(string category_id, string service_type, string service_id)
        {

            try
            {
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.CHECK_PAY_CREDIT_BY_SERVICEID ", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("PSERVICE_ID", OracleDbType.Varchar2, service_id, ParameterDirection.Input);
                dsCmd.Parameters.Add("PCATEGORY_ID", OracleDbType.Varchar2, category_id, ParameterDirection.Input);
                dsCmd.Parameters.Add("PSERVICE_TYPE", OracleDbType.Varchar2, service_type, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
