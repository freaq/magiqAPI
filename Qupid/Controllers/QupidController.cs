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

namespace Qupid.Controllers
{
    [Produces("application/json")]
    public class QupidController : Controller
    {
        [HttpGet]
        public IQueryable<string> GetAll()
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;


            //DataContext dataContext = new DataContext(apiConfiguration.ConnectionString);


            using (SqlConnection sqlConnection = new SqlConnection(apiConfiguration.ConnectionString))
            {
                string from = "";
                if (!String.IsNullOrEmpty(route.Schema))
                {
                    from += route.Schema + ".";
                }

                from += route.Table;


                string sql = "use " + apiConfiguration.DatabaseName + " "
                    + "select * from " + from;



                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(sql, sqlConnection);

                List<string> lastNames = new List<string>();

                DataTable dt = new DataTable();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string lastName = dr.GetString(6);

                    lastNames.Add(lastName);
                }

                return lastNames.AsQueryable();
            }

                


            //return new string[] { route.Id.ToString(), route.Name, route.Prefix, route.Table };
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
