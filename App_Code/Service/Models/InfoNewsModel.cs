using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InfoNewsModel
/// </summary>
public class InfoNewsModel
{
    public decimal ID { set; get; }
    public string NEWS_TITLE{ set; get; }
    public string NEWS_CONTENT { set; get; }
    public DateTime CREATE_DATE { set; get; }
    public DateTime CREATED_DATE { set; get; }
    public DateTime NEXT_DATE { set; get; }
    public string SENDING_DATE { set; get; }
    public string APP_SRC { set; get; }
    public string FUNC_SRC { set; get; }
    public decimal? NEWS_ID { set; get; }
    public decimal? PARENT_ID { set; get; }
    public decimal? IS_READ { set; get; }
    public string NEWS_TITLE_IMG_URL { set; get; }
    public string NEWS_INSIDE_LINK { set; get; }
    public string NEWS_TO { set; get; }
    public string NEWS_COMM_TYPE { set; get; }
    public decimal? STATUS { set; get; }
    public decimal? PRIORITY { set; get; }
    public decimal? IS_VIEW_DETAIL { set; get; }
    public decimal? IS_ACTIVATE { set; get; }
    public DateTime COMPLETE_DATE { set; get; }
    public string REF_ID { set; get; }
    public decimal? TotalPage { set; get; }
    public string PAYLOAD_DATA { set; get; }
}

public class PAYLOAD_DATA
{
    public string NEWS_ID { set; get; }
    public string PARENT_ID { set; get; }
}