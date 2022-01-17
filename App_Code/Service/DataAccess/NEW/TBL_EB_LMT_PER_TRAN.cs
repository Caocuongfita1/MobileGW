using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TBL_EB_LMT_PER_TRAN
/// </summary>
public class TBL_EB_LMT_PER_TRAN
{

    public string REMAIN_LIMIT_PER_DAY { get; set; }
    public string LIMIT_PER_TRAN1 { get; set; }//Hạn mức giao dịch TRAN_TYPE_ACQ_247AC
    public string LIMIT_PER_TRAN2 { get; set; }//Hạn mức giao dịch TRAN_TYPE_ACQ_247CARD
    public string LIMIT_PER_TRAN3 { get; set; }//Hạn mức giao dịch TRAN_TYPE_247_DOMESTIC

    public string REG_USER { get; set; }//Define loai KH (cho eKYC)
    public string LIMIT_AMT_MONTH { get; set; }//Hạn mức giao dịch thang
}