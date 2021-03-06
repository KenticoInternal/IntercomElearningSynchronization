using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElearningData.Models;

namespace ElearningData
{
    public interface IElearningDataService
    {

        Task<List<CompletedUserCoursesModel>> GetLatestCompletedCoursesAsync(List<string> userEmails);
    }
}
