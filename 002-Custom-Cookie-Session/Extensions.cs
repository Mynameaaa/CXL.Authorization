using _002_Custom_Cookie_Session.Cookie;

namespace _002_Custom_Cookie_Session;

public static class Extensions
{
    public static WebApplication UseCustomAuthorization(this WebApplication app)
    {
        app.UseMiddleware<CustomAuthorizationMiddleware>();
        return app;
    }
}
