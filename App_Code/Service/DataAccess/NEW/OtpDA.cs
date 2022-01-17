using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using mobileGW.Service.Framework;
namespace mobileGW.Service.DataAccess
{
    /// <summary>
    /// Summary description for OtpDA
    /// </summary>
    public class OtpDA : IDisposable
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


        public OtpDA()
        {
            //
            dsApt = new OracleDataAdapter();
            //
        }

        public DataTable checkOtpBeforeTransaction(String cifNo, String inputTokenOtp, String requestId, int typeOtp)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_IBANKING_630.GET_USER_BY_REQUEST_ID", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;

                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.CUSTID, OracleDbType.Varchar2, cifNo, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.REQUEST_ID, OracleDbType.Varchar2, requestId, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.TYPE_OTP, OracleDbType.Int16, typeOtp, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.CHANNEL_ID, OracleDbType.Varchar2, "MOB", ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);

                dsApt.SelectCommand = dsCmd;

                dsApt.Fill(ds);
                Funcs.WriteLog("checkOtpBeforeTransaction: FINISH");
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        public DataTable insertTokenOtp(String cifNo, String tokenOtp, int typeOtp, double expireTime, int status)
        {
            return insertTokenOtp(cifNo, tokenOtp, typeOtp, expireTime, status,
                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);
        }

        public DataTable insertTokenOtp(String cifNo, String tokenOtp, int typeOtp, double expireTime, int status, String bm1,
            String bm2, String bm3, String bm4, String bm5, String bm6, String bm7, String bm8, String bm9, String bm10)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_IBANKING_630.INSERT_TBL_EB_TOKEN_OTP", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                //dsCmd.Parameters.Add(TRAN_ID, OracleDbType.* **
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.CUSTID, OracleDbType.Varchar2, cifNo, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.TOKEN_OTP, OracleDbType.Varchar2, tokenOtp, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.TYPE_OTP, OracleDbType.Int16, typeOtp, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.SECONDS_EXPIRE, OracleDbType.Double, expireTime, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.STATUS, OracleDbType.Int16, status, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM1, OracleDbType.Varchar2, bm1, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM2, OracleDbType.Varchar2, bm2, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM3, OracleDbType.Varchar2, bm3, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM4, OracleDbType.Varchar2, bm4, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM5, OracleDbType.Varchar2, bm5, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM6, OracleDbType.Varchar2, bm6, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM7, OracleDbType.Varchar2, bm7, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM8, OracleDbType.Varchar2, bm8, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM9, OracleDbType.Varchar2, bm9, ParameterDirection.Input);
                dsCmd.Parameters.Add(TBL_EB_TOKEN_OTP.BM10, OracleDbType.Varchar2, bm10, ParameterDirection.Input);

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

        public DataTable updateTokenOtp(String request_Id, String tokenOtp)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_IBANKING_630.UPDATE_TBL_EB_TOKEN_OTP", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                //dsCmd.Parameters.Add(TRAN_ID, OracleDbType.* **
                dsCmd.Parameters.Add("pREQUEST_ID", OracleDbType.Varchar2, request_Id, ParameterDirection.Input);
                dsCmd.Parameters.Add("pTOKEN_OTP", OracleDbType.Varchar2, tokenOtp, ParameterDirection.Input);

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
    }
}