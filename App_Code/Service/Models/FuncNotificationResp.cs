using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FuncNotificationResp
/// </summary>
public class FuncNotificationResp
{
    public decimal ID { set; get; }
    public decimal CATEGORY_ID { set; get; }
    public string NAME_VI { set; get; }
    public string NAME_EN { set; get; }
    public string CAT_NAME_EN { set; get; }
    public string CAT_NAME_VI { set; get; }
    public string NEWS_TYPE_CODE { set; get; }
    public bool IsChecked { set; get; }
}