using System.Collections.Generic;

namespace Qupid.Configuration
{
    public class ApiConfiguration
    {
        public string ConnectionString { get; set; }

        public bool ExtractConfigurationFromDatabase { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public RouteConfiguration DefaultRoute { get; set; }

        public List<RouteConfiguration> Routes = new List<RouteConfiguration>();

        public ApiConfiguration()
        {

        }
    }
}
