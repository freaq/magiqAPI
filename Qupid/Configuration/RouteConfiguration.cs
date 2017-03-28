using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qupid.Configuration
{
    public class RouteConfiguration
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public string Prefix { get; set; }

        public string Controller { get; set; }

        public string Resource { get; set; }

        public string Schema { get; set; }

        public string Table { get; set; }

        public readonly List<ColumnConfiguration> Columns = new List<ColumnConfiguration>();

        public string PrimaryKeyColumn { get; set; } = "Id";

        public string PrimaryKeyColumnDataType { get; set; }

        public readonly List<ActionConfiguration> Actions = new List<ActionConfiguration>();

        public RouteConfiguration()
        {

        }
    }
}
