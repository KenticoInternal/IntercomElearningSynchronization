using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Intercom.Models
{
    public class IntercomAddressableListModel
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
