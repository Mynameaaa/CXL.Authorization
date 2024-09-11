using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _005_Controller
{
    [Route("cxlApp")]
    public class CXLApp : ICXLController
    {

        [HttpGet]
        [CXLRoute("myGet", ApiName = "Get方法名字")]
        public string Get()
        {
            return "Get";
        }


        [HttpPost]
        public string Post(string first, string last)
        {
            return first + "--" + last;
        }

    }
}
