using System.Runtime.CompilerServices;

namespace _004_JWT_Custom
{
    public static class Appsettings
    {
        private static IConfiguration _configuration;

        public static IConfiguration InitConfigure(this IConfiguration configuration)
        {
            _configuration = configuration;
            return configuration;
        }

        public static string? app(string sectionKey) => _configuration[sectionKey];

        public static T? app<T>(string sectionKey) => _configuration.GetSection(sectionKey).Get<T>();

    }
}
