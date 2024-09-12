using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using static System.Net.Mime.MediaTypeNames;

namespace _004_JWT_Custom.Service
{
    [NoSchemeDefaultHandler]
    public class CXLAuthenticationHandler : AuthenticationHandler<CXLAuthenticationSchemeOptions>
    {
        /// <summary>
        /// 鉴权选项
        /// </summary>
        private readonly IOptionsMonitor<CXLAuthenticationSchemeOptions> _options;

        /// <summary>
        /// 日志工厂
        /// </summary>
        private readonly ILoggerFactory _logger;

        /// <summary>
        /// Url 编码
        /// </summary>
        private readonly UrlEncoder _urlEncoder;


        private readonly ISystemClock _clock;

        /// <summary>
        /// 发出质询返回的消息
        /// </summary>
        private Dictionary<string, string> _challengeMessages = new Dictionary<string, string>();

        public CXLAuthenticationHandler(IOptionsMonitor<CXLAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _options = options;
            _logger = logger;
            _urlEncoder = UrlEncoder;
            _clock = clock;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticateResult result = default(AuthenticateResult);

            var options = base.Options;

            #region 支持自定义鉴权策略

            var EventResult = options.InvokeAuthEvent(_logger);

            if (EventResult != null)
            {
                var Log = _logger.CreateLogger<CXLAuthenticationHandler>();
                Log.LogInformation("执行了由事件触发的鉴权策略");
                result = await EventResult;
                if (options.UseEventResult) return result;
            }

            #endregion

            //加载 Token 信息
            var token = InitToken();

            if (!token.Item1 || string.IsNullOrWhiteSpace(token.Item2))
            {
                // Token 验证失败，立即返回 401 错误
                return AuthenticateResult.Fail(token.Item2);
            }

            #region Token 验证

            ClaimsPrincipal claimsPrincipal = null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = base.Options.SecretKey, // 设置签名密钥
                    ValidateIssuer = base.Options.ValidateIssuer,  // 验证发行者
                    ValidateAudience = base.Options.ValidateAudience,  // 验证观众
                    ValidAudience = base.Options.Audience,
                    ValidIssuer = base.Options.Issuer,
                    ValidateLifetime = true, // 验证生命周期
                    ClockSkew = TimeSpan.FromSeconds(30) // 时钟偏移
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token.Item2, validationParameters, out validatedToken);
                claimsPrincipal = principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _challengeMessages.Add("message", "Token 信息已过期请重新获取");
                return AuthenticateResult.Fail(string.Empty);
            }
            catch (SecurityTokenSignatureKeyNotFoundException ex)
            {
                _challengeMessages.Add("message", "无效的 Token 信息");
                return AuthenticateResult.Fail(string.Empty);
            }

            #endregion

            // 构建 AuthenticationTicket
            var authentication = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return AuthenticateResult.Success(authentication);
        }

        /// <summary>
        /// 获取 Token
        /// </summary>
        /// <returns></returns>
        private (bool, string) InitToken()
        {
            string? token = string.Empty;

            token = base.Context.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(token))
            {
                token = base.Context.Request.Headers["CXLToken"];
                if (string.IsNullOrWhiteSpace(token))
                {
                    _challengeMessages.Add("message", "不存在票据信息！请先登录");
                    return (false, string.Empty);
                }

                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }
            }
            else
            {
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }
            }

            return (true, token);
        }

        //private ClaimsPrincipal ValidateToken(string Token)
        //{

        //}

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            // 设置 403 状态码
            Response.StatusCode = StatusCodes.Status403Forbidden;
            Response.ContentType = "application/json";
            return base.HandleForbiddenAsync(properties);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // 设置 401 状态码
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.ContentType = "application/json";

            // 检查是否存在 "Message" 参数，否则使用默认消息
            var message = properties?.Parameters.ContainsKey("Message") == true
                ? properties.Parameters["Message"]?.ToString()
                : "Unauthorized";

            var jsonResponse = new Dictionary<string, string>
            {
                { "systemMessage", message },
                { "redirectUrl", Options.RedirectUrl },
            };

            if (!_challengeMessages.Any())
                jsonResponse.Add("message", Options.DefualtChallageMessage);
            else
            {
                foreach (var messageKeyValue in _challengeMessages)
                {
                    jsonResponse[messageKeyValue.Key] = messageKeyValue.Value;
                }
            }

            var jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse);
            await Response.WriteAsync(jsonResult);
        }

        /// <summary>
        /// 初始化 Handler 信息
        /// </summary>
        /// <returns></returns>
        protected override Task InitializeHandlerAsync()
        {
            return base.InitializeHandlerAsync();
        }

    }
}
