using _003_JWT;
using _004_JWT_Custom.Service.Authorization.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _004_JWT_Custom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public string GetToken()
        {
            return JwtHelper.IssueJwt(new TokenModelJwt()
            {
                Role = "AdminSSS",
                Uid = 666,
                Work = "打工仔"
            });
        }

        [HttpPost]
        [Authorize, CXLAuthPolicy("CXLPolicyName", "CustomValidationAge")]
        public string ValidateToken()
        {
            return "Success";
        }

    }
}
