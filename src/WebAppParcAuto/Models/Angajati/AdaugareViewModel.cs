using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAppParcAuto.Models.Angajati
{
    public class AdaugareViewModel
    {
        public AngajatDto Angajat { get; set; }
        
        public string? DetaliiEroare { get; set; }

        public IList<ModelError>? ModelErrors { get; set; }

        public string? ReturnUrl { get; set; }


        public AdaugareViewModel()
        {
            Angajat = new AngajatDto();
        }
    }
}
