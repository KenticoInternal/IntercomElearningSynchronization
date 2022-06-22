using System.Collections.Generic;
using System.Threading.Tasks;
using Kontent.Models;

namespace Kontent
{
    public interface IKontentService
    {
        Task<List<TrainingCourseModel>> GetTrainingCoursesByIds(List<string> ids);
        NextTrainingCourseResult GetNextTrainingCourseByCourseId(List<TrainingCourseModel> courses, string id);
    }
}
