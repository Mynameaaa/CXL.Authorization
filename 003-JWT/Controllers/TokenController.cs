//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace _003_JWT.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class TokenController : ControllerBase
//    {

//        [HttpGet]
//        [AllowAnonymous]
//        public string GetToken()
//        {
//            var Token = JwtHelper.IssueJwt(new TokenModelJwt()
//            {
//                Role = "Admin",
//                Uid = 1212,
//                Work = "打工仔"
//            });
//            return Token;
//        }

//        [Authorize]
//        [HttpPost]
//        public bool ValidateToken()
//        {
//            return true;
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpPost]
//        public bool ValidateTokenPower()
//        {
//            return true;
//        }

//    }
//}
