//namespace _003_JWT.相关源码
//{
//    using Microsoft.AspNetCore.Authentication.JwtBearer;
//    using Microsoft.AspNetCore.Authentication;
//    using Microsoft.Extensions.Options;
//    using Microsoft.Extensions.DependencyInjection.Extensions;


//    /// <summary>
//    /// 配置JWT承载身份验证的扩展方法。
//    /// </summary>
//    public static class JwtBearerExtensions
//    {
//        /// <summary>
//        /// 使用默认方案启用JWT承载身份验证
//        /// <para>
//        /// JWT承载身份验证通过从<c>Authorization</c>请求标头中提取并验证JWT令牌来执行身份验证。
//        /// </para>
//        /// </summary>
//        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder)
//            => builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, _ => { });

//        /// <summary>
//        /// 使用预定义方案启用JWT承载身份验证。
//        /// <para>
//        /// JWT承载身份验证通过从<c>Authorization</c>请求标头中提取并验证JWT令牌来执行身份验证。
//        /// </para>
//        /// </summary>
//        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme)
//            => builder.AddJwtBearer(authenticationScheme, _ => { });

//        /// <summary>
//        /// 使用默认方案启用JWT承载身份验证
//        /// <para>
//        /// JWT承载身份验证通过从<c>Authorization</c>请求标头中提取并验证JWT令牌来执行身份验证。
//        /// </para>
//        /// </summary>
//        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, Action<JwtBearerOptions> configureOptions)
//            => builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions);

//        /// <summary>
//        /// 使用指定的方案启用JWT承载身份验证。
//        /// <para>
//        /// JWT承载身份验证通过从<c>Authorization</c>请求标头中提取并验证JWT令牌来执行身份验证。
//        /// </para>
//        /// </summary>
//        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
//        /// <param name="authenticationScheme">身份验证方案</param>
//        /// <param name="configureOptions">允许配置的委托 <see cref="JwtBearerOptions"/>.</param>
//        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
//        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, Action<JwtBearerOptions> configureOptions)
//            => builder.AddJwtBearer(authenticationScheme, displayName: null, configureOptions: configureOptions);

//        /// <summary>
//        /// 使用指定的方案启用JWT承载身份验证。
//        /// <para>
//        /// JWT承载身份验证通过从<c>Authorization</c>请求标头中提取并验证JWT令牌来执行身份验证。
//        /// </para>
//        /// </summary>
//        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
//        /// <param name="authenticationScheme">身份验证方案</param>
//        /// <param name="displayName">身份验证处理程序的显示名称</param>
//        /// <param name="configureOptions">允许配置的委托 <see cref="JwtBearerOptions"/>.</param>
//        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
//        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<JwtBearerOptions> configureOptions)
//        {
//            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<JwtBearerOptions>, JwtBearerConfigureOptions>());
//            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions>());
//            return builder.AddScheme<JwtBearerOptions, JwtBearerHandler>(authenticationScheme, displayName, configureOptions);
//        }
//    }

//}
