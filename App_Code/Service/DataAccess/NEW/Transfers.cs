using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;
using System.Xml;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace mobileGW.Service.DataAccess
{
	/// <summary>
	/// Summary description for Transfers.
	/// </summary>
	public class Transfers:IDisposable
	{
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

        #region "List properties"
        public const string TRAN_ID="TRAN_ID" ;
                public const string  CHANNEL_ID="CHANNEL_ID" ;
                public const string  MOD_CD="MOD_CD" ;
                public const string  TRAN_TYPE="TRAN_TYPE" ;
                public const string  CUSTID="CUSTID" ;
                public const string  SRC_ACCT="SRC_ACCT" ;
                public const string  DES_ACCT="DES_ACCT" ;
                public const string AMOUNT ="AMOUNT" ;
                public const string  CCY_CD="CCY_CD" ;
                public const string CONV_RATE ="CONV_RATE" ;
                public const string LCY_AMOUNT ="LCY_AMOUNT" ;
                public const string  TXDESC="TXDESC" ;
                public const string  POS_CD="POS_CD" ;
                public const string  MKR_ID="MKR_ID" ;
                public const string MKR_DT = "MKR_DT";
                public const string  APR_ID1="APR_ID_1" ;
                public const string APR_DT1 = "APR_DT1";
                public const string  APR_ID2="APR_ID2" ;
                public const string APR_DT2 = "APR_DT1";
                public const string AUTH_TYPE ="AUTH_TYPE" ;
                public const string STATUS="STATUS" ;
                public const string TRAN_PWD_IDX ="TRAN_PWD_IDX" ;
                public const string  SMSCODE="SMSCODE" ;
                public const string  SIGN_DATA="SIGN_DATA" ;
                public const string  CORE_ERR_CODE="CORE_ERR_CODE" ;
                public const string  CORE_ERR_DESC="CORE_ERR_DESC" ;
                public const string  CORE_REF_NO="CORE_REF_NO" ;
                public const string  CORE_TXDATE="CORE_TXDATE" ;
                public const string  CORE_TXTIME="CORE_TXTIME" ;
                public const string ORDER_AMOUNT ="ORDER_AMOUNT" ;
                public const string ORDER_AMOUNT_DIS ="ORDER_AMOUNT_DIS" ;
                public const string  ORDER_ID="ORDER_ID" ;
                public const string  PARTNER_CODE="PARTNER_CODE" ;
                public const string  CATEGORY_CODE="CATEGORY_CODE" ;
                public const string  SERVICE_CODE="SERVICE_CODE" ;
                public const string  MERCHANT_CODE="MERCHANT_CODE" ;
                public const string  SUSPEND_ACCT="SUSPEND_ACCT" ;
                public const string  FEE_ACCT="FEE_ACCT" ;
                public const string  VAT_ACCT="VAT_ACCT" ;
                public const string SUSPEND_AMOUNT ="SUSPEND_AMOUNT" ;
                public const string FEE_AMOUNT ="FEE_AMOUNT" ;
                public const string VAT_AMOUNT ="VAT_AMOUNT" ;
                public const string  DES_NAME="DES_NAME" ;
                public const string  BANK_CODE="BANK_CODE" ;
                public const string  BANK_NAME="BANK_NAME" ;
                public const string  BANK_CITY="BANK_CITY" ;
                public const string  BANK_BRANCH="BANK_BRANCH" ;
                public const string  BM1="BM1" ;
                public const string  BM2="BM2" ;
                public const string  BM3="BM3" ;
                public const string  BM4="BM4" ;
                public const string  BM5="BM5" ;
                public const string  BM6="BM6" ;
                public const string  BM7="BM7" ;
                public const string  BM8="BM8" ;
                public const string  BM9="BM9" ;
                public const string  BM10="BM10" ;
                public const string  BM11="BM11" ;
                public const string  BM12="BM12" ;
                public const string  BM13="BM13" ;
                public const string  BM14="BM14" ;
                public const string  BM15="BM15" ;
                public const string  BM16="BM16" ;
                public const string  BM17="BM17" ;
                public const string  BM18="BM18" ;
                public const string  BM19="BM19" ;
                public const string  BM20="BM20" ;
                public const string  BM21="BM21" ;
                public const string  BM22="BM22" ;
                public const string  BM23="BM23" ;
                public const string  BM24="BM24" ;
                public const string  BM25="BM25" ;
                public const string  BM26="BM26" ;
                public const string  BM27="BM27" ;
                public const string  BM28="BM28" ;
                public const string  BM29="BM29" ;

		private OracleCommand dsCmd;
		private OracleDataAdapter dsApt;

        #endregion "List properties"

        public Transfers()
		{
			//
			dsApt = new OracleDataAdapter();
			//
		}

		public DataTable insTransferTx(
        #region "insTransferTx"
            //string tran_id, -- id tu sinh
        string channel_id,
        string mod_cd,
        string tran_type,
        string custid,
        string src_acct,
        string des_acct,
        double amount,
        string ccy_cd,
        double conv_rate,
        double lcy_amount,
        string txdesc,
        string pos_cd,
        string mkr_id,
        string mkr_dt,
        string apr_id1,
        string apr_dt1,
        string apr_id2,
        string apr_dt2,
        int auth_type,
        int status,
        int tran_pwd_idx,
        string smscode,
        string sign_data,
        string core_err_code,
        string core_err_desc,
        string core_ref_no,
        string core_txdate,
        string core_txtime,
        double order_amount,
        double order_amount_dis,
        string order_id,
        string partner_code,
        string category_code,
        string service_code,
        string merchant_code,
        string suspend_acct,
        string fee_acct,
        string vat_acct,
        double suspend_amount,
        double fee_amount,
        double vat_amount,
        string des_name,
        string bank_code,
        string bank_name,
        string bank_city,
        string bank_branch,
        string bm1,
        string bm2,
        string bm3,
        string bm4,
        string bm5,
        string bm6,
        string bm7,
        string bm8,
        string bm9,
        string bm10,
        string bm11,
        string bm12,
        string bm13,
        string bm14,
        string bm15,
        string bm16,
        string bm17,
        string bm18,
        string bm19,
        string bm20,
        string bm21,
        string bm22,
        string bm23,
        string bm24,
        string bm25,
        string bm26,
        string bm27,
        string bm28,
        string bm29
        )
        {
            try { 
				DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_TX_NEW.INSERT_TBL_EB_TRAN", new OracleConnection(Config.gEBANKConnstr));
				dsCmd.CommandType = CommandType.StoredProcedure;
                //dsCmd.Parameters.Add(TRAN_ID, OracleDbType.* **
                dsCmd.Parameters.Add(CHANNEL_ID, OracleDbType.Varchar2, channel_id, ParameterDirection.Input);
                dsCmd.Parameters.Add(MOD_CD, OracleDbType.Varchar2, mod_cd, ParameterDirection.Input);
                dsCmd.Parameters.Add(TRAN_TYPE, OracleDbType.Varchar2, tran_type, ParameterDirection.Input);
                dsCmd.Parameters.Add(CUSTID, OracleDbType.Varchar2, custid, ParameterDirection.Input);
                dsCmd.Parameters.Add(SRC_ACCT, OracleDbType.Varchar2, src_acct, ParameterDirection.Input);
                dsCmd.Parameters.Add(DES_ACCT, OracleDbType.Varchar2, des_acct, ParameterDirection.Input);
                dsCmd.Parameters.Add(AMOUNT, OracleDbType.Double, amount, ParameterDirection.Input);
                dsCmd.Parameters.Add(CCY_CD, OracleDbType.Varchar2, ccy_cd, ParameterDirection.Input);
                dsCmd.Parameters.Add(CONV_RATE, OracleDbType.Double, conv_rate , ParameterDirection.Input);
                dsCmd.Parameters.Add(LCY_AMOUNT, OracleDbType.Double, lcy_amount, ParameterDirection.Input);
                dsCmd.Parameters.Add(TXDESC, OracleDbType.Varchar2, txdesc, ParameterDirection.Input);
                dsCmd.Parameters.Add(POS_CD, OracleDbType.Varchar2, pos_cd, ParameterDirection.Input);
                dsCmd.Parameters.Add(MKR_ID, OracleDbType.Varchar2, mkr_id, ParameterDirection.Input);
                dsCmd.Parameters.Add(MKR_DT, OracleDbType.Varchar2, mkr_dt, ParameterDirection.Input);
                dsCmd.Parameters.Add(APR_ID1, OracleDbType.Varchar2, apr_id1, ParameterDirection.Input);
                dsCmd.Parameters.Add(APR_DT1, OracleDbType.Varchar2, apr_dt1, ParameterDirection.Input);
                dsCmd.Parameters.Add(APR_ID2, OracleDbType.Varchar2, apr_id2, ParameterDirection.Input);
                dsCmd.Parameters.Add(APR_DT2, OracleDbType.Varchar2, apr_dt2, ParameterDirection.Input);
                dsCmd.Parameters.Add(AUTH_TYPE, OracleDbType.Int16, auth_type, ParameterDirection.Input);
                dsCmd.Parameters.Add(STATUS, OracleDbType.Int16, status, ParameterDirection.Input);
                dsCmd.Parameters.Add(TRAN_PWD_IDX, OracleDbType.Int16, tran_pwd_idx, ParameterDirection.Input);
                dsCmd.Parameters.Add(SMSCODE, OracleDbType.Varchar2, smscode, ParameterDirection.Input);
                dsCmd.Parameters.Add(SIGN_DATA, OracleDbType.Varchar2, sign_data, ParameterDirection.Input);
                dsCmd.Parameters.Add(CORE_ERR_CODE, OracleDbType.Varchar2, core_err_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(CORE_ERR_DESC, OracleDbType.Varchar2, core_err_desc, ParameterDirection.Input);
                dsCmd.Parameters.Add(CORE_REF_NO, OracleDbType.Varchar2, core_ref_no, ParameterDirection.Input);
                dsCmd.Parameters.Add(CORE_TXDATE, OracleDbType.Varchar2, core_txdate, ParameterDirection.Input);
                dsCmd.Parameters.Add(CORE_TXTIME, OracleDbType.Varchar2, core_txtime, ParameterDirection.Input);
                dsCmd.Parameters.Add(ORDER_AMOUNT, OracleDbType.Double, order_amount, ParameterDirection.Input);
                dsCmd.Parameters.Add(ORDER_AMOUNT_DIS, OracleDbType.Double, order_amount_dis, ParameterDirection.Input);
                dsCmd.Parameters.Add(ORDER_ID, OracleDbType.Varchar2, order_id, ParameterDirection.Input);
                dsCmd.Parameters.Add(PARTNER_CODE, OracleDbType.Varchar2, partner_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(CATEGORY_CODE, OracleDbType.Varchar2, category_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(SERVICE_CODE, OracleDbType.Varchar2, service_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(MERCHANT_CODE, OracleDbType.Varchar2, merchant_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(SUSPEND_ACCT, OracleDbType.Varchar2, suspend_acct, ParameterDirection.Input);
                dsCmd.Parameters.Add(FEE_ACCT, OracleDbType.Varchar2, fee_acct, ParameterDirection.Input);
                dsCmd.Parameters.Add(VAT_ACCT, OracleDbType.Varchar2, vat_acct, ParameterDirection.Input);
                dsCmd.Parameters.Add(SUSPEND_AMOUNT, OracleDbType.Double, suspend_amount, ParameterDirection.Input);
                dsCmd.Parameters.Add(FEE_AMOUNT, OracleDbType.Double, fee_amount, ParameterDirection.Input);
                dsCmd.Parameters.Add(VAT_AMOUNT, OracleDbType.Double, vat_amount, ParameterDirection.Input);
                dsCmd.Parameters.Add(DES_NAME, OracleDbType.Varchar2, des_name, ParameterDirection.Input);
                dsCmd.Parameters.Add(BANK_CODE, OracleDbType.Varchar2, bank_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(BANK_NAME, OracleDbType.Varchar2, bank_name, ParameterDirection.Input);
                dsCmd.Parameters.Add(BANK_CITY, OracleDbType.Varchar2, bank_city, ParameterDirection.Input);
                dsCmd.Parameters.Add(BANK_BRANCH, OracleDbType.Varchar2, bank_branch, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM1, OracleDbType.Varchar2, bm1, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM2, OracleDbType.Varchar2, bm2, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM3, OracleDbType.Varchar2, bm3, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM4, OracleDbType.Varchar2, bm4, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM5, OracleDbType.Varchar2, bm5, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM6, OracleDbType.Varchar2, bm6, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM7, OracleDbType.Varchar2, bm7, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM8, OracleDbType.Varchar2, bm8, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM9, OracleDbType.Varchar2, bm9, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM10, OracleDbType.Varchar2, bm10, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM11, OracleDbType.Varchar2, bm11, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM12, OracleDbType.Varchar2, bm12, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM13, OracleDbType.Varchar2, bm13, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM14, OracleDbType.Varchar2, bm14, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM15, OracleDbType.Varchar2, bm15, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM16, OracleDbType.Varchar2, bm16, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM17, OracleDbType.Varchar2, bm17, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM18, OracleDbType.Varchar2, bm18, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM19, OracleDbType.Varchar2, bm19, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM20, OracleDbType.Varchar2, bm20, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM21, OracleDbType.Varchar2, bm21, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM22, OracleDbType.Varchar2, bm22, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM23, OracleDbType.Varchar2, bm23, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM24, OracleDbType.Varchar2, bm24, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM25, OracleDbType.Varchar2, bm25, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM26, OracleDbType.Varchar2, bm26, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM27, OracleDbType.Varchar2, bm27, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM28, OracleDbType.Varchar2, bm28, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM29, OracleDbType.Varchar2, bm29, ParameterDirection.Input);
                
                dsCmd.Parameters.Add( Config.OUT_CUR, OracleDbType.RefCursor,ParameterDirection.Output);

				dsApt.SelectCommand = dsCmd;
				
				dsApt.Fill(ds);
				return ds.Tables[0];
                }catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return null;
            }
			
		}
        #endregion "insTransferTx"

        /*
         PROCEDURE UPDATE_TBL_EB_TRAN (
                                 pTRAN_ID   IN NUMBER
                                 , pSTATUS    IN NUMBER
                                 , pCORE_REF_NO   IN VARCHAR2                                 
                                 , pCORE_TXDATE   IN VARCHAR2
                                 , pCHANNEL_ID  IN VARCHAR2
                                 , pSIGN_DATA   IN VARCHAR2
   );
   
         */

        public DataSet uptTransferTx(double tranID, int status, string core_txno, string core_txdate, string channelID, string bm27="_NULL_")
        {
            try { 
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_TX_NEW.UPDATE_TBL_EB_TRAN_640", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add(TRAN_ID, OracleDbType.Double, tranID, ParameterDirection.Input);
                dsCmd.Parameters.Add(STATUS, OracleDbType.Double, status, ParameterDirection.Input);
                dsCmd.Parameters.Add( CORE_REF_NO, OracleDbType.Varchar2, core_txno, ParameterDirection.Input);
                dsCmd.Parameters.Add( CORE_TXDATE, OracleDbType.Varchar2, core_txdate, ParameterDirection.Input);
                dsCmd.Parameters.Add( CHANNEL_ID, OracleDbType.Varchar2, channelID, ParameterDirection.Input);
                dsCmd.Parameters.Add( SIGN_DATA, OracleDbType.Varchar2, "_NULL_", ParameterDirection.Input);
                //dsCmd.Parameters.Add( BM27, OracleDbType.Varchar2, "", ParameterDirection.Input);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds;    
                } catch (Exception ex)
            { 
                    
                    return null;
                    Funcs.WriteLog(ex.ToString());
                }
         
        }

        /// <summary>
        /// Dung cho tide booking de cap nhat mot so thong tin vao cac cot bm
        /// </summary>
        /// <param name="tranID"></param>
        /// <param name="status"></param>
        /// <param name="core_txno"></param>
        /// <param name="core_txdate"></param>
        /// <param name="channelID"></param>
        /// <param name="bm1"></param>
        /// <param name="bm2"></param>
        /// <param name="bm3"></param>
        /// <param name="bm4"></param>
        /// <param name="bm5"></param>
        /// <param name="bm6"></param>
        /// <param name="bm7"></param>
        /// <param name="bm8"></param>
        /// <param name="bm9"></param>
        /// <returns></returns>

        public DataSet uptTransferTx_Full(
            double tranID, int status, string core_txno, string core_txdate, string channelID
            , string bm1
            , string bm2
            , string bm3
            , string bm4
            , string bm5
            , string bm6
            , string bm7
            , string bm8
            , string bm9
            )
        {
            try
            {
                DataSet ds = new DataSet();
                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_TX_NEW.UPDATE_TBL_EB_TRAN_FULL", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add(TRAN_ID, OracleDbType.Double, tranID, ParameterDirection.Input);
                dsCmd.Parameters.Add(STATUS, OracleDbType.Int16, status, ParameterDirection.Input);
                dsCmd.Parameters.Add(CORE_REF_NO, OracleDbType.Varchar2, core_txno, ParameterDirection.Input);
                dsCmd.Parameters.Add(CORE_TXDATE, OracleDbType.Varchar2, core_txdate, ParameterDirection.Input);
                dsCmd.Parameters.Add(CHANNEL_ID, OracleDbType.Varchar2, channelID, ParameterDirection.Input);
                dsCmd.Parameters.Add(SIGN_DATA, OracleDbType.Varchar2, "_NULL_", ParameterDirection.Input);
                dsCmd.Parameters.Add( BM1, OracleDbType.Varchar2, bm1, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM2, OracleDbType.Varchar2, bm2, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM3, OracleDbType.Varchar2, bm3, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM4, OracleDbType.Varchar2, bm4, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM5, OracleDbType.Varchar2, bm5, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM6, OracleDbType.Varchar2, bm6, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM7, OracleDbType.Varchar2, bm7, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM8, OracleDbType.Varchar2, bm8, ParameterDirection.Input);
                dsCmd.Parameters.Add(BM9, OracleDbType.Varchar2, bm9, ParameterDirection.Input);
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

        #region "OLD FINPOST"
   //     public String postFINPOSTToCore(
   //         string custid, //for log only
   //         string tran_type, //for log only
   //         double tran_id
   //        , string src_acct
   //        , string gl_suspend
   //        , string gl_fee
   //        , string gl_vat
   //        , double amount_suspend
   //        , double amount_fee
   //        , double amount_vat
   //        , string txdesc
   //        , string pos_cd
   //        , ref string core_txno_ref
   //        , ref string core_txdate_ref
   //)
   //     {
   //         SHBUCS_XML.Service myWS = new SHBUCS_XML.Service();
   //         myWS.Timeout = Config.TIMEOUT_WITH_CORE;
   //         core_txno_ref = Config.refFormat + tran_id.ToString().PadLeft(9, '0');
   //         try
   //         {

   //             string postinfo;
   //             if (amount_fee > 0) //FEE_AMOUNT = 0
   //             {
   //                 postinfo = pos_cd + "|"
   //                 + src_acct + "|D|"
   //                 + (amount_suspend + amount_fee + amount_vat).ToString() + "|VND~"
   //                 + pos_cd + "|"
   //                 + gl_suspend + "|C|"
   //                 + amount_suspend.ToString() + "|VND~"
   //                 + pos_cd + "|"
   //                 + gl_fee + "|C|"
   //                 + amount_fee.ToString() + "|VND~"
   //                 + pos_cd + "|"
   //                 + gl_vat + "|C|"
   //                 + amount_vat.ToString() + "|VND";
   //             }
   //             else
   //             {
   //                 postinfo = pos_cd + "|"
   //                        + src_acct + "|D|"
   //                        + (amount_suspend).ToString() + "|VND~"
   //                        + pos_cd + "|"
   //                        + gl_suspend + "|C|"
   //                        + amount_suspend.ToString() + "|VND";
   //             }
   //             string retStr = myWS.FINANCIAL_POSTING(
   //                 Config.ChannelID
   //                 , Config.InterfaceID
   //                 , pos_cd
   //                 , postinfo
   //                 , txdesc
   //                 , ""
   //                 , core_txno_ref
   //                 );
   //             DataSet ds = Funcs.Node2Ds(retStr, "HEADER");

   //             if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
   //                 == Config.gResult_INTELLECT_Arr[0].Split('|')[0])
   //             {
   //                 // UPDATE_CARD_2_CARD(v_tran_id, Config.TX_STATUS_DONE, Config.refFormat + v_tran_id.ToString().PadLeft(9, '0'), String.Format("{0:dd/MM/yyyy}", DateTime.Now));
   //                 string TxnDate = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

   //                 core_txdate_ref = TxnDate;

   //                 // khong update trang thai luon vao bang TBL_EB
   //                 //Transfers tf = new Transfers();
   //                 //tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno, TxnDate, Config.ChannelID);
   //                 if (ds != null) ds.Dispose();
   //                 return Config.gResult_INTELLECT_Arr[0];
   //             }
   //             //goi hach toan vao core tra lai ma loi cu the thi revert
   //             //KHONG REVERT FOR SAFE
   //             else
   //             {

   //                 // revert transaction
   //                 //...
   //                 myWS.FINANCIALPOSTING_REVERT(
   //                     Config.ChannelID,
   //                     Config.InterfaceID,
   //                     core_txno_ref,
   //                     "0"
   //                     );
   //                 // UPDATE_CARD_2_CARD(v_tran_id, Config.TX_STATUS_FAIL, "", "");
   //                 //Transfers tf = new Transfers();
   //                 //tf.uptTransferTx(tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);

   //                 if (ds != null) ds.Dispose();
   //                 return Config.gResult_INTELLECT_Arr[1];
   //             }
   //         }
   //         catch (Exception ex)
   //         {
   //             Funcs.WriteLog(ex.ToString());
   //             // revert transaction
   //             // ...
   //             //myWS.FINANCIALPOSTING_REVERT(
   //             //    Config.ChannelID,
   //             //    Config.InterfaceID,
   //             //    Config.refFormat + v_tran_id.ToString().PadLeft(9, '0'),
   //             //    "0"
   //             //    );
   //             //Transfers tf = new Transfers();
   //             //tf.uptTransferTx(tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
   //             return Config.gResult_INTELLECT_Arr[1];
   //         }
   //         finally
   //         {
   //             myWS.Dispose();
   //         }
   //     }
        #endregion "OLD FINPOST"

        //NEW FINPOST
        

        #region "OLD FUNDTRANSFER"
        /*
        * OLD
        public string pstTransderTx(String CustID,double TranId,string SrcAcct,string DesAcct, double Amount,
           double feeAmount, String TxDesc, ref string core_txno_ref, ref string core_txdate_ref)
       {
           SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
           myWS.Timeout = Config.TIMEOUT_WITH_CORE;

           core_txno_ref = Config.refFormat + TranId.ToString().PadLeft(9, '0');
           try
           {
               string retStr = 
                   myWS.FUNDTRANSFER(Config.ChannelID,Config.InterfaceID,SrcAcct,DesAcct,
                   Decimal.Parse(Amount.ToString()),TxDesc,"",
                   core_txno_ref);

               DataSet ds = Funcs.Node2Ds(retStr,"HEADER");

               if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
                   ==Config.gResult_INTELLECT_Arr[0].Split('|')[0])
               {
                   //string TxnDate = ds.Tables[0].Rows[0]["res_Tran_Time"].ToString();
                   //TxnDate = TxnDate.Substring(TxnDate.Length-2,2) + "/" + TxnDate.Substring(TxnDate.Length-4,2) + "/" + TxnDate.Substring(0,4);
                   // Success update to DB
                   //uptTransferTx(TranId,Config.TX_STATUS_DONE, core_txno,TxnDate ,Config.ChannelID);

                   core_txdate_ref = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                   myWS.Dispose();
                   ds.Dispose();
                   myWS = null;
                   return Config.gResult_INTELLECT_Arr[0];
               }
               else
               {
                   // Revert 4 safe - because we already receive UNSUCESS MSG from CORE
                   myWS.TXNREVERT(Config.ChannelID,
                                   Config.InterfaceID,
                                   Config.refFormat + TranId.ToString().PadLeft(9,'0'),"",
                                   "HUY GD:" + TxDesc,
                                   "HUY GD:" + TxDesc);

                   // Update la bi loi
                   //uptTransferTx(TranId,Config.TX_STATUS_FAIL,"","" ,Config.ChannelID);

                   // Bao loi
                   myWS.Dispose();
                   ds.Dispose();
                   myWS = null;

                   // Return the error
                   for (int i=2;i<Config.gResult_INTELLECT_Arr.Length;i++)
                   {
                       if (Config.gResult_INTELLECT_Arr[i].Split('|')[0]==ds.Tables[0].Rows[0]["res_Result_Code"].ToString())
                       {
                           return Config.gResult_INTELLECT_Arr[i];
                       }
                   }

                   return Config.gResult_INTELLECT_Arr[1];


               }
           }
           catch (Exception ex)
           {
               Funcs.WriteLog( ex.ToString());
               myWS.TXNREVERT(Config.ChannelID,Config.InterfaceID,Config.refFormat + TranId.ToString().PadLeft(9,'0'),"",
                   "HUY GD:" + TxDesc,
                   "HUY GD:" + TxDesc);
               return Config.gResult_INTELLECT_Arr[1];
           }

       }

        */
        #endregion "OLD FUNDTRANSFER"


        /*NEW INTRA FUNDTRANSFER
         intFx: Chuyển khoản trong SHB gọi qua Integrator
             */
        public string pstTransderTx(String CustID,double TranId,string SrcAcct,string DesAcct, double Amount,
            double feeAmount, String TxDesc, ref string core_txno_ref, ref string core_txdate_ref)
		{
			core_txno_ref = Config.refFormat + TranId.ToString().PadLeft(9, '0');

            InternalPosting.IntXferAddResType res = null;
            

            bool result = false;

            InternalPosting.AppHdrType appHdr = new InternalPosting.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            InternalPosting.PairsType nsFrom = new InternalPosting.PairsType();
            nsFrom.Id = "EB";
            nsFrom.Name = "EB";

            InternalPosting.PairsType nsTo = new InternalPosting.PairsType();
            nsTo.Id = "CORE";
            nsTo.Name = "CORE";

            InternalPosting.PairsType[] listOfNsTo = new InternalPosting.PairsType[1];
            listOfNsTo[0] = nsTo;

            InternalPosting.PairsType BizSvc = new InternalPosting.PairsType();
            BizSvc.Id = "IntXfer";
            BizSvc.Name = "IntXfer";

            //String tranDTString = "02/10/2017 00:00:00";  //"2017-02-10T00:00:00"    //"01/08/2008 14:50:50.42";

            DateTime TransDt = DateTime.Now;
            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;

            //Body


            InternalPosting.IntXferAddReqType msgReq = new InternalPosting.IntXferAddReqType();
            msgReq.AppHdr = appHdr;
            msgReq.ChnlId = Config.ChannelID;
            msgReq.ItfId = Config.InterfaceID;
            msgReq.RefNo = core_txno_ref;
            msgReq.DbAcct = new InternalPosting.BankAcctIdType();
            msgReq.CrAcct = new InternalPosting.BankAcctIdType();
            msgReq.DbAcct.AcctId = SrcAcct;
            msgReq.CrAcct.AcctId = DesAcct;
            msgReq.TxAmt = decimal.Parse( Amount.ToString());
            msgReq.InRemark = TxDesc;
            msgReq.ExRemark = TxDesc;
            msgReq.DealType = string.Empty;
            appHdr.Signature =  Funcs.encryptMD5(msgReq.DbAcct.AcctId + msgReq.CrAcct.AcctId + msgReq.TxAmt + msgReq.InRemark + msgReq.ExRemark + msgReq.DealType + Config.SharedKeyMD5).ToUpper();
            
            //portypeClient
            InternalPosting.PortTypeClient ptc = new InternalPosting.PortTypeClient();

            try
            {
                res = ptc.IntXferAdd(msgReq);
                Funcs.WriteLog("CustID:" + CustID + "|res from ESB:" + res.ToString());

                ptc.Close();
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("CustID:" + CustID + "|res from ESB exception:" + ex.ToString());
            }

            
            //NEU GAP LOI CHUNG CHUNG
            if (res == null ||
                res.RespSts == null ||
                res.RespSts.Sts == null ||
                !res.RespSts.Sts.Equals("0")            
            )
            {
                core_txdate_ref = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                return Config.gResult_INTELLECT_Arr[1];
            }


            //Hach toan thanh cong
            if (Config.gResult_INTELLECT_Arr[0].Split('|')[0].Equals(res.ResStatus))
            {
                core_txdate_ref = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                return Config.gResult_INTELLECT_Arr[0];
            }
            //hach toan khong thanh cong, lay duoc ra ma loi
            else
            {
                //neu co loi cu the thi tra ve ma loi cu the, thong bao cho khach hang
                for (int i = 2; i < Config.gResult_INTELLECT_Arr.Length; i++)
                {
                    if (Config.gResult_INTELLECT_Arr[i].Split('|')[0].Equals(res.ResStatus))
                    {
                        return Config.gResult_INTELLECT_Arr[i];
                    }
                }

                //neu loi ngoai bang ma loi --> quy ve loi chung chung
                return Config.gResult_INTELLECT_Arr[1];
            }
        }

		public string revTransderTx(double TranId,string txDesc )
		{
			SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
			myWS.Timeout = Config.TIMEOUT_WITH_CORE;
			try
			{
				string retStr = 
					myWS.TXNREVERT(Config.ChannelID,Config.InterfaceID,	
					Config.refFormat + TranId.ToString().PadLeft(9,'0'),"",
					"HUY GD:" + txDesc ,
					"HUY GD:" + txDesc);

				DataSet ds = Funcs.Node2Ds(retStr,"HEADER");

				if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
					==Config.gResult_INTELLECT_Arr[0].Split('|')[0])
				{
					//uptTransferTx(TranId,Config.TX_STATUS_FAIL,ds.Tables[0].Rows[0]["res_Ref_No"].ToString(),"" ,Config.ChannelID);

					myWS.Dispose();
					ds.Dispose();
					myWS = null;
					return Config.gResult_INTELLECT_Arr[0];
				}
				else
				{
					return Config.gResult_INTELLECT_Arr[1];
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog(TranId.ToString() + "-->" +  ex.ToString());
				//myWS.TXNREVERT(Config.ChannelID,Config.InterfaceID,TranId.ToString(),"0");
				return Config.gResult_INTELLECT_Arr[1];
			}			
		}



        #region "REVERT FINPOST"
        //public string revFinPost(double TranId, string txDesc)
        //{
        //    SHBUCS_XML.Service myWS = new mobileGW.SHBUCS_XML.Service();
        //    myWS.Timeout = Config.TIMEOUT_WITH_CORE;
        //    try
        //    {
        //        string retStr =
        //            myWS.FINANCIALPOSTING_REVERT(Config.ChannelID, Config.InterfaceID,
        //            Config.refFormat + TranId.ToString().PadLeft(9, '0'), "");

        //        DataSet ds = Funcs.Node2Ds(retStr, "HEADER");

        //        if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
        //            == Config.gResult_INTELLECT_Arr[0].Split('|')[0])
        //        {
        //            //uptTransferTx(TranId,Config.TX_STATUS_FAIL,ds.Tables[0].Rows[0]["res_Ref_No"].ToString(),"" ,Config.ChannelID);

        //            myWS.Dispose();
        //            ds.Dispose();
        //            myWS = null;
        //            return Config.gResult_INTELLECT_Arr[0];
        //        }
        //        else
        //        {
        //            return Config.gResult_INTELLECT_Arr[1];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(TranId.ToString() + "-->" + ex.ToString());
        //        //myWS.TXNREVERT(Config.ChannelID,Config.InterfaceID,TranId.ToString(),"0");
        //        return Config.gResult_INTELLECT_Arr[1];
        //    }
        //}
        #endregion "REVERT FINPOST"

      
        // credit card transfer


        /*
         //OLD: chuyen khoan lien ngan hang qua CITAD
         public string PostDomesticTxnToCore(string custid, double tranid, string src_acct, string ccycd, string des_acct,
            string desName, string bankCode, string bankName, string bankCity, string bankBranch,
            double amount, double feeAmt, string txdesc, ref string core_txno_ref, ref string core_txdate_ref)
        {
            core_txno_ref = "";
            SHBUCS_XML.Service myWS = new SHBUCS_XML.Service();
            myWS.Timeout = Config.TIMEOUT_WITH_CORE;
            try
            {
                string valdate = (new SysVars()).getBUSDATE(Config.defaultBR, Config.txType_INTER);
                if (valdate == null) valdate = DateTime.Now.ToString("yyyyMMdd");
                string retStr =
                    myWS.FUNDTRANSFER_INTERBANK(
                        Config.ChannelID, Config.InterfaceID, src_acct, des_acct,
                    amount, ccycd, txdesc, valdate,
                    desName, "", "", "", "", "", "", "",  bankCode.Substring(2, 3),
                    bankCode.Substring(5), bankCode.Substring(0, 2), bankName, bankBranch, bankCity,
                    Config.refFormat_Book + tranid.ToString().PadLeft(9, '0'));

                DataSet ds = Funcs.Node2Ds(retStr, "HEADER");

                if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
                    == Config.gResult_INTELLECT_Arr[0].Split('|')[0])
                {
                    // Success update to DB
                    //uptBookingTx(BookId, Config.TX_STATUS_DONE, ds.Tables[0].Rows[0]["res_Ref_No"].ToString(), Config.ChannelID);
                    core_txno_ref = ds.Tables[0].Rows[0]["res_Ref_No"].ToString();
                    //core_txdate_ref = DateTime.Now.ToString("yyyyMMdd HH:mm");
                    //linhtn: fix bug 2016 12 26 
                    core_txdate_ref = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    //uptTransferTx(tranid, Config.TX_STATUS_DONE, core_txno_ref, valdate, Config.ChannelID);

                    myWS.Dispose();
                    ds.Dispose();
                    myWS = null;
                    return Config.gResult_INTELLECT_Arr[0];
                }
                else
                {
                    //// Revert
                    //myWS.TXNREVERT(Config.ChannelID, Config.InterfaceID, tranid.ToString(), "",
                    //    "HUY GD:" + txdesc,
                    //    "HUY GD:" + txdesc);

                    //// Update la bi loi
                    ////uptBookingTx(BookId, Config.TX_STATUS_FAIL, "", Config.ChannelID);
                    ////

                    //uptTransferTx(tranid, Config.TX_STATUS_FAIL, "",  "", Config.ChannelID);
                    //// Bao loi
                    //myWS.Dispose();
                    //ds.Dispose();
                    //myWS = null;

                    //// Return the error
                    //for (int i = 2; i < Config.gResult_INTELLECT_Arr.Length; i++)
                    //{
                    //    if (Config.gResult_INTELLECT_Arr[i].Split('|')[0] == ds.Tables[0].Rows[0]["res_Result_Code"].ToString())
                    //    {
                    //        return Config.gResult_INTELLECT_Arr[i];
                    //    }
                    //}

                    return Config.gResult_INTELLECT_Arr[1];


                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                //myWS.TXNREVERT(Config.ChannelID, Config.InterfaceID, tranid.ToString(), "",
                //    "HUY GD:" + txdesc,
                //    "HUY GD:" + txdesc);
                return Config.gResult_INTELLECT_Arr[1];
            }

        }
             */
        public string PostDomesticTxnToCore(string custid, double tranid, string src_acct, string ccycd, string des_acct,
            string desName, string bankCode, string bankName, string bankCity, string bankBranch, string bank_city_name, string bank_branch_name,
            double amount, double feeAmt, string txdesc, ref string core_txno_ref, ref string core_txdate_ref)
        {
            core_txno_ref = Config.ChannelID + tranid.ToString().PadLeft(9, '0');
            try
            {
                //string valdate = (new SysVars()).getBUSDATE(Config.defaultBR, Config.txType_INTER);

                string valdate = CoreIntegration.GetCoreTime(custid) ;

                //if (valdate == null) valdate = DateTime.Now.ToString("yyyyMMdd");
                if (valdate == string.Empty) valdate = DateTime.Now.ToString("yyyyMMdd");

                DomXfer.DomXferAddResType res = null;

                DomXfer.AppHdrType appHdr = new DomXfer.AppHdrType();
                appHdr.CharSet = "UTF-8";
                appHdr.SvcVer = "1.0";

                DomXfer.PairsType nsFrom = new DomXfer.PairsType();
                nsFrom.Id = "EB";
                nsFrom.Name = "EB";

                DomXfer.PairsType nsTo = new DomXfer.PairsType();
                nsTo.Id = "CORE";
                nsTo.Name = "CORE";

                DomXfer.PairsType[] listOfNsTo = new DomXfer.PairsType[1];
                listOfNsTo[0] = nsTo;

                DomXfer.PairsType BizSvc = new DomXfer.PairsType();
                BizSvc.Id = "DomXfer";
                BizSvc.Name = "DomXfer";

                //String tranDTString = "02/10/2017 00:00:00";  //"2017-02-10T00:00:00"    //"01/08/2008 14:50:50.42";

                DateTime TransDt = DateTime.Now;
                appHdr.From = nsFrom;
                appHdr.To = listOfNsTo;
                appHdr.MsgId = Funcs.GenESBMsgId();
                appHdr.MsgPreId = "";
                appHdr.BizSvc = BizSvc;
                appHdr.TransDt = TransDt;

                //Body
                DomXfer.DomXferAddReqType msgReq = new DomXfer.DomXferAddReqType();
                msgReq.AppHdr = appHdr;
                msgReq.ChnlId = Config.ChannelID;
                msgReq.ItfId = Config.InterfaceID;
                msgReq.RefNo = core_txno_ref;
                msgReq.DbAcct = src_acct;
                msgReq.CrAcct = des_acct;
                msgReq.TxAmt = decimal.Parse(amount.ToString());
                msgReq.TxCur = ccycd;
                msgReq.TxDt = DateTime.Now.ToString("yyyyMMdd");
                msgReq.ValDt = valdate;
                msgReq.WIB = "N";
                msgReq.BenName = Funcs.removeStress(desName);
                msgReq.BenAddr1 = "*";
                msgReq.BenAdd2 = "*";
                msgReq.BenCity = "*";
                msgReq.IdType = "*";
                msgReq.IdNo = "*";
                msgReq.IdIssuePlace = "*";
                msgReq.IdExpDt = "*";
                msgReq.CustRmk = txdesc;
                msgReq.IntRmk = string.Empty;
                msgReq.Mode = Config.MODE_CORETIME;

                msgReq.DealType = string.Empty;
                msgReq.ChrgType = "E";

                msgReq.BnkCode = bankCode.Length >= 6 ? decimal.Parse(bankCode.Substring(2, 3)) : decimal.Parse(bankCode);
                msgReq.BnkDesc = Funcs.removeStress(bankName);

                msgReq.BrCode = bankBranch; //BRANCH CODE
                msgReq.BrCd = "BR0001"; //default

                msgReq.BrDesc = Funcs.removeStress(bank_branch_name);
                msgReq.CtyCd = bankCity;
                msgReq.CtyDesc = Funcs.removeStress(bank_city_name);

                appHdr.Signature = Funcs.encryptMD5(msgReq.DbAcct + msgReq.CrAcct
                    + msgReq.TxAmt + msgReq.CustRmk +
                    msgReq.CtyCd + msgReq.BnkCode + msgReq.BrCode + Config.SharedKeyMD5).ToUpper();

                //portypeClient
                DomXfer.PortTypeClient ptc = new DomXfer.PortTypeClient();
                try
                {
                    res = ptc.Create(msgReq);
                    Funcs.WriteLog("custid:" + custid + "|PostDomesticTxnToCore|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    ptc.Close();
                }
                catch (Exception e)
                {
                    Funcs.WriteLog("custid:" + custid + "|PostDomesticTxnToCore Exception|" + e.ToString());
                }

                //write log

                if (res == null || res.RespSts == null || res.RespSts.Sts == null || !res.RespSts.Sts.Equals("0"))
                {
                    return Config.gResult_INTELLECT_Arr[1];
                }

                //Hach toan thanh cong
                if (res.RespSts.Sts.Equals("0"))
                {
                    core_txdate_ref = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    core_txno_ref = res.RefNo;
                    return Config.gResult_INTELLECT_Arr[0];
                }
                //hach toan khong thanh cong, lay duoc ra ma loi
                else
                {

                    //String messageCd = coreResult.Tables[0].Rows[0]["res_Result_Code"].ToString();

                    List<String> errorCds = new List<string>();
                    if (res != null && res.RespSts != null && res.RespSts.ErrInfo != null)
                    {
                        for (int i = 0; i < res.RespSts.ErrInfo.Length; i++)
                        {
                            errorCds.Add(res.RespSts.ErrInfo[i].ErrCd);
                        }

                    }

                    for (int i = 0; i < Config.gResult_INTELLECT_Arr.Length; i++)
                        foreach (String messageCd in errorCds)
                            if (Config.gResult_INTELLECT_Arr[i].Split('|')[0].Equals(messageCd))
                            {
                                return Config.gResult_INTELLECT_Arr[i];
                            }

                           return Config.gResult_INTELLECT_Arr[1];
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                //myWS.TXNREVERT(Config.ChannelID, Config.InterfaceID, tranid.ToString(), "",
                //    "HUY GD:" + txdesc,
                //    "HUY GD:" + txdesc);
                return Config.gResult_INTELLECT_Arr[1];
            }

        }
	}
}
