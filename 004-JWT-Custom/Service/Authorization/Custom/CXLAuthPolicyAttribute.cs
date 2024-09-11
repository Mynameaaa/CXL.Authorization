using Microsoft.AspNetCore.Authorization;

namespace _004_JWT_Custom.Service.Authorization.Custom
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CXLAuthPolicyAttribute : Attribute
    {
        public string Policy { get; set; }

        public CXLAuthPolicyAttribute(string policyName)
        {
            // 设置基类的 Policy 属性，将策略名称传递给 IAuthorizationPolicyProvider
            Policy = policyName;
        }
    }
}
