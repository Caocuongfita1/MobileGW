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
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Utility
/// </summary>
public class EVNHNDAO
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
    public EVNHNDAO()
    {
        //
        dsApt = new OracleDataAdapter();
        //
    }

    public DataTable GET_PARTNER_ELECTRICITY(string billCode)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "pkg_payment_new.GET_ELECTRICITY_PART_BY_PCODE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("V_BILLCODE", OracleDbType.Varchar2, billCode, ParameterDirection.Input);
            dsCmd.Parameters.Add("MY_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
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
