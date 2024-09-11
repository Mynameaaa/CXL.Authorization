using Microsoft.AspNetCore.Authorization;

namespace _004_JWT_Custom.Service.Authorization;
//
// 摘要:
//     确定授权请求是否成功。
public class CXLAuthorizationEvaluator : IAuthorizationEvaluator
{

    // 摘要:
    //     确定授权请求是否成功。
    //
    // 参数:
    //      context:
    //      授权信息。
    //
    // 返回结果:
    //      AuthorizationResult 授权结果
    public AuthorizationResult Evaluate(AuthorizationHandlerContext context)
    {
        if (!context.HasSucceeded)
        {
            return AuthorizationResult.Failed(context.HasFailed ? AuthorizationFailure.Failed(context.FailureReasons) : AuthorizationFailure.Failed(context.PendingRequirements));
        }

        return AuthorizationResult.Success();
    }
}
