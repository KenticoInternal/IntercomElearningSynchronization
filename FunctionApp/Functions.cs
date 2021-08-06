using System;
using System.Threading.Tasks;
using System.Web.Http;
using Business;
using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp
{
    public class Functions
    {
        private IBusinessService BusinessService { get; }

        private readonly string _testContactIdQueryStringName = "testContactId";

        public Functions(IBusinessService businessService)
        {
            BusinessService = businessService;
        }

        [FunctionName("SynchronizeDataTest")]
        public async Task<IActionResult> SynchronizeDataTestAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Start '{nameof(SynchronizeDataTestAsync)}'");

                string testContactId = null;
                if (req.Query.ContainsKey(_testContactIdQueryStringName))
                {
                    testContactId = req.Query[_testContactIdQueryStringName].ToString();
                    log.LogInformation($"Testing with contact id '{testContactId}'");
                }

                if (string.IsNullOrEmpty(testContactId))
                {
                    throw new NotSupportedException($"Please provide query string '{_testContactIdQueryStringName}' with intercom contact id.");

                }

                var result = await BusinessService.SetIntercomContactElearningAttributesAsync(testContactId);

                LogResultMessages(log, result);

                return new JsonResult(GetLogResultsObject(result));

            }
            catch (Exception ex)
            {
                var errorMsg = $"Could not synchronize intercom data: {ex}";
                log.LogError(ex, errorMsg);

                return new BadRequestErrorMessageResult(errorMsg);
            }

        }

        [FunctionName("SynchronizeData")]
        public async Task<IActionResult> SynchronizeDataAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] 
            HttpRequest req, 
            ILogger log) {

            try
            {
                log.LogInformation($"Start '{nameof(SynchronizeDataAsync)}'");

                var result = await BusinessService.SetIntercomContactElearningAttributesAsync();

                LogResultMessages(log, result);

                log.LogInformation($"End '{nameof(SynchronizeDataAsync)}'.");

                return new JsonResult(GetLogResultsObject(result));

            }
            catch (Exception ex)
            {
                var errorMsg = $"Could not synchronize intercom data: {ex}";
                log.LogError(ex, errorMsg);

                return new BadRequestErrorMessageResult(errorMsg);
            }
          
        }

        private SynchronizeFunctionResult GetLogResultsObject(SynchronizationResult result)
        {
            return new SynchronizeFunctionResult()
            {
                UsersWithoutCompletedCourses = result.UsersWithoutCompletedCourses.Count,
                UsersWithCourseButNoNextInPathCourses = result.UsersWithCourseButNoNextInPathCourses.Count,
                UsersWithoutAccessToElearning = result.UsersWithoutAccessToElearning.Count,
                UsersWithNextCourseInPath = result.UsersWithNextCourseInPath.Count
            };
        }

        private void LogResultMessages(ILogger log, SynchronizationResult result)
        {
            log.LogInformation($"Users with next course: {result.UsersWithNextCourseInPath.Count}");
            log.LogInformation($"Users without any completed courses: {result.UsersWithoutCompletedCourses.Count}");
            log.LogInformation($"Users with no available next in path course: {result.UsersWithCourseButNoNextInPathCourses.Count}");
            log.LogInformation($"Users without access to elearning: {result.UsersWithoutAccessToElearning.Count}");
        }

        public class SynchronizeFunctionResult
        {
            [JsonProperty("Users with next course")]
            public int UsersWithNextCourseInPath { get; set; }

            [JsonProperty("Users without any completed courses")]
            public int UsersWithoutCompletedCourses { get; set; }

            [JsonProperty("Users with no available next in path course")]
            public int UsersWithCourseButNoNextInPathCourses { get; set; }

            [JsonProperty("Users without access to elearning")]
            public int UsersWithoutAccessToElearning { get; set; }
        }
    }
}
