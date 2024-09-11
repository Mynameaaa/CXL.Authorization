using System.Security.Principal;

namespace _003_JWT
{
    public class CustomClaimIdentity : IIdentity
    {
        public string? AuthenticationType => "CustomType";

        public bool IsAuthenticated => true;

        public string? Name => nameof(CustomClaimIdentity);
    }
}
