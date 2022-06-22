using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElearningData.Models
{
    public class GetLatestCompletedCoursesModel
    {

        [JsonProperty("users")]
        public List<GetLatestCompletedUserCourseModel> Users { get; set; }

    }

    public class GetLatestCompletedUserCourseModel
    {

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("course")]
        public string CourseId { get; set; }

        [JsonProperty("completedUtc")]
        public DateTime? CompletedUtc { get; set; }

    }
}
