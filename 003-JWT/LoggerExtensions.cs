namespace _003_JWT;

public static class LoggerExtensions
{
    private static readonly Action<ILogger, Exception> _tokenValidationFailed = LoggerMessage.Define(LogLevel.Information, new EventId(1, "TokenValidationFailed"), "Failed to validate the token.");

    private static readonly Action<ILogger, Exception> _tokenValidationSucceeded = LoggerMessage.Define(LogLevel.Debug, new EventId(2, "TokenValidationSucceeded"), "Successfully validated the token.");

    private static readonly Action<ILogger, Exception> _errorProcessingMessage = LoggerMessage.Define(LogLevel.Error, new EventId(3, "ProcessingMessageFailed"), "Exception occurred while processing message.");

    public static void TokenValidationFailed(this ILogger logger, Exception ex)
    {
        _tokenValidationFailed(logger, ex);
    }

    public static void TokenValidationSucceeded(this ILogger logger)
    {
        _tokenValidationSucceeded(logger, null);
    }

    public static void ErrorProcessingMessage(this ILogger logger, Exception ex)
    {
        _errorProcessingMessage(logger, ex);
    }
}
