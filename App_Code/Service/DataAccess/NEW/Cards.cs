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

using mobileGW.Service.DataAccess;

using System.Xml;

/// <summary>
/// Summary description for Utility
/// </summary>


namespace mobileGW.Service.AppFuncs
{
    public class Cards
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
        public Cards()
        {
            //
            dsApt = new OracleDataAdapter();
            //
        }


        //public DataTable GET_CARD_LIST(string custid)
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANK_PCI_NEW.GET_CARD_CIF", new OracleConnection(Config.gCARDCnnstr));
        //        dsCmd.Parameters.Add("CUSTID", OracleDbType.Varchar2, custid, ParameterDirection.Input);
        //        dsCmd.Parameters.Add("OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
        //        dsCmd.CommandType = CommandType.StoredProcedure;
        //        dsApt.SelectCommand = dsCmd;
        //        dsApt.Fill(ds);
        //        return ds.Tables[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());
        //        return null;
        //    }            
        //}


        public DataSet getListCard(string custid)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANKAPPS_PCI_NEW.GET_CARD_LIST", new OracleConnection(Config.gCARDCnnstr));
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                //dsCmd.Parameters.Add("V_DES_ACCT", OracleDbType.Varchar2, DES_ACCT, ParameterDirection.Input);
                dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                // Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="credit_card_no">Số thẻ full masking</param>
        /// <returns></returns>
        public DataSet getCardInfo(string custid, string card_no, string card_type)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANKAPPS_PCI_NEW.getCardInfoByType", new OracleConnection(Config.gCARDCnnstr));
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("card_no", OracleDbType.Varchar2, card_no, ParameterDirection.Input);
                dsCmd.Parameters.Add("card_type", OracleDbType.Varchar2, card_type, ParameterDirection.Input);
                //dsCmd.Parameters.Add("V_DES_ACCT", OracleDbType.Varchar2, DES_ACCT, ParameterDirection.Input);
                //dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                // Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

       



        /// <summary>
        /// Khoa mo the
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="cardNo"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool HANDLE_CARD(String custid, String cardNo, String action)
        {
            try
            {

                SHBCardGW.Service myGW = new SHBCardGW.Service();

                myGW.Url = Config.URL_WS_CARD_SHB;
                myGW.Timeout = Config.TIMEOUT_WITH_CARD_SHB;

                string localtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                
                string retXML = myGW.HandleCardStatusByCardNo(
                    "100000",
                    Config.ChannelID,
                    localtime,
                    cardNo,
                    action,
                    "_NULL_",
                    "_NULL_",
                    "_NULL_",
                    "_NULL_",
                    "_NULL_",
                    Funcs.encryptMD5(
                    "100000"
                    + Config.ChannelID
                    + localtime
                    + cardNo
                    + action
                    + "_NULL_"
                    + "_NULL_"
                    + "_NULL_"
                    + "_NULL_"
                    + "_NULL_"
                    + Config.key_credit_card_gw
                    ).ToUpper()
                    );
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(retXML);
                string retStr = XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText;
                if (retStr == "00")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return false;
            }
            //
        }


        /*
          PROCEDURE GET_CARD_TRAN_BY_TYPE (
                                    v_cardno     IN     VARCHAR2                                     
                                    , v_enquiry_type in VARCHAR2
                                    , v_fromdate   IN     VARCHAR2   
                                    , v_todate     IN     VARCHAR2   
                                    , out_cur         OUT t_cursor)   
         */
        public DataSet getCardTran(string custid, string card_no, string enquiry_type, string vfromdate, string vtodate)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANKAPPS_PCI_NEW.GET_CARD_TRAN_BY_TYPE", new OracleConnection(Config.gCARDCnnstr));
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("card_no", OracleDbType.Varchar2, card_no, ParameterDirection.Input);
               // dsCmd.Parameters.Add("card_type", OracleDbType.Varchar2, card_type, ParameterDirection.Input);
                dsCmd.Parameters.Add("enquiry_type", OracleDbType.Varchar2, enquiry_type, ParameterDirection.Input);
                dsCmd.Parameters.Add("vfromdate", OracleDbType.Varchar2, vfromdate, ParameterDirection.Input);
                dsCmd.Parameters.Add("vtodate", OracleDbType.Varchar2, vtodate, ParameterDirection.Input);
                //dsCmd.Parameters.Add("V_DES_ACCT", OracleDbType.Varchar2, DES_ACCT, ParameterDirection.Input);
                dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                // Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }


        public string MakeTranByVoice
           (string mti,
           string channel_id,
           string local_time,
           string card_no,
           string ccycd,
           string tran_type,
           string amount
        )
        {
            try
            {
                SHBCardGW.Service myGW = new SHBCardGW.Service();

                myGW.Url = Config.URL_WS_CARD_SHB;
                myGW.Timeout = Config.TIMEOUT_WITH_CARD_SHB;

                string mac = Funcs.encryptMD5(mti + channel_id + local_time + card_no + ccycd + tran_type + amount + "" + "" + "0" + Config.key_credit_card_gw).ToUpper();
                //string retXML = myGW.MakeTranByVoice(mti, channel_id, local_time, card_no, ccycd, tran_type, amount, "", "", "0", mac);
                //anhnd2 29.01.2015 PCI DSS
                //MakeTranByVoiceByAccount
                string retXML = myGW.MakeTranByVoiceByAccount(mti, channel_id, local_time, card_no, ccycd, tran_type, amount, "", "", "0", mac);
                return retXML;
                //XmlDocument XmlDoc = new XmlDocument();
                //XmlDoc.LoadXml(retXML);
                //string errDes = XmlDoc.GetElementsByTagName("ERROR_DESC")[0].InnerText;
                //return XmlDoc.GetElementsByTagName("ERROR_CODE")[0].InnerText;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return string.Empty;
            }
        }

        public string setCreditCardLimit(string custid, string card_no, double withdrawl_daily_limit
            , double payment_daily_limit
            , double payment_online_limit
            )
        {
            try
            {
                string retStr = "";

                //goi ham setCreditCardLimit

                return retStr;
            }
            catch (Exception ex)
            {
                // Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        public DataTable getCreditCardLimit(string card_no)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANKAPPS_PCI_NEW.get_limit_type_by_card_no", new OracleConnection(Config.gCARDCnnstr));
                dsCmd.Parameters.Add("card_no", OracleDbType.Varchar2, card_no, ParameterDirection.Input);
                dsCmd.Parameters.Add("limit_type", OracleDbType.Varchar2, "%", ParameterDirection.Input);
                dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                // Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        public DataTable getCardHolderName(string acct_card_no)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANKAPPS_PCI_NEW.GET_CARD_NAME_BY_ACCTNO", new OracleConnection(Config.gCARDCnnstr));
                dsCmd.Parameters.Add("acct_card_no", OracleDbType.Varchar2, acct_card_no, ParameterDirection.Input);
                dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                // Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                Funcs.WriteLog(ex.ToString());
                return null;
                //return null;
            }
        }

        public DataTable get_cardno_by_cardno(string cardno)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANKAPPS_PCI_NEW.get_cardno_by_cardno", new OracleConnection(Config.gCARDCnnstr));
                dsCmd.Parameters.Add("cardno", OracleDbType.Varchar2, cardno, ParameterDirection.Input);
                dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                // Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

    }

}
