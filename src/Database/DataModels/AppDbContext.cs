using Database.ScalarMappedFunctions;
using Microsoft.EntityFrameworkCore;

namespace Database.DataModels
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Angajat> Angajati { get; set; } = null!;

        public virtual DbSet<Masina> Masini { get; set; } = null!;

        public virtual DbSet<Deplasare> Deplasari { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Select(s => s.ClrType))
                entityType.GetMethod("OnModelCreating")?.Invoke(entityType, [modelBuilder]);

            var udfCharindex = typeof(CharIndex).GetMethod(nameof(CharIndex.Udf));
            if (udfCharindex != null)
                modelBuilder.HasDbFunction(udfCharindex).HasName("CHARINDEX").IsBuiltIn(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
