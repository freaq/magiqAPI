using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qupid
{
    public class ConfigurationService
    {
        #region Singleton implementation
                
        private static readonly ConfigurationService instance = new ConfigurationService();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ConfigurationService()
        {
        }

        private ConfigurationService()
        {
        }

        public static ConfigurationService Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        
        private const string DEFAULT_ROUTE_CONFIGURATION_NAME = "DefaultRoute";

        private const string API_CONFIGURATION_FILE_PATH = "configuration/api.json";

        private const string ROUTES_CONFIGURATION_DIRECTORY_PATH = "configuration/routes";
        
        public string ServiceRootPath { get; private set; }

        public ApiConfiguration ApiConfiguration { get; private set; }

        public readonly List<RouteConfiguration> Routes = new List<RouteConfiguration>();

        public RouteConfiguration DefaultRoute { get; private set; }
        
                
        public void LoadConfiguration(string serviceRootPath)
        {
            ServiceRootPath = serviceRootPath;

            LoadApiConfiguration();

            LoadRouteConfiguration();
        }

        private void LoadApiConfiguration()
        {
            string apiConfigurationPath = Path.Combine(ServiceRootPath, API_CONFIGURATION_FILE_PATH);

            using (FileStream apiConfigurationFileStream = new FileStream(apiConfigurationPath, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(apiConfigurationFileStream))
                {
                    String json = streamReader.ReadToEnd();

                    ApiConfiguration = JsonConvert.DeserializeObject<ApiConfiguration>(json);
                }
            }
        }

        private void LoadRouteConfiguration()
        {
            string routeConfigurationsPath = Path.Combine(ServiceRootPath, ROUTES_CONFIGURATION_DIRECTORY_PATH);

            List<string> routeConfigurationFilePaths = Directory.GetFiles(routeConfigurationsPath).ToList<string>();

            foreach (string routeConfigurationFilePath in routeConfigurationFilePaths)
            {
                using (FileStream fileStream = new FileStream(routeConfigurationFilePath, FileMode.Open))
                {
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
}
