using _003_JWT;
using _004_JWT_Custom.Helper;
using _004_JWT_Custom.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _004_JWT_Custom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet]
        [AllowAnonymous]
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
        [Authorize]
        public string ValidateToken()
        {
            return "Success";
        }

        [HttpPut]
        public async Task<TokenModel> CreateToken()
        {
            TokenHelper tokenHelper = new TokenHelper();
            string privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PrivateKeyPath") ?? "Keys/public.pem");

            var tokenModel = new TokenDataModel()
            {
                Role = "Admin,User,BackAdmin",
                UserId = 666,
                Username = "ZWJJ"
            };

            Dictionary<string, string> dataModels = new Dictionary<string, string>();

            dataModels.Add("Name", tokenModel.Username);
            dataModels.Add("ID", tokenModel.UserId.ToString());
            dataModels.Add("Role", tokenModel.Role);

            return await _tokenService.GenerateJwtToken(privateKeyPath, dataModels);
        }

    }
}
