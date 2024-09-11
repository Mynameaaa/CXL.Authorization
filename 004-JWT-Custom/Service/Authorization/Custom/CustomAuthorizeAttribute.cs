namespace _004_JWT_Custom.Service.Authorization.Custom;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CustomAuthorizeAttribute : Attribute, IAuthorizeData
{
    public CustomAuthorizeAttribute() { }

    public CustomAuthorizeAttribute(string policy)
    {
        Policy = policy;
    }

    public string? Policy { get; set; }
    public string? Roles { get; set; }
    public string? AuthenticationSchemes { get; set; }

    public string Other => "CXL";
}
