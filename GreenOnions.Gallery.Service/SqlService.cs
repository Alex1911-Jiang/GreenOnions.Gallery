using GreenOnions.Gallery.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Service
{
    public class SqlService : ISqlService
    {
        private readonly string _ConnStr;
        public SqlService(IConfiguration configuration)
        {
            _ConnStr = configuration["ConnectionStrings:DbConnection"];
        }

        public async Task<DataSet> Select(string sql, params SqlParameter[] parameters)
        {
            using SqlConnection sqlConnection = new(_ConnStr);
            if (sqlConnection.State != ConnectionState.Open)
            {
                await sqlConnection.OpenAsync();
            }
            using SqlCommand sqlCommand = new(sql, sqlConnection);
            if (parameters != null && parameters.Length > 0)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            using SqlDataAdapter adapter = new();
            adapter.SelectCommand = sqlCommand;
            DataSet ds = new();
            adapter.Fill(ds);
            return ds;
        }

        public async Task<DataSet> SelectMultiTable(string sql, IDictionary<string,string> tableMappings, params SqlParameter[] parameters)
        {
            using SqlConnection sqlConnection = new(_ConnStr);
            if (sqlConnection.State != ConnectionState.Open)
            {
                await sqlConnection.OpenAsync();
            }
            using SqlCommand sqlCommand = new(sql, sqlConnection);
            if (parameters != null && parameters.Length > 0)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            using SqlDataAdapter adapter = new();
            foreach (KeyValuePair<string,string> tableMapping in tableMappings)
            {
                adapter.TableMappings.Add(tableMapping.Key, tableMapping.Value);
            }
            adapter.SelectCommand = sqlCommand;
            DataSet ds = new();
            adapter.Fill(ds);
            return ds;
        }

        public async Task<int> InsertUpdateDelete(string sql, params SqlParameter[] parameters)
        {
            using SqlConnection sqlConnection = new(_ConnStr);
            if (sqlConnection.State != ConnectionState.Open)
            {
                await sqlConnection.OpenAsync();
            }
            using SqlCommand sqlCommand = new(sql, sqlConnection);
            if (parameters != null && parameters.Length > 0)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            using SqlDataAdapter adapter = new();
            adapter.SelectCommand = sqlCommand;
            return adapter.SelectCommand.ExecuteNonQuery();
        }

        public async Task<long> InsertUpdateDeleteOutputIdentity(string sql, params SqlParameter[] parameters)
        {
            using SqlConnection sqlConnection = new(_ConnStr);
            if (sqlConnection.State != ConnectionState.Open)
            {
                await sqlConnection.OpenAsync();
            }
            using SqlCommand sqlCommand = new(sql + " SELECT @@IDENTITY", sqlConnection);
            if (parameters != null && parameters.Length > 0)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            using SqlDataAdapter adapter = new();
            adapter.SelectCommand = sqlCommand;
            DataSet ds = new();
            adapter.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt64( ds.Tables[0].Rows[0][0] );
                }
            }
            return -1;
        }
    }
}
