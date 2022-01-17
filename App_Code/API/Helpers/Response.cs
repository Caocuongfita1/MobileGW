using System.Collections.Generic;
using System.Net;

namespace mobileGW.Service.API
{

    public class Response<T> where T : class
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string TotalCount { get; set; }
        public string TotalPage { get; set; }
        public string TotalRecord { get; set; }
    }
    public class ResponseList<T> where T : class
    {
        public List<T> Data { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string TotalCount { get; set; }
        public string TotalPage { get; set; }
        public string TotalRecord { get; set; }
    }
    public class OracleCursor
    {
        public string STATUS_CODE { get; set; }
        public decimal ID { get; set; }
        public string NAME { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
    public class DeleteResponseModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class ResponseListNew<T> where T : class
    {
        public string status { get; set; }
        public string error { get; set; }
        public StatusDetail statusDetail { get; set; }
        public string jwtToken { get; set; }
        public string expireTime { get; set; }
        public List<T> listAccountNices { get; set; }
        public string timestamp { get; set; }
        public string exception { get; set; }
        public string path { get; set; }
        public string accountNo { get; set; }
        public string refNo { get; set; }

    }

    public class StatusDetail
    {
        public string respCode { get; set; }
        public string respDescEn { get; set; }
        public string respDescVn { get; set; }
    }

    public class BaseResponse
    {
        public string error { get; set; }
        public string status { get; set; }
        public StatusDetail statusDetail { get; set; }
    }
}