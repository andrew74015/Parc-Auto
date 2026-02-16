namespace WebAppParcAuto.Models
{
    public class ErrorVm
    {
        public string? Schema { get; set; }

        public string? Url { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public string? TraceIdentifier { get; set; }

        public string? ActivityId { get; set; }

        public int StatusCode { get; set; }
    }
}
