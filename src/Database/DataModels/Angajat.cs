using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Database.DataModels
{
    public class Angajat
    {
        [Key]
        public Guid Id { get; set; }

        public string Nume { get; set; } = null!;

        public string Prenume { get; set; } = null!;

        public string Telefon { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Cnp { get; set; } = null!;

        public int Marca { get; set; }


        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Angajat>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Cnp).IsUnique();
                entity.HasIndex(e => e.Marca).IsUnique();
            });
        }
    }
}
