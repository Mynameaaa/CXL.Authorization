using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace _006_Swagger
{
    public class CXLSwaggerGroupOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var groupAttribute = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<CXLSwaggerGroupAttribute>()
                .FirstOrDefault() ?? null;

            var groupMethodAttribute = context.MethodInfo.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<CXLSwaggerGroupAttribute>()
                .FirstOrDefault();

            if (groupAttribute == null && groupMethodAttribute == null)
            {
                var tagName = context.MethodInfo.DeclaringType?.Name.Replace("Controller", "") ?? "Defaults";
                operation.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = tagName }
                };

                context.ApiDescription.GroupName = "Default";
            }

            if (groupMethodAttribute != null)
            {
                operation.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = groupMethodAttribute.GroupName }
                };

                context.ApiDescription.GroupName = groupMethodAttribute.GroupName;
            }

            if (groupAttribute != null)
            {
                operation.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = groupAttribute.GroupName }
                };

                context.ApiDescription.GroupName = groupAttribute.GroupName;
            }
        }
    }
}
