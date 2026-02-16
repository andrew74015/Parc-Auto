using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAppParcAuto.Models
{
    [Bind(Prefix = "Angajat")]
    public class AngajatDto
    {
        public Guid Id { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Numele este obligatoriu")]
        public string? Nume { get; set; }

        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Prenumele este obligatoriu")]
        public string? Prenume { get; set; }


        [Display(Name = "Număr de telefon")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Numărul de telefon este obligatoriu")]
        [RegularExpression(@"^07[0-9]{2} [0-9]{3} [0-9]{3}$", ErrorMessage = "Numărul de telefon trebuie să fie de forma 07XX XXX XXX")]
        public string? Telefon { get; set; }


        [Display(Name = "Adresa de mail")]
        [EmailAddress(ErrorMessage = "Adresa de mail este invalidă")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Adresa de mail este obligatorie")]
        public string? Email { get; set; }

        [Display(Name = "CNP")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "CNP este obligatoriu")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "CNP-ul trebuie să aibă exact 13 caractere")]
        public string? Cnp { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Marca este obligatorie")]
        [Range(1000,9999, ErrorMessage = "Marca trebuie să aibe exact 4 cifre")]
        public int Marca { get; set; }      
        

        public Database.DataModels.Angajat ToDbAngajat()
        {
            return new Database.DataModels.Angajat
            {
                Id = Guid.NewGuid(),
                Nume = Nume ?? string.Empty,
                Prenume = Prenume ?? string.Empty,
                Telefon = Telefon ?? string.Empty,
                Email = Email ?? string.Empty,
                Cnp = Cnp ?? string.Empty,
                Marca = Marca
            };
        }


        public void UpdateDbAngajat(Database.DataModels.Angajat angajat)
        {
            angajat.Nume = Nume ?? string.Empty;
            angajat.Prenume = Prenume ?? string.Empty;
            angajat.Telefon = Telefon ?? string.Empty;
            angajat.Email = Email ?? string.Empty;
            angajat.Cnp = Cnp ?? string.Empty;
            angajat.Marca = Marca;
        }


        public static AngajatDto FromDbAngajat(Database.DataModels.Angajat angajat)
        {
            return new AngajatDto()
            {
                Id = angajat.Id,
                Nume = angajat.Nume,
                Prenume = angajat.Prenume,
                Telefon = angajat.Telefon,
                Email = angajat.Email,
                Cnp = angajat.Cnp,
                Marca = angajat.Marca
            };
        }
    }
}
