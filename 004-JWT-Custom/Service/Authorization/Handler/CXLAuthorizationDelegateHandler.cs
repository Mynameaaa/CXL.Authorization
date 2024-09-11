using Microsoft.AspNetCore.Authorization;

namespace _004_JWT_Custom.Service;

public class CXLAuthorizationDelegateHandler : AuthorizationHandler<CXLPermissionRequirementDelegate>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CXLPermissionRequirementDelegate requirement)
    {
        if (requirement.ValiationDelegate == null)
        {
            return Task.CompletedTask;
        }
        else
        {
            if (requirement.ValiationDelegate.Invoke(requirement))
            {

            }
            else
            {
                //如果调用 Fail() 后续的 HandleRequirementAsync 将不再执行执行
                context.Fail();
            }
        }
        return Task.CompletedTask;
    }
}
