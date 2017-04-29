using System.Linq;
using System.Data.SqlClient;
using System.Data;
using DbExtensions;
using Qupid.Configuration;
using System;

namespace Qupid.Services
{
    public class SqlServerService
    {
        public readonly string ConnectionString;

        public readonly RouteConfiguration Route;

        public SqlServerService(string connectionString, RouteConfiguration route)
        {
            ConnectionString = connectionString;
            Route = route;
        }

        public DataTable ExecuteResultQuery(string sqlQuery)
        {
            DataTable resultSetDataTable = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        // if route columns are configured
                        // then apply potential column mapping from database to api property
                        // else load the query results using the table column names
                        if (Route.Columns.Any())
                        {
                            foreach (ColumnConfiguration column in Route.Columns)
                            {
                                resultSetDataTable.Columns.Add(column.PropertyName);
                            }

                            while (sqlDataReader.Read())
                            {
                                DataRow row = resultSetDataTable.NewRow();
                                foreach (ColumnConfiguration column in Route.Columns)
                                {
                                    if (ContainsColumn(sqlDataReader, column.ColumnName))
                                    {
                                        row[column.PropertyName] = sqlDataReader.GetValueOrNull(column.ColumnName);
                                    }
                                }
                                resultSetDataTable.Rows.Add(row);
                            }
                        }
                        else
                        {
                            resultSetDataTable.Load(sqlDataReader);
                        }

                        sqlDataReader.Close();
                    }
                }

                sqlConnection.Close();
            }

            return resultSetDataTable;
        }

        public DataTable GenericExecuteResultQuery(string sqlQuery)
        {
            DataTable resultSetDataTable = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        resultSetDataTable.Load(sqlDataReader);

                        sqlDataReader.Close();
                    }
                }

                sqlConnection.Close();
            }

            return resultSetDataTable;
        }

        public void ExecuteNonQuery(string sqlQuery)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }
        }

        private bool ContainsColumn(SqlDataReader sqlDataReader, string columnName)
        {
            bool containsColumn = false;

            for (int i = 0; i < sqlDataReader.FieldCount; i++)
            {
                if (sqlDataReader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    containsColumn = true;
                    i = sqlDataReader.FieldCount;
                }
            }

            return containsColumn;
        }

        //public DataTable GetColumnSchema(string columnName)
        //{
        //    DataTable tableColumnSchema;

        //    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
        //    {
        //        sqlConnection.Open();

        //        string[] restrictionsColumns = new string[4];
        //        restrictionsColumns[2] = Route.Table;
        //        restrictionsColumns[3] = columnName;
        //        tableColumnSchema = sqlConnection.GetSchema("Columns", restrictionsColumns);

        //        sqlConnection.Close();
        //    }

        //    return tableColumnSchema;
        //}
    }
}
