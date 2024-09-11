using _003_JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    foreach (var item in typeof(SwaggerGroupEnum).GetEnumValues())
    {
        c.SwaggerDoc(item.ToString(), new OpenApiInfo()
        {
            Description = item.ToString(),
            Title = "This is Title",
            Version = "6.6.0.6",
        });
    }

    c.DocInclusionPredicate((docName, api) =>
    {
        if (!api.TryGetMethodInfo(out MethodInfo method)) return false;
        var attr = method.DeclaringType
        .GetCustomAttributes(true)
        .OfType<SwaggerGroupAttribute>()
        .FirstOrDefault() ?? method.GetCustomAttributes(true)
            .OfType<SwaggerGroupAttribute>()
            .FirstOrDefault();

        if (attr == null && docName == "Development")
        {
            return true;
        }
        else if (attr?.Group.ToString() == docName)
        {
            return true;
        }
        else
        {
            return false;
        }
    });

    // 开启小锁
    c.OperationFilter<AddResponseHeadersFilter>();
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

    c.DocumentFilter<CustomSwaggerDocumentFilter>();

    // 在header中添加token，传递到后台
    c.OperationFilter<SecurityRequirementsOperationFilter>();

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {

        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey
    });
});

builder.Services.AddSingleton(new Appsettings(builder.Configuration));

#region JWT 授权

builder.Services.AddAuthenticationCore(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
});

//注册鉴权服务并添加鉴权方案
//注册 Scheme 为 Options
//主要调用 AddAuthenticationCore
builder.Services.AddAuthentication(options =>
{
    //默认的策略
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //默认的授权策略
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //默认的授权策略
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
})
/**
 * 注册 IConfigureOptions<JwtBearerOptions>, JwtBearerConfigureOptions
 * 注册 IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions
 * AddScheme<JwtBearerOptions, JwtBearerHandler> //在 Scheme 元数据中设置 SchemeName + JwtBearerHandler
 */
.AddJwtBearer(options =>
{
    string Issuer = builder.Configuration["JWT:Issuer"];
    string Audience = builder.Configuration["JWT:Audience"];
    byte[] SecreityBytes = Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]);
    SecurityKey securityKey = new SymmetricSecurityKey(SecreityBytes);

    //赋值到 AuthenticationSchemeBuilder 并添加到 _schemes 集合中
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        //是否验证 Issuer(发行人)
        ValidateIssuer = true,
        //发行人
        ValidIssuer = Issuer,
        //是否验证 Audience(订阅人)
        ValidateAudience = true,
        //订阅人
        ValidAudience = Audience,
        //是否验证 SecreityKey
        ValidateIssuerSigningKey = true,
        //SecreityKey
        IssuerSigningKey = securityKey,
        //是否验证过期时间
        ValidateLifetime = true,
        //过期容错时间，解决服务器端时间不同步的问题(秒)
        ClockSkew = TimeSpan.FromSeconds(30),
        //是否要求过期时间
        RequireExpirationTime = true,
    };
    options.Events = new JwtBearerEvents()
    {
        //在身份认证的过程中任何失败都会执行次此事件
        //可以用于处理身份验证时出现的异常
        OnAuthenticationFailed = async context =>
        {
            var Logger = new LoggerFactory().CreateLogger("Authorization");
            Logger.LogError(context.Exception.Message);
            await Task.CompletedTask;
        },
        //身份验证失败后，Authentication 失败，鉴权失败
        //常用于响应 401 类型的消息，告诉对方当前身份验证失败了
        OnChallenge = async context =>
        {
            await context.Response.WriteAsync("身份验证失败");
            await context.Response.WriteAsync("无访问资源权限 401");
        },
        //身份认证失败后，Authorization 失败，授权失败
        //可以自定义 403 返回的响应
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("无访问资源权限 403");
        },
        //最先触发的事件
        //常用于提取 Token 信息
        OnMessageReceived = async context =>
        {
            await Task.CompletedTask;
        },
        // Token 令牌被验证成功后
        //这里可以执行一些关于 Token 令牌的操作，比如提取用户信息
        OnTokenValidated = async context =>
        {
            await Task.CompletedTask;
        }
    };
});

//注册授权策略
/**
 * 调用 AddAuthorizationCore
 * 
 */
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientResource", policy => policy.RequireRole("Client").Build());//单独角色
    options.AddPolicy("AdminResource", policy => policy.RequireRole("Admin").Build());
    options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));//或的关系
    options.AddPolicy("SystemAndAdmin", policy => policy.RequireRole("Admin").RequireRole("System"));//且的关系
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        //options.RoutePrefix = "";
        foreach (var item in typeof(SwaggerGroupEnum).GetEnumValues())
        {
            options.SwaggerEndpoint($"/swagger/{item}/swagger.json", $"{item}");
        }
    });
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
