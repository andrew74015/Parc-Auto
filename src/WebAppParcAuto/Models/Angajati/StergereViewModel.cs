using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAppParcAuto.Models.Angajati
{
    public class StergereViewModel
    {
        public AngajatDto Angajat { get; set; }
        
        public string?  DetaliiEroare { get; set; }

        public IList<ModelError>? ModelErrors { get; set; }

        public string? ReturnUrl { get; set; }


        public StergereViewModel()
        {
            Angajat = new AngajatDto();
        }
    }
}
