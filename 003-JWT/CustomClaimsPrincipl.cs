using System.Security.Claims;
using System.Security.Principal;

namespace _003_JWT
{
    public class CustomClaimsPrincipl : ClaimsPrincipal
    {
        //把拥有的证件都给当事人
        public CustomClaimsPrincipl(IEnumerable<ClaimsIdentity> identities) { }

        //当事人的主身份呢
        public virtual IIdentity Identity { get; }

        public virtual IEnumerable<ClaimsIdentity> Identities { get; }

        public virtual void AddIdentity(ClaimsIdentity identity)
        {
            this.AddIdentity(identity);
        }
    }
}
