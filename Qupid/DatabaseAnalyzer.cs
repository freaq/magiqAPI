using Newtonsoft.Json;
using Qupid.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

            using (SqlConnection sqlConnection = new SqlConnection(configurationService.ApiConfiguration.ConnectionString))
            {
                sqlConnection.Open();

                DataTable tableSchemas = sqlConnection.GetSchema("Tables");
                foreach (DataRow row in tableSchemas.Rows)
                {
                    string schema = row[1].ToString();
                    string table = row[2].ToString();

                    string routeConfigurationFilePath = Path.Combine(configurationService.RoutesConfigurationDirectoryPath, table);

                    // only write the route configuration file if it does not yet exist
                    if (!File.Exists(routeConfigurationFilePath))
                    {
                        RouteConfiguration routeConfiguration = new RouteConfiguration()
                        {
                            Id = Guid.NewGuid(),
                            Name = table + "Route",
                            Resource = table,
                            Schema = schema,
                            Table = table
                        };
                        
                        string[] restrictionsColumns = new string[4];
                        restrictionsColumns[2] = table;
                        DataTable schemaColumns = sqlConnection.GetSchema("Columns", restrictionsColumns);

                        foreach (DataRow rowColumn in schemaColumns.Rows)
                        {
                            string columnName = rowColumn[3].ToString();
                            ColumnConfiguration column = new ColumnConfiguration()
                            {
                                ColumnName = columnName
                            };

                            routeConfiguration.Columns.Add(column);
                        }
                        
                        string json = JsonConvert.SerializeObject(routeConfiguration);
                        
                        File.WriteAllText(routeConfigurationFilePath, json);
                    }
                }


                sqlConnection.Close();
            }
        }
    }
}
