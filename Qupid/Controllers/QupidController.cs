using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Data.SqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using DbExtensions;
using Qupid.Configuration;

namespace Qupid.Controllers
{
    [Produces("application/json")]
    public class QupidController : Controller
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                // construct the SQL query
                SqlBuilder query = new SqlBuilder();

                if (route.Columns.Any())
                {
                    foreach (ColumnConfiguration column in route.Columns)
                    {
                        query.SELECT(column.ColumnName);
                    }
                }
                else
                {
                    query.SELECT("*");
                }

                string from = "";
                if (!String.IsNullOrEmpty(route.Schema))
                {
                    from += route.Schema + ".";
                }

                from += route.Table;

                query.FROM(from);




                string sql = query.ToString();



                // run the SQL query
                DataTable resultSetDataTable = new DataTable();

                using (SqlConnection sqlConnection = new SqlConnection(apiConfiguration.ConnectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (route.Columns.Any())
                            {
                                foreach (ColumnConfiguration column in route.Columns)
                                {
                                    if (string.IsNullOrEmpty(column.PropertyName))
                                    {
                                        column.PropertyName = column.ColumnName;
                                    }

                                    resultSetDataTable.Columns.Add(column.PropertyName);
                                }

                                while (sqlDataReader.Read())
                                {
                                    DataRow row = resultSetDataTable.NewRow();
                                    foreach (ColumnConfiguration column in route.Columns)
                                    {
                                        row[column.PropertyName] = sqlDataReader.GetValue(column.ColumnName);
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

                return new OkObjectResult(resultSetDataTable);
            }
            else
            {
                Response.StatusCode = 501;
                return new ObjectResult(null);
            }

        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                // construct the SQL query
                SqlBuilder query = new SqlBuilder();

                if (route.Columns.Any())
                {                    
                    foreach (ColumnConfiguration column in route.Columns)
                    {                        
                        query.SELECT(column.ColumnName);
                    }
                }
                else
                {
                    query.SELECT("*");
                }

                string from = "";
                if (!String.IsNullOrEmpty(route.Schema))
                {
                    from += route.Schema + ".";
                }

                from += route.Table;

                query.FROM(from);

                using (SqlConnection sqlConnection = new SqlConnection(apiConfiguration.ConnectionString))
                {
                    sqlConnection.Open();

                    string[] restrictionsColumns = new string[4];
                    restrictionsColumns[2] = route.Table;
                    restrictionsColumns[3] = route.PrimaryKeyColumn;
                    DataTable schemaColumns = sqlConnection.GetSchema("Columns", restrictionsColumns);

                    string columnDataType = schemaColumns.Rows[0]["DATA_TYPE"].ToString();

                    if (columnDataType == "int")
                    {
                        query.WHERE(route.PrimaryKeyColumn + " = " + id);
                    }
                    else if (columnDataType == "nvarchar")
                    {
                        query.WHERE(route.PrimaryKeyColumn + " = '" + id + "'");
                    }

                    sqlConnection.Close();
                }

                    




                string sql = query.ToString();



                // run the SQL query
                DataTable resultSetDataTable = new DataTable();

                using (SqlConnection sqlConnection = new SqlConnection(apiConfiguration.ConnectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (route.Columns.Any())
                            {
                                foreach (ColumnConfiguration column in route.Columns)
                                {
                                    if (string.IsNullOrEmpty(column.PropertyName))
                                    {
                                        column.PropertyName = column.ColumnName;
                                    }

                                    resultSetDataTable.Columns.Add(column.PropertyName);
                                }

                                if (sqlDataReader.HasRows)
                                {
                                    while (sqlDataReader.Read())
                                    {
                                        DataRow row = resultSetDataTable.NewRow();
                                        foreach (ColumnConfiguration column in route.Columns)
                                        {
                                            row[column.PropertyName] = sqlDataReader.GetValue(column.ColumnName);
                                        }
                                        resultSetDataTable.Rows.Add(row);
                                    }
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

                if (resultSetDataTable.Rows.Count > 0)
                {
                    return new OkObjectResult(resultSetDataTable.Rows[0]);
                }
                else
                {                    
                    return new NotFoundObjectResult(null);
                }
            }
            else
            {
                Response.StatusCode = 501;
                return new ObjectResult(null);
            }
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}
