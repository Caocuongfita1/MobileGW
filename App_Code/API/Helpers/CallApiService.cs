using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using System;
using mobileGW.Service.Framework;

namespace mobileGW.Service.API
{
    public static class ApiAcctNice
    {
        public static string URL_CHECK_LOGIN = "/api/ebanking/ebankuser/check_ebank_login";
        public static string URL_GET_ACCT_NICE_LIST = "/api/corebanking/get_account_nice_list";
        public static string URL_CREATE_ACCT = "/api/corebanking/create_account";
    }

    public static class ApiPushNotification
    {
        public static string URL_PUSH = "/api/ccomms/immediate";
        public static string URL_HISTORY_COUNT_UNREAD = "/api/ccomms/history/count/unread?userId={userId}&appSrc={appSrc}";
        public static string URL_HISTORY_SET_READ = "/api/ccomms/history/setread?id={id}&userId={userId}";
        public static string URL_HISTORY_GET_LIST = "/api/ccomms/history?pageSize={pageSize}&pageIndex={pageIndex}&appSrc={appSrc}&refId={refId}";
        public static string URL_HISTORY_VIEW = "/api/ccomms/history/{id}";
    }

    public static class ApiSaving
    {
        public static string URL_SAVING_MENU = "/api/ebanking/ebankuser/getSavingPackage";
    }

    public class CallApiService<T> where T : class
    {
        static RestClient _client;
        static RestRequest _request;
        public CallApiService(string token,string _userName)
        {
            var permissionToken = "";
            var userName = _userName;
            _request = new RestRequest();
            _request.AddHeader("cache-control", "no-cache");
            _request.AddHeader("X-UserName", userName);
            _request.AddHeader("X-PermissionToken", permissionToken);
            _request.AddHeader("Authorization", "Bearer " + token);
            _request.AddHeader("Content-Type", "application/json");
        }
        public Response<T> GetAPI(string baseUrl, string apiUri, int timeOut = 0)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new Response<T>();
                _request.Resource = apiUri;
                _request.Method = Method.GET;

                if (timeOut != 0)
                {
                    _request.Timeout = timeOut;
                }
                else
                {
                    _request.Timeout = Config.TIMEOUT_WITH_API;
                }

                IRestResponse response = _client.Execute(_request);

                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<Response<T>>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                string strMess = ex.Message;
                Funcs.WriteLog("CALL API RES: " + ex.ToString());
                return null;
            }
        }
        public ResponseList<T> GetListAPI(string baseUrl, string apiUri)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new ResponseList<T>();
                _request.Resource = apiUri;
                _request.Method = Method.GET;
                _request.Timeout = Config.TIMEOUT_WITH_API;

                IRestResponse response = _client.Execute(_request);

                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<ResponseList<T>>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("GetVoucherListAPIs (GetListAPI) RES: ERROR - " + ex);
                return null;
            }
        }
        public Response<T> PostAPI(string baseUrl, string apiUri, object data, string UserName)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new Response<T>();
                _request.Resource = apiUri;
                _request.Method = Method.POST;
                _request.Timeout = Config.TIMEOUT_WITH_API;
                //_request.AddHeader("X-UserName", UserName);
                Funcs.WriteLog("CALL API REQ: " + JsonConvert.SerializeObject(data));

                _request.AddParameter("undefined", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
                IRestResponse response = _client.Execute(_request);

                
                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<Response<T>>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                string strMess = ex.Message;
                return null;
            }
        }
        public Response<ResponseModelOracle> PutAPI(string baseUrl, string apiUri, object data)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new Response<ResponseModelOracle>();
                _request.Resource = apiUri;
                _request.Method = Method.PUT;
                _request.Timeout = Config.TIMEOUT_WITH_API;
                //_request.AddHeader("X-UserName", UserName);
                Funcs.WriteLog("CALL API REQ: " + JsonConvert.SerializeObject(data));

                _request.AddParameter("undefined", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
                IRestResponse response = _client.Execute(_request);

                
                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<Response<ResponseModelOracle>>(response.Content);
                return result;
            }
            catch
            {
                return null;
            }
        }
        public Response<OracleCursor> DeleteOneAPI(string baseUrl, string apiUri, string UserName)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new Response<OracleCursor>();
                _request.Resource = apiUri;
                _request.Method = Method.DELETE;
                _request.Timeout = Config.TIMEOUT_WITH_API;
                //_request.AddHeader("X-UserName", UserName);
                IRestResponse response = _client.Execute(_request);

                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<Response<OracleCursor>>(response.Content);
                return result;
            }
            catch
            {
                return null;
            }
        }
        public Response<List<DeleteResponseModel>> DeleteManyAPI(string baseUrl, string apiUri, object data, string UserName)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new Response<List<DeleteResponseModel>>();
                _request.Resource = apiUri;
                _request.Method = Method.DELETE;
                _request.Timeout = Config.TIMEOUT_WITH_API;
                //_request.AddHeader("X-UserName", UserName);

                Funcs.WriteLog("CALL API REQ: " + JsonConvert.SerializeObject(data));

                _request.AddParameter("undefined", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
                IRestResponse response = _client.Execute(_request);

                
                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<Response<List<DeleteResponseModel>>>(response.Content);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public ResponseListNew<T> PostAPIList(string baseUrl, string apiUri, object data, string key = null)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new ResponseListNew<T>();
                _request.Resource = apiUri;
                _request.Method = Method.POST;
                _request.Timeout = Config.TIMEOUT_WITH_API;

                if (!String.IsNullOrEmpty(key))
                {
                    _request.AddHeader("x-api-key", "Bearer " + key);
                }

                Funcs.WriteLog("CALL API REQ: " + JsonConvert.SerializeObject(data));

                _request.AddParameter("undefined", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
                IRestResponse response = _client.Execute(_request);

                
                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<ResponseListNew<T>>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("PostAPIList (PostAPIList) RES: ERROR - " + ex);
                return null;
            }
        }

        public Response<T> PostAPINew(string baseUrl, string apiUri, object data, string key = null)
        {
            try
            {
                _client = new RestClient(baseUrl);

                var result = new Response<T>();
                _request.Resource = apiUri;
                _request.Method = Method.POST;
                _request.Timeout = Config.TIMEOUT_WITH_API;

                if (!String.IsNullOrEmpty(key))
                {
                    _request.AddHeader("x-api-key", "Bearer " + key);
                }

                Funcs.WriteLog("CALL API REQ: " + JsonConvert.SerializeObject(data));

                _request.AddParameter("undefined", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
                IRestResponse response = _client.Execute(_request);

                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<Response<T>>(response.Content);
                    return result;
                } 
                else
                {
                    result.StatusCode = StatusCode.Fail;
                    return result;
                }
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("EXCEPTION CALL SEND: " + ex.ToString());
                return null;
            }
        }

        public T PostAPIClassT(string baseUrl, string apiUri, object data, string key = null)
        {
            try
            {
                T result = null;

                _client = new RestClient(baseUrl);
                _request.Resource = apiUri;
                _request.Method = Method.POST;
                _request.Timeout = Config.TIMEOUT_WITH_API;

                if (!String.IsNullOrEmpty(key))
                {
                    _request.AddHeader("x-api-key", "Bearer " + key);
                }

                Funcs.WriteLog("CALL API REQ: " + JsonConvert.SerializeObject(data));

                _request.AddParameter("undefined", JsonConvert.SerializeObject(data), ParameterType.RequestBody);

                IRestResponse response = _client.Execute(_request);

                Funcs.WriteLog("CALL API RES: " + JsonConvert.SerializeObject(response));

                if (response.StatusCode == HttpStatusCode.OK) result = JsonConvert.DeserializeObject<T>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                Funcs.WriteLog("EXCEPTION CALL SEND: " + ex.ToString());
                return null;
            }
        }
    }
}