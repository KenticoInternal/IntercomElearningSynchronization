using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ElearningData.Models;

namespace ElearningData
{
    public class ElearningDataService : BaseElearningDataService, IElearningDataService
    {
        private string GetLastCompletedCourseFunctionUrl => Environment.GetEnvironmentVariable("GetLastCompletedCourseFunctionUrl");
        
        public ElearningDataService(IHttpClientFactory clientFactory) : base(clientFactory) { }

        public async Task<GetLatestCompletedCourseModel> GetLatestCompletedCourseAsync(string userEmail)
        {
            var response = await GetResponseAsync<GetLatestCompletedCourseModel>(GetLatestCompletedCourseUrl(userEmail));

            return response;
        }

        private string GetLatestCompletedCourseUrl(string userEmail)
        {
            return $"{GetLastCompletedCourseFunctionUrl}&email={userEmail}";
        }
    }
}
