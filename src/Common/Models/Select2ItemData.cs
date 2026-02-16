using Newtonsoft.Json;

namespace Common.Models
{
    public class Select2ItemData
    {
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        [JsonProperty("tag")]
        public Dictionary<string, object>? Tag { get; set; }

        [JsonIgnore]
        public string SearchKey { get; set; } = null!;
    }
}
