using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAppParcAuto.Models
{
    [Bind(Prefix = "Masina")]
    public class MasinaDto
    {
        public Guid Id { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Marca este obligatorie")]
        public string? Marca { get; set; }


        [Display(Name = "Număr de kilometri")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Numărul de kilometri este obligatoriu")]
        [Range(0, 1000000, ErrorMessage = "Numărul de kilometri trebuie să nu aibe mai mult de 7 cifre")]
        public int NumarDeKilometri { get; set; }


        [Display(Name = "An fabricație")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Anul fabricației este obligatoriu")]
        [Range(2000, 2100, ErrorMessage = "Anul fabricației nu poate fi un paradox")]
        public int AnFabricatie { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Modelul este obligatoriu")]
        public string? Model { get; set; }


        [Display(Name = "Serie de șasiu")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Seria de șasiu este obligatorie")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Seria de șasiu trebuie să aibe exact 17 caractere")]
        public string? SerieDeSasiu { get; set; }


        [Display(Name = "Număr de înmatriculare")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Numărul de înmatriculare este obligatoriu")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Numărul de înmatriculare trebuie să aibe între 6 și 8 caractere")]
        public string? NumarDeInmatriculare { get; set; }


        public Database.DataModels.Masina ToDbMasina()
        {
            return new Database.DataModels.Masina
            {
                Id = Guid.NewGuid(),
                Marca = Marca ?? string.Empty,
                NumarDeKilometri = NumarDeKilometri,
                AnFabricatie = AnFabricatie,
                Model = Model ?? string.Empty,
                SerieDeSasiu = SerieDeSasiu ?? string.Empty,
                NumarDeInmatriculare = NumarDeInmatriculare ?? string.Empty
            };
        }


        public void UpdateDbMasina(Database.DataModels.Masina masina)
        {
            masina.Marca = Marca ?? string.Empty;
            masina.NumarDeKilometri = NumarDeKilometri;
            masina.AnFabricatie = AnFabricatie;
            masina.Model = Model ?? string.Empty;
            masina.SerieDeSasiu = SerieDeSasiu ?? string.Empty;
            masina.NumarDeInmatriculare = NumarDeInmatriculare ?? string.Empty;
        }


        public static MasinaDto FromDbMasina(Database.DataModels.Masina masina)
        {
            return new MasinaDto()
            {
                Id = masina.Id,
                Marca = masina.Marca,
                NumarDeKilometri = masina.NumarDeKilometri,
                AnFabricatie = masina.AnFabricatie,
                Model = masina.Model,
                SerieDeSasiu = masina.SerieDeSasiu,
                NumarDeInmatriculare = masina.NumarDeInmatriculare
            };
        }
    }
}
