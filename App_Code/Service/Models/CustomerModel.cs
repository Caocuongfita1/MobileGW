using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CustomerModel
/// </summary>
public class CustomerModel
{
    public string AUTH_METHOD_NAME { get; set; } //goi dich vu

    public string CUSTNAME { get; set; }

    public string EMAIL { get; set; }

    public string USERNAME { get; set; }


    public string CUSTID { get; set; }

    public string MOBIL_NO { get; set; }

    public string AUTH_INFO_EXT1 { get; set; } // cho kenh dang ky shb mobile

    public string IS_ENABLED { get; set; } //dung de check co tam khoa dich vu mobile hay ko

    public string AUTH_METHOD { get; set; } //dung de check goi dang ky kenh mobile

    public string ISMGOLD { get; set; }

    public string ISMBASIC { get; set; }

    public string ISNOTHING { get; set; }

    public string OLD_AUTH_METHOD { get; set; }

    public string OLD_MOBIL_NO { get; set; }

    public string DATE_TIME { get; set; }

    public string MOBILE_NO_REGISTER { get; set; }

    public string DEFAULT_ACCT { get; set; }

    public string SEC_EXP_DT { get; set; }
}

public class Password
{

    public string currPassword { get; set; } //mat khau hien tai

    public string newPassword { get; set; }

    public string newPassAgain { get; set; }


}

public class TBL_EB_TRAN_SMS_CODE
{
    public string SMSCODE { get; set; }

}


public class Users
{
    public string SEX_CD { get; set; }

    public string STAFF_CD { get; set; }

    public string CUSTNAME { get; set; }

    public string EMAIL { get; set; }

    public string TIME_LOGIN { get; set; }

    public double MIN_BAL_VAL { get; set; }

    public string IB_TOKEN { get; set; }

    //time of eSecure
    public string ESEC_ISS_DT { get; set; }

    public string SEC_EXP_DT { get; set; }

    public string CUSTID { get; set; }

    public string CHANNEL_ID { get; set; }

    public string USERNAME { get; set; }

    public string ORG_PWD_CLEAR { get; set; }

    public string ORG_PWD { get; set; }

    public string PWD { get; set; }

    public string PKG_LIMIT_ID { get; set; }

    public string IS_OVERRIDE_LIMIT { get; set; }
    public string REG_BRANCH { get; set; }

    public string LIMIT_AMT_INTRA { get; set; }

    public string LIMIT_AMT_INTER { get; set; }
    public string LIMIT_AMT_PAYMENT { get; set; }

    public string LIMIT_AMT_STOCK { get; set; }

    public string CUR_AMT_INTRA { get; set; }

    public string CUR_AMT_INTER { get; set; }
    public string CUR_AMT_PAYMENT { get; set; }

    public string CUR_AMT_STOCK { get; set; }

    public string IS_ENABLED { get; set; }

    public string IS_ACTIVATED { get; set; }

    public string REQ_PWD_CHANGE { get; set; }

    public string AUTH_METHOD { get; set; }

    public string LAST_AUTH_METHOD { get; set; }

    public string AUTH_INFO_EXT1 { get; set; }

    public string AUTH_INFO_EXT2 { get; set; }

    public string LAST_AUTH_INFO_EXT1 { get; set; }

    public string LAST_AUTH_INFO_EXT2 { get; set; }

    public string DEFAULT_ACCT { get; set; }

    public string DEFAULT_LANG { get; set; }

    public string FAVORITE_MENU { get; set; }

    public string BM1 { get; set; }

    public string BM2 { get; set; }

    public string BM3 { get; set; }

    public string BM4 { get; set; }

    public string BM5 { get; set; }

    public string BM6 { get; set; }

    public string BM7 { get; set; }

    public string BM8 { get; set; }

    public string BM9 { get; set; }

    public string BM10 { get; set; }


}

public class RetCode
{
    public string RET_CODE { get; set; }
    public string RET_VAL { get; set; }

    public string MDATA { get; set; }
    public string REL_VAL { get; set; }
    public string BSNS_DT { get; set; }

    public string TRAN_ID { get; set; }
}