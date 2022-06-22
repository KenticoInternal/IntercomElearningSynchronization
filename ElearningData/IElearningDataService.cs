using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearningData.Models;

namespace ElearningData
{
    public interface IElearningDataService
    {

        Task<GetLatestCompletedCoursesModel> GetLatestCompletedCoursesAsync(List<string> userEmails);
    }
}
