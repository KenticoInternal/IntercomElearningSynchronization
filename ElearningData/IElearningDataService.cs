using System;
using System.Threading.Tasks;
using ElearningData.Models;

namespace ElearningData
{
    public interface IElearningDataService
    {

        Task<GetLatestCompletedCourseModel> GetLatestCompletedCourseAsync(string userEmail);
    }
}
