using System;

namespace Qupid.Configuration
{
    public class ActionConfiguration
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public string Template { get; set; }

        public string Query { get; set; }

        public ActionConfiguration()
        {

        }
    }
}
