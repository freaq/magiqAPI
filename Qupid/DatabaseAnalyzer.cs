using Newtonsoft.Json;
using Qupid.Configuration;
using Qupid.Services;
using System;
using System.IO;

namespace Qupid
{
    public class DatabaseAnalyzer
    {
        public DatabaseAnalyzer()
        {

        }

        public void ExtractConfigurationFromDatabase()
        {
            ConfigurationService configurationService = ConfigurationService.Instance;

            SqlServerSchema schema = new SqlServerSchema(configurationService.ApiConfiguration.ConnectionString);

            foreach (SqlServerTable table in schema.Tables)
            {
                string routeConfigurationFilePath = Path.Combine(configurationService.RoutesConfigurationDirectoryPath, table.Name);

                // only write the route configuration file if it does not yet exist
                if (!File.Exists(routeConfigurationFilePath))
                {
                    RouteConfiguration routeConfiguration = new RouteConfiguration()
                    {
                        Id = Guid.NewGuid(),
                        Name = table.Name + "Route",
                        Resource = table.Name,
                        Schema = table.Schema,
                        Table = table.Name
                    };

                    foreach (SqlServerColumn column in table.Columns)
                    {
                        ColumnConfiguration columnConfiguration = new ColumnConfiguration()
                        {
                            ColumnName = column.Name
                        };

                        routeConfiguration.Columns.Add(columnConfiguration);
                    }

                    string json = JsonConvert.SerializeObject(routeConfiguration);

                    File.WriteAllText(routeConfigurationFilePath, json);
                }
            }
        }
    }
}
