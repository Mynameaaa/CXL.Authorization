using System.Security.Claims;

namespace _003_JWT
{
    public class CustomClaim : Claim
    {
        public CustomClaim(string type, string value) : base(type, value)
        {

        }

    }
}
