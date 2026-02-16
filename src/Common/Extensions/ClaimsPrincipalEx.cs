using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Common.Extensions
{
    public static class ClaimsPrincipalEx
    {
        public static bool GetIsAuthenticated(this ClaimsPrincipal? user) => user?.Identity?.IsAuthenticated ?? false;


        public static string? GetClaimValue(this ClaimsPrincipal? user, string claimName)
        {
            if (user == null || !user.GetIsAuthenticated())
                return default;

            return user.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
        }


        public static void LogTo(this ClaimsPrincipal? user, ILogger logger, LogLevel logLevel, string preMessage = "")
        {
            if (!user.GetIsAuthenticated())
                return;

            var objectToLog = new
            {
                Claims = new List<string>(),
            };

            if (user != null)
                foreach (var claim in user.Claims)
                    objectToLog.Claims.Add($"{claim.Type} : {claim.Value}");

            if (user?.Identity?.Name == null)
                logger.Log(logLevel, $"{preMessage} \n{JsonConvert.SerializeObject(objectToLog, Formatting.Indented)}");
            else
                logger.Log(logLevel, $"{preMessage} Identity {user.Identity?.Name}\n{JsonConvert.SerializeObject(objectToLog, Formatting.Indented)}");
        }
    }
}
