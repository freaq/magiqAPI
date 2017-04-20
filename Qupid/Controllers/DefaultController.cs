using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Data;
using Qupid.Configuration;
using Qupid.Services;
using System.IO;

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
        public IActionResult GetByInt(int id)
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string sqlQuery = ControllerService.GetDefaultGetByIntQuery(route, id);

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

        [HttpGet]
        public IActionResult GetByString(string id)
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string sqlQuery = ControllerService.GetDefaultGetByStringQuery(route, id);

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
