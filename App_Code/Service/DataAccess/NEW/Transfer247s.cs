using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using mobileGW.Service.Framework;

using System.Xml;


namespace mobileGW.Service.DataAccess
{
    /// <summary>
    /// Summary description for Transfers.
    /// </summary>
    public class Transfer247s : IDisposable
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

        private OracleCommand dsCmd;
        private OracleDataAdapter dsApt;

        #endregion "List properties"

        public Transfer247s()
        {
            //
            dsApt = new OracleDataAdapter();

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);
            //
        }

        /// <summary>
        /// Linhtn add new 20160720
        /// Gọi ws để chuyển khoản qua thẻ, tài khoản
        /// </summary>
        /// <param name="tranid"></param>
        /// <param name="custid"></param>
        /// <param name="src_acct"></param>
        /// <param name="des_acct"></param>
        /// <param name="amount"></param>
        /// <param name="txdesc"></param>
        /// <param name="bank_code"></param>
        /// <param name="info_ref"></param>
        /// <returns></returns>
        public String postTF247toNAPAS(
           double tranid
           , string custid //for log only
           , string tran_type // temp for log only
            , string src_acct
           , string des_acct
           , double amount
           , string txdesc
           , string des_name
           , string bank_code
           , ref string info_ref
       )
        {
            string ret = "";
            string retws = "";
            string finalRet = "";


            Funcs.WriteLog("CIF: " + custid + "|BEGIN CALL 247 TO NAPAS:" + "|SRC_ACCT:" + src_acct
                + "|DES_ACCT:" + src_acct
                + "|BANK_CODE:" + bank_code
                 + "|TXDESC:" + txdesc
                );
            try
            {

                NapasIBT.Service myWS = new NapasIBT.Service();
                myWS.Url = Config.URL_WS_NAPAS_247;
                myWS.Timeout = Config.TIMEOUT_WITH_NAPAS_247;

                // goi sang napas ham chuyen tien qua the, tai khoan

                string retXML = "";

                //hot fix 2017 01 04
                //amount gui sang IBT them 02 so 00 o cuoi truoc khi padleft
                string tmpAmt = amount.ToString() + "00";

                if (tran_type == Config.TRAN_TYPE_ACQ_247CARD)
                {
                   

                   retXML = myWS.SHB_Tranfer_To_Card(
                   src_acct
                   , "910000"
                   //, amount.ToString().PadLeft(12, '0')
                   , tmpAmt.ToString().PadLeft(12, '0')
                   , tranid.ToString()
                   , "7399"
                   , "970443"
                   , "11111111"
                   , "SHB IBT                  HANOI       VNM".PadRight(40, ' ')
                   , des_acct //Destination_Number
                   , txdesc// Narration
                   );

                }
                else
                {
                    retXML = myWS.SHB_Tranfer_To_Acct(
                    src_acct
                    , "912020"
                    //, amount.ToString().PadLeft(12, '0')
                   , tmpAmt.ToString().PadLeft(12, '0')
                   , tranid.ToString()
                    , "7399"
                    , "970443"
                    , "11111111"
                    //"SHB IBT ACCT                HANOI       VNM".PadRight(40, ' '),
                    , des_name.PadRight(40, ' ') //account mae
                    , des_acct //Destination_Number,
                    , txdesc // Narration
                    , bank_code //Destination_Bank
                    );

                }

                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(retXML);
                string retStr = XmlDoc.GetElementsByTagName("Response_Code")[0].InnerText;

                //if (retStr==Config.gResult_SML_Arr[0].Split('|')[0] || retStr==Config.gResult_SML_Arr[2].Split('|')[0])
                //map RC =68 cua SML thanh 08 cua SHB. timeout

                if (retStr == Config.gResult_SML_Arr[0].Split('|')[0])
                {
                    finalRet = Config.ERR_CODE_DONE;
                    info_ref = Config.gResult_SML_Arr[0];
                    //return retStr;
                }
                else if (retStr == Config.gResult_SML_Arr[2].Split('|')[0]) // timeout 08
                {
                    info_ref = Config.gResult_SML_Arr[2];
                    finalRet = Config.ERR_CODE_TIMEOUT;
                    //	return 	"08|Giao dịch đang được xử lý tại ngân hàng thụ hưởng, quý khách vui lòng kiểm tra với người nhận.|Transaction sucessfully";
                }
                //NEU TRA VE MA LOI CU THE THI REVERT
                else
                {
                    //revSMLTranfer(double.Parse(Audit_Number));
                    //UPDATE_ACCT_2_ACCT(double.Parse(Audit_Number), Config.TX_STATUS_FAIL, "", "");
                    //if (retStr == Config.gResult_SML_Arr[10].Split('|')[0]) // timeout 08
                    //{
                    //    return Config.gResult_SML_Arr[10];
                    //    //	return 	"08|Giao dịch đang được xử lý tại ngân hàng thụ hưởng, quý khách vui lòng kiểm tra với người nhận.|Transaction sucessfully";
                    //}
                    //else
                    //{
                    //    return Config.gResult_SML_Arr[1];
                    //}

                    for (int i = 3; i < Config.gResult_SML_Arr.Length; i++)
                    {
                        if (Config.gResult_SML_Arr[i].Split('|')[0].Equals(retStr))
                        {
                            info_ref = Config.gResult_SML_Arr[i];
                            finalRet = Config.ERR_CODE_REVERT;
                            return finalRet;
                        }
                    }

                    //KHONG CO TRONG BANG LOI THI KHONG REVERT
                    info_ref = Config.gResult_SML_Arr[1];
                    finalRet = Config.ERR_CODE_GENERAL;
                    
                }
                return finalRet;

            }
            //NEU EXCEPTION KHONG HACH TOAN REVERT
            //EXCEPTION = MA LOI TIMEOUT
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                finalRet = Config.ERR_CODE_TIMEOUT;
                return finalRet;
            }

        }

        public String postTF247ToCore(
                double tran_id
                , string src_acct
                , string gl_suspend
                , string gl_fee
                , string gl_vat
                , double amount_suspend
                , double amount_fee
                , double amount_vat
                , string txdesc
                , string pos_cd
                , ref string core_txno_ref
                , ref string core_txdate_ref
        )
        {
            SHBUCS_XML.Service myWS = new SHBUCS_XML.Service();
            myWS.Timeout = Config.TIMEOUT_WITH_CORE;
            core_txno_ref = Config.refFormat + tran_id.ToString().PadLeft(9, '0');
            try
            {

                string postinfo;
                if (amount_fee > 0) //FEE_AMOUNT = 0
                {
                    postinfo = pos_cd + "|"
                    + src_acct + "|D|"
                    + (amount_suspend + amount_fee + amount_vat).ToString() + "|VND~"
                    + pos_cd + "|"
                    + gl_suspend + "|C|"
                    + amount_suspend.ToString() + "|VND~"
                    + pos_cd + "|"
                    + gl_fee + "|C|"
                    + amount_fee.ToString() + "|VND~"
                    + pos_cd + "|"
                    + gl_vat + "|C|"
                    + amount_vat.ToString() + "|VND";
                }
                else
                {
                    postinfo = pos_cd + "|"
                           + src_acct + "|D|"
                           + (amount_suspend).ToString() + "|VND~"
                           + pos_cd + "|"
                           + gl_suspend + "|C|"
                           + amount_suspend.ToString() + "|VND";
                }

              

                string retStr = myWS.FINANCIAL_POSTING(
                    Config.ChannelID
                    , Config.InterfaceID
                    , pos_cd
                    , postinfo
                    , txdesc
                    , ""
                    , core_txno_ref
                    );
                DataSet ds = Funcs.Node2Ds(retStr, "HEADER");

                if (ds.Tables[0].Rows[0]["res_Result_Code"].ToString()
                    == Config.gResult_INTELLECT_Arr[0].Split('|')[0])
                {
                    // UPDATE_CARD_2_CARD(v_tran_id, Config.TX_STATUS_DONE, Config.refFormat + v_tran_id.ToString().PadLeft(9, '0'), String.Format("{0:dd/MM/yyyy}", DateTime.Now));
                    string TxnDate = String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now);

                    core_txdate_ref = TxnDate;

                    // khong update trang thai luon vao bang TBL_EB
                    //Transfers tf = new Transfers();
                    //tf.uptTransferTx(tran_id, Config.TX_STATUS_DONE, core_txno, TxnDate, Config.ChannelID);
                    if (ds != null) ds.Dispose();
                    return Config.gResult_INTELLECT_Arr[0];
                }
                //goi hach toan vao core tra lai ma loi cu the thi revert
                else
                {

                    // revert transaction
                    //...
                    myWS.FINANCIALPOSTING_REVERT(
                        Config.ChannelID,
                        Config.InterfaceID,
                        core_txno_ref,
                        "0"
                        );
                    // UPDATE_CARD_2_CARD(v_tran_id, Config.TX_STATUS_FAIL, "", "");
                    //Transfers tf = new Transfers();
                    //tf.uptTransferTx(tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);

                    if (ds != null) ds.Dispose();
                    return Config.gResult_INTELLECT_Arr[1];
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                // revert transaction
                // ...
                //myWS.FINANCIALPOSTING_REVERT(
                //    Config.ChannelID,
                //    Config.InterfaceID,
                //    Config.refFormat + v_tran_id.ToString().PadLeft(9, '0'),
                //    "0"
                //    );
                //Transfers tf = new Transfers();
                //tf.uptTransferTx(tran_id, Config.TX_STATUS_FAIL, "", "", Config.ChannelID);
                return Config.gResult_INTELLECT_Arr[1];
            }
            finally
            {
                myWS.Dispose();
            }
        }


        //linhtn 20160719
        /// <summary>
        /// Thêm bảng đầu BIN thẻ --> từ số thẻ --> đầu BIN --> tên ngân hàng
        /// </summary>
        /// <param name="BIN"></param>
        /// <returns></returns>

        public DataTable getBankNameByBIN(string bin_card)
        {
            try
            {

                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.getBankNameByBIN ", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("bin_card", OracleDbType.Varchar2, bin_card, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable getBankCode247(string citad_code)
        {
            try
            {

                DataSet ds = new DataSet();

                dsCmd = new OracleCommand(Config.gEBANKSchema + "PKG_MOBILEBANKING_NEW.GET_BANKCODE_247_NEW", new OracleConnection(Config.gEBANKConnstr));
                dsCmd.CommandType = CommandType.StoredProcedure;
                dsCmd.Parameters.Add("PCITAD_CODE", OracleDbType.Varchar2, citad_code, ParameterDirection.Input);
                dsCmd.Parameters.Add(Config.OUT_CUR, OracleDbType.RefCursor, ParameterDirection.Output);
                dsApt.SelectCommand = dsCmd;
                dsApt.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
