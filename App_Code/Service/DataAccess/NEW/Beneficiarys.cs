using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;


namespace mobileGW.Service.DataAccess
{

    /// <summary>
    /// Summary description for TBL_EB_USER_CHANNELs
    /// </summary>
    public class Beneficiarys
    {
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
        public Beneficiarys()
        {
            //
            dsApt = new OracleDataAdapter();
            //
        }

        /// <summary>
        /// linhtn 13 jul 2016
        /// lay danh sach thu huong theo tran type
        /// rieng phan the tin dung phai xu ly them ngoai phan goi store nay
        /// phai goi them ws card de lay list the tin dung cua minh
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="trantype"></param>
        /// <returns></returns>

        public DataTable GET_BEN_LIST_CUSTID_TRANTYPE(string custid, string trantype)
        {
            try { 
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand( Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_BEN_LIST_CUSTID_TRANTYPE", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add( TBL_EB_BEN.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add( TBL_EB_BEN.TRAN_TYPE, OracleDbType.Varchar2, trantype, ParameterDirection.Input);
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds.Tables[0];
            }catch (Exception ex)
            { 
                    Funcs.WriteLog (ex.ToString());
                    return null;
            }
        }

        /* 
        PROCEDURE DEL_BEN_CUSTID_TRANTYPE(
         PCUSTID IN VARCHAR2 -- OR USER NAME
       ,PTRAN_TYPE IN VARCHAR2   
       ,PACCTNO IN VARCHAR2
       , OUT_CUR OUT REF_CUR)
         * */

        /// <summary>
        /// linhtn 13 jul 2016
        /// xoa nguoi thu huong theo ma khach hang, loai giao dich, so tai khoan thu huong 
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="trantype"></param>
        /// <param name="benAcct"></param>
        /// <returns></returns>
        public DataTable DEL_BEN_CUSTID_TRANTYPE(string custid, string trantype, string acct, string category_id, string service_id, string bankCode)
        {
            try{
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.DEL_BEN_CUSTID_TRANTYPE_V2", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;
            dsCmd.Parameters.Add("PCUSTID", OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add("P_BANK_CODE", OracleDbType.Varchar2, bankCode, ParameterDirection.Input);
            dsCmd.Parameters.Add("PTRAN_TYPE", OracleDbType.Varchar2, trantype, ParameterDirection.Input);
            dsCmd.Parameters.Add("PACCTNO", OracleDbType.Varchar2, acct, ParameterDirection.Input);
            dsCmd.Parameters.Add("PCATEGORYID", OracleDbType.Varchar2, category_id, ParameterDirection.Input);
            dsCmd.Parameters.Add("PSERVICEID", OracleDbType.Varchar2, service_id, ParameterDirection.Input);
            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds.Tables[0];
             
            }catch (Exception ex)
            { 
                    Funcs.WriteLog (ex.ToString());
                    return null;
            }
        }


        /*
          PROCEDURE UPDATE_BEN(
             pCUSTID IN  VARCHAR2
            , pTRAN_TYPE IN  VARCHAR2
            , pACCTNO IN  VARCHAR2
            , pACCT_NAME IN  VARCHAR2
            , pACCT_NICK IN  VARCHAR2
            , pDEFAULT_TXDESC IN  VARCHAR2
            , pBANK_CODE IN  VARCHAR2
            , pBANK_NAME IN  VARCHAR2
            , pBANK_BRANCH IN  VARCHAR2
            , pBANK_CITY IN  VARCHAR2
            , pCATEGORY_ID IN  VARCHAR2
            , pSERVICE_ID IN  VARCHAR2
            , pLASTCHANGE IN  VARCHAR2
            , pBM1 IN  VARCHAR2
            , pBM2 IN  VARCHAR2
            , pBM3 IN  VARCHAR2
            , pBM4 IN  VARCHAR2
            , pBM5 IN  VARCHAR2
            , pBM6 IN  VARCHAR2
            , pBM7 IN  VARCHAR2
            , pBM8 IN  VARCHAR2
            , pBM9 IN  VARCHAR2
            , pBM10 IN  VARCHAR2
            , OUT_CUR OUT REF_CUR
            )
         */

        /// <summary>
        /// linhtn 13 jul 2016
        /// Cap nhat danh sach thu huong
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="trantype"></param>
        /// <param name="acct"></param>
        /// <param name="acct_name"></param>
        /// <param name="acct_nick"></param>
        /// <param name="default_txdesc"></param>
        /// <param name="bank_code"></param>
        /// <param name="bank_name"></param>
        /// <param name="bank_branch"></param>
        /// <param name="bank_city"></param>
        /// <param name="category_id"></param>
        /// <param name="service_id"></param>
        /// <param name="lastchange"></param>
        /// <param name="bm1"></param>
        /// <param name="bm2"></param>
        /// <param name="bm3"></param>
        /// <param name="bm4"></param>
        /// <param name="bm5"></param>
        /// <param name="bm6"></param>
        /// <param name="bm7"></param>
        /// <param name="bm8"></param>
        /// <param name="bm9"></param>
        /// <param name="bm10"></param>
        /// <returns></returns>
        public DataTable UPDATE_BEN(
                string custid
                , string trantype
                , string acct
                , string acct_name
            , string acct_nick
            , string default_txdesc
            , string bank_code
            , string bank_name
            , string bank_branch
            , string bank_city
            , string category_id
            , string service_id
            , string lastchange
            , string bm1
            , string bm2
            , string bm3
            , string bm4
            , string bm5
            , string bm6
            , string bm7
            , string bm8
            , string bm9
            , string bm10
            )
        {
            try { 
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.UPDATE_BEN", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;

            dsCmd.Parameters.Add(TBL_EB_BEN.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.TRAN_TYPE, OracleDbType.Varchar2, trantype, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.ACCTNO, OracleDbType.Varchar2, acct, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.ACCT_NAME, OracleDbType.Varchar2, acct_name, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.ACCT_NICK, OracleDbType.Varchar2, acct_nick, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.DEFAULT_TXDESC, OracleDbType.Varchar2, default_txdesc, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_CODE, OracleDbType.Varchar2, bank_code, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_NAME, OracleDbType.Varchar2, bank_name, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_BRANCH, OracleDbType.Varchar2, bank_branch, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_CITY, OracleDbType.Varchar2, bank_city, ParameterDirection.Input);

            dsCmd.Parameters.Add(TBL_EB_BEN.CATEGORY_ID, OracleDbType.Varchar2, category_id, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.SERVICE_ID, OracleDbType.Varchar2, service_id, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.LASTCHANGE, OracleDbType.Varchar2, lastchange, ParameterDirection.Input);

            dsCmd.Parameters.Add(TBL_EB_BEN.BM1, OracleDbType.Varchar2, bm1, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM2, OracleDbType.Varchar2, bm2, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM3, OracleDbType.Varchar2, bm3, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM4, OracleDbType.Varchar2, bm4, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM5, OracleDbType.Varchar2, bm5, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM6, OracleDbType.Varchar2, bm6, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM7, OracleDbType.Varchar2, bm7, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM8, OracleDbType.Varchar2, bm8, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM9, OracleDbType.Varchar2, bm9, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM10, OracleDbType.Varchar2, bm10, ParameterDirection.Input);

            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds.Tables[0];
        } catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
 
             }
        }

        /*
            
            PROCEDURE INSERT_BEN(
             pCUSTID IN  VARCHAR2
            , pTRAN_TYPE IN  VARCHAR2
            , pACCTNO IN  VARCHAR2
            , pACCT_NAME IN  VARCHAR2
            , pACCT_NICK IN  VARCHAR2
            , pDEFAULT_TXDESC IN  VARCHAR2
            , pBANK_CODE IN  VARCHAR2
            , pBANK_NAME IN  VARCHAR2
            , pBANK_BRANCH IN  VARCHAR2
            , pBANK_CITY IN  VARCHAR2
            , pCATEGORY_ID IN  VARCHAR2
            , pSERVICE_ID IN  VARCHAR2
            , pLASTCHANGE IN  VARCHAR2
            , pBM1 IN  VARCHAR2
            , pBM2 IN  VARCHAR2
            , pBM3 IN  VARCHAR2
            , pBM4 IN  VARCHAR2
            , pBM5 IN  VARCHAR2
            , pBM6 IN  VARCHAR2
            , pBM7 IN  VARCHAR2
            , pBM8 IN  VARCHAR2
            , pBM9 IN  VARCHAR2
            , pBM10 IN  VARCHAR2
            , OUT_CUR OUT REF_CUR
            )
         */
        public DataTable INSERT_BEN(
              string custid
              , string trantype
              , string acct
              , string acct_name
          , string acct_nick
          , string default_txdesc
          , string bank_code
          , string bank_name
          , string bank_branch
          , string bank_city
          , string category_id
          , string service_id
          , string lastchange
          , string bm1
          , string bm2
          , string bm3
          , string bm4
          , string bm5
          , string bm6
          , string bm7
          , string bm8
          , string bm9
          , string bm10
          )
        {
            try { 
            DataSet ds = new DataSet();
            dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.INSERT_BEN", new OracleConnection(Config.gEBANKConnstr));
            dsCmd.CommandType = CommandType.StoredProcedure;

            dsCmd.Parameters.Add(TBL_EB_BEN.CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.TRAN_TYPE, OracleDbType.Varchar2, trantype, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.ACCTNO, OracleDbType.Varchar2, acct, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.ACCT_NAME, OracleDbType.Varchar2, acct_name, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.ACCT_NICK, OracleDbType.Varchar2, acct_nick, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.DEFAULT_TXDESC, OracleDbType.Varchar2, default_txdesc, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_CODE, OracleDbType.Varchar2, bank_code, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_NAME, OracleDbType.Varchar2, bank_name, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_BRANCH, OracleDbType.Varchar2, bank_branch, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BANK_CITY, OracleDbType.Varchar2, bank_city, ParameterDirection.Input);

            dsCmd.Parameters.Add(TBL_EB_BEN.CATEGORY_ID, OracleDbType.Varchar2, category_id, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.SERVICE_ID, OracleDbType.Varchar2, service_id, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.LASTCHANGE, OracleDbType.Varchar2, lastchange, ParameterDirection.Input);

            dsCmd.Parameters.Add(TBL_EB_BEN.BM1, OracleDbType.Varchar2, bm1, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM2, OracleDbType.Varchar2, bm2, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM3, OracleDbType.Varchar2, bm3, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM4, OracleDbType.Varchar2, bm4, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM5, OracleDbType.Varchar2, bm5, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM6, OracleDbType.Varchar2, bm6, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM7, OracleDbType.Varchar2, bm7, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM8, OracleDbType.Varchar2, bm8, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM9, OracleDbType.Varchar2, bm9, ParameterDirection.Input);
            dsCmd.Parameters.Add(TBL_EB_BEN.BM10, OracleDbType.Varchar2, bm10, ParameterDirection.Input);

            dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
            dsApt.SelectCommand = dsCmd;
            dsApt.Fill(ds);
            return ds.Tables[0];
            } catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
                }
        }

    }
}