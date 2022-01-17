using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ChangePasswordModel
/// </summary>
public class CasaDetailModel
{
        public string SRC_ACCT { get; set; }
        public string DES_ACCT { get; set; }
        public string DES_NAME { get; set; }
        public string BANK_CODE { get; set; }
        public string BANK_NAME { get; set; }
        public string TRAN_TYPE { get; set; }
        public string CHANNEL_ID { get; set; }
        public string CUSTID { get; set; }
        public double AMOUNT { get; set; }
        public string TXDESC { get; set; }
        public string CORE_REF_NO { get; set; }
        public double STATUS { get; set; }
        public double SUSPEND_AMOUNT { get; set; }
        public double FEE_AMOUNT { get; set; }
        public double VAT_AMOUNT { get; set; }
        public string CORE_TXDATE { get; set; }
        public string POS_CD { get; set; }
        public string ORDER_ID { get; set; }
        public string  MERCHANT_CODE { get; set; }
        public string CCY_CD { get; set; }
    
}