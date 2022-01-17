using mobileGW.Service.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iBanking.CustomModels
{
    public class SavingMenuREQModel
    {
        public string cifNo { get; set; }
        public string channelId { get; set; }
        public string deviceInfo { get; set; }
    }

    public class SavingMenuRESModel : BaseResponse
    {
        public string jwtToken { get; set; }
        public string expireTime { get; set; }
        public string messageId { get; set; }
        public List<SavingProductModel> listProduct { get; set; }
    }

    public class SavingProductModel
    {
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string limitOnPromotion { get; set; }
        public string limitOnCustomer { get; set; }
        public string productCode { get; set; }
        public string productTypeVn { get; set; }
        public string productTypeEn { get; set; }
        public string titleVn { get; set; }
        public string titleEn { get; set; }
        public string descriptionVn { get; set; }
        public string descriptionEn { get; set; }
        public string isLinkPromotion { get; set; }
        public string typeSaving { get; set; }
        public string desPromotionVN { get; set; }
        public string desPromotionEN { get; set; }
    }
}