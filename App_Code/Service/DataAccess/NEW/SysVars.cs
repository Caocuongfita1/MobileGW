using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;


namespace mobileGW.Service.DataAccess
{
	/// <summary>
	/// Summary description for SysVars.
	/// </summary>
	public class SysVars:IDisposable
	{
		public const String SYSVARNAME_PARM = "V_SYS_VAR_NAME";
		public const String SYSVARSTS_PARM = "V_SYS_VAR_STATUS";
		public const String LASTCHANGE_PARM = "V_LAST_CHANGE";
		public const String BRCODE_PARM = "V_BRCODE";
		public const String MODE_PARM = "V_MODE";
		public const String CURSOR_PARM = "OUT_CUR";

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

		public SysVars()
		{
			//
				dsApt = new OracleDataAdapter();
			//
		}

		public string getBUSDATE(string BRCODE,string mode)
		{
			try
			{
				dsCmd = new OracleCommand( "PKG_IBANKING.GET_BUSDATE_BR",new OracleConnection(Config.gINTELLECTConnstr));
				dsCmd.CommandType = CommandType.StoredProcedure;
				dsCmd.Parameters.Add(BRCODE_PARM, OracleDbType.Varchar2,ParameterDirection.Input);
				dsCmd.Parameters.Add(MODE_PARM, OracleDbType.Varchar2,ParameterDirection.Input);
				dsApt.SelectCommand = dsCmd;
				dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor,ParameterDirection.Output);
				dsCmd.Parameters[BRCODE_PARM].Value = BRCODE;
				dsCmd.Parameters[MODE_PARM].Value = mode;
				DataSet ds = new DataSet();
				dsApt.Fill(ds);
				if ((ds.Tables[0]!=null)&&(ds.Tables[0].Rows.Count>0))
				{
					return ds.Tables[0].Rows[0][0].ToString();
				}
				return null;
			}
			catch ( Exception ex)
			{
				Funcs.WriteLog("*[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]"+  ex.ToString());
				return null;
			}
		}

		public SysVarData getSysVar(string SysVarName)
		{
			try
			{
				SysVarData sysVatData  = new SysVarData();
                dsCmd = new OracleCommand("PKG_MOBILEBANKING_NEW.GETSYSVAR_NAME", new OracleConnection(Config.gEBANKConnstr));
				dsCmd.CommandType = CommandType.StoredProcedure;
				dsCmd.Parameters.Add(SYSVARNAME_PARM, OracleDbType.Varchar2,ParameterDirection.Input);
				dsApt.SelectCommand = dsCmd;
				dsCmd.Parameters.Add(CURSOR_PARM, OracleDbType.RefCursor,ParameterDirection.Output);
				dsCmd.Parameters[SYSVARNAME_PARM].Value = SysVarName;
				dsApt.Fill(sysVatData,SysVarData.SYSVAR_TABLE);
				return sysVatData;
			}
			catch ( Exception ex)
			{
				Funcs.WriteLog("*[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]"+  ex.ToString());
				return null;
			}
		}

		public bool uptSysVar(string SysVarName, long vvalue)
		{
			try
			{
				SysVarData sysVatData  = new SysVarData();
				dsCmd = new OracleCommand("PKG_IBANKING.UPTSYSVAR_NAME",new OracleConnection(Config.gEBANKConnstr));
				dsCmd.CommandType = CommandType.StoredProcedure;
				dsCmd.Parameters.Add(SYSVARNAME_PARM, OracleDbType.Varchar2,ParameterDirection.Input);
				dsCmd.Parameters.Add(SYSVARSTS_PARM, OracleDbType.Long,ParameterDirection.Input);
				dsCmd.Parameters.Add(LASTCHANGE_PARM, OracleDbType.Varchar2,ParameterDirection.Input);
				dsApt.SelectCommand = dsCmd;
				dsCmd.Parameters[SYSVARNAME_PARM].Value = SysVarName;
				dsCmd.Parameters[SYSVARSTS_PARM].Value = vvalue;
				dsCmd.Parameters[LASTCHANGE_PARM].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
				dsApt.Fill(sysVatData,SysVarData.SYSVAR_TABLE);
				return true;
			}
			catch ( Exception ex)
			{
				Funcs.WriteLog("*[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]"+  ex.ToString());
				return false;
			}
		}




    }
}
