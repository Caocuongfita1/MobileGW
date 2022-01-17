using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using mobileGW.Service.Framework;
using System.Web.Script.Serialization;

namespace mobileGW.Service.DataAccess
{
    /// <summary>
    /// Summary description for OtpDA
    /// </summary>
    public class GiftDAO : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }
        protected void Dispose(bool disposing)
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

        private OracleCommand dsCmd;
        private OracleDataAdapter dsApt;


        public GiftDAO()
        {
            //
            dsApt = new OracleDataAdapter();
            //
        }

        public DataTable INSERT_TBL_EB_GIFT(string custId, GiftModel model)
        {

            Funcs.WriteLog("CIF_NO: " + custId + "|BEGIN INSERT_TBL_EB_GIFT: " + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(model)));

            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_GIVE_GIFT.INSERT_TBL_EB_GIFT", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;

                dsCmd.Parameters.Add("P_CIF_SEND_GIFT", OracleDbType.Varchar2, model.CIF_SEND_GIFT, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_SRC_ACCT", OracleDbType.Varchar2, model.SRC_ACCT, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_DES_ACCT", OracleDbType.Varchar2, model.DES_ACCT, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_GIFT_TYPE", OracleDbType.Varchar2, model.GIFT_TYPE, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_GIFT_CARD_URL", OracleDbType.Varchar2, model.GIFT_CARD_URL, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_GIFT_CARD_MESSAGE", OracleDbType.Varchar2, model.GIFT_CARD_MESSAGE, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_AMOUNT", OracleDbType.Double, model.AMOUNT, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CCYCD", OracleDbType.Varchar2, model.CCYCD, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_REF_NO", OracleDbType.Varchar2, model.REF_NO, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_REMARK", OracleDbType.Varchar2, model.REMARK, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_REMARK_EARMARK", OracleDbType.Varchar2, model.REMARK_EARMARK, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_EFF_DT", OracleDbType.Varchar2, model.EFF_DT, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_EXP_DT", OracleDbType.Varchar2, model.EXP_DT, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_DES_NAME", OracleDbType.Varchar2, model.DES_NAME, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_SRC_NAME", OracleDbType.Varchar2, model.SRC_NAME, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHANNEL_ID", OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR1", OracleDbType.Varchar2, model.CHAR1, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR2", OracleDbType.Varchar2, model.CHAR2, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR3", OracleDbType.Varchar2, model.CHAR3, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR4", OracleDbType.Varchar2, model.CHAR4, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR5", OracleDbType.Varchar2, model.CHAR5, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR6", OracleDbType.Varchar2, model.CHAR6, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR7", OracleDbType.Varchar2, model.CHAR7, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR8", OracleDbType.Varchar2, model.CHAR8, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CHAR9", OracleDbType.Varchar2, model.CHAR9, ParameterDirection.Input);

                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);

                dsApt.SelectCommand = dsCmd;

                dsApt.Fill(ds);

                //Funcs.WriteLog("CIF_NO: " + custId + "|DONE INSERT_TBL_EB_GIFT" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(ds.Tables[0])));

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|EXCEPTION INSERT_TBL_EB_GIFT" + ex.Message.ToString());
                return null;
            }
        }

        public DataTable GET_INFO_TBL_EB_GIFT(string custId, string refNo)
        {

            Funcs.WriteLog("CIF_NO: " + custId + "|BEGIN GET_INFO_TBL_EB_GIFT: " + refNo);

            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_GIVE_GIFT.SP_GET_INFO_TBL_EB_GIFT", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;

                dsCmd.Parameters.Add("P_CIF_NO", OracleDbType.Varchar2, custId, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_REF_NO", OracleDbType.Varchar2, refNo, ParameterDirection.Input);

                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);

                dsApt.SelectCommand = dsCmd;

                dsApt.Fill(ds);

                //Funcs.WriteLog("CIF_NO: " + custId + "|DONE GET_INFO_TBL_EB_GIFT" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(ds.Tables[0])));

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|EXCEPTION GET_INFO_TBL_EB_GIFT" + ex.Message.ToString());
                return null;
            }
        }

        public DataSet UPDATE_TBL_EB_GIFT(string custId, string refNo, string status, string type, string holdStatus, string coreStatus, string errDesc)
        {
            Funcs.WriteLog("CIF_NO: " + custId + "|BEGIN UPDATE_TBL_EB_GIFT: REF_NO: " + refNo + "|STATUS: " + status);

            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_GIVE_GIFT.UPDATE_TBL_EB_GIFT", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("P_CHANNEL", OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_REF_NO", OracleDbType.Varchar2, refNo, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_STATUS", OracleDbType.Varchar2, status, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_TYPE_UPDATE", OracleDbType.Varchar2, type, ParameterDirection.Input); 
                dsCmd.Parameters.Add("P_HOLD_STATUS", OracleDbType.Varchar2, holdStatus, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_CORE_STATUS", OracleDbType.Varchar2, coreStatus, ParameterDirection.Input);
                dsCmd.Parameters.Add("P_ERRORDESC", OracleDbType.Varchar2, errDesc, ParameterDirection.Input);

                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CIF_NO: " +  custId +"|EXCEPTION: " + ex.ToString());
                return null;
                
            }
        }
    }
}