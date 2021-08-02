using System.Threading.Tasks;
using Kontent.Models;

namespace Kontent
{
    public interface IKontentService
    {
        Task<TrainingCourseModel> GetTrainingCourseByTalentLmsId(string id);
        Task<NextTrainingCourseResult> GetNextTrainingCourseByTalentLmsId(string id);
    }
}
