using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _003_JWT.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class TestSwaggerController : ControllerBase
{

    [HttpGet]
    [Authorize]
    [SwaggerGroup(SwaggerGroupEnum.Development)]
    public string Get()
    {
        return "Get";
    }

    [HttpPost]
    [SwaggerGroup(SwaggerGroupEnum.Test)]
    public string Post()
    {
        return "Get";
    }

    [HttpDelete]
    [SwaggerGroup(SwaggerGroupEnum.Pre)]
    public string Delete()
    {
        return "Get";
    }

    [HttpPut]
    [SwaggerGroup(SwaggerGroupEnum.Production)]
    public string Put()
    {
        return "Put";
    }

    [HttpPatch]
    [SwaggerGroup(SwaggerGroupEnum.Production)]
    public string Patch()
    {
        return "Patch";
    }

    [HttpOptions]
    [SwaggerGroup(SwaggerGroupEnum.Development)]
    public string Options()
    {
        return "Options";
    }

    [HttpGet]
    public string GETETETETETETETTETETETET()
    {
        return "GETETETETETETETTETETETET";
    }

}
