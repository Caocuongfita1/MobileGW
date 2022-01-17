using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

/// <summary>
/// Summary description for Log4NetContext
/// </summary>
/// 
public class Log4NetContext
{
    private ILog log;
    public Log4NetContext()
    {
        log = log4net.LogManager.GetLogger(typeof(Log4NetContext));
    }

    public Log4NetContext(Type typeClass)
    {
        log = log4net.LogManager.GetLogger(typeClass);
    }

    //Co the tao them cac phuong thuc khac de ghi log tuy vao muc dich ghi

    public void WriteLog(String step, String type)
    {

    }

    public void Info(String input)
    {
        log.Info(input);
    }
    public void Debug(String input)
    {
        log.Debug(input);
    }
    public void Warn(String input)
    {
        log.Warn(input);
    }
    public void Error(String input)
    {
        log.Error(input);
    }
    public void Fatal(String input)
    {
        log.Fatal(input);
    }
}