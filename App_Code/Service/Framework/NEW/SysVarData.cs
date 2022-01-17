using System;
using System.Data;

namespace mobileGW.Service.Framework
{
	/// <summary>
	/// Summary description for SysVar.
	/// </summary>
	public class SysVarData: DataSet
	{
		public const String SYSVAR_TABLE = "SYSVAR_TABLE";
		public const String SYSVARNAME_FIELD = "SYS_VAR_NAME";
		public const String SYSVARSTS_FIELD = "SYS_VAR_STATUS";
		public const String SYSVARDESC_FIELD = "SYS_VAR_DESC";
		
		public SysVarData()
		{
			//
			CreateDataSet();
			//
		}

		public void CreateDataSet()
		{
			DataTable table;
			DataColumnCollection columns;
			table = new DataTable(SYSVAR_TABLE);
			columns = table.Columns;
			columns.Add(SYSVARNAME_FIELD,typeof(System.String));
			columns.Add(SYSVARSTS_FIELD,typeof(System.Int64));
			columns.Add(SYSVARDESC_FIELD,typeof(System.String));
			this.Tables.Add(table);
		}
	}
}
