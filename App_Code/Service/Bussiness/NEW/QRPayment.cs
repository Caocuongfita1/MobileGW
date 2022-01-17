using mobileGW.Service.AppFuncs;
using mobileGW.Service.DataAccess;
using mobileGW.Service.Framework;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using VNPAYQrCode;

/// <summary>
/// Summary description for QRPayment
/// </summary>
public class QRPayment
{

    private class QrVnpaycodeItemType
    {
        public string Quanity { get; set; }
        public string QRInfor { get; set; }
        public string Note { get; set; }
    }

    public QRPayment()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string qrDiscount(Hashtable hashTbl, string ip, string userAgent)
    {
        string retStr = Config.RESPONE_GET_QR_DISCOUNT;
        string errCode = Config.ERR_CODE_GENERAL;
        string errDescVi = LanguageConfig.ErrorVoucherVi99;
        string errDescEn = LanguageConfig.ErrorVoucherEn99;
        string value = "0";
        string cusId = (Funcs.getValFromHashtbl(hashTbl, "CIF_NO"));
        string bankCode = Config.SHB_BIN;
        string mobile = (Funcs.getValFromHashtbl(hashTbl, "MOBILE"));
        string payType = (Funcs.getValFromHashtbl(hashTbl, "PAYTYPE"));
        string voucher = (Funcs.getValFromHashtbl(hashTbl, "VOUCHER"));
        if (voucher.Equals("_NULL_"))
        {
            voucher = "";
        }
        string payCode = (Funcs.getValFromHashtbl(hashTbl, "PAYCODE"));
        if (payCode.Equals("_NULL_"))
        {
            payCode = "";
        }
        string itemEncode = (Funcs.getValFromHashtbl(hashTbl, "ITEM")); // du lieu la base64
        string item = "{}";
        try
        {
            item = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(itemEncode));
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + cusId + "|ERROR PARSE AMOUNT|" + ex.ToString());
        }

        double amount = 0;
        try
        {
            amount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("|ERROR PARSE AMOUNT|" + ex.ToString());
        }

        #region config header, param for request
        VNPAYQrCode.AppHdrType appHdr = new VNPAYQrCode.AppHdrType();
        appHdr.CharSet = "UTF-8";
        appHdr.SvcVer = "1.0";

        VNPAYQrCode.PairsType nsFrom = new VNPAYQrCode.PairsType();
        nsFrom.Id = "ESB";
        nsFrom.Name = "ESB";

        VNPAYQrCode.PairsType nsTo = new VNPAYQrCode.PairsType();
        nsTo.Id = "VNPAY_QRCODE";
        nsTo.Name = "VNPAY_QRCODE";

        VNPAYQrCode.PairsType[] listOfNsTo = new VNPAYQrCode.PairsType[1];
        listOfNsTo[0] = nsTo;

        VNPAYQrCode.PairsType BizSvc = new VNPAYQrCode.PairsType();
        BizSvc.Id = "VNPAYQrCode_pmt";
        BizSvc.Name = "VNPAYQrCode_pmt";

        DateTime TransDt = DateTime.Now;

        appHdr.From = nsFrom;
        appHdr.To = listOfNsTo;
        appHdr.MsgId = Funcs.GenESBMsgId();
        appHdr.MsgPreId = "";
        appHdr.BizSvc = BizSvc;
        appHdr.TransDt = TransDt;
        #endregion
        try
        {
            VNPAYQrCode.VNPAYCheckQrCodeReqType msgReq = new VNPAYQrCode.VNPAYCheckQrCodeReqType();
            msgReq.AppHdr = appHdr;
            msgReq.bankCode = Config.SHB_BIN;
            msgReq.payCode = payCode;
            msgReq.payType = payType;
            msgReq.mobile = mobile;
            msgReq.debitAmount = Convert.ToString(amount);
            msgReq.voucherCode = voucher;
            List<QrVnpaycodeItemType> tempObject = (List<QrVnpaycodeItemType>)JsonConvert.DeserializeObject(item, typeof(List<QrVnpaycodeItemType>));
            List<VNPAYQrCode.QrcodeItemType> listObj = new List<VNPAYQrCode.QrcodeItemType>();
            if (tempObject != null)
            {
                foreach (QrVnpaycodeItemType iLoop in tempObject)
                {
                    VNPAYQrCode.QrcodeItemType qrCodeItem = new VNPAYQrCode.QrcodeItemType();
                    qrCodeItem.quantity = iLoop.Quanity;
                    qrCodeItem.qrInfor = iLoop.QRInfor;
                    qrCodeItem.note = iLoop.Note;
                    listObj.Add(qrCodeItem);
                }
            }
            msgReq.item = listObj.ToArray();
            Funcs.WriteLog("custid:" + cusId + "first Call Esb");
            Funcs.WriteLog("custid:" + cusId + "|Request message discountQrpayment:" + new JavaScriptSerializer().Serialize(msgReq));
            VNPAYQrCode.PortTypeClient ptc = new VNPAYQrCode.PortTypeClient();
            VNPAYQrCode.VNPAYCheckQrCodeResType res = ptc.VNPAYCheckQrCode(msgReq);

            Funcs.WriteLog("custid:" + cusId + "|discountQrPayment|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));
            errCode = res.resCode;
            //00 , 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70
            //Cac ma tren duoc tiep tuc thanh toan QR

            Funcs.WriteLog("custid:" + cusId + "FINISH CALL ESB errCode" + errCode);

            errDescVi = res.resDesc;
            errDescEn = res.resDesc;
            value = res.promotionValue;
            /*if (!errCode.Equals(Config.ERR_CODE_DONE) && !errCode.Equals("01"))
            {

                if (errCode.Equals("60") || errCode.Equals("61"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi60_61;
                }
                if (errCode.Equals("62"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi62;
                }
                if (errCode.Equals("63"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi63;
                }
                if (errCode.Equals("64"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi64;
                }
                if (errCode.Equals("65"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi65;
                }
                if (errCode.Equals("66"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi66;
                }
                if (errCode.Equals("67"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi67;
                }
                if (errCode.Equals("68"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi68;
                }
                if (errCode.Equals("69"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi69;
                }
                errCode = Config.ERR_CODE_GENERAL;
            }
            if (errCode.Equals("01"))
            {
                errDescVi = LanguageConfig.ErrorVoucherVi01;
            }*/

            if (errCode.Equals(Config.ERR_CODE_DONE) || errCode.Equals("61") || errCode.Equals("62") || errCode.Equals("63")
                || errCode.Equals("64") || errCode.Equals("65") || errCode.Equals("66") || errCode.Equals("67")
                || errCode.Equals("68") || errCode.Equals("69") || errCode.Equals("70"))
            {
                String retCode = errCode.Equals(Config.ERR_CODE_DONE) ? Config.ERR_CODE_DONE : Config.ERR_CODE_GENERAL;
                if (errCode.Equals("60") || errCode.Equals("61"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi60_61;
                }
                if (errCode.Equals("62"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi62;
                }
                if (errCode.Equals("63"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi63;
                }
                if (errCode.Equals("64"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi64;
                }
                if (errCode.Equals("65"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi65;
                }
                if (errCode.Equals("66"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi66;
                }
                if (errCode.Equals("67"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi67;
                }
                if (errCode.Equals("68"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi68;
                }
                if (errCode.Equals("69"))
                {
                    errDescVi = LanguageConfig.ErrorVoucherVi69;
                }
                errCode = retCode;
            }
            else
            {
                errCode = Config.ERR_CODE_QR_NOT_PAYMENT;//ERR_CODE_QR_NOT_PAYMENT
            }

            if (String.IsNullOrEmpty(value))
            {
                value = "0";
            }
            ptc.Close();
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + cusId + "ERROR Call Esb" + ex.Message);
            errCode = Config.ERR_CODE_GENERAL;
        }
        retStr = retStr.Replace(Config.ERR_CODE_VAL, errCode);
        retStr = retStr.Replace("{ERR_DESC_VI}", errDescVi);
        retStr = retStr.Replace("{ERR_DESC_EN}", errDescEn);
        retStr = retStr.Replace("{VALUE}", value);
        return retStr;
    }

    public string qrPayment(Hashtable hashTbl, string ip, string userAgent)
    {
        string custId = (Funcs.getValFromHashtbl(hashTbl, "CIF_NO"));
        string srcAct = (Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT"));
        string desAcct = string.Empty;
        double amount = 0;
        try
        {
            amount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + custId + "|ERROR PARSE AMOUNT|" + ex.ToString());
            return Config.ERR_MSG_GENERAL;
        }
        double totalAmount = 0;
        try
        {
            totalAmount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "TOTAL_AMOUNT"));
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + custId + "|ERROR PARSE TOTAL_AMOUNT|" + ex.ToString());
            totalAmount = amount;
            //return Config.ERR_MSG_GENERAL;
        }
        string voucherCode = Funcs.getValFromHashtbl(hashTbl, "VOUCHER");
        if (voucherCode.Equals("_NULL_") || voucherCode.Equals(Config.NOT_FOUND))
        {
            voucherCode = String.Empty;
        }
        string mobile = (Funcs.getValFromHashtbl(hashTbl, "MOBILE"));
        string bankCode = Config.SHB_BIN;
        string payDate = (Funcs.getValFromHashtbl(hashTbl, "PAYDATE"));
        string addtionalDataEncode = (Funcs.getValFromHashtbl(hashTbl, "ADDITIONALDATA"));// du lieu la base64
        string messageType = (Funcs.getValFromHashtbl(hashTbl, "MESSAGETYPE"));
        string partnerId = (Funcs.getValFromHashtbl(hashTbl, "PARTNER_ID"));
        string username = (Funcs.getValFromHashtbl(hashTbl, "USERNAME"));
        string itemEncode = (Funcs.getValFromHashtbl(hashTbl, "ITEM")); // du lieu la base64
        string pwd = (Funcs.getValFromHashtbl(hashTbl, "TRANPWD"));
        string terminal = (Funcs.getValFromHashtbl(hashTbl, "TERMINAL"));
        string orderCode = (Funcs.getValFromHashtbl(hashTbl, "ORDER_CODE")); //So bien lai cua QR1 - Type 2
        if (orderCode.Equals("_NULL_"))
        {
            orderCode = null;
        }
        string txDesc = (Funcs.getValFromHashtbl(hashTbl, "TXDESC"));
        string masterMerchant = Funcs.getValFromHashtbl(hashTbl, "MASTER_MERCHANT").Trim();
        bool isDefaultDesc = false;
        if (string.IsNullOrEmpty(txDesc))
        {
            isDefaultDesc = true;
            txDesc = "THANH TOAN QR CODE";
        }
        if (txDesc.Equals(Config.NULL_VALUE))
        {
            isDefaultDesc = true;
            txDesc = "THANH TOAN QR CODE";
        }
        if (txDesc.Equals(Config.NOT_FOUND))
        {
            isDefaultDesc = true;
            txDesc = "THANH TOAN QR CODE";
        }
        txDesc = Funcs.removeStress(txDesc);
        string retStr = Config.QR_PAYMENT;

        //decode data 
        string addtionalData = string.Empty;
        string item = string.Empty;
        try
        {
            addtionalData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(addtionalDataEncode));
            item = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(itemEncode));
        }
        catch (Exception e)
        {
            return Config.ERR_MSG_GENERAL;
        }

        //lay thong tin desAcct cua vnpay
        if (partnerId.ToUpper().Equals(Config.PARTNER_ID_VNPAY))
        {
            desAcct = Config.ACCT_SUSPEND_VNPAY_QR_PAYMENT;
            if (masterMerchant.Equals(Config.SHB_BIN))
            {
                //master merchant is shb
                desAcct = Config.ACCT_SUSPEND_SHB_QR_PAYMENT;
            }
        }
        else
        {
            return Config.ERR_MSG_GENERAL;
        }

        //check tai khoan chuyen co thuoc cif ko?
        bool check = Auth.CustIdMatchScrAcct(custId, srcAct);
        if (!check)
        {
            return Config.ERR_MSG_GENERAL;
        }

        #region FOR TOKEN
        string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
        string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
        int typeOtp = Int16.Parse(typeOtpStr);

        if (typeOtp == 2)
        {
            pwd = Funcs.encryptMD5(pwd + custId);
        }

        #endregion
        string checkBeforeTrans = "";

        #region FOR TOKEN
        switch (typeOtp)
        {
            case 2:
                checkBeforeTrans = Auth.CHECK_BEFORE_TRANSACTION(custId, srcAct, desAcct, amount, pwd, Config.TRAN_TYPE_QR_PAYMENT);
                break;
            case 4:
                checkBeforeTrans = TokenOTPFunc.CheckBeforeTransaction(Config.TRAN_TYPE_QR_PAYMENT, amount, custId, pwd, requestId, typeOtp);
                break;
            case 5:
                checkBeforeTrans = TokenOTPFunc.CheckBeforeTransactionTOKEN(Config.TRAN_TYPE_QR_PAYMENT, amount, custId, pwd, requestId, typeOtp);
                break;
            default:
                break;
        }
        #endregion

        if (checkBeforeTrans.Equals(Config.ERR_CODE_DONE))
        {
            double eb_tran_id = 0;
            string core_txno_ref = string.Empty;
            string core_txdate_ref = string.Empty;

            //hach toan do day
            Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|BEGIN INSERT EB TRAN");

            Transfers tf = new Transfers();
            DataTable eb_tran = new DataTable();

            #region "insert TBL_EB_TRAN"
            eb_tran = tf.insTransferTx(
                Config.ChannelID
                , string.Empty //mod_cd
                , Config.TRAN_TYPE_QR_PAYMENT //tran_type
                , custId //custid
                , srcAct//src_acct
                , desAcct //des_acct
                , amount //amount
                , "VND" //ccy_cd
                , 1//convert rate
                , amount //lcy_amount
                , txDesc
                , string.Empty //pos_cd
                , string.Empty //mkr_id
                , string.Empty //mkr dt
                , string.Empty //apr id 1
                , string.Empty //apr dt 1
                , string.Empty //apr id 2
                , string.Empty //apr dt 2
                , typeOtp //auth_type
                , Config.TX_STATUS_WAIT // status
                , 0 // tran pwd idx
                , string.Empty //sms code
                , string.Empty //sign data
                , string.Empty //core err code
                , string.Empty //core err desc
                , string.Empty //core ref_no
                , string.Empty //core txdate
                , string.Empty //core txtime
                , totalAmount // order_amount
                , amount // order_dis
                , voucherCode //order_id voucherCode
                , partnerId //partner code
                , string.Empty //category code
                , string.Empty //service code
                , masterMerchant //merchant code -> save master merchant
                , string.Empty//suspend account
                , string.Empty//fee account
                , string.Empty//vat account
                , 0 //suppend amount
                , 0 //fee amount
                , 0 //vat amount
                , string.Empty // des name ten tai khoan thu huong
                , string.Empty // bank code
                , string.Empty // ten ngan hang
                , string.Empty // ten thanh pho
                , string.Empty // ten chi nhanh
                , terminal //bm1 
                , orderCode //bm2 
                , string.Empty //bm3
                , string.Empty //bm4
                , string.Empty //bm5
                , string.Empty //bm6
                , string.Empty //bm7
                , string.Empty //bm8
                , string.Empty //bm9
                , string.Empty //bm10
                , string.Empty //bm11
                , string.Empty //bm12
                , string.Empty //bm13
                , string.Empty //bm14
                , string.Empty //bm15
                , string.Empty //bm16
                , string.Empty //bm17
                , string.Empty //bm18
                , string.Empty //bm19
                , string.Empty //bm20
                , string.Empty //bm21
                , string.Empty //bm22
                , string.Empty //bm23
                , string.Empty //bm24
                , string.Empty //bm25
                , string.Empty //bm26
                , requestId //bm27
                , ip//  string.Empty //bm28
                , userAgent// string.Empty//bm29                   
            );
            #endregion "insert TBL_EB_TRAN"

            //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
            if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
            {
                eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                //if (isDefaultDesc) {
                txDesc = txDesc + "- GD" + eb_tran_id + "-" + terminal + (!string.IsNullOrEmpty(orderCode) ? ("-" + orderCode) : string.Empty);
                // }
                Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|BEGIN POST TO CORE" + "|eb_tran_id:" + eb_tran_id);

                string result = CoreIntegration.postFINPOSTToCore
                   (custId
                   , Config.TRAN_TYPE_QR_PAYMENT
                   , eb_tran_id
                   , srcAct
                   , desAcct //gl suspend
                   , string.Empty //gl fee
                   , string.Empty// gl vat
                   , amount // suspend amount
                   , 0 // fee amount
                   , 0 //vat amount
                   , txDesc
                   , Config.HO_BR_CODE
                   , ref core_txno_ref
                   , ref core_txdate_ref
                   );

                //write logs
                Funcs.WriteLog("CustID " + custId + " AdditionalData Json QR CODE: " + addtionalData);
                Funcs.WriteLog("CustID " + custId + " Item Json QR CODE: " + item);
                //Convert Item from msg Mobile to msg Esb
                List<QrVnpaycodeItemType> tempObject = (List<QrVnpaycodeItemType>)JsonConvert.DeserializeObject(item, typeof(List<QrVnpaycodeItemType>));
                List<QrcodeItemType> listObj = new List<QrcodeItemType>();
                if (tempObject != null)
                {
                    foreach (QrVnpaycodeItemType iLoop in tempObject)
                    {
                        QrcodeItemType qrCodeItem = new QrcodeItemType();
                        qrCodeItem.quantity = iLoop.Quanity;
                        qrCodeItem.qrInfor = iLoop.QRInfor;
                        qrCodeItem.note = iLoop.Note;
                        listObj.Add(qrCodeItem);
                    }
                }

                if (result.Equals(Config.gResult_INTELLECT_Arr[0]))
                {
                    Funcs.WriteLog("CustID " + custId + "result from core success: " + result);
                    VNPAYQrCodeResType res = confirmQrPayment(custId, mobile, bankCode, srcAct, payDate,
                        addtionalData, Convert.ToString(amount), Convert.ToString(totalAmount), voucherCode,
                        result.Split('|')[0], result.Split('|')[1], Convert.ToString(eb_tran_id), messageType,
                        Convert.ToString(eb_tran_id), username, listObj.ToArray());

                    //Xu ly tiep du lieu tra ve tu vnpay qr payment
                    if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
                    {
                        string vnpayResCode = res.resCode;
                        Funcs.WriteLog("custid:" + custId + "|vnpayResCode:" + vnpayResCode);
                        if (!string.IsNullOrEmpty(vnpayResCode))
                        {
                            string resPartnerWS = checkResponseCode(vnpayResCode);
                            Funcs.WriteLog("custid:" + custId + "|resPartnerWS:" + resPartnerWS);
                            //XU LY DU LIEU TRA VE
                            //Truong hop timeout
                            if (Config.ERR_CODE_TIMEOUT.Equals(resPartnerWS))
                            {
                                //TIMEOUT CUNG TINH LA STATUS EBANK TRAN = 1. + HAN MUC GIAO DICH
                                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, Config.ChannelID);

                                return Config.ERR_MSG_TIMEOUT;
                            }
                            //truong hop revert
                            else if (Config.ERR_CODE_REVERT.Equals(resPartnerWS))
                            {
                                string revStr = CoreIntegration.revFinPost(eb_tran_id, txDesc);

                                if (Config.gResult_INTELLECT_Arr[0].Equals(revStr))
                                {
                                    Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "REVERT SUCCESSFUL tran_id:" + eb_tran_id.ToString());
                                    revertConfirmQrPayment(custId, Convert.ToString(eb_tran_id), Convert.ToString(eb_tran_id), null, "00", "Revert giao dich thanh cong");
                                }
                                else
                                {
                                    Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "REVERT UNSUCCESSFUL tran_id:" + eb_tran_id.ToString());
                                    revertConfirmQrPayment(custId, Convert.ToString(eb_tran_id), Convert.ToString(eb_tran_id), null, "99", "Revert giao dich khong thanh cong");
                                }
                                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);

                                return Config.ERR_MSG_GENERAL;
                            }
                            //truong hop loi chung
                            else if (Config.ERR_CODE_GENERAL.Equals(resPartnerWS))
                            {
                                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);
                                return Config.ERR_MSG_GENERAL;
                            }
                            //truong hop xu ly thanh cong
                            else if (Config.ERR_CODE_DONE.Equals(resPartnerWS))
                            {
                                // CAP NHAT TRANG THAI VAO BANG TBL_EB_TRAN
                                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, Config.ChannelID);

                                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                retStr = retStr.Replace(Config.ERR_DESC_VAL, "QR PAYMENT IS COMPLETED TRAN_ID = " + eb_tran_id);
                                retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);
                                retStr = retStr.Replace("{PAYMENT_CODE}", Convert.ToString(eb_tran_id));
                            }

                            else
                            {
                                return Config.ERR_MSG_GENERAL;
                            }
                        }
                    }
                    else
                    {
                        //update tbl_eb_tran
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);
                        return Config.ERR_MSG_GENERAL;
                    }

                }
                else
                {
                    //hach toan core ko thanh cong
                    Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "BILL POST TO CORE FAILED tran_id:" + eb_tran_id.ToString());
                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);
                    Funcs.WriteLog("CustID " + custId + "result from core failed: " + result);
                    VNPAYQrCodeResType res = confirmQrPayment(custId, mobile, bankCode, srcAct, payDate,
                        addtionalData, Convert.ToString(amount), Convert.ToString(totalAmount), voucherCode,
                        result.Split('|')[0], result.Split('|')[1], Convert.ToString(eb_tran_id), messageType,
                        Convert.ToString(eb_tran_id), username, listObj.ToArray());
                    return Config.ERR_MSG_GENERAL;
                }
            }
            else
            {
                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");

                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);

                Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|INSERT TBL TRAN FAILED");
            }
        }
        else
        {
            retStr = checkBeforeTrans;
            Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|CHECK BEFORE TRAN FAILED");
        }

        Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|END");

        return retStr;
    }

    public string qrPaymentByCredit(Hashtable hashTbl, string ip, string userAgent)
    {

        string custId = (Funcs.getValFromHashtbl(hashTbl, "CIF_NO"));
        string srcAct = (Funcs.getValFromHashtbl(hashTbl, "SRC_ACCT"));
        string desAcct = string.Empty;
        double amount = 0;
        try
        {
            amount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "AMOUNT"));
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + custId + "|ERROR PARSE AMOUNT|" + ex.ToString());
            return Config.ERR_MSG_GENERAL;
        }
        double totalAmount = 0;
        try
        {
            totalAmount = Double.Parse(Funcs.getValFromHashtbl(hashTbl, "TOTAL_AMOUNT"));
        }
        catch (Exception ex)
        {
            Funcs.WriteLog("custid:" + custId + "|ERROR PARSE TOTAL_AMOUNT|" + ex.ToString());
            totalAmount = amount;
            //return Config.ERR_MSG_GENERAL;
        }

        Funcs.WriteLog("custid:" + custId + "|QR PAYMENT BY CREDIT BEGIN: ");

        string voucherCode = Funcs.getValFromHashtbl(hashTbl, "VOUCHER");
        if (voucherCode.Equals("_NULL_") || voucherCode.Equals(Config.NOT_FOUND))
        {
            voucherCode = String.Empty;
        }

        string mobile = (Funcs.getValFromHashtbl(hashTbl, "MOBILE"));
        string bankCode = Config.SHB_BIN;
        string payDate = (Funcs.getValFromHashtbl(hashTbl, "PAYDATE"));
        string addtionalDataEncode = (Funcs.getValFromHashtbl(hashTbl, "ADDITIONALDATA"));// du lieu la base64
        string messageType = (Funcs.getValFromHashtbl(hashTbl, "MESSAGETYPE"));
        string partnerId = (Funcs.getValFromHashtbl(hashTbl, "PARTNER_ID"));
        string username = (Funcs.getValFromHashtbl(hashTbl, "USERNAME"));
        string itemEncode = (Funcs.getValFromHashtbl(hashTbl, "ITEM")); // du lieu la base64
        string pwd = (Funcs.getValFromHashtbl(hashTbl, "TRANPWD"));
        string terminal = (Funcs.getValFromHashtbl(hashTbl, "TERMINAL"));
        string orderCode = (Funcs.getValFromHashtbl(hashTbl, "ORDER_CODE")); //So bien lai cua QR1 - Type 2
        if (orderCode.Equals("_NULL_"))
        {
            orderCode = null;
        }

        string txDesc = (Funcs.getValFromHashtbl(hashTbl, "TXDESC"));
        string masterMerchant = Funcs.getValFromHashtbl(hashTbl, "MASTER_MERCHANT").Trim();
        bool isDefaultDesc = false;
        if (string.IsNullOrEmpty(txDesc))
        {
            isDefaultDesc = true;
            txDesc = "THANH TOAN QR CODE";
        }
        if (txDesc.Equals(Config.NULL_VALUE))
        {
            isDefaultDesc = true;
            txDesc = "THANH TOAN QR CODE";
        }
        if (txDesc.Equals(Config.NOT_FOUND))
        {
            isDefaultDesc = true;
            txDesc = "THANH TOAN QR CODE";
        }
        txDesc = Funcs.removeStress(txDesc);
        string retStr = Config.QR_PAYMENT;

        //decode data 
        string addtionalData = string.Empty;
        string item = string.Empty;

        string ref_no_to_svfe = "";
        string set_date_to_svfe = "";

        try
        {
            addtionalData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(addtionalDataEncode));
            item = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(itemEncode));
        }
        catch (Exception e)
        {
            return Config.ERR_MSG_GENERAL;
        }

        //lay thong tin desAcct cua vnpay
        if (partnerId.ToUpper().Equals(Config.PARTNER_ID_VNPAY))
        {
            desAcct = Config.ACCT_SUSPEND_VNPAY_QR_PAYMENT;
            if (masterMerchant.Equals(Config.SHB_BIN))
            {
                //master merchant is shb
                desAcct = Config.ACCT_SUSPEND_SHB_QR_PAYMENT;
            }
        }
        else
        {
            return Config.ERR_MSG_GENERAL;
        }

        //check tai khoan chuyen co thuoc cif ko?
        //bool check = Auth.CustIdMatchScrAcct(custId, srcAct);

        //if (!check)
        //{
        //    return Config.ERR_MSG_GENERAL;
        //}

        #region FOR TOKEN
        string requestId = Funcs.getValFromHashtbl(hashTbl, "REQUEST_ID");
        string typeOtpStr = Funcs.getValFromHashtbl(hashTbl, "TYPE_OTP");
        int typeOtp = Int16.Parse(typeOtpStr);

        if (typeOtp == 2)
        {
            pwd = Funcs.encryptMD5(pwd + custId);
        }

        #endregion
        string checkBeforeTrans = "";

        #region FOR TOKEN
        switch (typeOtp)
        {
            case 2:
                checkBeforeTrans = Auth.CHECK_BEFORE_TRANSACTION(custId, srcAct, desAcct, amount, pwd, Config.TRAN_TYPE_QR_PAYMENT);
                break;
            case 4:
                checkBeforeTrans = TokenOTPFunc.CheckBeforeTransaction(Config.TRAN_TYPE_QR_PAYMENT, amount, custId, pwd, requestId, typeOtp);
                break;
            case 5:
                checkBeforeTrans = TokenOTPFunc.CheckBeforeTransactionTOKEN(Config.TRAN_TYPE_QR_PAYMENT, amount, custId, pwd, requestId, typeOtp);
                break;
            default:
                break;
        }
        #endregion

        if (checkBeforeTrans.Equals(Config.ERR_CODE_DONE))
        {
            double eb_tran_id = 0;
            string core_txno_ref = string.Empty;
            string core_txdate_ref = string.Empty;

            //hach toan do day
            Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|BEGIN INSERT EB TRAN");

            Transfers tf = new Transfers();
            DataTable eb_tran = new DataTable();

            #region "insert TBL_EB_TRAN"
            eb_tran = tf.insTransferTx(
                Config.ChannelID
                , string.Empty //mod_cd
                , Config.TRAN_TYPE_QR_PAYMENT //tran_type
                , custId //custid
                , srcAct//src_acct
                , desAcct //des_acct
                , amount //amount
                , "VND" //ccy_cd
                , 1//convert rate
                , amount //lcy_amount
                , txDesc
                , string.Empty //pos_cd
                , string.Empty //mkr_id
                , string.Empty //mkr dt
                , string.Empty //apr id 1
                , string.Empty //apr dt 1
                , string.Empty //apr id 2
                , string.Empty //apr dt 2
                , typeOtp //auth_type
                , Config.TX_STATUS_WAIT // status
                , 0 // tran pwd idx
                , string.Empty //sms code
                , string.Empty //sign data
                , string.Empty //core err code
                , string.Empty //core err desc
                , string.Empty //core ref_no
                , string.Empty //core txdate
                , string.Empty //core txtime
                , totalAmount // order_amount
                , amount // order_dis
                , voucherCode //order_id voucherCode
                , partnerId //partner code
                , string.Empty //category code
                , string.Empty //service code
                , masterMerchant //merchant code -> save master merchant
                , string.Empty//suspend account
                , string.Empty//fee account
                , string.Empty//vat account
                , 0 //suppend amount
                , 0 //fee amount
                , 0 //vat amount
                , string.Empty // des name ten tai khoan thu huong
                , string.Empty // bank code
                , string.Empty // ten ngan hang
                , string.Empty // ten thanh pho
                , string.Empty // ten chi nhanh
                , terminal //bm1 
                , orderCode //bm2 
                , string.Empty //bm3
                , string.Empty //bm4
                , string.Empty //bm5
                , string.Empty //bm6
                , string.Empty //bm7
                , string.Empty //bm8
                , string.Empty //bm9
                , string.Empty //bm10
                , string.Empty //bm11
                , string.Empty //bm12
                , string.Empty //bm13
                , string.Empty //bm14
                , string.Empty //bm15
                , string.Empty //bm16
                , string.Empty //bm17
                , string.Empty //bm18
                , string.Empty //bm19
                , string.Empty //bm20
                , string.Empty //bm21
                , string.Empty //bm22
                , string.Empty //bm23
                , string.Empty //bm24
                , string.Empty //bm25
                , string.Empty //bm26
                , requestId //bm27
                , ip//  string.Empty //bm28
                , userAgent// string.Empty//bm29                   
            );
            #endregion "insert TBL_EB_TRAN"

            //INSERT THANH CONG DU LIEU VAO EBANK TRANSACTION --> TRANG THAI CHO THANH TOAN
            if ((eb_tran != null) && (eb_tran.Rows.Count > 0))
            {
                eb_tran_id = double.Parse(eb_tran.Rows[0][TBL_EB_TRAN.TRAN_ID].ToString());
                //if (isDefaultDesc) {
                txDesc = txDesc + "- GD" + eb_tran_id + "-" + terminal + (!string.IsNullOrEmpty(orderCode) ? ("-" + orderCode) : string.Empty);
                // }

                var ref_to_card_api = Config.refFormat + eb_tran_id.ToString().PadLeft(9, '0');

                Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|BEGIN CREDIT PURCHASE" + "|eb_tran_id:" + eb_tran_id);

                string tmpAmt = amount.ToString() + "00";
                tmpAmt = tmpAmt.ToString().PadLeft(12, '0');

                CardAPI.PurchaseResType resCredit = CardIntegration.purcharseCardAPI(
                                                custId,
                                                srcAct,
                                                ref_to_card_api,
                                                tmpAmt,
                                                Funcs.getConfigVal("QR_PAYMENT_MCC"),
                                                Funcs.getConfigVal("QR_PAYMENT_TID"),
                                                Config.CardAPIConfig.ServiceCd,
                                                Funcs.getConfigVal("QR_PAYMENT_MID")
                                                );

                if (resCredit != null && resCredit.RespSts.Sts.Equals("0"))
                {
                    ref_no_to_svfe = resCredit.RefNo;
                    set_date_to_svfe = resCredit.SettDate;

                    string src_acc_credit_gl = Funcs.GetAccGLCredit(srcAct);

                    if (string.IsNullOrEmpty(src_acc_credit_gl))
                    {
                        return Config.ERR_MSG_GENERAL;
                    }

                    Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|BEGIN POST TO CORE" + "|eb_tran_id:" + eb_tran_id);

                    string result = CoreIntegration.postFINPOSTToCore
                       (custId
                       , Config.TRAN_TYPE_QR_PAYMENT
                       , eb_tran_id
                       , src_acc_credit_gl
                       , desAcct //gl suspend
                       , string.Empty //gl fee
                       , string.Empty// gl vat
                       , amount // suspend amount
                       , 0 // fee amount
                       , 0 //vat amount
                       , txDesc
                       , Config.HO_BR_CODE
                       , ref core_txno_ref
                       , ref core_txdate_ref
                       );

                    //write logs
                    Funcs.WriteLog("CustID " + custId + " AdditionalData Json QR CODE: " + addtionalData);
                    Funcs.WriteLog("CustID " + custId + " Item Json QR CODE: " + item);
                    //Convert Item from msg Mobile to msg Esb
                    List<QrVnpaycodeItemType> tempObject = (List<QrVnpaycodeItemType>)JsonConvert.DeserializeObject(item, typeof(List<QrVnpaycodeItemType>));
                    List<QrcodeItemType> listObj = new List<QrcodeItemType>();
                    if (tempObject != null)
                    {
                        foreach (QrVnpaycodeItemType iLoop in tempObject)
                        {
                            QrcodeItemType qrCodeItem = new QrcodeItemType();
                            qrCodeItem.quantity = iLoop.Quanity;
                            qrCodeItem.qrInfor = iLoop.QRInfor;
                            qrCodeItem.note = iLoop.Note;
                            listObj.Add(qrCodeItem);
                        }
                    }

                    if (result.Equals(Config.gResult_INTELLECT_Arr[0]))
                    {
                        Funcs.WriteLog("CustID " + custId + "result from core success: " + result);
                        
                        string accoutNo = srcAct.Substring(0, 6) + "XXXX" + srcAct.Substring(srcAct.ToString().Length - 4, 4);

                        VNPAYQrCodeResType res = confirmQrPayment(custId, mobile, bankCode, accoutNo, payDate,
                            addtionalData, Convert.ToString(amount), Convert.ToString(totalAmount), voucherCode,
                            result.Split('|')[0], result.Split('|')[1], Convert.ToString(eb_tran_id), messageType,
                            Convert.ToString(eb_tran_id), username, listObj.ToArray());

                        //Xu ly tiep du lieu tra ve tu vnpay qr payment
                        if (res != null && res.RespSts != null && res.RespSts.Sts != null && res.RespSts.Sts.Equals("0"))
                        {
                            string vnpayResCode = res.resCode;
                            Funcs.WriteLog("custid:" + custId + "|vnpayResCode:" + vnpayResCode);
                            if (!string.IsNullOrEmpty(vnpayResCode))
                            {
                                string resPartnerWS = checkResponseCode(vnpayResCode);
                                Funcs.WriteLog("custid:" + custId + "|resPartnerWS:" + resPartnerWS);
                                //XU LY DU LIEU TRA VE
                                //Truong hop timeout
                                if (Config.ERR_CODE_TIMEOUT.Equals(resPartnerWS))
                                {
                                    //TIMEOUT CUNG TINH LA STATUS EBANK TRAN = 1. + HAN MUC GIAO DICH
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, Config.ChannelID);

                                    return Config.ERR_MSG_TIMEOUT;
                                }
                                //truong hop revert
                                else if (Config.ERR_CODE_REVERT.Equals(resPartnerWS))
                                {
                                    //string revStr = CoreIntegration.revFinPost(eb_tran_id, txDesc);
                                    
                                    //if (Config.gResult_INTELLECT_Arr[0].Equals(revStr))
                                    //{
                                    //    Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "REVERT SUCCESSFUL tran_id:" + eb_tran_id.ToString());

                                    //    revertConfirmQrPayment(custId, Convert.ToString(eb_tran_id), Convert.ToString(eb_tran_id), null, "00", "Revert giao dich thanh cong");

                                    //    Funcs.WriteLog("custid:" + custId + "QR PAYMENT revert successfull tran_id:" + eb_tran_id);

                                    //    CardAPI.ReversalResType revertRes = CardIntegration.revertCardAPI(
                                    //                                custId,
                                    //                                srcAct,
                                    //                                ref_to_card_api,
                                    //                                tmpAmt,
                                    //                                ref_no_to_svfe,
                                    //                                set_date_to_svfe,
                                    //                                Config.CardAPIConfig.MerchantType,
                                    //                                Config.CardAPIConfig.TermId,
                                    //                                Config.CardAPIConfig.ServiceCd
                                    //                            );

                                    //    if (revertRes != null && revertRes.RespSts.Sts.Equals("0"))
                                    //    {
                                    //        Funcs.WriteLog("custid:" + custId + "QR PAYMENT revert to SVFE successfull ref_no_to_svfe:" + ref_no_to_svfe);
                                    //    }
                                    //    else
                                    //    {
                                    //        Funcs.WriteLog("custid:" + custId + "QR PAYMENT revert to SVFE not successfull ref_no_to_svfe:" + ref_no_to_svfe);
                                    //    }

                                    //}
                                    //else
                                    //{
                                    //    Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "REVERT UNSUCCESSFUL tran_id:" + eb_tran_id.ToString());
                                    //    revertConfirmQrPayment(custId, Convert.ToString(eb_tran_id), Convert.ToString(eb_tran_id), null, "99", "Revert giao dich khong thanh cong");
                                    //}
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);

                                    return Config.ERR_MSG_GENERAL;
                                }
                                //truong hop loi chung
                                else if (Config.ERR_CODE_GENERAL.Equals(resPartnerWS))
                                {
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);
                                    return Config.ERR_MSG_GENERAL;
                                }
                                //truong hop xu ly thanh cong
                                else if (Config.ERR_CODE_DONE.Equals(resPartnerWS))
                                {
                                    // CAP NHAT TRANG THAI VAO BANG TBL_EB_TRAN
                                    tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_DONE, core_txno_ref, core_txdate_ref, Config.ChannelID);

                                    retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_DONE);
                                    retStr = retStr.Replace(Config.ERR_DESC_VAL, "QR PAYMENT IS COMPLETED TRAN_ID = " + eb_tran_id);
                                    retStr = retStr.Replace("{TRANID}", core_txno_ref);
                                    retStr = retStr.Replace("{TRAN_DATE}", core_txdate_ref);
                                    retStr = retStr.Replace("{PAYMENT_CODE}", Convert.ToString(eb_tran_id));
                                }

                                else
                                {
                                    return Config.ERR_MSG_GENERAL;
                                }
                            }
                        }
                        else
                        {
                            //update tbl_eb_tran
                            tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);
                            return Config.ERR_MSG_GENERAL;
                        }

                    }
                    else
                    {
                        //hach toan core ko thanh cong
                        Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "BILL POST TO CORE FAILED tran_id:" + eb_tran_id.ToString());
                        tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);
                        Funcs.WriteLog("CustID " + custId + "result from core failed: " + result);

                        VNPAYQrCodeResType res = confirmQrPayment(custId, mobile, bankCode, srcAct, payDate,
                            addtionalData, Convert.ToString(amount), Convert.ToString(totalAmount), voucherCode,
                            result.Split('|')[0], result.Split('|')[1], Convert.ToString(eb_tran_id), messageType,
                            Convert.ToString(eb_tran_id), username, listObj.ToArray());

                        return Config.ERR_MSG_GENERAL;
                    }
                }
                else
                {
                    retStr = resCredit.RespSts.ErrInfo[0].ErrCd.ToString();
                    return retStr;
                }
            }
            else
            {
                retStr = retStr.Replace(Config.ERR_CODE_VAL, Config.CD_EB_TRANS_ERR_GENERAL);
                retStr = retStr.Replace(Config.ERR_DESC_VAL, "CANT INSERT TO TBL_EB_TRANS");

                tf.uptTransferTx(eb_tran_id, Config.TX_STATUS_FAIL, core_txno_ref, core_txdate_ref, Config.ChannelID);

                Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|INSERT TBL TRAN FAILED");
            }
        }
        else
        {
            retStr = checkBeforeTrans;
            Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|CHECK BEFORE TRAN FAILED");
        }

        Funcs.WriteLog("custid:" + custId + "|QR PAYMENT PARTNER ID:" + partnerId + "|MESSAGE TYPE:" + messageType + "|END");

        return retStr;
    }

    public string checkResponseCode(string responseCode)
    {
        string resReturn = Config.ERR_CODE_GENERAL;
        if (Config.RET_CODE_QR_VNPAY[0].Split('|')[0].Equals(responseCode))
        {
            //done
            resReturn = Config.ERR_CODE_DONE;
        }
        else if (Config.RET_CODE_QR_VNPAY[1].Split('|')[0].Equals(responseCode))
        {
            //timeout
            resReturn = Config.ERR_CODE_TIMEOUT;
        }
        else if (Config.RET_CODE_QR_VNPAY[2].Split('|')[0].Equals(responseCode))
        {
            //timeout
            resReturn = Config.ERR_CODE_TIMEOUT;
        }
        else
        {
            for (int i = 4; i < Config.RET_CODE_QR_VNPAY.Length; i++)
            {
                if (Config.RET_CODE_QR_VNPAY[i].Split('|')[0].Equals(responseCode))
                {
                    resReturn = Config.ERR_CODE_REVERT;
                    return resReturn;
                }
            }
        }
        return resReturn;
    }

    public VNPAYRevertPaymentResType revertConfirmQrPayment(string cif, string refundTrace, string bankTrace, string payCode, string code, string message)
    {
        VNPAYRevertPaymentResType res = null;
        try
        {
            #region header
            VNPAYQrCode.AppHdrType appHdr = new VNPAYQrCode.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            VNPAYQrCode.PairsType nsFrom = new VNPAYQrCode.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            VNPAYQrCode.PairsType nsTo = new VNPAYQrCode.PairsType();
            nsTo.Id = "VNPAY_QRCODE";
            nsTo.Name = "VNPAY_QRCODE";

            VNPAYQrCode.PairsType[] listOfNsTo = new VNPAYQrCode.PairsType[1];
            listOfNsTo[0] = nsTo;

            VNPAYQrCode.PairsType BizSvc = new VNPAYQrCode.PairsType();
            BizSvc.Id = "VNPAYQrCode_pmt";
            BizSvc.Name = "VNPAYQrCode_pmt";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            VNPAYQrCode.VNPAYRevertPaymentReqType msgReq = new VNPAYRevertPaymentReqType();
            msgReq.AppHdr = appHdr;
            msgReq.bankCode = Config.SHB_BIN;
            msgReq.code = code;
            msgReq.message = message;
            msgReq.payCode = "_NULL_";
            msgReq.bankTrace = bankTrace;
            msgReq.refundTrace = bankTrace;
            msgReq.paydate = DateTime.Now.ToString("ddMMyyyy");

            Funcs.WriteLog("custid:" + cif + "|Request revert qrpayment:" + new JavaScriptSerializer().Serialize(msgReq));


            VNPAYQrCode.PortTypeClient ptc = new VNPAYQrCode.PortTypeClient();
            res = ptc.VNPAYRevertPayment(msgReq);

            Funcs.WriteLog("custid:" + cif + "|VNPAYRevertPayment|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("VNPAYRevertPayment EXCEPTION FROM ESB: " + ex.ToString());
        }
        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="custId"></param>
    /// <param name="mobile"></param>
    /// <param name="bankCode"></param>
    /// <param name="accountNo"></param>
    /// <param name="payDate"></param>
    /// <param name="addtionalData"></param>
    /// <param name="realAmount"></param>
    /// <param name="respCode"></param>
    /// <param name="respDesc"></param>
    /// <param name="traceBankId"></param>
    /// <param name="messageType"></param>
    /// <param name="orderCode"></param>
    /// <param name="userName"></param>
    /// <param name="listQrCodeObj"></param>
    /// <returns></returns>
    public VNPAYQrCodeResType confirmQrPayment(string custId, string mobile, string bankCode, string accountNo,
        string payDate, string addtionalData, string realAmount, string totalAmount, string voucherCode, string respCode, string respDesc, string traceBankId, string messageType,
        string orderCode, string userName, QrcodeItemType[] listQrCodeObj)
    {
        VNPAYQrCodeResType res = null;

        try
        {
            #region header
            VNPAYQrCode.AppHdrType appHdr = new VNPAYQrCode.AppHdrType();
            appHdr.CharSet = "UTF-8";
            appHdr.SvcVer = "1.0";

            VNPAYQrCode.PairsType nsFrom = new VNPAYQrCode.PairsType();
            nsFrom.Id = "ESB";
            nsFrom.Name = "ESB";

            VNPAYQrCode.PairsType nsTo = new VNPAYQrCode.PairsType();
            nsTo.Id = "VNPAY_QRCODE";
            nsTo.Name = "VNPAY_QRCODE";

            VNPAYQrCode.PairsType[] listOfNsTo = new VNPAYQrCode.PairsType[1];
            listOfNsTo[0] = nsTo;

            VNPAYQrCode.PairsType BizSvc = new VNPAYQrCode.PairsType();
            BizSvc.Id = "VNPAYQrCode_pmt";
            BizSvc.Name = "VNPAYQrCode_pmt";

            DateTime TransDt = DateTime.Now;

            appHdr.From = nsFrom;
            appHdr.To = listOfNsTo;
            appHdr.MsgId = Funcs.GenESBMsgId();
            appHdr.MsgPreId = "";
            appHdr.BizSvc = BizSvc;
            appHdr.TransDt = TransDt;
            #endregion

            VNPAYQrCodeReqType msgReq = new VNPAYQrCodeReqType();
            msgReq.AppHdr = appHdr;

            msgReq.mobile = mobile;
            msgReq.bankCode = bankCode;
            msgReq.accountNo = accountNo;
            msgReq.payDate = DateTime.Now.ToString("yyyyMMddHHmmss");//payDate;
            msgReq.addtionalData = addtionalData;
            msgReq.debitAmount = totalAmount;
            msgReq.realAmount = realAmount;
            msgReq.promotionCode = voucherCode;
            msgReq.respCode = respCode;
            msgReq.respDesc = respDesc;
            msgReq.traceTransfer = traceBankId;
            msgReq.messageType = messageType;
            msgReq.orderCode = orderCode;
            msgReq.userName = userName;
            msgReq.item = listQrCodeObj;
            msgReq.checkSum = string.Empty;

            Funcs.WriteLog("custid:" + custId + "|Request message confirmQrpayment:" + new JavaScriptSerializer().Serialize(msgReq));

            VNPAYQrCode.PortTypeClient ptc = new VNPAYQrCode.PortTypeClient();
            res = ptc.Confirm(msgReq);

            Funcs.WriteLog("custid:" + custId + "|confirmQrPayment|" + Funcs.getMaskingStr(new JavaScriptSerializer().Serialize(res)));

            ptc.Close();
        }
        catch (Exception ex)
        {
            //write log
            Funcs.WriteLog("confirmQrPayment EXCEPTION FROM ESB: " + ex.ToString());
        }
        return res;
    }
}