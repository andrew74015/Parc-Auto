using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAppParcAuto.Models.Masini
{
    public class EditareViewModel
    {
        public MasinaDto Masina { get; set; }

        public string? DetaliiEroare { get; set; }

        public IList<ModelError>? ModelErrors { get; set; }

        public string? ReturnUrl { get; set; }


        public EditareViewModel()
        {
            Masina = new MasinaDto();
        }
    }
}
