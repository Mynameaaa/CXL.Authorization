using _004_JWT_Custom.Service.Authorization.Requirement;
using _004_JWT_Custom.Service.动态策略;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using System.Threading;

namespace _004_JWT_Custom.Service;


public class CXLAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    // 默认的策略提供者，用于回退到默认的策略机制
    private DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public CXLAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        // 调用默认策略提供者
        var result = _fallbackPolicyProvider.GetDefaultPolicyAsync();
        return result;
    }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
        // 可选：为某些情况下返回备用策略
        var result = _fallbackPolicyProvider.GetFallbackPolicyAsync();
        return result;
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        // 根据策略名称创建或返回对应的 AuthorizationPolicy
        if (policyName.StartsWith("CXL"))
        {
            // 根据策略名称生成 AuthorizationPolicy，例如
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new CXLRequirement(policyName)) // 自定义需求
                .Build();
            return Task.FromResult(policy);
        }

        // 如果策略名称不以 "CXL" 开头，则使用默认提供者
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}


#region 源码

//public class CXLAuthorizationPolicyProvider : IAuthorizationPolicyProvider
//{
//    private static readonly AsyncLock _mutex = new();
//    private readonly AuthorizationOptions _authorizationOptions;
//    public CXLAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
//    {
//        BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
//        _authorizationOptions = options.Value;
//    }

//    // 若不需要自定义实现，则均使用默认的
//    private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

//    public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
//    {
//        if (policyName is null) throw new ArgumentNullException(nameof(policyName));

//        // 若策略实例已存在，则直接返回
//        var policy = await BackupPolicyProvider.GetPolicyAsync(policyName);
//        if (policy is not null)
//        {
//            return policy;
//        }

//        using (await _mutex.LockAsync())
//        {
//            policy = await BackupPolicyProvider.GetPolicyAsync(policyName);
//            if (policy is not null)
//            {
//                return policy;
//            }

//            if (policyName.StartsWith(CXLAuthorizeAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase)
//                && int.TryParse(policyName[CXLAuthorizeAttribute.PolicyPrefix.Length..], out var age))
//            {
//                // 动态创建策略
//                var builder = new AuthorizationPolicyBuilder();
//                // 添加 Requirement
//                builder.AddRequirements(new CXLPermissionRequirement(age));
//                policy = builder.Build();
//                // 将策略添加到选项
//                _authorizationOptions.AddPolicy(policyName, policy);

//                return policy;
//            }
//        }

//        return null;
//    }

//    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
//    {
//        return await BackupPolicyProvider.GetDefaultPolicyAsync();
//    }

//    public async Task<AuthorizationPolicy> GetFallbackPolicyAsync()
//    {
//        return await BackupPolicyProvider.GetFallbackPolicyAsync();
//    }
//} 

#endregion
