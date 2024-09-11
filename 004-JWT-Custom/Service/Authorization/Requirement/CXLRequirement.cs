using Microsoft.AspNetCore.Authorization;

namespace _004_JWT_Custom.Service.Authorization.Requirement
{
    public class CXLRequirement : IAuthorizationRequirement
    {
        public string PolicyName { get; }

        public CXLRequirement(string policyName)
        {
            PolicyName = policyName;
        }
    }
}
