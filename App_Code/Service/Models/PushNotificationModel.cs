using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PushNotificationModel
/// </summary>
public class PushNotificationModel
{
    public double ID { get; set; }
    public string NEWS_CONTENT { get; set; }
    public string PAYLOAD_DATA { get; set; }
    public string NEWS_TITLE { get; set; }
    public string NEWS_TO { get; set; }
    public string NEWS_COMM_TYPE { get; set; }
    public double STATUS { get; set; }
    public double PRIORITY { get; set; }
    public double IS_VIEW_DETAIL { get; set; }
    public DateTime CREATE_DATE { get; set; }
    public DateTime MODIFIED_DATE { get; set; }
    public string PARENT_ID { get; set; }
    public string APP_SRC { get; set; }
    public string FUNC_SRC { get; set; }
    public string REF_ID { get; set; }
}