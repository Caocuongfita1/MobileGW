using System;
using System.Xml;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using mobileGW.Service.Framework;

namespace mobileGW.SHBUCS_XML
{
	/// <summary>
	/// Summary description for Service.
	/// </summary>
	public class Service
	{
		//Real
		/*
		string NVPPath = "D:\\LiveApp\\ibanking\\SHBUCS_XML\\NVP.xml";
		string ServerIP = "10.4.28.54"; // real M3
		*/
		

		//Test
		/**/
		//string NVPPath = "D:\\Projects\\Ebank 2.0\\EBANK_PROJECT\\iBanking\\SHBUCS_XML\\NVP.xml";
		//string NVPPath = "E:\\EBANK_PROJECT\\iBanking\\SHBUCS_XML\\NVP.xml";
		
//		string NVPPath = "D:\\App test\\mobileGW\\SHBUCS_XML\\NVP.xml";
//		//string NVPPath = "C:\\Inetpub\\wwwroot\\mobileGW\\SHBUCS_XML\\NVP.xml";
//		
//		string ServerIP = "10.4.28.58"; // test
//		//string ServerIP = "10.4.28.57"; // training
//			int Port = 8024;
		
		string NVPPath =  Config.gINTEGRATOR_NVP_PATH; //;"D:\\App test\\iBanking_Onepay\\SHBUCS_XML\\NVP.xml";		
		string ServerIP = Config.gINTEGRATOR_SERVER_IP; //"10.4.28.58"; // test		
		int Port = int.Parse( Config.gINTEGRATOR_SERVER_PORT) ;//8024;

		XmlDocument XmlDoc = new XmlDocument();
		public int Timeout;
		int TimeOut=60;
		
		//string ServerIP = "10.4.28.55"; // real bk M4
		
	
		string errGeneral = "99999";
		string errGeneralDesc = "Lỗi không xác định!";
		string errSuccess = "00000";
		/*
		  RTE = Transaction error in CORE
		  RCE = Core Down
		 */
		
		public Service()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			XmlDoc.Load(NVPPath);
		}
		public void Dispose()
		{

		}
		private string sendMsg(string ServerIP,int ServerPort,string MsgCnt)
		{
			System.Net.Sockets.TcpClient tcpClient;
			NetworkStream networkStream;

			try
			{
				tcpClient = new System.Net.Sockets.TcpClient();
				System.Net.IPEndPoint myEP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse( ServerIP),ServerPort);
				tcpClient.SendTimeout = TimeOut;
				tcpClient.NoDelay = true;
				tcpClient.Connect(myEP);
   				networkStream = tcpClient.GetStream();
   				if (networkStream.CanWrite & networkStream.CanRead) 
				{
       		
					MsgCnt = MsgCnt.Remove( MsgCnt.Length-2,2);
					Byte[] sendBytes = Encoding.UTF8.GetBytes(MsgCnt);
       				networkStream.Write(sendBytes, 0, sendBytes.Length);
       			
       				byte[] bytes ;
					string returndata = "";
					tcpClient.ReceiveBufferSize = 8912*5;
					bytes = new byte[tcpClient.ReceiveBufferSize + 1];
					networkStream.Read (bytes, 0, (int)tcpClient.ReceiveBufferSize);
						
					returndata = Encoding.UTF8.GetString(bytes);						
					       				
       				// Output the data received from the host to the console.
       				
					tcpClient.Close();
					tcpClient = null;
					networkStream.Close();
					networkStream = null;

       				return returndata.Trim().Replace("\0","");
   				}
   				else 
				{
            		tcpClient.Close();
					tcpClient = null;
					networkStream.Close();
					networkStream = null;
					return "";
   				}
				
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				tcpClient = null;
				networkStream = null;
				return "";
			}
		}
		

		public string BALANCE_ENQUIRY(string CHANNELID,string INTERFACEID,string ACCTNO,string REMARK,string REFNO)
		{
			string XMLNODE="BALANCE_ENQUIRY";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");

			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				// Lay thong so ve ki tu phan cach 
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();

				// Kiem tra so node va them vao bang Hash va Key
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				// Gan cac thong so can thiet, cac thong so khac da co gia tri default trong file XML
				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Acc_No"]= ACCTNO;
				myHash["req_Remarks"]= REMARK;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"]= DateTime.Now.ToString("yyyyMMdd");

				// Generate ra Outgoing mess de gui di
				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Gui den Intellect Integrator

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");

				// Xu ly message tra ve
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");

				if (retStr.Length>0)
				{
					// Chuoi tra ve khac rong, xu ly tiep
					// Lay thong tin message tra ve
					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					////string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					// Nap thong tin message tra ve vao bang Hash va Key
					string[] myArray = Regex.Split(retStr,Spliter_Regex);					
					retXML += "<" +  XMLNODE +">";
					retXML += "<HEADER>";

					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
						//Gen XML
						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";

					}
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";

					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					// Kiem tra thong tin tra ve
					if ( myHash["res_Result_Code"].ToString()!= errSuccess)
					{	
						// Neu loi thi tra loi cho client
						return  Funcs.getXMLerror  ( XMLNODE, myHash["res_Result_Code"].ToString(), "");
					}					
					//return resStr;
					return retXML;
				}
				else
				{	// Chuoi tra ve la rong, bao loi!!!!
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror  ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				// Cac loi linh tinh
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror  ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}

		
		public string FINANCIAL_POSTING(string CHANNELID,string INTERFACEID, string SOURCE_BRANCH_CODE, string POSTINFO,string EXT_REMARKS, string INT_REMARKS, string REFNO)
		{
			string XMLNODE = "FINANCIAL_POSTING";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			Funcs.WriteLog_IFACE(" - Parse the POSTINFO - [" + POSTINFO + "]");

			// Xu ly thong tin trong tham so POSTINFO 
			// Tham so se duoc viet theo dang nhu sau - BRANCH_CODE|ACCTNO|D/C|AMT|CCY~ACCTNO|D/C|AMT|CCY
			string[] myArray = POSTINFO.Split('~');
            
			// Bat dau sinh ra outgoing message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				// Lay thong tin ky tu phan chia
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				
				// Duyet qua cac Node cua outgoing message va nap vao hash va key
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					if (myNode.ChildNodes[i].Name!="SEGMENT")
					{
						myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
						myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
					}	
				}

				// Gan cac thong so can thiet
				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");
				myHash["req_Src_Br_Cd"]= SOURCE_BRANCH_CODE;				
				
				// Gan so luong segment
				myHash["req_No_Of_Seg"]= myArray.Length;

				// Chuyen phan message hien tai da co ra String de dung hash va Key array vao viec khac
				string retStr="";
				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}

				// Chuyen vao node SEGMENT
				myNode = myNode.SelectSingleNode("SEGMENT");
				myHash = new Hashtable();
				myKeys = new ArrayList();

				// Nap vao Hash va Key thogn tin cua SEGMENT
				for (int j=0;j<myNode.ChildNodes.Count;j++)
				{
					myHash.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[j].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText);
				}

				// Gan thong tin POSTINFO vao mang hash
				for (int i=0;i<myArray.Length;i++)
				{
					
					myHash["req_Seg_No"] = i+1;
					myHash["req_Acc_Br_Cd"] = myArray[i].Split('|')[0];
					myHash["req_Acc_No"] = myArray[i].Split('|')[1];
					myHash["req_Dr_Cr_Flg"] = myArray[i].Split('|')[2];
					myHash["req_Amt"] = myArray[i].Split('|')[3];
					myHash["req_CCY"] = myArray[i].Split('|')[4];
					myHash["req_Ext_Remarks"] = EXT_REMARKS;
					myHash["req_Int_Remarks"] = INT_REMARKS;
					

					// Chuyen thanh String de gui di
					for (int k=0;k<myKeys.Count;k++)
					{
						if (myHash[myKeys[k].ToString()].ToString().Length>0)
						{
							retStr += myKeys[k].ToString() + "=" + myHash[myKeys[k].ToString()] + Spliter;
						}
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
                // Gui thong tin sang Intellect Intergator

                //linhtn fix bug 2017 01 24

                //fix loi message length = 902 gui vao core bi timeout
                if (retStr.Length == 902)
                {
                    retStr = retStr.Replace(EXT_REMARKS, EXT_REMARKS + " ");
                }

                Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");

				if (retStr.Length>0)
				{

					//myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");
					// Xu ly message tra ve

					//string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					myArray = Regex.Split(retStr,Spliter_Regex);
					retXML += "<" +  XMLNODE +">";	
					retXML += "<HEADER>";
					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split('=')[0],myArray[i].ToString().Split('=')[1]);
						myKeys.Add(myArray[i].ToString().Split('=')[0]);
						//Gen XML
						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";
						if (myArray[i].ToString().Split('=')[0]=="res_No_Of_Seg") break;
					}
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";	
					// Tra ve message
					//resStr = myHash["res_Result_Code"].ToString();
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					
					if  ( myHash["res_Result_Code"].ToString() != errSuccess)
					{
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");//, myHash["res_Err_Desc"].ToString());
					}						
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string FUNDTRANSFER(string CHANNELID,string INTERFACEID,string ACCTNO,string CR_ACCTNO,decimal AMOUNT,string REMARK, string INT_REMARK, string REFNO )
		{
			string XMLNODE="FUNDTRANSFER";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				//fix for test

				myHash["req_Acc_No"]= ACCTNO;
				myHash["req_Cr_Acc_No"]= CR_ACCTNO;
				myHash["req_Int_Remarks"]= INT_REMARK;
				myHash["req_Ext_Remarks"]= REMARK;
				myHash["req_Amt"]= AMOUNT;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"] = "20100430";//DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					////string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					retXML += "<" +  XMLNODE +">";	
					retXML += "<HEADER>";
					
					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
						//Gen XML
						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";							
					}
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";	

					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					if ( myHash["res_Result_Code"].ToString() != errSuccess)
					{
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");//, myHash["res_Err_Desc"].ToString());
					}
					//return resStr;
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return errGeneral;
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				//return errGeneral;
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string FXRATE_ENQUIRY(string CHANNELID,string INTERFACEID,string QUERYINFO,string RATETYPE,string REFNO)
		{
			string XMLNODE = "FXRATE_ENQUIRY";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			Funcs.WriteLog_IFACE(" - Parse the QUERYINFO - [" + QUERYINFO + "]");
			// Parse the QUERYINFO - VND~USD~EUR
			string[] myArray = QUERYINFO.Split('~');
            
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();

				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					if (myNode.ChildNodes[i].Name!="SEGMENT")
					{
						myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
						myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
					}	
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Rate_Type"]= RATETYPE;
				myHash["req_No_Of_Seg"]= myArray.Length;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				myNode = myNode.SelectSingleNode("SEGMENT");
				myHash = new Hashtable();
				myKeys = new ArrayList();

				for (int j=0;j<myNode.ChildNodes.Count;j++)
				{
					myHash.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[j].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText);
				}

				for (int i=0;i<myArray.Length;i++)
				{
					
					myHash["req_Seg_No"] = i+1;
					myHash["req_Ccy_Cd"] = myArray[i].ToString();

					for (int k=0;k<myKeys.Count;k++)
					{
						if (myHash[myKeys[k].ToString()].ToString().Length>0)
						{
							retStr += myKeys[k].ToString() + "=" + myHash[myKeys[k].ToString()] + Spliter;
						}
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");

				if (retStr.Length>0)
				{

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					////string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();
					myArray = Regex.Split(retStr,Spliter_Regex);

					int idx;
					retXML += "<" +  XMLNODE +">";
					retXML += "<HEADER>";
					for (idx=0;idx<myArray.Length-1;idx++)
					{
						myHash.Add(myArray[idx].ToString().Split('=')[0],myArray[idx].ToString().Split('=')[1]);
						myKeys.Add(myArray[idx].ToString().Split('=')[0]);
						//Gen XML
						retXML += "<" + myArray[idx].ToString().Split('=')[0] + ">";
						retXML += myArray[idx].ToString().Split('=')[1];
						retXML += "</" + myArray[idx].ToString().Split('=')[0] + ">";

						if (myArray[idx].ToString().Split('=')[0]=="res_No_Of_Seg") break;
					}
					retXML += "</HEADER>";
					if (myHash["res_Result_Code"].ToString()==errSuccess)
					{
						int numRec = Int32.Parse(myHash["res_No_Of_Seg"].ToString());

						myNode = myNode.SelectSingleNode("SEGMENT");
						idx++;
						//BEGIN XML SEGMENT
						retXML += "<SEGMENT>";
						for (int i=0;i<numRec;i++)
						{
						
							myKeys = new ArrayList();
							myHash = new Hashtable();

							// Tao ra Message
							for (int j=0;j<myNode.ChildNodes.Count;j++)
							{
								myHash.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[j].SelectSingleNode("VAL").InnerText);
								myKeys.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText);
							}

							// Gan gia tri
												
							myHash[myArray[idx].ToString().Split('=')[0]]=myArray[idx].ToString().Split('=')[1];
							idx++;
							retXML += "<res_Seg_No>";
							while ((myArray[idx].ToString().Split('=')[0]!="res_Seg_No")&&(idx<myArray.Length-1))
							{
								myHash[myArray[idx].ToString().Split('=')[0]]=myArray[idx].ToString().Split('=')[1];
								retXML += "<" + myArray[idx].ToString().Split('=')[0] + ">";
								retXML += myArray[idx].ToString().Split('=')[1];
								retXML += "</" + myArray[idx].ToString().Split('=')[0] + ">";
								idx++;
							}
							retXML += "</res_Seg_No>";
						
							//							// Bien thanh message
							//							for (int j=0;j<myKeys.Count;j++)
							//							{
							//								if (myHash[myKeys[j].ToString()].ToString().Length>0)
							//								{
							//									resStr += myHash[myKeys[j].ToString()] + Spliter;
							//								}
							//							}
							//							resStr = resStr.Substring(0,resStr.Length-Spliter.Length) + "###";
						}
						//						resStr = errSuccess + "###" + resStr;
						//END XML SEGMENT
						retXML += "</SEGMENT>";
						retXML += "</" + XMLNODE +">";	
						Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
						Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
						Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
						//return resStr;
						return retXML;
					}
					else
					{
						Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
						Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
						Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
						return  Funcs.getXMLerror  ( XMLNODE, myHash["res_Result_Code"].ToString(), "");
					}
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return errGeneral;
					return  Funcs.getXMLerror  ( XMLNODE, errGeneral, errGeneralDesc);					
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror  ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string LOANACC_ENQUIRY(string CHANNELID,string INTERFACEID,string ACCTNO,string REFNO)
		{
			string XMLNODE="LOANACC_ENQUIRY";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Loan_Ref"]= ACCTNO;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					//string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					string retXML = string.Empty;
					retXML += "<" +  XMLNODE +">";	
					retXML += "<HEADER>";

					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
						//GEN XML
						retXML += "<" + myArray[i].ToString().Split("=".ToCharArray())[0] + ">";
						retXML += myArray[i].ToString().Split("=".ToCharArray())[1];
						retXML += "</" + myArray[i].ToString().Split("=".ToCharArray())[0] + ">";

					}

					//resStr = myHash["res_Result_Code"].ToString() + Spliter + myHash["res_Overdue_Amt"].ToString() ;
					
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return resStr;
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";	
					//return retStr;
					if ( myHash["res_Result_Code"].ToString()!= errSuccess)
					{
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");// myHash["res_Err_Desc"].ToString());					
					}
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror  ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string FINANCIALPOSTING_REVERT(string CHANNELID,string INTERFACEID,string REVERT_REFNO, string REFNO)
		{
			string XMLNODE="FINANCIALPOSTING_REVERT";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Reversal_Ref_No"]= REVERT_REFNO;
				//myHash["req_Ref_No"]= REFNO;
                myHash["req_Ref_No"] = "*";
                myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					//string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					retXML += "<" +  XMLNODE +">";	
					retXML += "<HEADER>";

					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
						//Gen XML
						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";
					}
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";	

					//resStr = myHash["res_Result_Code"]  + Spliter + myHash["res_Ref_No"];
					
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					if  ( myHash["res_Result_Code"].ToString() != errSuccess)
					{
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");// myHash["res_Err_Desc"].ToString());
					}	
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string TXNREVERT(string CHANNELID,string INTERFACEID,string REVERT_REFNO, string REF_NO, string INT_REMARKS, string EXT_REMARKS)
		{
			string XMLNODE="TXNREVERT";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}
				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Txn_Ref_No"]= REVERT_REFNO;
				myHash["req_Ref_No"]= REF_NO;
				myHash["req_Int_Remarks"]= INT_REMARKS;
				myHash["req_Ext_Remarks"]= EXT_REMARKS;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					//string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();
					retXML += "<" +  XMLNODE +">";
					retXML += "<HEADER>";					
					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);

						//Gen XML
						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";
					}
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";	
					//resStr = myHash["res_Result_Code"]  + Spliter + myHash["res_Ref_No"];
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");

					if ( myHash["res_Result_Code"].ToString() != errSuccess)
					{
						//resStr = myHash["res_Result_Code"].ToString() + "|" + myHash["res_Err_Desc"].ToString();
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");//myHash["res_Err_Desc"].ToString());
					}					
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string SIBOOKING(string CHANNELID,string INTERFACEID,string ACCTNO,double AMOUNT,string CR_ACCTNO,string REMARK,string EFF_DATE,
			string EXP_DATE,string CHARGE_FLAG,double NO_OF_PAYMENTS,string FREQUENCY_TYPE, double FREQUENCY, string REFNO)
		{
			string XMLNODE="SIBOOKING";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Acc_No"]= ACCTNO;
				myHash["req_Cr_Acc_No"]= CR_ACCTNO;
				myHash["req_Eff_Dt"]= EFF_DATE;
				myHash["req_Exp_Dt"]= EXP_DATE;
				myHash["req_Amt"]= AMOUNT;
				myHash["req_Charge_Flag"]= CHARGE_FLAG;
				myHash["req_No_Of_Payments"]= NO_OF_PAYMENTS;
				myHash["req_Customer_Remark"]= REMARK;
				//myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				//KIEM TRA XEM ONE TIME HAY RECURRING?
				if (NO_OF_PAYMENTS>1)
				{
					myHash["req_No_Of_Payments"]=NO_OF_PAYMENTS;
					myHash["req_Frequency_Type"]= FREQUENCY_TYPE;
					myHash["req_Frequency"]= FREQUENCY;
				}

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{
					//REMOVE DUPLICATE KEY: res_Status
					retStr = Funcs.removeDuplicateKey( retStr, "res_Status");

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					//string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					retXML += "<" +  XMLNODE +">";	
					retXML += "<HEADER>";
					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
						//Gen XML
						retXML += "<" + myArray[i].ToString().Split("=".ToCharArray())[0] + ">";
						retXML += myArray[i].ToString().Split("=".ToCharArray())[1];
						retXML += "</" + myArray[i].ToString().Split("=".ToCharArray())[0] + ">";						
					}
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";	
					//resStr = myHash["res_Result_Code"]  + Spliter + myHash["res_Ref_No"];
					
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					//return resStr;
					if ( myHash["res_Result_Code"]!=null)
					{
						if ( myHash["res_Result_Code"].ToString() != errSuccess)
						{
							retXML =  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");
							Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
							Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
							return retXML;//myHash["res_Err_Desc"].ToString());
							
						}
					}
					else
					{
						if ( myHash["res_Status"].ToString()!= errSuccess)
						{	
							retXML =   Funcs.getXMLerror  ( XMLNODE, myHash["res_Status"].ToString(), "");
							Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
							Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
							return retXML;
						}
					}
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);				
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string SWBOOKING(string CHANNELID,string INTERFACEID,string ACCTNO,double AMOUNT, string CCYCD,  string SW_ACCTNO,string REMARK,string EFF_DATE,
			string REV_SWEEP_FLAG,double MIN_SLAB, double MAX_SLB, double SW_UNIT,string CHARGE_FLAG,double NO_OF_PAYMENTS,
			string FREQUENCY_TYPE, double FREQUENCY, string REFNO)
		{
			string XMLNODE="SWBOOKING";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Acc_No"]= ACCTNO;
				myHash["req_Sweep_Acc_No"]= SW_ACCTNO;
				myHash["req_Eff_Dt"]= EFF_DATE;
				myHash["req_Rev_Sweep_Flag"]= REV_SWEEP_FLAG;
				myHash["req_Charge_Flag"]= CHARGE_FLAG;
				myHash["req_No_Of_Payments"]= NO_OF_PAYMENTS;

				myHash["req_Min_Slab"]= MIN_SLAB;
				myHash["req_Max_Slab"]= MAX_SLB;
				myHash["req_Sweep_Unit"]= SW_UNIT;

				myHash["req_Customer_Remark"]= REMARK;
				//myHash["req_Ref_No"]= REFNO;
				myHash["req_Sweep_Amt"] = AMOUNT;
				myHash["req_CCY"] = CCYCD;
				
				//KIEM TRA XEM ONE TIME HAY RECURRING?
				if (NO_OF_PAYMENTS>1)
				{
					myHash["req_No_Of_Payments"]=NO_OF_PAYMENTS;
					myHash["req_Frequency_Type"]= FREQUENCY_TYPE;
					myHash["req_Frequency"]= FREQUENCY;
				}

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{
					//REMOVE DUPLICATE KEY - res_Status
					retStr = Funcs.removeDuplicateKey(retStr,  "res_Status");

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					////string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);

					retXML += "<" +  XMLNODE +">";	
					retXML += "<HEADER>";
					for (int i=0;i<myArray.Length-1;i++)
					{
						try
						{
							myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
							myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
							//Gen XML
							retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
							retXML += myArray[i].ToString().Split('=')[1];
							retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";						
						}
						catch
						{}
					}
					retXML += "</HEADER>" ;
					retXML += "</" +  XMLNODE +">";	
					//resStr = myHash["res_Result_Code"]  + Spliter + myHash["res_Ref_No"];
					
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return resStr;
					if ( myHash["res_Result_Code"].ToString() != errSuccess)
					{
						//resStr = myHash["res_Result_Code"].ToString() + "|" + myHash["res_Err_Desc"].ToString();
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");//myHash["res_Err_Desc"].ToString());
					}
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


		public string SO_DETAIL(string CHANNELID,string INTERFACEID, string ACCOUNT, string SI_REFNO,string REFNO)
		{
			string XMLNODE="SO_DETAIL";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Acc_No"]= ACCOUNT;
				myHash["req_SI_Ref_No"]= SI_REFNO;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{
					//LINHTN: REMOVE SOME SPECIAL CHARACTER
					retStr = retStr.Replace( "<","");
					retStr = retStr.Replace( ">","");

					retStr = Funcs.removeDuplicateKey( retStr, "hdr_Tran_Id");
					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					//string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					retXML += "<" + XMLNODE + ">";
					retXML += "<HEADER>";

					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
						//Gen XML
						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";
					}
					retXML += "</HEADER>";
					retXML += "</" + XMLNODE +">";
					//resStr = myHash["res_Result_Code"]  + Spliter + myHash["res_Acc_No"];
					
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return resStr;
					if ( myHash["res_Result_Code"].ToString() != errSuccess ) 
					{
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");// myHash["res_Err_Desc"].ToString());
					}
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return errGeneral;
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				//return errGeneral;
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}

		
		public string SO_SUMMARY(string CHANNELID,string INTERFACEID,string CIFNO,string REFNO)
		{
			string XMLNODE="SO_SUMMARY";
			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string [] myArray;
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();

				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_CIF_No"]= CIFNO;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{
					//REMOVE DUPLICATE KEY
					retStr = Funcs.removeDuplicateKey( retStr, "hdr_Tran_Id");
					//LINHTN: REMOVE SOME SPECIAL CHARACTER
					retStr = retStr.Replace( "<","");
					retStr = retStr.Replace( ">","");


					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					////string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();
					myArray = Regex.Split(retStr,Spliter_Regex);

					int idx;
					retXML += "<" + XMLNODE +">";
					retXML += "<HEADER>";

					for (idx=0;idx<myArray.Length-1;idx++)
					{
						myHash.Add(myArray[idx].ToString().Split('=')[0],myArray[idx].ToString().Split('=')[1]);
						myKeys.Add(myArray[idx].ToString().Split('=')[0]);
						//Gen XML
						retXML += "<" + myArray[idx].ToString().Split('=')[0] + ">";
						retXML += myArray[idx].ToString().Split('=')[1];
						retXML += "</" + myArray[idx].ToString().Split('=')[0] + ">";
						if (myArray[idx].ToString().Split('=')[0]=="res_No_Of_Seg") break;
					}
					retXML += "</HEADER>";
					
					if (myHash["res_Result_Code"].ToString()==errSuccess)
					{
						int numRec = Int32.Parse(myHash["res_No_Of_Seg"].ToString());

						myNode = myNode.SelectSingleNode("SEGMENT");

						idx++;
						retXML += "<SEGMENT>";	
						for (int i=0;i<numRec;i++)
						{
						
							myKeys = new ArrayList();
							myHash = new Hashtable();

							// Tao ra Message
							for (int j=0;j<myNode.ChildNodes.Count;j++)
							{
								myHash.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[j].SelectSingleNode("VAL").InnerText);
								myKeys.Add(myNode.ChildNodes[j].SelectSingleNode("NAME").InnerText);
							}

							// Gan gia tri
						
						
							myHash[myArray[idx].ToString().Split('=')[0]]=myArray[idx].ToString().Split('=')[1];
							idx++;
							retXML += "<res_Seg_No>";
							while ((myArray[idx].ToString().Split('=')[0]!="res_Seg_No")&&(idx<myArray.Length-1))
							{
								myHash[myArray[idx].ToString().Split('=')[0]]=myArray[idx].ToString().Split('=')[1];
								retXML += "<" + myArray[idx].ToString().Split('=')[0] + ">";
								retXML += myArray[idx].ToString().Split('=')[1];
								retXML += "</" + myArray[idx].ToString().Split('=')[0] + ">";
								idx++;
							}
							retXML += "</res_Seg_No>";
							//							// Bien thanh message
							//							for (int j=0;j<myKeys.Count;j++)
							//							{
							//								if (myHash[myKeys[j].ToString()].ToString().Length>0)
							//								{
							//									resStr += myHash[myKeys[j].ToString()] + Spliter;
							//								}
							//							}
							//
							//							resStr = resStr.Substring(0,resStr.Length-Spliter.Length) + "###";
						}
						retXML += "</SEGMENT>";
						retXML += "</" + XMLNODE + ">";
						//						resStr = errSuccess + "###" + resStr;
						Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
						Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
						Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
						return retXML;						
					}
					else
					{
						Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
						Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
						Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");//myHash["res_Err_Desc"].ToString());
					}
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				//return errGeneral;
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);

			}
		}

		
		public string SO_CANCELLATION(string CHANNELID,string INTERFACEID,string SI_REFNO,string REMARK,string REFNO)
		{
			string XMLNODE="SO_CANCELLATION";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_SI_Ref_No"]= SI_REFNO;
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Cancellation_Remark"]= REMARK;
				myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II

				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{
					//REMOVE DUPLICATE KEY- res_Status
					retStr = Funcs.removeDuplicateKey( retStr, "res_Status");
					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					////string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					retXML += "<SO_CANCELLATION>";
					retXML += "<HEADER>";
					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);

						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";

					}
					retXML += "</HEADER>";
					retXML += "</SO_CANCELLATION>";
					//resStr = myHash["res_Result_Code"]  + Spliter + myHash["res_Ref_No"];
					
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return resStr;
					if ( myHash["res_Result_Code"].ToString() != errSuccess)
					{
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");// myHash["res_Err_Desc"].ToString());
					}
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return errGeneral;
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				//return errGeneral;
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}
		

		public string FUNDTRANSFER_INTERBANK(string CHANNELID,string INTERFACEID,
			string SRC_ACCTNO, string DES_ACCTNO, double AMOUNT, string CCY, string REMARK, string VALUEDATE,
			string BEN_NAME, string BEN_ADD1, string BEN_ADD2, string BEN_CITY,
			string BEN_IDTYPE, string BEN_ID, string BEN_ID_ISSUEPLACE, string BEN_ID_EXPDATE,
			string BANK_CODE, string BR_CODE, string CITY_CODE, string BANK_DESC, string BR_DESC, string CITY_DESC,
			string REFNO)
		{
			string XMLNODE="FUNDTRANSFER_INTERBANK";

			Funcs.WriteLog_IFACE("====================== ENTER " + XMLNODE + " FUNC ============================");
			// Generate the message
			Hashtable myHash = new Hashtable();
			ArrayList myKeys = new ArrayList();
			string Spliter;
			string Spliter_Regex;
			XmlNode myNode;
			string retXML = string.Empty;
			try
			{
				Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");
				myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
				Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
				Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
				int numNode = myNode.ChildNodes.Count;
				for (int i=0;i< numNode;i++)
				{
					myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText,myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
					myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
				}

				myHash["req_Chnl_Id"]= CHANNELID;
				myHash["req_Interface_Id"]= INTERFACEID;
				myHash["req_Acc_No"] = SRC_ACCTNO;
				myHash["req_Credit_Acc_No"] = DES_ACCTNO;
				myHash["req_Amt"] = AMOUNT; 
				myHash["req_Txn_Ccy"]= CCY;
				myHash["req_Cust_Rmk"]= REMARK;
				
				myHash["req_Val_Dt"] = VALUEDATE;
				myHash["req_Ben_Name"] = BEN_NAME;
				myHash["req_Ben_Add1"] = BEN_ADD1;		
				myHash["req_Ben_Add2"] = BEN_ADD2;		
				myHash["req_Ben_City"] = BEN_CITY;
				myHash["req_Id_Type"] = BEN_IDTYPE;
				myHash["req_Id_No"]= BEN_ID;
				myHash["req_IdIssuePlace"] = BEN_ID_ISSUEPLACE;	 
				myHash["req_Id_Exp_Dt"] = BEN_ID_EXPDATE;
				myHash["req_Bank_Code"] = BANK_CODE;
				myHash["req_Br_Code"] = BR_CODE;
				myHash["req_Cty_Code"] = CITY_CODE;

				myHash["req_Bnk_Desc"] = BANK_DESC;
				myHash["req_Br_Desc"] = BR_DESC;
				myHash["req_Cty_Desc"] = CITY_DESC;
				
				myHash["req_Ref_No"]= REFNO;
				myHash["req_Txn_Dt"] = VALUEDATE;//DateTime.Now.ToString("yyyyMMdd");

				string retStr="";

				for (int i=0;i<myKeys.Count;i++)
				{
					if (myHash[myKeys[i].ToString()].ToString().Length>0)
					{
						retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
					}
				}
				Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
				// Send to II
//
//				Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
//				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Send the Outgoing Message Before Remove Stress. - [" + retStr + "]");
				retStr = Funcs.removeStress( retStr);
				Funcs.WriteLog_IFACE(" - Send the Outgoing Message After Remove Stress. - [" + retStr + "]");
				retStr = sendMsg(ServerIP,Port,retStr);
				Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
				// Process the message
				Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
				if (retStr.Length>0)
				{
					//REMOVE DUPLICATE res_Status
					retStr = Funcs.removeDuplicateKey( retStr, "res_Status");

					myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");

					////string resStr="";
					myKeys = new ArrayList();
					myHash = new Hashtable();

					string[] myArray = Regex.Split(retStr,Spliter_Regex);
					retXML += "<" +  XMLNODE +">";	
					retXML += "<HEADER>";
					for (int i=0;i<myArray.Length-1;i++)
					{
						myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0],myArray[i].ToString().Split("=".ToCharArray())[1]);
						myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
						
						//Gen XML
						retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
						retXML += myArray[i].ToString().Split('=')[1];
						retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";
					}
					retXML += "</HEADER>";
					retXML += "</" +  XMLNODE +">";	
					//resStr = myHash["res_Result_Code"]  + Spliter + myHash["res_Ref_No"];
					
					Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
					Funcs.WriteLog_IFACE(" - Return Msg to Client: ["+ retXML +"]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					//return resStr;
					if  ( myHash["res_Result_Code"].ToString() != errSuccess)
					{
						return  Funcs.getXMLerror ( XMLNODE, myHash["res_Result_Code"].ToString(), "");// myHash["res_Err_Desc"].ToString());
					}	
					return retXML;
				}
				else
				{
					Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
					Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
					return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
				}				
			}
			catch (Exception ex)
			{
				Funcs.WriteLog_IFACE(" * Error: + ["+ ex.Message +"]");
				Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
				return  Funcs.getXMLerror ( XMLNODE, errGeneral, errGeneralDesc);
			}
		}


        /*linhtn 20160719*/

        /// <summary>
        /// Hàm Tạo sổ tiết kiệm: chi tiết xem mapping document
        /// </summary>
        /// <param name="INTERFACEID"></param>
        /// <param name="CHANNELID"></param>
        /// <param name="REFNO"></param>
        /// <param name="ACCTNO"></param>
        /// <param name="CCY"></param>
        /// <param name="TENURE_UNIT"></param>
        /// <param name="TENURE"></param>
        /// <param name="AMOUNT"></param>
        /// <param name="PROD_CD"></param>
        /// <param name="DEP_TYPE"></param>
        /// <param name="PRIN_ON_MAT"></param>
        /// <param name="INT_ON_MAT"></param>
        /// <param name="ADDNL_FIELD"></param>
        /// <param name="REQ_POS_CODE"></param>
        /// <returns></returns>
        public string TIDEBOOKING(string INTERFACEID, string CHANNELID, string REFNO, string ACCTNO, string CCY,// string TIDE_NO, 
                                  string TENURE_UNIT, double TENURE, double AMOUNT, string PROD_CD, string DEP_TYPE, string PRIN_ON_MAT,
                                  string INT_ON_MAT, string ADDNL_FIELD, string POS_CODE, out string RES_CORE)
        {
            string XMLNODE = "TIDEBOOKING";

            Funcs.WriteLog("====================== ENTER " + XMLNODE + " FUNC ============================");
            // Generate the message
            Hashtable myHash = new Hashtable();
            ArrayList myKeys = new ArrayList();
            string Spliter;
            string Spliter_Regex;
            XmlNode myNode;
            string retXML = string.Empty;
            try
            {
                Funcs.WriteLog_IFACE(" - Start Generate the Outgoing Message. ");

                myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");

                Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
                Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
                int numNode = myNode.ChildNodes.Count;

                for (int i = 0; i < numNode; i++)
                {
                    myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText, myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
                    myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
                }

                myHash["req_Interface_Id"] = INTERFACEID;
                myHash["req_Chnl_Id"] = CHANNELID;
                myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");
                myHash["req_Ref_No"] = REFNO;
                myHash["req_Acc_No"] = ACCTNO;
                myHash["req_Ccy"] = CCY;
                myHash["req_Tenure_Unit"] = TENURE_UNIT;
                myHash["req_Tenure"] = TENURE;
                myHash["req_Amt"] = AMOUNT;
                myHash["req_Prod_Cd"] = PROD_CD;
                myHash["req_Dep_Type"] = DEP_TYPE;
                myHash["req_Prin_On_Mat"] = PRIN_ON_MAT;
                myHash["req_Int_On_Mat"] = INT_ON_MAT;
                myHash["req_Addnl_Field"] = ADDNL_FIELD;
                myHash["req_Pos_Code"] = POS_CODE;

                string retStr = "";
                for (int i = 0; i < myKeys.Count; i++)
                {
                    if (myHash[myKeys[i].ToString()].ToString().Length > 0)
                    {
                        retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
                    }
                }
                Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
                // Send to II

                Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
                retStr = sendMsg(ServerIP, Port, retStr);

                Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
                // Process the message
                Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
                if (retStr.Length > 0)
                {
                    myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");
                    ////string resStr="";
                    myKeys = new ArrayList();
                    myHash = new Hashtable();

                    string[] myArray = Regex.Split(retStr, Spliter_Regex);
                    retXML += "<" + XMLNODE + ">";
                    retXML += "<HEADER>";

                    for (int i = 0; i < myArray.Length - 1; i++)
                    {
                        myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0], myArray[i].ToString().Split("=".ToCharArray())[1]);
                        myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
                        //Gen XML
                        retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
                        retXML += myArray[i].ToString().Split('=')[1];
                        retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";
                    }
                    retXML += "</HEADER>";
                    retXML += "</" + XMLNODE + ">";

                    Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
                    Funcs.WriteLog_IFACE(" - Return Msg to Client: [" + retXML + "]");
                    Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");

                    RES_CORE = retXML;

                    if (myHash["res_Result_Code"].ToString() != errSuccess)
                    {
                        return Funcs.getXMLerror(XMLNODE, myHash["res_Result_Code"].ToString(), "");//, myHash["res_Err_Desc"].ToString());
                    }

                    return retXML;
                }
                else
                {
                    Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
                    Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
                    //return errGeneral;
                    RES_CORE = "";
                    return Funcs.getXMLerror(XMLNODE, errGeneral, errGeneralDesc);
                }
            }
            catch (Exception ex)
            {
                RES_CORE = "";
                Funcs.WriteLog_IFACE(" * Error: + [" + ex.Message + "]");
                Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
                //return errGeneral;
                return Funcs.getXMLerror(XMLNODE, errGeneral, errGeneralDesc);
            }
        }



        /// <summary>
        /// Hàm Tất toán sổ tiết kiệm: chi tiết xem mapping document
        /// </summary>
        /// <param name="INTERFACEID"></param>
        /// <param name="CHANNELID"></param>
        /// <param name="REFNO"></param>
        /// <param name="ACCTNO"></param>
        /// <param name="DEPOSITNO"></param>
        /// <param name="WDLTYPE"></param>
        /// <returns></returns>
        public string TIDEWDL(string INTERFACEID, string CHANNELID, string REFNO, string DES_ACCTNO,
            string DEPOSITNO, string WDLTYPE, out string RES_CORE)
        {
            string XMLNODE = "TIDEWDL";

            Funcs.WriteLog("====================== ENTER " + XMLNODE + " FUNC ============================");
            // Generate the message
            Hashtable myHash = new Hashtable();
            ArrayList myKeys = new ArrayList();
            string Spliter;
            string Spliter_Regex;
            XmlNode myNode;
            string retXML = string.Empty;
            try
            {
                Funcs.WriteLog(" - Start Generate the Outgoing Message. ");
                myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/REQUEST");
                Spliter = XmlDoc.SelectSingleNode("NVP/DELIMITER").InnerText.Trim();
                Spliter_Regex = XmlDoc.SelectSingleNode("NVP/DELIMITER_REGEX").InnerText.Trim();
                int numNode = myNode.ChildNodes.Count;

                for (int i = 0; i < numNode; i++)
                {
                    myHash.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText, myNode.ChildNodes[i].SelectSingleNode("VAL").InnerText);
                    myKeys.Add(myNode.ChildNodes[i].SelectSingleNode("NAME").InnerText);
                }

                myHash["req_Interface_Id"] = INTERFACEID;
                myHash["req_Chnl_Id"] = CHANNELID;
                myHash["req_Txn_Dt"] = DateTime.Now.ToString("yyyyMMdd");
                myHash["req_Ref_No"] = REFNO;
                myHash["req_Acc_No"] = DES_ACCTNO;
                myHash["req_Deposit_No"] = DEPOSITNO;
                myHash["req_Wdl_Type"] = WDLTYPE;


                string retStr = "";
                for (int i = 0; i < myKeys.Count; i++)
                {
                    if (myHash[myKeys[i].ToString()].ToString().Length > 0)
                    {
                        retStr += myKeys[i].ToString() + "=" + myHash[myKeys[i].ToString()] + Spliter;
                    }
                }
                Funcs.WriteLog_IFACE(" - Stop Generate the Outgoing Message. ");
                // Send to II

                Funcs.WriteLog_IFACE(" - Send the Outgoing Message. - [" + retStr + "]");
                retStr = sendMsg(ServerIP, Port, retStr);
                Funcs.WriteLog_IFACE(" - Receive the Incoming Message. - [" + retStr + "]");
                // Process the message
                Funcs.WriteLog_IFACE(" - Start Process the Incoming Message. ");
                if (retStr.Length > 0)
                {
                    myNode = XmlDoc.SelectSingleNode("NVP/" + XMLNODE + "/RESPONSE");
                    ////string resStr="";
                    myKeys = new ArrayList();
                    myHash = new Hashtable();

                    string[] myArray = Regex.Split(retStr, Spliter_Regex);
                    retXML += "<" + XMLNODE + ">";
                    retXML += "<HEADER>";

                    for (int i = 0; i < myArray.Length - 1; i++)
                    {
                        myHash.Add(myArray[i].ToString().Split("=".ToCharArray())[0], myArray[i].ToString().Split("=".ToCharArray())[1]);
                        myKeys.Add(myArray[i].ToString().Split("=".ToCharArray())[0]);
                        //Gen XML
                        retXML += "<" + myArray[i].ToString().Split('=')[0] + ">";
                        retXML += myArray[i].ToString().Split('=')[1];
                        retXML += "</" + myArray[i].ToString().Split('=')[0] + ">";
                    }
                    retXML += "</HEADER>";
                    retXML += "</" + XMLNODE + ">";

                    Funcs.WriteLog_IFACE(" - Stop Process the Incoming Message. ");
                    Funcs.WriteLog_IFACE(" - Return Msg to Client: [" + retXML + "]");
                    Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");

                    RES_CORE = retXML;

                    if (myHash["res_Result_Code"].ToString() != errSuccess)
                    {
                        return Funcs.getXMLerror(XMLNODE, myHash["res_Result_Code"].ToString(), "");//, myHash["res_Err_Desc"].ToString());
                    }
                    return retXML;
                }
                else
                {
                    RES_CORE = "";
                    Funcs.WriteLog_IFACE(" * Error: [Network error, can not receive response message.]");
                    Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
                    //return errGeneral;
                    return Funcs.getXMLerror(XMLNODE, errGeneral, errGeneralDesc);
                }
            }
            catch (Exception ex)
            {
                RES_CORE = "";
                Funcs.WriteLog_IFACE(" * Error: + [" + ex.Message + "]");
                Funcs.WriteLog_IFACE("====================== EXIT " + XMLNODE + " FUNC ============================");
                //return errGeneral;
                return Funcs.getXMLerror(XMLNODE, errGeneral, errGeneralDesc);
            }
        }

				
	
}
}
