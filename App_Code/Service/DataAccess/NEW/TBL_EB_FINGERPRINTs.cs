using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;
namespace mobileGW.Service.DataAccess
{
    /// <summary>
    /// Summary description for TBL_EB_FINGERPRINTs
    /// </summary>
    public class TBL_EB_FINGERPRINTs
    {
        public const String CURSOR_PARM = "REF_CUR";

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
        public TBL_EB_FINGERPRINTs()
        {
            //
            dsApt = new OracleDataAdapter();
        }


        /*
         PCUSTID IN VARCHAR2 
           , PPWD IN VARCHAR2
           , PCHANNEL_ID IN VARCHAR2
           , PSETTING_TYPE IN VARCHAR2
           , PDEVICE_ID IN VARCHAR2
           , PDEVICE_TYPE IN VARCHAR2
           , PTOKEN IN VARCHAR2           
           , PIP IN VARCHAR2              
           , PUSER_AGENT IN VARCHAR2  
         * */
        public DataSet SET_FINGER_PRINT(string custid, string pwd,  string setting_type, string device_id, 
            string device_type, string token, string ip, string user_agent)
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_FINGERPRINT.SET_FINGER_PRINT", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;

            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.PWD, OracleDbType.Varchar2, pwd, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.SETTING_TYPE, OracleDbType.Varchar2, setting_type, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.DEVICE_ID, OracleDbType.Varchar2, device_id, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.DEVICE_TYPE, OracleDbType.Varchar2, device_type, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.TOKEN, OracleDbType.Varchar2, token, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.IP, OracleDbType.Varchar2, ip, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.USER_AGENT, OracleDbType.Varchar2, user_agent, ParameterDirection.Input);
            dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds, TBL_EB_FINGERPRINT.TBLNAME);
            return ds;
        }


        public DataSet GET_USER_BY_USER_FP(string custid, string pwd, string setting_type, string device_id,
            string device_type, string token, string ip, string user_agent)
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_FINGERPRINT.GET_USER_BY_USER_FP", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.PWD, OracleDbType.Varchar2, pwd, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.SETTING_TYPE, OracleDbType.Varchar2, setting_type, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.DEVICE_ID, OracleDbType.Varchar2, device_id, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.DEVICE_TYPE, OracleDbType.Varchar2, device_type, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.TOKEN, OracleDbType.Varchar2, token, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.IP, OracleDbType.Varchar2, ip, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.USER_AGENT, OracleDbType.Varchar2, user_agent, ParameterDirection.Input);
            dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds, TBL_EB_USER_CHANNEL.TBLNAME);
            return ds;

        }

        public bool RESET_USER_FP_TOKEN(string custid, string token)
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_FINGERPRINT.RESET_USER_FP_TOKEN", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.TOKEN, OracleDbType.Varchar2, token, ParameterDirection.Input);
            dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds, TBL_EB_USER_CHANNEL.TBLNAME);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                if (ds.Tables[0].Rows[0]["RET"].ToString() == Config.ERR_CODE_DONE)
                    return true;
                else
                    return false;
            else
                return false;

        }
        public bool LOGOUT_AND_EXPIRE_TOKEN(string custid, string token)
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.LOGOUT_AND_EXPIRE_TOKEN", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.CHANNEL_ID, OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_FINGERPRINT.TOKEN, OracleDbType.Varchar2, token, ParameterDirection.Input);
            dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds, TBL_EB_USER_CHANNEL.TBLNAME);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                if (ds.Tables[0].Rows[0]["RET"].ToString() == Config.ERR_CODE_DONE)
                    return true;
                else
                    return false;
            else
                return false;

        }


        
    }

}