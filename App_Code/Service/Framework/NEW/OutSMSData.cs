using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
namespace mobileGW.Service.Framework
{
	/// <summary>
	/// Summary description for OutSMS.
	/// </summary>
	public class OutSMSData 
	{
		public const String OSMS_TABLE = "MT_QUEUE";
		public const String ID_FIELD = "ID";
		public const String SERVICE_ID_FIELD = "SERVICE_ID";
		public const String USER_ID_FIELD = "USER_ID";
		public const String INFO_FIELD = "INFO";
		public const String REQUEST_ID_FIELD = "REQUEST_ID";
		public const String PROVIDER_ID_FIELD = "PROVIDER_ID";
		public const String RECEIVE_TIME_FIELD = "RECEIVE_TIME";
		public const String SEND_TIME_FIELD = "SEND_TIME";
		public const String TOTAL_MSG_FIELD = "TOTAL_MSG";
		public const String MSG_IDX_FIELD = "MSG_IDX";
		public const String IS_MORE_FIELD = "IS_MORE";

		public OutSMSData()
		{

		}

	}
}
