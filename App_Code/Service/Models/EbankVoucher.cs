using System;
using System.Collections.Generic;

namespace mobileGW.Service.API
{
    public class EbankVoucherBaseModel
    {
        public string VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string SerialNum { get; set; }
        public string PinNum { get; set; }
        public string VoucherDescriptionVn { get; set; }
        public string VoucherDescriptionEn { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ValueType { get; set; }
        public double AmountVal { get; set; }
        public string PercentVal { get; set; }
        public double MinTransAmount { get; set; }
        public double MaxTransAmout { get; set; }
        public string IsEligible { get; set; }
        public double RemainQuantity { get; set; }
        public double MaxQuantity { get; set; }
        public string CampName { get; set; }
        public string CampDescription { get; set; }
    }

    public class EbankVoucherUpdateModel
    {
        public string PinNum { get; set; }
        public string CustomerId { get; set; }
        public string ChannelId { get; set; }
        public string TranType { get; set; }
        public double TranAmount { get; set; }
        public double DiscountAmount { get; set; }
        public string TranRefNo { get; set; }

    }

    public class EbankVoucherCheckModel
    {
        public string PinNum { get; set; }
        public string CustomerId { get; set; }
        public string ChannelId { get; set; }
        public string TranType { get; set; }
        public double TranAmount { get; set; }
        public double DiscountAmount { get; set; }

    }

    public class EbankVoucherQueryModel
    {
        public string CustomerId { get; set; }
        public string ChannelId { get; set; }
        public string TranType { get; set; }
        public double TranAmount { get; set; }
    }
}
