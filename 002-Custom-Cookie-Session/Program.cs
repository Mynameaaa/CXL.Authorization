using _002_Custom_Cookie_Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Runtime.Intrinsics.Arm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region 自定义认证

builder.Services
    .AddAuthentication(CustomAuthenticationScheme.CustomScheme)
    .AddCookie(CustomAuthenticationScheme.CustomScheme, options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.LogoutPath = new PathString("/Account/Logout");
        options.AccessDeniedPath = new PathString("/Account/AccessDenied");
        options.ReturnUrlParameter = "returnUrl";

        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        //options.Cookie.Expiration = TimeSpan.FromMinutes(30);
        //options.Cookie.MaxAge = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;

        options.Cookie.Name = "auth";
        //options.Cookie.Domain = ".xxx.cn";
        options.Cookie.Path = "/";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.IsEssential = true;
        options.CookieManager = new ChunkingCookieManager();

        //options.DataProtectionProvider ??= dp;
        var dataProtector = options.DataProtectionProvider.CreateProtector($"Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware--{CookieAuthenticationDefaults.AuthenticationScheme}--v2");
        options.TicketDataFormat = new TicketDataFormat(dataProtector);

        options.Events.OnSigningIn = context =>
        {
            Console.WriteLine($"{context.Principal.Identity.Name} 正在登录...");
            return Task.CompletedTask;
        };

        options.Events.OnSignedIn = context =>
        {
            Console.WriteLine($"{context.Principal.Identity.Name} 已登录");
            return Task.CompletedTask;
        };

        options.Events.OnSigningOut = context =>
        {
            Console.WriteLine($"{context.HttpContext.User.Identity.Name} 注销");
            return Task.CompletedTask;
        };

        options.Events.OnValidatePrincipal += context =>
        {
            Console.WriteLine($"{context.Principal.Identity.Name} 验证 Principal");
            return Task.CompletedTask;
        };
    });

#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
