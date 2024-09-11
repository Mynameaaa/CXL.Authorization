namespace _003_JWT.相关源码
{
    using System;
    using Microsoft.AspNetCore.Authorization.Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// 在服务中设置授权服务的扩展方法
    /// </summary>
    public static class AuthorizationServiceCollectionExtensions
    {
        /// <summary>
        /// 将授权服务添加到容器中
        /// </summary>
        public static IServiceCollection AddAuthorizationCore(this IServiceCollection services)
        {
            //注册选项服务
            services.AddOptions();

            services.TryAdd(ServiceDescriptor.Transient<IAuthorizationService, DefaultAuthorizationService>());
            services.TryAdd(ServiceDescriptor.Transient<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>());
            services.TryAdd(ServiceDescriptor.Transient<IAuthorizationHandlerProvider, DefaultAuthorizationHandlerProvider>());
            services.TryAdd(ServiceDescriptor.Transient<IAuthorizationEvaluator, DefaultAuthorizationEvaluator>());
            services.TryAdd(ServiceDescriptor.Transient<IAuthorizationHandlerContextFactory, DefaultAuthorizationHandlerContextFactory>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IAuthorizationHandler, PassThroughAuthorizationHandler>());
            return services;
        }

        /// <summary>
        /// 将授权服务添加到容器中
        /// </summary>
        /// <param name="configure">用于配置提供的的操作委托 </param>
        public static IServiceCollection AddAuthorizationCore(this IServiceCollection services, Action<AuthorizationOptions> configure)
        {
            services.Configure(configure);
            return services.AddAuthorizationCore();
        }
    }

}
