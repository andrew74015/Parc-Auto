using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAppParcAuto.Models.Masini
{
    public class AdaugareViewModel
    {
        public MasinaDto Masina { get; set; }

        public string? DetaliiEroare { get; set; }

        public IList<ModelError>? ModelErrors { get; set; }

        public string? ReturnUrl { get; set; }


        public AdaugareViewModel()
        {
            Masina = new MasinaDto();
        }
    }
}
