using _004_JWT_Custom;
using _004_JWT_Custom.Service;
using _004_JWT_Custom.Service.Authorization;
using _004_JWT_Custom.Service.鉴权相关;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new Appsettings(builder.Configuration));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 定义 JWT Bearer 的安全定义
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "请输入带有 'Bearer ' 前缀的Token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.OperationFilter<TokenOperationFilter>();
});

#region 注册鉴权授权服务

//鉴权相关服务
builder.Services.AddScoped<IAuthenticationHandlerProvider, CXLAuthenticationHandlerProvider>();
builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, CXLAuthorizationHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, CXLAuthorizationDelegateHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, CXLAuthorizationAllRequirementHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationHandler, CXLAuthorizationHandler>());


//授权相关服务
builder.Services.AddTransient<IAuthorizationService, CXLAuthorizationService>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CXLAuthorizationPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandlerContextFactory, CXLAuthorizationHandlerContextFactory>();
builder.Services.AddSingleton<IAuthorizationEvaluator, CXLAuthorizationEvaluator>();

#endregion

#region JWT 鉴权

string Issuer = builder.Configuration["Audience:Issuer"];
string Audience = builder.Configuration["Audience:Audience"];
byte[] SecreityBytes = Encoding.UTF8.GetBytes(builder.Configuration["Audience:Secret"]);
SecurityKey securityKey = new SymmetricSecurityKey(SecreityBytes);

builder.Services.AddCXLAuthentication(options =>
{
    options.DefaultScheme = CXLConstantScheme.Scheme;
    options.DefaultForbidScheme = CXLConstantScheme.Scheme;
    options.DefaultChallengeScheme = CXLConstantScheme.Scheme;
}).AddScheme<CXLAuthenticationSchemeOptions, CXLAuthenticationHandler>(CXLConstantScheme.Scheme, options =>
{
    options.Age = 18;
    options.DisplayName = "ZWJ";
    //发行人
    options.Issuer = Issuer;
    options.ValidateAudience = true;
    options.Audience = Audience;
    options.ValidateIssuer = true;
    options.SecretKey = securityKey;
    options.DefualtChallageMessage = "无效的 Token 或未找到合适的 Token";
    options.RedirectUrl = "https://google.com";
    ////自定义鉴权逻辑
    //options.AuthEvent += logger =>
    //{
    //    options.UseEventResult = true;
    //    return Task.FromResult(AuthenticateResult.Fail("你好"));
    //};
});

#endregion

#region JWT 授权

builder.Services.AddAuthorization(options =>
{
    //必须包含角色Claim
    options.AddPolicy("SystemRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomRole", policy => policy.RequireClaim(ClaimTypes.Role));

    //包含角色 Claim 且值必须为 Admin
    options.AddPolicy("SystemRoleValue", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomRoleValue", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));

    //包含角色 Claim 且值必须为 Admin 或者 Agent
    options.AddPolicy("SystemRoleValueAnyOne", policy => policy.RequireRole("Admin", "Agent"));
    options.AddPolicy("CustomRoleValueAnyOne", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "Agent"));

    //包含角色 Claim 且值必须包含 Admin 和 Agent，集合形式才可以通过验证
    options.AddPolicy("CustomRoleValueAll", policy => policy.RequireClaim(ClaimTypes.Role, "Admin").RequireClaim(ClaimTypes.Role, "Agent"));
    options.AddPolicy("SystemRoleValueAll", policy => policy.RequireRole("Admin").RequireRole("Agent"));

    //包含 Age Claim
    options.AddPolicy("CustomAgeAndWork", policy => policy.RequireClaim("Age"));

    //包含 Age Claim 和 Work Claim
    options.AddPolicy("CustomAgeAndWork", policy => policy.RequireClaim("Age").RequireClaim("Work"));

    //包含 Age 并且值为 18
    options.AddPolicy("CustomAgeValue", policy => policy.RequireClaim("Age", "18"));

    //包含 Age 并且值为 18 或者 21
    options.AddPolicy("CustomAgeValueAnyOne", policy => policy.RequireClaim("Age", "18", "21"));

    //包含 StoreName 并且值为 Root 和 Admin
    options.AddPolicy("CustomAgeValueAnyOne", policy => policy.RequireClaim("StoreName", "Root").RequireClaim("Admin"));


    //包含角色 Claim 或者 名称 Claim
    options.AddPolicy("CustomRoleOrName", policy =>
    {
        policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(c => c.Type == ClaimTypes.Role) ||
             context.User.HasClaim(c => c.Type == ClaimTypes.Name);
        });
    });

    //自定义简单策略授权
    options.AddPolicy("CustomValidationAge", policy => policy.Requirements.Add(new CXLPermissionRequirement(18)));
    options.AddPolicy("CustomDelegateValidation", policy => policy.Requirements.Add(new CXLPermissionRequirementDelegate(options =>
    {
        //自定义验证逻辑
        if (options.UserName.Equals("张无忌"))
            return true;
        return false;
    }, "张无忌", "明教教主", "光明顶", "九阳神功与乾坤大挪移护体")));
});

#endregion

var app = builder.Build();

CXLAuthorizationService defaultAuthorizationService = app.Services.GetService<IAuthorizationService>() as CXLAuthorizationService;

DefaultAuthorizationHandlerProvider handlerprovider = app.Services.GetService<IAuthorizationHandlerProvider>() as DefaultAuthorizationHandlerProvider;

DefaultAuthorizationEvaluator evaluator = app.Services.GetService<IAuthorizationEvaluator>() as DefaultAuthorizationEvaluator;

DefaultAuthorizationHandlerContextFactory contexfactory = app.Services.GetService<IAuthorizationHandlerContextFactory>() as DefaultAuthorizationHandlerContextFactory;

AuthorizationHandler<IAuthorizationRequirement> handler = app.Services.GetService<IAuthorizationHandler>() as AuthorizationHandler<IAuthorizationRequirement>;

PolicyEvaluator policy = app.Services.GetService<IPolicyEvaluator>() as PolicyEvaluator;

AuthorizationMiddlewareResultHandler authorizationMiddlewareResultHandler = app.Services.GetService<IAuthorizationMiddlewareResultHandler>() as AuthorizationMiddlewareResultHandler;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

//app.UseMiddleware<CXLAuthorizationMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
