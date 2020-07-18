using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBWTechniqueBackend.Model
{
    class ProxyModel
    {
        public int ProxyID { get; set; }
        public string ProxyName { get; set; }
        public string ProxyAddress { get; set; }
        public DateTime ProxyDateTime { get; set; }
        public bool ProxyStatus { get; set; }
    }
}
