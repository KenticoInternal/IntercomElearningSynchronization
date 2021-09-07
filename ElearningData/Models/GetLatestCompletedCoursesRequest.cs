using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElearningData.Models
{
    public class GetLatestCompletedCoursesRequest
    {
        [JsonProperty("users")]
        public List<string> Users { get; set; }
    }
}
