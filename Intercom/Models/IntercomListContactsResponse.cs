using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Intercom.Models
{
    public class IntercomListContactsResponse
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public List<IntercomContact> Data { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("pages")]
        public IntercomPagesModel Pages { get; set; }
    }
}
