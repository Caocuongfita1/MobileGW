using System;
using System.Data;

namespace mobileGW.Service.Framework
{
    /// <summary>
    /// Summary description for SOBookingData.
    /// </summary>
    public class TIDE_BALANCE 
    {

      //ACCTNO ^ CCYCD ^ PROD_CD ^ CURR_PRIN_AMT ^ TENURE ^ UNIT_TENURE ^INT_RATE ^ VAL_DT ^ MAT_DT ^ CURR_MAT_AMT  ^ AUTO_REN_NO^PROD_DESC
        public const String ACCTNO = "ACCTNO";
        public const String CCYCD = "CCYCD"; //704
        public const String CCY = "CCYCD"; //VND
        public const String CCY_CD = "CCY_CD"; //VND

        public const String LAST_DATE = "LASTDATE";
        public const String OPEN_DATE = "OPNDATE";
        
        public const String PROD_CD = "PROD_CD";
        public const String PROD_DESC = "PROD_DESC";

        public const String POS_CD = "POS_CD";
        public const String POS_DESC = "POS_DESC";

        public const String CURR_PRIN_AMT = "CURR_PRIN_AMT";

        public const String INT_RATE = "INT_RATE";
        public const String RATE = "RATE";
        public const String VAL_DT = "VAL_DT";
        public const String MAT_DT = "MAT_DT";
        
        public const String CURR_MAT_AMT = "CURR_MAT_AMT";

        public const String INT_AMT = "INT_AMT";

        public const String AUTO_REN_NO = "AUTO_REN_NO";

        public const String TENURE = "TENURE";

        public const String UNIT_TENURE = "UNIT_TENURE";

        public const String DEPOSIT_NO = "DEPOSIT_NO";

        public const String TOTAL = "TOTAL";

        public const String INSTRUCTION = "INSTRUCTION";
        
        
    }
}
