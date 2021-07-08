using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Intercom.Models
{
    public class IntercomAddressableList
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public List<IntercomAddressableListModel> Data { get; set; }
    }
}
