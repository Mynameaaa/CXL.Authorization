using _006_Swagger;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = "cxlDirectory",
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("Order", new OpenApiInfo { Title = "Order API", Version = "v1" });
    c.SwaggerDoc("User", new OpenApiInfo { Title = "User API", Version = "v1" });
    c.SwaggerDoc("Stock", new OpenApiInfo { Title = "Stock API", Version = "v1" });
    c.SwaggerDoc("Default", new OpenApiInfo { Title = "Default API", Version = "v1" });

    c.OperationFilter<CXLSwaggerGroupOperationFilter>();
});

Console.WriteLine(Directory.GetCurrentDirectory());
Console.WriteLine(AppContext.BaseDirectory);
Console.WriteLine(builder.Environment.ContentRootPath);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {

    });
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "doc";

        c.SwaggerEndpoint("/swagger/Order/swagger.json", "Order API V1");
        c.SwaggerEndpoint("/swagger/User/swagger.json", "User API V1");
        c.SwaggerEndpoint("/swagger/Stock/swagger.json", "Stock API V1");
        c.SwaggerEndpoint("/swagger/Default/swagger.json", "Default API V1");
    });
}

app.Use((context, next) =>
{
    Console.WriteLine(Directory.GetCurrentDirectory());
    Console.WriteLine(AppContext.BaseDirectory);
    Console.WriteLine(builder.Environment.ContentRootPath);
    return next(context);
});

app.UseAuthorization();

app.MapControllers();

app.Run();
