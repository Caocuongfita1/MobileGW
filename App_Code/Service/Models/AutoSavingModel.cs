using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ChangePasswordModel
/// </summary>
public class AutoSavingModel
{
    public string REF_NO { get; set; }
    public string CIF_NO { get; set; }
    public string LEGACY_AC { get; set; }
    public string AC_NAME { get; set; }
    public double DEPOSIT_TYPE { get; set; }
    public string CCY_CD { get; set; }
    public double MIN_BAL { get; set; }
    public double PRIN_AMT { get; set; }
    public string TENURE { get; set; }
    public string TENURE_UNIT { get; set; }
    public double MAT_TYPE { get; set; }
    public string FREQ_BOOKING { get; set; }
    public string START_DT { get; set; }
    public string LAST_BOOK_DT { get; set; }
    public string POS_CD { get; set; }
    public double REG_ST { get; set; }
    public string SRC_REG { get; set; }
    public string MKR_ID { get; set; }
    public string MKR_DT { get; set; }
    public string AUTH_ID { get; set; }
    public string AUTH_DT { get; set; }
}

public class AutoSavingItemModel
{
    public string LEGACY_AC { get; set; }
    public double MIN_BAL { get; set; }
    public double PRIN_AMT { get; set; }
}

public class HistAutoSavingItemModel
{
    public string OPEN_DATE { get; set; }
    public string ACCOUNT_NUMBER { get; set; }
    public double AMOUNT { get; set; }
}

public class ListAutoSavingModel
{
    public string ErrCode { get; set; }
    public string ErrDesc { get; set; }
    public List<AutoSavingItemModel> ListAutoSaving { get; set; }
}

public class ListHistAutoSavingModel
{
    public string ErrCode { get; set; }
    public string ErrDesc { get; set; }
    public List<HistAutoSavingItemModel> ListHistAutoSaving { get; set; }
}