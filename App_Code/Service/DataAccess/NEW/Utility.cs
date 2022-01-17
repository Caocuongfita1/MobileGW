using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;
using mobileGW.Service.DataAccess;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Utility
/// </summary>
public class Utility
{
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
    public Utility()
    {
        //
        dsApt = new OracleDataAdapter();
        //
    }



    public DataTable CHECK_MULTI_CURRENCY(string SRC_ACCT, string DES_ACCT)
    {
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_EB_QRY.CHECK_MULTI_CURRENCY", new OracleConnection(Config.gINTELLECTConnstr));
            dsCmd.Parameters.Add("V_SRC_ACCT", OracleDbType.Varchar2, SRC_ACCT, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_DES_ACCT", OracleDbType.Varchar2, DES_ACCT, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("*[CHECK_MULTI_CURRENCY " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
            return null;
        }
    }


    public DataTable CHECK_CASA_NODEBIT(string SRC_ACCT)
    {
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_EB_QRY.CHECK_CASA_NODEBIT", new OracleConnection(Config.gINTELLECTConnstr));
            dsCmd.Parameters.Add("V_ACCT", OracleDbType.Varchar2, SRC_ACCT, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("*[CHECK_CASA_NODEBIT " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]" + ex.ToString());
            return null;
        }
    }

    public DataTable GET_BANK_CODE_CITAD()
    {
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_BANK_CODE_CITAD", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            return null;

        }
    }

    public DataTable GET_CITY_CITAD(string bank_code)
    {
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_CITY_CITAD", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add(TBL_EB_TRAN.BANK_CODE, OracleDbType.Varchar2, bank_code, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;

            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            return null;

        }
    }


    public DataTable GET_BANK_BRANCH_CITAD(string bank_code, string bank_city)
    {
        DataSet ds = new DataSet();
        dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_BANK_BRANCH_CITAD", new OracleConnection(Config.gEBANKConnstr));
        dsCmd.Parameters.Add(TBL_EB_TRAN.BANK_CODE, OracleDbType.Varchar2, bank_code, ParameterDirection.Input);
        dsCmd.Parameters.Add(TBL_EB_TRAN.BANK_CITY, OracleDbType.Varchar2, bank_city, ParameterDirection.Input);
        dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
        dsCmd.CommandType = CommandType.StoredProcedure;
        dsApt.SelectCommand = dsCmd;
        dsApt.Fill(ds);

        return ds.Tables[0];
    }

    public DataTable GET_BANK_CODE_247_AC2AC()
    {
        DataSet ds = new DataSet();
        dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_BANK_CODE_247_AC2AC", new OracleConnection(Config.gEBANKConnstr));
        dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
        dsCmd.CommandType = CommandType.StoredProcedure;
        dsApt.SelectCommand = dsCmd;
        dsApt.Fill(ds);

        return ds.Tables[0];
    }

    public DataTable GET_SERVICES()
    {
        DataSet ds = new DataSet();
        dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_SERVICES", new OracleConnection(Config.gEBANKConnstr));
        dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
        dsCmd.CommandType = CommandType.StoredProcedure;
        dsApt.SelectCommand = dsCmd;
        dsApt.Fill(ds);

        return ds.Tables[0];
    }


    public DataTable GET_CATEGORY_BY_TRAN_TYPE(string tran_type)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_CATEGORY_BY_TRAN_TYPE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add(PAY_SERVICE.TRAN_TYPE, OracleDbType.Varchar2, tran_type, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }

    #region "STOCK"
    public DataTable GET_STOCK_BRANCH_LIST(string tran_type, string branch_code)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_STOCK_BRANCH_LIST", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("tran_type", OracleDbType.Varchar2, tran_type, ParameterDirection.Input);
            dsCmd.Parameters.Add("branch_code", OracleDbType.Varchar2, branch_code, ParameterDirection.Input);

            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }

    #endregion "STOCK"


    #region "CHARITY"
    public DataTable GET_CHARITY_LIST(string charity_code)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_CHARITY_LIST", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("charity_code", OracleDbType.Varchar2, charity_code, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }

    #endregion "CHARITY"

    #region "TIDE ONLINE"
    public DataTable GET_ACCT_TIDE_OL_INFO_LIST(string custid, string acctno, string ccycd)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_TIDE_OL_INFO_LIST", new OracleConnection(Config.gINTELLECTConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
            dsCmd.Parameters.Add("ccycd", OracleDbType.Varchar2, ccycd, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }


    /// <summary>
    /// Linhtn 19 jul 2016
    /// Lấy lãi suất tiết kiệm theo loại tiền, kỳ hạn
    /// </summary>
    /// <param name="ccycd"></param>
    /// <param name="tenure"></param>
    /// <param name="unit_tenure"></param>
    /// <returns></returns>
    public DataTable getTIDERATE(string ccycd, string tenure, string unit_tenure, double amount)
    {
        // IN VARCHAR2,  in varchar2,  in varchar2
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_COMMON_CORE.getTideRate", new OracleConnection(Config.gINTELLECTConnstr));

            dsCmd.Parameters.Add("ccycd", OracleDbType.Varchar2, ccycd, ParameterDirection.Input);
            dsCmd.Parameters.Add("tenure", OracleDbType.Varchar2, tenure, ParameterDirection.Input);
            dsCmd.Parameters.Add("unit_tenure", OracleDbType.Varchar2, unit_tenure, ParameterDirection.Input);
            dsCmd.Parameters.Add("amount", OracleDbType.Double, amount, ParameterDirection.Input);

            dsCmd.Parameters.Add("OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }

    /*
     Message Keys	Mandatory / Optional	Key Type	Default Value	Remarks
hdr_Tran_Id	M	String	TIDEBOOKING	Transaction Identifier
hdr_Status	M	String	NULL	Header Identifier
req_Interface_Id	M	String		Interface Identifier
req_Chnl_Id	M	String		Channel Identifier
req_Txn_Dt	M	String		Transaction Date
req_Ref_No	M	String		Reference Number
req_Acc_No	M	String		CASA Account Number for Funding
req_Ccy	M	Number		Currency Code
req_Tide_No	M	String		Tide Base Number
req_Tenure_Unit	M	String		Tenure Unit
req_Tenure	M	Number		Tenure
req_Amt	M	Number		TIDE Amount
req_Prod_Cd	M	Number		Product Code
req_Dep_Type	M	String		Possible Values - RGLR - Regular Deposits, RECR - Recurring Deposits
req_Prin_On_Mat	M	String		Possible Values - CASA - CASA, ROLL - Rollover
req_Int_On_Mat	M	String		Possible Values - CASA - CASA, ROLL - Rollover
				
				
				
				
Message Keys	Mandatory / Optional	Key Type	Default Value	Remarks
res_Ref_No	M	String		Reference Number
res_Deposit_No	M	String		Deposit Number
res_Txn_Ref_No	M	String		Transaction Reference Number
res_Legacy_Tide_No	M	String		Legacy TIDE Account Number
res_Int_Rate	M	Double		Interest Rate
res_Int_Amt_On_Mat		Double		Interest Amount on Maturity

     */

    /*
     //OLD
     /// <summary>
    /// Linhtn 19 jul 2016
    /// </summary>
    /// <param name="tran_id"></param>
    /// <param name="refno"></param>
    /// <param name="debit_account"></param>
    /// <param name="ccy"></param>
    /// <param name="unit_tenure"></param>
    /// <param name="tenure"></param>
    /// <param name="amount"></param>
    /// <param name="prod_cd"></param>
    /// <param name="dep_type"></param>
    /// <param name="prin_on_mat"></param>
    /// <param name="int_on_mat"></param>
    /// <param name="addnl_field"></param>
    /// <param name="pos_code"></param>
    /// <param name="core_txno_ref"></param>
    /// <param name="core_txdate"></param>
    /// <param name="res_Legacy_Tide_No"></param>
    /// <param name="res_Mat_Dt"></param>
    /// <param name="res_Int_Amt_On_Mat"></param>
    /// <returns></returns>
    public string postTideBookingToCore(
        double tran_id
        // , string refno // REFNO
        , string debit_account // ACCTNO
        , string ccy // CCY,// string TIDE_NO, 
        , string unit_tenure // TENURE_UNIT
        , double tenure //TENURE
        , double amount //AMOUNT
        , string prod_cd //PROD_CD
        , string dep_type// DEP_TYPE
        , string prin_on_mat //PRIN_ON_MAT
        , string int_on_mat //INT_ON_MAT
        , string addnl_field // ADDNL_FIELD
        , string pos_code // POS_CODE
        , ref string core_txno_ref
        , ref string core_txdate
        , ref string res_Legacy_Tide_No
        , ref string res_Mat_Dt
        , ref string res_Int_Amt_On_Mat
        , ref string res_val_dt
        , ref string res_Int_Rate
        )
    {


        mobileGW.SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
        myWS.Timeout = Config.TIMEOUT_WITH_CORE;

        Transfers tf = new Transfers();
        string retws = "";
        core_txno_ref = "";
        res_Legacy_Tide_No = "";
        res_Mat_Dt = "";
        res_Int_Amt_On_Mat = "";
        string refno = "";

        try
        {

            refno = Config.refFormat + tran_id.ToString().PadLeft(9, '0');
            string retStr =
                myWS.TIDEBOOKING(Config.InterfaceID, Config.ChannelID, refno, debit_account, ccy, unit_tenure, tenure, amount, prod_cd,
                dep_type, prin_on_mat, int_on_mat, addnl_field, pos_code, out retws);


            DataSet ds = Funcs.Node2Ds(retStr, "HEADER");

            if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
                == Config.gResult_INTELLECT_Arr[0].Split('|')[0])
            {

                string TxnDate = ds.Tables[0].Rows[0]["res_Tran_Time"].ToString();
                res_Legacy_Tide_No = ds.Tables[0].Rows[0]["res_Legacy_Tide_No"].ToString();
                string ref_No_Core = ds.Tables[0].Rows[0]["res_Ref_No"].ToString();
                string mat_date = ds.Tables[0].Rows[0]["res_Mat_Dt"].ToString();
                res_Mat_Dt = mat_date.Substring(6, 2) + "/" + mat_date.Substring(4, 2) + "/" + mat_date.Substring(0, 4);
                res_val_dt = TxnDate.Substring(6, 2) + "/" + TxnDate.Substring(4, 2) + "/" + TxnDate.Substring(0, 4);

                res_Int_Amt_On_Mat = ds.Tables[0].Rows[0]["res_Int_Amt_On_Mat"].ToString();

                res_Int_Rate = ds.Tables[0].Rows[0]["res_Int_Rate"].ToString();

                //core_txno_ref = ref_No_Core;
                //linhtn fix 2016 12 30

                core_txno_ref = ds.Tables[0].Rows[0]["res_Ref_No"].ToString();
                core_txdate = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);


                //update TBL_EB_TRAN
                //string TxnDate = String.Format("{0:dd/MM/yyyy}", DateTime.Now);

                //tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno_ref, TxnDate, Config.ChannelID);

                //objTran.updateEB_TRAN(TRANID, Config.TX_STATUS_DONE, ds.Tables[0].Rows[0]["res_Legacy_Tide_No"].ToString(), "*",
                //    ds.Tables[0].Rows[0]["res_Ref_No"].ToString(), TxnDate, "*", ds.Tables[0].Rows[0]["res_Result_Code"].ToString(), "*", ref_No_Core, "*", "*", "*", "*", "*");

                myWS.Dispose();
                ds.Dispose();
                myWS = null;
                return Config.gResult_INTELLECT_Arr[0];
            }
            else
            {
                // Update la bi loi

                //objTran.updateEB_TRAN(TRANID, Config.TX_STATUS_FAIL, "*", "*",
                //    "*", "*", "*", ds.Tables[0].Rows[0]["res_Result_Code"].ToString(), "*", "*", "*", "*", "*", "*", "*");

                // tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno_ref, "", Config.ChannelID);

                myWS.Dispose();
                ds.Dispose();
                myWS = null;

                // Return the error
                for (int i = 2; i < Config.gResult_INTELLECT_Arr.Length; i++)
                {
                    if (Config.gResult_INTELLECT_Arr[i].Split('|')[0] == ds.Tables[0].Rows[0]["res_Result_Code"].ToString())
                    {
                        return Config.gResult_INTELLECT_Arr[i];
                    }
                }
                return Config.gResult_INTELLECT_Arr[1];
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            //RES_CORE = "";
            //// Update la bi loi
            //objTran.updateEB_TRAN(TRANID, Config.TX_STATUS_FAIL, "*", "*",
            //    "*", "*", "*", Config.TIMEOUTCORE, "*", "*", "*", "*", "*", "*", "*");

            // tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno_ref, "", Config.ChannelID);
            //return Config.TIMEOUTCORE;

            //return Config.gResult_INTELLECT_Arr[1];
            return Config.ERR_CODE_TIMEOUT_WHEN_POST_TO_CORE;
        }
        finally
        {
            tf = null;
        }
    }
         
         */

    /// <summary>
    /// Linhtn 19 jul 2016
    /// </summary>
    /// <param name="tran_id"></param>
    /// <param name="refno"></param>
    /// <param name="debit_account"></param>
    /// <param name="ccy"></param>
    /// <param name="unit_tenure"></param>
    /// <param name="tenure"></param>
    /// <param name="amount"></param>
    /// <param name="prod_cd"></param>
    /// <param name="dep_type"></param>
    /// <param name="prin_on_mat"></param>
    /// <param name="int_on_mat"></param>
    /// <param name="addnl_field"></param>
    /// <param name="pos_code"></param>
    /// <param name="core_txno_ref"></param>
    /// <param name="core_txdate"></param>
    /// <param name="res_Legacy_Tide_No"></param>
    /// <param name="res_Mat_Dt"></param>
    /// <param name="res_Int_Amt_On_Mat"></param>
    /// <returns></returns>
    public string postTideBookingToCore(
        string custid //them tham so nay de ghi log
        , double tran_id
        // , string refno // REFNO
        , string debit_account // ACCTNO
        , string ccy // CCY,// string TIDE_NO, 
        , string unit_tenure // TENURE_UNIT
        , double tenure //TENURE
        , double amount //AMOUNT
        , string prod_cd //PROD_CD
        , string dep_type// DEP_TYPE
        , string prin_on_mat //PRIN_ON_MAT
        , string int_on_mat //INT_ON_MAT
        , string addnl_field // ADDNL_FIELD
        , string pos_code // POS_CODE
        , ref string core_txno_ref
        , ref string core_txdate
        , ref string res_Legacy_Tide_No
        , ref string res_Mat_Dt
        , ref string res_Int_Amt_On_Mat
        , ref string res_val_dt
        , ref string res_Int_Rate
        )
    {


        Transfers tf = new Transfers();
        string retws = "";
        core_txno_ref = "";
        res_Legacy_Tide_No = "";
        res_Mat_Dt = "";
        res_Int_Amt_On_Mat = "";
        string refno = "";

        refno = Config.refFormat + tran_id.ToString().PadLeft(9, '0');


        PassBook.PassBookCreateResType res = null;
        PassBook.AppHdrType appHdr = new PassBook.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        PassBook.PairsType nsFrom = new PassBook.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        PassBook.PairsType nsTo = new PassBook.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        PassBook.PairsType[] listOfNsTo = new PassBook.PairsType[1];
        listOfNsTo[0] = nsTo;

        PassBook.PairsType BizSvc = new PassBook.PairsType();
        BizSvc.Id = "PassBookCreate";
        BizSvc.Name = "PassBookCreate";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        PassBook.PassBookCreateReqType msgReq = new PassBook.PassBookCreateReqType();
        msgReq.AppHdr = appHdr;
        msgReq.ChnlId = Config.ChannelID;
        msgReq.ItfId = Config.InterfaceID;
        msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
        msgReq.RefNo = refno;
        msgReq.AccNo = debit_account;
        msgReq.Ccy = Decimal.Parse(ccy);
        msgReq.TideNo = string.Empty;
        msgReq.TenureUnit = unit_tenure;
        msgReq.Tenure = Convert.ToDecimal(tenure);
        msgReq.Amt = Convert.ToDecimal(amount);
        msgReq.ProdCd = Convert.ToDecimal(prod_cd);
        msgReq.DepType = dep_type;
        msgReq.PrinOnMat = prin_on_mat; //not need
        msgReq.IntOnMat = int_on_mat;//not need
        msgReq.PosCode = pos_code;
        msgReq.AddnlField = "*";

        //portypeClient
        try
        {
            appHdr.Signature = Funcs.encryptMD5(msgReq.AccNo + msgReq.Amt + Config.SharedKeyMD5).ToUpper();

            PassBook.PortTypeClient ptc = new PassBook.PortTypeClient();
            res = ptc.Create(msgReq);
            Funcs.WriteLog("custid:" + custid + "|postTideBookingToCore|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CustID:" + custid + "|postTideBookingToCore exception:" + ex.ToString());
        }


        //NEU GAP LOI CHUNG CHUNG
        if (res == null ||
            res.RespSts == null ||
            res.RespSts.Sts == null ||
            !res.RespSts.Sts.Equals("0")
            || !res.ResultCode.Equals("00000")
            )
        {
            core_txdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
  
            return Config.gResult_INTELLECT_Arr[1];
        }


        //Hach toan thanh cong
        if (res.RespSts.Sts.Equals("0"))
        {
            core_txdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            //string TxnDate = res.TranTime; //coreResult.Tables[0].Rows[0]["res_Tran_Time"].ToString();
            string TxnDate = res.TranTime;
            res_Legacy_Tide_No = res.LegacyTideNo; // ds.Tables[0].Rows[0]["res_Legacy_Tide_No"].ToString();
            string ref_No_Core = res.RefNo; //ds.Tables[0].Rows[0]["res_Ref_No"].ToString();
            string mat_date = res.MatDt;// ds.Tables[0].Rows[0]["res_Mat_Dt"].ToString();
            res_Mat_Dt = mat_date.Substring(6, 2) + "/" + mat_date.Substring(4, 2) + "/" + mat_date.Substring(0, 4);
            res_val_dt = TxnDate.Substring(6, 2) + "/" + TxnDate.Substring(4, 2) + "/" + TxnDate.Substring(0, 4);

            res_Int_Amt_On_Mat = res.IntAmtOnMat.ToString();// ds.Tables[0].Rows[0]["res_Int_Amt_On_Mat"].ToString();

            res_Int_Rate = res.IntRate.ToString();// ds.Tables[0].Rows[0]["res_Int_Rate"].ToString();

            //core_txno_ref = ref_No_Core;
            //linhtn fix 2016 12 30

            core_txno_ref = res.RefNo; //ds.Tables[0].Rows[0]["res_Ref_No"].ToString();
            core_txdate = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

            return Config.gResult_INTELLECT_Arr[0];
        }
        //hach toan khong thanh cong, lay duoc ra ma loi
        else 
        {
            //neu co loi cu the thi tra ve ma loi cu the, thong bao cho khach hang
            for (int i = 2; i < Config.gResult_INTELLECT_Arr.Length; i++)
            {
                if (Config.gResult_INTELLECT_Arr[i].Split('|')[0].Equals(res.ResultCode))
                {
                    return Config.gResult_INTELLECT_Arr[i];
                }
            }

            //neu loi ngoai bang ma loi --> quy ve loi chung chung
            return Config.gResult_INTELLECT_Arr[1];
        }

    }
    

    #region "OLD TIDEWDL"

    //    /*
    //    Message Keys	Mandatory / Optional	Key Type	Default Value	Remarks
    //hdr_Tran_Id	M	String	TIDEWDL	Transaction Identifier
    //hdr_Status	M	String	NULL	Header Identifier
    //req_Interface_Id	M	String		Interface Identifier
    //req_Chnl_Id	M	String		Channel Identifier
    //req_Txn_Dt	M	String		Transaction Date
    //req_Ref_No	M	String		Reference Number
    //req_Acc_No	M	String		CASA Account Number for Crediting deposit Amount
    //req_Deposit_No	M	Number		Deposit Number [BaseNo||Serial No||Renewal No]
    //req_Wdl_Type	M	String	F	Full Withdrawal



    //Message Keys	Mandatory / Optional	Key Type	Default Value	Remarks
    //res_Ref_No	M	String		Reference Number
    //res_Deposit_No	M	String		Deposit Number
    //res_Txn_Ref_No	M	String		Transaction Reference Number
    //res_Amt_Withdrawn	M	String		Closure Amount credited to CASA

    //    */

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="tran_id"></param>
    //    /// <param name="refno"></param>
    //    /// <param name="des_acctno">tài khoản CASA nhận tiền</param>
    //    /// <param name="deposit_no">số tài khoản tiết kiệm</param>
    //    /// <param name="wdl_type"></param>
    //    /// <param name="core_txno_ref"></param>
    //    /// <param name="interest_amount_received">Closure Amount credited to CASA</param>
    //    /// <returns></returns>
    //    public string postTideWDLToCore(
    //        double tran_id
    //        // , string refno
    //        , string des_acctno
    //        , string deposit_no
    //        , string wdl_type
    //        , ref string core_txno_ref
    //        , ref string core_txdate_ref
    //        , ref double res_Amt_Withdrawn
    //        )
    //    {
    //        mobileGW.SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
    //        myWS.Timeout = Config.TIMEOUT_WITH_CORE;
    //        Transfers tf = new Transfers();
    //        string retws = "";
    //        string refno = "";
    //        try
    //        {

    //            refno = Config.refFormat + tran_id.ToString().PadLeft(9, '0');
    //            string retStr =
    //                myWS.TIDEWDL(Config.InterfaceID, Config.ChannelID, refno, des_acctno, deposit_no, wdl_type, out retws);


    //            DataSet ds = Funcs.Node2Ds(retStr, "HEADER");

    //            if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
    //                == Config.gResult_INTELLECT_Arr[0].Split('|')[0])
    //            {
    //                //string TxnDate = ds.Tables[0].Rows[0]["res_Tran_Time"].ToString();
    //                //string TxnDate = ds.Tables[0].Rows[0]["res_Tran_Time"].ToString();
    //                string TxnDate = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

    //                string ref_No_Core = ds.Tables[0].Rows[0]["res_Ref_No"].ToString();

    //                // interest_amount_received = double.Parse(ds.Tables[0].Rows[0]["res_Ref_No"].ToString());

    //                core_txno_ref = ref_No_Core;
    //                res_Amt_Withdrawn = double.Parse(ds.Tables[0].Rows[0]["res_Amt_Withdrawn"].ToString());
    //                core_txdate_ref = TxnDate;

    //                // Success update to DB                    
    //                //objTran.updateEB_TRAN(TRANID, Config.TX_STATUS_DONE, "*", "*",
    //                //    ds.Tables[0].Rows[0]["res_Ref_No"].ToString(), TxnDate, "*", ds.Tables[0].Rows[0]["res_Result_Code"].ToString(), "*", ref_No_Core, "*", "*", "*", "*", "*");

    //                tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno_ref, TxnDate, Config.ChannelID);
    //                myWS.Dispose();
    //                ds.Dispose();
    //                myWS = null;
    //                return Config.gResult_INTELLECT_Arr[0];
    //            }
    //            else
    //            {
    //                // Update la bi loi
    //                tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno_ref, "", Config.ChannelID);


    //                //objTran.updateEB_TRAN(TRANID, Config.TX_STATUS_FAIL, "*", "*",
    //                //    "*", "*", "*", ds.Tables[0].Rows[0]["res_Result_Code"].ToString(), "*", "*", "*", "*", "*", "*", "*");

    //                myWS.Dispose();
    //                ds.Dispose();
    //                myWS = null;

    //                // Return the error
    //                for (int i = 2; i < Config.gResult_INTELLECT_Arr.Length; i++)
    //                {
    //                    if (Config.gResult_INTELLECT_Arr[i].Split('|')[0] == ds.Tables[0].Rows[0]["res_Result_Code"].ToString())
    //                    {
    //                        return Config.gResult_INTELLECT_Arr[i];
    //                    }
    //                }
    //                return Config.gResult_INTELLECT_Arr[1];
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Funcs.WriteLog(ex.ToString());
    //            //RES_CORE = "";
    //            //// Update la bi loi
    //            //objTran.updateEB_TRAN(TRANID, Config.TX_STATUS_FAIL, "*", "*",
    //            //    "*", "*", "*", Config.TIMEOUTCORE, "*", "*", "*", "*", "*", "*", "*");
    //            //return Config.TIMEOUTCORE;
    //            tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno_ref, "", Config.ChannelID);
    //            //return Config.TIMEOUTCORE;

    //            //return Config.gResult_INTELLECT_Arr[1];

    //            //TIMEOUT 
    //            return Config.ERR_CODE_TIMEOUT_WHEN_POST_TO_CORE;
    //        }
    //        finally
    //        {
    //            tf = null;
    //        }
    //    }


    #endregion "OLD TIDEWDL"

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tran_id"></param>
    /// <param name="refno"></param>
    /// <param name="des_acctno">tài khoản CASA nhận tiền</param>
    /// <param name="deposit_no">số tài khoản tiết kiệm</param>
    /// <param name="wdl_type"></param>
    /// <param name="core_txno_ref"></param>
    /// <param name="interest_amount_received">Closure Amount credited to CASA</param>
    /// <returns></returns>
    public string postTideWDLToCore(
        string custId
        , double tran_id
        // , string refno
        , string des_acctno
        , string deposit_no
        , string wdl_type
        , ref string core_txno_ref
        , ref string core_txdate_ref
        , ref double res_Amt_Withdrawn
        )
    {
        //mobileGW.SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
        //myWS.Timeout = Config.TIMEOUT_WITH_CORE;
        Transfers tf = new Transfers();
        string retws = "";
        string refno = "";
        refno = Config.refFormat + tran_id.ToString().PadLeft(9, '0');
        string result = "";

        PassBook.PassBookClosureResType res = null;
        PassBook.AppHdrType appHdr = new PassBook.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        PassBook.PairsType nsFrom = new PassBook.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        PassBook.PairsType nsTo = new PassBook.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        PassBook.PairsType[] listOfNsTo = new PassBook.PairsType[1];
        listOfNsTo[0] = nsTo;

        PassBook.PairsType BizSvc = new PassBook.PairsType();
        BizSvc.Id = "PassBookWithdrawal";
        BizSvc.Name = "PassBookWithdrawal";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        PassBook.PassBookClosureReqType msgReq = new PassBook.PassBookClosureReqType();
        msgReq.AppHdr = appHdr;
        msgReq.ChnlId = Config.ChannelID;
        msgReq.ItfId = Config.InterfaceID;
        msgReq.DepositNo = deposit_no;
        msgReq.AccNo = des_acctno;
        msgReq.RefNo = refno;
        msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
        msgReq.WdlType = wdl_type;
        
        try
        {
            appHdr.Signature = Funcs.encryptMD5(msgReq.AccNo + msgReq.DepositNo + Config.SharedKeyMD5).ToUpper();

            PassBook.PortTypeClient ptc = new PassBook.PortTypeClient();
            res = ptc.Closure(msgReq);

            Funcs.WriteLog("custid:" + custId + "|postTideWDLToCore|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("custid:" + custId + "|postTideWDLToCore Exception|" + e.ToString());
        }

        if (res == null ||
            res.RespSts == null ||
            res.RespSts.Sts == null || !res.RespSts.Sts.Equals("0"))
        {

            result = Config.gResult_INTELLECT_Arr[1];
            try
            {
                for (int i = 0; i < Config.gResult_INTELLECT_Arr.Length; i++)
                    if (Config.gResult_INTELLECT_Arr[i].Split('|')[0].Equals(res.RespSts.ErrCd))
                    {
                        result = Config.gResult_INTELLECT_Arr[i];

                    }
            }
            catch (Exception e)
            {

                // Helper.WriteLog(l4NC, e.Message + e.StackTrace, custid);
                result = Config.gResult_INTELLECT_Arr[1];

            }
            return result;

        }

        //Hach toan thanh cong
        if (res.RespSts.Sts.Equals("0"))
        {

            //string TxnDate = coreResult.Tables[0].Rows[0]["res_Tran_Time"].ToString();
            string TxnDate = res.TranTime;
            //row["res_Amt_Withdrawn"] = res.AmtWithdrawn;
            res_Amt_Withdrawn = double.Parse( res.AmtWithdrawn.ToString());

            core_txno_ref = res.RefNo;
            core_txdate_ref = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

            result = Config.gResult_INTELLECT_Arr[0];
            //lay them mot so tham so out o day
            return result;
        }
        else
        {
            //neu co loi cu the thi tra ve ma loi cu the, thong bao cho khach hang
            for (int i = 2; i < Config.gResult_INTELLECT_Arr.Length; i++)
            {
                if (Config.gResult_INTELLECT_Arr[i].Split('|')[0].Equals(res.ResultCode))
                {
                    return Config.gResult_INTELLECT_Arr[i];
                }
            }

            //neu loi ngoai bang ma loi --> quy ve loi chung chung
            return Config.gResult_INTELLECT_Arr[1];
        }
        //return res;
    }

    public string postFlexTideWDLToCore(
        string custId
        , double tran_id
        // , string refno
        , string des_acctno
        , string deposit_no
        , string wdl_type
        , double amount
        , ref string core_txno_ref
        , ref string core_txdate_ref
        , ref string parentAcctNo
        , ref double currPrinAmt
        , ref double currMatAmt
        , ref double intAmount
        , ref double tenure
        , ref string unitTenure
        , ref string unitTenureEn
        , ref string unitTenureVn
        , ref string numOfChildTideSuccess
        )
    {
        //mobileGW.SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
        //myWS.Timeout = Config.TIMEOUT_WITH_CORE;
        //Transfers tf = new Transfers();

        string retws = "";
        string refno = "";
        refno = Config.refFormat + tran_id.ToString().PadLeft(9, '0');
        string result = "";

        FlexTide.WithdrawalTideResType res = null;

        FlexTide.AppHdrType appHdr = new FlexTide.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        FlexTide.PairsType nsFrom = new FlexTide.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        FlexTide.PairsType nsTo = new FlexTide.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        FlexTide.PairsType[] listOfNsTo = new FlexTide.PairsType[1];
        listOfNsTo[0] = nsTo;

        FlexTide.PairsType BizSvc = new FlexTide.PairsType();
        BizSvc.Id = "WithdrawalTide";
        BizSvc.Name = "WithdrawalTide";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        FlexTide.WithdrawalTideReqType msgReq = new FlexTide.WithdrawalTideReqType();
        msgReq.AppHdr = appHdr;
        msgReq.ChnlId = Config.ChannelID;
        msgReq.ItfId = Config.InterfaceID;
        msgReq.AccNo = des_acctno;
        msgReq.DepositNo = deposit_no;
        msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
        msgReq.WdlType = wdl_type;
        msgReq.Amount = amount.ToString();
        msgReq.CustId = custId;

        try
        {
            appHdr.Signature = Funcs.encryptMD5(msgReq.AccNo + msgReq.DepositNo + Config.SharedKeyMD5).ToUpper();

            FlexTide.PortTypeClient ptc = new FlexTide.PortTypeClient();
            res = ptc.WithdrawalTide(msgReq);

            Funcs.WriteLog("custid:" + custId + "|postFlexTideWDLToCore|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception e)
        {
            Funcs.WriteLog("custid:" + custId + "|postFlexTideWDLToCore Exception|" + e.ToString());
        }

        if (res == null ||
            res.RespSts == null ||
            res.RespSts.Sts == null)
        {

            result = Config.gResult_Flex_Tide_Booking_Arr[1];
            return result;

        }

        //Hach toan thanh cong
        if (res.RespSts.Sts.Equals("0"))
        {

            //string TxnDate = coreResult.Tables[0].Rows[0]["res_Tran_Time"].ToString();
            //string TxnDate = res.TranTime;
            //row["res_Amt_Withdrawn"] = res.AmtWithdrawn;
            //res_Amt_Withdrawn = double.Parse(res.AmtWithdrawn.ToString());

            core_txno_ref = refno;
            core_txdate_ref = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

            result = Config.gResult_Flex_Tide_Booking_Arr[0];

            parentAcctNo = res.parentAcctNo;
            currPrinAmt = Double.Parse(res.currPrinAmt);
            currMatAmt = Double.Parse(res.currMatAmt);
            intAmount = Double.Parse(res.intAmount);
            tenure = Double.Parse(res.tenure);
            unitTenure = res.unitTenure;
            unitTenureEn = res.unitTenureEn;
            unitTenureVn = res.unitTenureVn;
            numOfChildTideSuccess = res.numOfChildTideSuccess;

            //lay them mot so tham so out o day
            return result;
        }
        else
        {
            //neu co loi cu the thi tra ve ma loi cu the, thong bao cho khach hang
            for (int i = 2; i < Config.gResult_Flex_Tide_Booking_Arr.Length; i++)
            {
                if (Config.gResult_Flex_Tide_Booking_Arr[i].Split('|')[0].Equals(res.errCode))
                {
                    return Config.gResult_Flex_Tide_Booking_Arr[i];
                }
            }

            //neu loi ngoai bang ma loi --> quy ve loi chung chung
            return Config.gResult_Flex_Tide_Booking_Arr[1];
        }
        //return res;
    }



    //LINHTN: FIX LAY POS MO SO TIET KIEM THEO POS MO TAI KHOAN
    // public DataTable GET_POS_BY_CIF(string custid)
    public DataTable GET_POS_BY_CIF_ACCT(string custid, string acctno)

    {
        // IN VARCHAR2,  in varchar2,  in varchar2
        try
        {
            DataSet ds = new DataSet();
            //dsCmd = new OracleCommand("pkg_common_core.GET_POS_BY_CIF", new OracleConnection(Config.gINTELLECTConnstr));

            dsCmd = new OracleCommand("pkg_common_core.GET_POS_BY_CIF_ACCT", new OracleConnection(Config.gINTELLECTConnstr));

            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);

            dsCmd.Parameters.Add("OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }


    #endregion "TIDE ONLINE"

    #region "SETTING"

    public DataSet GET_SETTING(string custid, string channel_id)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_CATEGORY_BY_TRAN_TYPE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("channel_id", OracleDbType.Varchar2, channel_id, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }

    public DataSet SET_SETTING_AVATAR(string custid, string channel_id, byte[] avatar)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.SET_SETTING_AVATAR", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("channel_id", OracleDbType.Varchar2, channel_id, ParameterDirection.Input);

            dsCmd.Parameters.Add("AVATAR", OracleDbType.Blob, avatar, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }

    /*
     SET_ACCT_DEFAULT: Set tài khoản mặc định
SET_FAV_MENU: Set favorite menu
SET_LANG: Set ngôn ngữ mặc  định
SET_AVATAR: Set hình ảnh đại diện

     */
    public DataSet SET_SETTING_OTHER(string custid, string channel_id, string type, string val)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.SET_SETTING_OTHER", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("channel_id", OracleDbType.Varchar2, channel_id, ParameterDirection.Input);

            dsCmd.Parameters.Add("type", OracleDbType.Varchar2, type, ParameterDirection.Input);
            dsCmd.Parameters.Add("val", OracleDbType.Varchar2, val, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }

    #endregion "SETTING"

    #region "INBOX OUTBOX FOR CUST"

    /// <summary>
    /// lấy top 10 
    /// </summary>
    /// <param name="custid"></param>
    /// <param name="mail_type">inbox, outbox</param>
    /// <returns></returns>
    public DataSet GET_TOP10_MAIL(string custid, string mail_type)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_COMMON_NEW.GET_TOP10_MAIL", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("channel_id", OracleDbType.Varchar2, mail_type, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }

    public DataSet SEND_TO_OUTBOX(string custid, string subject, string content)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_COMMON_NEW.SEND_TO_OUTBOX", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("subject", OracleDbType.Varchar2, subject, ParameterDirection.Input);
            dsCmd.Parameters.Add("content", OracleDbType.Varchar2, content, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }

    /// <summary>
    /// Xoa inbox, outbox
    /// </summary>
    /// <param name="custid"></param>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public DataSet DELELE_MAIL(string custid, string type, double id)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_COMMON_NEW.DELELE_MAIL", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("subject", OracleDbType.Varchar2, type, ParameterDirection.Input);
            dsCmd.Parameters.Add("id", OracleDbType.Int16, id, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }
    #endregion "INBOX OUTBOX FOR CUST"

    #region "GIOI THIEU BAN BE"

   //    PROCEDURE  REF_INVITE (      
   //pcustid IN VARCHAR2
   //,PREF_INVITE IN VARCHAR2   
   //, OUT_CUR OUT REF_CUR
   // ) 

    public DataSet UPDATE_REF_INVITE(string custid, string pref_invite, string channel_id)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_COMMON_NEW.UPDATE_REF_INVITE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("pref_invite", OracleDbType.Varchar2, pref_invite, ParameterDirection.Input);
            dsCmd.Parameters.Add("channel_id", OracleDbType.Varchar2, channel_id, ParameterDirection.Input);
             dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }

    public DataSet CHECK_REF_INVITE(string custid, string channel_id)
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_COMMON_NEW.CHECK_REF_INVITE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("channel_id", OracleDbType.Varchar2, channel_id, ParameterDirection.Input);
            dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }

    }

    #endregion "GIOI THIEU BAN BE"


    #region "SEARCH LOCATION BRANCH ATM"

    
    public DataSet GET_LOCATION_LIST(string type, string Latitude, string Longitude)
    {
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_COMMON_NEW.GET_LOCATION_LIST", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(LOCATION.LOCATION_TYPE, OracleDbType.Varchar2, type, ParameterDirection.Input);
            dsCmd.Parameters.Add(LOCATION.LATITUDE, OracleDbType.Varchar2, Latitude, ParameterDirection.Input);
            dsCmd.Parameters.Add(LOCATION.LONGITUDE, OracleDbType.Varchar2, Longitude, ParameterDirection.Input);
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }

    #endregion "SEARCH LOCATION BRANCH ATM"

    #region "FX & TIDE RATE"
    public DataSet GET_FX_RATE( )
    {
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_FX_RATE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }

    public DataSet GET_TIDE_RATE( )
    {
        try
        {
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_TIDE_RATE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }
    #endregion "FX & TIDE RATE"


    /*Linhtn addnew 2016 18 11
    Them log action 
    */

    public bool INS_TBL_EB_ACTION(
            string pCHANNEL_ID
        , string pMOD_CD
        , string pTRAN_TYPE
        , string pCUSTID
        , string pIP
         , string pUSER_AGENT
         , string pSTATUS
         , string pTXDESC
         , string pLAST_CHANGE // default la sysdate
         , string pBM1
         , string pBM2
         , string pBM3
         , string pBM4
         , string pBM5
         , string pBM6
         , string pBM7
         , string pBM8
         , string pBM9
         , string pBM10
         , string pBM11
         , string pBM12
         , string pBM13
         , string pBM14
         , string pBM15
         , string pBM16
         , string pBM17
         , string pBM18
         , string pBM19
         , string pBM20
         , string pBM21
         , string pBM22
         , string pBM23
         , string pBM24
         , string pBM25
         , string pBM26
         , string pBM27
         , string pBM28
         , string pBM29
         , int pIS_PROCESSED  //default = 0
        , string pTIME_PROCESSED  //default = sysdate
 )

    {
        #region "INS_TBL_EB_ACTION"

        Funcs.WriteLog("custid:" + pCUSTID + "|BEGIN INS_TBL_EB_ACTION");
        bool ret = false;

        try
        {

            DataSet ds = new DataSet();

            dsCmd = new OracleCommand( Config.gEBANKSchema +  "pkg_ebank_fraud.INS_TBL_EB_ACTION ", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;

            dsCmd.Parameters.Add("pCHANNEL_ID", OracleDbType.Varchar2, pCHANNEL_ID, ParameterDirection.Input);
            dsCmd.Parameters.Add("pMOD_CD", OracleDbType.Varchar2, pMOD_CD, ParameterDirection.Input);
            dsCmd.Parameters.Add("pTRAN_TYPE", OracleDbType.Varchar2, pTRAN_TYPE, ParameterDirection.Input);
            dsCmd.Parameters.Add("pCUSTID", OracleDbType.Varchar2, pCUSTID, ParameterDirection.Input);
            dsCmd.Parameters.Add("pIP", OracleDbType.Varchar2, pIP, ParameterDirection.Input);
            dsCmd.Parameters.Add("pUSER_AGENT", OracleDbType.Varchar2, pUSER_AGENT, ParameterDirection.Input);
            dsCmd.Parameters.Add("pSTATUS", OracleDbType.Varchar2, pSTATUS, ParameterDirection.Input);
            dsCmd.Parameters.Add("pTXDESC", OracleDbType.Varchar2, pTXDESC, ParameterDirection.Input);
            dsCmd.Parameters.Add("pLAST_CHANGE", OracleDbType.Varchar2, pLAST_CHANGE, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM1", OracleDbType.Varchar2, pBM1, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM2", OracleDbType.Varchar2, pBM2, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM3", OracleDbType.Varchar2, pBM3, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM4", OracleDbType.Varchar2, pBM4, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM5", OracleDbType.Varchar2, pBM5, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM6", OracleDbType.Varchar2, pBM6, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM7", OracleDbType.Varchar2, pBM7, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM8", OracleDbType.Varchar2, pBM8, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM9", OracleDbType.Varchar2, pBM9, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM10", OracleDbType.Varchar2, pBM10, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM11", OracleDbType.Varchar2, pBM11, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM12", OracleDbType.Varchar2, pBM12, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM13", OracleDbType.Varchar2, pBM13, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM14", OracleDbType.Varchar2, pBM14, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM15", OracleDbType.Varchar2, pBM15, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM16", OracleDbType.Varchar2, pBM16, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM17", OracleDbType.Varchar2, pBM17, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM18", OracleDbType.Varchar2, pBM18, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM19", OracleDbType.Varchar2, pBM19, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM20", OracleDbType.Varchar2, pBM20, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM21", OracleDbType.Varchar2, pBM21, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM22", OracleDbType.Varchar2, pBM22, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM23", OracleDbType.Varchar2, pBM23, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM24", OracleDbType.Varchar2, pBM24, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM25", OracleDbType.Varchar2, pBM25, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM26", OracleDbType.Varchar2, pBM26, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM27", OracleDbType.Varchar2, pBM27, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM28", OracleDbType.Varchar2, pBM28, ParameterDirection.Input);
            dsCmd.Parameters.Add("pBM29", OracleDbType.Varchar2, pBM29, ParameterDirection.Input);
            dsCmd.Parameters.Add("pIS_PROCESSED", OracleDbType.Int16, pIS_PROCESSED, ParameterDirection.Input);
            dsCmd.Parameters.Add("pTIME_PROCESSED", OracleDbType.Varchar2, pTIME_PROCESSED, ParameterDirection.Input);

            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);


            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ret =  true;
            }
            else
            {   
                ret = false;
            }

        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            ret = false;
        }


        Funcs.WriteLog("custid:" + pCUSTID + "|END INS_TBL_EB_ACTION");
        return ret;

        #endregion "INS_TBL_EB_ACTION"
    }


    /*REGION INSERT SMS*/

    #region "INSERT SMS"
    public bool INSERT_SMS(
            Double v_id,
            String v_service,
            String v_pos_cd,
            String v_user_id,
            String v_info,
            Double v_request_id,
            String v_provider_id,
            String v_receive_time,
            String v_send_time,
            Int16 v_total_msg,
            Int16 v_msg_idx,
            Int16 v_is_more
    )
    {
        try
        {
            DataSet ds = new DataSet();

            //PROCEDURE INSERTNEWSMS(v_id IN LONG, v_service_id IN VARCHAR2, v_user_id IN VARCHAR2, v_info IN VARCHAR2, 
            //v_request_id IN LONG, v_provider_id IN VARCHAR2, v_receive_time IN VARCHAR2, v_send_time IN VARCHAR2
            //, v_total_msg IN LONG, v_msg_idx IN LONG, v_is_more IN LONG) IS


                      dsCmd = new OracleCommand(Config.gSMSUSERSchema + "SHBMOBI.INSERTNEWSMS", new OracleConnection(Config.gMOBILConnstr));
            dsCmd.Parameters.Add( "ID", OracleDbType.Int64, v_id, ParameterDirection.Input );
            dsCmd.Parameters.Add( "SERVICE_ID", OracleDbType.Varchar2, v_service, ParameterDirection.Input);
            dsCmd.Parameters.Add( "USER_ID", OracleDbType.Varchar2, v_user_id, ParameterDirection.Input);
            dsCmd.Parameters.Add( "INFO", OracleDbType.Varchar2, v_info, ParameterDirection.Input);
            dsCmd.Parameters.Add("REQUEST_ID", OracleDbType.Int64, v_request_id, ParameterDirection.Input);
            dsCmd.Parameters.Add("PROVIDER_ID", OracleDbType.Varchar2, v_provider_id, ParameterDirection.Input);
            dsCmd.Parameters.Add("RECEIVE_TIME", OracleDbType.Varchar2, v_receive_time, ParameterDirection.Input);
            dsCmd.Parameters.Add("SEND_TIME", OracleDbType.Varchar2, v_send_time,  ParameterDirection.Input);
            dsCmd.Parameters.Add("TOTAL_MSG", OracleDbType.Int16, v_total_msg,   ParameterDirection.Input);
            dsCmd.Parameters.Add("MSG_IDX", OracleDbType.Int16,v_msg_idx,  ParameterDirection.Input);
            dsCmd.Parameters.Add("IS_MORE_FIELD", OracleDbType.Int16, v_is_more, ParameterDirection.Input);

            //dsCmd.Parameters.Add("V_OUT_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;

            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            
            insert_mt_queue_his(v_id,
            v_service,
            "0" + v_user_id.Substring(2),
            v_info,
            v_request_id,
            v_provider_id,
            v_receive_time,
            v_send_time,
            v_total_msg,
            v_msg_idx,
            v_is_more,
            v_pos_cd,
            //"110000",
            "IBANKACT", "8", "", "", "", "", "", "", "", "", "", "");

            return true;

           // return ds.Tables[0];

        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return false;

        }
    }

    public bool insert_mt_queue_his(
          Double v_id,
          String v_service,
          String v_user_id,
          String v_info,
          Double v_request_id,
          String v_provider_id,
          String v_receive_time,
          String v_send_time,
          Int16 v_total_msg,
          Int16 v_msg_idx,
          Int16 v_is_more,
          String v_branch_code,
          String v_channel_id,
          String v_interface_id,
          String v_char1,
          String v_char2,
          String v_char3,
          String v_char4,
          String v_char5,
          String v_char6,
          String v_char7,
          String v_char8,
          String v_char9,
          String v_char10)
    {

        DataSet ds = new DataSet();

        dsCmd = new OracleCommand(Config.gSMSUSERSchema + "SHBMOBI.INSERT_MT_QUEUE_HIS", new OracleConnection(Config.gMOBILConnstr));
        dsCmd.Parameters.Add("ID", OracleDbType.Int64, v_id, ParameterDirection.Input);
        dsCmd.Parameters.Add("SERVICE_ID", OracleDbType.Varchar2, v_service, ParameterDirection.Input);
        dsCmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, v_user_id, ParameterDirection.Input);
        dsCmd.Parameters.Add("INFO", OracleDbType.Varchar2, v_info, ParameterDirection.Input);
        dsCmd.Parameters.Add("REQUEST_ID", OracleDbType.Int64, v_request_id, ParameterDirection.Input);
        dsCmd.Parameters.Add("PROVIDER_ID", OracleDbType.Varchar2, v_provider_id, ParameterDirection.Input);
        dsCmd.Parameters.Add("RECEIVE_TIME", OracleDbType.Varchar2, v_receive_time, ParameterDirection.Input);
        dsCmd.Parameters.Add("SEND_TIME", OracleDbType.Varchar2, v_send_time, ParameterDirection.Input);
        dsCmd.Parameters.Add("TOTAL_MSG", OracleDbType.Int16, v_total_msg, ParameterDirection.Input);
        dsCmd.Parameters.Add("MSG_IDX", OracleDbType.Int16, v_msg_idx, ParameterDirection.Input);
        dsCmd.Parameters.Add("IS_MORE", OracleDbType.Int16, v_is_more, ParameterDirection.Input);

        dsCmd.Parameters.Add("BRANCH_CODE", OracleDbType.Varchar2, v_branch_code, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHANNEL_ID", OracleDbType.Varchar2, v_channel_id, ParameterDirection.Input);
        dsCmd.Parameters.Add("INTERFACE_ID", OracleDbType.Varchar2, v_interface_id, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR1", OracleDbType.Varchar2, v_char1, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR2", OracleDbType.Varchar2, v_char2, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR3", OracleDbType.Varchar2, v_char3, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR4", OracleDbType.Varchar2, v_char4, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR5", OracleDbType.Varchar2, v_char5, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR6", OracleDbType.Varchar2, v_char6, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR7", OracleDbType.Varchar2, v_char7, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR8", OracleDbType.Varchar2, v_char8, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR9", OracleDbType.Varchar2, v_char9, ParameterDirection.Input);
        dsCmd.Parameters.Add("CHAR10", OracleDbType.Varchar2, v_char10, ParameterDirection.Input);

        dsCmd.CommandType = CommandType.StoredProcedure;

        dsApt.SelectCommand = dsCmd;
        dsApt.Fill(ds);
        return true; 
       
    }

    #endregion "INSERT SMS"


    #region Rut Goc Linh Hoat
    public string postFlexTideBookingToCore(
        string custid //them tham so nay de ghi log
        , double tran_id
        // , string refno // REFNO
        , string debit_account // ACCTNO
        , string ccy // CCY,// string TIDE_NO, 
        , string unit_tenure // TENURE_UNIT
        , double tenure //TENURE
        , double amount //AMOUNT
        , string prod_cd //PROD_CD
        , string dep_type// DEP_TYPE
        , string prin_on_mat //PRIN_ON_MAT
        , string int_on_mat //INT_ON_MAT
        , string addnl_field // ADDNL_FIELD
        , string pos_code // POS_CODE
        , string numOfChild
        , string requestDate
        , string requestId
        , ref string core_txno_ref
        , ref string core_txdate
        , ref string resTransId
        , ref string depositNoParentTide
        , ref string acccountNoParentTide
        , ref string numOfParentTide
        , ref string numOfChildTideSuccess
        , ref string valDate
        , ref string matDate
        , ref string tenureUnit
        , ref string orgAmountChild
        , ref string interestAmountChild
        , ref string interestAmountParent
        , ref string tenureRes
        )
    {


        Transfers tf = new Transfers();
        string retws = "";
        core_txno_ref = "";
        //res_Legacy_Tide_No = "";
        //res_Mat_Dt = "";
        //res_Int_Amt_On_Mat = "";
        string refno = "";

        refno = Config.refFormat + tran_id.ToString().PadLeft(9, '0');


        FlexTide.BookingTideResType res = null;

        FlexTide.AppHdrType appHdr = new FlexTide.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        FlexTide.PairsType nsFrom = new FlexTide.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        FlexTide.PairsType nsTo = new FlexTide.PairsType();
        nsTo.Id = "CORE";
        nsTo.Name = "CORE";

        FlexTide.PairsType[] listOfNsTo = new FlexTide.PairsType[1];
        listOfNsTo[0] = nsTo;

        FlexTide.PairsType BizSvc = new FlexTide.PairsType();
        BizSvc.Id = "PassBookCreate";
        BizSvc.Name = "PassBookCreate";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;

        //Body
        FlexTide.BookingTideReqType msgReq = new FlexTide.BookingTideReqType();
        msgReq.AppHdr = appHdr;
        msgReq.accNo = debit_account;
        msgReq.additionalField = "*";
        msgReq.amount = Convert.ToString(amount);
        msgReq.ccycd = ccy;
        msgReq.channelId = Config.ChannelID;
        msgReq.custId = custid;
        msgReq.depType = dep_type;
        msgReq.intOnMat = int_on_mat;
        msgReq.itfld = Config.InterfaceID;
        msgReq.numOfChild = numOfChild;
        msgReq.posCode = pos_code;
        msgReq.printOnMat = prin_on_mat;
        msgReq.prodCd = prod_cd;
        msgReq.requestDate = requestDate;
        msgReq.requestId = requestId;
        msgReq.terune = Convert.ToString(tenure);
        msgReq.txnDt = DateTime.Now.ToString("yyyyMMdd");
        msgReq.unitTerune = unit_tenure;

        //portypeClient
        try
        {
            appHdr.Signature = Funcs.encryptMD5(msgReq.accNo + msgReq.amount + Config.SharedKeyMD5).ToUpper();

            FlexTide.PortTypeClient ptc = new FlexTide.PortTypeClient();
            res = ptc.BookingTide(msgReq);
            Funcs.WriteLog("custid:" + custid + "|postFlexTideBookingToCore|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CustID:" + custid + "|postFlexTideBookingToCore exception:" + ex.ToString());
        }


        //NEU GAP LOI CHUNG CHUNG
        if (res == null ||
            res.RespSts == null ||
            res.RespSts.Sts == null ||
            !res.RespSts.Sts.Equals("0")
            || !res.errCode.Equals("00")
            )
        {
            core_txdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            return Config.gResult_Flex_Tide_Booking_Arr[1];
        }


        //Hach toan thanh cong
        if (res.RespSts.Sts.Equals("0"))
        {
            core_txdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            ////string TxnDate = res.TranTime; //coreResult.Tables[0].Rows[0]["res_Tran_Time"].ToString();
            //string TxnDate = res.valDate;
            
            //res_Legacy_Tide_No = res.le; // ds.Tables[0].Rows[0]["res_Legacy_Tide_No"].ToString();
            //string ref_No_Core = res.RefNo; //ds.Tables[0].Rows[0]["res_Ref_No"].ToString();
            //string mat_date = res.MatDt;// ds.Tables[0].Rows[0]["res_Mat_Dt"].ToString();
            //res_Mat_Dt = mat_date.Substring(6, 2) + "/" + mat_date.Substring(4, 2) + "/" + mat_date.Substring(0, 4);
            //res_val_dt = TxnDate.Substring(6, 2) + "/" + TxnDate.Substring(4, 2) + "/" + TxnDate.Substring(0, 4);

            //res_Int_Amt_On_Mat = res.IntAmtOnMat.ToString();// ds.Tables[0].Rows[0]["res_Int_Amt_On_Mat"].ToString();

            //res_Int_Rate = res.IntRate.ToString();// ds.Tables[0].Rows[0]["res_Int_Rate"].ToString();

            ////core_txno_ref = ref_No_Core;
            ////linhtn fix 2016 12 30

            core_txno_ref = refno; //ds.Tables[0].Rows[0]["res_Ref_No"].ToString();
            core_txdate = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

            resTransId = res.resTransId;
            depositNoParentTide = res.depositNoParentTide;
            acccountNoParentTide = res.acccountNoParentTide;
            numOfParentTide = res.numOfParentTide;
            numOfChildTideSuccess = res.numOfChildTideSuccess;
            valDate = res.valDate.Substring(6, 2) + "/" + res.valDate.Substring(4, 2) + "/" + res.valDate.Substring(0, 4);
            matDate = res.matDate.Substring(6, 2) + "/" + res.matDate.Substring(4, 2) + "/" + res.matDate.Substring(0, 4);
            tenureRes = res.tenure;
            tenureUnit = res.tenureUnit;
            orgAmountChild = res.orgAmountChild;
            interestAmountChild = res.interestAmountChild;
            interestAmountParent = res.interestAmountParent;

            return Config.gResult_Flex_Tide_Booking_Arr[0];
        }
        //hach toan khong thanh cong, lay duoc ra ma loi
        else
        {
            //neu co loi cu the thi tra ve ma loi cu the, thong bao cho khach hang
            for (int i = 2; i < Config.gResult_Flex_Tide_Booking_Arr.Length; i++)
            {
                if (Config.gResult_Flex_Tide_Booking_Arr[i].Split('|')[0].Equals(res.errCode))
                {
                    return Config.gResult_Flex_Tide_Booking_Arr[i];
                }
            }

            //neu loi ngoai bang ma loi --> quy ve loi chung chung
            return Config.gResult_Flex_Tide_Booking_Arr[1];
        }

    }
    #endregion

    #region GET CORE DATE
    public DataSet getCoreDate()
    {
        try
        {

            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "pkg_ebank_util.GET_BUSDATE_BR", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.Parameters.Add("V_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);

            return ds;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog(ex.ToString());
            return null;
        }
    }
    #endregion

}
