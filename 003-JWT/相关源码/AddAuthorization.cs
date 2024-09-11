namespace _003_JWT.相关源码
{
    using Microsoft.AspNetCore.Authorization.Policy;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    public static class PolicyServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddAuthorizationPolicyEvaluator();
            return services;
        }
    }

}
