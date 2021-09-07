using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearningData.Models;

namespace ElearningData
{
    public interface IElearningDataService
    {

        Task<GetLatestCompletedCourseModel> GetLatestCompletedCourseAsync(string userEmail);
        Task<GetLatestCompletedCoursesModel> GetLatestCompletedCoursesAsync(List<string> userEmails);
    }
}
