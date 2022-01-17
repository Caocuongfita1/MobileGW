using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ResponseModel
/// </summary>
public class ResponseModel
{
    public string STATUS_CODE { set; get; } 
}

public class ResponseModelOracle
{
    public decimal Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}

public class PushDeviceModel
{
    public string COMM_TYPE { set; get; }
    public string CUSTID { set; get; }
    public string DEVICE_TOKEN { set; get; }
}