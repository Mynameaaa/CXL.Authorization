﻿using Microsoft.AspNetCore.Authorization;

namespace _004_JWT_Custom.Service.Authorization;

public class CXLAuthorizationHandlerProvider : IAuthorizationHandlerProvider
{
    private readonly Task<IEnumerable<IAuthorizationHandler>> _handlersTask;

    //
    //  摘要:
    //      创建 DefaultAuthorizationHandlerProvider 实例
    //
    //  参数:
    //      handlers:
    //      授权处理程序
    public CXLAuthorizationHandlerProvider(IEnumerable<IAuthorizationHandler> handlers)
    {
        _handlersTask = Task.FromResult(handlers);
    }

    public Task<IEnumerable<IAuthorizationHandler>> GetHandlersAsync(AuthorizationHandlerContext context)
    {
        return _handlersTask;
    }
}
