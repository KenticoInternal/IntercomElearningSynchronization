using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ElearningData.Models
{
    internal class ReportingItemModel
    {

        [JsonProperty("_partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty("courseId")]
        public string CourseId { get; set; }

        [JsonProperty("courseTitle")]
        public string CourseTitle { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("completedDate")]
        public DateTime? CompletedDate { get; set; }
    }
}
