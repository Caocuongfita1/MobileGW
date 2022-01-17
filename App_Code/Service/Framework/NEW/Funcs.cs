using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Xml;
using System.Net;
using System.Configuration;
using System.Collections;
using System.Timers;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using System.Web.Script.Serialization;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using log4net;
using log4net.Appender;
using System.Globalization;

namespace mobileGW.Service.Framework
{
    /// <summary>
    /// Summary description for Funcs.
    /// </summary>
    public class Funcs
    {
        private static log4net.ILog l4NC = log4net.LogManager.GetLogger(typeof(Funcs));
        public Funcs()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region "For ESB Integration"

        public static string MD5HashEncoding(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] < 16)
                    sb.Append("0" + hash[i].ToString("x"));
                //ret += "0" + a.ToString ("x");
                else
                    sb.Append(hash[i].ToString("x"));
            }
            string str = sb.ToString().ToUpper();
            return str;
        }

        public static string GenESBMsgId()
        {
            var randomNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + DateTime.Now.Ticks;
            randomNumber = System.Text.RegularExpressions.Regex.Replace(randomNumber, "[^0-9a-zA-Z]+", "");
            return randomNumber;
        }


        public static string GetLast(string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }

        public static string GET_TRAN_ID()
        {
            string temp = "000000";
            DataTable dt = new DataTable();
            try
            {

                OracleCommand cmd = new OracleCommand(Config.gEBANKSchema + "pkg_payment_new.GET_TRAN_ID", new OracleConnection(Config.gEBANKConnstr));

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("V_TYPE", OracleDbType.Varchar2, "*", ParameterDirection.Input);
                cmd.Parameters.Add("V_CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleDataAdapter apt = new OracleDataAdapter();
                apt.SelectCommand = cmd;
                apt.Fill(dt);

                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    temp = dt.Rows[0]["TRAN_ID"].ToString();
                    return temp;
                }
                else
                {
                    return temp;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return temp;
            }

        }


        #endregion "For ESB Integration"

        #region "Security"
        //myGW.Url = System.Configuration.ConfigurationSettings.AppSettings.Get("NganluongGW.Nganluong.WS_WITH_SMS");

        public static string getConfigVal(string key)
        {
            try
            {
                string val = string.Empty;
                val = ConfigurationSettings.AppSettings.Get(key);
                return val;
            }
            catch
            {
                return "";
            }


        }
        public static string getConn(string dbName) //ex: EBANKDB
        {
            try
            {
                string connStr = string.Empty;
                connStr = ConfigurationSettings.AppSettings.Get(dbName + "_FULL");
                string pass = string.Empty;
                pass = ConfigurationSettings.AppSettings.Get(dbName + "_PASS");
                string key = string.Empty;
                key = ConfigurationSettings.AppSettings.Get("SHARED_KEY");
                string realPass = string.Empty;
                realPass = eDecrypts(pass, key);
                return connStr.Replace("[PASSWORD]", realPass);
                //return connStr;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="PlainText">Text to be encrypted</param>
        /// <param name="Password">Password to encrypt with</param>
        /// <param name="Salt">Salt to encrypt with</param>
        /// <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="PasswordIterations">Number of iterations to do</param>
        /// <param name="InitialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="KeySize">Can be 128, 192, or 256</param>
        /// <returns>An encrypted string</returns>

        public static string eEncrypt(string PlainText, string Password)
        {
            try
            {

                string Salt = "Kosher";
                string HashAlgorithm = "SHA1";
                int PasswordIterations = 2;
                string InitialVector = "OFRna73m*aze01xY";
                int KeySize = 256;

                if (PlainText == null) return "";

                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);

                byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);

                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);

                RijndaelManaged SymmetricKey = new RijndaelManaged();

                SymmetricKey.Mode = CipherMode.CBC;
                byte[] CipherTextBytes = null;

                using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
                {

                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                        {
                            CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);

                            CryptoStream.FlushFinalBlock();
                            CipherTextBytes = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                SymmetricKey.Clear();

                return Convert.ToBase64String(CipherTextBytes);
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("Could not encrypt password" + ex.ToString());
                return "";
            }

        }

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="CipherText">Text to be decrypted</param>
        /// <param name="Password">Password to decrypt with</param>
        /// <param name="Salt">Salt to decrypt with</param>
        /// <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="PasswordIterations">Number of iterations to do</param>
        /// <param name="InitialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="KeySize">Can be 128, 192, or 256</param>
        /// <returns>A decrypted string</returns>

        public static string eDecrypts(string CipherText, string Password)
        {
            try
            {
                //return CipherText;
                string Salt = "Kosher";
                string HashAlgorithm = "SHA1";
                int PasswordIterations = 2;
                string InitialVector = "OFRna73m*aze01xY";
                int KeySize = 256;

                if (CipherText == null) return "";

                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
                byte[] CipherTextBytes = Convert.FromBase64String(CipherText);
                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
                RijndaelManaged SymmetricKey = new RijndaelManaged();
                SymmetricKey.Mode = CipherMode.CBC;
                byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
                int ByteCount = 0;
                using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
                {
                    using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                        {
                            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }

                SymmetricKey.Clear();
                return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("Invalid password");
                return "";
            }

        }


        public static string encrypt_function(string Plain_Text, string inpKey)
        {
            // Gen Key
            byte[] Key = System.Text.ASCIIEncoding.ASCII.GetBytes(inpKey);
            // Generate IV
            byte[] IV = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                IV[i] = 0;
            }
            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            //I crypto transform is used to perform the actual decryption vs encryption, hash function are also a version of crypto transform.
            ICryptoTransform Encryptor = null;
            //Crypto streams allow for encryption in memory.
            CryptoStream Crypto_Stream = null;

            System.Text.UTF8Encoding Byte_Transform = new System.Text.UTF8Encoding();

            //Just grabbing the bytes since most crypto functions need bytes.
            //byte[] PlainBytes = Byte_Transform.GetBytes( Convert.FromBase64String (Plain_Text));

            byte[] PlainBytes = Byte_Transform.GetBytes(Plain_Text);

            try
            {
                Crypto = new RijndaelManaged();
                Crypto.Key = Key;
                Crypto.IV = IV;

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call
                //If these are changed nothing will work right.
                Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)
                Crypto_Stream.Write(PlainBytes, 0, PlainBytes.Length);

            }
            finally
            {
                //if the crypto blocks are not clear lets make sure the data is gone
                if (Crypto != null)
                    Crypto.Clear();
                //Close because of my need to close things when done.
                Crypto_Stream.Close();
            }
            //Return the memory byte array
            return Convert.ToBase64String(MemStream.ToArray());
        }


        public static string decrypt_function(string encodedBase64Str, string inpKey)
        {
            // Decode Base 64 to Byte array
            //Convert.FromBase64String (Plain_Text)
            byte[] Cipher_Text = Convert.FromBase64String(encodedBase64Str);

            // Gen Key
            byte[] Key = System.Text.ASCIIEncoding.ASCII.GetBytes(inpKey);
            // Generate IV
            byte[] IV = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                IV[i] = 0;
            }

            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            ICryptoTransform Decryptor = null;
            CryptoStream Crypto_Stream = null;
            StreamReader Stream_Read = null;
            string Plain_Text;

            try
            {
                Crypto = new RijndaelManaged();
                //Crypto.Padding= PaddingMode.PKCS7;
                //Crypto.Padding= PaddingMode.Zeros;
                //Crypto.Mode= CipherMode.CBC;
                //Crypto.Mode= CipherMode.ECB;
                Crypto.Key = Key;
                //Crypto.KeySize = 256;
                //Crypto.BlockSize= 128;
                Crypto.IV = IV;

                MemStream = new MemoryStream(Cipher_Text);

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.
                Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.
                Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.
                Stream_Read = new StreamReader(Crypto_Stream);
                Plain_Text = Stream_Read.ReadToEnd();
            }
            finally
            {
                if (Crypto != null)
                    Crypto.Clear();

                MemStream.Flush();
                MemStream.Close();
            }
            return Plain_Text;
        }

        public static string Encrypt2(string msg)
        {
            string ret = "";
            for (int i = 0; i < msg.Length; i++)
            {
                //170: mat na dung khi ma hoa
                ret += (char)((int)msg[i] ^ 170);
            }
            return ret;
        }


        #endregion "Security"


        #region "Others"
        public static string getValueByName(string message, string name)
        {
            Hashtable hashTbl = new Hashtable();
            string value = "";
            string[] arr = message.Split(Config.ROW_DLMT.ToCharArray());
            for (int i = 0; i < arr.Length; i++)
            {
                hashTbl.Add(arr[i].Split(Config.COL_DLMT.ToCharArray())[0], arr[i].Split(Config.COL_DLMT.ToCharArray())[1]);
            }
            try
            {
                value = (string)hashTbl[name];
            }
            catch
            {
                value = Config.NOT_FOUND;
            }
            return value;//.Trim();
        }

        public static string getValFromHashtbl(Hashtable hashTbl, string name)
        {
            string value = "";
            try
            {
                value = ((string)hashTbl[name]).Trim();
            }
            catch
            {
                value = Config.NOT_FOUND;
            }
            return value;//.Trim();
        }

        /// <summary>
        /// HUNGTD - 03/12/2014
        /// Hàm masking CARD NO
        /// </summary>
        /// <param name="data">cardno - số thẻ cần masking</param>
        /// <returns>Số thể đã masking</returns>
        public static string ShowMarkingCardNo(string cardno)
        {

            if (cardno.Length == 16)
            {
                string card_no_prefix = cardno.Substring(0, 4);
                string card_no_suffix = cardno.Substring(12, 4);
                return card_no_prefix + "XXXXXX" + card_no_suffix;
            }
            else if (cardno.Length == 42)
            {
                string card_no_prefix = cardno.Substring(0, 6);
                string card_no_suffix = cardno.Substring(38, 4);
                return card_no_prefix + "XXXXXX" + card_no_suffix;
            }
            return cardno;
        }

        public static Hashtable stringToHashtbl(string message)
        {

            Hashtable hashTbl = new Hashtable();
            try
            {
                string[] arr = message.Split(Config.ROW_DLMT.ToCharArray());
                for (int i = 0; i < arr.Length; i++)
                {
                    hashTbl.Add(arr[i].Split(Config.COL_DLMT.ToCharArray())[0], arr[i].Split(Config.COL_DLMT.ToCharArray())[1]);
                }
                return hashTbl;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("String to hash error " + ex.ToString());
                return null;
            }

        }

        public static string sendMsg(string reqMessage, string reqURL)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(reqURL);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = reqMessage;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //           Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        //anhnd2 them funcs mask card_no :
        //19.12.2014 PCI DSS
        public static string MaskCardNo(string Card_No)
        {
            /*
            string Card_No_Masked;
            StringBuilder aStringBuilder = new StringBuilder(Card_No);
            aStringBuilder.Remove(6, 6);
            aStringBuilder.Insert(6, "XXXXXX");
            Card_No_Masked = aStringBuilder.ToString();
            return Card_No_Masked;
            */

            if (Card_No.Length == 16)
            {
                string Card_No_Masked;
                StringBuilder aStringBuilder = new StringBuilder(Card_No);
                aStringBuilder.Remove(6, 6);
                aStringBuilder.Insert(6, "XXXXXX");
                aStringBuilder.Insert(4, " ");
                aStringBuilder.Insert(9, " ");
                aStringBuilder.Insert(14, " ");
                Card_No_Masked = aStringBuilder.ToString();
                return Card_No_Masked;

            }
            else if (Card_No.Length == 42)
            {
                string Card_No_Masked;
                StringBuilder aStringBuilder = new StringBuilder(Card_No);
                aStringBuilder.Remove(6, 32);
                aStringBuilder.Insert(6, "XXXXXX");
                aStringBuilder.Insert(4, " ");
                aStringBuilder.Insert(9, " ");
                aStringBuilder.Insert(14, " ");
                Card_No_Masked = aStringBuilder.ToString();
                return Card_No_Masked;
            }
            return String.Empty;
        }


        public static string MaskingString(string inputStr)
        {
            var aStringBuilder = new System.Text.StringBuilder(inputStr);
            try
            {

                //try to masking token: show first 16 characters
                int start = inputStr.IndexOf("TOKEN#");
                if (start > 0)
                {
                    aStringBuilder.Remove(start + 6, 16);
                }

                //try to masking tranpwd: show first 16 characters
                int start1 = inputStr.IndexOf("PWD#");
                if (start1 > 0)
                {
                    aStringBuilder.Remove(start1 + 4, 16);
                }
                //Funcs.WriteLog("response string masking:" + aStringBuilder.ToString());
                return aStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Funcs.WriteFileLog("INPUT: " + inputStr + "|ERROR MASKING: " + ex.ToString());
                Funcs.WriteLog("INPUT: " + inputStr + "|ERROR MASKING: " + ex.ToString());
                return "*** error masking string ***";
            }

        }

        public static string getMaskingStr(string msg)
        {
            try
            {
                //HUNGTD - 27/05/2015 - Remove neu la 16 so
                String pattern = "[0-9]{16}";

                foreach (Match match in Regex.Matches(msg, pattern, RegexOptions.IgnoreCase))
                {
                    string sCardNo = match.Value;
                    string sMaskingCard = ShowMarkingCardNo(sCardNo);

                    msg = msg.Replace(sCardNo, sMaskingCard);
                }
            }
            catch (Exception e)
            {

            }
            return msg;
        }

        /// <summary>
        /// anhnd2 - 14/01/2014 PCI-DSS   
        /// Hàm kiếm tra thông tin tài khoản/số thẻ khi chuyển khoản liên ngân hàng qua thẻ.
        /// Chặn nếu:
        ///  - 4 VISA, MasterCard, 35 JCB, 36 Diner, 37 Amex
        ///  - Số thẻ trong khoảng 16 đến 19 
        /// </summary>
        /// <returns>boolean</returns>
        /// <param name="str">str - số thẻ/tài khoản</param>
        /// Code credit to HungTd
        public static bool ValidCard_Acc_No_PCIDSS(string str)
        {

            if (String.IsNullOrEmpty(str))
                return false;
            //anhnd2 26.06.2015
            // Sua lai ham loc so the.

            bool result = true;

            result = !CheckIsGlobalBIN(str);

            return result;


        }

        //anhnd2 
        //PCIDSS 25.03.2015
        // tao Funcs masking PWD ghi log password.
        public static string MsgPassMask(String msg)
        {
            //"PASS#"  Login check
            try
            {
                string pwdheader = "PASS#";
                if (msg.Contains(pwdheader))
                {
                    string[] Arr = msg.Split(new string[] { "PASS#" }, StringSplitOptions.None);
                    string pwd = Arr[1].Substring(0, 32);
                    string pwdmask = "PASS#" + pwd.Substring(0, 4) + "[PASSWORD]" + pwd.Substring(28, 4);
                    Arr[1] = Arr[1].Replace(pwd, pwdmask);
                    msg = string.Join("", Arr);

                }

                //CUR_PWD#  Change PWD

                if (msg.Contains("CUR_PWD#"))
                {
                    string[] Arr = msg.Split(new string[] { "CUR_PWD#" }, StringSplitOptions.None);
                    string pwd = Arr[1].Substring(0, 73);
                    string pwdmask = "CUR_PWD#" + pwd.Substring(0, 4) + "[PASSWORD]" + pwd.Substring(28, 4) + "|NEW_PWD#" + pwd.Substring(41, 4) + "[PASSWORD]" + pwd.Substring(69, 4);
                    Arr[1] = Arr[1].Replace(pwd, pwdmask);
                    msg = string.Join("", Arr);

                }

                //TRANPWD# Pass giao dich

                if (msg.Contains("TRANPWD#"))
                {
                    string[] Arr = msg.Split(new string[] { "TRANPWD#" }, StringSplitOptions.None);
                    string pwd = Arr[1].Substring(0, 32);
                    string pwdmask = "TRANPWD#" + pwd.Substring(0, 4) + "[PASSWORD]" + pwd.Substring(28, 4);
                    Arr[1] = Arr[1].Replace(pwd, pwdmask);
                    msg = string.Join("", Arr);

                }
            }
            catch
            {
            }
            return msg;

        }


        //anhnd2 
        //PCIDSS 26.06.2015

        public static bool CheckIsGlobalBIN(string BIN)
        {
            if (String.IsNullOrEmpty(BIN))
                return false;
            else if (BIN.Length < 13)
                return false;

            if (!IsNumeric(BIN.Substring(0, 4)))
                return false;

            int BIN_Len = BIN.Length;
            int BIN_1_1 = int.Parse(BIN.Substring(0, 1));
            int BIN_1_2 = int.Parse(BIN.Substring(0, 2));
            int BIN_1_3 = int.Parse(BIN.Substring(0, 3));
            int BIN_1_4 = int.Parse(BIN.Substring(0, 4));

            if (BIN.Length == 15 && (BIN_1_2 == 34 || BIN_1_2 == 37))//American Express
                return true;
            else if (BIN.Length == 14 &&
                (BIN_1_2 == 36 || BIN_1_2 == 38 || BIN_1_2 == 39) ||
                (BIN_1_3 >= 300 && BIN_1_3 <= 305) ||
                BIN_1_3 == 309) //Diners Club International
                return true;
            else if (BIN.Length == 16 && BIN_1_4 >= 3528 && BIN_1_4 <= 3589) //JCB
                return true;
            else if (BIN.Length == 16 && BIN_1_2 >= 51 && BIN_1_2 <= 55) // MasterCard
                return true;
            else if ((BIN.Length == 13 || BIN.Length == 16) && BIN_1_1 == 4) // Visa
                return true;

            return false;
        }


        public static string RemoveSpecialCharacters(string input)
        {
            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, String.Empty);
        }

        private static Random X = new Random();

        #endregion "Others"

        #region "Fee Handle"
        public static double calcFee(string feeType, double amount)
        {
            double feeamt = 0;
            switch (feeType)
            {
                case Config.txType_INTER:
                    feeamt = (amount * Config.interbank_feePercent) / 100;
                    if (feeamt < Config.interbank_minFee) feeamt = Config.interbank_minFee;
                    if (feeamt > Config.interbank_maxFee) feeamt = Config.interbank_maxFee;
                    feeamt = feeamt * (1.1);
                    return feeamt;
                default:
                    return 0;

            }
        }


        /// <summary>
        /// linhtn 20160720
        /// Tạm tính fee. Sau bổ sung thiết kế vào DB
        /// Đối với giao dịch dưới 500 triệu thì phí hiển thị đúng: 0.11% số tiền 
        /// nhưng giao dịch trên 500 triệu thì phí phải là 0.22% số tiền
        /// </summary>
        /// <param name="custid"></param>
        /// <param name="tran_type"></param>
        /// <param name="amount"></param>
        /// /// <param name="channel_id"> Đầu chờ, sau fee tính theo cả kênh giao dịch</param>
        /// <returns></returns>
        public static bool getTotalFee(string custid, string channel_id, string tran_type, double amount, string acc_no, string benf_account, string bank_code, out double total_fee, out double fee_not_vat, out double vat_amt)
        {
            total_fee = -1;
            fee_not_vat = -1;
            vat_amt = -1;

            DataTable dt = new DataTable();

            try
            {

                OracleCommand cmd = new OracleCommand(Config.gEBANKSchema + "pkg_fcs_fee.PRC_GET_TOTAL_FEE_COVID", new OracleConnection(Config.gEBANKConnstr));

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("pv_ref_total_fee", OracleDbType.RefCursor, ParameterDirection.Output);
                cmd.Parameters.Add("pv_cif_no", OracleDbType.Varchar2, custid, ParameterDirection.Input);
                cmd.Parameters.Add("pv_acct_no", OracleDbType.Varchar2, acc_no, ParameterDirection.Input);
                cmd.Parameters.Add("pv_channel", OracleDbType.Varchar2, channel_id, ParameterDirection.Input);
                cmd.Parameters.Add("pv_trans_type", OracleDbType.Varchar2, tran_type, ParameterDirection.Input);
                cmd.Parameters.Add("pv_amount", OracleDbType.Varchar2, amount.ToString(), ParameterDirection.Input);
                cmd.Parameters.Add("pv_benf_account", OracleDbType.Varchar2, benf_account, ParameterDirection.Input);
                cmd.Parameters.Add("pv_bank_code", OracleDbType.Varchar2, bank_code, ParameterDirection.Input);

                OracleDataAdapter apt = new OracleDataAdapter();
                apt.SelectCommand = cmd;
                apt.Fill(dt);

                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    total_fee = double.Parse(dt.Rows[0]["TOTAL_FEE"].ToString());
                    fee_not_vat = double.Parse(dt.Rows[0]["FEE_NOT_VAT"].ToString());
                    vat_amt = double.Parse(dt.Rows[0]["VAT_AMT"].ToString());

                    return true;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
            }

            return false;
        }

        #endregion

        #region "XML Handle"

        public static string getXMLerror(string tran_type, string error_code, string error_desc)
        {
            string ret = string.Empty;
            ret += "<" + tran_type + ">" + "<HEADER>";
            ret += "<res_Result_Code>" + error_code + "</res_Result_Code>";
            ret += "<res_Err_Desc>" + error_desc + "</res_Err_Desc>";
            ret += "</HEADER>" + "</" + tran_type + ">";
            return ret;
        }

        public static string removeDuplicateKey(string inputStr, string key)
        {
            string ret = inputStr;
            int pos1 = 0;
            int pos2 = 0;

            pos1 = inputStr.IndexOf(key);
            pos2 = inputStr.IndexOf(key, pos1 + 1);

            // Neu co cai thu 2 thi
            if (pos2 >= 0)
            {
                //Cat cai thu 2 di
                ret = ret.Remove(pos2, inputStr.IndexOf("~*", pos2) - pos2 + 2);
            }

            return ret;
            /*
                        pos1 = inputStr.IndexOf( key);
                        pos = inputStr.IndexOf( key);
                        string next1 = string.Empty;
                        string next2 = string.Empty;
                        if (pos >=0)
                        {
                            while ( ( next1 != "~") || ( next2 != "*"))
                            {
                                pos ++;
                                next1 = inputStr.Substring(pos, 1);
                                next2 = inputStr.Substring(pos + 1, 1);
                            }
                            ret = inputStr.Substring(0,pos1) + inputStr.Substring(pos+2);
                        }	
                        else
                        {
                            ret = inputStr;
                        }		
                        return ret;*/
        }

        public static void exportToExcel(DataTable source, string fileName, string strHeader, string strFooter, string strMore)
        {

            System.IO.StreamWriter excelDoc;

            excelDoc = new System.IO.StreamWriter(fileName);
            const string startExcelXML = "<xml version>\r\n<Workbook " +
                      "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                      " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                      "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                      "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                      "office:spreadsheet\">\r\n <Styles>\r\n " +
                      "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                      "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                      "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                      "\r\n <Protection/>\r\n </Style>\r\n " +
                      "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                      "x:Family=\"Swiss\" ss:Bold=\"1\"  />\r\n </Style>\r\n " +
                      "<Style ss:ID=\"HeaderStyle\">\r\n <Font " +
                      "x:Family=\"Swiss\" ss:Bold=\"1\" ss:Size=\"14\" ss:Color=\"#E27809\"  />\r\n </Style>\r\n " +
                      "<Style ss:ID=\"SubHeaderStyle\">\r\n <Font " +
                      "x:Family=\"Swiss\" ss:Bold=\"1\" ss:Size=\"10\" ss:Color=\"#0055E5\"  />\r\n </Style>\r\n " +

                      "<Style ss:ID=\"FooterStyle\">\r\n <Font " +
                      "x:Family=\"Swiss\" ss:Bold=\"1\" ss:Size=\"11\" ss:Color=\"#E27809\" />\r\n </Style>\r\n " +

                      //ss:WrapText=\"1\" ss:AutoFitHeight=\"1\" ss:AutoFitWidth=\"1\" ss:Horizontal=\"Center\" ss:Vertical=\"Top\"
                      //ss:WrapText=\"1\" ss:Horizontal=\"Center\" ss:Vertical=\"Top\"  
                      //					  "<Style ss:ID=\"FooterStyle\">\r\n <Font " +
                      //					  "x:Family=\"Swiss\" ss:Bold=\"1\" ss:Size=\"12\" ss:Color=\"#808080\" />\r\n </Style>\r\n " +

                      "<Style ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                      " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                      "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                      //"ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +					  
                      "ss:Format=\"#,###\"/>\r\n </Style>\r\n " +
                      "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                      "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                      "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                      "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                      "</Styles>\r\n ";
            const string endExcelXML = "</Workbook>";

            int rowCount = 0;
            int sheetCount = 1;
            /*
		   <xml version>
		   <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
		   xmlns:o="urn:schemas-microsoft-com:office:office"
		   xmlns:x="urn:schemas-microsoft-com:office:excel"
		   xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
		   <Styles>
		   <Style ss:ID="Default" ss:Name="Normal">
			 <Alignment ss:Vertical="Bottom"/>
			 <Borders/>
			 <Font/>
			 <Interior/>
			 <NumberFormat/>
			 <Protection/>
		   </Style>
		   <Style ss:ID="BoldColumn">
			 <Font x:Family="Swiss" ss:Bold="1"/>
		   </Style>
		   <Style ss:ID="StringLiteral">
			 <NumberFormat ss:Format="@"/>
		   </Style>
		   <Style ss:ID="Decimal">
			 <NumberFormat ss:Format="0.0000"/>
		   </Style>
		   <Style ss:ID="Integer">
			 <NumberFormat ss:Format="0"/>
		   </Style>
		   <Style ss:ID="DateLiteral">
			 <NumberFormat ss:Format="mm/dd/yyyy;@"/>
		   </Style>
		   </Styles>
		   <Worksheet ss:Name="Sheet1">
		   </Worksheet>
		   </Workbook>
		   */
            excelDoc.Write(startExcelXML);
            excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
            excelDoc.Write("<Table>");

            excelDoc.Write("<Row><Cell ss:MergeAcross=\"" + (source.Columns.Count - 1).ToString() + "\" ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">");
            excelDoc.Write(strHeader);
            excelDoc.Write("</Data></Cell></Row>");

            excelDoc.Write("<Row><Cell ss:MergeAcross=\"" + (source.Columns.Count - 1).ToString() + "\" ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">");
            excelDoc.Write("");
            excelDoc.Write("</Data></Cell></Row>");

            excelDoc.Write("<Row><Cell ss:MergeAcross=\"" + (source.Columns.Count - 1).ToString() + "\" ss:StyleID=\"SubHeaderStyle\"><Data ss:Type=\"String\">");
            excelDoc.Write("Ngày sao kê: " + string.Format("{0:dd/MM/yyyy}", System.DateTime.Now));
            excelDoc.Write("</Data></Cell></Row>");

            excelDoc.Write("<Row><Cell ss:MergeAcross=\"" + (source.Columns.Count - 1).ToString() + "\" ss:StyleID=\"SubHeaderStyle\"><Data ss:Type=\"String\">");
            excelDoc.Write(strMore);
            excelDoc.Write("</Data></Cell></Row>");

            excelDoc.Write("<Row><Cell ss:MergeAcross=\"" + (source.Columns.Count - 1).ToString() + "\" ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">");
            excelDoc.Write("");
            excelDoc.Write("</Data></Cell></Row>");

            excelDoc.Write("<Row>");
            for (int x = 0; x < source.Columns.Count; x++)
            {
                excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                excelDoc.Write(source.Columns[x].ColumnName);
                excelDoc.Write("</Data></Cell>");
            }
            excelDoc.Write("</Row>");
            foreach (DataRow x in source.Rows)
            {
                rowCount++;
                //if the number of rows is > 64000 create a new page to continue output

                if (rowCount == 64000)
                {
                    rowCount = 0;
                    sheetCount++;
                    excelDoc.Write("</Table>");
                    excelDoc.Write(" </Worksheet>");
                    excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                    excelDoc.Write("<Table>");
                }
                excelDoc.Write("<Row>"); //ID=" + rowCount + "

                for (int y = 0; y < source.Columns.Count; y++)
                {
                    System.Type rowType;
                    rowType = x[y].GetType();
                    switch (rowType.ToString())
                    {
                        case "System.String":
                            string XMLstring = x[y].ToString();
                            XMLstring = XMLstring.Trim();
                            XMLstring = XMLstring.Replace("&", "&");
                            XMLstring = XMLstring.Replace(">", ">");
                            XMLstring = XMLstring.Replace("<", "<");
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                "<Data ss:Type=\"String\">");
                            excelDoc.Write(XMLstring);
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DateTime":
                            //Excel has a specific Date Format of YYYY-MM-DD followed by  

                            //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000

                            //The Following Code puts the date stored in XMLDate 

                            //to the format above

                            DateTime XMLDate = (DateTime)x[y];
                            string XMLDatetoString = ""; //Excel Converted Date

                            XMLDatetoString = XMLDate.Year.ToString() +
                                "-" +
                                (XMLDate.Month < 10 ? "0" +
                                XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                "-" +
                                (XMLDate.Day < 10 ? "0" +
                                XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                "T" +
                                (XMLDate.Hour < 10 ? "0" +
                                XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                ":" +
                                (XMLDate.Minute < 10 ? "0" +
                                XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                ":" +
                                (XMLDate.Second < 10 ? "0" +
                                XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                                ".000";
                            excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                "<Data ss:Type=\"DateTime\">");
                            excelDoc.Write(XMLDatetoString);
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Boolean":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                "<Data ss:Type=\"String\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                "<Data ss:Type=\"Number\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                "<Data ss:Type=\"Number\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DBNull":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                "<Data ss:Type=\"String\">");
                            excelDoc.Write("");
                            excelDoc.Write("</Data></Cell>");
                            break;
                        default:
                            throw (new Exception(rowType.ToString() + " not handled."));
                    }
                }
                excelDoc.Write("</Row>");
            }
            excelDoc.Write("<Row><Cell ss:MergeAcross=\"" + (source.Columns.Count - 1).ToString() + "\" ss:StyleID=\"FooterStyle\"><Data ss:Type=\"String\">");
            excelDoc.Write("");
            excelDoc.Write("</Data></Cell></Row>");

            excelDoc.Write("<Row><Cell ss:MergeAcross=\"" + (source.Columns.Count - 1).ToString() + "\" ss:StyleID=\"FooterStyle\"><Data ss:Type=\"String\">");
            excelDoc.Write(strFooter);
            excelDoc.Write("</Data></Cell></Row>");


            excelDoc.Write("</Table>");
            excelDoc.Write(" </Worksheet>");
            excelDoc.Write(endExcelXML);
            excelDoc.Close();
        }

        public static DataSet Node2Ds(string XML, string NodeName)
        {
            try
            {
                XML = XML.Replace("&", "");
                System.Xml.XmlNode myNode = Funcs.str2XMLNode(XML).SelectSingleNode(NodeName);
                return Funcs.XmlString2DataSet(myNode.OuterXml);
            }
            catch (Exception ex)
            {
                ex = ex;
                return null;
            }
        }
        public static DataSet XmlString2DataSet(string xmlString)
        {
            //create a new DataSet that will hold our values
            DataSet quoteDataSet = null;

            //check if the xmlString is not blank

            try
            {
                //create a StringReader object to read our xml string
                using (StringReader stringReader = new StringReader(xmlString))
                {
                    //initialize our DataSet
                    quoteDataSet = new DataSet();

                    //load the StringReader to our DataSet
                    quoteDataSet.ReadXml(stringReader);
                }
            }
            catch
            {
                //return null
                quoteDataSet = null;
            }

            //return the DataSet containing the stock information
            return quoteDataSet;
        }

        public static System.Xml.XmlNode str2XMLNode(String strXML)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(strXML);
            return xmlDoc.ChildNodes[0];
        }

        public static bool setXMLValuebyName(System.Xml.XmlNode Node, String NName, String Value)
        {
            for (int i = 0; i <= Node.ChildNodes.Count - 1; i++)
            {
                if (Node.ChildNodes[i].Name == NName)
                {
                    Node.ChildNodes[i].InnerText = Value;
                    return true;
                }
            }
            return false;
        }

        public static string getXMLValuebyName(System.Xml.XmlNode Node, String NName)
        {
            for (int i = 0; i <= Node.ChildNodes.Count - 1; i++)
            {
                if (Node.ChildNodes[i].Name == NName)
                {
                    return Node.ChildNodes[i].InnerText;
                }
            }
            return null;
        }

        #endregion "XML Handle"

        #region "String Handle"

        public static string UTF8ToUTF7(string str)
        {
            //byte[] utf7_bytes = Encoding.UTF7.GetBytes(str);
            //string s = Encoding.Unicode.GetString(utf7_bytes);
            //s = Encoding.UTF7.GetString(utf7_bytes);
            //return s;
            //return Encoding.Unicode.GetString();
            return Encoding.UTF7.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF7, Encoding.Unicode.GetBytes(str)));

        }

        public static string removeStress(string text)
        {
            string[] chars = new string[] { "a", "A", "e", "E", "o", "O", "u", "U", "i", "I", "d", "D", "y", "Y" };
            string[][] uni = new string[14][];
            uni[0] = new string[] { "á", "à", "ạ", "ả", "ã", "â", "ấ", "ầ", "ậ", "ẩ", "ẫ", "ă", "ắ", "ằ", "ặ", "ẳ", "� �" };
            uni[1] = new string[] { "Á", "À", "Ạ", "Ả", "Ã", "Â", "Ấ", "Ầ", "Ậ", "Ẩ", "Ẫ", "Ă", "Ắ", "Ằ", "Ặ", "Ẳ", "� �" };
            uni[2] = new string[] { "é", "è", "ẹ", "ẻ", "ẽ", "ê", "ế", "ề", "ệ", "ể", "ễ" };
            uni[3] = new string[] { "É", "È", "Ẹ", "Ẻ", "Ẽ", "Ê", "Ế", "Ề", "Ệ", "Ể", "Ễ" };
            uni[4] = new string[] { "ó", "ò", "ọ", "ỏ", "õ", "ô", "ố", "ồ", "ộ", "ổ", "ỗ", "ơ", "ớ", "ờ", "ợ", "ở", "� �" };
            uni[5] = new string[] { "Ó", "Ò", "Ọ", "Ỏ", "Õ", "Ô", "Ố", "Ồ", "Ộ", "Ổ", "Ỗ", "Ơ", "Ớ", "Ờ", "Ợ", "Ở", "� �" };
            uni[6] = new string[] { "ú", "ù", "ụ", "ủ", "ũ", "ư", "ứ", "ừ", "ự", "ử", "ữ" };
            uni[7] = new string[] { "Ú", "Ù", "Ụ", "Ủ", "Ũ", "Ư", "Ứ", "Ừ", "Ự", "Ử", "Ữ" };
            uni[8] = new string[] { "í", "ì", "ị", "ỉ", "ĩ" };
            uni[9] = new string[] { "Í", "Ì", "Ị", "Ỉ", "Ĩ" };
            uni[10] = new string[] { "đ" };
            uni[11] = new string[] { "Đ" };
            uni[12] = new string[] { "ý", "ỳ", "ỵ", "ỷ", "ỹ" };
            uni[13] = new string[] { "Ý", "Ỳ", "Ỵ", "Ỷ", "Ỹ" };
            int j;
            for (int i = 0; i <= 13; i++)
            {
                for (j = 0; j < uni[i].Length; j++)
                {
                    text = text.Replace(uni[i][j], chars[i]);
                }
            }

            return text;
        }

        public static String getRndSmartCardToken(int strLength)
        {
            string[] Vert = { "A", "B", "C", "D", "E", "F", "G", "H" };
            string[] Hozi = { "0", "1", "2", "3", "4", "5", "6" };

            string Selchar;
            string tmpStr = "";
            int Selpos;

            for (int x = 0; x < strLength; ++x)
            {
                if (x % 2 == 0)
                {
                    Selpos = X.Next(0, Vert.Length - 1);
                    Selchar = Vert[Selpos];
                    tmpStr += Selchar;
                }
                else
                {
                    Selpos = X.Next(0, Hozi.Length - 1);
                    Selchar = Hozi[Selpos];
                    tmpStr += Selchar;
                }
            }
            return tmpStr;

        }

        public static string getSecCard(string SecCard, string Pos)
        {
            //			if (SecCard.Length==0) return "   ";
            //			//'A1A2A3'
            //			//A2
            //			string[] Vert = {"A","B","C","D","E"};
            //			string[] Hozi = {"0","1","2","3","4","5","6","7","8","9"};
            //			char firstChar ;
            //			char secondChar;
            //			int num;
            //			string retStr="";
            //			for (int i=0;i<=2;i++)
            //			{	
            //				firstChar = Pos.Substring(i*2,1).ToCharArray()[0];
            //				secondChar = Pos.Substring(i*2+1,1).ToCharArray()[0];
            //				num = (Convert.ToInt16(firstChar) - 65)*10 + (Convert.ToInt16(secondChar) - 48);
            //				retStr += SecCard.Substring(num,1);
            //			}
            string retStr = "";
            char firstChar;
            char secondChar;
            int num;
            // A2
            firstChar = Pos.Substring(0, 1).ToCharArray()[0];
            secondChar = Pos.Substring(1, 1).ToCharArray()[0];
            //num = (Convert.ToInt16(firstChar) - 65)*10 + (Convert.ToInt16(secondChar) - 48) *3;
            num = (Convert.ToInt16(secondChar) - 48) * 24 + (Convert.ToInt16(firstChar) - 65) * 3;
            retStr += SecCard.Substring(num, 3);
            return retStr;
        }
        public static bool check_pwd(string str)
        {
            string vstr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@$~!@#$%^&*()-=+ @$.-, /`";
            string vnum = "1234567890";
            string vchar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < str.Length; i++)
            {
                if (vstr.IndexOf(str.Substring(i, 1).ToUpper()) < 0)
                {
                    return false;
                }
            }

            if (str.Length < 6)
            {
                return false;
            }

            bool result = false;

            for (int i = 0; i < vnum.Length; i++)
            {
                if (str.IndexOf(vnum.Substring(i, 1).ToUpper()) > 0)
                {
                    result = true;
                    break;
                }
            }

            if (result)
            {
                result = false;

                for (int i = 0; i < vchar.Length; i++)
                {
                    if (str.ToUpper().IndexOf(vchar.Substring(i, 1).ToUpper()) > 0)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        public static String getAlphabets(int strLength)
        {
            String RandomString = "";

            for (int x = 0; x < strLength; ++x)
            {
                if (x % 2 == 0)
                {
                    char tmpChar = '0';

                    while (tmpChar == '0' || tmpChar == '8')
                    {
                        tmpChar = (char)(X.Next(48, 57));
                    }

                    RandomString += tmpChar;
                }
                else
                {
                    char tmpChar = 'O';
                    while ((tmpChar == 'O') || (tmpChar == 'I') || (tmpChar == 'B'))
                    {
                        tmpChar = (char)(X.Next(65, 90));
                    }
                    RandomString += tmpChar;
                }
            }

            return RandomString.ToLower();
        }
        public static string reFormat(String src, string Type)
        {
            string tmpsrc = "";
            try
            {
                switch (Type)
                {
                    case "EXPDATE":
                        return src.Substring(0, 2) + "/" + src.Substring(2, 2) + "/" + src.Substring(4, 2);
                    case "CODE":

                        while (src.Length >= 4)
                        {
                            tmpsrc += src.Substring(0, 4) + "-";
                            src = src.Substring(4);
                        }
                        if (src.Length > 0)
                        {
                            tmpsrc += src;
                        }
                        else
                        {
                            tmpsrc = tmpsrc.Substring(0, tmpsrc.Length - 1);
                        }
                        return tmpsrc;
                    case "PERIOD":

                        tmpsrc = src;
                        return tmpsrc;
                    default:
                        return src;
                }
            }
            catch
            {
                return src;
            }
        }
        #endregion "String Handle"

        #region "File Handle"

        public static void WriteLog_IFACE(String msg)
        {
            //msg = Funcs.getMaskingStr(msg);
            WriteLog(msg);
            //bool StayInLoop;
            //int trys = 0;
            //int waitTime = 3000; // Thu trong 3s 
            //int sleepTime = 100;// moi lan sleep 1/10 s, thu trong 30 lan
            //StreamWriter SW = null;
            //do
            //{
            //    StayInLoop = false;
            //    try
            //    {
            //        SW = File.AppendText(Config.gLogFile_IFACE + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            //        SW.WriteLine("\r\n ====================================================================");
            //        SW.WriteLine("\r\n [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]: " + msg);
            //        break;
            //    }
            //    catch
            //    {
            //        trys++;
            //        StayInLoop = true;
            //        System.Threading.Thread.Sleep(sleepTime);
            //        if (SW != null)
            //        {
            //            SW.Close();
            //        }
            //    }
            //}
            //while (StayInLoop && trys < waitTime / sleepTime);
            //if (SW != null)
            //{
            //    SW.Close();
            //}
        }

        public static void WriteLog4net(log4net.ILog l4N, object input)
        {
            String logJson = String.Empty;
            if (input != null && l4N != null)
            {
                try
                {
                    logJson = (string)input;
                    //logJson = Funcs.getMaskingStr(json);
                    l4N.Info(logJson);
                }
                catch (Exception ex)
                {
                    WriteFileLog(logJson);
                }
            }
        }
        public static void WriteLog(String msg)
        {
            StringBuilder sbLog = new StringBuilder();
            //sbLog.AppendLine(Config.gLogFile + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            //sbLog.AppendLine("====================================================================");
            sbLog.AppendLine(msg);
            WriteLog4net(l4NC, sbLog.ToString());
        }

        public static void WriteInfo(String msg)
        {
            WriteLog(msg);
        }

        public static void WriteLog_old(String msg)
        {
            try
            {
                StreamWriter SW;
                SW = File.AppendText(Config.gLogFile + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                SW.WriteLine("\r\n ====================================================================");
                SW.WriteLine("\r\n [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]:ERROR: [" + msg + "]");
                SW.Close();
            }
            catch
            {
            }
        }
        #endregion "File Handle"

        #region "Datetime Handle"

        public static DateTime str2date(string src)
        {
            string tmpstr = src.Substring(3, 2);// 10/12/2009
            switch (tmpstr)
            {
                case "01":
                    tmpstr = "Jan"; break;
                case "02":
                    tmpstr = "Feb"; break;
                case "03":
                    tmpstr = "Mar"; break;
                case "04":
                    tmpstr = "Apr"; break;
                case "05":
                    tmpstr = "May"; break;
                case "06":
                    tmpstr = "Jun"; break;
                case "07":
                    tmpstr = "Jul"; break;
                case "08":
                    tmpstr = "Aug"; break;
                case "09":
                    tmpstr = "Sep"; break;
                case "10":
                    tmpstr = "Oct"; break;
                case "11":
                    tmpstr = "Nov"; break;
                case "12":
                    tmpstr = "Dec"; break;
            }
            src = src.Substring(0, 2) + " " + tmpstr + "," + src.Substring(6, 4);
            return Convert.ToDateTime(src);

        }

        public static string showDate(string src)
        {
            if ((src == null) || (src == "")) src = "99991231";
            return src.Substring(6, 2) + "/" + src.Substring(4, 2) + "/" + src.Substring(0, 4);
        }

        #endregion"Datetime Handle"

        #region "Others Handle"
        public static string NoHack(String src)
        {
            try
            {

                //return src.Trim().Replace("'","''").Replace(",","").Replace("|", "").Replace("#", "");
                return src.Replace("'", "''").Replace(",", "").Replace("|", "").Replace("#", "").Replace("^", "").Replace("$", "");
            }
            catch
            {
                return null;
            }
        }

        public static bool IsNumeric(string s)
        {
            try
            {
                Double.Parse(s);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string Encrypt(string strKey, string strText)

        {

            PasswordDeriveBytes cdk = new PasswordDeriveBytes(strKey, null);



            // generate an RC2 key

            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            byte[] key = cdk.CryptDeriveKey("RC2", "MD5", 128, iv);

            //Console.WriteLine(key.Length * 8);



            // setup an RC2 object to encrypt with the derived key

            RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();

            rc2.Key = key;

            rc2.IV = new byte[] { 21, 22, 23, 24, 25, 26, 27, 28 };



            // now encrypt with it

            byte[] plaintext = Encoding.UTF8.GetBytes(strText);

            MemoryStream ms = new MemoryStream();

            CryptoStream cs = new CryptoStream(ms, rc2.CreateEncryptor(),

                CryptoStreamMode.Write);



            cs.Write(plaintext, 0, plaintext.Length);

            cs.Close();

            byte[] encrypted = ms.ToArray();



            // Convert to Base64

            string txtOut = Convert.ToBase64String(encrypted);



            // THIS IS THE ENCRYPTED TEXT

            return txtOut;

        }



        public static string Decrypt(string strKey, string strText)

        {

            PasswordDeriveBytes cdk = new PasswordDeriveBytes(strKey, null);



            // generate an RC2 key

            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            byte[] key = cdk.CryptDeriveKey("RC2", "MD5", 128, iv);

            //Console.WriteLine(key.Length * 8);



            // setup an RC2 object to encrypt with the derived key

            RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();

            rc2.Key = key;

            rc2.IV = new byte[] { 21, 22, 23, 24, 25, 26, 27, 28 };



            // now encrypt with it

            //byte[] plaintext = Encoding.UTF8.GetBytes(strText);

            byte[] plaintext = Convert.FromBase64String(strText);

            MemoryStream ms = new MemoryStream();

            CryptoStream cs = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(plaintext, 0, plaintext.Length);

            cs.Close();

            byte[] encrypted = ms.ToArray();



            System.Text.Encoding encoding = System.Text.Encoding.UTF8;



            // THIS IS THE DECRYPTED TEXT

            return encoding.GetString(encrypted);



            //this.txtOut.Text = txtOut;

        }

        public static string encryptMD5(String input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {


                if (hash[i] < 16)
                    sb.Append("0" + hash[i].ToString("x"));
                //ret += "0" + a.ToString ("x");
                else
                    sb.Append(hash[i].ToString("x"));
            }
            string str = sb.ToString();
            return str;
        }
        public static Double getFee(String Acctno, string ccy, bool same)
        {
            switch (ccy)
            {
                case "VND":
                    if (same)
                    {
                        return Config.feeAmount_SC_VND;
                    }
                    else
                    {
                        return Config.feeAmount_DC_VND;
                    }
                default:
                    if (same)
                    {
                        return Config.feeAmount_SC_OTHER;
                    }
                    else
                    {
                        return Config.feeAmount_DC_OTHER;
                    }

            }
        }


        public static string GetWebPage(string URL)
        {
            try
            {
                // Create a request for the URL.   
                WebRequest request = WebRequest.Create(URL);

                //request.Proxy = null;

                // If required by the server, set the credentials.
                request.Credentials = CredentialCache.DefaultCredentials;

                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Display the status.
                // Console.WriteLine (response.StatusDescription);

                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

                // Cleanup the streams and the response.
                reader.Close();
                dataStream.Close();
                response.Close();

                return (responseFromServer);

            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.ToString());
                return "";
            }
        }
        #endregion "Other Handle"

        #region ReadAllFile
        /// <summary>
        /// Reads all file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string ReadAllFile(string filePath)
        {
            string text = string.Empty;

            if (System.IO.File.Exists(filePath))
            {
                text = System.IO.File.ReadAllText(filePath);
            }

            return text;
        }

        #endregion ReadAllFile

        public static bool Check_Valid_UUID(string uuid)
        {
            return true;
        }

        public static String getAlphabets1(int strLength)
        {
            String RandomString = "";

            for (int x = 0; x < strLength; ++x)
            {
                if (x % 2 == 0)
                {
                    char tmpChar = '0';

                    while (tmpChar == '0' || tmpChar == '8')
                    {
                        tmpChar = (char)(X.Next(48, 57));
                    }

                    RandomString += tmpChar;
                }
                else
                {
                    char tmpChar = 'O';
                    while ((tmpChar == 'O') || (tmpChar == 'I') || (tmpChar == 'B'))
                    {
                        tmpChar = (char)(X.Next(65, 90));
                    }
                    RandomString += tmpChar;
                }
            }

            return RandomString.ToLower();
        }

        public static bool sendEmail(string to, string subject, string content, AlternateView view = null, bool isBodyHtml = false)
        {
            bool sendVal = false;

            try
            {
                string[] arrMailTo = to.Split(';');
                if (arrMailTo.Length <= 0) return sendVal;
                string mailTo = string.Empty;

                for (int i = 0; i < arrMailTo.Length; i++)
                {
                    System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();
                    myEmail.BodyEncoding = System.Text.Encoding.UTF8;
                    myEmail.IsBodyHtml = true;

                    myEmail.Priority = MailPriority.High;
                    myEmail.To.Add(new MailAddress(arrMailTo[i]));
                    //add new bcc email
                    // myEmail.Bcc.Add(new MailAddress(SecurityIBHelper.getConfigVal("MAIL_BCC_USER")));

                    myEmail.Subject = subject;
                    myEmail.From = new MailAddress(Config.Email.fromEmail, Config.Email.senderEmail);
                    myEmail.Body = content;
                    if (isBodyHtml)
                    {
                        myEmail.IsBodyHtml = isBodyHtml;
                    }
                    else
                    {
                        if (view != null)
                        {
                            myEmail.AlternateViews.Add(view);
                        }
                    }

                    SmtpClient emailClient = new SmtpClient(Config.Email.IPMailServer, Config.Email.mailServerPort);
                    System.Net.NetworkCredential smtpUserInfor = new System.Net.NetworkCredential(Config.Email.username, Config.Email.password);
                    emailClient.Credentials = smtpUserInfor;
                    emailClient.EnableSsl = true;

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    { return true; };

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    emailClient.Send(myEmail);

                    sendVal = true;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog(ex.Message.ToString());
            }

            return sendVal;
        }

        public static string ConvertMoney(string input)
        {
            try
            {
                return Convert.ToDecimal(input, new System.Globalization.CultureInfo("en-US")).ToString("#,###0.##", System.Globalization.CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
            catch (Exception e)
            {
                return string.Empty;
            }

        }

        public static string maskingEmail(string email)
        {
            string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
            string result = Regex.Replace(email, pattern, m => new string('*', m.Length));
            return result;
        }

        public static string maskingMobile(string mobile)
        {
            string befor = mobile.Substring(0,3);
            string after = mobile.Substring(mobile.Length - 3, 3);
            string result = string.Format("{0}XXXX{1}", befor, after);
            return result;
        }

        public static string GetAccGLCredit(string inputString)
        {
            string str = "";
            if (inputString.Substring(0, 1).Equals("5"))
            {
                str = ConfigurationManager.AppSettings.Get("ACCT_GL_CARD_CREDIT_5");
            }

            if (inputString.Substring(0, 1).Equals("4"))
            {
                str = ConfigurationManager.AppSettings.Get("ACCT_GL_CARD_CREDIT_4");
            }

            return str;
        }

        public static String RemoveVietnameseCharacter(String str)

        {

            if (string.IsNullOrEmpty(str)) return str;

            for (int i = 1; i < VietnameseSigns.Length; i++)

            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            str = str.ToUpper();


            return str;

        }

        public static String RemoveVietnameseCharacterNoUpper(String str)
        {

            if (string.IsNullOrEmpty(str)) return str;

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }

            return str;

        }

        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"

        };

        //QUANNM3 - Ghi them log vao file 
        public static void WriteFileLog(string errCode)
        {
            try
            {
                string driver = "E:";
                if (Directory.Exists("E:\\"))
                {
                    driver = "E:";
                }
                else if (Directory.Exists("D:\\"))
                {
                    driver = "D:";
                }

                string path = Funcs.getConfigVal("LOG_FILE");

                path = GetLogFileName(path) + DateTime.Now.ToString("yyyyMMdd");

                if (!errCode.Equals(""))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FileStream file = new FileStream(path + "\\" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + Guid.NewGuid().ToString() + ".txt", FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(file);
                    sw.Write(errCode);
                    sw.Close();
                    file.Close();
                }
            }
            catch
            {

            }
        }

        public static string GetLogFileName(string path)
        {
            try
            {
                var rootAppender = LogManager.GetRepository()
                                         .GetAppenders()
                                         .OfType<FileAppender>()
                                         .FirstOrDefault();
                if (rootAppender != null)
                {
                    path = Path.GetDirectoryName(rootAppender.File) + "\\";
                }
            }
            catch (Exception ex)
            {
                
            }
            
            return path;
        }

        public static string GetError(string[] strError, string errCode)
        {
            string strMessage = "";
            try
            {
                if (errCode != null)
                {
                    for (int i = 0; i < strError.Length; i++)
                    {
                        if (strError[i].Split('|')[0].Equals(errCode))
                        {
                            string temp = String.Empty;
                            temp = strError[i].Split('|')[1].ToString() + "$" + strError[i].Split('|')[2].ToString();

                            return temp;
                        }
                    }
                }
                else
                {
                    return "LOI KHONG XAC DINH$ERROR";
                }
            }
            catch
            {
                return "LOI KHONG XAC DINH$ERROR";
            }

            return strMessage;
        }

        public static string GetCoreDate(string format)
        {
            Utility ul = new Utility();
            DataSet ds = ul.getCoreDate();

            string vldate = string.Empty;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (format.Equals("dd/MM/yyyy"))
                {
                    vldate = DateTime.ParseExact(ds.Tables[0].Rows[0]["CORE_DT"].ToString(), "yyyyMMdd", null).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    vldate = DateTime.ParseExact(ds.Tables[0].Rows[0]["CORE_DT"].ToString(), "yyyyMMdd", null).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                }

                
            }

            return vldate;
        }

        public static bool CompeDateTime(string from,string to)
        {
            DateTime fromDate = DateTime.ParseExact(from, "dd/MM/yyyy", null);
            DateTime toDate = DateTime.ParseExact(to, "dd/MM/yyyy", null);

            int cmp = fromDate.CompareTo(toDate);
            
            if (cmp > 0)
            {
                return false;
            }
            else  return true;
		}

        public static string MatchEvaluator(Match m)
        {
            string temp = m.Value.ToString().Substring(2);
            temp = temp.Substring(0, temp.Length - 1);
            char c = (char)int.Parse(temp);
            return c.ToString();
        }

        public static string getMonth(int src)
        {
            string tmpstr = "";
            string res = "";

            switch (src)
            {
                case 1:
                    tmpstr = "January"; break;
                case 2:
                    tmpstr = "February"; break;
                case 3:
                    tmpstr = "March"; break;
                case 4:
                    tmpstr = "April"; break;
                case 5:
                    tmpstr = "May"; break;
                case 6:
                    tmpstr = "June"; break;
                case 7:
                    tmpstr = "July"; break;
                case 8:
                    tmpstr = "August"; break;
                case 9:
                    tmpstr = "September"; break;
                case 10:
                    tmpstr = "October"; break;
                case 11:
                    tmpstr = "November"; break;
                case 12:
                    tmpstr = "December"; break;
            }

            res = "Số dư bình quân tháng " + src + Config.COL_REC_DLMT + "Average balance in " + tmpstr;

            return res;
		}

        public static String gResult_Acct_Open_Arr(int index)
        {
            String message = Config.gResult_Acct_Open_Arr[index];
            String[] messageItems = message.Split('|');


            message = messageItems[1] + Config.COL_REC_DLMT + messageItems[2];

            return message;
		}

        public static string UpperFirstCharacter(String input)
        {
            if (String.IsNullOrEmpty(input)) return string.Empty;
            input = input.ToLower();
            char[] a = input.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }
    }
}
