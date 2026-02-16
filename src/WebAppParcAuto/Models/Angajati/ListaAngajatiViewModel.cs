using Newtonsoft.Json;

namespace WebAppParcAuto.Models.Angajati
{
    public class ListaAngajatiViewModel
    {
        [JsonIgnore]
        public List<AngajatDto> Angajati { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public int TotalRecords { get; set; }

        public int TotalFilteredRecords { get; set; }

        public string? SearchKey { get; set; }


        public ListaAngajatiViewModel()
        {
            Angajati = [];
        }
    }
}
