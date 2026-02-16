using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebAppParcAuto.Models
{
    [Bind(Prefix = "Deplasare")]
    public class DeplasareDto
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Descrierea este obligatorie")]
        public string? Descriere { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Motivul este obligatoriu")]
        public string? Motiv { get; set; }


        [Display(Name = "Dată și oră plecare")]
        [Required(ErrorMessage = "Data/oră plecare este obligatorie")]
        public DateTime DataPlecare { get; set; }


        [Display(Name = "Dată și oră sosire")]
        [Required(ErrorMessage = "Data/oră plecare este obligatorie")]
        public DateTime? DataSosire { get; set; }


        [Display(Name = "Locație plecare")]
        [Required(ErrorMessage = "Obligatoriu")]
        public string? SrcX { get; set; }  // long


        [Required(ErrorMessage = " ")]
        public string? SrcY { get; set; } // lat


        [Display(Name = "Locație sosire")]
        [Required(ErrorMessage = "Obligatoriu")]
        public string? DstX { get; set; }


        [Required(ErrorMessage = " ")]
        public string? DstY { get; set; }


        [Display(Name = "Distanță KM")]
        [Range(1, 1000000, ErrorMessage = "Căutarea rutei este obligatorie")]
        public int Distanta { get; set; }


        [Display(Name = "Mașină")]
        [Required(ErrorMessage = "Mașina este obligatorie")]
        public Guid MasinaId { get; set; }


        public MasinaDto? Masina { get; set; }


        [Display(Name = "Angajat")]
        [Required(ErrorMessage = "Angajatul este obligatoriu")]
        public Guid AngajatId { get; set; }


        public AngajatDto? Angajat { get; set; }


        [Display(Name = "Observații")]
        public string? Observatii { get; set; }


        public Database.DataModels.Deplasare ToDbDeplasare()
        {
            var numberFormat = new CultureInfo("en").NumberFormat;

            return new Database.DataModels.Deplasare
            {
                Id = Guid.NewGuid(),
                Descriere = Descriere ?? string.Empty,
                Motiv = Motiv ?? string.Empty,
                DataPlecare = DataPlecare,
                DataSosire = DataSosire,
                SrcX = Double.Parse(SrcX ?? "0", numberFormat),
                SrcY = Double.Parse(SrcY ?? "0", numberFormat),
                DstX = Double.Parse(DstX ?? "0", numberFormat),
                DstY = Double.Parse(DstY ?? "0", numberFormat),
                Distanta = Distanta,
                MasinaId = MasinaId,
                AngajatId = AngajatId,
                Observatii = Observatii
            };
        }


        public void UpdateDbDeplasare(Database.DataModels.Deplasare deplasare)
        {
            var numberFormat = new CultureInfo("en").NumberFormat;

            deplasare.Masina.NumarDeKilometri = deplasare.Masina.NumarDeKilometri - deplasare.Distanta + Distanta;

            deplasare.Motiv = Motiv ?? string.Empty;
            deplasare.Descriere = Descriere ?? string.Empty;
            deplasare.DataPlecare = DataPlecare;
            deplasare.DataSosire = DataSosire;
            deplasare.SrcX = Double.Parse(SrcX ?? "0", numberFormat);
            deplasare.SrcY = Double.Parse(SrcY ?? "0", numberFormat);
            deplasare.DstX = Double.Parse(DstX ?? "0", numberFormat);
            deplasare.DstY = Double.Parse(DstY ?? "0", numberFormat);
            deplasare.Distanta = Distanta;
            deplasare.MasinaId = MasinaId;
            deplasare.AngajatId = AngajatId;
            deplasare.Observatii = Observatii;
        }


        public static DeplasareDto FromDbDeplasare(Database.DataModels.Deplasare deplasare)
        {
            var numberFormat = new CultureInfo("en").NumberFormat;

            return new DeplasareDto()
            {
                Id = deplasare.Id,
                Motiv = deplasare.Motiv,
                Descriere = deplasare.Descriere,
                DataPlecare = deplasare.DataPlecare,
                DataSosire= deplasare.DataSosire,
                SrcX = deplasare.SrcX.ToString(numberFormat),
                SrcY = deplasare.SrcY.ToString(numberFormat),
                DstX = deplasare.DstX.ToString(numberFormat),
                DstY = deplasare.DstY.ToString(numberFormat),
                Distanta = deplasare.Distanta,
                MasinaId = deplasare.MasinaId,
                Masina = MasinaDto.FromDbMasina(deplasare.Masina),
                AngajatId = deplasare.AngajatId,
                Angajat = AngajatDto.FromDbAngajat(deplasare.Angajat),
                Observatii = deplasare.Observatii
            };
        }
    }
}
