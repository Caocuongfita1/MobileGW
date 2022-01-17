using mobileGW.Service.Bussiness;
using mobileGW.Service.Framework;
using mobileGW.Service.DataAccess;
using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Xml;

using System.Collections;
using System.Net;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for Financial_Transfer
/// </summary>
namespace mobileGW.Service.AppFuncs
{
    public class Transfer247
    {
        public Transfer247()
        {
            // accept SSL cert
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);
            //
        }

        public string GET_247_ACCT_HOLDER_NEW(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO"); //
            string acct_des = Funcs.getValFromHashtbl(hashTbl, "ACCTNO"); //
            acct_des = Funcs.RemoveSpecialCharacters(acct_des);
            Funcs.WriteLog("GET_247_ACCT_HOLDER_NEW: " + custid + "-" + acct_des);
            string bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE"); //	
            string bank_code_247 = "";

            //CHECK bank_code co trong list CK 247 khong ?

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            string retStr = Config.GET_247_ACCT_HOLDER;
            try
            {
                DataTable dt = new DataTable();
                Transfer247s getBankCode247 = new Transfer247s();
                dt = getBankCode247.getBankCode247(bank_code);
                if (dt != null && dt.Rows.Count == 1)
                {
                    bank_code_247 = dt.Rows[0][TBL_EB_BEN.BANK_CODE].ToString();

                    if (!string.IsNullOrEmpty(bank_code_247))
                    {
                        if (bank_code_247.Equals("999999"))
                        {
                            retStr = Config.ERR_MSG_GENERAL;
                        }
                        else
                        {
                            //tungdt8 fix bug 06012020 check ki tu dac biet
                            Regex regex = new Regex(@"^[a-zA-Z0-9]*$"); //.*[0-9].*

                            Match match = regex.Match(acct_des);

                            if (!match.Success)
                            {
                                return Config.ERR_MSG_GENERAL;
                            }

                            //tungdt8 fix bug 20122019
                            if (Funcs.getConfigVal("IBFT_VERSION").Equals("2.0"))
                            {
                                retStr = QueryAccHolderName(custid, acct_des, bank_code_247);
                            }
                            else
                            {
                                retStr = QueryAccHolderName_V1(custid, acct_des, bank_code_247);
                            }
                        }
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
                Funcs.WriteLog("QUERY ACCOUNT TO SMARTLINK FAILED. EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            return retStr;
        }

        public string GET_247_CARD_HOLDER(Hashtable hashTbl)
        {

            string card_no = Funcs.getValFromHashtbl(hashTbl, "CARD_NO"); //
            card_no = Funcs.RemoveSpecialCharacters(card_no);
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO"); // query ko can so tai khoan nguon --> random guid
            string retStr = "";

            //tungdt8 fix bug 06012020 check ki tu dac biet
            Regex regex = new Regex(@"^[a-zA-Z0-9]*$"); //.*[0-9].*

            Match match = regex.Match(card_no);

            if (!match.Success)
            {
                return Config.ERR_MSG_GENERAL;
            }

            if (Funcs.getConfigVal("IBFT_VERSION").Equals("2.0"))
            {
                retStr = QueryCardHolderName(card_no, custid);
            }
            else
            {
                retStr = QueryCardHolderName_V1(card_no, custid);
            }

            return retStr;
        }

        public string QueryCardHolderName_V1(string card_no, string custid)
        {
            string card_holder_name = "";
            string bank_name = "";
            string errCode = "";
            string errDesc = "";
            string retStr = Config.GET_247_CARD_HOLDER;

            Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER  BEGIN");

            DataTable dt = new DataTable();
            Transfer247s bin2bankname = new Transfer247s();
            dt = bin2bankname.getBankNameByBIN(card_no.Substring(0, 6));
            if (dt != null && dt.Rows.Count == 1)
            {
                bank_name = dt.Rows[0][TBL_EB_BEN.BANK_NAME].ToString();

                IBXferV1.IBXferInquiryResType res = null;
                IBXferV1.AppHdrType appHdr = new IBXferV1.AppHdrType();
                appHdr.CharSet = "UTF-8";
                appHdr.SvcVer = "1.1";

                IBXferV1.PairsType nsFrom = new IBXferV1.PairsType();
                nsFrom.Id = "ESB";
                nsFrom.Name = "ESB";

                IBXferV1.PairsType nsTo = new IBXferV1.PairsType();
                nsTo.Id = "CORE";
                nsTo.Name = "CORE";

                IBXferV1.PairsType[] listOfNsTo = new IBXferV1.PairsType[1];
                listOfNsTo[0] = nsTo;

                IBXferV1.PairsType BizSvc = new IBXferV1.PairsType();
                BizSvc.Id = "InterBankQuery";
                BizSvc.Name = "InterBankQuery";

                DateTime TransDt = DateTime.Now;

                appHdr.From = nsFrom;
                appHdr.To = listOfNsTo;
                appHdr.MsgId = Funcs.GenESBMsgId();
                appHdr.MsgPreId = "";
                appHdr.BizSvc = BizSvc;
                appHdr.TransDt = TransDt;

                //Body
                IBXferV1.IBXferInquiryReqType msgReq = new IBXferV1.IBXferInquiryReqType();
                msgReq.AppHdr = appHdr;
                msgReq.DbAcctId = custid;
                msgReq.ServiceCd = "430000";  //430000 cua the, 432020 cua tai khoan. 27/11/2018: IBFT: 430000 -> 432000
                msgReq.CrAcctId = card_no;
                msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
                msgReq.MerchantType = "7399";
                msgReq.TermId = "11111111";
                msgReq.CardAcceptor = "SHB".PadRight(40, ' ');
                msgReq.BenId = "";
                msgReq.Remark = "SHB MB ENQUIRY ACCT HOLDER NAME BY CARD NO";

                msgReq.AuditNo = Funcs.GetLast(new Payments().getNextTranId(), 6); // so giao dich la 6 ky tu


                //portypeClient
                try
                {
                    IBXferV1.PortTypeClient ptc = new IBXferV1.PortTypeClient();
                    res = ptc.Inquiry(msgReq);

                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    ptc.Close();
                }
                catch (Exception e)
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER|" + e.ToString());
                }

                Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER NAPAS RESPONSE CODE CARD ENQUIRY CARDNO:" + card_no + "|RET:" + res.RespSts.Sts.ToString());


                if (res == null || !res.RespSts.Sts.Equals("0"))
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER QUERY CARD HOLDER NAME TO NAPAS FAILED. Code return : " + res.RespSts.Sts.ToString() + ". ACCOUNT NAME IS EMPTY");
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;
                }
                else //tra cuu thong tin chu the thanh cong
                {
                    if (res.AcctName.Trim().Equals(string.Empty)) //tra cuu thanh cong nhung ten chu the = ""
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        return retStr;
                    }
                    else
                    {
                        card_holder_name = res.AcctName;
                        errCode = Config.ERR_CODE_DONE;
                        errDesc = "GET CARD HOLDER NAME BY CARD NO SMLGW SUCCESSFULL";

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, errCode);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, errDesc);
                        retStr = retStr.Replace("{BEN_NAME}", card_holder_name);
                        retStr = retStr.Replace("{BANK_NAME}", bank_name);

                        Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER  END");

                        return retStr;

                    }
                }
            }
            else //khong lay duoc ten ngan hang tu dau BIN cua the
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        public string QueryCardHolderName(string card_no, string custid)
        {
            string card_holder_name = "";
            string bank_name = "";
            string errCode = "";
            string errDesc = "";
            string retStr = Config.GET_247_CARD_HOLDER;

            Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER  BEGIN");

            DataTable dt = new DataTable();
            Transfer247s bin2bankname = new Transfer247s();
            dt = bin2bankname.getBankNameByBIN(card_no.Substring(0, 6));
            if (dt != null && dt.Rows.Count == 1)
            {
                bank_name = dt.Rows[0][TBL_EB_BEN.BANK_NAME].ToString();

                IBXfer.IBXferInquiryResType res = null;
                IBXfer.AppHdrType appHdr = new IBXfer.AppHdrType();
                appHdr.CharSet = "UTF-8";
                appHdr.SvcVer = "2.0";

                IBXfer.PairsType nsFrom = new IBXfer.PairsType();
                nsFrom.Id = "ESB";
                nsFrom.Name = "ESB";

                IBXfer.PairsType nsTo = new IBXfer.PairsType();
                nsTo.Id = "CORE";
                nsTo.Name = "CORE";

                IBXfer.PairsType[] listOfNsTo = new IBXfer.PairsType[1];
                listOfNsTo[0] = nsTo;

                IBXfer.PairsType BizSvc = new IBXfer.PairsType();
                BizSvc.Id = "InterBankQuery";
                BizSvc.Name = "InterBankQuery";

                DateTime TransDt = DateTime.Now;

                appHdr.From = nsFrom;
                appHdr.To = listOfNsTo;
                appHdr.MsgId = Funcs.GenESBMsgId();
                appHdr.MsgPreId = "";
                appHdr.BizSvc = BizSvc;
                appHdr.TransDt = TransDt;

                //Body
                IBXfer.IBXferInquiryReqType msgReq = new IBXfer.IBXferInquiryReqType();
                msgReq.AppHdr = appHdr;
                msgReq.DbAcctId = custid;
                msgReq.ServiceCd = "432000";  //430000 cua the, 432020 cua tai khoan. 27/11/2018: IBFT: 430000 -> 432000
                msgReq.CrAcctId = card_no;
                msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
                msgReq.MerchantType = "7399";
                msgReq.TermId = "11111111";
                msgReq.CardAcceptor = "SHB".PadRight(40, ' ');
                msgReq.BenId = "";
                msgReq.Remark = "SHB MB ENQUIRY ACCT HOLDER NAME BY CARD NO";

                msgReq.AuditNo = Funcs.GetLast(new Payments().getNextTranId(), 6); // so giao dich la 6 ky tu


                //portypeClient
                try
                {
                    IBXfer.PortTypeClient ptc = new IBXfer.PortTypeClient();
                    res = ptc.Inquiry(msgReq);

                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    ptc.Close();
                }
                catch (Exception e)
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER|" + e.ToString());
                }

                Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER NAPAS RESPONSE CODE CARD ENQUIRY CARDNO:" + card_no + "|RET:" + res.RespSts.Sts.ToString());


                if (res == null || !res.RespSts.Sts.Equals("0"))
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER QUERY CARD HOLDER NAME TO NAPAS FAILED. Code return : " + res.RespSts.Sts.ToString() + ". ACCOUNT NAME IS EMPTY");
                    retStr = Config.ERR_MSG_GENERAL;
                    return retStr;
                }
                else //tra cuu thong tin chu the thanh cong
                {
                    if (res.AcctName.Trim().Equals(string.Empty)) //tra cuu thanh cong nhung ten chu the = ""
                    {
                        retStr = Config.ERR_MSG_GENERAL;
                        return retStr;
                    }
                    else
                    {
                        card_holder_name = res.AcctName;
                        errCode = Config.ERR_CODE_DONE;
                        errDesc = "GET CARD HOLDER NAME BY CARD NO SMLGW SUCCESSFULL";

                        retStr = retStr.Replace(Config.ERR_CODE_VAL, errCode);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, errDesc);
                        retStr = retStr.Replace("{BEN_NAME}", card_holder_name);
                        retStr = retStr.Replace("{BANK_NAME}", bank_name);

                        Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER  END");

                        return retStr;

                    }
                }
            }
            else //khong lay duoc ten ngan hang tu dau BIN cua the
            {
                retStr = Config.ERR_MSG_GENERAL;
                return retStr;
            }
        }

        public string QueryCardHolderNamePhase2(string _card_no, string custid)
        {
            string retStr = Config.ERR_CODE_GENERAL;
            string card_no = Funcs.RemoveSpecialCharacters(_card_no);
            string bank_name = "";


            Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER PHASE2 BEGIN");

            DataTable dt = new DataTable();
            Transfer247s bin2bankname = new Transfer247s();
            dt = bin2bankname.getBankNameByBIN(card_no.Substring(0, 6));
            if (dt != null && dt.Rows.Count == 1)
            {
                bank_name = dt.Rows[0][TBL_EB_BEN.BANK_NAME].ToString();

                IBXfer.IBXferInquiryResType res = null;
                IBXfer.AppHdrType appHdr = new IBXfer.AppHdrType();
                appHdr.CharSet = "UTF-8";
                appHdr.SvcVer = "2.0";

                IBXfer.PairsType nsFrom = new IBXfer.PairsType();
                nsFrom.Id = "ESB";
                nsFrom.Name = "ESB";

                IBXfer.PairsType nsTo = new IBXfer.PairsType();
                nsTo.Id = "CORE";
                nsTo.Name = "CORE";

                IBXfer.PairsType[] listOfNsTo = new IBXfer.PairsType[1];
                listOfNsTo[0] = nsTo;

                IBXfer.PairsType BizSvc = new IBXfer.PairsType();
                BizSvc.Id = "InterBankQuery";
                BizSvc.Name = "InterBankQuery";

                DateTime TransDt = DateTime.Now;

                appHdr.From = nsFrom;
                appHdr.To = listOfNsTo;
                appHdr.MsgId = Funcs.GenESBMsgId();
                appHdr.MsgPreId = "";
                appHdr.BizSvc = BizSvc;
                appHdr.TransDt = TransDt;

                //Body
                IBXfer.IBXferInquiryReqType msgReq = new IBXfer.IBXferInquiryReqType();
                msgReq.AppHdr = appHdr;
                msgReq.DbAcctId = custid;
                msgReq.ServiceCd = "432000";  //430000 cua the, 432020 cua tai khoan. 27/11/2018: IBFT: 430000 -> 432000
                msgReq.CrAcctId = card_no;
                msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
                msgReq.MerchantType = "7399";
                msgReq.TermId = "11111111";
                msgReq.CardAcceptor = "SHB".PadRight(40, ' ');
                msgReq.BenId = "";
                msgReq.Remark = "SHB MB ENQUIRY ACCT HOLDER NAME BY CARD NO";

                msgReq.AuditNo = Funcs.GetLast(new Payments().getNextTranId(), 6); // so giao dich la 6 ky tu


                //portypeClient
                try
                {
                    IBXfer.PortTypeClient ptc = new IBXfer.PortTypeClient();
                    res = ptc.Inquiry(msgReq);

                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
                    ptc.Close();
                }
                catch (Exception e)
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER|" + e.ToString());
                    retStr = Config.ERR_CODE_GENERAL;
                    return retStr;
                }

                Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER NAPAS RESPONSE CODE CARD ENQUIRY CARDNO:" + card_no + "|RET:" + res.RespSts.Sts.ToString());


                if (res == null || !res.RespSts.Sts.Equals("0"))
                {
                    Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER QUERY CARD HOLDER NAME TO NAPAS FAILED. Code return : " + res.RespSts.Sts.ToString() + ". ACCOUNT NAME IS EMPTY");
                    retStr = Config.ERR_CODE_GENERAL;
                    return retStr;
                }
                else //tra cuu thong tin chu the thanh cong
                {
                    if (res.AcctName.Trim().Equals(string.Empty)) //tra cuu thanh cong nhung ten chu the = ""
                    {
                        retStr = Config.ERR_CODE_GENERAL;
                        return retStr;
                    }
                    else
                    {
                        retStr = Config.ERR_CODE_DONE;
                        Funcs.WriteLog("custid:" + custid + "|GET_247_CARD_HOLDER PHASE2 END");
                        return retStr;

                    }
                }
            }
            else //khong lay duoc ten ngan hang tu dau BIN cua the
            {
                retStr = Config.ERR_CODE_GENERAL;
                return retStr;
            }

        }

        public string GET_247_ACCT_HOLDER(Hashtable hashTbl)
        {
            string custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO"); //
            string acct_des = Funcs.getValFromHashtbl(hashTbl, "ACCTNO"); //	
            string bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE"); //	
            string retStr = "";

            acct_des = Funcs.RemoveSpecialCharacters(acct_des);

            //tungdt8 fix bug 06012020 check ki tu dac biet
            Regex regex = new Regex(@"^[a-zA-Z0-9]*$"); //.*[0-9].*

            Match match = regex.Match(acct_des);

            if (!match.Success)
            {
                return Config.ERR_MSG_GENERAL;
            }

            if (Funcs.getConfigVal("IBFT_VERSION").Equals("2.0"))
            {
                retStr = QueryAccHolderName(custid, acct_des, bank_code);
            }
            else
            {
                retStr = QueryAccHolderName_V1(custid, acct_des, bank_code);
            }

            return retStr;
        }


        public string QueryAccHolderName(string custid, string acct_des, string bank_code)
        {
            Funcs.WriteLog("custid:" + custid + "|GET_247_ACCT_HOLDER BEGIN");

            string retStr = Config.GET_247_ACCT_HOLDER;
            string acct_name = string.Empty;
            try
            {
                //retStr = Config.GET_ACCOUNT_INFO_SMLGW;

                IBXfer.IBXferInquiryResType res = null;
                IBXfer.AppHdrType appHdr = new IBXfer.AppHdrType();
                appHdr.CharSet = "UTF-8";
                appHdr.SvcVer = "2.0";//2.0

                IBXfer.PairsType nsFrom = new IBXfer.PairsType();
                nsFrom.Id = "ESB";
                nsFrom.Name = "ESB";

                IBXfer.PairsType nsTo = new IBXfer.PairsType();
                nsTo.Id = "CORE";
                nsTo.Name = "CORE";

                IBXfer.PairsType[] listOfNsTo = new IBXfer.PairsType[1];
                listOfNsTo[0] = nsTo;

                IBXfer.PairsType BizSvc = new IBXfer.PairsType();
                BizSvc.Id = "InterBankQuery";
                BizSvc.Name = "InterBankQuery";

                DateTime TransDt = DateTime.Now;

                appHdr.From = nsFrom;
                appHdr.To = listOfNsTo;
                appHdr.MsgId = Funcs.GenESBMsgId();
                appHdr.MsgPreId = "";
                appHdr.BizSvc = BizSvc;
                appHdr.TransDt = TransDt;

                //Body
                IBXfer.IBXferInquiryReqType msgReq = new IBXfer.IBXferInquiryReqType();
                msgReq.AppHdr = appHdr;
                msgReq.DbAcctId = custid;
                msgReq.ServiceCd = "432020";  //430000 cua the, 432020 cua tai khoan
                msgReq.CrAcctId = acct_des;
                msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
                msgReq.MerchantType = "7399";
                msgReq.TermId = "11111111";
                msgReq.CardAcceptor = "SHB".PadRight(40, ' ');
                msgReq.BenId = bank_code;
                msgReq.Remark = "SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO";

                msgReq.AuditNo = Funcs.GetLast(new Payments().getNextTranId(), 6); // so giao dich la 6 ky tu

                Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO REQUEST" +
                    Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

                //portypeClient
                try
                {
                    IBXfer.PortTypeClient ptc = new IBXfer.PortTypeClient();
                    res = ptc.Inquiry(msgReq);

                    Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                    ptc.Close();
                }
                catch (Exception e)
                {
                    //  Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusId);
                    Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO" + e.ToString());
                }

                if (res == null || !res.RespSts.Sts.Equals("0"))
                {
                    Funcs.WriteLog("QUERY ACCOUNT TO SMARTLINK FAILED. Code return : " + res.RespSts.Sts + ". ACCOUNT NAME IS EMPTY");
                    retStr = Config.ERR_MSG_GENERAL;
                }
                else
                {
                    acct_name = res.AcctName;
                    Funcs.WriteLog(string.Format("QUERY ACCOUNT TO SMARTLINK SUCCESS. ACCOUNT_NAME: {0}", acct_name));
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "GET ACCOUNT NAME SUCCESSFUL");
                    retStr = retStr.Replace("{ACCT_NO}", acct_des);
                    retStr = retStr.Replace("{BEN_NAME}", acct_name);
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("QUERY ACCOUNT TO SMARTLINK FAILED. EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            Funcs.WriteLog("custid:" + custid + "|GET_247_ACCT_HOLDER  BEGIN");

            return retStr;
        }

        public string QueryAccHolderName_V1(string custid, string acct_des, string bank_code)
        {
            Funcs.WriteLog("custid:" + custid + "|GET_247_ACCT_HOLDER BEGIN");

            string retStr = Config.GET_247_ACCT_HOLDER;
            string acct_name = string.Empty;
            try
            {
                //retStr = Config.GET_ACCOUNT_INFO_SMLGW;

                IBXferV1.IBXferInquiryResType res = null;
                IBXferV1.AppHdrType appHdr = new IBXferV1.AppHdrType();
                appHdr.CharSet = "UTF-8";
                appHdr.SvcVer = "1.1";//2.0

                IBXferV1.PairsType nsFrom = new IBXferV1.PairsType();
                nsFrom.Id = "ESB";
                nsFrom.Name = "ESB";

                IBXferV1.PairsType nsTo = new IBXferV1.PairsType();
                nsTo.Id = "CORE";
                nsTo.Name = "CORE";

                IBXferV1.PairsType[] listOfNsTo = new IBXferV1.PairsType[1];
                listOfNsTo[0] = nsTo;

                IBXferV1.PairsType BizSvc = new IBXferV1.PairsType();
                BizSvc.Id = "InterBankQuery";
                BizSvc.Name = "InterBankQuery";

                DateTime TransDt = DateTime.Now;

                appHdr.From = nsFrom;
                appHdr.To = listOfNsTo;
                appHdr.MsgId = Funcs.GenESBMsgId();
                appHdr.MsgPreId = "";
                appHdr.BizSvc = BizSvc;
                appHdr.TransDt = TransDt;

                //Body
                IBXferV1.IBXferInquiryReqType msgReq = new IBXferV1.IBXferInquiryReqType();
                msgReq.AppHdr = appHdr;
                msgReq.DbAcctId = custid;
                msgReq.ServiceCd = "432020";  //430000 cua the, 432020 cua tai khoan
                msgReq.CrAcctId = acct_des;
                msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
                msgReq.MerchantType = "7399";
                msgReq.TermId = "11111111";
                msgReq.CardAcceptor = "SHB".PadRight(40, ' ');
                msgReq.BenId = bank_code;
                msgReq.Remark = "SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO";

                msgReq.AuditNo = Funcs.GetLast(new Payments().getNextTranId(), 6); // so giao dich la 6 ky tu

                Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO REQUEST" +
                    Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

                //portypeClient
                try
                {
                    IBXferV1.PortTypeClient ptc = new IBXferV1.PortTypeClient();
                    res = ptc.Inquiry(msgReq);

                    Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                    ptc.Close();
                }
                catch (Exception e)
                {
                    //  Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusId);
                    Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO" + e.ToString());
                }

                if (res == null || !res.RespSts.Sts.Equals("0"))
                {
                    Funcs.WriteLog("QUERY ACCOUNT TO SMARTLINK FAILED. Code return : " + res.RespSts.Sts + ". ACCOUNT NAME IS EMPTY");
                    retStr = Config.ERR_MSG_GENERAL;
                }
                else
                {
                    acct_name = res.AcctName;
                    Funcs.WriteLog(string.Format("QUERY ACCOUNT TO SMARTLINK SUCCESS. ACCOUNT_NAME: {0}", acct_name));
                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.ERR_CODE_DONE);
                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "GET ACCOUNT NAME SUCCESSFUL");
                    retStr = retStr.Replace("{ACCT_NO}", acct_des);
                    retStr = retStr.Replace("{BEN_NAME}", acct_name);
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("QUERY ACCOUNT TO SMARTLINK FAILED. EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_MSG_GENERAL;
            }

            Funcs.WriteLog("custid:" + custid + "|GET_247_ACCT_HOLDER  BEGIN");

            return retStr;
        }

        public string QueryAccHolderNamePhase2(string custid, string acct_des, string bank_code)
        {
            Funcs.WriteLog("custid:" + custid + "|GET_247_ACCT_HOLDER PHASE2 BEGIN");

            string retStr = Config.ERR_CODE_GENERAL;
            string acct_name = string.Empty;
            try
            {
                //retStr = Config.GET_ACCOUNT_INFO_SMLGW;

                IBXfer.IBXferInquiryResType res = null;
                IBXfer.AppHdrType appHdr = new IBXfer.AppHdrType();
                appHdr.CharSet = "UTF-8";
                appHdr.SvcVer = "2.0";//2.0

                IBXfer.PairsType nsFrom = new IBXfer.PairsType();
                nsFrom.Id = "ESB";
                nsFrom.Name = "ESB";

                IBXfer.PairsType nsTo = new IBXfer.PairsType();
                nsTo.Id = "CORE";
                nsTo.Name = "CORE";

                IBXfer.PairsType[] listOfNsTo = new IBXfer.PairsType[1];
                listOfNsTo[0] = nsTo;

                IBXfer.PairsType BizSvc = new IBXfer.PairsType();
                BizSvc.Id = "InterBankQuery";
                BizSvc.Name = "InterBankQuery";

                DateTime TransDt = DateTime.Now;

                appHdr.From = nsFrom;
                appHdr.To = listOfNsTo;
                appHdr.MsgId = Funcs.GenESBMsgId();
                appHdr.MsgPreId = "";
                appHdr.BizSvc = BizSvc;
                appHdr.TransDt = TransDt;

                //Body
                IBXfer.IBXferInquiryReqType msgReq = new IBXfer.IBXferInquiryReqType();
                msgReq.AppHdr = appHdr;
                msgReq.DbAcctId = custid;
                msgReq.ServiceCd = "432020";  //430000 cua the, 432020 cua tai khoan
                msgReq.CrAcctId = acct_des;
                msgReq.TxnDt = DateTime.Now.ToString("yyyyMMdd");
                msgReq.MerchantType = "7399";
                msgReq.TermId = "11111111";
                msgReq.CardAcceptor = "SHB".PadRight(40, ' ');
                msgReq.BenId = bank_code;
                msgReq.Remark = "SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO";

                msgReq.AuditNo = Funcs.GetLast(new Payments().getNextTranId(), 6); // so giao dich la 6 ky tu

                Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO REQUEST" +
                    Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(msgReq)));

                //portypeClient
                try
                {
                    IBXfer.PortTypeClient ptc = new IBXfer.PortTypeClient();
                    res = ptc.Inquiry(msgReq);

                    Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

                    ptc.Close();
                }
                catch (Exception e)
                {
                    //  Helper.WriteLog(l4NC, e.Message + e.StackTrace, cusId);
                    Funcs.WriteLog("custid:" + custid + "|SHB MB ENQUIRY ACCT HOLDER NAME BY ACCT NO" + e.ToString());
                    retStr = Config.ERR_CODE_GENERAL;
                }

                if (res == null || !res.RespSts.Sts.Equals("0"))
                {
                    Funcs.WriteLog("QUERY ACCOUNT TO SMARTLINK FAILED. Code return : " + res.RespSts.Sts + ". ACCOUNT NAME IS EMPTY");
                    retStr = Config.ERR_CODE_GENERAL;
                }
                else
                {
                    acct_name = res.AcctName;
                    Funcs.WriteLog(string.Format("QUERY ACCOUNT TO SMARTLINK SUCCESS. ACCOUNT_NAME: {0}", acct_name));
                    retStr = Config.ERR_CODE_DONE;
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog("QUERY ACCOUNT TO SMARTLINK FAILED. EXCEPTION: " + ex.ToString());
                retStr = Config.ERR_CODE_GENERAL;
            }

            Funcs.WriteLog("custid:" + custid + "|GET_247_ACCT_HOLDER PHASE2  END");

            return retStr;
        }

        public string TRANSFER247(Hashtable hashTbl, string ip, string user_agent)
        {

            //CMD#TRANSFER_247|TRAN_TYPE#ACQ_247AC|CIF_NO#0310008705|SRC_ACCT#1000013376|DES_ACCT#711A239398391|AMOUNT#20000000|ACCT_NAME#NGUYEN VAN A|BANK_CODE#977777|BANK_NAME#Ngân hàng ngoại thương|TXDESC#CKMOBILE|TRANPWD#fksdfjf385738jsdfjsdf9|SAVE_TO_BENLIST#1|TOKEN#XXXXXXXXXXXXXXXXXXXXXXXXXX";
            string custid = "";
            custid = Funcs.getValFromHashtbl(hashTbl, "CIF_NO");
            string src_acct = "";
            string des_acct = "";
            double amount = 0;
            string bank_code;
            string txDesc = "";
            string save_to_benlist = "";
            string des_name = "";
            double eb_tran_id = 0;
            string bank_name = "";
            string tranfer_fee = "";

            src_acct = Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT");
            des_acct = Funcs.getValFromHashtbl(hashTbl, "DES_ACCT");

            des_acct = Funcs.RemoveSpecialCharacters(des_acct.Trim());

            //tungdt8 fix bug 06012020 check ki tu dac biet
            Regex regex = new Regex(@"^[a-zA-Z0-9]*$"); //.*[0-9].*

            Match match = regex.Match(des_acct);

            if (!match.Success)
            {
                return Config.ERR_MSG_GENERAL;
            }

            amount = double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT").Replace(",", "").Replace(".", ""));
            bank_code = Funcs.getValFromHashtbl(hashTbl, "BANK_CODE");
            save_to_benlist = Funcs.getValFromHashtbl(hashTbl, "SAVE_TO_BENLIST");
            des_name = Funcs.getValFromHashtbl(hashTbl, "ACCT_NAME");
            bank_name = Funcs.getValFromHashtbl(hashTbl, "BANK_NAME");
            tranfer_fee = Funcs.getValFromHashtbl(hashTbl, "TRANSFER_FEE");

            //des_name = (des_name == Config.NULL_VALUE ? "" : des_acct);

            des_name = (des_name == Config.NULL_VALUE ? "" : des_name);

            txDesc = Funcs.getValFromHashtbl(hashTbl, "TXDESC");

            txDesc = Funcs.RemoveSpecialCharacters(txDesc);

            string pwd = Funcs.getValFromHashtbl(hashTbl, "TRANPWD");

            string tran_type = Funcs.getValFromHashtbl(hashTbl, "TRAN_TYPE");

            #region FOR TOKEN
            string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
            string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
            int typeOtp = Int16.Parse(typeOtpStr);

            if (typeOtp == 2)
            {
                pwd = Funcs.encryptMD5(pwd + custid);
            }

            #endregion

            double total_fee = 0;
            double fee_amt = 0;
            double vat_amt = 0;

            //Tinh lai fee dau server: khong de client gui fee len
            bool get_total_fee = false;

            if (!Funcs.getConfigVal("CHARGE_FEE_VER").Equals("2.0"))
            {
                get_total_fee = Funcs.getTotalFee(custid, Config.ChannelID, tran_type, amount, src_acct, des_acct, bank_code, out total_fee, out fee_amt, out vat_amt);
            }
            else
            {
                get_total_fee = AccIntegration.GET_CHARGE_FEE(custid, src_acct, Config.ChannelID, tran_type, amount, des_acct, bank_code, "704", ref total_fee, ref fee_amt, ref vat_amt);
            }

            if (!get_total_fee)
            {
                Funcs.WriteLog("custid:" + custid + "|TRANSFER247  ERROR GET FEE: SRC_ACC: " + src_acct);
                return Config.ERR_MSG_GENERAL;
            }

            string check_before_trans = "";

            string gl_suspend = Config.ACCT_SUSPEND_NAPAS_247ACCT;
            string gl_fee = Config.ACCT_FEE_NAPAS_247ACCT;
            string gl_vat = Config.ACCT_VAT_NAPAS_247ACCT;
            string pos_cd = Config.HO_BR_CODE;
            string core_txno_ref = "";
            string core_txdate_ref = "";
            string ws_info_ref = "";
            string retStr = Config.TRANSFER_247;
            string channel_id = Config.ChannelID;

            Funcs.WriteLog("custid:" + custid + "|TRANSFER247  BEGIN DES_ACCT:" + des_acct);

            if (tranfer_fee.Equals(Config.TRANFER_FEE_ACQ))
            {
                //amount = amount;
            }
            else if (tranfer_fee.Equals(Config.TRANFER_FEE_BEN))
            {
                if (amount > total_fee)
                    amount = amount - total_fee;
                else
                    return Config.ERR_MSG_AMOUNT_LESS_FEE;
            }
            else
            {
                return Config.ERR_MSG_GENERAL;
            }

            try
            {
                bool check = CoreIntegration.CheckAccountBelongCif(custid, src_acct, "CASA");
                if (!check)
                {
                    return Config.ERR_MSG_GENERAL;
                }

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
                    DataTable dt1 = new DataTable();
                    string bank_code_247 = string.Empty;
                    Transfer247s getBankCode247 = new Transfer247s();
                    dt1 = getBankCode247.getBankCode247(bank_code);
                    if (dt1 != null && dt1.Rows.Count == 1)
                        bank_code_247 = dt1.Rows[0][TBL_EB_BEN.BANK_CODE].ToString();
                    else
                    {
                        Funcs.WriteLog("GET BANK_CODE_247 FAILED: Code citad have not 247");
                        return Config.ERR_MSG_GENERAL;
                    }

                    Transfers transfer = new Transfers();
                    DataTable eb_tran = new DataTable();

                    Funcs.WriteLog("custid:" + custid + "|TRANSFER247  DES_ACCT:" + des_acct
                        + "BEGIN INSERT EB TRAN");


                    #region "insert TBL_EB_TRAN"
                    eb_tran = transfer.insTransferTx(
                    Config.ChannelID
                    , "TF247" //mod_cd
                    , tran_type //tran_type
                    , custid //custid
                    , src_acct//src_acct
                    , des_acct //des_acct
                    //linhtn hot fix 2017 01 04: truong nay luu amount + total fee
                    , total_fee + amount //amount
                    , "VND" //ccy_cd
                    , 1//convert rate
                    , total_fee + amount //lcy_amount
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
                    , gl_suspend //""//suspend account
                    , gl_fee //""//fee account
                    , gl_vat //""//vat account
                    , amount //suppend amount
                    , fee_amt //fee amount
                    , vat_amt //vat amount
                    , "" // des name ten tai khoan thu huogn
                    , bank_code // bank code
                    , bank_name // ten ngan hang  //linhtn add new 2017 02 21: luu them bank_name
                    , "" // ten thanh pho
                    , "" // ten chi nhanh
                    , des_name //"" //bm1  //linhtn add new 2017 02 21: luu them ten nguoi thu huong
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
                    , tranfer_fee // type tinh phi
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
                    , user_agent // ""//bm29                   
                );

                    #endregion "insert TBL_EB_TRAN"
                    //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
                    if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
                    {
                        eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                    }

                    //fix for FINPOST (bi loi khi messsage length = 999
                    if (total_fee.ToString().Length + amount.ToString().Length + 4 * txDesc.Length == 140)
                    {
                        txDesc += " ";
                        //Funcs.WriteInfo("CIF:" + Session["Login"].ToString() + ":SML TIMEOUT POST TO CORE DES = " + lblDesc.Text + " AMOUNT: " + lblAmount.Text);
                    }

                    string result = "";
                    Transfer247s tf247 = new Transfer247s();

                    //result = smls.ptsSMLTranfer(
                    //                                tran_id,
                    //                                src_acct,
                    //                                Double.Parse(amount),
                    //                                Config.SML_FEE_AMT,
                    //                                Config.SML_VAT_AMT,
                    //                                txDesc
                    //                                );

                    //HACH TOAN  VAO CORE

                    Funcs.WriteLog("custid:" + custid + "|TRANSFER247  DES_ACCT:" + des_acct
                      + "BEGIN POST TO CORE");

                    //result = tf247.postTF247ToCore(
                    //result = CoreIntegration.postTF247ToCore(
                    result = CoreIntegration.postFINPOSTToCore(
                    custid
                    , tran_type
                    , eb_tran_id
                    , src_acct
                    , gl_suspend
                    , gl_fee
                    , gl_vat
                    , amount
                    , fee_amt
                    , vat_amt
                    , txDesc
                    , pos_cd
                    , ref core_txno_ref
                    , ref core_txdate_ref
                    );

                    #region "POST TO CORE DONE"
                    if (result == Config.gResult_INTELLECT_Arr[0])
                    {

                        Funcs.WriteLog("custid:" + custid + "|TRANSFER247  DES_ACCT:" + des_acct
              + "POST TO CORE DONE --> CALL NAPAS WS");

                        string retWS = "";

                        if (Funcs.getConfigVal("IBFT_VERSION").Equals("2.0"))
                        {
                            retWS = IBT_integration.postTF247toNAPAS(
                                    eb_tran_id.ToString()
                                    , custid
                                    , tran_type
                                    , src_acct
                                    , des_acct
                                    , amount
                                    , txDesc
                                    , des_name
                                    , bank_code_247
                                    , ref ws_info_ref);
                        }
                        else
                        {
                            retWS = IBT_integration.postTF247toNAPASV1(
                                    eb_tran_id.ToString()
                                    , custid
                                    , tran_type
                                    , src_acct
                                    , des_acct
                                    , amount
                                    , txDesc
                                    , des_name
                                    , bank_code_247
                                    , ref ws_info_ref);
                        }


                        Funcs.WriteLog("custid:" + custid + "|TRANSFER247 DES_ACCT:" + des_acct
           + "|CALL NAPAS WS DONE RETWS:" + retWS);

                        //if ((retWS == Config.gResult_SML_Arr[0]) || (retWS == Config.gResult_SML_Arr[2]))

                        if (retWS == Config.ERR_CODE_DONE)
                        {
                            //xu ly tiep save to ben list o duoi

                        }
                        if (retWS == Config.ERR_CODE_TIMEOUT)
                        {
                            //TRUONG HOP TIMEOUT VAN TINH LA STATUS = 1 DE CONG HAN MUC
                            transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, channel_id);

                            return Config.ERR_MSG_TIMEOUT;

                        }
                        // endif neu topup thanh cong
                        if (retWS == Config.ERR_CODE_REVERT)// neu tra ve loi cu the thi revert                                    
                        {
                            //string revStr = transfer.revTransderTx(eb_tran_id, txDesc);

                            //string revStr = transfer.revFinPost(eb_tran_id, txDesc);
                            string revStr = CoreIntegration.revFinPost(eb_tran_id, txDesc);

                            if (revStr == Config.gResult_INTELLECT_Arr[0])
                            {
                                Funcs.WriteLog("custid:" + custid + "TRANSFER247 revert successfull tran_id:" + eb_tran_id);
                            }

                            else
                            {
                                Funcs.WriteLog("custid:" + custid + "TRANSFER247 revert not succesfull tran_id:" + eb_tran_id);
                            }
                            transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);

                            return Config.ERR_MSG_GENERAL;

                        }
                        if (retWS == Config.ERR_CODE_GENERAL)//                          
                        {
                            transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, channel_id);
                            return Config.ERR_MSG_GENERAL;
                        }


                        //CAP NHAT VAO BANG EB TRAN 
                        transfer.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, Config.ChannelID);

                        //SAVE TO BEN LIST
                        if (save_to_benlist == "0")
                        {
                            retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                            retStr = retStr.Replace(Config.ERR_DESC_VAL, "247 TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id.ToString());
                            retStr = retStr.Replace("{TRANID}", core_txno_ref);
                            retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                        }
                        else if (save_to_benlist == "1")
                        {

                            Funcs.WriteLog("custid:" + custid + "|TRANSFER247   DES_ACCT:" + des_acct
                             + "|BEGIN SAVE TO BEN LIST");
                            Beneficiarys ben = new Beneficiarys();
                            DataTable dt = new DataTable();
                            dt = ben.INSERT_BEN(
                                custid
                                , new Beneficiary().getTranTypePilot(custid, tran_type)
                                , des_acct
                                , des_name
                                , "" //des_nick_name
                                , txDesc
                                , bank_code//bank_code
                                , bank_name//bank_name
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
                                , ""// bm8
                                , ip// ""// bm9
                                , user_agent// ""// bm10
                                );

                            if (dt != null && dt.Rows[0][TBL_EB_BEN.RET].ToString() == "1")
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "247 TRANSFER IS COMPLETED TRAN_ID=" + eb_tran_id.ToString() + " SAVE TO BENLIST DONE");

                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref); // case sensitive);

                                Funcs.WriteLog("custid:" + custid + "|TRANSFER247   DES_ACCT:" + des_acct
                            + "|BEGIN SAVE TO BEN LIST DONE");
                            }
                            else
                            {
                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE_BEN_FAILED);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "247 TRANSFER IS COMPLETED, SAVE TO BENLIST FAILED TRAN_ID=" + eb_tran_id);


                                Funcs.WriteLog("custid:" + custid + "|TRANSFER247   DES_ACCT:" + des_acct + "|BEGIN SAVE TO BEN LIST FAILED");
                            }
                            //giai phong bo nho                            
                            ben = null;
                            dt = null;
                        }

                    }//end if hach toan vao core
                    #endregion "HACH TOAN VAO CORE"
                    else
                    {
                        //HACH TOAN VAO CORE KHONG THANH CONG
                        retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                        retStr = retStr.Replace(Config.ERR_DESC_VAL, "247 TRANSFER FAILED");

                        Funcs.WriteLog("custid:" + custid + "|TRANSFER247   DES_ACCT:" + des_acct
                        + "|POST TO CORE FAILED");
                    }

                }
                else  // Esle check before trans
                {
                    retStr = check_before_trans;
                }

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                retStr = retStr.Replace(Config.ERR_DESC_VAL, "TRANSFER247 FAILED");
            }

            return retStr;
        }
    }

}