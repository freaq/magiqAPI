using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qupid.Configuration
{
    public class ApiConfiguration
    {
        public string ConnectionString { get; set; }

        public bool ExtractConfigurationFromDatabase { get; set; }

        public ApiConfiguration()
        {

        }
    }
}
