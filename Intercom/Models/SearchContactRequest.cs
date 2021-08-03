using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intercom.Models
{
    public class SearchContactRequest
    {
        [JsonProperty("query")]
        public SearchContactQueryMultiple Query { get; set; }
    }

    public class SearchContactQueryMultiple
    {
        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("value")]
        public List<SearchContactQueryValue> Value { get; set; }
    }

    public class SearchContactQueryValue
    {
        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }
    }
}
