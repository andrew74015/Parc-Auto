using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Database.DataModels
{
    public class Masina
    {
        [Key]
        public Guid Id { get; set; }

        public string Marca { get; set; } = null!;

        public int NumarDeKilometri { get; set; }

        public int AnFabricatie { get; set; }

        public string Model { get; set; } = null!;

        public string SerieDeSasiu { get; set; } = null!;

        public string NumarDeInmatriculare { get; set; } = null!;


        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Masina>(entity =>
            {
                entity.HasIndex(e => e.SerieDeSasiu).IsUnique();
                entity.HasIndex(e => e.NumarDeInmatriculare).IsUnique();
            });
        }
    }
}
