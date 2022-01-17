using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AcctNiceModel
/// </summary>
public class AcctNiceModel
{
    public string condition { get; set; }
    public string acctSearchString { get; set; }
    public string choseAcctNo { get; set; }
    public List<AcctNiceItemModel> listAcctNice { get; set; }
    public String receivingAccountNo { get; set; }
    public String displayAccountNo { get; set; }
    public String transferAmount { get; set; }
    public String transferAccountNo { get; set; }
    public String transferCurrency { get; set; }
    public String transferAccountNoAndCurrentBalance { get; set; }
    public String referralCode { get; set; }
    public String content { get; set; }
    public Double currentBalance { get; set; }
    public String transferAccountName { get; set; }
    public double transferVAT { get; set; }
    public double countNextOther { get; set; }
    public string refNo { get; set; }
}

public class AcctNiceItemModel
{
    public string accountNo { get; set; }
    public string currencyCode { get; set; }
    public string currencyName { get; set; }
    public double feeAmount { get; set; }
    public double vatAmount { get; set; }
    public double totalAmount { get; set; }
    public string groupType { get; set; }
    public double discountAmt { get; set; }
    public double discountFeeAmt { get; set; }
    public double discountVatAmt { get; set; }
    public bool freeCharge { get; set; }
}

public class AcctNiceQueryList
{
    public string accountNo { get; set; }
    public string accountType { get; set; }
    public double pageIndex { get; set; }
    public double pageSize { get; set; }
    public string channelId { get; set; }
}

public class AcctNiceCheckLogin
{
    public string channelId { get; set; }
    public string custId { get; set; }
    public string ebToken { get; set; }
    public string machine { get; set; }
}

public class PostAcctNineModel
{
    public string accountNo { get; set; }
}

public class AcctNiceProcessModel
{
    public String receivingAccountNo { get; set; }
    public String displayAccountNo { get; set; }
    public String transferAmount { get; set; }
    public String transferAccountNo { get; set; }
    public String transferCurrency { get; set; }
    public String transferAccountNoAndCurrentBalance { get; set; }
    public String referralCode { get; set; }
    public String content { get; set; }
    public Double currentBalance { get; set; }
    public String transferAccountName { get; set; }
}

public class AcctNiceCreateModel
{
    public string accountFinacialStatus { get; set; }
    public string accountFinacialSubStatus { get; set; }
    public string accountName { get; set; }
    public string accountNameInLocalLanguage { get; set; }
    public string accountNo { get; set; }
    public string accountStatus { get; set; }
    public string action { get; set; }
    public string aprDT { get; set; }
    public string aprID { get; set; }
    public string block { get; set; }
    public string branchCode { get; set; }
    public string charge { get; set; }
    public string chargeTotal { get; set; }
    public string chargeVAT { get; set; }
    public string cifNo { get; set; }
    public string country { get; set; }
    public string createType { get; set; }
    public string currencyCode { get; set; }
    public string currentComboCode { get; set; }
    public string district { get; set; }
    public string mkrDT { get; set; }
    public string mkrID { get; set; }
    public string newComboCode { get; set; }
    public string noteCode { get; set; }
    public string noteDesc { get; set; }
    public string passportFutureDateLateSubmission { get; set; }
    public string passportOriginal1 { get; set; }
    public string productCode { get; set; }
    public string residentPermFutureDateLateSubmission { get; set; }
    public string residentPermitCopy1 { get; set; }
    public string rmPrimaryCode { get; set; }
    public string rmPrimaryName { get; set; }
    public string rmSecondaryCode { get; set; }
    public string rmSecondaryName { get; set; }
    public string schemeCode { get; set; }
    public string securitiesCompany { get; set; }
    public string serviceTier { get; set; }
    public string statementCycle { get; set; }
    public string statementDeliveryMode { get; set; }
    public string street { get; set; }
    public string town { get; set; }
    public string ward { get; set; }
    public string zipCode { get; set; }
    public string feeAcct { get; set; }
    public string partnerBranchCode { get; set; }
}