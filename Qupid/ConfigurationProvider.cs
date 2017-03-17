using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Qupid
{
    public class ConfigurationProvider
    {
        private const string DEFAULT_ROUTE_CONFIGURATION_NAME = "DefaultRoute";

        private readonly IHostingEnvironment _hostingEnvironment;

        public ApiConfiguration ApiConfiguration { get; private set; }

        public readonly List<RouteConfiguration> Routes = new List<RouteConfiguration>();

        public RouteConfiguration DefaultRoute { get; private set; }

        


        public ConfigurationProvider(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            LoadApiConfiguration();
        }

        private void LoadApiConfiguration()
        {
            LoadJson();
        }


        public void LoadJson()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;


            string apiConfigurationPath = Path.Combine(contentRootPath, "configuration/api.json");

            FileStream apiConfigurationFileStream = new FileStream(apiConfigurationPath, FileMode.Open);

            using (StreamReader streamReader = new StreamReader(apiConfigurationFileStream))
            {
                String json = streamReader.ReadToEnd();

                ApiConfiguration = JsonConvert.DeserializeObject<ApiConfiguration>(json);
            }



            string routeConfigurationsPath = Path.Combine(contentRootPath, "configuration/routes");

            List<string> routeConfigurationFilePaths = Directory.GetFiles(routeConfigurationsPath).ToList<string>();

            foreach (string routeConfigurationFilePath in routeConfigurationFilePaths)
            {
                FileStream fileStream = new FileStream(routeConfigurationFilePath, FileMode.Open);

                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    String json = streamReader.ReadToEnd();

                    RouteConfiguration routeConfiguration = JsonConvert.DeserializeObject<RouteConfiguration>(json);

                    // set the default route configuration
                    if (routeConfiguration.Name == DEFAULT_ROUTE_CONFIGURATION_NAME)
                    {
                        DefaultRoute = routeConfiguration;
                    }
                    else
                    {
                        // add the custom route configuration to the list
                        Routes.Add(routeConfiguration);
                    }
                }
            }
        }
    }
}
