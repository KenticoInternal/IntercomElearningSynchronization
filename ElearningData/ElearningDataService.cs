using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ElearningData.Models;
using Newtonsoft.Json;

namespace ElearningData
{
    public class ElearningDataService : BaseElearningDataService, IElearningDataService
    {
        private string GetLastCompletedCourseFunctionUrl => Environment.GetEnvironmentVariable("GetLastCompletedCourseFunctionUrl");
        private string GetLastCompletedCoursesFunctionUrl => Environment.GetEnvironmentVariable("GetLastCompletedCoursesFunctionUrl");
        
        public ElearningDataService(IHttpClientFactory clientFactory) : base(clientFactory) { }

        public async Task<GetLatestCompletedCourseModel> GetLatestCompletedCourseAsync(string userEmail)
        {
            var response = await GetResponseAsync<GetLatestCompletedCourseModel>(GetLatestCompletedCourseUrl(userEmail));

            return response;
        }

        public async Task<GetLatestCompletedCoursesModel> GetLatestCompletedCoursesAsync(List<string> userEmails)
        {
            var requestData = new GetLatestCompletedCoursesRequest()
            {
                Users = userEmails
            };

            var response = await PostResponseAsync<GetLatestCompletedCoursesModel>(JsonConvert.SerializeObject(requestData), GetLatestCompletedCoursesUrl());

            return response;
        }

        private string GetLatestCompletedCoursesUrl()
        {
            return $"{GetLastCompletedCoursesFunctionUrl}";
        }

        private string GetLatestCompletedCourseUrl(string userEmail)
        {
            return $"{GetLastCompletedCourseFunctionUrl}&email={userEmail}";
        }
    }
}
