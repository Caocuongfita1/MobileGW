using System;
using mobileGW.Service.Framework;
using mobileGW.Service.DataAccess;

namespace mobileGW.Service.Bussiness
{
	/// <summary>
	/// Summary description for SysVar.
	/// </summary>
	public class SysVar
	{
		public SysVar()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string getBusDate(String BrCode,string mode)
		{
			SysVars sysvars = new SysVars();
			string res =  sysvars.getBUSDATE(BrCode,mode);
			sysvars.Dispose();

			return res;
		}

		public SysVarData getSysVar(String SysVarName)
		{
			SysVarData sysVarData = new SysVarData();
			SysVars sysvars = new SysVars();
			sysVarData = sysvars.getSysVar(SysVarName);
			sysvars.Dispose();

			if ((sysVarData!=null)&&(sysVarData.Tables[SysVarData.SYSVAR_TABLE].Rows.Count > 0))
			{
				return sysVarData;
			}
			else
			{
				return null;
			} 
		}

		public bool uptSysVar(String SysVarName, long vvalue)
		{
		
			SysVars sysvars = new SysVars();
			bool res = sysvars.uptSysVar(SysVarName,vvalue);
			sysvars.Dispose();
			return res;
		}

	}

}
