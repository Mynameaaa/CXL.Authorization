//namespace _003_JWT.相关源码
//{
//    // Licensed to the .NET Foundation under one or more agreements.
//    // The .NET Foundation licenses this file to you under the MIT license.

//    using System.Diagnostics.CodeAnalysis;
//    using global::Microsoft.AspNetCore.Authentication;
//    using global::Microsoft.Extensions.DependencyInjection.Extensions;
//    using global::Microsoft.Extensions.Options;
//    using Microsoft.Extensions.DependencyInjection;

//    /// <summary>
//    /// 用于配置身份验证
//    /// </summary>
//    public class AuthenticationBuilder
//    {
//        public AuthenticationBuilder(IServiceCollection services)
//        {
//            Services = services;
//        }

//        /// <summary>
//        /// 正在配置的服务
//        /// </summary>
//        public virtual IServiceCollection Services { get; }

//        private AuthenticationBuilder AddSchemeHelper<TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(string authenticationScheme, string? displayName, Action<TOptions>? configureOptions)
//            where TOptions : AuthenticationSchemeOptions, new()
//            where THandler : class, IAuthenticationHandler
//        {
//            Services.Configure<AuthenticationOptions>(o =>
//            {
//                o.AddScheme(authenticationScheme, scheme =>
//                {
//                    scheme.HandlerType = typeof(THandler);
//                    scheme.DisplayName = displayName;
//                });
//            });
//            if (configureOptions != null)
//            {
//                Services.Configure(authenticationScheme, configureOptions);
//            }
//            Services.AddOptions<TOptions>(authenticationScheme).Validate(o =>
//            {
//                o.Validate(authenticationScheme);
//                return true;
//            });
//            Services.AddTransient<THandler>();
//            Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TOptions>, PostConfigureAuthenticationSchemeOptions<TOptions>>());
//            return this;
//        }

//        /// <summary>
//        /// Adds a <see cref="AuthenticationScheme"/> which can be used by <see cref="IAuthenticationService"/>.
//        /// </summary>
//        /// <typeparam name="TOptions">The <see cref="AuthenticationSchemeOptions"/>用于配置处理程序</typeparam>
//        /// <typeparam name="THandler">The <see cref="AuthenticationHandler{TOptions}"/>用于处理此方案</typeparam>
//        /// <param name="authenticationScheme">这是策略的名称</param>
//        /// <param name="displayName">这是显示名称</param>
//        /// <param name="configureOptions">用于配置方案选项</param>
//        /// <returns>The builder.</returns>
//        public virtual AuthenticationBuilder AddScheme<TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(string authenticationScheme, string? displayName, Action<TOptions>? configureOptions)
//            where TOptions : AuthenticationSchemeOptions, new()
//            where THandler : AuthenticationHandler<TOptions>
//            => AddSchemeHelper<TOptions, THandler>(authenticationScheme, displayName, configureOptions);


//        public virtual AuthenticationBuilder AddScheme<TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(string authenticationScheme, Action<TOptions>? configureOptions)
//            where TOptions : AuthenticationSchemeOptions, new()
//            where THandler : AuthenticationHandler<TOptions>
//            => AddScheme<TOptions, THandler>(authenticationScheme, displayName: null, configureOptions: configureOptions);


//        public virtual AuthenticationBuilder AddRemoteScheme<TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(string authenticationScheme, string? displayName, Action<TOptions>? configureOptions)
//            where TOptions : RemoteAuthenticationOptions, new()
//            where THandler : RemoteAuthenticationHandler<TOptions>
//        {
//            Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TOptions>, EnsureSignInScheme<TOptions>>());
//            return AddScheme<TOptions, THandler>(authenticationScheme, displayName, configureOptions: configureOptions);
//        }

//        public virtual AuthenticationBuilder AddPolicyScheme(string authenticationScheme, string? displayName, Action<PolicySchemeOptions> configureOptions)
//            => AddSchemeHelper<PolicySchemeOptions, PolicySchemeHandler>(authenticationScheme, displayName, configureOptions);

//        //用于确保始终存在一个不是其本身的默认登录方案
//        private sealed class EnsureSignInScheme<TOptions> : IPostConfigureOptions<TOptions> where TOptions : RemoteAuthenticationOptions
//        {
//            private readonly AuthenticationOptions _authOptions;

//            public EnsureSignInScheme(IOptions<AuthenticationOptions> authOptions)
//            {
//                _authOptions = authOptions.Value;
//            }

//            public void PostConfigure(string? name, TOptions options)
//            {
//                options.SignInScheme ??= _authOptions.DefaultSignInScheme ?? _authOptions.DefaultScheme;
//            }
//        }

//        // 如果测试尚未设置，请在所有选项实例上从DI设置TimeProvider。
//        private sealed class PostConfigureAuthenticationSchemeOptions<TOptions> : IPostConfigureOptions<TOptions>
//            where TOptions : AuthenticationSchemeOptions
//        {
//            public PostConfigureAuthenticationSchemeOptions(TimeProvider timeProvider)
//            {
//                TimeProvider = timeProvider;
//            }

//            private TimeProvider TimeProvider { get; }

//            public void PostConfigure(string? name, TOptions options)
//            {
//                options.TimeProvider ??= TimeProvider;
//            }
//        }
//    }

//}
