namespace _004_JWT_Custom.Service.Authorization.Custom;

public interface IAuthorizeData
{
    // 策略
    string? Policy { get; set; }

    // 角色，可以通过英文逗号将多个角色分隔开，从而形成一个列表
    string? Roles { get; set; }

    // 身份认证方案，可以通过英文逗号将多个身份认证方案分隔开，从而形成一个列表
    string? AuthenticationSchemes { get; set; }

    string Other { get; }

}
