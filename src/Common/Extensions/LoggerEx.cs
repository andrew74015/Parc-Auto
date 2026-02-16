using Microsoft.Extensions.Logging;

namespace Common.Extensions
{
    public static class LoggerEx
    {
        public static void LogInformation(this ILogger logger, string? message)
        {
            logger.LogInformation("{Message}", message);
        }
    }
}
