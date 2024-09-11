using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _001_Cookie_Session.Controllers
{

    /// <summary>
    /// 负责 颁发、验证、销毁 Cookie 信息
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CookieIssueController : ControllerBase
    {

        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<BaseResult> UserSignInAsync(string UserName, string Password)
        {
            try
            {
                if (UserName != "zwj" || Password != "123456")
                {
                    return new BaseResult { Message = "登陆验证失败" };
                }

                var user = new UserInfo
                {
                    UserName = UserName,
                    Password = Password,
                    Address = "湖南xxx",
                    Email = "110@163.com",
                    UserID = 1,
                    Work = "电子厂"
                };

                //个人身份信息
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim("Work", user.Work),
                };

                //通过 Claims 来创建 ClaimIdentity，类似于通过用户信息来创建一张用户的身份证
                ClaimsIdentity cl = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //允许刷新身份验证会话
                    AllowRefresh = true,
                    //身份验证票证过期时间 3 分钟
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(3),
                    //允许持久化
                    IsPersistent = true,
                    //cookies 过期时间
                    IssuedUtc = DateTimeOffset.UtcNow.AddMinutes(3),
                    //重定向 url 地址
                    RedirectUri = "",
                };

                //授权 cookies 
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(cl), authProperties);

                return new BaseResult
                {
                    Message = $"用户{user.UserName}登录成功,登录时间{DateTime.UtcNow}",
                    Code = 200,
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new BaseResult { Message = ex.ToString() };
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        /* 该接口需要授权并且使用的授权策略为 CookieAuthenticationDefaults.AuthenticationScheme */
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet]
        public BaseResult GetUser()
        {
            //
            if (HttpContext.User.Identity != null)
            {
                if (HttpContext.User.Identity.IsAuthenticated)  //判断用户是否通过认证
                {
                    string name = HttpContext.User.Claims.ToList()[0].Value;

                    return new BaseResult
                    {
                        Code = 200,
                        Message = $"当前用户是{name}"
                    };
                }
                else
                {
                    return new BaseResult
                    {
                        Code = 400,
                        Message = "未登录"
                    };
                }
            }

            return new BaseResult
            {
                Code = 400,
                Message = "无权访问"
            };
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> UserSignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new BaseResult
            {
                Code = 200,
                Message = "注销成功"
            };
        }

    }
}
