using DRL.Library;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DRL.Model.DataBase
{
    public static class SqlDBHelper
    {
        static CommonHelper CommonHelper = new CommonHelper();

        private static int timeOut = 500;

        public static bool ExecuteNonQueryWithErrorHandling(
            string procedureName,
            ref List<SqlParameter> parameters,
            string connString,
            out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand(procedureName, conn))
                    {
                        command.CommandTimeout = timeOut;
                        command.CommandType = CommandType.StoredProcedure;

                        // Add existing parameters
                        if (parameters != null)
                            command.Parameters.AddRange(parameters.ToArray());

                        // Add OUTPUT parameters for error info (must match SP)
                        var errMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(errMessageParam);
                        // Execute
                        command.ExecuteNonQuery();

                        // Retrieve output values
                        errorMessage = errMessageParam.Value as string;
                        // If no error message, assume success
                        return string.IsNullOrEmpty(errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected .NET/SQL exceptions (e.g., connection failure)
                errorMessage = ex.Message;
                return false;
            }
        }

        public static int ExecuteNonQuery(string procedureName, ref List<SqlParameter> parameters, string connString)
        {
            try
            {
                int result = 0;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand(procedureName, conn))
                    {
                        command.CommandTimeout = timeOut;
                        command.CommandText = procedureName;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(parameters.ToArray());
                        DebugSqlStatement(command, procedureName);
                        result = command.ExecuteNonQuery();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public static async Task<IEnumerable<T>> ExecuteReaderAsync<T>(
            string procedureName,
            SqlParameter[] parameters,
            string connString,
            Func<DbDataReader, T> map)
        {
            var entities = new List<T>();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                using (SqlCommand command = new SqlCommand(procedureName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = timeOut; // Set appropriate timeout
                    command.Parameters.AddRange(parameters);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            entities.Add(map(reader));
                        }
                    }
                }
            }

            return entities;
        }

        public static IEnumerable<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map, string connString)
        {
            var entities = new List<T>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = timeOut;
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            entities.Add(map(result));
                        }
                    }
                }
            }
            return entities;
        }

        public async static Task<IEnumerable<T>> RawSqlQueryAsync<T>(string query, Func<DbDataReader, T> map, string connString)
        {
            var entities = new List<T>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = timeOut;
                    // Execute the query asynchronously with cancellation support
                    using (var result = await command.ExecuteReaderAsync()) // Pass the token here
                    {
                        while (await result.ReadAsync())  // Pass the token to support cancellation
                        {
                            entities.Add(map(result));
                        }
                    }
                }
            }
            return entities;
        }
        public static bool LogQuery(string commandStr, List<SqlParameter> paramList)
        {
            string ConnectionString = CommonHelper.GetDefaultConnectionString();

            bool result = false;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (SqlCommand command = new SqlCommand(commandStr, conn))
                {
                    command.Parameters.AddRange(paramList.ToArray());
                    int count = command.ExecuteNonQuery();
                    result = count > 0;
                }
            }
            return result;
        }

        public static ActionStatus CreateDatabase(string connectionString, string dataBaseName)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmdPrimary = conn.CreateCommand();
                    cmdPrimary.CommandTimeout = timeOut;
                    cmdPrimary.CommandText = string.Format("CREATE DATABASE [{0}]", dataBaseName);
                    cmdPrimary.CommandType = System.Data.CommandType.Text;
                    cmdPrimary.ExecuteNonQuery();
                    conn.Close();
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ex.InnerException != null ? ex.InnerException.Message : null;
            }
            return result;
        }

        public static ActionStatus JobStatus(string query, string connectionString)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmdPrimary = conn.CreateCommand();
                    cmdPrimary.CommandTimeout = timeOut;
                    cmdPrimary.CommandText = string.Format(query);
                    cmdPrimary.CommandType = System.Data.CommandType.Text;
                    cmdPrimary.ExecuteNonQuery();
                    conn.Close();
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ex.InnerException != null ? ex.InnerException.Message : null;
            }
            return result;
        }

        public static ActionStatus ExecuteDbScriptFromFile(DatabaseModel objDatabaseModel, string currentConnectionString)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                using (SqlConnection conn = new SqlConnection(currentConnectionString))
                {
                    string sqlquery = File.ReadAllText(Path.Combine(objDatabaseModel.BasePath, objDatabaseModel.SQLFileName));
                    string[] commands = sqlquery.Split(new[] { "GO\r\n", "GO ", "GO\t", "GO" }, StringSplitOptions.RemoveEmptyEntries);
                    conn.Open();
                    foreach (var c in commands)
                    {
                        var command = new SqlCommand(c, conn);
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.MessageDetail = ex.InnerException != null ? ex.InnerException.Message : null;
            }
            return result;
        }

        public static object ExecuteScalar(string procedureName, ref List<SqlParameter> parameters, string connString)
        {
            try
            {
                object result = null;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand(procedureName, conn))
                    {
                        command.CommandTimeout = timeOut;
                        command.CommandText = procedureName;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(parameters.ToArray());
                        result = command.ExecuteScalar();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static async Task<KendoGridDataResult<T>> SqlQueryWithPaginationAsync<T>(
            string connectionString, string procedureName,
            KendoGridRequest request, Func<DbDataReader, T> map, Func<string, string> param = null
            )
        {
            KendoGridDataResult<T> kendoGridDataResult = new KendoGridDataResult<T> { Data = new List<T>(), Total = 0 };

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Skip", request.Skip);
                    command.Parameters.AddWithValue("@Take", request.Take);
                    command.Parameters.AddWithValue("@SortColumn", request.SortColumn);
                    command.Parameters.AddWithValue("@SortDirection", request.SortDirection);

                    // Convert filters dictionary to SQL condition string
                    string filters = "";
                    if (request.Filters != null && request.Filters.Count > 0)
                    {
                        foreach (var filter in request.Filters)
                        {
                            if (!string.IsNullOrEmpty(filter.Value))
                            {
                                filters += $" {filter.Key} LIKE '%{filter.Value}%' AND";
                            }
                        }
                        filters = filters.TrimEnd("AND".ToCharArray());
                    }

                    if (param != null) filters = param(filters);

                    command.Parameters.AddWithValue("@Filters", string.IsNullOrEmpty(filters) ? DBNull.Value : (object)filters);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Read the paginated customers
                        while (await reader.ReadAsync())
                        {
                            kendoGridDataResult.Data.Add(map(reader));
                        }

                        // Move to the next result set (total count)
                        if (await reader.NextResultAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                kendoGridDataResult.Total = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                            }
                        }
                    }
                }
            }
            return kendoGridDataResult;
        }

        private static void DebugSqlStatement(SqlCommand command, string procedureName)
        {
            // DEBUG: Print a T-SQL script to execute the command in SSMS
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-- DEBUG SCRIPT FOR SQL SERVER MANAGEMENT STUDIO");
            sb.AppendLine("-- Copy and paste the following into SSMS to run manually.");
            sb.AppendLine("-- Ensure you are connected to the correct database.");
            sb.AppendLine();
            sb.AppendLine("DECLARE ");
            var declareLines = new List<string>();
            var execParams = new List<string>();
            int pSize = 4000;

            foreach (SqlParameter p in command.Parameters)
            {
                string paramName = p.ParameterName;
                string paramValue = p.Value == null || p.Value == DBNull.Value ? "NULL" : p.Value.ToString();
                SqlDbType sqlType = p.SqlDbType;
                if (p.Size > 0)
                {
                    pSize = p.Size;
                }
                // Determine a suitable declaration type (simplified, can be expanded)
                string declType;
                switch (sqlType)
                {
                    case SqlDbType.Int:
                        declType = "INT";
                        break;
                    case SqlDbType.BigInt:
                        declType = "BIGINT";
                        break;
                    case SqlDbType.VarChar:
                    case SqlDbType.NVarChar:
                        declType = $"NVARCHAR({pSize})";
                        break;
                    case SqlDbType.DateTime:
                        declType = "DATETIME";
                        break;
                    case SqlDbType.UniqueIdentifier:
                        declType = "UNIQUEIDENTIFIER";
                        break;
                    // Add more cases as needed for other types
                    default:
                        declType = "NVARCHAR(4000)"; // Default fallback
                        break;
                }

                if (p.Direction == ParameterDirection.Output)
                {
                    // For output params, just add to EXEC, don't initialize in DECLARE
                    execParams.Add($"{paramName} = {paramName} OUTPUT");
                }
                else
                {
                    // Format value based on type for safe insertion into SQL
                    string formattedValue = FormatSqlValue(paramValue, sqlType);
                    declareLines.Add($"    {paramName} {declType} = {formattedValue}");
                    execParams.Add(paramName);
                }
            }
            sb.AppendLine(string.Join(",\n", declareLines));
            sb.AppendLine();
            sb.AppendLine($"EXEC {procedureName} {string.Join(", ", execParams)};");
            sb.AppendLine();
            sb.AppendLine("-- Output Parameters:");
            foreach (SqlParameter p in command.Parameters)
            {
                if (p.Direction == ParameterDirection.Output)
                {
                    sb.AppendLine($"-- SELECT {p.ParameterName} AS '{p.ParameterName}'; -- To see output value after execution");
                }
            }
            sb.AppendLine("-- END DEBUG SCRIPT");

            Debug.WriteLine(sb.ToString());
        }

        private static string FormatSqlValue(string val, SqlDbType sqlType)
        {
            if (val == "NULL") return "NULL";

            switch (sqlType)
            {
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    // Wrap string/date types in single quotes, escaping internal quotes
                    return $"'{val.Replace("'", "''")}'";
                case SqlDbType.Bit:
                    // Convert boolean strings to 1 or 0
                    if (bool.TryParse(val, out bool boolVal))
                    {
                        return boolVal ? "1" : "0";
                    }
                    goto default; // If parsing fails, treat as default string
                default:
                    // Numbers and other types can be passed as-is
                    return val;
            }
        }
        public class DatabaseModel
        {
            public string BasePath { get; set; }
            public string SQLFileName { get; set; }
        }
    }
}
