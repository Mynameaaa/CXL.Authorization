namespace _003_JWT.相关源码
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.

    /// <summary>
    /// 在中设置身份验证服务的扩展方法 <see cref="IServiceCollection" />.
    /// </summary>
    public static class AuthenticationCoreServiceCollectionExtensions
    {
        /// <summary>
        ///  <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <param name="services">DI 容器<see cref="IServiceCollection"/>.</param>
        /// <returns>DI 容器</returns>
        public static IServiceCollection AddAuthenticationCore(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            //鉴权服务
            services.TryAddScoped<IAuthenticationService, AuthenticationService>();
            //与用户身份主体相关
            services.TryAddSingleton<IClaimsTransformation, NoopClaimsTransformation>(); // Can be replaced with scoped ones that use DbContext
            //鉴权处理器提供者
            services.TryAddScoped<IAuthenticationHandlerProvider, AuthenticationHandlerProvider>();
            //鉴权策略处理器提供者
            services.TryAddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
            return services;
        }

        /// <summary>
        /// services.AddAuthentication() 实际调用的方法 <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <param name="services">DI 容器<see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">Scheme 选项<see cref="AuthenticationOptions"/>.</param>
        /// <returns>DI 容器</returns>
        public static IServiceCollection AddAuthenticationCore(this IServiceCollection services, Action<AuthenticationOptions> configureOptions)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureOptions);

            //注册鉴权相关服务
            services.AddAuthenticationCore();
            //配置 Scheme Options
            services.Configure(configureOptions);
            return services;
        }
    }

}
