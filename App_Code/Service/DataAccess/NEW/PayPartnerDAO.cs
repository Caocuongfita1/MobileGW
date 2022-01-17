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
    public class PayPartnerDAO
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
        public PayPartnerDAO()
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
        public DataTable GetPartnerInfo(string PartnerCd, string billCode)
        {
            try
            {
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "pkg_payment_new.GET_PARTNER_ACCNO_BY_PCODE", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("V_BILLCODE", OracleDbType.Varchar2, billCode, ParameterDirection.Input);
                dsCmd.Parameters.Add("V_PARTNERID", OracleDbType.Varchar2, PartnerCd, ParameterDirection.Input);

                dsCmd.Parameters.Add("MY_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
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
