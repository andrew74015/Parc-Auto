using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database.DataModels
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var srcDirectory = Directory.GetParent(Directory.GetCurrentDirectory());

            if (srcDirectory == null || !srcDirectory.Exists)
                throw new ApplicationException(@"Folder not found .\..\src");

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(srcDirectory.FullName)
                .AddJsonFile(@".\WebAppParcAuto\appsettings.json");

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environmentName))
                environmentName = "Development";

            if (!string.IsNullOrWhiteSpace(environmentName) && environmentName != "Production")
                configurationBuilder.AddJsonFile($@".\WebAppParcAuto\\appsettings.{environmentName}.json");

            var configuration = configurationBuilder.Build();

            var defaultDatabaseConnection = configuration.GetConnectionString("DefaultDatabaseConnection");

            Console.WriteLine($"\nEnvironment: {environmentName}");
            Console.WriteLine($"DatabaseConnection_Default: {defaultDatabaseConnection}\n");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(defaultDatabaseConnection);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
