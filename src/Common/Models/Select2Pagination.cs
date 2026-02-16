namespace Common.Models
{
    public class Select2Pagination
    {
        public int TotalRecords { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int PageLast { get; set; }

        public bool IsFiltered { get; set; }
    }
}
