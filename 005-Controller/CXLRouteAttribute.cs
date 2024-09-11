using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _005_Controller
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CXLRouteAttribute : Attribute
    {
        public string Route { get; set; }

        public string ApiName { get; set; }

        public CXLRouteAttribute(string route)
        {
            Route = route;
        }
    }
}
