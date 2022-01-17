using iBanking.Common;
using mobileGW.Service.API;
using mobileGW.Service.Framework;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for PushNotification
/// </summary>
public class PushNotification
{
    public PushNotification()
    {
    }
    public string GetUnreadNews(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.ERR_MSG_GENERAL;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string newsId = Funcs.getValFromHashtbl(hashTbl, "NEWS_ID");
        string parentId = Funcs.getValFromHashtbl(hashTbl, "PARENT_ID");
        //OracleDynamicParameters dynParams = new OracleDynamicParameters();
        //dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
        //dynParams.Add("PCIF_NO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
        //dynParams.Add("PAPP_SRC", "SHB_MOBILE", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
        //dynParams.Add("PFUNC_SRC", null, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

        try
        {
            Funcs.WriteLog("CIF_NO:" + cifNo + "|GET UNREAD REQ: PAPP_SRC: SHB_MOBILE| BEGIN:");

            TotalRecordModel respModel = GET_COUNT_UNREAD(cifNo, "SHB_MOBILE");

            Funcs.WriteLog("CIF_NO:" + cifNo + "|GET UNREAD RES :" + new JavaScriptSerializer().Serialize(respModel));

            if (respModel != null)
            {
                if (respModel.TotalNumber > 0)
                {
                    resultStr = Config.RESPONE_TOTAL_UNREAD_NEWS;
                    resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                    resultStr = resultStr.Replace("{ERR_DESC}", "THANH CONG");
                    resultStr = resultStr.Replace("{TOTAL_RECORD}", respModel.TotalNumber.ToString());

                    Funcs.WriteLog("CIF_NO:" + cifNo + "|GET UNREAD RES : SUCCESS");
                }
            }

            Funcs.WriteLog("CIF_NO:" + cifNo + "|GET UNREAD RES : END");

            return resultStr;
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GetUnreadNews: " + ex.Message.ToString());
            return Config.ERR_MSG_FORMAT_NEW.Replace("{0}",Config.ERR_CODE_GENERAL).Replace("{1}", "LOI GET_COUNT_UNREAD_NEW").Replace("{2}", cifNo);
        }
    }

    public string SetUnreadNews(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.ERR_MSG_GENERAL;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string newsId = Funcs.getValFromHashtbl(hashTbl, "NEWS_ID");
        string parentId = Funcs.getValFromHashtbl(hashTbl, "PARENT_ID");
        //OracleDynamicParameters dynParams = new OracleDynamicParameters();
        //dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
        //dynParams.Add("PCIF_NO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
        //dynParams.Add("PPARENT_ID", parentId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
        //dynParams.Add("PAPP_SRC", "SHB_MOBILE", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
        //dynParams.Add("PFUNC_SRC", null, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

        ResponseModelOracle respModel = SET_UNREAD(cifNo, newsId);

        if (respModel != null)
        {
            if (respModel.Status.Equals(Config.ERR_CODE_DONE))
            {
                resultStr = Config.SUCCESS_MSG_GENERAL;
            }
        }

        return resultStr;
    }
    public string HTMLBody(string title,string strBody,string dateTime)
    {
        string baseUrl = "";
        try {
             baseUrl = ConfigurationSettings.AppSettings.Get("URL_IMG_HOST");
        }
        catch(Exception ex)
        {
            baseUrl = "Err";
        }
        StringBuilder strTHML = new StringBuilder();
        strTHML.AppendLine("<!DOCTYPE html>");
        strTHML.AppendLine("<html>");
        strTHML.AppendLine("<head>");
        strTHML.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1'>");
        strTHML.AppendLine("<style>.responsive {max-width: 100%;height: auto;}</style> ");
        strTHML.AppendLine("</head> ");
        strTHML.AppendLine("<body>");
        strTHML.Append("<h2>");
        strTHML.Append(title);
        strTHML.AppendLine("</h2>");
        strTHML.AppendLine("<div style=\"position: relative; \"><img src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyNpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNS1jMDE0IDc5LjE1MTQ4MSwgMjAxMy8wMy8xMy0xMjowOToxNSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIChNYWNpbnRvc2gpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjhCNEE0RUVGN0I2RjExRTlBMTA4RDEyRDhBMTAzMkQ4IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjhCNEE0RUYwN0I2RjExRTlBMTA4RDEyRDhBMTAzMkQ4Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6OEI0QTRFRUQ3QjZGMTFFOUExMDhEMTJEOEExMDMyRDgiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6OEI0QTRFRUU3QjZGMTFFOUExMDhEMTJEOEExMDMyRDgiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz5xxTReAAAFDklEQVR42uxXXUxbZRg+PfSUtkBbKDCoLbYdlBIygTGiESODMBBCAuqymRjjRuKF3umFGmOi3pioMXFBE43e+TM1y6IJyZiIQFI3Njfkr7QFwt+g7dYyy1oo/fd5CZ0FDrbDi124NznJOd/5zvc+3/s+7/N+RxCLxZj7aSxzn+0BACHfIPGCjxvRaJSJRCIMy7KMkONoIg0L6BN6Fw6HN9+lpaXx75ZlkwOghdra2pi5ubldjuVyufAhtbpOp9U2ZclkVRhX4XWaQCBwBYNBy8L8fN/S0tIvLrfbQ6gSgWAO093dzej1+m3+BDt3Ss6Ki4u3ASAzGAzHH6moeCsjI6MqvhOaS9/HHdE9gNyw2WxdoyMjZxCRYOIak5OTTFlZWfIUpKen373PysqS1h09+qlarT5Nzz6fz4Zdnl9eXr605vNNw0koMzPz4fz8/Bq1RtMuVyger6mp+bDUYGjv7e095XK5ZuLhpyjsNN4IlJeXM1arlcHC0vaOjvPYdTMc3TGbzW9Pms1feb1eP80Vi8Vy7J5bW1tz07NIJGJKDIa2ysrKj/Ct0b+xMX+xp6f5ptM5RQDwPWM0GpNXAYEgtHX19Z+Rc4Bc+N1kqr8yNNQVd473bEtra/+JkydnwQ0DjSH8jHliohtOn1hfX++ViMXaxsbGc3gvozVTLkNi88Hi4qc1Gs0p3K8ODg52IK/DPKxW4MoiviWOu93ulZ4LF54F2D9lMtmh6urqd/nCzwuAkCoUCg5hfI/KjMI+ZbON7FGu4a0U7qpZ5N6LqL1ERNTp9S+DQwdpY0kBULmpVKonEfpDINzMxPj4l/sVmcXFxesOh+McNEOs0+le4NMWlk8stFptK5WW3W7/EQQL7LH7aDIA5HB2dvZriioqpAXEFCQFIBQKGZlcXkEfLy8tmf7REQGbeMGEqUTBYbePBAOBdYlEUiqRSnNSleICQr2xsXGDnquPHHkD4tQZCoW2zQPB9HuxO274ZgW5v4mI6rBuNoZWkgGgMLEUgQgRAgY+FOTk5JQEAoGdQGOYEv43AAAYoWurCthUIhDD5NvEAYTtAJ4tf1y9+v7Y6OjnexxeYii3ub0AcBwnh0ApcevHut6UmhFCb8Hk2rz8/Orp6ekBv99/i679VIIyN7cUILKRiklct5KSkGp1cWHhVwICITrOUdv9D6bX6Z5JA7FBxoFVjyeSUhmi2VyErDohSI+VlJQ8tV/nSqWyQFNU1BkFlebm579JSQco95BSj81q/ZjAVB0+/Elubm72vTqnjlpbW3smXSxWOp3ObpwVLlOJp9QLiLFjY2NdHo9nCBVQiqbzQ15enuxenLe0tHxQqFKdCIdCf12/du1ValR8/YAXAEUBhAkM9Pc/D04sSKXSY8eamvq0Ot2jgiTOAVTT1Nz83YGCgtcR8pDJZHoRijrDdxxLeh4gKywsLG1oaPge6lgJMDGnw/EtTktn7aRwwaCLahxlloM0GSHh7cj5aURAibm34bzTarH8HOcW33mAFwAdm6ampu6OoZ/LoYbvoKG8gjym0xw4X4cTJ+6jIo5TciJRNrGdCIcDyE/Dw8Nvgsy2xLUtFssuALxSTGRJDNkq7Le+vteKioq+AIjncDBtgkgZkSr9Vl59SNk4ut8gyHYWDegSyXbiGimngIwOpDtlN64RZOhqjFgiUW5pOwsI3hD0/s7qKgVkcwM7CUfPAL95bEsK4MGv2f8KwN8CDAAfcKQvHprONQAAAABJRU5ErkJggg==\" style=\"position: 0; width: 16px; height: 16px; \"/><span style = \"font-size: 12px;margin-top: 10px;position: absolute;top: -8px;color: #606161;\">");
        strTHML.AppendLine(dateTime);
        strTHML.AppendLine("</span></div>");
        int count = 1;
        try
        {
            foreach (Match m in Regex.Matches(strBody, "(<img.*?class=[\"'])([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
            {
                strBody = strBody.Replace(m.Value, m.Groups[1].ToString().Substring(0, m.Groups[1].ToString().Length - 7) + m.Groups[3].ToString().Substring(1));
            }
            foreach (Match m in Regex.Matches(strBody, "(<img.*?height=[\"'])([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
            {
                strBody = strBody.Replace(m.Value, m.Groups[1].ToString().Substring(0, m.Groups[1].ToString().Length - 8) + m.Groups[3].ToString().Substring(1));
            }
            foreach (Match m in Regex.Matches(strBody, "(<img.*?height:)([^\"]*)([;].*?>)", RegexOptions.IgnoreCase))
            {
                strBody = strBody.Replace(m.Value, m.Groups[1].ToString().Substring(0, m.Groups[1].ToString().Length - 7) + m.Groups[3].ToString().Substring(1));
            }

            foreach (Match m in Regex.Matches(strBody, "(<img.*?width=[\"'])([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
            {
                strBody = strBody.Replace(m.Value, m.Groups[1].ToString().Substring(0, m.Groups[1].ToString().Length - 7) + m.Groups[3].ToString().Substring(1));
            }
            foreach (Match m in Regex.Matches(strBody, "(<img.*?width:)([^\"]*)([;].*?>)", RegexOptions.IgnoreCase))
            {
                strBody = strBody.Replace(m.Value, m.Groups[1].ToString().Substring(0, m.Groups[1].ToString().Length - 6) + m.Groups[3].ToString().Substring(1));
            }
            foreach (Match m in Regex.Matches(strBody, "(<img.*?width:)([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
            {
                strBody = strBody.Replace(m.Value, m.Groups[1].ToString().Substring(0, m.Groups[1].ToString().Length - 6) + m.Groups[3].ToString().Substring(1));
            }
            //string html = "<img id=\"img1\" src=\"images/img1.jpg\"></br><img id=\"img2\" src=\"images/img2.jpg\">";
            foreach (Match m in Regex.Matches(strBody, "(<img.*?src=[\"'])([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
            {
                int i = m.Groups[2].ToString().ToUpper().IndexOf("HTTP");
                if (i != 0)
                {
                    strBody = strBody.Replace(m.Value, m.Groups[1].ToString() + baseUrl + m.Groups[2].ToString() + m.Groups[3].ToString());
                    count += 1;
                }
            }

            foreach (Match m in Regex.Matches(strBody, "(<img.*?src=[\"'])([^\"]*)(['\"].*?>)", RegexOptions.IgnoreCase))
            {
                strBody = strBody.Replace(m.Value, m.Groups[1].ToString() + m.Groups[2].ToString() + "\" class=\"responsive\" width=\"100%\"" + m.Groups[3].ToString().Substring(1));
            }
        }
        catch (Exception ex)
        {
            string strEx = ex.Message;
        }
        strTHML.AppendLine(strBody);
        strTHML.AppendLine("</body>");
        strTHML.AppendLine("</html>");
        return strTHML.ToString();
    }
    /// <summary>
    /// Req:
    /// CMD#VIEW_NEWS|CIF_NO#0107575700|TOKEN#_{}|NEWS_ID#_{}_
    /// Resp:
    /// ERR_CODE#00|ERR_DESC#THANH CONG|NEWS_CONTENT#_{}_| NEWS_TITLE#{}| NEWS_INSIDE_LINK#_{}_|NEWS_DATE#_{}_
    /// </summary>
    /// <param name="hashTbl"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public string GetDetailNewsNotification(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.ERR_MSG_GENERAL;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string newsId = Funcs.getValFromHashtbl(hashTbl, "NEWS_ID");
        try
        {
            /*
           GET_DETAIL_NEWS_FOR_CUST (pCIFNO     IN     VARCHAR2,
                                       pIdNews    IN     VARCHAR2,
                                       MY_CUR        OUT REF_CUR);
                                         */
            //OracleDynamicParameters dynParams = new OracleDynamicParameters();
            //dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //dynParams.Add("pCIFNO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            //dynParams.Add("pIdNews", newsId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            InfoNewsModel respModel = GET_READ_NEW(cifNo, newsId);

            if (respModel != null)
            {
                resultStr = Config.RESPONE_GET_DETAIL_NEWS;
                //Funcs.WriteLog("GetDetailNewsNotification: " + respModel.SENDING_DATE);
                string base64Encoded = HTMLBody(respModel.NEWS_TITLE, respModel.NEWS_CONTENT, respModel.COMPLETE_DATE.ToString("dd/MM/yyyy HH:mm"));
                //Funcs.WriteLog("base64Encoded Before: " + base64Encoded);
                //Funcs.WriteLog("NEWS_TITLE: " + respModel.NEWS_TITLE);
                //Funcs.WriteLog("NEWS_CONTENT: " + respModel.NEWS_CONTENT);
                byte[] data = System.Text.Encoding.UTF8.GetBytes(base64Encoded);
                base64Encoded = System.Convert.ToBase64String(data);
                //Funcs.WriteLog("base64Encoded After: " + base64Encoded);
                resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                resultStr = resultStr.Replace("{ERR_DESC}", "THANH CONG");
                resultStr = resultStr.Replace("{NEWS_CONTENT}", base64Encoded);
                resultStr = resultStr.Replace("{NEWS_INSIDE_LINK}", respModel.NEWS_INSIDE_LINK);
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GetCustomerInfo: " + ex.Message.ToString());
        }
        return resultStr;
    }
    /// <summary>
    /// Req:
    /// CMD#LIST_NEWS|CIF_NO#0107575700|TOKEN#_{}_
    /// Resp:
    /// ERR_CODE#00|ERR_DESC#THANH CONG|LIST_NEWS#NEWS_ID$_{}_$NEWS_TITLE$_{}_$NEWS_DATE$_{}_$NEWS_IMG$_{}_ ^…
    /// </summary>
    /// <param name="hashTbl"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public string GetListNewsNotification(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.ERR_MSG_GENERAL;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string pageSize = Funcs.getValFromHashtbl(hashTbl, "PAGE_SIZE");
        string pageIndex = Funcs.getValFromHashtbl(hashTbl, "PAGE_INDEX");
        try
        {
            //OracleDynamicParameters dynParams = new OracleDynamicParameters();
            //dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            //dynParams.Add("PPAGE_SIZE", Int32.Parse(pageSize), dbType: OracleDbType.Decimal, direction: ParameterDirection.Input);
            //dynParams.Add("PPAGE_INDEX", Int32.Parse(pageIndex), dbType: OracleDbType.Decimal, direction: ParameterDirection.Input);
            //dynParams.Add("PCIF_NO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            //dynParams.Add("PAPP_SRC", "SHB_MOBILE", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            //dynParams.Add("PFUNC_SRC", null, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            //dynParams.Add("pListIdNews", null, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            List<InfoNewsModel> respModel = GET_LIST_NEWS(cifNo, pageSize, pageIndex, "SHB_MOBILE");

            if (respModel != null && respModel.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                for(int i=0;i< respModel.Count; i++)
                {
                    InfoNewsModel itemNewsModel = respModel[i];

                    //if (respModel[i].PAYLOAD_DATA != null) {

                        //PAYLOAD_DATA data = JsonConvert.DeserializeObject<PAYLOAD_DATA>(itemNewsModel.PAYLOAD_DATA);

                        builder.Append("NEWS_ID").Append(Config.COL_REC_DLMT)
                        .Append(itemNewsModel.ID).Append(Config.COL_REC_DLMT)
                        .Append("PARENT_ID").Append(Config.COL_REC_DLMT)
                        .Append(itemNewsModel.PARENT_ID).Append(Config.COL_REC_DLMT)
                        .Append("NEWS_TITLE").Append(Config.COL_REC_DLMT)
                        .Append(itemNewsModel.NEWS_TITLE).Append(Config.COL_REC_DLMT)
                        .Append("NEWS_DATE").Append(Config.COL_REC_DLMT)
                        .Append(itemNewsModel.COMPLETE_DATE == null ?
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm") :
                        itemNewsModel.COMPLETE_DATE.ToString("dd/MM/yyyy HH:mm")
                        ).Append(Config.COL_REC_DLMT)
                        .Append("NEWS_IMG").Append(Config.COL_REC_DLMT)
                        .Append(itemNewsModel.NEWS_TITLE_IMG_URL).Append(Config.COL_REC_DLMT)
                        .Append("IS_READ").Append(Config.COL_REC_DLMT)
                        .Append(itemNewsModel.IS_READ).Append(Config.COL_REC_DLMT);
                    //}

                    if ( i != respModel.Count-1)
                    {
                        builder.Append(Config.ROW_REC_DLMT);
                    }
                }

                resultStr = Config.RESPONE_GET_LIST_NEWS;
                resultStr = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                resultStr = resultStr.Replace("{ERR_DESC}", "THANH CONG");
                resultStr = resultStr.Replace("{LIST_NEWS}", builder.ToString());
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GetCustomerInfo: " + ex.Message.ToString());
        }
        return resultStr;
    }

    /// <summary>
    /// Req:
    /// CMD#UPDATE_FUNCTION_PUSH|CIF_NO#0107575700#TOKEN#_{}_|STATUS_ALL#_{}_| LIST_FUNCTION #ITEM_CODE1^ITEM_CODE2^…
    /// Resp:
    /// ERR_CODE#00|ERR_DESC#THANH CONG
    /// </summary>
    /// <param name="hashTbl"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    public string UpdateListFuncNotification(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.ERR_MSG_GENERAL;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string status = Funcs.getValFromHashtbl(hashTbl, "STATUS_ALL");
        decimal statusAll = 0;
        Decimal.TryParse(status, out statusAll);

        string lstFunc = Funcs.getValFromHashtbl(hashTbl, "LIST_FUNCTION");
        Funcs.WriteLog("lstFunc:"+ lstFunc);
        Funcs.WriteLog("cifNo:" + cifNo);
        Funcs.WriteLog("status:" + status);
        if (string.IsNullOrEmpty(lstFunc))
        {
            lstFunc = "^^";
        }
        try
        {
            /*
            UPDATE_REG_FUNC_FOR_CUST (pCIFNO          IN     VARCHAR2,
                                         pCommType       IN     VARCHAR2,
                                         pRegFunc        IN     VARCHAR2,
                                         pStatus         IN     NUMBER,
                                         pChannel           IN VARCHAR2,
                                         MY_CUR             OUT REF_CUR)
                                         */
            OracleDynamicParameters dynParams = new OracleDynamicParameters();
            dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            dynParams.Add("pCIFNO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCommType", "OTT", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pRegFunc", lstFunc, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pStatus", statusAll, dbType: OracleDbType.Decimal, direction: ParameterDirection.Input);
            dynParams.Add("pChannel", "MOB", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            ResponseModel respModel = (ResponseModel)new ConnectionFactory(Config.gEBANKConnstr)
                               .ExecuteData<ResponseModel>(CommandType.StoredProcedure, "PKG_PUSH_NOTIFICATION.UPDATE_REG_FUNC_FOR_CUST", dynParams).First();
            if (respModel != null)
            {
                if (respModel.STATUS_CODE.Equals(Config.ERR_CODE_DONE))
                {
                    resultStr = Config.SUCCESS_MSG_GENERAL;
                }
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GetCustomerInfo: " + ex.Message.ToString());
        }
        return resultStr;
    }
    /// <summary>
    /// Req:
    /// CMD#LIST_FUNCTION_PUSH|CIF_NO#_{CIF_NO}_|TOKEN#_TOKEN_
    /// Resp:
    /// ERR_CODE#00|ERR_DESC#THANH CONG|STATUS_ALL#1|
    /// LIST_FUNCTION#
    /// ITEM_NAME_VI$_{}_$ITEM_NAME_EN$_{}_$ITEM_CODE$_{}_$CATEGORY_ID$_{}_$CATEGORY_NAME_VI$_{}_$CATEGORY_NAME_EN$_{}_$ITEM_STATUS$_{}_^ … 
    /// </summary>
    /// <param name="hashTbl"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public string GetListFuncNotification(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.ERR_MSG_GENERAL;
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        try
        {
            //get all function that get allow user register notification
            List<FuncNotificationResp> lstFuncNotificationResp = new List<FuncNotificationResp>();
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            lstFuncNotificationResp = (List<FuncNotificationResp>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<FuncNotificationResp>
                             (CommandType.StoredProcedure, "PKG_PUSH_NOTIFICATION.GET_NOTIFICATION_FUNC",
                             dynParams);

            if (lstFuncNotificationResp.Count <= 0)
            {
                return resultStr;
            }
           
            dynParams = new OracleDynamicParameters();
            dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            dynParams.Add("pCIFNO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCommType", "OTT", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            InfoCustRegisterResp respModel = (InfoCustRegisterResp)new ConnectionFactory(Config.gEBANKConnstr)
                               .ExecuteData<InfoCustRegisterResp>(CommandType.StoredProcedure, "PKG_PUSH_NOTIFICATION.GET_REG_FOR_CUST", dynParams).First();
            if(respModel != null)
            {
                String regFuncRegistered = respModel.REG_FUNC;
                if(regFuncRegistered == null)
                {
                    regFuncRegistered = "";
                }
                StringBuilder lstFunc = new StringBuilder();
                for (int i = 0; i < lstFuncNotificationResp.Count; i++)
                {
                    FuncNotificationResp regFuncItem = lstFuncNotificationResp[i];
                    lstFunc.Append("ITEM_NAME_VI").Append(Config.COL_REC_DLMT)
                            .Append(regFuncItem.NAME_VI).Append(Config.COL_REC_DLMT)
                            .Append("ITEM_NAME_EN").Append(Config.COL_REC_DLMT)
                            .Append(regFuncItem.NAME_EN).Append(Config.COL_REC_DLMT)
                            .Append("ITEM_CODE").Append(Config.COL_REC_DLMT)
                            .Append(regFuncItem.NEWS_TYPE_CODE).Append(Config.COL_REC_DLMT)
                            .Append("CATEGORY_ID").Append(Config.COL_REC_DLMT)
                            .Append(regFuncItem.CATEGORY_ID).Append(Config.COL_REC_DLMT)
                            .Append("CATEGORY_NAME_VI").Append(Config.COL_REC_DLMT)
                            .Append(regFuncItem.CAT_NAME_VI).Append(Config.COL_REC_DLMT)
                            .Append("CATEGORY_NAME_EN").Append(Config.COL_REC_DLMT)
                            .Append(regFuncItem.CAT_NAME_EN).Append(Config.COL_REC_DLMT)
                            .Append("ITEM_STATUS").Append(Config.COL_REC_DLMT)
                            .Append(regFuncRegistered.Contains(regFuncItem.NEWS_TYPE_CODE) ? "1" : "0");
                    if(i != lstFuncNotificationResp.Count - 1)
                    {
                        lstFunc.Append(Config.ROW_REC_DLMT);
                    } 
                }
                resultStr = Config.RESPONE_GET_LIST_FUNCTION_PUSH;
                resultStr  = resultStr.Replace("{ERR_CODE}", Config.ERR_CODE_DONE);
                resultStr = resultStr.Replace("{ERR_DESC}", "THANH CONG");
                resultStr = resultStr.Replace("{STATUS_ALL}", Decimal.ToInt32(respModel.STATUS) == 1 ? "1": "0");
                resultStr = resultStr.Replace("{LIST_FUNCTION}", lstFunc.ToString());
            }
            else
            {
                return resultStr;
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GetCustomerInfo: " + ex.Message.ToString());
        }
        return resultStr;
    }

    /// <summary>
    /// Format
    /// CMD#REGISTER_PUSH_NOTIFICATION|APP_DEVICEID#|CHANNEL#MOB?NET|DEVICE#ANDROID?IOS|CIF_NO#_{}_|TOKEN#_{}_
    /// </summary>
    /// <param name="hashTbl"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public string Register(Hashtable hashTbl, string ip, string userAgent)
    {
        string resultStr = Config.ERR_MSG_GENERAL;
        //get parameters from client
        string cifNo = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
        string device = Funcs.getValFromHashtbl(hashTbl, "DEVICE");
        string appDeviceId = Funcs.getValFromHashtbl(hashTbl, "APP_DEVICEID");
        string channel = Funcs.getValFromHashtbl(hashTbl, "CHANNEL");
        try
        {
            //get all function that get allow user register notification
            List<FuncNotificationResp> lstFuncNotificationResp = new List<FuncNotificationResp>();
            var dynParams = new OracleDynamicParameters();
            dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);

            lstFuncNotificationResp = (List<FuncNotificationResp>)new ConnectionFactory(Config.gEBANKConnstr)
                             .GetItems<FuncNotificationResp>
                             (CommandType.StoredProcedure, "PKG_PUSH_NOTIFICATION.GET_NOTIFICATION_FUNC", 
                             dynParams);

            if (lstFuncNotificationResp.Count <= 0)
            {
                return resultStr;
            }
            StringBuilder regFuncBuilder = new StringBuilder();
            for (int i = 0; i < lstFuncNotificationResp.Count; i++)
            {
                FuncNotificationResp regFuncItem = lstFuncNotificationResp[i];
                regFuncBuilder.Append(regFuncItem.NEWS_TYPE_CODE);
                if( i != lstFuncNotificationResp.Count - 1)
                {
                    regFuncBuilder.Append(Config.ROW_REC_DLMT);
                }
            }
            /*
            REG_COMM_TYPE_FOR_CUST (pCIFNO          IN     VARCHAR2,
                                     pCommType       IN     VARCHAR2,
                                     pCommTo         IN     VARCHAR2,
                                     pRegFunc        IN     VARCHAR2,
                                     pStatus         IN     NUMBER,
                                     pChannel        IN     VARCHAR2,
                                     pDeviceSystem   IN     VARCHAR2,
                                     MY_CUR             OUT REF_CUR);
             */
            dynParams = new OracleDynamicParameters();
            dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            dynParams.Add("pCIFNO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCommType", "OTT", dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pCommTo", appDeviceId, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pRegFunc", regFuncBuilder.ToString(), dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pStatus", 1, dbType: OracleDbType.Decimal, direction: ParameterDirection.Input);
            dynParams.Add("pChannel", channel, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);
            dynParams.Add("pDeviceSystem", device, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            ResponseModel respModel = (ResponseModel)new ConnectionFactory(Config.gEBANKConnstr)
                               .ExecuteData<ResponseModel>(CommandType.StoredProcedure, "PKG_PUSH_NOTIFICATION.REG_COMM_TYPE_FOR_CUST", dynParams).First();
            if (respModel.STATUS_CODE.Equals(Config.ERR_CODE_DONE))
            {
                resultStr = Config.SUCCESS_MSG_GENERAL;
            }
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("GetCustomerInfo: " + ex.Message.ToString());
        }

        return resultStr;
    }

    public PushDeviceModel GetDeviceId(string cifNo)
    {

        PushDeviceModel res = new PushDeviceModel();

        try
        {
            OracleDynamicParameters dynParams = new OracleDynamicParameters();
            dynParams.Add("MY_CUR", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
            dynParams.Add("PCIF_NO", cifNo, dbType: OracleDbType.Varchar2, direction: ParameterDirection.Input);

            res = (PushDeviceModel)new ConnectionFactory(Config.gEBANKConnstr)
                           .GetItems<PushDeviceModel>(CommandType.StoredProcedure, "PKG_EB_CCOMS.GET_DEVICE_TOKEN_BY_CUSTID", dynParams).First();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("CIF_NO: " + cifNo + "|GetDeviceId: " + ex.ToString());
        }
        return res;
    }

    /// <summary>
    /// Get list Promotions by mobile
    /// </summary>
    /// <param name="mobileNum"></param>
    /// <returns></returns>
    public bool addPushToCCOMS(PushNotificationModel objQuery, ref string errCode, string key = null)
    {
        bool isDone = false;
        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_CCOMS");

            CallApiService<DeleteResponseModel> _NewsCallApiService = new CallApiService<DeleteResponseModel>(Funcs.getConfigVal("TOKEN_CCOMS"), "");

            Response<DeleteResponseModel> objResponse = _NewsCallApiService.PostAPINew(BaseUrl, ApiPushNotification.URL_PUSH, objQuery, key);

            Funcs.WriteLog("addPushToCCOMS: " + JsonConvert.SerializeObject(objResponse));

            if (objResponse != null && objResponse.StatusCode.Equals(0) && objResponse.Data != null && objResponse.Data.Status.Equals("00"))
            {
                Funcs.WriteLog("addPushToCCOMS: SUCCESSFULL " + JsonConvert.SerializeObject(objResponse));
                isDone = true;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("addPushToCCOMS: " + ex.Message.ToString());
            isDone = false;
        }

        return isDone;
    }

    public bool sendPushEmail(string custID, string to, string title, string content, string ccomtype, double tranId, string core_ref_no)
    {
        bool sendPush = false;
        try
        {
            string errCode = String.Empty;

            if (ccomtype.Equals("OTT"))
            {
                PushDeviceModel modelPush = GetDeviceId(custID);

                if (modelPush != null && modelPush.DEVICE_TOKEN.Length > 0)
                {
                    to = modelPush.DEVICE_TOKEN;
                }
                else
                {
                    Funcs.WriteLog("custid: " + custID + " sendPushEmail FAIL|NOT GET DEVICE TOKEN FOR PUSH");
                    return false;
                }
            }

            //sendMail
            PushNotificationModel modePush = new PushNotificationModel();

            modePush.ID = 0;
            modePush.NEWS_CONTENT = content;
            modePush.PAYLOAD_DATA = "";
            modePush.NEWS_TITLE = title;
            modePush.NEWS_TO = to;
            modePush.NEWS_COMM_TYPE = ccomtype;
            modePush.STATUS = 0;
            modePush.PRIORITY = 0;
            modePush.IS_VIEW_DETAIL = 0;
            modePush.CREATE_DATE = DateTime.Now;
            modePush.MODIFIED_DATE = DateTime.Now;
            modePush.PARENT_ID = Funcs.getConfigVal("PUSH_PARENT_ID");
            modePush.APP_SRC = ccomtype.Equals("EMAIL") ? "SHBBRC" : Funcs.getConfigVal("PUSH_APP_SRC");
            modePush.FUNC_SRC = Funcs.getConfigVal("PUSH_FUNC_SRC");
            modePush.REF_ID = custID;

            sendPush = new PushNotification().addPushToCCOMS(modePush, ref errCode);

            if (sendPush)
            {
                Funcs.WriteLog("custid: " + custID + " sendPushEmail SUCCESS|" + to);
            }
            else
            {
                Funcs.WriteLog("custid: " + custID + " sendPushEmail FAIL|" + to);
            }

        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid: " + custID + " sendPushEmail FAIL|" + ex.ToString());
            return false;
        }

        return sendPush;
    }

    public List<InfoNewsModel> GET_LIST_NEWS(string custId, string pageSize, string pageIndex, string appSrc)
    {
        List<InfoNewsModel> listNews = null;

        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_CCOMS");

            CallApiService<InfoNewsModel> _NewsCallApiService = new CallApiService<InfoNewsModel>(Funcs.getConfigVal("TOKEN_CCOMS"), "");

            ResponseList<InfoNewsModel> objResponse = _NewsCallApiService.GetListAPI(BaseUrl, ApiPushNotification.URL_HISTORY_GET_LIST.Replace("{pageSize}", pageSize).Replace("{pageIndex}", pageIndex).Replace("{appSrc}", appSrc).Replace("{refId}", custId));

            Funcs.WriteLog("CIF_NO: " + custId + "|GET_LIST_NEWS: " + JsonConvert.SerializeObject(objResponse));

            if (objResponse != null && objResponse.Data != null)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|GET_LIST_NEWS: SUCCESSFULL |" + JsonConvert.SerializeObject(objResponse));

                listNews = objResponse.Data;
            }
            else
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|GET_LIST_NEWS: ERROR |" + JsonConvert.SerializeObject(objResponse));
                return null;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("CIF_NO: " + custId + "|GET_LIST_NEWS: " + ex.Message.ToString());
            return null;
        }

        return listNews;
    }

    public TotalRecordModel GET_COUNT_UNREAD(string custId, string appSrc)
    {
        TotalRecordModel res = new TotalRecordModel();

        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_CCOMS");

            CallApiService<TotalRecordModel> _NewsCallApiService = new CallApiService<TotalRecordModel>(Funcs.getConfigVal("TOKEN_CCOMS"), "");

            Response<TotalRecordModel> objResponse = _NewsCallApiService.GetAPI(BaseUrl, ApiPushNotification.URL_HISTORY_COUNT_UNREAD.Replace("{userId}", custId).Replace("{appSrc}", appSrc), Int32.Parse(Funcs.getConfigVal("TIMEOUT_GET_COUNT_UNREAD")));

            Funcs.WriteLog("CIF_NO: " + custId + "|GET_COUNT_UNREAD: " + JsonConvert.SerializeObject(objResponse));

            if (objResponse != null && objResponse.Data != null)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|GET_COUNT_UNREAD: SUCCESSFULL |" + JsonConvert.SerializeObject(objResponse));

                res = objResponse.Data;
            }
            else
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|GET_COUNT_UNREAD: ERROR |" + JsonConvert.SerializeObject(objResponse));
                return null;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("CIF_NO: " + custId + "|GET_COUNT_UNREAD: " + ex.Message.ToString());
            return null;
        }

        return res;
    }

    public ResponseModelOracle SET_UNREAD(string custId, string id)
    {
        ResponseModelOracle res;

        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_CCOMS");

            CallApiService<ResponseModelOracle> _NewsCallApiService = new CallApiService<ResponseModelOracle>(Funcs.getConfigVal("TOKEN_CCOMS"), "");

            Response<ResponseModelOracle> objResponse = _NewsCallApiService.PutAPI(BaseUrl, ApiPushNotification.URL_HISTORY_SET_READ.Replace("{userId}", custId).Replace("{id}", id),null);

            Funcs.WriteLog("CIF_NO: " + custId + "|SET_UNREAD: " + JsonConvert.SerializeObject(objResponse));

            if (objResponse != null && objResponse.Data != null)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|SET_UNREAD: CALL SUCCESSFULL |" + JsonConvert.SerializeObject(objResponse));

                res = objResponse.Data;
            }
            else
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|SET_UNREAD: ERROR |" + JsonConvert.SerializeObject(objResponse));
                return null;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("CIF_NO: " + custId + "|SET_UNREAD: " + ex.Message.ToString());
            return null;
        }

        return res;
    }

    public InfoNewsModel GET_READ_NEW(string custId, string id)
    {
        InfoNewsModel res;

        try
        {
            string BaseUrl = Funcs.getConfigVal("URL_CCOMS");

            CallApiService<InfoNewsModel> _NewsCallApiService = new CallApiService<InfoNewsModel>(Funcs.getConfigVal("TOKEN_CCOMS"), "");

            Response<InfoNewsModel> objResponse = _NewsCallApiService.GetAPI(BaseUrl, ApiPushNotification.URL_HISTORY_VIEW.Replace("{id}", id));

            Funcs.WriteLog("CIF_NO: " + custId + "|GET_HISTORY_VIEW: " + JsonConvert.SerializeObject(objResponse));

            if (objResponse != null && objResponse.Data != null)
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|GET_HISTORY_VIEW: SUCCESSFULL |" + JsonConvert.SerializeObject(objResponse));

                res = objResponse.Data;
            }
            else
            {
                Funcs.WriteLog("CIF_NO: " + custId + "|GET_HISTORY_VIEW: ERROR |" + JsonConvert.SerializeObject(objResponse));
                return null;
            }
        }
        catch (Exception ex)
        {
            //Write log
            Funcs.WriteLog("CIF_NO: " + custId + "|GET_HISTORY_VIEW: " + ex.Message.ToString());
            return null;
        }

        return res;
    }
}