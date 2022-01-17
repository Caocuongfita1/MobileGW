using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using mobileGW;
using mobileGW.Service.Bussiness;
using mobileGW.Service.Framework;
using System.Threading;
using mobileGW.Service.DataAccess;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for Utils
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class Beneficiary
    {
        public Beneficiary()
        {
            //
            // TODO: Add constructor logic here
            //
          
        }

        public string GET_BENLIST_BY_TRAN_TYPE(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string trantype =   Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");          
            String retStr = Config.GET_BEN_LIST_CUSTID_TRANTYPE;
            
            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Beneficiarys da = new Beneficiarys();
                DataTable dt = new DataTable();
                dt = da.GET_BEN_LIST_CUSTID_TRANTYPE(custid, trantype);
                //2. Gen reply message
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strTemp = "";
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.ACCTNO] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.ACCTNO].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.ACCT_NAME] == DBNull.Value ? "_NULL_" : Funcs.NoHack (dt.Rows[j][TBL_EB_BEN.ACCT_NAME].ToString())) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.ACCT_NICK] == DBNull.Value ? "_NULL_" : Funcs.NoHack( dt.Rows[j][TBL_EB_BEN.ACCT_NICK].ToString())) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.DEFAULT_TXDESC] == DBNull.Value ? "_NULL_" : Funcs.NoHack( dt.Rows[j][TBL_EB_BEN.DEFAULT_TXDESC].ToString())) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BANK_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BANK_CODE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BANK_NAME] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BANK_NAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BANK_BRANCH] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BANK_BRANCH].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BANK_CITY] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BANK_CITY].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.CATEGORY_ID] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.CATEGORY_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.SERVICE_ID] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.SERVICE_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.TRAN_TYPE] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.TRAN_TYPE].ToString()) + Config.COL_REC_DLMT;

                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM1] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM1].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM2] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM2].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM3] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM3].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM4] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM4].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM5] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM5].ToString()) + Config.COL_REC_DLMT;
                        
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM6] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM6].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM7] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM7].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM8] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM8].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[j][TBL_EB_BEN.BM9] == DBNull.Value ? "_NULL_" : dt.Rows[j][TBL_EB_BEN.BM9].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET BENLIST SUCCESSFUL");
                    retStr = retStr.Replace("{TRAN_TYPE}", trantype);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    return retStr;

                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        /// <summary>
        /// Check cif pilot
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        public bool checkCifPilot(string custId)
        {
            //dung cho pilot
            String deploymentState = Funcs.getConfigVal("DEPLOYMENT_STATE");
            if (deploymentState.Equals("pilot"))
            {

                String cifPilot = Funcs.getConfigVal("CIF_PILOT");
                String[] cifs = cifPilot.Split(',');
                if (!cifs.Contains(custId))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get tran type for pilot
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="tranType"></param>
        /// <returns></returns>
        public string getTranTypePilot(string custId, string tranType)
        {
            try
            {
                if (Config.TRAN_TYPE_DOMESTIC.Equals(tranType)
                    || Config.TRAN_TYPE_ACQ_247AC.Equals(tranType))
                {
                    bool isCifPilot = checkCifPilot(custId);
                    if (isCifPilot)
                    {
                        tranType = Config.TRAN_TYPE_DOMESTIC_ACC;
                    }
                }
            } catch (Exception ex)
            {
                Funcs.WriteLog("custid:" + custId + "|GET TRANTYPE FOR PILOT FAIL");
            }
            return tranType;
        }

        public string INSERT_BEN(Hashtable hashTbl, string ip, string user_agent)
        {
            
            //string retStr = Confi
            string retStr = Config.HANDLE_BENLIST;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string acct = Funcs.NoHack( Funcs.getValFromHashtbl(hashTbl, "ACCTNO"));
            string acct_name = Funcs.NoHack( Funcs.getValFromHashtbl(hashTbl, "ACCTNAME"));

            //gan lai acct name
            acct_name = (acct_name == Config.NULL_VALUE ? "" : acct_name);
            string acct_nick = Funcs.NoHack ( Funcs.getValFromHashtbl(hashTbl, "ACCT_NICK"));
            //gan lai des name
            acct_nick = (acct_nick == Config.NULL_VALUE ? "" : acct_nick);

            string default_txdesc = Funcs.NoHack ( Funcs.getValFromHashtbl(hashTbl, "DEFAULT_TXDESC"));
            string bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");
            string bank_name = Funcs.getValFromHashtbl(hashTbl, "BANK_NAME");
            string bank_branch = Funcs.getValFromHashtbl(hashTbl, "BANK_BRANCH");
            string bank_city = Funcs.getValFromHashtbl(hashTbl, "BANK_CITY");
            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");
            
            string bm1 = Funcs.getValFromHashtbl(hashTbl, "BM1");
            string bm2 = Funcs.getValFromHashtbl(hashTbl, "BM2");
            string bm3 = Funcs.getValFromHashtbl(hashTbl, "BM3");
            string bm4 = Funcs.getValFromHashtbl(hashTbl, "BM4");
            string bm5 = Funcs.getValFromHashtbl(hashTbl, "BM5");
            string bm6 = Funcs.getValFromHashtbl(hashTbl, "BM6");
            string bm7 = Funcs.getValFromHashtbl(hashTbl, "BM7");
            string bm8 = Funcs.getValFromHashtbl(hashTbl, "BM8");
            string bm9 = Funcs.getValFromHashtbl(hashTbl, "BM9");
            string bm10 = Funcs.getValFromHashtbl(hashTbl, "BM10");


            //neu  gia tri truyen len la _NULL_
            acct_nick = (acct_nick == Config.NULL_VALUE ? "" : acct_nick);
            default_txdesc = (default_txdesc == Config.NULL_VALUE ? "" : default_txdesc);
            bank_code = (bank_code == Config.NULL_VALUE ? "" : bank_code);
            bank_name = (bank_name == Config.NULL_VALUE ? "" : bank_name);
            bank_city = (bank_city == Config.NULL_VALUE ? "" : bank_city);
            category_id = (category_id == Config.NULL_VALUE ? "" : category_id);
            service_id = (service_id == Config.NULL_VALUE ? "" : service_id);

            bm1 = (bm1 == Config.NULL_VALUE ? "" : bm1);
            bm2 = (bm1 == Config.NULL_VALUE ? "" : bm2);
            bm3 = (bm1 == Config.NULL_VALUE ? "" : bm3);
            bm4 = (bm1 == Config.NULL_VALUE ? "" : bm4);
            bm5 = (bm1 == Config.NULL_VALUE ? "" : bm5);
            bm6 = (bm1 == Config.NULL_VALUE ? "" : bm6);
            bm7 = (bm1 == Config.NULL_VALUE ? "" : bm7);

            bm8 = Config.ChannelID;
            bm9 = ip ;
            bm10 = user_agent;

            Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN BEGIN");

            if ( (custid == "") || acct =="")
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN  INSERT FAILED|ACCTNO IS NULL");
                return retStr;
            }

            //nếu category_code = 'MOBILE' --> goi ham lay service id

            if (category_id == "MOBILE")
            {
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN  CATEGORY=MOBILE|FIND SERVICE_ID");

                //tran_type = Config.TRAN_TYPE_BILL_MOBILE;
                Payments pms = new Payments();
                DataTable dt1 = new DataTable();

                dt1 = pms.getServicePartner_byBill(acct, tran_type);

                if (dt1 != null && dt1.Rows.Count == 1)
                {
                    service_id = dt1.Rows[0][PAY_SER_PART.SERVICE_ID].ToString();
                }
                else
                {
                    return Config.ERR_MSG_GENERAL;
                }
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN  CATEGORY:MOBILE|SERVICE_ID:" +service_id);
            }

            
            try
            {
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN  INSERT");

                Beneficiarys da = new Beneficiarys();
                DataTable dt = new DataTable();
                dt = da.INSERT_BEN(custid
                                 , getTranTypePilot(custid, tran_type)
                                 , acct
                                 , acct_name
                                 , acct_nick
                                 , default_txdesc
                                 , bank_code
                                 , bank_name
                                 , bank_branch
                                 , bank_city
                                 , category_id
                                 , service_id
                                 , ""
                                 , bm1
                                 , bm2
                                 , bm3
                                 , bm4
                                 , bm5
                                 , bm6
                                 , bm7
                                 , bm8
                                 , bm9
                                 , bm10);

               if(dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "INSERT BENLIST SUCCESSFUL");
                    retStr = retStr.Replace("{TRAN_TYPE}", tran_type);
                    Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN  INSERT DONE");
                }
                else
               {
                   
                   retStr = Config.ERR_MSG_GENERAL;
                   Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN  INSERT FAILED");

               }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST ADDNEW BEN  INSERT FAILED EXCEPTION");
                return retStr;
            }
            return retStr;
        }

        public string DELETE_BEN(Hashtable hashTbl)
        {
            
            //string retStr = Confi
            string retStr = Config.HANDLE_BENLIST;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string acct = Funcs.getValFromHashtbl(hashTbl, "ACCTNO");
            string bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");

            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");

            if ((custid == "") || acct == "")
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST DELETE BEN  FAILED|ACCTNO IS NULL");
                return retStr;
            }


            try
            {
                Beneficiarys da = new Beneficiarys();
                DataTable dt = new DataTable();
                dt = da.DEL_BEN_CUSTID_TRANTYPE(custid, getTranTypePilot(custid, tran_type), acct, category_id, service_id, bank_code);

                if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "DELETE BENLIST SUCCESSFUL");
                    retStr = retStr.Replace("{TRAN_TYPE}", "HANDLE_BENLIST");
                    Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST DELETE BEN END DONE");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST DELETE BEN END FAILED");
                    return retStr;
                 
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST DELETE BEN END FAILED EXCEPTION");
                return retStr;
            }
            return retStr;
        }

        public string UPDATE_BEN(Hashtable hashTbl, string ip, string user_agent)
        {
            //string retStr = Confi
            string retStr = Config.HANDLE_BENLIST;
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string acct = Funcs.NoHack( Funcs.getValFromHashtbl(hashTbl, "ACCTNO"));

            string acct_name = Funcs.NoHack( Funcs.getValFromHashtbl(hashTbl, "ACCTNAME"));

            //gan lai acct name
            acct_name = (acct_name == Config.NULL_VALUE ? "" : acct_name);


            string acct_nick = Funcs.NoHack( Funcs.getValFromHashtbl(hashTbl, "ACCT_NICK"));

            //gan lai acct nick
            acct_nick = (acct_nick == Config.NULL_VALUE ? "" : acct_nick);


            string default_txdesc = Funcs.NoHack( Funcs.getValFromHashtbl(hashTbl, "DEFAULT_TXDESC"));

            string bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");
            string bank_name = Funcs.getValFromHashtbl(hashTbl, "BANK_NAME");
            string bank_branch = Funcs.getValFromHashtbl(hashTbl, "BANK_BRANCH");
            string bank_city = Funcs.getValFromHashtbl(hashTbl, "BANK_CITY");
            string category_id = Funcs.getValFromHashtbl(hashTbl, "CATEGORY_ID");
            string service_id = Funcs.getValFromHashtbl(hashTbl, "SERVICE_ID");
            string bm1 = Funcs.getValFromHashtbl(hashTbl, "BM1");
            string bm2 = Funcs.getValFromHashtbl(hashTbl, "BM2");
            string bm3 = Funcs.getValFromHashtbl(hashTbl, "BM3");
            string bm4 = Funcs.getValFromHashtbl(hashTbl, "BM4");
            string bm5 = Funcs.getValFromHashtbl(hashTbl, "BM5");
            string bm6 = Funcs.getValFromHashtbl(hashTbl, "BM6");
            string bm7 = Funcs.getValFromHashtbl(hashTbl, "BM7");
            string bm8 = Funcs.getValFromHashtbl(hashTbl, "BM8");
            string bm9 = Funcs.getValFromHashtbl(hashTbl, "BM9");
            string bm10 = Funcs.getValFromHashtbl(hashTbl, "BM10");

            //neu  gia tri truyen len la _NULL_
            acct_nick = (acct_nick == Config.NULL_VALUE ? "" : acct_nick);
            default_txdesc = (default_txdesc == Config.NULL_VALUE ? "" : default_txdesc);
            bank_code = (bank_code == Config.NULL_VALUE ? "" : bank_code);
            bank_name = (bank_name == Config.NULL_VALUE ? "" : bank_name);
            bank_city = (bank_city == Config.NULL_VALUE ? "" : bank_city);
            category_id = (category_id == Config.NULL_VALUE ? "" : category_id);
            service_id = (service_id == Config.NULL_VALUE ? "" : service_id);

            bm1 = (bm1 == Config.NULL_VALUE ? "" : bm1);
            bm2 = (bm1 == Config.NULL_VALUE ? "" : bm2);
            bm3 = (bm1 == Config.NULL_VALUE ? "" : bm3);
            bm4 = (bm1 == Config.NULL_VALUE ? "" : bm4);
            bm5 = (bm1 == Config.NULL_VALUE ? "" : bm5);
            bm6 = (bm1 == Config.NULL_VALUE ? "" : bm6);
            bm7 = (bm1 == Config.NULL_VALUE ? "" : bm7);

            bm8 = Config.ChannelID;
            bm9 = ip;
            bm10 = user_agent;

            Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST UPDATE BEN BEGIN");

            if ((custid == "") || acct == "")
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST UPDATE BEN  FAILED|ACCTNO IS NULL");
                return retStr;
            }

            try
            {
                Beneficiarys da = new Beneficiarys();
                DataTable dt = new DataTable();
                dt = da.UPDATE_BEN(custid
                                 , getTranTypePilot(custid, tran_type)
                                 , acct
                                 , acct_name
                                 , acct_nick
                                 , default_txdesc
                                 , bank_code
                                 , bank_name
                                 , bank_branch
                                 , bank_city
                                 , category_id
                                 , service_id
                                 , ""
                                 , bm1
                                 , bm2
                                 , bm3
                                 , bm4
                                 , bm5
                                 , bm6
                                 , bm7
                                 , bm8
                                 , bm9
                                 , bm10);

                if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "UPDATE BENLIST SUCCESSFUL");
                    retStr = retStr.Replace("{TRAN_TYPE}", tran_type);
                    Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST UPDATE BEN END DONE");
                    return retStr;

                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST UPDATE BEN END FAILED");
                    return retStr;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("custid:" + custid + "|HANDLE BENLIST UPDATE BEN END FAILED EXCEPTION");
                return retStr;
            }
           
        }
   }
}