using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PolicyDetailDLVN
/// </summary>
namespace mobileGW.Service.Models
{
    public class PayPartnerClass
    {
        public String SERVICE_PROVIDER_ID { get; set; }
        public String BRANCH_ID { get; set; }
        public String NAME { get; set; }
        public String ADDRESS { get; set; }
        public String PHONE { get; set; }
        public String HOT_LINE { get; set; }
        public String TAX_NO { get; set; }
        public String PARENT_BRANCH_ID { get; set; }
        public String TYPE { get; set; }
        public String ACCOUNT_NO { get; set; }
        public String CIF { get; set; }
        public String MKR_ID { get; set; }
        public String AUTH_ID { get; set; }
        public String MKR_DT { get; set; }
        public String AUTH_DT { get; set; }
        public String ATV { get; set; }
    }
}