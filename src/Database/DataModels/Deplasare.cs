using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.DataModels
{
    public class Deplasare
    {
        [Key]
        public Guid Id { get; set; }

        public string Descriere { get; set; } = null!;

        public string Motiv { get; set; } = null!;

        public DateTime DataPlecare { get; set; }

        public DateTime? DataSosire { get; set; }

        public double SrcX { get;set; }

        public double SrcY { get; set; }

        public double DstX { get; set; }
        
        public double DstY { get; set; }

        public int Distanta { get; set; } 

        public Guid MasinaId { get; set; }

        [ForeignKey(nameof(MasinaId))]
        public Masina Masina { get; set; } = null!;

        public Guid AngajatId { get; set; }

        [ForeignKey(nameof(AngajatId))]
        public Angajat Angajat { get; set; } = null!;    

        public string? Observatii { get; set; }
    }
}
