using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBWTechniqueData.Models
{
    public class ProxyModel
    {
        public int ID { get; set; }
        public string IpPort { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Country { get; set; }
        public DateTime LastChecked { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public DateTime FoundDate { get; set; }
    }
}