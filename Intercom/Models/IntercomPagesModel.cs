using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Intercom.Models
{
    public class IntercomPagesModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("next")]
        public IntercomPagesNextModel Next { get; set; }
    }

    public class IntercomPagesNextModel
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("starting_after")]
        public string StartingAfter { get; set; }
    }
}
