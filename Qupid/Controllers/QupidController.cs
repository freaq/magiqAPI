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

            // construct the SQL query
            string from = "";
            if (!String.IsNullOrEmpty(route.Schema))
            {
                from += route.Schema + ".";
            }

            from += route.Table;

            string databaseUseStatement = "USE " + apiConfiguration.DatabaseName + Environment.NewLine + Environment.NewLine;

            SqlBuilder query = new SqlBuilder(databaseUseStatement).SELECT("*").FROM(from);

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
                        resultSetDataTable.Load(sqlDataReader);

                        sqlDataReader.Close();
                    }
                }

                sqlConnection.Close();
            }



            return new OkObjectResult(resultSetDataTable);
        }

        [HttpGet]
        public string Get(int id)
        {
            return "value";
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
