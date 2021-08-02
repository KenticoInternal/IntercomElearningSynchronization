using System;
using System.Threading.Tasks;
using System.Web.Http;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

                var updatedContacts = await BusinessService.SetIntercomContactElearningAttributesAsync(testContactId);

                log.LogInformation($"End '{nameof(SynchronizeDataTestAsync)}'. Synchronized '{updatedContacts.Count}' contacts");

                return new OkObjectResult($"Synchronized '{updatedContacts.Count}' contacts");

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

                var updatedContacts = await BusinessService.SetIntercomContactElearningAttributesAsync();

                log.LogInformation($"End '{nameof(SynchronizeDataAsync)}'. Synchronized '{updatedContacts.Count}' contacts");

                return new OkObjectResult($"Synchronized '{updatedContacts.Count}' contacts");

            }
            catch (Exception ex)
            {
                var errorMsg = $"Could not synchronize intercom data: {ex}";
                log.LogError(ex, errorMsg);

                return new BadRequestErrorMessageResult(errorMsg);
            }
          
        }
    }
}
