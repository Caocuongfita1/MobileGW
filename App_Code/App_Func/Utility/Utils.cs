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

using System.Net;
using System.Xml.Linq;
using System.Net.Mail;
using System.IO;
using System.Web.Script.Serialization;
using System.Linq;
using iBanking.CustomModels;

/// <summary>
/// Summary description for Utils
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class Utils
    {
        public Utils()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string GET_CUR_VER(String type)
        {
            #region "GET_CUR_VER"
            string retStr = Config.GET_CUR_VER;
            try
            {

                //TYPE = MBJ, MBA, MBI
                SysVar sysvar = new SysVar();
                SysVarData sysvardata = sysvar.getSysVar(type);
                sysvar = null;
                if (sysvardata != null)
                {
                    retStr = retStr.Replace("{CUR_VER}", sysvardata.Tables[SysVarData.SYSVAR_TABLE].Rows[0][SysVarData.SYSVARDESC_FIELD].ToString());
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "GET CURRENT VERSION SUCCESSFULL");
                    //ADD NEW image
                    retStr = retStr.Replace("{COUNT_IMG}", Funcs.getConfigVal("COUNT_IMG"));
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }

                //giai phong bo nho: 
                if (sysvardata != null) sysvardata.Dispose();
                sysvar = null;

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }

            return retStr;
            #endregion "GET_CUR_VER"
        }

        public string GetLimitTransactionBy(String custid, String channel_id, String tran_type)
        {
            string ret = Config.GET_LIMIT_TRANSACTION_AMOUNT;
            List<TBL_EB_LMT_PER_TRAN> lst = new List<TBL_EB_LMT_PER_TRAN>();
            try
            {
                var dynParams = new iBanking.Common.OracleDynamicParameters();
                dynParams.Add("PCUSTID", custid, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("PCHANNEL_ID", channel_id, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("PTRAN_TYPE", tran_type, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("OUT_CUR", dbType: Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, direction: ParameterDirection.Output);
                lst = (List<TBL_EB_LMT_PER_TRAN>)new ConnectionFactory(Config.gEBANKConnstr)
                .GetItems<TBL_EB_LMT_PER_TRAN>(CommandType.StoredProcedure, "PKG_LIMIT.GET_LIMIT_INFO_V2", dynParams);
                if (lst.Count > 0)
                {
                    ret = ret.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    ret = ret.Replace("{ERR_DESC}", "GET LIMIT TRANSACTION AMOUNT SUCCESSFUL");
                    ret = ret.Replace("{LIMIT_247AC}", lst[0].LIMIT_PER_TRAN1);
                    ret = ret.Replace("{LIMIT_247CARD}", lst[0].LIMIT_PER_TRAN2);
                    ret = ret.Replace("{LIMIT_DOMESTIC}", lst[0].LIMIT_PER_TRAN3);
                    ret = ret.Replace("{REMAIN_LIMIT_PER_DAY}", lst[0].REMAIN_LIMIT_PER_DAY);
                }
                else
                {
                    ret = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("GetLimitTransactionBy-" + custid + "-" + ex.ToString());
                ret = Config.ERR_MSG_GENERAL;
            }
            return ret;
        }


        public string GetLimitTransactionByV2(String custid, String channel_id, String tran_type)
        {
            string ret = Config.GET_LIMIT_TRANSACTION_AMOUNT_V2;
            List<TBL_EB_LMT_PER_TRAN> lst = new List<TBL_EB_LMT_PER_TRAN>();
            try
            {
                var dynParams = new iBanking.Common.OracleDynamicParameters();
                dynParams.Add("PCUSTID", custid, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("PCHANNEL_ID", channel_id, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("PTRAN_TYPE", tran_type, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("OUT_CUR", dbType: Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, direction: ParameterDirection.Output);
                lst = (List<TBL_EB_LMT_PER_TRAN>)new ConnectionFactory(Config.gEBANKConnstr)
                .GetItems<TBL_EB_LMT_PER_TRAN>(CommandType.StoredProcedure, "PKG_LIMIT.GET_LIMIT_INFO_V2", dynParams);
                if (lst.Count > 0)
                {
                    ret = ret.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    ret = ret.Replace("{ERR_DESC}", "GET LIMIT TRANSACTION AMOUNT SUCCESSFUL");
                    ret = ret.Replace("{LIMIT_247AC}", lst[0].LIMIT_PER_TRAN1);
                    ret = ret.Replace("{LIMIT_247CARD}", lst[0].LIMIT_PER_TRAN2);
                    ret = ret.Replace("{LIMIT_DOMESTIC}", lst[0].LIMIT_PER_TRAN3);
                    ret = ret.Replace("{REMAIN_LIMIT_PER_DAY}", lst[0].REMAIN_LIMIT_PER_DAY);
                    ret = ret.Replace("{REG_USER}", lst[0].REG_USER);
                    ret = ret.Replace("{LIMIT_AMT_MONTH}", lst[0].LIMIT_AMT_MONTH);
                }
                else
                {
                    ret = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("GetLimitTransactionBy-" + custid + "-" + ex.ToString());
                ret = Config.ERR_MSG_GENERAL;
            }
            return ret;
        }
        public string UPDATE_LANGUE_BY_USER(String mob_user, String lang)
        {
            #region "UPDATE_LANGUE_BY_USER"
            string retStr = "";
            try
            {

                DataSet ds = new DataSet();
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                ds = da.UPDATE_LANGUE_BY_USER(mob_user, lang);
                if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                {
                    string err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                    if (err == Config.ERR_CODE_DONE)
                    {
                        retStr = Config.SUCCESS_MSG_GENERAL;
                    }
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                    }
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
                //giai phong bo nho: 
                if (ds != null) ds.Dispose();
                da = null;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
            return retStr;
            #endregion "UPDATE_LANGUE_BY_USER"
        }

        public string UPDATE_AVATAR_BY_USER(String mob_user, String avatar)
        {
            #region "UPDATE_AVATAR_BY_USER"
            string retStr = "";
            try
            {

                DataSet ds = new DataSet();
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                ds = da.UPDATE_AVATAR_BY_USER(mob_user, avatar);

                if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                {
                    string err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                    if (err == Config.ERR_CODE_DONE)
                    {
                        retStr = Config.SUCCESS_MSG_GENERAL;
                    }
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                    }
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
                //giai phong bo nho: 
                if (ds != null) ds.Dispose();
                da = null;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
            return retStr;
            #endregion "UPDATE_AVATAR_BY_USER"
        }

        public string UPDATE_FAV_MENU_BY_USER(String mob_user, String fav_menu)
        {
            #region "UPDATE_FAV_MENU_BY_USER"
            string retStr = "";
            string err = "";
            try
            {
                DataSet ds = new DataSet();
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                ds = da.UPDATE_FAV_MENU_BY_USER(mob_user, fav_menu);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();

                    if (err == Config.ERR_CODE_DONE)
                    {
                        retStr = Config.SUCCESS_MSG_GENERAL;
                    }
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                    }
                }
                //giai phong bo nho: 
                if (ds != null) ds.Dispose();
                da = null;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }

            return retStr;
            #endregion "UPDATE_FAV_MENU_BY_USER"
        }


        public string GET_BANK_CODE_CITAD()
        {
            #region "GET_BANK_CODE_CITAD"
            string retStr = Config.GET_BANK_CODE_CITAD;

            try
            {
                DataTable dt = new DataTable();
                Utility util = new Utility();
                dt = util.GET_BANK_CODE_CITAD();
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strTemp = strTemp + (dt.Rows[i][TBL_EB_TRAN.BANK_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TBL_EB_TRAN.BANK_CODE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][TBL_EB_TRAN.BANK_NAME] == DBNull.Value ? "_NULL_" : dt.Rows[i][TBL_EB_TRAN.BANK_NAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i]["BANK_NAME_EN"] == DBNull.Value ? "_NULL_" : dt.Rows[i]["BANK_NAME_EN"].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET BANK_CODE SUCCESSFUL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;
            #endregion "GET_BANK_CODE_CITAD"
        }

        public string GET_CITY_CITAD(string bank_code)
        {
            #region "GET_CITY_CITAD"
            string retStr = Config.GET_CITY_CITAD;

            try
            {
                DataTable dt = new DataTable();
                Utility GET_CITY_CITAD = new Utility();
                dt = GET_CITY_CITAD.GET_CITY_CITAD(bank_code);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strTemp = strTemp + (dt.Rows[i]["CITY_CODE"] == DBNull.Value ? "_NULL_" : dt.Rows[i]["CITY_CODE"].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i]["CITY_DESC"] == DBNull.Value ? "_NULL_" : dt.Rows[i]["CITY_DESC"].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET BANK_CITY SUCCESSFUL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;
            #endregion "GET_CITY_CITAD"
        }

        public string GET_BANK_BRANCH_CITAD(string bank_code, string city_code)
        {
            #region "GET_BANK_BRANCH_CITAD"
            string retStr = Config.GET_BANK_BRANCH_CITAD;

            try
            {
                DataTable dt = new DataTable();
                Utility util = new Utility();
                dt = util.GET_BANK_BRANCH_CITAD(bank_code, city_code);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strTemp = strTemp + (dt.Rows[i]["BRANCH_CODE"] == DBNull.Value ? "_NULL_" : dt.Rows[i]["BRANCH_CODE"].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i]["BRANCH_NAME"] == DBNull.Value ? "_NULL_" : dt.Rows[i]["BRANCH_NAME"].ToString()) + Config.COL_REC_DLMT;
                        //linhtn hot fix 2017 01 06 chinh lai message tra ve bi nham ROW_REC_DLMT
                        strTemp = strTemp + (dt.Rows[i]["BANK_CODE"] == DBNull.Value ? "_NULL_" : dt.Rows[i]["BANK_CODE"].ToString()) + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET BANK_BRANCH SUCCESSFUL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;
            #endregion "GET_CITY_CITAD"
        }

        public string GET_BANK_CODE_247_AC2AC(System.Collections.Hashtable hash)
        {
            string retStr = Config.GET_BANK_CODE_247_AC2AC;
            try
            {
                DataTable dt = new DataTable();
                Utility util = new Utility();
                dt = util.GET_BANK_CODE_247_AC2AC();
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strTemp = strTemp + (dt.Rows[i][TBL_EB_TRAN.BANK_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TBL_EB_TRAN.BANK_CODE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][TBL_EB_TRAN.BANK_NAME] == DBNull.Value ? "_NULL_" : dt.Rows[i][TBL_EB_TRAN.BANK_NAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i]["BANK_NAME_EN"] == DBNull.Value ? "_NULL_" : dt.Rows[i]["BANK_NAME_EN"].ToString()) + Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET 247 BANK_CODE SUCCESSFUL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;
        }

        public string GET_SERVICES(System.Collections.Hashtable hashTbl)
        {
            string cif_no = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string retStr = "";

            try
            {
                DataTable dt = new DataTable();
                Utility util = new Utility();
                dt = util.GET_SERVICES();
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //SERVICE_ID|SERVICE_NAME|SERVICE_NAME_EN|SERVICE_DESC|PAYCODE_LBL|PAYCODE_LBL_EN|PARTNER_ID|SERVICE_CODE|CATEGORY_ID|CATEGORY_CODE|BM1|BM2|BM3|BM4|BM5|BM6|BM7|BM8|BM9

                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_ID] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_NAME] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_NAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_NAME_EN] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_NAME_EN].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_DESC] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_DESC].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.PAYCODE_LBL] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.PAYCODE_LBL].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.PAYCODE_LBL_EN] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.PAYCODE_LBL_EN].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SER_PART.PARTNER_ID] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SER_PART.PARTNER_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.SERVICE_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.SERVICE_CODE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.CATEGORY_ID] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.CATEGORY_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SER_PART.CATEGORY_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SER_PART.CATEGORY_CODE].ToString()) + Config.COL_REC_DLMT;

                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM1] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM1].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM2] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM2].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM3] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM3].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM4] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM4].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM5] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM5].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM6] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM6].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM7] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM7].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM8] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM8].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_SERVICE.BM9] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_SERVICE.BM9].ToString()) + Config.COL_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET 247 BANK_CODE SUCCESSFUL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;
        }


        public string GET_CATEGORY_BY_TRAN_TYPE(System.Collections.Hashtable hashTbl)
        {
            string cif_no = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string retStr = "";

            retStr = Config.GET_CATEGORY_BY_TRAN_TYPE;

            try
            {
                DataTable dt = new DataTable();
                Utility util = new Utility();
                dt = util.GET_CATEGORY_BY_TRAN_TYPE(tran_type);
                if (dt != null && dt.Rows.Count > 0)
                {


                    string strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //CATEGORY_ID|CATEGORY_NAME|CATEGORY_NAME_EN|CATEGORY_DESC|BM1|BM2|BM3|BM4|BM5|BM6|BM7|BM8|BM9

                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.CATEGORY_ID] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.CATEGORY_ID].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.CATEGORY_NAME] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.CATEGORY_NAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.CATEGORY_NAME_EN] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.CATEGORY_NAME_EN].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.CATEGORY_DESC] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.CATEGORY_DESC].ToString()) + Config.COL_REC_DLMT;

                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM1] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM1].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM2] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM2].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM3] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM3].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM4] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM4].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM5] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM5].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM6] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM6].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM7] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM7].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM8] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM8].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][PAY_CATEGORY.BM9] == DBNull.Value ? "_NULL_" : dt.Rows[i][PAY_CATEGORY.BM9].ToString()) + Config.COL_REC_DLMT
                        + Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{TRAN_TYPE}", tran_type);
                    retStr = retStr.Replace("{ERR_DESC}", "GET 247 BANK_CODE SUCCESSFUL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;
        }


        #region "STOCK"
        /// <summary>
        /// Lấy danh sách công ty chứng khoán
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public string GET_STOCK_BRANCH_LIST(Hashtable hashTbl)
        {

            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");
            string branch_code = "%";

            string retStr = "";
            string strTemp = "";

            retStr = Config.GET_STOCK_BRANCH_LIST;

            try
            {
                DataTable dt = new DataTable();
                Utility ut = new Utility();

                dt = ut.GET_STOCK_BRANCH_LIST(tran_type, branch_code);

                if (dt != null && dt.Rows.Count > 0)
                {
                    strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //BRANCH_CODE^BRANCH_NAME^BRANCH_NAME_EN

                        strTemp = strTemp + (dt.Rows[i][STOCK_DATA.BRANCH_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][STOCK_DATA.BRANCH_CODE]) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][STOCK_DATA.ACCTNAME] == DBNull.Value ? "_NULL_" : dt.Rows[i][STOCK_DATA.ACCTNAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][STOCK_DATA.ACCTNAME_EN] == DBNull.Value ? "_NULL_" : dt.Rows[i][STOCK_DATA.ACCTNAME_EN].ToString()) + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                        ;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{TRAN_TYPE}", tran_type);
                    retStr = retStr.Replace("{ERR_DESC}", "GET LIST STOCK BRANCH SUCCESSFULL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());

                retStr = Config.ERR_MSG_GENERAL;

            }


            /*
             public static String GET_STOCK_BRANCH_LIST= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

Cấu trúc của RECORD: 

BRANCH_CODE^BRANCH_NAME^BRANCH_NAME_EN

             */

            return retStr;

        }

        // them 1 ham o lop util từ tên cty chứng khoán, mã chi nhánh công ty chứng khoán --> số tài khoản tương ứng với chi nhánh để hạch toán


        //CMD#STOCK_TRANSFER|CIF_NO#0310008705|SRC_ACCT#1000013376|DES_ACCT#069C000886.00|DES_NAME#NGUYEN VAN A|AMOUNT#30000000|TRAN_TYPE#TOPUP_SHS|BRANCH_CODE#HANOI|BRANCH_NAME#HA NOI|TRANPWD#fksdfjf385738jsdfjsdf9| SAVE_TO_BENLIST#1|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
        public string STOCK_TRANSFER(Hashtable hashTbl, string ip, string user_agent)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");

            double amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");



            string branch_code = Funcs.getValFromHashtbl(hashTbl, "BRANCH_CODE");
            string branch_name = Funcs.getValFromHashtbl(hashTbl, "BRANCH_NAME");
            string sponsor_name = Funcs.getValFromHashtbl(hashTbl, "SPONSOR_NAME");

            string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

            string des_acct_customer = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");


            string retStr = "";
            string des_acct = "";
            string check_before_trans = "";

            string tran_type = "";
            double eb_tran_id = 0;
            string core_txno_ref = "";
            string core_txdate_ref = "";
            string channel_id = Config.ChannelID;

            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME"); //DES_NAME
            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");
            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion
            tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            //fix txdesc
            txdesc = des_name + " NAP TIEN CHUNG KHOAN " + des_acct_customer;



            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            //bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            //if (!check)
            //{
            //    return Config.ERR_MSG_GENERAL;
            //}

            bool check = CoreIntegration.CheckAccountBelongCif(custid, src_acct, "CASA");
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }


            //1.kiem tra thong tin mat khau giao dich (khong check han muc)

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            if (check_before_trans == Config.ERR_CODE_DONE)
            {

                //2.lay so tai khoan quy tu thien tu ma quy tu thien (CHARITY_CODE)
                DataTable dt = new DataTable();
                Utility ut = new Utility();

                dt = ut.GET_STOCK_BRANCH_LIST(tran_type, branch_code);

                if (dt != null && dt.Rows.Count == 1)
                {
                    des_acct = dt.Rows[0][STOCK_DATA.ACCTNO].ToString();
                }
                else
                {
                    return Config.ERR_MSG_GENERAL;
                }


                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"

                //3.Cap nhat vao bang tbl_eb_ben (tran_type = CHARITY). SPONSOR --> BM1.
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txdesc //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , typeOtp //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , "" //sms code
                    , "" //sign data
                    , "" //core err code
                    , "" //core err desc
                    , "" //core ref_no
                    , "" //core txdate
                    , "" //core txtime
                    , 0 // order_amount
                    , 0 // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , des_acct_customer //bm1
                    , "" //bm2
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , "" //bm7
                    , "" //bm8
                    , "" //bm9
                    , "" //bm10
                    , "" //bm11
                    , "" //bm12
                    , "" //bm13
                    , "" //bm14
                    , "" //bm15
                    , "" //bm16
                    , "" //bm17
                    , "" //bm18
                    , "" //bm19
                    , "" //bm20
                    , "" //bm21
                    , "" //bm22
                    , "" //bm23
                    , "" //bm24
                    , "" //bm25
                    , "" //bm26
                    , requestId //bm27
                    , ip //"" //bm28
                    , user_agent//""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {

                    //4. Hach toan vao core (tuong tu nhu chuyen khoan trong SHB).

                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    retStr = Config.FUNDTRANSFER_INTRA;
                    //string result = tf.pstTransderTx(
                    string result = CoreIntegration.pstTransderTx(
                                  custid
                                  , eb_tran_id
                                  , src_acct
                                  , des_acct
                                  , amount
                                  , 0
                                  , txdesc
                                  , ref core_txno_ref
                                  , ref core_txdate_ref);
                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {

                        // cap nhat trang thai giao dich thanh cong tbl_eb_tran
                        tf.uptTransferTx(eb_tran_id
                           , Config.TX_STATUS_DONE
                           , core_txno_ref
                           , core_txdate_ref
                           , channel_id);


                        //5. Save to ben list

                        //SAVE TO BEN LIST
                        if (save_to_benlist == "0")
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "STOCK TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                        }
                        else if (save_to_benlist == "1")
                        {
                            Beneficiarys ben = new Beneficiarys();
                            DataTable dt1 = new DataTable();
                            dt1 = ben.INSERT_BEN(
                                custid
                                , tran_type
                                , des_acct_customer //des_acct
                                , des_name
                                , des_name
                                , txdesc
                                , ""//bank_code
                                , ""//bank_name
                                , branch_code// ""//bank_branch
                                , ""//bank_city
                                , "" //category_id
                                , "" //service_id
                                , "" //lastchange default = sysdate da xu ly o db
                                , ""// bm1 
                                , ""// bm2
                                , ""// bm3
                                , ""// bm4
                                , ""// bm5
                                , ""// bm6
                                , ""// bm7
                                , Config.ChannelID// ""// bm8
                                , ip // bm9
                                , user_agent// ""// bm10
                                );

                            if (dt1 != null && dt1.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "STOCK TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id + " SAVE TO BENLIST DONE");

                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                            }
                            else
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "STOCK TRANSFER IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);
                            }
                            //giai phong bo nho                            
                            ben = null;
                            dt = null;
                        }
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {
                        tf.uptTransferTx(eb_tran_id
                          , Config.TX_STATUS_FAIL
                          , core_txno_ref
                          , core_txdate_ref
                          , channel_id);

                        retStr = Config.CD_EB_TRANS_ERR_GENERAL;
                    }
                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                }
            }
            else
            {
                retStr = check_before_trans;
            }
            return retStr;

        }

        #endregion "STOCK"
        /*
REQ=CMD#GET_STOCK_BRANCH_LIST|CIF_NO#0310005018|STOCK_COMPANY#SHS|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

Response

public static String GET_STOCK_BRANCH_LIST= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
        + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

Cấu trúc của RECORD: 

BRANCH_CODE^BRANCH_NAME
     */

        #region "CHARITY"
        /*
       Request

"REQ=CMD#GET_CHARITY_LIST|CIF_NO#0310005018|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";


Response

public static String GET_CHARITY_LIST= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
			+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}";

Cấu trúc của RECORD: 

CHARITY_CODE^ CHARITY_NAME ^ CHARITY_NAME_EN

    */

        public string GET_CHARITY_LIST(Hashtable hashTbl)
        {

            string charity_code = "%";
            string retStr = "";
            string strTemp = "";
            retStr = Config.GET_CHARITY_LIST;
            try
            {
                DataTable dt = new DataTable();
                Utility ut = new Utility();

                dt = ut.GET_CHARITY_LIST(charity_code);

                if (dt != null && dt.Rows.Count > 0)
                {
                    strTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //CHARITY_CODE^ CHARITY_NAME ^ CHARITY_NAME_EN

                        strTemp = strTemp + (dt.Rows[i][CHARITY_DATA.CHARITY_CODE] == DBNull.Value ? "_NULL_" : dt.Rows[i][CHARITY_DATA.CHARITY_CODE]) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][CHARITY_DATA.ACCTNAME] == DBNull.Value ? "_NULL_" : dt.Rows[i][CHARITY_DATA.ACCTNAME].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + (dt.Rows[i][CHARITY_DATA.ACCTNAME_EN] == DBNull.Value ? "_NULL_" : dt.Rows[i][CHARITY_DATA.ACCTNAME_EN].ToString()) + Config.COL_REC_DLMT
                            + Config.ROW_REC_DLMT;
                        ;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{TRAN_TYPE}", charity_code);
                    retStr = retStr.Replace("{ERR_DESC}", "GET LIST CHARITY LIST SUCCESSFULL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());

                retStr = Config.ERR_MSG_GENERAL;

            }
            return retStr;

        }



        /*
       REQ=CMD#CHARITY_TRANSFER|CIF_NO#0310008705|SRC_ACCT#1000013376|CHARITY_ACCT#1000010000|AMOUNT#30000000|SPONSOR#ONG NGUYEN VAN A|TXDESC#DIEN GIAI GIAO DICH|TRANPWD#fksdfjf385738jsdfjsdf9| SAVE_TO_BENLIST#1|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";


Response

public static String CHARITY_TRANSFER= "ERR_CODE" + COL_DLMT + "{ERR_CODE}" 
          + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
          + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
          + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
+ ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
       */
        public string CHARITY_TRANSFER(Hashtable hashTbl, string ip, string user_agent)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            double amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");


            string charity_code = Funcs.getValFromHashtbl(hashTbl, "CHARITY_CODE");
            string sponsor_name = Funcs.getValFromHashtbl(hashTbl, "SPONSOR");
            string txdesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

            string retStr = "";
            string des_acct = "";
            string check_before_trans = "";

            string tran_type = Config.TRAN_TYPE_CHARITY;
            double eb_tran_id = 0;
            string core_txno_ref = "";
            string core_txdate_ref = "";

            string des_name = Funcs.getValFromHashtbl(hashTbl, "DES_NAME"); //DES_NAME
            string save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");

            string channel_id = Config.ChannelID;

            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            //KIEM TRA TAI KHOAN MO SO THUOC CIF
            //bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
            //if (!check)
            //{
            //    return Config.ERR_MSG_GENERAL;
            //}

            bool check = CoreIntegration.CheckAccountBelongCif(custid, src_acct, "CASA");
            if (!check)
            {
                return Config.ERR_MSG_GENERAL;
            }

            //1.kiem tra thong tin mat khau giao dich (khong check han muc)

            #region FOR TOKEN
            switch (typeOtp)
            {
                case 2:
                    check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);
                    break;
                case 4:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                case 5:
                    check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                    break;
                default:
                    break;
            }
            #endregion

            if (check_before_trans == Config.ERR_CODE_DONE)
            {

                //2.lay so tai khoan quy tu thien tu ma quy tu thien (CHARITY_CODE)
                DataTable dt = new DataTable();
                Utility ut = new Utility();

                dt = ut.GET_CHARITY_LIST(charity_code);

                if (dt != null && dt.Rows.Count == 1)
                {
                    des_acct = dt.Rows[0][CHARITY_DATA.ACCTNO].ToString();
                }
                else
                {
                    return Config.ERR_MSG_GENERAL;
                }


                Transfers tf = new Transfers();
                DataTable eb_tran = new DataTable();

                //  eb_tran = transfer.insTransferTx
                #region "insert TBL_EB_TRAN"

                //3.Cap nhat vao bang tbl_eb_ben (tran_type = CHARITY). SPONSOR --> BM1.
                eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txdesc //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , typeOtp //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , "" //sms code
                    , "" //sign data
                    , "" //core err code
                    , "" //core err desc
                    , "" //core ref_no
                    , "" //core txdate
                    , "" //core txtime
                    , 0 // order_amount
                    , 0 // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , "" //bm1
                    , "" //bm2
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , "" //bm7
                    , "" //bm8
                    , "" //bm9
                    , "" //bm10
                    , "" //bm11
                    , "" //bm12
                    , "" //bm13
                    , "" //bm14
                    , "" //bm15
                    , "" //bm16
                    , "" //bm17
                    , "" //bm18
                    , "" //bm19
                    , "" //bm20
                    , "" //bm21
                    , "" //bm22
                    , "" //bm23
                    , "" //bm24
                    , "" //bm25
                    , "" //bm26
                    , requestId //bm27
                    , ip //"" //bm28
                    , user_agent //""//bm29                   
                );

                #endregion "insert TBL_EB_TRAN"
                //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                {


                    //4. Hach toan vao core (tuong tu nhu chuyen khoan trong SHB).

                    eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    retStr = Config.FUNDTRANSFER_INTRA;
                    //string result = tf.pstTransderTx(
                    string result = CoreIntegration.pstTransderTx(
                                  custid
                                  , eb_tran_id
                                  , src_acct
                                  , des_acct
                                  , amount
                                  , 0
                                  , txdesc
                                  , ref core_txno_ref
                                  , ref core_txdate_ref);
                    //NEU HACH TOAN VAO CORE THANH CONG
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {
                        tf.uptTransferTx(eb_tran_id
                            , Config.TX_STATUS_DONE
                            , core_txno_ref
                            , core_txdate_ref
                            , channel_id);


                        //5. Save to ben list

                        //SAVE TO BEN LIST
                        if (save_to_benlist == "0")
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "CHARITY TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                        }
                        else if (save_to_benlist == "1")
                        {
                            Beneficiarys ben = new Beneficiarys();
                            DataTable dt1 = new DataTable();
                            dt1 = ben.INSERT_BEN(
                                custid
                                , tran_type
                                , des_acct
                                , des_name
                                , des_name
                                , txdesc
                                , ""//bank_code
                                , ""//bank_name
                                , ""//bank_branch
                                , ""//bank_city
                                , "" //category_id
                                , "" //service_id
                                , "" //lastchange default = sysdate da xu ly o db
                                , ""// bm1
                                , ""// bm2
                                , ""// bm3
                                , ""// bm4
                                , ""// bm5
                                , ""// bm6
                                , ""// bm7
                                , Config.ChannelID // ""// bm8 -- luu channel ID
                                , ip // ""// bm9
                                , user_agent //""// bm10
                                );

                            if (dt1 != null && dt1.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CHARITY TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id + " SAVE TO BENLIST DONE");

                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);
                            }
                            else
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CHARITY TRANSFER IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);
                            }
                            //giai phong bo nho                            
                            ben = null;
                            dt = null;
                        }
                    }
                    //NEU HACH TOAN VAO CORE KHONG THANH CONG
                    else
                    {
                        tf.uptTransferTx(eb_tran_id
                          , Config.TX_STATUS_FAIL
                          , core_txno_ref
                          , core_txdate_ref
                          , channel_id);
                        retStr = Config.CD_EB_TRANS_ERR_GENERAL;
                    }
                }
                //KO INSERT VÀO EB_TRANS DC.
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");
                }
            }
            else
            {
                retStr = check_before_trans;
            }
            return retStr;

        }

        #endregion "CHARITY"


        #region "TIDE ONLINE"

        /*
         requestBody = " REQ=CMD#GET_ACCT_TIDE_OL_INFO_LIST|CIF_NO#0310008705|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
 

public static String GET_ACCT_TIDE_OL_INFO_LIST= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
+ ROW_DLMT + "TOTAL_AMOUNT" + COL_DLMT + "{TOTAL_AMOUNT}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"
;	

Trong đó:

TOTAL_AMOUNT: Tổng tiền gốc gửi quy đổi (VNĐ)

Cấu trúc RECORD:

ACCTNO ^ CCYCD ^ PRODUCT_CODE ^ CURR_PRIN_AMT ^ TENURE ^ UNIT_TENURE ^INT_RATE ^ VAL_DT ^ MAT_DT ^ CURR_MAT_AMT  ^ AUTO_REN_NO

Trong đó:
ACCTNO: Số tài khoản
CCYCD: loại tiền
PRODUCT_CODE: Mã sản phẩm → Server sẽ check nếu = 470 (tiết kiệm online) → trả về thêm biến “ALLOW_TIDEWDL” cho phép tất toán = 1
CCYCD: Loại tiền VND,….
CURR_PRIN_AMT: Gốc gửi
INT_RATE: Lãi suất
TENURE: kỳ hạn 
UNIT_TENURE:  đơn vị (M/W/D/Y)
MAT_DT: Ngày đến hạn
CURR_MAT_AMT: Lãi dự kiến

VAL_DT: Ngày mở (Nếu roll thì ngày ngày là ngày của kỳ mới) dd/MM/yyyy

AUTO_REN_NO: Lần đầu mở thì = 00, mỗi lần ROLL thì tăng lên 1

Check đúng hạn: VAL_DT = Ngày hiện tại, AUTO_REN_NO <> 00 → báo luôn cho khách hàng

         */

        public string GET_ACCT_TIDE_OL_INFO_LIST(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

            //string ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD");

            //1. lấy tổng tiền quy đổi (VND) sổ tiết kiệm online, danh sách sổ tiết kiệm online
            //acct_no = "%";
            //ccycd = "%";
            string retStr = "";
            string strTemp = "";
            string total_amount = "";
            retStr = Config.GET_ACCT_TIDE_OL_INFO_LIST;
            try
            {
                DataTable dt = new DataTable();
                Utility ut = new Utility();

                //dt = ut.GET_ACCT_TIDE_OL_INFO_LIST(custid, acct_no, ccycd);

                AccList.AcctListInqResType res = CoreIntegration.getAcctTideOlInfoList(custid, "004", "CIF");

                if (res != null && res.RespSts.Sts.Equals("0"))
                {
                    strTemp = "";

                    List<AccList.AcctRecType> listRec = new List<AccList.AcctRecType>(res.AcctRec);

                    if (listRec == null || listRec.Count < 1)
                    {
                        return Config.ERR_MSG_GENERAL;
                    }

                    foreach (var item in listRec)
                    {
                        //CCTNO ^ CCYCD ^ PRODUCT_CODE ^ CURR_PRIN_AMT ^ TENURE ^ UNIT_TENURE ^INT_RATE ^ VAL_DT ^ MAT_DT ^ CURR_MAT_AMT  ^ AUTO_REN_NO ^INT_AMT ^ DEPOSIT_NO

                        strTemp = strTemp + (string.IsNullOrEmpty(item.BankAcctId.AcctId) ? "_NULL_" : item.BankAcctId.AcctId) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.BankAcctId.AcctCur) ? "_NULL_" : item.BankAcctId.AcctCur)
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.ProdCD) ? "_NULL_" : item.ProdCD) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.AvailBal) ? "0" : item.AvailBal)
                            + Config.COL_REC_DLMT; 
                        strTemp = strTemp + (string.IsNullOrEmpty(item.Tenure) ? "_NULL_" : item.Tenure) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.UnitTenure) ? "_NULL_" : item.UnitTenure) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.IntRate) ? "_NULL_" : item.IntRate) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.ValDt) ? "_NULL_" : item.ValDt) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.MatDt) ? "_NULL_" : item.MatDt) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.MatAmt) ? "0" : item.MatAmt) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.AutoRenNo) ? "_NULL_" : item.AutoRenNo) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.IntAmt) ? "0" : item.IntAmt) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.DepositNo) ? "_NULL_" : item.DepositNo) 
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.Instruction) ? "_NULL_" : item.Instruction)
                            + Config.COL_REC_DLMT;
                        strTemp = strTemp + (string.IsNullOrEmpty(item.IsCoholder) ? "0" : item.IsCoholder)

                            + Config.ROW_REC_DLMT;
                        ;
                        
                        //total_amount = String.Format("{0:#,###0}", dt.Rows[0][TIDE_BALANCE.TOTAL]);

                    }

                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{TOTAL_AMOUNT}", total_amount);
                    retStr = retStr.Replace("{ERR_DESC}", "GET LIST TIDE ACCT SUCCESSFULL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());

                retStr = Config.ERR_MSG_GENERAL;

            }
            return retStr;

        }

        /*
msg="REQ=CMD#GET_TIDE_INTEREST_RATE|CIF_NO#0310005018|CCYCD#VND|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

Response:
public static String GET_TIDE_INTEREST_RATE= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"		
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"		

Trong đó:
 RECORD: CCYCD^TENURE^UNIT_TENURE^INT_RATE

         */

        #region "GET_TIDE_INTEREST_RATE OLD"
        //public string GET_TIDE_INTEREST_RATE(Hashtable hashTbl)
        //{
        //    string cif_no = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        //    string ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD"); //TRUYEN VAO CHU 704
        //    string tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE"); //TRUYEN VAO CHU VND
        //    string unit_tenure = Funcs.getValFromHashtbl(hashTbl, "UNIT_TENURE"); //TRUYEN VAO CHU VND

        //    ccycd = "704";
        //    tenure = "%";
        //    unit_tenure = "%";
        //    double amount = 0;


        //    string retStr = "";
        //    string strTemp = "";

        //    retStr = Config.GET_TIDE_INTEREST_RATE;
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        Utility ut = new Utility();

        //        dt = ut.getTIDERATE(ccycd, tenure, unit_tenure, amount);

        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            strTemp = "";
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                //    CCYCD^TENURE^UNIT_TENURE^INT_RATE

        //                //strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.CCY_CD] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.CCY_CD]) + Config.COL_REC_DLMT;
        //                strTemp = strTemp + "VND" + Config.COL_REC_DLMT;
        //                strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.TENURE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.TENURE].ToString()) + Config.COL_REC_DLMT;
        //                strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.UNIT_TENURE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.UNIT_TENURE].ToString()) + Config.COL_REC_DLMT;
        //                strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.RATE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.RATE].ToString()) + Config.COL_REC_DLMT
        //                        + Config.ROW_REC_DLMT;
        //                ;

        //            }
        //            strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

        //            retStr = retStr.Replace("{RECORD}", strTemp);
        //            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
        //            retStr = retStr.Replace("{ERR_DESC}", "GET GET_TIDE_INTEREST_RATE SUCCESSFULL");
        //        }
        //        else
        //        {
        //            retStr = Config.ERR_MSG_GENERAL;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Funcs.WriteLog(ex.ToString());

        //        retStr = Config.ERR_MSG_GENERAL;

        //    }
        //    return retStr;

        //}

        #endregion "GET_TIDE_INTEREST_RATE OLD"
        public string GET_TIDE_INTEREST_RATE(Hashtable hashTbl)
        {
            string cif_no = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD"); //TRUYEN VAO CHU 704
            string tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE"); //TRUYEN VAO CHU VND
            string unit_tenure = Funcs.getValFromHashtbl(hashTbl, "UNIT_TENURE"); //TRUYEN VAO CHU VND

            ccycd = "704";
            tenure = "%";
            unit_tenure = "%";
            double amount = 0;


            string retStr = "";
            string strTemp = "";

            retStr = Config.GET_TIDE_INTEREST_RATE;
            try
            {
                TideRate.TideRateInquiryResType res = CoreIntegration.getTideRate("*");

                if (res != null && res.TideRate != null)
                {
                    strTemp = "";
                    foreach (var item in res.TideRate)
                    {

                        strTemp = strTemp + "VND" + Config.COL_REC_DLMT;
                        //strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.TENURE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.TENURE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + item.Tenure + Config.COL_REC_DLMT;
                        //strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.UNIT_TENURE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.UNIT_TENURE].ToString()) + Config.COL_REC_DLMT;
                        strTemp = strTemp + item.TunureUnit + Config.COL_REC_DLMT;
                        //strTemp = strTemp + (dt.Rows[i][TIDE_BALANCE.RATE] == DBNull.Value ? "_NULL_" : dt.Rows[i][TIDE_BALANCE.RATE].ToString()) + Config.COL_REC_DLMT
                        strTemp = strTemp + item.Rate + Config.COL_REC_DLMT
                                + Config.ROW_REC_DLMT;
                        ;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                    retStr = retStr.Replace("{RECORD}", strTemp);
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "GET GET_TIDE_INTEREST_RATE SUCCESSFULL");
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());

                retStr = Config.ERR_MSG_GENERAL;

            }
            return retStr;

        }

        /*
         
REQ=CMD#TIDEBOOKING|CIF_NO#0310008705|SRC_ACCT#SO_TAI_KHOAN_CASA|AMOUNT#5000000|TXDESC#MO SO TIET KIEM|TENURE#3|TENURE_UNIT#M|TRANPWD#fksdfjf385738jsdfjsdf9|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

       
Response

public static String TIDEBOOKING= "ERR_CODE" + COL_DLMT + "{ERR_CODE}" 
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
            + ROW_DLMT + "CIF_NO" + COL_DLMT + "{CIF_NO}"
            + ROW_DLMT + "TRANID" + COL_DLMT + "{TRANID}"
+ ROW_DLMT + "TIDE_ACCT" + COL_DLMT + "{TIDE_ACCT}"
+ ROW_DLMT + "TRAN_DATE" + COL_DLMT + "{TRAN_DATE}"
            ;        	

         */

        /// <summary>
        /// Mở sổ tiết kiệm Online, chỉ chấp nhận sổ VND
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public string TIDEBOOKING(Hashtable hashTbl, string ip, string user_agent)
        {


            string retStr = Config.TIDEBOOKING;

            try
            {

                string custid = "";
                custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                string src_acct = "";
                string des_acct = "";
                double amount = 0;
                string txDesc = "";
                //string tranPWD = "";
                double eb_tran_id = 0;
                src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
                //des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");

                //des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
                amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT").Replace(",", "").Replace(".", ""));
                //txDesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

                txDesc = "MO SO TIET KIEM ONLINE";

                string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

                string tran_type = Config.TRAN_TYPE_TIDEBOOKING; //Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

                string tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE");
                string unit_tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE_UNIT");
                string mat_instruction = Funcs.getValFromHashtbl(hashTbl, "MAT_INSTRUCTION");
                string prin_on_mat = mat_instruction.Split('*')[0].ToString();
                string int_on_mat = mat_instruction.Split('*')[1].ToString();

                pwd = Funcs.encryptMD5(pwd + custid);

                string check_before_trans = "";


                string pos_cd = "";
                Utility ut = new Utility();
                DataTable dt0 = new DataTable();

                Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING BEGIN GET POS_CD BY CIF");
                pos_cd = CoreIntegration.getPosCdByCif(custid, src_acct);

                if (string.IsNullOrEmpty(pos_cd))
                {
                    Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING COULD NOT GET POS_CD");
                    return Config.ERR_MSG_GENERAL;
                }
                //string prod_cd = Config.PROD_CD_TIDE_ONLINE;
                string prod_cd = Funcs.getValFromHashtbl(hashTbl, "PRODCD");

                //check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);

                //ANHND2 xem laij check_before_trans voi tidebooking
                //vi tide booking khong can check mat khau

                string mToken = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

                string encryptStringClient = Funcs.getValFromHashtbl(hashTbl, "TOKEN_ENCR");
                string encryptString = Cryptography.EncryptText(custid+ mToken+ prod_cd, Cryptography._key);

                if (!encryptStringClient.ToUpper().Equals(encryptString.ToUpper()))
                {
                    Funcs.WriteLog("CIF:" + custid + "|encryptStringClient:" + encryptStringClient);
                    Funcs.WriteLog("CIF:" + custid + "|encryptString:" + encryptString);
                    return Config.ERR_MSG_GENERAL;
                }

                string resCode = String.Empty;
                string resDescVn = String.Empty;
                string resDescEn = String.Empty;

                SavingMenuRESModel menu = new SavingDAO().GetSavingMenu(custid, new iBanking.CustomModels.SavingMenuREQModel { channelId = Config.ChannelID, cifNo = custid, deviceInfo = user_agent}, null,ref resCode,ref resDescVn, ref resDescEn);

                if (!resCode.Equals(Config.ERR_CODE_DONE) || menu == null)
                {
                    Funcs.WriteLog("CIF:" + custid + "|ERROR: LOI TU DOMAIN SERVICE");
                    return Config.ERR_MSG_GENERAL;
                }

                bool checkExist = false;
                foreach (var item in menu.listProduct)
                {
                    if (item.productCode.Equals(prod_cd))
                    {
                        checkExist = true;
                    }
                }

                if (!checkExist)
                {
                    Funcs.WriteLog("CIF:" + custid + "|ERROR: Prod_CD Invalid");
                    return Config.ERR_MSG_GENERAL;
                }

                string checkMsg = AccIntegration.checkNationality(custid);

                Hashtable hashTblCheckNationality = Funcs.stringToHashtbl(checkMsg);
                if (Funcs.getValFromHashtbl(hashTblCheckNationality, "RET_CODE").Equals("00") && Funcs.getValFromHashtbl(hashTblCheckNationality, "ERR_CODE").Equals("00"))
                {
                    check_before_trans = "00";
                }
                else
                {
                    check_before_trans = "99";
                }

                string core_txno_ref = "";
                string core_txdate_ref = "";
                string res_Int_Amt_On_Mat = "";
                string res_Legacy_Tide_No = "";
                string res_Mat_Dt = "";

                string res_val_dt = "";

                string res_Int_Rate = "";

                //get Infor by CustId
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                DataSet ds = new DataSet();
                ds = da.GET_USER_BY_CIF(custid);
                DataTable dt = new DataTable();
                if (ds != null && ds.Tables[0] != null)
                {
                    dt = ds.Tables[0];
                }
                else
                {
                    return Config.ERR_MSG_GENERAL;
                }

                //KIEM TRA TAI KHOAN MO SO THUOC CIF
                //bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
                //if (!check)
                //{
                //    return Config.ERR_MSG_GENERAL;
                //}

                bool check = CoreIntegration.CheckAccountBelongCif(custid, src_acct, "CASA");
                if (!check)
                {
                    return Config.ERR_MSG_GENERAL;
                }

                check = false;

                // KIEM TRA TOI SO LUONG SO TIET KIEM TOI DA DUOC BOOK
                // MAC DINH LA CHECK O CLIENT
                // VAN CHECK THEM 1 LAN NUA O SERVER
                int countDeposit = CoreIntegration.getAllTideBooking(custid);
                if (countDeposit == -1)
                {
                    //case bi exception
                    return Config.ERR_MSG_GENERAL;
                }
                else if (countDeposit >= Config.LIMIT_TOTAL_TIDE)
                {
                    return Config.ERR_MSG_VIOLATE_MAX_TIDE_OL;
                }

                //tungdt8 fix 2020 07 22
                //KIEM TRA HAN MUC TAI KHOAN THAU CHI
                Utils util = new Utils();
                bool checkLimitThauChi = util.checkLimitThauChi(custid, Config.TRAN_TYPE_TIDEBOOKING, src_acct, des_acct, amount);
                if (!checkLimitThauChi)
                {
                    return Config.ERR_MSG_FORMAT.Replace("{0}", Config.ERR_CODE_LIMIT_THAU_CHI).Replace("{1}", Funcs.getConfigVal("LIMIT_THAU_CHI_DES")).Replace("{2}", custid);
                }

                if (check_before_trans == Config.ERR_CODE_DONE)
                {
                    Transfers tf = new Transfers();
                    DataTable eb_tran = new DataTable();

                    Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING BEGIN INSERT EB TRAN");
                    #region "insert TBL_EB_TRAN"
                    eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "TIDE" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txDesc //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , Int32.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.AUTH_METHOD].ToString()) //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , "" //sms code
                    , "" //sign data
                    , "" //core err code
                    , "" //core err desc
                    , "" //core ref_no
                    , "" //core txdate
                    , "" //core txtime
                    , 0 // order_amount
                    , 0 // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , "" //bm1
                    , "" //bm2
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , "" //bm7
                    , "" //bm8
                    , "" //bm9
                    , "" //bm10
                    , "" //bm11
                    , "" //bm12
                    , "" //bm13
                    , "" //bm14
                    , "" //bm15
                    , "" //bm16
                    , "" //bm17
                    , "" //bm18
                    , "" //bm19
                    , "" //bm20
                    , "" //bm21
                    , "" //bm22
                    , "" //bm23
                    , "" //bm24
                    , "" //bm25
                    , "" //bm26
                    , "" //bm27
                    , ip// "" //bm28
                    , user_agent// ""//bm29                   
                );

                    #endregion "insert TBL_EB_TRAN"
                    //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                    if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                    {
                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                        #region "goi hach toan vao core"
                        //HACH TOAN VAO CORE
                        string result = "";
                        Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING BEGIN POST TO CORE");
                        result = ut.postTideBookingToCore(
                              custid
                              , eb_tran_id
                              , src_acct
                              , "704" //fix for VND 
                              , unit_tenure
                              , double.Parse(tenure)
                              , amount
                              , prod_cd
                              , "RGLR"
                              , prin_on_mat
                              , int_on_mat
                              , "*"
                              , pos_cd
                              , ref core_txno_ref
                              , ref core_txdate_ref
                              , ref res_Legacy_Tide_No
                              , ref res_Mat_Dt
                              , ref res_Int_Amt_On_Mat
                              , ref res_val_dt
                              , ref res_Int_Rate
                              );
                        #endregion "goi hach toan vao core"

                        if (result == Config.gResult_INTELLECT_Arr[0])
                        {
                            #region "hach toan vao core thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING BEGIN POST TO CORE DONE TRANID:" + eb_tran_id.ToString());

                            retStr = Config.TIDEBOOKING;
                            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                            retStr = retStr.Replace("{ERR_DESC}", "TIDEBOOKING DONE");

                            retStr = retStr.Replace("{CIF_NO}", custid);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);
                            retStr = retStr.Replace("{TIDE_ACCT}", res_Legacy_Tide_No);
                            retStr = retStr.Replace("{MAT_DT}", res_Mat_Dt);
                            retStr = retStr.Replace("{INT_AMT}", res_Int_Amt_On_Mat);
                            retStr = retStr.Replace("{INT_RATE}", res_Int_Rate);
                            retStr = retStr.Replace("{VAL_DT}", res_val_dt);
                            retStr = retStr.Replace("{VAL_DT}", res_val_dt);



                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_DONE
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , res_Legacy_Tide_No //bm1
                                , res_Mat_Dt //bm2
                                , res_Int_Amt_On_Mat//bm3
                                , prin_on_mat//bm4
                                , int_on_mat //bm5
                                , "" //bm6
                                , "" //bm7
                                , "" //bm8
                                , "" //bm9
                                );

                            #endregion "hach toan vao core thanh cong"
                        }
                        else // mo so tiet kiem khong thanh cong
                        {
                            #region "hach toan vao core khong thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING BEGIN POST TO CORE FAILED TRANID:" + eb_tran_id.ToString());
                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_FAIL
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , "" //bm1
                                , "" //bm2
                                , ""//bm3
                                , ""//bm4
                                , "" //bm5
                                , "" //bm6
                                , "" //bm7
                                , "" //bm8
                                , "" //bm9
                                );

                            retStr = Config.ERR_MSG_GENERAL;

                            #endregion "hach toan vao core khong thanh cong"
                        }


                    } // khong insert duoc eb tran
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING COULD NOT INSERT EB TRAN");
                    }
                }
                else //CHECK BEFORE TRAN FAILED
                {
                    retStr = check_before_trans;
                    Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING check before tran failed");
                }
                //smls = null;
                //if (smldata != null) smldata.Dispose();

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("TIDEBOOKING:" + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            return retStr;
        }

        public string FLEXTIDEBOOKING(Hashtable hashTbl, string ip, string user_agent)
        {
            string retStr = Config.FLEXTIDEBOOKING;
            string custid = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = "";
            string des_acct = "";
            double amount = 0;
            string txDesc = "";
            //string tranPWD = "";
            double eb_tran_id = 0;
            src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            //des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");

            //des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
            amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT").Replace(",", "").Replace(".", ""));
            //txDesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

            if(amount <= 0)
            {
                return Config.ERR_MSG_AMOUNT_LESS_FEE;
            }

            txDesc = "MO SO TIET KIEM ONLINE";

            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

            string tran_type = Config.TRAN_TYPE_TIDEBOOKING; //Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            string tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE");
            string unit_tenure = Funcs.getValFromHashtbl(hashTbl, "TENURE_UNIT");
            string mat_instruction = Funcs.getValFromHashtbl(hashTbl, "MAT_INSTRUCTION");
            string prin_on_mat = mat_instruction.Split('*')[0].ToString();
            string int_on_mat = mat_instruction.Split('*')[1].ToString();

            pwd = Funcs.encryptMD5(pwd + custid);

            string check_before_trans = "";

            string pos_cd = "";

            string numOfChild = Funcs.getValFromHashtbl(hashTbl, "NUM_OF_PARENT_TIDE");

            try
            {
                Utility ut = new Utility();
                DataTable dt0 = new DataTable();

                Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING BEGIN GET POS_CD BY CIF");
                pos_cd = CoreIntegration.getPosCdByCif(custid, src_acct);

                if (string.IsNullOrEmpty(pos_cd))
                {
                    Funcs.WriteLog("CIF:" + custid + "|TIDEBOOKING COULD NOT GET POS_CD");
                    return Config.ERR_MSG_GENERAL;
                }
                string prod_cd = Config.PROD_CD_FLEX_TIDE_ONLINE;


                //check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, src_acct, des_acct, amount, pwd, tran_type);

                //ANHND2 xem laij check_before_trans voi tidebooking
                //vi tide booking khong can check mat khau


                string checkMsg = AccIntegration.checkNationality(custid);

                Hashtable hashTblCheckNationality = Funcs.stringToHashtbl(checkMsg);
                if (Funcs.getValFromHashtbl(hashTblCheckNationality, "RET_CODE").Equals("00") && Funcs.getValFromHashtbl(hashTblCheckNationality, "ERR_CODE").Equals("00"))
                {
                    check_before_trans = "00";
                }
                else
                {
                    check_before_trans = "99";
                }

                string core_txno_ref = "";
                string core_txdate_ref = "";

                string resTransId = "";
                string depositNoParentTide = "";
                string acccountNoParentTide = "";
                string numOfParentTide = "";
                string numOfChildTideSuccess = "";
                string valDate = "";
                string matDate = "";
                string tenureRes = "";
                string tenureUnit = "";
                string orgAmountChild = "";
                string interestAmountChild = "";
                string interestAmountParent = "";


                //get Infor by CustId
                TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
                DataSet ds = new DataSet();
                ds = da.GET_USER_BY_CIF(custid);
                DataTable dt = new DataTable();
                if (ds != null && ds.Tables[0] != null)
                {
                    dt = ds.Tables[0];
                }
                else
                {
                    return Config.ERR_MSG_GENERAL;
                }

                //KIEM TRA TAI KHOAN MO SO THUOC CIF
                //bool check = Auth.CustIdMatchScrAcct(custid, src_acct);
                //if (!check)
                //{
                //    return Config.ERR_MSG_GENERAL;
                //}

                bool check = CoreIntegration.CheckAccountBelongCif(custid, src_acct, "CASA");
                if (!check)
                {
                    return Config.ERR_MSG_GENERAL;
                }

                check = false;

                // KIEM TRA TOI SO LUONG SO TIET KIEM TOI DA DUOC BOOK
                // MAC DINH LA CHECK O CLIENT
                // VAN CHECK THEM 1 LAN NUA O SERVER
                int countDeposit = CoreIntegration.getAllTideBooking(custid);
                if (countDeposit == -1)
                {
                    //case bi exception
                    return Config.ERR_MSG_GENERAL;
                }
                else if (countDeposit >= Config.LIMIT_TOTAL_TIDE)
                {
                    return Config.ERR_MSG_VIOLATE_MAX_TIDE_OL;
                }


                var checkEod = CoreIntegration.getEODStatus(hashTbl);

                Hashtable hashTblcheckEod = Funcs.stringToHashtbl(checkEod);
                if (!Funcs.getValFromHashtbl(hashTblcheckEod, "ERR_CODE").Equals("00"))
                {
                    return checkEod;
                }

                ////tungdt8 fix 2020 07 22
                ////KIEM TRA HAN MUC TAI KHOAN THAU CHI
                //Utils util = new Utils();
                //bool checkLimitThauChi = util.checkLimitThauChi(custid, Config.TRAN_TYPE_TIDEBOOKING, src_acct, des_acct, amount);
                //if (!checkLimitThauChi)
                //{
                //    return Config.ERR_MSG_FORMAT.Replace("{0}", Config.ERR_CODE_LIMIT_THAU_CHI).Replace("{1}", Funcs.getConfigVal("LIMIT_THAU_CHI_DES")).Replace("{2}", custid);
                //}

                if (check_before_trans == Config.ERR_CODE_DONE)
                {
                    Transfers tf = new Transfers();
                    DataTable eb_tran = new DataTable();

                    Funcs.WriteLog("CIF:" + custid + "|FLEX TIDEBOOKING BEGIN INSERT EB TRAN");
                    #region "insert TBL_EB_TRAN"
                    eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "TIDE" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txDesc //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , Int32.Parse(dt.Rows[0][TBL_EB_USER_CHANNEL.AUTH_METHOD].ToString()) //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , "" //sms code
                    , "" //sign data
                    , "" //core err code
                    , "" //core err desc
                    , "" //core ref_no
                    , "" //core txdate
                    , "" //core txtime
                    , 0 // order_amount
                    , 0 // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , "" //bm1
                    , "" //bm2
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , prod_cd //bm7
                    , numOfChild //bm8
                    , "" //bm9
                    , "" //bm10
                    , "" //bm11
                    , "" //bm12
                    , "" //bm13
                    , "" //bm14
                    , "" //bm15
                    , "" //bm16
                    , "" //bm17
                    , "" //bm18
                    , "" //bm19
                    , "" //bm20
                    , "" //bm21
                    , "" //bm22
                    , "" //bm23
                    , "" //bm24
                    , "" //bm25
                    , "" //bm26
                    , "" //bm27
                    , ip// "" //bm28
                    , user_agent// ""//bm29                   
                );

                    #endregion "insert TBL_EB_TRAN"
                    //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                    if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                    {
                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());

                        #region "goi hach toan vao core"
                        //HACH TOAN VAO CORE
                        string result = "";
                        Funcs.WriteLog("CIF:" + custid + "|FLEX TIDEBOOKING BEGIN POST TO CORE");
                        result = ut.postFlexTideBookingToCore(
                              custid
                              , eb_tran_id
                              , src_acct
                              , "704" //fix for VND 
                              , unit_tenure
                              , double.Parse(tenure)
                              , amount
                              , prod_cd
                              , "RGLR"
                              , prin_on_mat
                              , int_on_mat
                              , "*"
                              , pos_cd
                              , numOfChild
                              , DateTime.Now.ToString()
                              , eb_tran_id.ToString()
                              , ref core_txno_ref
                              , ref core_txdate_ref
                              , ref resTransId
                              , ref depositNoParentTide
                              , ref acccountNoParentTide
                              , ref numOfParentTide
                              , ref numOfChildTideSuccess
                              , ref valDate
                              , ref matDate
                              , ref tenureUnit
                              , ref orgAmountChild
                              , ref interestAmountChild
                              , ref interestAmountParent
                              , ref tenureRes
                              );
                        #endregion "goi hach toan vao core"

                        if (result == Config.gResult_Flex_Tide_Booking_Arr[0])
                        {
                            #region "hach toan vao core thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|FLEX TIDEBOOKING BEGIN POST TO CORE DONE TRANID:" + eb_tran_id.ToString());

                            retStr = Config.FLEXTIDEBOOKING;
                            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                            retStr = retStr.Replace("{ERR_DESC}", "FLEX TIDEBOOKING DONE");

                            retStr = retStr.Replace("{CIF_NO}", custid);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);
                            retStr = retStr.Replace("{RESTRANSID}", resTransId);
                            retStr = retStr.Replace("{DEPOSITNOPARENTTIDE}", depositNoParentTide);
                            retStr = retStr.Replace("{ACCCOUNTNOPARENTTIDE}", acccountNoParentTide);
                            retStr = retStr.Replace("{NUMOFPARENTTIDE}", numOfParentTide);
                            retStr = retStr.Replace("{NUMOFCHILDTIDESUCCESS}", numOfChildTideSuccess);
                            retStr = retStr.Replace("{VALDATE}", valDate);
                            retStr = retStr.Replace("{MATDATE}", matDate);
                            retStr = retStr.Replace("{TENURE}", tenureRes);
                            retStr = retStr.Replace("{TENUREUNIT}", tenureUnit);
                            retStr = retStr.Replace("{ORGAMOUNTCHILD}", orgAmountChild);
                            retStr = retStr.Replace("{INTERESTAMOUNTCHILD}", interestAmountChild);
                            retStr = retStr.Replace("{INTERESTAMOUNTPARENT}", interestAmountParent);
                            retStr = retStr.Replace("{TOTALAMOUNTSUCCESS}", (Double.Parse(orgAmountChild) * Double.Parse(numOfChildTideSuccess)).ToString());
                            

                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_DONE
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , acccountNoParentTide //bm1
                                , matDate //bm2
                                , interestAmountParent//bm3
                                , prin_on_mat//bm4
                                , int_on_mat //bm5
                                , "" //bm6
                                , prod_cd //bm7
                                , numOfParentTide //bm8
                                , numOfChildTideSuccess //bm9
                                );

                            #endregion "hach toan vao core thanh cong"
                        }
                        else // mo so tiet kiem khong thanh cong
                        {
                            #region "hach toan vao core khong thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|FLEX TIDEBOOKING BEGIN POST TO CORE FAILED TRANID:" + eb_tran_id.ToString());
                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_FAIL
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , "" //bm1
                                , "" //bm2
                                , ""//bm3
                                , ""//bm4
                                , "" //bm5
                                , "" //bm6
                                , "" //bm7
                                , "" //bm8
                                , "" //bm9
                                );

                            retStr = Config.ERR_MSG_GENERAL;

                            #endregion "hach toan vao core khong thanh cong"
                        }


                    } // khong insert duoc eb tran
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("CIF:" + custid + "|FLEX TIDEBOOKING COULD NOT INSERT EB TRAN");
                    }
                }
                else //CHECK BEFORE TRAN FAILED
                {
                    retStr = check_before_trans;
                    Funcs.WriteLog("CIF:" + custid + "|FLEX TIDEBOOKING check before tran failed");
                }
                //smls = null;
                //if (smldata != null) smldata.Dispose();

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("custId: " + custid + "|FLEX TIDEBOOKING:" + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            return retStr;
        }

        /// <summary>
        /// Tất toán sổ tiết kiệm
        /// </summary>
        /// <param name="hashTbl"></param>
        /// <returns></returns>
        public string TIDEWDL(Hashtable hashTbl, string ip, string user_agent)
        {
            string retStr = Config.TIDEWDL;
            try
            {

                // REQ=CMD#TIDEWDL|CIF_NO#0310008705|SRC_ACCT#SO_TAI_KHOAN_TIET_KIEM|DES_ACCT#1000010000
                //|TXDESC#TAT TOAN TAI KHOAN|TRANPWD#fksdfjf385738jsdfjsdf9|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

                string custid = "";
                string des_acct = "";
                double amount = 0;
                string txDesc = "";
                double eb_tran_id = 0;
                custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                string deposit_no = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
                des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
                //txDesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

                txDesc = "TAT TOAN SO TIET KIEM";

                string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

                string tran_type = Config.TRAN_TYPE_TIDEWDL;

                double res_Amt_Withdrawn = 0;

                #region FOR TOKEN
                string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
                string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
                int typeOtp = Int16.Parse(typeOtpStr);

                if (typeOtp == 2)
                {
                    pwd = Funcs.encryptMD5(pwd + custid);
                }

                #endregion

                string check_before_trans = "";
                

                //KIEM TRA TAI KHOAN NHAN TAT TOAN THUOC CIF
                //bool check = Auth.CustIdMatchScrAcct(custid, des_acct);
                //if (!check)
                //{
                //    return Config.ERR_MSG_GENERAL;
                //}

                bool check = CoreIntegration.CheckAccountBelongCif(custid, des_acct, "CASA");
                if (!check)
                {
                    return Config.ERR_MSG_GENERAL;
                }

                check = false;

                //KIEM TRA SO SO TIET KIEM THUOC CIF
                //check = Auth.CheckDepositBelongCIF(custid, deposit_no);
                //if (!check)
                //{
                //    return Config.ERR_MSG_GENERAL;
                //}


                check = CoreIntegration.CheckAccountBelongCif(custid, deposit_no, "DEPOSIT");
                if (!check)
                {
                    return Config.ERR_MSG_GENERAL;
                }


                #region FOR TOKEN
                switch (typeOtp)
                {
                    case 2:
                        check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, deposit_no, des_acct, amount, pwd, tran_type);
                        break;
                    case 4:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    case 5:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    default:
                        break;
                }
                #endregion

                string core_txno_ref = "";
                string core_txdate_ref = "";
                string wdl_type = "F"; //default là F 

                if (check_before_trans == Config.ERR_CODE_DONE)
                {
                    Transfers tf = new Transfers();
                    DataTable eb_tran = new DataTable();

                    Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN INSERT EB TRAN:");

                    #region "insert TBL_EB_TRAN"
                    eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "TIDE" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , deposit_no//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txDesc //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , typeOtp //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , "" //sms code
                    , "" //sign data
                    , "" //core err code
                    , "" //core err desc
                    , "" //core ref_no
                    , "" //core txdate
                    , "" //core txtime
                    , 0 // order_amount
                    , 0 // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , "" //bm1
                    , "" //bm2
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , "" //bm7
                    , "" //bm8
                    , "" //bm9
                    , "" //bm10
                    , "" //bm11
                    , "" //bm12
                    , "" //bm13
                    , "" //bm14
                    , "" //bm15
                    , "" //bm16
                    , "" //bm17
                    , "" //bm18
                    , "" //bm19
                    , "" //bm20
                    , "" //bm21
                    , "" //bm22
                    , "" //bm23
                    , "" //bm24
                    , "" //bm25
                    , "" //bm26
                    , requestId //bm27
                    , ip// "" //bm28
                    , user_agent //""//bm29                   
                );

                    #endregion "insert TBL_EB_TRAN"
                    //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                    if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                    {

                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                        Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN POST TO CORE TRANID:" + eb_tran_id.ToString());

                        #region "hach toan vao core"
                        //HACH TOAN VAO CORE
                        string result = "";
                        Utility ut = new Utility();

                        result = ut.postTideWDLToCore
                            (custid
                            , eb_tran_id
                            , des_acct
                            , deposit_no
                            , wdl_type
                            , ref core_txno_ref
                            , ref core_txdate_ref
                            , ref res_Amt_Withdrawn
                            );

                        #endregion "hach toan vao core"

                        if (result == Config.gResult_INTELLECT_Arr[0])
                        {

                            #region "hach toan vao core thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN POST TO CORE DONE TRANID:" + eb_tran_id.ToString());

                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_DONE
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , wdl_type //bm1
                                , res_Amt_Withdrawn.ToString() //bm2
                                , ""//bm3
                                , ""//bm4
                                , "" //bm5
                                , "" //bm6
                                , "" //bm7
                                , "" //bm8
                                , "" //bm9
                                );

                            retStr = Config.TIDEWDL;
                            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                            retStr = retStr.Replace("{ERR_DESC}", "TIDEWDL DONE");

                            retStr = retStr.Replace("{CIF_NO}", custid);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);
                            //retStr = retStr.Replace("{TIDE_ACCT}", res_Legacy_Tide_No);
                            //retStr = retStr.Replace("{MAT_DT}", res_Mat_Dt);
                            retStr = retStr.Replace("{INT_AMT}", res_Amt_Withdrawn.ToString());
                            //retStr = retStr.Replace("{VAL_DT}", val_dt);
                            #endregion "hach toan vao core thanh cong"

                        }
                        else // mo so tiet kiem khong thanh cong
                        {
                            #region "hach toan vao core khong thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN POST TO CORE FAILED ELSE 1 TRANID:" + eb_tran_id.ToString());

                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_FAIL
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , "" //bm1
                                , "" //bm2
                                , ""//bm3
                                , ""//bm4
                                , "" //bm5
                                , "" //bm6
                                , "" //bm7
                                , "" //bm8
                                , "" //bm9
                                );
                            retStr = Config.ERR_MSG_GENERAL;

                            #endregion "hach toan vao core khong thanh cong"
                        }

                    }
                    else // khong cap nhat duoc eb tran
                    {
                        //Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN POST TO CORE FAILED ELSE 2 TRANID:" + eb_tran_id.ToString());

                        ////HACH TOAN VAO CORE KHONG THANH CONG
                        //retStr = Config.ERR_MSG_GENERAL;

                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("CIF:" + custid + "|TIDEWDL COULD NOT INSERT EB TRAN");
                    }
                }
                else// check before tran
                {
                    retStr = check_before_trans;
                    Funcs.WriteLog("CIF:" + custid + "|TIDEWDL check before tran failed");
                }
                //smls = null;
                //if (smldata != null) smldata.Dispose();

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("TIDEWDL:" + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            return retStr;
        }

        public string FLEXTIDEWDL(Hashtable hashTbl, string ip, string user_agent)
        {
            string retStr = Config.FLEXTIDEWDL;
            try
            {

                // REQ=CMD#TIDEWDL|CIF_NO#0310008705|SRC_ACCT#SO_TAI_KHOAN_TIET_KIEM|DES_ACCT#1000010000
                //|TXDESC#TAT TOAN TAI KHOAN|TRANPWD#fksdfjf385738jsdfjsdf9|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";

                string custid = "";
                string des_acct = "";
                double amount = 0;
                string txDesc = "";
                double eb_tran_id = 0;
                custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
                string deposit_no = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
                des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");
                //txDesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

                txDesc = "TAT TOAN SO TIET KIEM";

                string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

                string tran_type = Config.TRAN_TYPE_TIDEWDL;

                double res_Amt_Withdrawn = 0;

                #region FOR TOKEN
                string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
                string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
                int typeOtp = Int16.Parse(typeOtpStr);

                if (typeOtp == 2)
                {
                    pwd = Funcs.encryptMD5(pwd + custid);
                }

                #endregion

                string check_before_trans = "";
                amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT").Replace(",", "").Replace(".", ""));

                if (amount <= 0)
                {
                    return Config.ERR_MSG_AMOUNT_LESS_FEE;
                }

                //KIEM TRA TAI KHOAN NHAN TAT TOAN THUOC CIF
                //bool check = Auth.CustIdMatchScrAcct(custid, des_acct);
                //if (!check)
                //{
                //    return Config.ERR_MSG_GENERAL;
                //}

                bool check = CoreIntegration.CheckAccountBelongCif(custid, des_acct, "CASA");
                if (!check)
                {
                    return Config.ERR_MSG_GENERAL;
                }

                check = false;

                //KIEM TRA SO SO TIET KIEM THUOC CIF
                //check = Auth.CheckDepositBelongCIF(custid, deposit_no);
                //if (!check)
                //{
                //    return Config.ERR_MSG_GENERAL;
                //}


                check = CoreIntegration.CheckAccountBelongCif(custid, deposit_no, "DEPOSIT");
                if (!check)
                {
                    return Config.ERR_MSG_GENERAL;
                }


                #region FOR TOKEN
                switch (typeOtp)
                {
                    case 2:
                        check_before_trans = Auth.CHECK_BEFORE_TRANSACTION(custid, deposit_no, des_acct, amount, pwd, tran_type);
                        break;
                    case 4:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransaction(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    case 5:
                        check_before_trans = TokenOTPFunc.CheckBeforeTransactionTOKEN(tran_type, amount, custid, pwd, requestId, typeOtp);
                        break;
                    default:
                        break;
                }
                #endregion

                string core_txno_ref = "";
                string core_txdate_ref = "";
                string wdl_type = "F"; //default là F 
                string parentAcctNo = "";
                double currPrinAmt = 0;
                double currMatAmt = 0;
                double intAmount = 0;
                double tenure = 0;
                string unitTenure = "";
                string unitTenureEn = "";
                string unitTenureVn = "";
                string numOfChildTideSuccess = "0";

                if (check_before_trans == Config.ERR_CODE_DONE)
                {
                    Transfers tf = new Transfers();
                    DataTable eb_tran = new DataTable();

                    Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN INSERT EB TRAN:");

                    #region "insert TBL_EB_TRAN"
                    eb_tran = tf.insTransferTx(
                    Config.ChannelID
                    , "TIDE" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , deposit_no//src_acct
                    , des_acct //des_acct
                    , amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , amount //lcy_amount
                    , txDesc //txdesc
                    , "" //pos_cd
                    , "" //mkr_id
                    , "" //mkr dt
                    , "" //apr id 1
                    , "" //apr dt 1
                    , "" //apr id 2
                    , "" //apr dt 2
                    , typeOtp //auth_type
                    , Config.TX_STATUS_WAIT // status
                    , 0 // tran pwd idx
                    , "" //sms code
                    , "" //sign data
                    , "" //core err code
                    , "" //core err desc
                    , "" //core ref_no
                    , "" //core txdate
                    , "" //core txtime
                    , 0 // order_amount
                    , 0 // order_dis
                    , "" //order_id
                    , "" //partner code
                    , "" //category code
                    , "" //service code
                    , "" //merchant code
                    , ""//suspend account
                    , ""//fee account
                    , ""//vat account
                    , 0 //suppend amount
                    , 0 //fee amount
                    , 0 //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , "" // bank code
                    , "" // ten ngan hang
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , "" //bm1
                    , "" //bm2
                    , "" //bm3
                    , "" //bm4
                    , "" //bm5
                    , "" //bm6
                    , "" //bm7
                    , "" //bm8
                    , "" //bm9
                    , "" //bm10
                    , "" //bm11
                    , "" //bm12
                    , "" //bm13
                    , "" //bm14
                    , "" //bm15
                    , "" //bm16
                    , "" //bm17
                    , "" //bm18
                    , "" //bm19
                    , "" //bm20
                    , "" //bm21
                    , "" //bm22
                    , "" //bm23
                    , "" //bm24
                    , "" //bm25
                    , "" //bm26
                    , requestId //bm27
                    , ip// "" //bm28
                    , user_agent //""//bm29                   
                );

                    #endregion "insert TBL_EB_TRAN"
                    //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                    if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                    {

                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                        Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN POST TO CORE TRANID:" + eb_tran_id.ToString());

                        #region "hach toan vao core"
                        //HACH TOAN VAO CORE
                        string result = "";
                        Utility ut = new Utility();

                        result = ut.postFlexTideWDLToCore
                            (custid
                            ,eb_tran_id
                            , des_acct
                            , deposit_no
                            , wdl_type
                            , amount
                            , ref core_txno_ref
                            , ref core_txdate_ref
                            , ref parentAcctNo
                            , ref currPrinAmt
                            , ref currMatAmt
                            , ref intAmount
                            , ref tenure
                            , ref unitTenure
                            , ref unitTenureEn
                            , ref unitTenureVn
                            , ref numOfChildTideSuccess
                            );

                        #endregion "hach toan vao core"

                        if (result == Config.gResult_Flex_Tide_Booking_Arr[0])
                        {

                            #region "hach toan vao core thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN POST TO CORE DONE TRANID:" + eb_tran_id.ToString());

                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_DONE
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , wdl_type //bm1
                                , intAmount.ToString() //bm2
                                , currPrinAmt.ToString()//bm3
                                , currMatAmt.ToString()//bm4
                                , numOfChildTideSuccess //bm5
                                , "" //bm6
                                , "" //bm7
                                , "" //bm8
                                , "" //bm9
                                );

                            
                            retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                            retStr = retStr.Replace("{ERR_DESC}", "FLEXTIDEWDL DONE");
                            retStr = retStr.Replace("{CIF_NO}", custid);
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);
                            retStr = retStr.Replace("{PARENTACCTNO}", parentAcctNo);
                            retStr = retStr.Replace("{CURRPRINAMT}", currPrinAmt.ToString());
                            retStr = retStr.Replace("{CURRMATAMT}", currMatAmt.ToString());
                            retStr = retStr.Replace("{INTAMOUNT}", intAmount.ToString());
                            retStr = retStr.Replace("{TENURE}", tenure.ToString());
                            retStr = retStr.Replace("{UNITTENURE}", unitTenure);
                            retStr = retStr.Replace("{UNITTENUREEN}", unitTenureEn);
                            retStr = retStr.Replace("{UNITTENUREVN}", unitTenureVn);
                            retStr = retStr.Replace("{NUMOFCHILDTIDESUCCESS}", numOfChildTideSuccess);

                            #endregion "hach toan vao core thanh cong"

                        }
                        else // mo so tiet kiem khong thanh cong
                        {
                            #region "hach toan vao core khong thanh cong"
                            Funcs.WriteLog("CIF:" + custid + "|FLEXTIDEWDL BEGIN POST TO CORE FAILED ELSE 1 TRANID:" + eb_tran_id.ToString());

                            //cap nhat lai vao bang TBL_EB_TRAN
                            tf.uptTransferTx_Full(
                                eb_tran_id
                                , Config.TX_STATUS_FAIL
                                , core_txno_ref
                                , core_txdate_ref
                                , Config.ChannelID
                                , "" //bm1
                                , "" //bm2
                                , ""//bm3
                                , ""//bm4
                                , "" //bm5
                                , "" //bm6
                                , "" //bm7
                                , "" //bm8
                                , "" //bm9
                                );
                            retStr = Config.ERR_MSG_GENERAL;

                            #endregion "hach toan vao core khong thanh cong"
                        }

                    }
                    else // khong cap nhat duoc eb tran
                    {
                        //Funcs.WriteLog("CIF:" + custid + "|TIDEWDL BEGIN POST TO CORE FAILED ELSE 2 TRANID:" + eb_tran_id.ToString());

                        ////HACH TOAN VAO CORE KHONG THANH CONG
                        //retStr = Config.ERR_MSG_GENERAL;

                        retStr = Config.ERR_MSG_GENERAL;
                        Funcs.WriteLog("CIF:" + custid + "|FLEXTIDEWDL COULD NOT INSERT EB TRAN");
                    }
                }
                else// check before tran
                {
                    retStr = check_before_trans;
                    Funcs.WriteLog("CIF:" + custid + "|FLEXTIDEWDL check before tran failed");
                }
                //smls = null;
                //if (smldata != null) smldata.Dispose();

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("TIDEWDL:" + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            return retStr;
        }

        #endregion "TIDE ONLINE"

        #region "FEE HANDLE"
        public string GET_FEE_BY_TRAN_TYPE(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

            double amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));

            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            string ccycd = Funcs.getValFromHashtbl(hashTbl, "CCYCD");

            string src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            string benf_account = Funcs.getValFromHashtbl(hashTbl, "BEN_ACCT");
            string bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");

            string channel_id = Config.ChannelID;

            //lấy ra Total Fee. Đã gồm cả VAT
            double total_fee = 0;
            double fee_amt = 0;
            double vat_amt = 0;


            //Tinh lai fee dau server: khong de client gui fee len
            bool get_total_fee = false;

            if (Funcs.getConfigVal("CHARGE_FEE_VER").Equals("1.0"))
            {
                get_total_fee = Funcs.getTotalFee(custid, Config.ChannelID, tran_type, amount, src_acct, benf_account, bank_code, out total_fee, out fee_amt, out vat_amt);
            }
            else
            {
                get_total_fee = AccIntegration.GET_CHARGE_FEE(custid, src_acct, Config.ChannelID, tran_type, amount, benf_account, bank_code, "704", ref total_fee, ref fee_amt, ref vat_amt);
            }


            if (!get_total_fee)
            {
                Funcs.WriteLog("custid:" + custid + "|TRANSFER247  ERROR GET FEE: SRC_ACC: " + src_acct);
                return Config.ERR_MSG_GENERAL;
            }

            string retStr = Config.GETGET_FEE_BY_TRAN_TYPE;

            Funcs.WriteLog("CIF:" + custid + "|GET_FEE TRAN_TYPE:" + tran_type + "|AMOUNT:" + amount);
            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
            retStr = retStr.Replace(Config.ERR_DESC_VAL, "GET FEE DONE ");
            retStr = retStr.Replace("{CIF_NO}", custid);
            retStr = retStr.Replace("{TRAN_TYPE}", tran_type);
            retStr = retStr.Replace("{TOTAL_FEE}", total_fee.ToString());

            return retStr;
        }
        #endregion "FEE HANDLE"

        #region "SETTING"
        public static string SETTING(Hashtable hashTbl)
        {
            string retStr = Config.SETTING;


            /*
             Request

"REQ=CMD#SETTING|CIF#0310008705|TYPE#SET_ACCT_DEFAULT|VALUE#1000013376|TOKEN#8888888888888888888888888";
             * 
             * REQ=CMD#SETTING|CIF#0310008705|TYPE#SET_AVATAR|VALUE#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|TOKEN#8888888888888888888888888";

Response

public static String SETTING= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
             */

            string custid = "";

            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string val = Funcs.getValFromHashtbl(hashTbl, "VALUE");
            string type_setting = Funcs.getValFromHashtbl(hashTbl, "TYPE");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            //call ham update setting o day

            Utility ut = new Utility();
            Funcs.WriteLog("CIF:" + custid + "|SETTING BEGIN TYPE:" + type_setting);

            if (type_setting == "SET_AVATAR")
            {

                byte[] newBytes = Convert.FromBase64String(val);
                ut.SET_SETTING_AVATAR(custid, Config.ChannelID, newBytes);
                retStr = Config.SETTING;
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{ERR_DESC}", "UPDATE SETTING DONE");


            }
            else if (type_setting != "")// co gia tri
            {
                //linhtn add new 2017 02 08
                //neu la set tai khoan mac dinh thi check them tk mac dinh thuoc CIF
                if (type_setting == "SET_ACCT_DEFAULT")
                {
                    //bool check = Auth.CustIdMatchScrAcct(custid, val);
                    //if (!check)
                    //{
                    //    return Config.ERR_MSG_GENERAL;
                    //}

                    bool check = CoreIntegration.CheckAccountBelongCif(custid, val, "CASA");
                    if (!check)
                    {
                        return Config.ERR_MSG_GENERAL;
                    }
                }

                ut.SET_SETTING_OTHER(custid, Config.ChannelID, type_setting, val);
                retStr = Config.SETTING;
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{ERR_DESC}", "UPDATE SETTING DONE");


            }
            Funcs.WriteLog("CIF:" + custid + "|SETTING END RET:" + retStr);

            //giai phong bo nho
            ut = null;
            return retStr;

        }

        #endregion "SETTING"


        #region "INBOX OUTBOX FOR CUST"
        /*
         "REQ=CMD#GET_INBOX_OUTBOX|CIF#0310008705|TYPE#INBOX|TOKEN#8888888888888888888888888";

Response

public static String GET_INBOX_OUTBOX= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
			+ ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
+ ROW_DLMT + "RECORD" + COL_DLMT + "{RECORD}"

         */

        public static string GET_INBOX_OUTBOX(Hashtable hashTbl)
        {
            //   Thread.Sleep(600000);
            string retStr = Config.GET_INBOX_OUTBOX;

            string custid = "";

            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");

            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            //call ham update setting o day

            Utility ut = new Utility();
            Funcs.WriteLog("CIF:" + custid + "|GET_INBOX_OUTBOX BEGIN");

            DataSet ds = new DataSet();
            ds = ut.GET_TOP10_MAIL(custid, "INBOX");

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{ERR_DESC}", "GET_INBOX_OUTBOX DONE");

                string strTemp = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    strTemp = strTemp + (ds.Tables[0].Rows[i]["ID"] == DBNull.Value ? "_NULL_" : ds.Tables[0].Rows[i]["ID"].ToString()) + Config.COL_REC_DLMT;
                    strTemp = strTemp + (ds.Tables[0].Rows[i]["SUBJECT"] == DBNull.Value ? "_NULL_" : ds.Tables[0].Rows[i]["SUBJECT"].ToString()) + Config.COL_REC_DLMT;
                    strTemp = strTemp + (ds.Tables[0].Rows[i]["CONTENT"] == DBNull.Value ? "_NULL_" : ds.Tables[0].Rows[i]["CONTENT"].ToString()) + Config.COL_REC_DLMT;
                    strTemp = strTemp + "SHB"
                        + Config.ROW_REC_DLMT;
                    ;
                }
                strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);

                retStr = retStr.Replace("{RECORD}", strTemp);
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{ERR_DESC}", "GET_INBOX_OUTBOX DONE");


            }
            else
            {
                retStr = Config.ERR_MSG_NO_DATA_FOUND;
                Funcs.WriteLog("CIF:" + custid + "|GET_INBOX_OUTBOX NO DATA FOUND");
                retStr = Config.ERR_MSG_GENERAL;
            }


            Funcs.WriteLog("CIF:" + custid + "|GET_INBOX_OUTBOX END RET:" + retStr);
            //giai phong bo nho
            ut = null;

            return retStr;

        }


        /*
         "REQ=CMD#SEND_MAIL_OUTBOX|CIF#0310008705|SUBJECT#ABC|CONTENT#NOI DUNG EMAIL GUI TOI SHB KHONG CO KY TU DAC BIET XU LY O CLIENT|TOKEN#8888888888888888888888888";

Response

public static String SEND_MAIL_OUTBOX= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"

         */
        public static string SEND_MAIL_OUTBOX(Hashtable hashTbl)
        {
            string retStr = Config.SEND_MAIL_OUTBOX;

            string custid = "";

            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string subject = Funcs.getValFromHashtbl(hashTbl, "SUBJECT");
            string content = Funcs.getValFromHashtbl(hashTbl, "CONTENT");

            //call ham update setting o day

            Utility ut = new Utility();
            Funcs.WriteLog("CIF:" + custid + "|SEND_MAIL_OUTBOX BEGIN");

            DataSet ds = new DataSet();
            ds = ut.SEND_TO_OUTBOX(custid, subject, content);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{ERR_DESC}", "SEND_MAIL_OUTBOX  DONE");

            }
            else
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF:" + custid + "|SEND_MAIL_OUTBOX FAILED");
            }


            Funcs.WriteLog("CIF:" + custid + "|SEND_MAIL_OUTBOX END");
            //giai phong bo nho
            ut = null;
            return retStr;

        }

        public static string DELETE_MAIL(Hashtable hashTbl)
        {
            string retStr = Config.DELETE_MAIL;

            string custid = "";

            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string id = Funcs.getValFromHashtbl(hashTbl, "ID");

            //call ham update setting o day

            Utility ut = new Utility();
            Funcs.WriteLog("CIF:" + custid + "|DELELE_MAIL BEGIN");

            DataSet ds = new DataSet();
            ds = ut.DELELE_MAIL(custid, "INBOX", double.Parse(id.ToString()));

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                retStr = retStr.Replace("{ERR_DESC}", "DELELE_MAIL  DONE");

            }
            else
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF:" + custid + "|DELELE_MAIL FAILED");
            }


            Funcs.WriteLog("CIF:" + custid + "|DELELE_MAIL END");

            return retStr;

        }



        /*
         "REQ=CMD#SEND_MAIL_OUTBOX|CIF#0310008705|SUBJECT#ABC|CONTENT#NOI DUNG EMAIL GUI TOI SHB KHONG CO KY TU DAC BIET XU LY O CLIENT|TOKEN#8888888888888888888888888";

Response

public static String SEND_MAIL_OUTBOX= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"
         */


        #endregion "INBOX OUTBOX FOR CUST"


        #region "NHAP MA NGUOI GIOI THIEU"

        //            Request

        //"REQ=CMD#REF_INVITE|CIF#0310008705|INVITE_CODE#1234|TOKEN#8888888888888888888888888";

        //Response

        //public static String REF_INVITE= "ERR_CODE" + COL_DLMT + "{ERR_CODE}"
        //            + ROW_DLMT + "ERR_DESC" + COL_DLMT + "{ERR_DESC}"


        public static string CHECK_REF_INVITE(Hashtable hashTbl)
        {
            string retStr = Config.CHECK_REF_INVITE;

            string custid = "";

            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            //call ham update setting o day

            Utility ut = new Utility();
            Funcs.WriteLog("CIF:" + custid + "|CHECK_REF_INVITE BEGIN ");

            DataSet ds = new DataSet();
            ds = ut.CHECK_REF_INVITE(custid, Config.ChannelID);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["RET_CODE"].ToString() == "1")
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "CHECK_REF_INVITE DONE");

                    Funcs.WriteLog("CIF:" + custid + "|CHECK_REF_INVITE DONE");
                }
                else
                {
                    //Ma gioi thieu khong hop le
                    retStr = Config.ERR_MSG_GENERAL;
                    Funcs.WriteLog("CIF:" + custid + "|CHECK_REF_INVITE FAILED");
                }

            }
            else
            {
                retStr = Config.ERR_MSG_GENERAL;
                Funcs.WriteLog("CIF:" + custid + "|CHECK_REF_INVITE FAILED");
            }


            Funcs.WriteLog("CIF:" + custid + "|CHECK_REF_INVITE END RET:" + retStr);
            //giai phong bo nho
            ut = null;
            return retStr;

        }

        public static string UPDATE_REF_INVITE(Hashtable hashTbl)
        {
            string retStr = Config.UPDATE_REF_INVITE;

            string custid = "";

            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string invite_code = Funcs.getValFromHashtbl(hashTbl, "INVITE_CODE");
            string token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");

            //call ham update setting o day

            Utility ut = new Utility();
            Funcs.WriteLog("CIF:" + custid + "|UPDATE_REF_INVITE BEGIN REF_INVITE:" + invite_code);

            DataSet ds = new DataSet();
            ds = ut.UPDATE_REF_INVITE(custid, invite_code, Config.ChannelID);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["RET_CODE"].ToString() == "1")
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{ERR_DESC}", "UPDATE REF_INVITE DONE");

                }
                else
                {
                    //Ma gioi thieu khong hop le
                    retStr = Config.ERR_MSG_GENERAL;
                }

            }
            else
            {
                retStr = Config.ERR_MSG_GENERAL;
            }


            Funcs.WriteLog("CIF:" + custid + "|UPDATE_REF_INVITE END RET:" + retStr);
            //giai phong bo nho
            ut = null;
            return retStr;

        }

        #endregion "NHAP MA NGUOI GIOI THIEU"


        #region "SEARCH BRANCH ATM LOCATION"


        public static string GET_LOCATION_LIST_BY_ADDRESS(Hashtable hashTbl)
        {
            String retStr = Config.GET_LOCATION_LIST;

            try
            {
                string type = Funcs.getValFromHashtbl(hashTbl, "TYPE");

                string address = Funcs.getValFromHashtbl(hashTbl, "ADDRESS");

                // address = Funcs.removeStress(address);

                Funcs.WriteLog("GET_LOCATION_LIST_BY_ADDRESS:" + address);

                string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

                System.Net.WebRequest request = WebRequest.Create(requestUri);
                WebResponse response = request.GetResponse();
                XDocument xdoc = XDocument.Load(response.GetResponseStream());

                XElement result = xdoc.Element("GeocodeResponse").Element("result");
                XElement locationElement = result.Element("geometry").Element("location");
                XElement lat = locationElement.Element("lat");
                XElement lng = locationElement.Element("lng");

                string cur_lat = lat.Value;
                string cur_long = lng.Value;


                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Utility da = new Utility();
                DataSet ds = new DataSet();
                ds = da.GET_LOCATION_LIST(type, cur_lat, cur_long);

                //ds format:

                //2. Gen reply message
                // RECORD: NAME|ADDRESS|LATITUDE|LONGITUDE|DISTANCE|MOBILE_NO
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    retStr = retStr.Replace("{CUR_LAT}", cur_lat);
                    retStr = retStr.Replace("{CUR_LONG}", cur_long);

                    string strTemp = "";
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        strTemp = strTemp +
                            ds.Tables[0].Rows[j][LOCATION.NAME].ToString() +
                            Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j][LOCATION.ADDRESS].ToString() +
                             Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j][LOCATION.LATITUDE].ToString() +
                             Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j][LOCATION.LONGITUDE].ToString() +
                               Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j][LOCATION.DISTANCE].ToString() +
                             Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j][LOCATION.MOBILE_NO].ToString() +
                         Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                    return retStr;
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        public static string GET_LOCATION_LIST(Hashtable hashTbl)
        {
            String retStr = Config.GET_LOCATION_LIST;
            try
            {

                string type = Funcs.getValFromHashtbl(hashTbl, "TYPE");

                string cur_lat = Funcs.getValFromHashtbl(hashTbl, "CUR_LATITUDE");
                string cur_long = Funcs.getValFromHashtbl(hashTbl, "CUR_LONGITUDE");

                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Utility da = new Utility();
                DataSet ds = new DataSet();
                ds = da.GET_LOCATION_LIST(type, cur_lat, cur_long);

                //ds format:

                //2. Gen reply message
                // RECORD: NAME|ADDRESS|LATITUDE|LONGITUDE|DISTANCE|MOBILE_NO
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    string strTemp = "";
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        strTemp = strTemp +
                            ds.Tables[0].Rows[j][LOCATION.NAME].ToString() +
                            Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j][LOCATION.ADDRESS].ToString() +
                             Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j][LOCATION.LATITUDE].ToString() +
                             Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j][LOCATION.LONGITUDE].ToString() +
                               Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j][LOCATION.DISTANCE].ToString() +
                             Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j][LOCATION.MOBILE_NO].ToString() +
                         Config.ROW_REC_DLMT;

                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                    return retStr;
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }


        #endregion "SEARCH BRANCH ATM LOCATION"

        #region "FX & TIDE RATE"
        public static string GET_FX_RATE(Hashtable hashTbl)
        {

            String retStr = Config.GET_FX_RATE;
            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Utility da = new Utility();
                DataSet ds = new DataSet();
                ds = da.GET_FX_RATE();

                //ds format:
                // CCYCD|BUY_RATE|TRANSFER_RATE|SELL_RATE


                //2. Gen reply message
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    string strTemp = "";
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        strTemp = strTemp +
                            ds.Tables[0].Rows[j]["CCYCD"].ToString() +
                            Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j]["BUY_RATE"].ToString() +
                             Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j]["TRANSFER_RATE"].ToString() +
                             Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j]["SELL_RATE"].ToString() +
                         Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                    return retStr;
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;

                }

            }
            catch
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        public static string GET_TIDE_RATE(Hashtable hashTbl)

        {
            String retStr = Config.GET_TIDE_RATE;
            try
            {
                //1. Call function: GET_ACCT_LIST_HOMESCREEN_N
                Utility da = new Utility();
                DataSet ds = new DataSet();
                ds = da.GET_TIDE_RATE();

                //ds format:
                //TENURE|TENURE_UNIT|TENURE_UNIT_EN|VND_RATE|USD_RATE|EUR_RATE

                //2. Gen reply message
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retStr = retStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);

                    string strTemp = "";
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        strTemp = strTemp +
                            ds.Tables[0].Rows[j]["TENURE"].ToString() +
                            Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j]["TENURE_UNIT"].ToString() +
                             Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j]["TENURE_UNIT_EN"].ToString() +
                             Config.COL_REC_DLMT +
                             ds.Tables[0].Rows[j]["VND_RATE"].ToString() +
                             Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j]["USD_RATE"].ToString() +
                            Config.COL_REC_DLMT +
                            ds.Tables[0].Rows[j]["EUR_RATE"].ToString() +
                         Config.ROW_REC_DLMT;
                    }
                    strTemp = strTemp.Remove(strTemp.Length - Config.ROW_REC_DLMT.Length, Config.ROW_REC_DLMT.Length);
                    retStr = retStr.Replace("{RECORD}", strTemp);
                    return retStr;
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;
                }

            }
            catch
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }
        #endregion "FX & TIDE RATE"

        #region "FINGER PRINT SETUP"
        public static string SET_FP(Hashtable hashTbl, string ip, string user_agent)
        {
            string cif_no = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string setup_type = Funcs.getValFromHashtbl(hashTbl, "TYPE");
            string device_id = Funcs.NoHack(Funcs.getValFromHashtbl(hashTbl, "UUID"));
            string device_type = Funcs.getValFromHashtbl(hashTbl, "DEVICE_TYPE");
            string fp_token = Funcs.getValFromHashtbl(hashTbl, "TOKEN");
            string pwd = Funcs.encryptMD5(Funcs.getValFromHashtbl(hashTbl, "PWD") + cif_no);
            String retStr = Config.SET_FP;

            try
            {
                if (Funcs.Check_Valid_UUID(device_id))
                {
                    DataSet ds = new DataSet();
                    TBL_EB_FINGERPRINTs tbl_fp = new TBL_EB_FINGERPRINTs();
                    ds = tbl_fp.SET_FINGER_PRINT(cif_no, pwd, setup_type, device_id, device_type, fp_token, ip, user_agent);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        string err = ds.Tables[0].Rows[0][TBL_EB_USER_CHANNEL.RET].ToString();
                        fp_token = ds.Tables[0].Rows[0]["FP_TOKEN"].ToString();
                        if (tbl_fp != null) tbl_fp.Dispose();
                        ds = null;

                        retStr = retStr.Replace("{ERR_CODE}", err);
                        retStr = retStr.Replace("{FP_TOKEN_VALUE}", fp_token);
                        retStr = retStr.Replace("{CIF_NO}", cif_no);
                        if (err == Config.ERR_CODE_DONE)
                            retStr = retStr.Replace("{ERR_DESC}", "FP SETUP SUCCESSFUL");
                        else retStr = retStr.Replace("{ERR_DESC}", "FP SETUP FAILED");
                    }
                    else
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                    }
                }
                else
                {
                    retStr = Config.ERR_MSG_GENERAL;
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }
            return retStr;

        }

        #endregion "FINGER PRINT SETUP"



        #region "TUANNM10 -Dang ky MB tren MB"
        #region "GET_INFO_REG_MOB"
        public string GET_INFO_REG_MOB(Hashtable hashTbl)
        {

            string cif_no = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string pwd = Funcs.getValFromHashtbl(hashTbl, "PWD");
            string status_rs = new ActionDAO().ValidateUser(cif_no, pwd);
            string retStr = Config.ChangeSM;

            string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
            //string pattern = @"[\w-\._\+%]*(?=[\w]{4}@)";

            if (status_rs.Split('|')[0].Equals("00"))
            {
                CustomerModel model = new CustomerModel();
                try
                {

                    model = new ActionDAO().GetCustomerInfo(cif_no, "MOB");
                    if (model.CUSTID != null) //model khac null tuc la co lay dc du lieu kenh MOB => da tung dang ky roi thi moi co dong mob
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{ERR_DESC}", "DA TUNG DANG KY");
                        retStr = retStr.Replace("{AUTH_INFO_EXT1}", model.AUTH_INFO_EXT1);
                        retStr = retStr.Replace("{AUTH_METHOD}", model.AUTH_METHOD);
                        retStr = retStr.Replace("{AUTH_METHOD_NAME}", model.AUTH_METHOD_NAME);
                        retStr = retStr.Replace("{CUSTNAME}", model.CUSTNAME);
                        if (!string.IsNullOrEmpty(model.EMAIL))
                        {
                            string maskingEmail = System.Text.RegularExpressions.Regex.Replace(model.EMAIL, pattern, m => new string('*', m.Length));
                            retStr = retStr.Replace("{EMAIL}", maskingEmail);
                        }

                        retStr = retStr.Replace("{OLD_AUTH_METHOD}", model.AUTH_METHOD);
                        retStr = retStr.Replace("{OLD_MOBIL_NO}", model.AUTH_INFO_EXT1);
                    }
                    else
                    {
                        model = new ActionDAO().GetCustomerInfo(cif_no, "NET");
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{ERR_DESC}", "DANG KY MOI");
                        retStr = retStr.Replace("{AUTH_INFO_EXT1}", "");
                        retStr = retStr.Replace("{AUTH_METHOD}", "");
                        retStr = retStr.Replace("{AUTH_METHOD_NAME}", "");
                        retStr = retStr.Replace("{CUSTNAME}", model.CUSTNAME);
                        if (!string.IsNullOrEmpty(model.EMAIL))
                        {
                            string maskingEmail = System.Text.RegularExpressions.Regex.Replace(model.EMAIL, pattern, m => new string('*', m.Length));
                            retStr = retStr.Replace("{EMAIL}", maskingEmail);
                        }
                        retStr = retStr.Replace("{OLD_AUTH_METHOD}", "");
                        retStr = retStr.Replace("{OLD_MOBIL_NO}", "");
                    }

                }
                catch (Exception ex)
                {
                    Funcs.WriteLog(ex.ToString());
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_MSG_GENERAL);
                    retStr = retStr.Replace("{ERR_DESC}", "LOI");
                    retStr = retStr.Replace("{AUTH_INFO_EXT1}", "");
                    retStr = retStr.Replace("{AUTH_METHOD}", "");
                    retStr = retStr.Replace("{AUTH_METHOD_NAME}", "");
                    retStr = retStr.Replace("{CUSTNAME}", "");
                    retStr = retStr.Replace("{EMAIL}", "");
                    retStr = retStr.Replace("{OLD_AUTH_METHOD}", "");
                    retStr = retStr.Replace("{OLD_MOBIL_NO}", "");
                    return retStr;
                }
            }
            else
            {
                retStr = retStr.Replace(Config.ERR_CODE_VAL, "88");
                retStr = retStr.Replace("{ERR_DESC}", "XAC THUC MAT KHAU KHONG DUNG");
                retStr = retStr.Replace("{AUTH_INFO_EXT1}", "");
                retStr = retStr.Replace("{AUTH_METHOD}", "");
                retStr = retStr.Replace("{AUTH_METHOD_NAME}", "");
                retStr = retStr.Replace("{CUSTNAME}", "");
                retStr = retStr.Replace("{EMAIL}", "");
                retStr = retStr.Replace("{OLD_AUTH_METHOD}", "");
                retStr = retStr.Replace("{OLD_MOBIL_NO}", "");
            }
            return retStr;
        }
        #endregion "GET_INFO_REG_MOB"

        #region "CHECK_LOGIN_SM"
        public string CHECK_LOGIN_SM(string custid, string pwd)
        {
            string retStr = Config.VAL_LOGIN_NET;
            try
            {
                Funcs.WriteLog("START CHECK_LOGIN_SM: ");
                string hashPassword = pwd;
                string status_rs = new ActionDAO().ValidateUser(custid, pwd);
                Funcs.WriteLog("status_rs: " + status_rs.ToString());
                string auth_method = "";
                //check xem cif da dang ky MB chua
                //KHONG DANG KY => chua dang ky
                CustomerModel model = new CustomerModel();
                model = new ActionDAO().GetCustomerInfo(status_rs.Split('|')[1], "MOB");
                try
                {
                    if (status_rs.Split('|')[0].Equals("00"))
                    {
                        auth_method = new ActionDAO().GET_AUTH_METHOD_NET(status_rs.Split('|')[1]);
                    }
                }
                catch (Exception ex)
                {
                    //Write log
                    Funcs.WriteLog("GET_AUTH_METHOD_NET: " + ex.Message.ToString());
                }
                Funcs.WriteLog("CHECK_LOGIN_SM status_rs: " + status_rs.Split('|')[0].ToString());
                retStr = retStr.Replace(Config.ERR_CODE_VAL, status_rs.Split('|')[0]);
                retStr = retStr.Replace("{AUTH_METHOD}", auth_method);
                retStr = retStr.Replace("{USERNAME}", custid);
                retStr = retStr.Replace("{CIF_NO}", status_rs.Split('|')[1]);
                retStr = retStr.Replace("{CHECK_REG_MOB}", model.AUTH_METHOD_NAME);

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("GET_AUTH_METHOD_NET: " + ex.Message.ToString());
            }

            return retStr;
        }
        #endregion

        //GET_OTP
        public string GET_OTP(string cif_no, string pwd, string mob_reg)
        {
            string retStr = Config.GET_OTP_REG;
            //check so dien thoai da dang ky chua
            bool check_Mob = checkUserInfo(cif_no, mob_reg);
            string challenge = "";
            string requestId = "";

            if (check_Mob == true)
            {
                retStr = retStr.Replace(Config.ERR_CODE_VAL, "E90");
                retStr = retStr.Replace("{TRAN_ID}", "");
                retStr = retStr.Replace("{SMS_CODE}", "");
                retStr = retStr.Replace("{MOBILE_NO}", "");
            }
            else
            {
                string status_rs = new ActionDAO().ValidateUser(cif_no, pwd);
                if (status_rs.Split('|')[0].Equals("00"))
                {
                    string auth_method = "";
                    string smsCode = "";
                    double tran_id = 0;
                    bool rs_sms = false;
                    Users userSession = new Users();
                    userSession = new ActionDAO().GET_INFO_USER_NET(cif_no)[0];
                    auth_method = userSession.AUTH_METHOD;


                    smsCode = new Utils().LoadAuthMode(userSession, auth_method, out challenge, out requestId);

                    tran_id = new ActionDAO().InsertCustomerShbMobileSetting(userSession, smsCode);
                    String phoneNumber = userSession.AUTH_INFO_EXT1.ToString();
                    String sensitivityNumber = phoneNumber.Substring(0, 2) + "xxxxxxx" + phoneNumber.Substring(phoneNumber.Length - 3, 3);
                    //Insert Into SMS table
                    if (auth_method.Equals("2"))
                    {
                        rs_sms = (bool)SMSIntegration.SendOTP(userSession, smsCode, tran_id);
                    }

                    if (auth_method.Equals("3"))
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{TRAN_ID}", tran_id.ToString());
                        retStr = retStr.Replace("{SMS_CODE}", smsCode.ToString());
                        retStr = retStr.Replace("{MOBILE_NO}", "");

                    }

                    if (auth_method.Equals("5"))
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{TRAN_ID}", tran_id.ToString());
                        retStr = retStr.Replace("{REQUEST_ID}", requestId);
                        retStr = retStr.Replace("{CHALLENGE_CODE}", challenge);
                    }

                    if (tran_id != 0 && rs_sms)
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                        retStr = retStr.Replace("{TRAN_ID}", tran_id.ToString());
                        retStr = retStr.Replace("{SMS_CODE}", smsCode.ToString());
                        retStr = retStr.Replace("{MOBILE_NO}", sensitivityNumber.ToString());
                    }
                    else
                    {
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                        retStr = retStr.Replace("{TRAN_ID}", "");
                        retStr = retStr.Replace("{SMS_CODE}", "");
                        retStr = retStr.Replace("{MOBILE_NO}", "");
                    }
                }
                else
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, "88");
                    retStr = retStr.Replace("{TRAN_ID}", "");
                    retStr = retStr.Replace("{SMS_CODE}", "");
                }
            }
            return retStr;
        }

        private bool checkUserInfo(string CUSTID, string mobileNo)
        {

            RetCode retCode = new ActionDAO().CheckUserInfo(CUSTID, "MOB", mobileNo);
            if (!Config.gResult_Setting_SHB_Mobile_Arr[0].Split('|')[0].Equals(retCode.RET_CODE))
            {
                for (int i = 1; i < Config.gResult_Setting_SHB_Mobile_Arr.Length; i++)
                {
                    if (Config.gResult_Setting_SHB_Mobile_Arr[i].Split('|')[0].Equals(retCode.RET_CODE))
                    {
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public string REGISTER_MGOLD(string cif_no, string AUTH_INFO_EXT1, string OLD_AUTH_METHOD, string OLD_MOBIL_NO,
             string smsCode, double tranID, string requestID)
        {
            //tren IB thi AUTH_INFO_EXT1: so dien thoai dang ky
            string AUTH_METHOD = "";
            AUTH_METHOD = new ActionDAO().GET_AUTH_METHOD_NET(cif_no);
            string confirmCode = new Utils().getSMS_CODE(tranID.ToString());
            string retStr = Config.CONFIRM_REG;
            string message = string.Empty;
            string AUTH_METHOD_NEW = "";
            string AUTH_METHOD_NAME = "";
            Users userSession = new Users();
            userSession = new ActionDAO().GET_INFO_USER_NET(cif_no)[0];

            bool isValid = false;
            isValid = ValidateAuthCode(userSession.CUSTID, userSession.AUTH_INFO_EXT1, AUTH_METHOD, confirmCode, smsCode, tranID, requestID);
            if (!isValid)
            {
                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_INVALID_ACTIVE_CODE);
                retStr = retStr.Replace("{TRAN_ID}", tranID.ToString());
            }
            else
            {
                switch (Int32.Parse(AUTH_METHOD))
                {
                    case 2:
                        AUTH_METHOD_NEW = "4";
                        AUTH_METHOD_NAME = "SMS OTP";
                        break;
                    case 3:
                        AUTH_METHOD_NEW = "2";
                        AUTH_METHOD_NAME = "mGold";
                        break;
                    case 5:
                        AUTH_METHOD_NEW = Config.TypeMToken;
                        AUTH_METHOD_NAME = "Smart OTP";
                        break;
                    default:
                        break;
                }
                message = changeShbMobileSetting(userSession, OLD_AUTH_METHOD, AUTH_METHOD_NEW, AUTH_METHOD_NAME, OLD_MOBIL_NO, AUTH_INFO_EXT1, tranID, out isValid);
                //E90|Số điện thoại đã đăng ký|Error in register mobile number. Please enter mobile number field again
                string mess_code = "";
                if (!string.IsNullOrEmpty(message))
                {
                    mess_code = message.Split('|')[0].ToString();
                }

                if (isValid && mess_code.Equals("E00"))
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace("{TRAN_ID}", tranID.ToString());
                }
                else if (mess_code.Equals("E90"))
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, "E90");
                    retStr = retStr.Replace("{TRAN_ID}", tranID.ToString());
                }
                else if (mess_code.Equals("E99"))
                {
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                    retStr = retStr.Replace("{TRAN_ID}", tranID.ToString());
                }
            }

            return retStr;
        }

        public string getSMS_CODE(String trans_id)
        {
            return new ActionDAO().getSMS_CODE(trans_id);
        }



        /// Lay ra phuong thuc xac thuc cua tai khoan
        private string LoadAuthMode(Users userSession, string authMode, out string challenge, out string requestId)
        {
            challenge = "";
            requestId = "";
            authMode = userSession.AUTH_METHOD.ToString();
            String auInfoExt1 = userSession.AUTH_INFO_EXT1;

            String smsCode = string.Empty;
            String Scard_Mdata = string.Empty;


            switch (authMode)
            {
                //Xac thuc bang mat khau dang nhap
                case "1":
                    smsCode = "1";
                    break;
                //Xac thuc bang sms token
                case "2":
                    String phoneNumber = userSession.AUTH_INFO_EXT1.ToString();
                    smsCode = Funcs.getAlphabets1(6).ToUpper();

                    break;

                //xac thuc bang eSecure
                case "3":

                    String cardSerial = auInfoExt1;
                    smsCode = Funcs.getRndSmartCardToken(2).ToUpper();
                    break;

                //xac thuc bang PKI
                case "4":
                    smsCode = "4";
                    break;
                case "5":
                    // TRUONG HOP GET QUESTION TU SOFT TOKEN
                    TokenOTP.CreateTransactionResType res = null;
                    OtpDA otpDa = new OtpDA();
                    DataTable dt = otpDa.insertTokenOtp(userSession.CUSTID, "_NULL_", Int32.Parse(Config.TypeMToken), 0.0, 0);

                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        try
                        {
                            res = TokenOTPIntegration.CreateTransaction(userSession.CUSTID, dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString());

                            if (res != null && res.RespSts.Sts.Equals("0"))
                            {
                                double request_id = double.Parse(dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString());
                                requestId = Convert.ToString(request_id);
                                dt = otpDa.updateTokenOtp(dt.Rows[0][TBL_EB_TOKEN_OTP.REQUEST_ID].ToString(), res.challenge);
                                challenge = res.challenge;
                            }
                        }
                        catch (Exception e)
                        {
                            Funcs.WriteLog("Exception: " + e.Message);
                            object a = e.Message;
                            Funcs.WriteLog("Lỗi Hàm GetRequestOTPById: " + a);
                        }
                    }

                    smsCode = requestId;
                    break;
                default:
                    break;
            }
            return smsCode;
        }

        public bool ValidateAuthCode(string cifNo, string AUTH_INFO_EXT1, string AUTH_METHOD, string confirmCode, string smsCode, double tranID, string requestId)
        {
            string authMode = AUTH_METHOD.ToString();
            Transfers tf = new Transfers();

            if (String.IsNullOrEmpty(smsCode) && (authMode.Equals("2") || authMode.Equals("3") || authMode.Equals(Config.TypeMToken)))
            {
                tf.uptTransferTx(tranID, 2, string.Empty, string.Empty, "MOB");
                return false;
            }

            switch (authMode)
            {
                //Xac thuc bang mat khau dang nhap
                case "1":
                    return false;
                //break;
                case "2":
                    if (confirmCode == null || String.IsNullOrEmpty(smsCode) || !smsCode.ToLower().Equals(confirmCode.ToLower()))
                    {

                        tf.uptTransferTx(tranID, 2, string.Empty, string.Empty, "MOB");
                        return false;
                    }

                    break;
                case "3":
                    if (confirmCode == null || String.IsNullOrEmpty(smsCode))
                    {

                        tf.uptTransferTx(tranID, 2, string.Empty, string.Empty, "MOB");
                        return false;
                        //redirect về trang bao loi
                    }

                    //Lay ra bang esecure
                    String cardSerial = AUTH_INFO_EXT1;

                    bool isException = false;

                    String position = confirmCode;
                    String pCode = smsCode;


                    List<RetCode> returnCd = ActionDAO.CHECK_ESECURE(position, pCode, cardSerial, out isException);
                    if (returnCd == null || returnCd.Count == 0 || !returnCd[0].REL_VAL.Equals("1"))
                    {
                        //redirect ve trang bao loi

                        tf.uptTransferTx(tranID, 2, string.Empty, string.Empty, "MOB");
                        return false;
                    }

                    break;
                case "4":
                    return false;
                case "5":

                    TokenOTP.VerifyOTPCRResType res = null;

                    try
                    {
                        res = TokenOTPIntegration.VerifyOTPCR(cifNo, requestId, smsCode);

                        if (res != null && res.RespSts.Sts.Equals("0"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;

                        }
                    }
                    catch (Exception e)
                    {
                        Funcs.WriteLog("Exception: " + e.Message);
                        object a = e.Message;
                        Funcs.WriteLog("Lỗi Hàm VerifyOTPCR: " + a);
                        return false;
                    }
                default: break;
            }

            return true;
        }

        public string changeShbMobileSetting(Users userSession, string OLD_AUTH_METHOD, string AUTH_METHOD, string AUTH_METHOD_NAME, string OLD_MOBIL_NO, string AUTH_INFO_EXT1, double tranID, out bool success)
        {
            try
            {
                Transfers tf = new Transfers();
                success = false;

                string newPass = Funcs.getAlphabets(6);
                string nameType = "";
                switch (AUTH_METHOD)
                {
                    case "4":
                        nameType = "MSMS";
                        break;
                    case "5":
                        nameType = "MTOKEN";
                        break;
                    default:
                        break;
                }

                RetCode retCode = new ActionDAO().UpdateUserMobile(
                    userSession.CUSTID
                    , Decimal.Parse(OLD_AUTH_METHOD)
                    , Decimal.Parse(AUTH_METHOD)
                    , OLD_MOBIL_NO
                    , AUTH_INFO_EXT1
                    , newPass
                    , userSession.REG_BRANCH
                    , nameType);

                if (Config.gResult_Setting_SHB_Mobile_Arr[0].Split('|')[0].Equals(retCode.RET_CODE))
                {
                    success = true;
                    try
                    {
                        string pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\Upgrade-SHB-Mobile.html";

                        if (OLD_AUTH_METHOD.Equals(Decimal.Parse(AUTH_METHOD)))
                        {
                            pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\Change-SHB-Mobile.html";
                        }

                        if (Config.MOB_REV_AUTH_METHOD.Equals(Decimal.Parse(AUTH_METHOD)))
                        {
                            pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\Remove-SHB-Mobile.html";
                        }

                        if (Config.MOB_REG_AUTH_METHOD.Equals(Decimal.Parse(AUTH_METHOD)))
                        {
                            pathFile = AppDomain.CurrentDomain.BaseDirectory + @"rpt\Register-SHB-Mobile.html";
                        }

                        string strContent = Funcs.ReadAllFile(pathFile);

                        strContent = strContent.Replace("{DATE}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                        strContent = strContent.Replace("{CUSTNAME}", userSession.CUSTNAME.Trim());
                        strContent = strContent.Replace("{CIF}", userSession.CUSTID);

                        if (Config.MOB_REV_AUTH_METHOD.Equals(Decimal.Parse(OLD_AUTH_METHOD))
                            && Config.MOB_REG_AUTH_METHOD.Equals(Decimal.Parse(AUTH_METHOD)))
                        {//dang ky moi
                            strContent = strContent.Replace("{PACKAGE}", AUTH_METHOD_NAME);
                            strContent = strContent.Replace("{PASSWORD}", newPass);
                        }
                        else
                        {
                            if (!OLD_AUTH_METHOD.Equals(AUTH_METHOD))
                            { //nang cap goi
                                strContent = strContent.Replace("{OLD_PACKAGE}", "MBASIC");
                                strContent = strContent.Replace("{MOBILE_NO}", AUTH_INFO_EXT1);
                                strContent = strContent.Replace("{PASSWORD}", newPass);
                            }

                            if (OLD_AUTH_METHOD.Equals(AUTH_METHOD))
                            { //doi so dien thoi
                                strContent = strContent.Replace("{PACKAGE}", AUTH_METHOD_NAME);
                                strContent = strContent.Replace("{OLD_MOBILE}", OLD_MOBIL_NO);
                                strContent = strContent.Replace("{NEW_MOBILE}", AUTH_INFO_EXT1);
                                strContent = strContent.Replace("{PASSWORD}", newPass);

                            }
                        }


                        string fileGGPlay = AppDomain.CurrentDomain.BaseDirectory + @"Content\images\badge\google-play.png";
                        string fileItunes = AppDomain.CurrentDomain.BaseDirectory + @"Content\images\badge\apple-itunes.png";

                        AlternateView view = getEmbeddedImage(strContent, fileGGPlay, "google-play", fileItunes, "apple-itunes");

                        if (!string.IsNullOrEmpty(strContent))
                        {
                            Funcs.sendEmail(userSession.EMAIL, "DANG KY/THAY DOI DICH VU SHB MOBILE - " + userSession.CUSTID, strContent, view); //Len Production thi mo SENDMAIL
                        }
                    }
                    catch (Exception ex)
                    {
                        Funcs.WriteLog("changeShbMobileSetting 1.1: " + ex.Message.ToString());
                    }


                    tf.uptTransferTx(tranID, 1, string.Empty, string.Empty, userSession.CHANNEL_ID);

                    return Config.gResult_Setting_SHB_Mobile_Arr[0];
                }
                else
                {

                    tf.uptTransferTx(tranID, 2, string.Empty, string.Empty, userSession.CHANNEL_ID);

                    for (int i = 1; i < Config.gResult_Setting_SHB_Mobile_Arr.Length; i++)
                    {
                        if (Config.gResult_Setting_SHB_Mobile_Arr[i].Split('|')[0].Equals(retCode.RET_CODE))
                        {
                            return Config.gResult_Setting_SHB_Mobile_Arr[i];
                        }
                    }
                    return Config.gResult_Setting_SHB_Mobile_Arr[1];
                }

            }
            catch (Exception ex)
            {
                success = false;
                return Config.gResult_Setting_SHB_Mobile_Arr[1];
            }
        }

        public string changeShbMobilePackage(Users userSession, string OLD_AUTH_METHOD, string AUTH_METHOD, string AUTH_METHOD_NAME, string OLD_MOBIL_NO, string AUTH_INFO_EXT1, double tranID
            , string LIMIT_AMOUNT
            , string LIMIT_AMOUNT_INTER
            , string LIMIT_AMOUNT_STOCK
            , out bool success)
        {
            try
            {
                Transfers tf = new Transfers();
                success = false;

                RetCode retCode = new ActionDAO().UpdateUserMobileToken(
                    userSession.CUSTID
                    , Decimal.Parse(OLD_AUTH_METHOD)
                    , Decimal.Parse(AUTH_METHOD)
                    , OLD_MOBIL_NO
                    , AUTH_INFO_EXT1
                    , userSession.REG_BRANCH
                    , LIMIT_AMOUNT
                    , LIMIT_AMOUNT_INTER
                    , LIMIT_AMOUNT_STOCK);

                if (Config.gResult_Setting_SHB_Mobile_Arr[0].Split('|')[0].Equals(retCode.RET_CODE))
                {
                    success = true;
                    tf.uptTransferTx(tranID, 1, string.Empty, string.Empty, userSession.CHANNEL_ID);

                    return Config.gResult_Setting_SHB_Mobile_Arr[0];
                }
                else
                {

                    tf.uptTransferTx(tranID, 2, string.Empty, string.Empty, userSession.CHANNEL_ID);

                    for (int i = 1; i < Config.gResult_Setting_SHB_Mobile_Arr.Length; i++)
                    {
                        if (Config.gResult_Setting_SHB_Mobile_Arr[i].Split('|')[0].Equals(retCode.RET_CODE))
                        {
                            return Config.gResult_Setting_SHB_Mobile_Arr[i];
                        }
                    }

                    return Config.gResult_Setting_SHB_Mobile_Arr[1];
                }

            }
            catch (Exception ex)
            {
                success = false;
                return Config.gResult_Setting_SHB_Mobile_Arr[1];
            }
        }

        private AlternateView getEmbeddedImage(string bodyContent, string filePathPlay, string contentIdPlay, string filePathItunes, string contentIdItunes)
        {

            AlternateView av1 = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyContent, null, System.Net.Mime.MediaTypeNames.Text.Html);
            MemoryStream ms1 = new MemoryStream(File.ReadAllBytes(filePathPlay));
            ms1.Position = 0;
            LinkedResource lr1 = new LinkedResource(ms1, "image/gif");
            lr1.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Gif;
            lr1.ContentId = contentIdPlay;
            av1.LinkedResources.Add(lr1);

            MemoryStream ms2 = new MemoryStream(File.ReadAllBytes(filePathItunes));
            ms2.Position = 0;
            LinkedResource lr2 = new LinkedResource(ms2, "image/gif");
            lr2.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Gif;
            lr2.ContentId = contentIdItunes;
            av1.LinkedResources.Add(lr2);

            return av1;
        }
        #endregion "TUANNM10 -Dang ky MB tren MB"

        public string REGISTER_MTOKEN(string cif_no)
        {
            string AUTH_METHOD = "";
            string message = string.Empty;
            string NEW_AUTH_METHOD = Config.TypeMToken;
            string OLD_AUTH_METHOD = "";
            string AUTH_METHOD_NAME = Config.NameMToken;
            string OLD_MOBIL_NO = "";
            string AUTH_INFO_EXT1 = "";
            bool isValid = false;

            //tren IB thi AUTH_INFO_EXT1: so dien thoai dang ky
            Users userSession = new Users();
            userSession = new ActionDAO().GET_INFO_USER_MOB(cif_no)[0];
            DataSet ds = new DataSet();
            TBL_EB_USER_CHANNELs da = new TBL_EB_USER_CHANNELs();
            double tran_id = new ActionDAO().InsertCustomerShbMobileSetting(userSession, "");
            ds = da.GET_USER_BY_CIF(cif_no);
            EBANK_LIMIT_GROUP resLimit = new ActionDAO().GET_LIMIT_GROUP(Config.NameMToken, Config.ChannelID);
            if ((ds != null) && (ds.Tables[0].Rows.Count > 0) && resLimit != null)
            {
                OLD_AUTH_METHOD = ds.Tables[0].Rows[0]["AUTH_METHOD"].ToString();
                OLD_MOBIL_NO = ds.Tables[0].Rows[0]["AUTH_INFO_EXT1"].ToString();
                AUTH_INFO_EXT1 = ds.Tables[0].Rows[0]["AUTH_INFO_EXT1"].ToString();
                message = changeShbMobilePackage(userSession, OLD_AUTH_METHOD, NEW_AUTH_METHOD, AUTH_METHOD_NAME, OLD_MOBIL_NO, AUTH_INFO_EXT1, tran_id
                    , resLimit.LIMIT_AMOUNT
                    , resLimit.LIMIT_AMOUNT_INTER
                    , resLimit.LIMIT_AMOUNT_STOCK
                    , out isValid);
            }

            return message;
        }

        public bool checkLimitThauChi(String custId, String tranType, String transferAcc, String receivingAccount, double tran_amount)
        {
            RetCode lst = new RetCode();
            try
            {
                var dynParams = new iBanking.Common.OracleDynamicParameters();
                dynParams.Add("PCUSTID", custId, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("PSRC_ACCT", transferAcc, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("PDES_ACCT", receivingAccount, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("PTRAN_AMOUNT", tran_amount, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Double, direction: ParameterDirection.Input);
                dynParams.Add("PTRAN_TYPE", tranType, dbType: Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, direction: ParameterDirection.Input);
                dynParams.Add("OUT_CUR", dbType: Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, direction: ParameterDirection.Output);

                lst = (RetCode)new ConnectionFactory(Config.gEBANKConnstr)
                .GetItems<RetCode>(CommandType.StoredProcedure, "PKG_LIMIT.GET_LIMIT_THAU_CHI", dynParams).First();

                if (lst != null)
                {
                    Funcs.WriteLog("custid:" + custId + "|checkLimitThauChi |" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(lst)));

                    if (lst.RET_CODE == Config.ERR_CODE_DONE)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("checkLimitThauChi-" + custId + " - " + ex.ToString());
                return false;
            }

            return false;
        }

        private static Random random = new Random();
        public static string RandomString(string prefix, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
