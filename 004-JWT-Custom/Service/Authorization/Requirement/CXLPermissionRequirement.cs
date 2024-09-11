using Microsoft.AspNetCore.Authorization;

namespace _004_JWT_Custom.Service
{
    public class CXLPermissionRequirement : IAuthorizationRequirement
    {
        public CXLPermissionRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }

        public int MinimumAge { get; set; }

    }
}
