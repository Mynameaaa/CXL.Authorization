using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region 添加身份验证中间件

//添加身份验证中间件服务 AddAuthentication，并使用 AddCookie 方法注入 cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme/* 使用 Cookie 策略进行身份验证 */)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = null;
    });

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* 鉴权授权中间件两者添加时注意顺序 */

//启用鉴权中间件
app.UseAuthentication();

//启用授权中间件
app.UseAuthorization();

app.MapControllers();

app.Run();
