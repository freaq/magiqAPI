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
using Qupid.Services;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace Qupid.Controllers
{
    [Produces("application/json")]
    public class DefaultController : Controller
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string sqlQuery = ControllerService.GetDefaultGetAllQuery(route);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                DataTable resultSetDataTable = sqlServerService.ExecuteResultQuery(sqlQuery);

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
                string sqlQuery = ControllerService.GetDefaultGetQuery(route, id);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                DataTable resultSetDataTable = sqlServerService.ExecuteResultQuery(sqlQuery);

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
        public void Post()
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string json;
                using (StreamReader streamReader = new StreamReader(Request.Body))
                {
                    json = streamReader.ReadToEnd();
                }

                string sqlQuery = ControllerService.GetDefaultPostQuery(route, json);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                sqlServerService.ExecuteNonQuery(sqlQuery);
            }
        }

        [HttpPut]
        public void Put(int id)
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string json;
                using (StreamReader streamReader = new StreamReader(Request.Body))
                {
                    json = streamReader.ReadToEnd();
                }

                string sqlQuery = ControllerService.GetDefaultPutQuery(route, id, json);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                sqlServerService.ExecuteNonQuery(sqlQuery);
            }
        }

        [HttpDelete]
        public void Delete(int id)
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string sqlQuery = ControllerService.GetDefaultDeleteQuery(route, id);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                sqlServerService.ExecuteNonQuery(sqlQuery);
            }
        }
    }
}
