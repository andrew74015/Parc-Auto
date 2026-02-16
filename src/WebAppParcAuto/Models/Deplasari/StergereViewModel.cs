using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAppParcAuto.Models.Deplasari
{
    public class StergereViewModel
    {
        public DeplasareDto Deplasare { get; set; }

        public string? DetaliiEroare { get; set; }

        public IList<ModelError>? ModelErrors { get; set; }

        public string? ReturnUrl { get; set; }


        public StergereViewModel()
        {
            Deplasare = new DeplasareDto();
        }
    }
}
