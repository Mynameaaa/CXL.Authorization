namespace _003_JWT;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerGroupAttribute : Attribute
{
    public SwaggerGroupEnum Group { get; set; }

    public SwaggerGroupAttribute(SwaggerGroupEnum groupEnum)
    {
        this.Group = groupEnum;
    }
}
