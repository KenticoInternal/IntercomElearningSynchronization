using Newtonsoft.Json;

namespace ElearningData.Models
{
    public class GetLatestCompletedCourseModel
    {

        [JsonProperty("courseId")]
        public string CourseId { get; set; }

    }
}
