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
                string sqlQuery = ControllerService.GetStringReplacedQuery(Request, route, action);

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
            return Get(id);
        }

        [HttpGet]
        public IActionResult GetByString(string id)
        {
            return Get(id);
        }

        private IActionResult Get(object id)
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string sqlQuery = ControllerService.GetStringReplacedQuery(Request, route, action, id);

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

                string sqlQuery = ControllerService.GetStringReplacedQuery(Request, route, action, json);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                sqlServerService.ExecuteNonQuery(sqlQuery);
            }
        }

        [HttpPut]
        public void PutByInt(int id)
        {
            Put(id);
        }

        [HttpPut]
        public void PutByString(string id)
        {
            Put(id);
        }

        private void Put(object id)
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

                string sqlQuery = ControllerService.GetStringReplacedQuery(Request, route, action, id, json);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                sqlServerService.ExecuteNonQuery(sqlQuery);
            }
        }

        [HttpDelete]
        public void DeleteByInt(int id)
        {
            Delete(id);
        }

        [HttpDelete]
        public void DeleteByString(string id)
        {
            Delete(id);
        }

        private void Delete(object id)
        {
            ApiConfiguration apiConfiguration = RouteData.DataTokens["apiConfiguration"] as ApiConfiguration;
            RouteConfiguration route = RouteData.DataTokens["routeConfiguration"] as RouteConfiguration;
            ActionConfiguration action = RouteData.DataTokens["actionConfiguration"] as ActionConfiguration;

            if (route.Enabled && action.Enabled)
            {
                string sqlQuery = ControllerService.GetStringReplacedQuery(Request, route, action, id);

                SqlServerService sqlServerService = new SqlServerService(apiConfiguration.ConnectionString, route);

                sqlServerService.ExecuteNonQuery(sqlQuery);
            }
        }
    }
}
