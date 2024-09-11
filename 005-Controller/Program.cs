using _005_Controller;

var builder = WebApplication.CreateBuilder(args);

// 添加自定义控制器和路由配置
builder.Services.AddControllers(options =>
{
    // 添加自定义的路由规则
    options.Conventions.Add(new CXLRouteConvention());
})
.ConfigureApplicationPartManager(manager =>
{
    // 添加自定义的 CXL 控制器提供程序
    manager.FeatureProviders.Add(new CXLControllerFeatureProvider());
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();
app.Run();

