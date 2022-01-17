using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;

namespace mobileGW.Service.DataAccess
{

    /// <summary>
    /// Summary description for TBL_EB_USER_CHANNELs
    /// </summary

    public class TBL_EB_USER_CHANNELs
    {


        public const String MOB_USER = "MOB_USER";
        public const String MOB_PWD = "MOB_PWD";
        public const String RECEIVED_TIME = "RECEIVED_TIME";


        public const String TOKEN = "TOKEN";
        public const String MOB_MOBILE_NO = "MOB_MOBILE_NO";
        public const String CHANNEL_ID = "CHANNEL_ID";

        public const String CURSOR_PARM = "REF_CUR";
        public const String FROMDATE_PARM = "V_FROMDATE";
        public const String TODATE_PARM = "V_TODATE";
        public const String AUTHEN_MODE = "V_AUTHEN_MODE";
        public const String ACCTNO = "V_ACCTNO";
        public const String ACTIVE_CODE = "ACTIVE_CODE";


        private OracleCommand dsCmd;
        //end edit
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
        public TBL_EB_USER_CHANNELs()
        {
            //
            dsApt = new OracleDataAdapter();
            //
        }
        public DataSet GET_USER_BY_USER_PWD(string custid, string pwd)
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.GET_USER_BY_USER_PWD", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.PWD, OracleDbType.Varchar2, pwd, ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds, TBL_EB_USER_CHANNEL.TBLNAME);
            return ds;

        }
        public DataSet UPDATE_PWD_BY_USER_PWD(string custid, string pwd)
            {

                DataSet ret = new DataSet();
                dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.UPDATE_PWD_BY_USER_PWD", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.PWD, OracleDbType.Varchar2, pwd, ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ret);
            //retStr = ret.Tables[0].Rows[0]["ret"].ToString();
            return ret;

        }
        public DataSet CHECK_LOGIN_AND_ACTIVE(string mobile_no, string pwd)
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.CHECK_LOGIN_AND_ACTIVE", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add(MOB_MOBILE_NO, OracleDbType.Varchar2, mobile_no, ParameterDirection.Input);
                dsCmd.Parameters.Add(MOB_PWD, OracleDbType.Varchar2, pwd, ParameterDirection.Input);
                dsCmd.Parameters.Add(RECEIVED_TIME, OracleDbType.Varchar2, "", ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
                // Xu ly code tra ve o day 
            }
        public DataSet ACTIVE_CONFIRM(string mobile_no, string active_code)
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.CHECK_ACTIVE_CODE", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add(MOB_MOBILE_NO, OracleDbType.Varchar2, mobile_no, ParameterDirection.Input);
                dsCmd.Parameters.Add(ACTIVE_CODE, OracleDbType.Varchar2, active_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(RECEIVED_TIME, OracleDbType.Varchar2, "", ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
        }
        public DataSet UPDATE_LANGUE_BY_USER(string custid, string lang)
        {
            DataSet ds = new DataSet();
            	dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.UPDATE_LANGUAGE_BY_USER", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.DEFAULT_LANG, OracleDbType.Varchar2, lang, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, "MOB", ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
        }
        public DataSet UPDATE_AVATAR_BY_USER(string custid, string avatar)
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.UPDATE_LANGUAGE_BY_USER", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.DEFAULT_LANG, OracleDbType.Blob, avatar, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;

        }
        public DataSet UPDATE_FAV_MENU_BY_USER(string custid, string fav_menu)
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.UPDATE_LANGUAGE_BY_USER", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.FAVORITE_MENU, OracleDbType.Varchar2, fav_menu, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, "MOB", ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;

        }
        public DataSet GET_MOBILE_TOKEN(string custid, string channel_id, string received_time, string ip, string user_agent)
        {

                DataSet ret = new DataSet();
            dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.INSERT_MOBILE_TOKEN", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNELs.RECEIVED_TIME, OracleDbType.Varchar2, received_time, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM1, OracleDbType.Varchar2, "", ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM2, OracleDbType.Varchar2, "", ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM3, OracleDbType.Varchar2, "", ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM4, OracleDbType.Varchar2, "", ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM5, OracleDbType.Varchar2, "", ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM6, OracleDbType.Varchar2, "", ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM7, OracleDbType.Varchar2, "", ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM8, OracleDbType.Varchar2, ip, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.BM9, OracleDbType.Varchar2, user_agent, ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ret);

            return ret;

        }

        public DataSet CHECK_TOKEN(string custid, string channel_id, string token, string received_time)
            {
            DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.CHECK_AND_UPDATE_MOBILE_TOKEN", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID , ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNELs.TOKEN, OracleDbType.Varchar2, token, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNELs.RECEIVED_TIME, OracleDbType.Varchar2, received_time, ParameterDirection.Input);
                dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
            return ds;

        }

        public DataSet GET_USER_BY_CIF(string custid)
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_IBANKING_630.GET_USER_BY_CIF", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds, TBL_EB_USER_CHANNEL.TBLNAME);
            return ds;

        }

        public DataSet UPDATE_BM_TOKEN_POPUP(string custid)
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_IBANKING_630.UPDATE_BM_TOKEN_POPUP", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
        }

        public DataSet GET_USER_BY_MOBILE(string mobile)
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_IBANKING_630.GET_USER_BY_MOBILE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.AUTH_INFO_EXT1, OracleDbType.Varchar2, mobile, ParameterDirection.Input);
            dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds, TBL_EB_USER_CHANNEL.TBLNAME);
            return ds;

        }

    }
}

//Begin edit
