using Newtonsoft.Json;

namespace WebAppParcAuto.Models.Masini
{
    public class ListaMasiniViewModel
    {
        [JsonIgnore]
        public List<MasinaDto> Masini { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public int TotalRecords { get; set; }

        public int TotalFilteredRecords { get; set; }

        public string? SearchKey { get; set; }


        public ListaMasiniViewModel()
        {
            Masini = [];
        }
    }
}
