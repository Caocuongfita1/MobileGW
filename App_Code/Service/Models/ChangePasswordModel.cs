using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ChangePasswordModel
/// </summary>
public class ChangePasswordModel
{
        public string CUSTID { get; set; }
        public string USERNAME { get; set; }
        public string PASS_NO { get; set; }
        public string LEGACY_AC { get; set; }
        public string AUTH_METHOD { get; set; }
}

public class ChangePasswordQueryModel
{
    public string CUSTID { get; set; }
    public string EMAIL { get; set; }
    public string SMSCODE { get; set; }
    public string CUST_NAME { get; set; }
    public string PACKAGE { get; set; }
    public string MOBILE_NO { get; set; }


}