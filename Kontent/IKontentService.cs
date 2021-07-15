using System.Threading.Tasks;
using Kontent.Models;

namespace Kontent
{
    public interface IKontentService
    {
        Task<TrainingCourseModel> GetTrainingCourseByTalentLmsId(string id);
        Task<TrainingCourseModel> GetNextTrainingCourseByTalentLmsId(string id);
    }
}
