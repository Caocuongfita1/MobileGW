using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;


namespace mobileGW.Service.DataAccess
{

    /// <summary>
    /// Summary description for TBL_EB_USER_CHANNELs
    /// </summary>
    public class Enquirys
    {
        //public Enquirys()
        //{
        //    //
        //    // TODO: Add constructor logic here
        //    //
        //}

        //public class Ẹnquirys : IDisposable
        //{


        //}

        //Begin edit
        private OracleCommand dsCmd;
        //end edit
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
        public Enquirys()
        {
            //
            dsApt = new OracleDataAdapter();
            //
        }

        public DataSet GET_ACCT_LIST_HOMESCREEN_N(string custid)
        {
            try { 
             DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_LIST_HOMESCREEN_N ", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add( "custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
            
            } catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        public DataSet GET_ACCT_BALANCE_HOMESCREEN_N(string custid, string acctno)
        {
            try {
                string default_acctno = "ALL";
                default_acctno = GET_MOB_ACCT_DEFAULT(custid, acctno);

                if (default_acctno == "") default_acctno = "ALL";

                DataSet ds = new DataSet();

                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_BALANCE_HOMESCREEN_N", new OracleConnection(Config.gINTELLECTConnstr));

                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("PCUSTID", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("PACCTNO", OracleDbType.Varchar2, default_acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;           
            } catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        //        REQ=CMD#GET_ACCT_BALANCE_HOMESCREEN_N|CIF_NO#0000204244|ACCT#1000013376|TYPE#001|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX;

        //Response

        //public static String GET_ACCT_BALANCE_HOMESCREEN_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        //            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        //            + ROW_DLMT + "
        //" + COL_DLMT + "{CIF_NO
        //    }"
        //			+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
        //			+ ROW_DLMT + "AVAIL_BALANCE" + COL_DLMT + "{AVAIL_BALANCE}"
        //			+ ROW_DLMT + "CURR_BALANCE" + COL_DLMT + "{CURR_BALANCE}";

        public string GET_MOB_ACCT_DEFAULT(string custid, string acctno)
        {

            if (acctno == "ALL")
            {
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_MOB_ACCT_DEFAULT", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("channel_id", OracleDbType.Varchar2, Config.ChannelID, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.DEFAULT_ACCT].ToString();
                }
                else
                {
                    return "";
                }
            }

            else
            {
                return acctno;
            }

        }


        /*
        TRUY VẤN THÔNG TIN TÀI KHOẢN			
-------------------
//truy van thong tin tai khoan
//thong tin de ve bieu do o tren
//thong tin chi tiet ve cac tai khoan		

Request
"REQ=CMD# GET_ACCT_LIST_QRY_N|ACTIVE_CODE#123456|CIF_NO#0310008705|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

	
public static String GET_ACCT_LIST_QRY_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"	
			+ ROW_DLMT + "CASA_TOTAL" + COL_DLMT + "{CASA_TOTAL}"	
			+ ROW_DLMT + "TIDE_TOTAL" + COL_DLMT + "{TIDE_TOTAL}"	
			+ ROW_DLMT + "LOAN_TOTAL" + COL_DLMT + "{LOAN_TOTAL}"			
			+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
			;
			
RECORD LIST CAC TAI KHOAN: LOAI TAI KHOAN| SO TAI KHOAN| LOAI TIEN

public const String prod_cd_CASA = "001";
public const String prod_cd_TIDE = "002";
public const String prod_cd_LENDING = "003";

        */

        /// <summary>
        /// Linhtn update 20160720
        /// chuyển gọi chung package pkg_common_core
        /// </summary>
        /// <param name="custid"></param>
        /// <returns></returns>
        public DataSet GET_ACCT_LIST_QRY_N(string custid)
        {
            try
            {

                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_LIST_QRY_N", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
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

        /*
        Request " REQ=CMD#GET_ACCT_CASA_INFO_N|CIF_NO#0310008705|ACCTNO#1000013376|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
Response
public static String GET_ACCT_CASA_INFO_N = "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
			+ ROW_DLMT + "ACCT_CURR_BALANCE" + COL_DLMT + "{ACCT_CURR_BALANCE}"
			+ ROW_DLMT + "ACCT_AVAI_BALANCE" + COL_DLMT + "{ACCT_AVAI_BALANCE}"
			+ ROW_DLMT + "RECORD_BALANCE_HIS" + COL_DLMT + "{RECORD_BALANCE_HIS}"
			+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
			;	
			
RECORD_BALANCE_HIS: NGAY (DD/MM/YYYY) | BALANCE CUOI NGAY

RECORD_ACTIVITY: Default là trả về 5 giao dịch gần nhất

NGAY (DD/MM/YYYY) | AMOUNT (Định dạng + - số tiền, đã format ở server) | Diễn giải giao dịch (đã remove các ký tự đặc biệt)

12/01/2016^+15.000^chuyen khoan
$
11/01/2016^-10.000^chuyen khoan test
        */

       


        /// <summary>
        /// Lấy số dư 30 ngày gần nhất của TK CASA để vẽ biểu đồ
        /// </summary>
        /// <param name="acctno"></param>
        /// <returns></returns>
        public DataTable CASA_BALANCE_HIST(string cif_no , string acctno)
        {
            try
            {

                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.CASA_BALANCE_HIST", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("cif_no", OracleDbType.Varchar2, cif_no, ParameterDirection.Input);
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add("out_cur1", OracleDbType.RefCursor, ParameterDirection.Output);
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
        Request " REQ=CMD#GET_ACCT_CASA_TRAN__BY_ENQ_TYPE_N|CIF_NO#0310008705|ACCTNO#1000013376|ENQUIRY_TYPE#THIS_MONTH|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
Response
public static String GET_ACCT_CASA_TRAN__BY_ENQ_TYPE_N= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
			+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
			;	
			

Trong đó: 

RECORD_ACTIVITY: Default là trả về 5 giao dịch gần nhất

NGAY (DD/MM/YYYY) | AMOUNT (Định dạng + - số tiền, đã format ở server) | Diễn giải giao dịch (đã remove các ký tự đặc biệt)

 

ENQUIRY_TYPE: 
Giao dịch trong ngày hôm nay: TODAY
Giao dịch trong tháng này: THIS_MONTH
Giao dịch trong tháng trước: LAST_MONTH
Giao dịch tuần này: THIS_WEEK
Giao dịch tuần trước: LAST_WEEK

        */

        public DataTable GET_ACCT_CASA_TRAN_BY_ENQ_TYPE_N(string cif_no, string acctno, string enquiry_type, string fromDate, string toDate)
        {
            try
            {

                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_CASA_LIST_TRAN", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("cif_no", OracleDbType.Varchar2, cif_no, ParameterDirection.Input);
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add("enquiry_type", OracleDbType.Varchar2, enquiry_type, ParameterDirection.Input);
                dsCmd.Parameters.Add("pfromdate", OracleDbType.Varchar2, fromDate, ParameterDirection.Input);
                dsCmd.Parameters.Add("enquiry_type", OracleDbType.Varchar2, toDate, ParameterDirection.Input);

                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);

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




        public DataSet GET_ACCT_LN_INFO_N(string acctno)
        {
            try {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_LN_INFO_N", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add("out_cur1", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.Parameters.Add("out_cur2", OracleDbType.RefCursor, ParameterDirection.Output);
                dsCmd.Parameters.Add("out_cur3", OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;

            }catch  (Exception ex)
            {

                Funcs.WriteLog(ex.ToString());
                return null;
            }
       
        }

        /// <summary>
        /// linhtn: addnew 23 jul 2016
        /// Liệt kê giao dịch tải khoản vay
        /// </summary>
        /// <param name="acctno"></param>
        /// <param name="enquiry_type"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public DataSet GET_ACCT_LN_TRAN_BY_ENQ_TYPE_N(string custid, string acctno, string enquiry_type, string fromDate, string toDate)
        {
            try
            {

                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GETTRAN_ACCTNO_LN", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add("enquiry_type", OracleDbType.Varchar2, enquiry_type, ParameterDirection.Input);
                dsCmd.Parameters.Add("pfromdate", OracleDbType.Varchar2, fromDate, ParameterDirection.Input);
                dsCmd.Parameters.Add("toDate", OracleDbType.Varchar2, toDate, ParameterDirection.Input);

                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);

                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Lấy 05 giao dịch trả nợ sắp tới theo lịch
        /// 
        /// PROCEDURE GET_LIST_PAYMENT_DATE(
//V_CIFNO IN VARCHAR2,
//V_ACCTNO IN VARCHAR2,
//V_ENQ_TYPE IN VARCHAR2, 
//V_FROM_DATE IN VARCHAR2,
//V_TO_DATE IN VARCHAR2
//, MY_CUR OUT REF_CUR)
        /// </summary>
        /// <param name="acctno"></param>
        /// <returns></returns>
        public DataSet listRepaymentSchedule(string custid, string acctno, string enq_type, string pfromdate, string ptodate)
        {
            try
            {

                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_LIST_PAYMENT_DATE", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add("enq_type", OracleDbType.Varchar2, enq_type, ParameterDirection.Input);
                dsCmd.Parameters.Add("pfromdate", OracleDbType.Varchar2, pfromdate, ParameterDirection.Input);
                dsCmd.Parameters.Add("ptodate", OracleDbType.Varchar2, ptodate, ParameterDirection.Input);
               
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



        /*
requestBody = " REQ=CMD#GET_ACCT_TIDE_INFO_N|CIF_NO#0310008705|ACCTNO#1000013376|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
public static String GET_ACCT_TIDE_INFO_N= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
+ ROW_DLMT + "CCYCD" + COL_DLMT + {CCYCD}"
+ ROW_DLMT + "PRODUCT_CODE" + COL_DLMT + {PRODUCT_CODE}"
+ ROW_DLMT + "CURR_PRIN_AMT" + COL_DLMT + {CURR_PRIN_AMT}"

+ ROW_DLMT + "TENURE" + COL_DLMT + "{TENURE}"
+ ROW_DLMT + "UNIT_TENURE" + COL_DLMT + "{UNIT_TENURE}"
+ ROW_DLMT + "INT_RATE" + COL_DLMT + "{INT_RATE}"
+ ROW_DLMT + "MAT_DT" + COL_DLMT + "{MAT_DT}"
+ ROW_DLMT + "VAL_DT" + COL_DLMT + "{VAL_DT}"
+ ROW_DLMT + "CURR_MAT_AMT" + COL_DLMT + "{CURR_MAT_AMT}"
+ ROW_DLMT + "IS_SHOW_INTEREST" + COL_DLMT + "{IS_SHOW_INTEREST}"
+ ROW_DLMT + "ALLOW_TIDEWDL_ONLINE " + COL_DLMT + "{ALLOW_TIDEWDL_ONLINE}"
+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"
;	

Trong đó:
PRODUCT_CODE: Mã sản phẩm → Server sẽ check nếu = 470 (tiết kiệm online) → trả về thêm biến “ALLOW_TIDEWDL” cho phép tất toán = 1
CCYCD: Loại tiền VND,….
CURR_PRIN_AMT: Gốc gửi
INT_RATE: Lãi suất
TENURE: kỳ hạn 
UNIT_TENURE:  đơn vị (M/W/D/Y)
MAT_DT: Ngày đến hạn
CURR_MAT_AMT: Lãi dự kiến

VAL_DT: Ngày mở (Nếu roll thì ngày ngày là ngày của kỳ mới)

IS_SHOW_INTEREST: Có hiển thị lãi suất hay không
ALLOW_TIDEWDL_ONLINE: Cho phép tất toán online hay không. Nếu = 1→ cho phép tất toán online (hiển thị thêm nút tất toán).
        */

        //    PROCEDURE GET_ACCT_TIDE_INFO_N(     
        //    pcustid IN VARCHAR2
        //    , PACCTNO IN VARCHAR2
        //    , PCCY IN VARCHAR2
        //    , OUT_CUR OUT REF_CUR
        //)
        public DataTable GET_ACCT_TIDE_INFO_N(string custid
                , string acctno
                , string ccycd
        )
        {
            try { 
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_TIDE_INFO_N", new OracleConnection(Config.gINTELLECTConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
            dsCmd.Parameters.Add("ccycd", OracleDbType.Varchar2, ccycd, ParameterDirection.Input);
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
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
        requestBody = " REQ=CMD#GET_ACCT_LOAN_INFO_N|ACTIVE_CODE#123456|CIF_NO#0310008705|ACCTNO#1000013376|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
 

public static String GET_ACCT_LOAN_INFO_N= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "ACCTNO" + COL_DLMT + "{ACCTNO}"
+ ROW_DLMT + "CCYCD" + COL_DLMT + "{CCYCD}"
 + ROW_DLMT + "OBALANCE" + COL_DLMT + {OBALANCE}"
+ ROW_DLMT + "OPENDT" + COL_DLMT + {OPENDT}"
+ ROW_DLMT + "EXPDT" + COL_DLMT + {EXPDT}"
+ ROW_DLMT + "RECORD_ACTIVITY" + COL_DLMT + "{RECORD_ACTIVITY}"

Trong đó:


OBALANCE: Số tiền gốc vay
OUT_STANDING: Dư nợ hiện tại
OPENDT: Ngày mở tài khoản
EXPDT: Ngày đến hạn
PAID: lãi phải trả (PRINPAID + PRINPAID)
RES_INT_RATE: Lãi suất
        */

        public DataSet GET_ACCT_LOAN_INFO_N(string custid, string acctno)
        {
            try {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_LOAN_INFO_N", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;
            } catch ( Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
            
        }

        /*
         lay thong tin detail cua tai khoan casa
         */
        public DataSet GET_ACCT_LIST_DETAIL(string custid, string acctno, string ccycd)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_LIST_DETAIL", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add("ccycd", OracleDbType.Varchar2, ccycd, ParameterDirection.Input);
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

        /*
         Request
msg="REQ=CMD#GET_ACCT_BALANCE_LIST_N|CIF_NO#0310005018|TYPE#001|CCYCD#VND|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

Response: 

public static String GET_ACCT_BALANCE_LIST_N= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"			
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"


        */
        public DataSet GET_ACCT_BALANCE_LIST_N(string custid, string acctno, string ccycd)
        {
            try{
            DataSet ds = new DataSet();
            
            dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_BALANCE_LIST_N", new OracleConnection(Config.gINTELLECTConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
            dsCmd.Parameters.Add("ccycd", OracleDbType.Varchar2, ccycd, ParameterDirection.Input);
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
             } catch ( Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

        
        public DataSet check_casa_belong_cif (string custid, string acctno)
        {
            try{
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_COMMON_CORE.CHECK_CASA_BELONG_CIF", new OracleConnection(Config.gINTELLECTConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
             } catch ( Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// Kiem tra so so tiet kiem thuoc CIF khong?
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="deposit_no"></param>
        /// <returns></returns>
        public DataSet CHECK_DEPOSIT_BELONG_CIF(string custid, string deposit_no)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand("PKG_COMMON_CORE.CHECK_DEPOSIT_BELONG_CIF", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("deposit_no", OracleDbType.Varchar2, deposit_no, ParameterDirection.Input);
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


        /// <summary>
        /// Kiem tra so the (masking) co thuoc CIF khong?
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="cardno"></param>
        /// <returns></returns>
        public DataSet CHECK_CARD_BELONG_CIF(string custid, string cardno)
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gCARDAPPSchema + "PKG_EBANKAPPS_PCI_NEW.CHECK_CARD_BELONG_CIF", new OracleConnection(Config.gCARDCnnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("cardno", OracleDbType.Varchar2, cardno, ParameterDirection.Input);
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



        /// <summary>
        /// lay danh sach tai khoan nhan
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="acctno"></param>
        /// <param name="ccycd"></param>
        /// <returns></returns>
        public DataSet GET_ACCT_LIST_RECEIVE(string custid, string acctno, string ccycd)
        {
            try
            {
                //OLD
                DataSet ds = new DataSet();

                dsCmd = new OracleCommand("PKG_COMMON_CORE.GET_ACCT_LIST_RECEIVE", new OracleConnection(Config.gINTELLECTConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("custid", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add("acctno", OracleDbType.Varchar2, acctno, ParameterDirection.Input);
                dsCmd.Parameters.Add("ccycd", OracleDbType.Varchar2, ccycd, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;

                //NEW ESB

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }


        public DataSet GET_BENNAME_FROM_CASA_ACCOUNT(string des_acct)
        {
            try { 
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand("PKG_COMMON_CORE.GETCLIENT_ACCTNO", new OracleConnection(Config.gINTELLECTConnstr));

            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add("v_acctno", OracleDbType.Varchar2, des_acct, ParameterDirection.Input);
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

        public DataSet GET_BEN_LIST_CUSTID_TRANTYPE(string custid, string trantype)
        {
            try{
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_BEN_LIST_CUSTID_TRANTYPE", new OracleConnection(Config.gINTELLECTConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add(TBL_EB_USER_CHANNEL.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_TRAN.TRAN_TYPE, OracleDbType.Varchar2, trantype, ParameterDirection.Input);
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds;
            } catch ( Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
        }

    }
}