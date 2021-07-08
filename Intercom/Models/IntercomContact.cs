﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intercom.Models
{
    public class IntercomContact
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Role")]
        public string Role { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("custom_attributes")]
        public JObject CustomAttributes { get; set; }

        [JsonProperty("companies")]
        public IntercomAddressableList IntercomAddressableList { get; set; }

        public string SignUpDate => CustomAttributes.SelectToken("sign_up_date").Value<string>();
    }
}
