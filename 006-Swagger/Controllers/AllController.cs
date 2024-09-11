using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _006_Swagger.Controllers
{
    [CXLSwaggerGroup("Order")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return Directory.GetCurrentDirectory();
        }
    }

    [CXLSwaggerGroup("User")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return AppContext.BaseDirectory;
        }
    }

    [CXLSwaggerGroup("Stock")]
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IWebHostEnvironment env;

        public StockController(IWebHostEnvironment env)
        {
            this.env = env;
        }

        [HttpGet]
        public string Get()
        {
            Console.WriteLine(env.ContentRootPath);
            return "GET";
        }
    }
}
