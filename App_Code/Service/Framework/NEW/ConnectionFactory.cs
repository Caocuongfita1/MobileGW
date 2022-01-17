using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;

/// <summary>
/// Summary description for ConnectionFactory
/// </summary>
public class ConnectionFactory
{
    private readonly string _connectionString;
    //private static log4net.ILog l4NC = log4net.LogManager.GetLogger(typeof(ConnectionFactory));

    public ConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Get open oracle connection
    /// </summary>
    /// <returns></returns>
    private OracleConnection GetOpenConnection()
    {
        var connection = new OracleConnection(_connectionString);
        connection.Open();

        return connection;
    }

    /// <summary>
    /// Get query item using dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_connection"></param>
    /// <param name="commandType"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public IEnumerable<T> ExecuteData<T>(CommandType commandType, string sql, object parameters = null)
    {
        using (var connection = GetOpenConnection())
        {
            var result = connection.Query<T>(sql, parameters, commandType: commandType);
            connection.Dispose();
            connection.Close();

            return result;
        }
    }

    public IEnumerable<T> GetItems<T>(CommandType commandType, string sql, object parameters = null)
    {
        using (var connection = GetOpenConnection())
        {
            var result = connection.Query<T>(sql, parameters, commandType: commandType).ToList();
            connection.Dispose();
            connection.Close();

            return result;
        }

    }
}