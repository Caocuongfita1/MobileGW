using System;
using System.Collections.Generic;

namespace mobileGW.Service.API
{
    #region Response

    /// <summary>
    /// Đối tượng phản hồi
    /// </summary>
    public class Response
    {
        public Response(StatusCode code, string message)
        {
            StatusCode = code;
            Message = message;
        }
        public Response(string message)
        {
            Message = message;
        }
        public Response()
        {
        }
        public StatusCode StatusCode { get; set; } //= StatusCode.Success;
        public string Message { get; set; } //= "Thành công";
        public decimal TotalCount { get; set; }// = 0;
        public decimal TotalPage { get; set; } //= 0;
    }

    /// <summary>
    /// Phản hồi lỗi
    /// </summary>
    public class ResponseError : Response
    {
        public ResponseError(StatusCode code, string message, IList<Dictionary<string, string>> errorDetail = null) : base(
            code,
            message)
        {
            ErrorDetail = errorDetail;
        }
        public IList<Dictionary<string, string>> ErrorDetail { get; set; }
    }

    /// <summary>
    /// Phản hồi dạng đối tượng
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseObject<T> : Response
    {
        public ResponseObject(T data)
        {
            Data = data;
        }
        public ResponseObject(T data, decimal totalCount, decimal totalPage)
        {
            Data = data;
            TotalCount = totalCount;
            TotalPage = totalPage;
        }
        public ResponseObject(T data, string message)
        {
            Data = data;
            Message = message;
        }
        public ResponseObject(T data, string message, StatusCode code)
        {
            StatusCode = code;
            Data = data;
            Message = message;
        }
        public ResponseObject(T data, string message, StatusCode code, decimal totalCount, decimal totalPage)
        {
            StatusCode = code;
            Data = data;
            Message = message;
            TotalPage = totalPage;
            TotalCount = totalCount;
        }
        public T Data { get; set; }
    }
    #endregion


    /// <summary>
    /// Đối tượng mã trả về
    /// </summary>
    /// 
    public class StatusCode
    {
        public const int Success = 0;
        public const int FailRole = -1;
        public const int Fail = 99;
        public const int FailSession = -99;

        public const int FailApp = -98;
        public const int FailFunction = -97;
        public const int FailPermisstions = -96;
    }
    //public enum StatusCode
    //{
    //    Success = 0,
    //    FailRole = -1,
    //    Fail = 99,
    //    FailSession = -99,
    //    FailApp = -98,
    //    FailFunction = -97,
    //    FailPermisstions = -96
    //}

    /// <summary>
    /// Phản hồi kết quả xóa dữ liệu
    /// </summary>
    public class ResponseDelete : Response
    {
        public ResponseDelete(decimal id, string name)
        {
            Data = new ResponseDeleteModel { Id = id, Name = name };
        }
        public ResponseDelete(StatusCode code, string message, decimal id, string name) : base(code, message)
        {
            Data = new ResponseDeleteModel { Id = id, Name = name };
        }
        public ResponseDelete()
        {
        }
        public ResponseDeleteModel Data { get; set; }
    }

    /// <summary>
    /// Phản hồi kết quả xóa nhiều dữ liệu
    /// </summary>
    public class ResponseDeleteMulti : Response
    {
        public ResponseDeleteMulti(IList<ResponseDelete> data)
        {
            Data = data;
        }
        public ResponseDeleteMulti()
        {
        }
        public IList<ResponseDelete> Data { get; set; }
    }

    /// <summary>
    /// Đối tượng kết quả xóa
    /// </summary>
    public class ResponseDeleteModel
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
    }

    #region ORACLE
    public class ResponseOracle
    {
        public string STATUS_CODE { get; set; }
        public decimal ID { get; set; }
        public string NAME { get; set; }
    }
    #endregion
}