using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qupid.Configuration
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

        private const string API_CONFIGURATION_FILE_PATH = "configuration/api.json";

        private const string ROUTES_CONFIGURATION_DIRECTORY_PATH = "configuration/routes";

        public string ServiceRootPath { get; private set; }

        public string ApiConfigurationFilePath { get; private set; }

        public string RoutesConfigurationDirectoryPath { get; private set; }

        public ApiConfiguration ApiConfiguration { get; private set; }

        public void LoadConfiguration(string serviceRootPath)
        {
            ServiceRootPath = serviceRootPath;
            ApiConfigurationFilePath = Path.Combine(ServiceRootPath, API_CONFIGURATION_FILE_PATH);
            RoutesConfigurationDirectoryPath = Path.Combine(ServiceRootPath, ROUTES_CONFIGURATION_DIRECTORY_PATH);

            LoadApiConfiguration();

            LoadRouteConfiguration();
        }

        private void LoadApiConfiguration()
        {
            using (FileStream apiConfigurationFileStream = new FileStream(ApiConfigurationFilePath, FileMode.Open))
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
            List<string> routeConfigurationFilePaths = Directory.GetFiles(RoutesConfigurationDirectoryPath).ToList();

            foreach (string routeConfigurationFilePath in routeConfigurationFilePaths)
            {
                using (FileStream fileStream = new FileStream(routeConfigurationFilePath, FileMode.Open))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        String json = streamReader.ReadToEnd();

                        RouteConfiguration route = JsonConvert.DeserializeObject<RouteConfiguration>(json);

                        foreach (ColumnConfiguration column in route.Columns)
                        {
                            // if no custom property name is configured
                            // then use the table column name as the api property name                            
                            if (String.IsNullOrEmpty(column.PropertyName))
                            {
                                column.PropertyName = column.ColumnName;
                            }
                        }

                        // add the custom route configuration to the list
                        ApiConfiguration.Routes.Add(route);
                    }
                }
            }
        }
    }
}
