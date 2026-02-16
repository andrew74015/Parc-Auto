using Common.Extensions;
using Database.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Declarations;
using System.Diagnostics;

namespace Services.Implementations
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializer> _logger;


        public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        public async Task Initialize()
        {
            using var scope = _serviceProvider.CreateScope();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var stopWatch = new Stopwatch();

                var migrationList = await dbContext.Database.GetPendingMigrationsAsync();

                if (migrationList.Any())
                {
                    foreach (var migration in migrationList)
                        _logger.LogInformation($"Migrare: {migration}");

                    stopWatch.Restart();

                    dbContext.Database.Migrate();

                    stopWatch.Stop();

                    _logger.LogInformation($"Migrările au fost aplicate în {stopWatch.Elapsed}");
                }
                else
                    _logger.LogInformation($"Nicio migrare disponibilă");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Initialize()");
            }
        }
    }
}
