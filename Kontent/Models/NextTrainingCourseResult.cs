
namespace Kontent.Models
{
    public class NextTrainingCourseResult
    {
        public TrainingCourseModel LatestCompletedCourse { get; set; }
        public TrainingCourseModel NextCourseInPath { get; set; }
    }
}
