using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intercom.Models
{

    public class SearchContactPaginationRequest
    {
        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("starting_after")]
        public string StartingAfter { get; set; }
    }

    public class SearchContactRequest
    {
        [JsonProperty("query")]
        public SearchContactQueryMultiple Query { get; set; }

        [JsonProperty("pagination")]
        public SearchContactPaginationRequest Pagination { get; set; }
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
