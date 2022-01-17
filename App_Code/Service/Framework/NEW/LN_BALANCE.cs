using System;
using System.Data;

namespace ibanking_pol.Service.Framework
{
    /// <summary>
    /// Summary description for BalanceData.
    /// </summary>
    public class LNBalanceData
    {
        public const String TBL_LN_BALANCE = "BALANCE";
        // Current Balance
        public const String ORGINAL_BAL_FIELD = "OBALANCE";
        public const String RLSMAT_FIELD = "RLSAMT";
        public const String ACCT_BAL_FIELD = "BALANCE";
        public const String PRINPAID_FIELD = "PRINPAID";
        public const String OPEN_DATE_FIELD = "OPENDT";
        public const String ACCT_TYPE_FIELD = "TYPENAME";
        public const String ACCT_CCY_FIELD = "CCYCD";
        public const String ACCT_NO_FIELD = "ACCTNO";
        public const String CUST_ID_FIELD = "CUSTID";
        public const String EXP_DATE_FIELD = "EXPDT";
        public const String OUT_STANDING_FIELD = "OUT_STANDING";
        public const String NEXT_INT_DUE_FIELD = "NEXT_INT_DUE";
        public const String NEXT_PRIN_DUE_FIELD = "NEXT_PRIN_DUE";

        public const String SINT_FIELD = "SINT";
        public const String INTPAID_FIELD = "INTPAID";
        public const String INTPREPAID_FIELD = "INTPREPAID";
        public const String INTPENPAID_FIELD = "IPENPAID";


        // them cot RET

        public const string RET = "RET";
        public LNBalanceData()
        {
            //

        }

    }
}
