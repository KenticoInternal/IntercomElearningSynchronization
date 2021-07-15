using System.Threading.Tasks;
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

        public Functions(IBusinessService businessService)
        {
            BusinessService = businessService;
        }

        [FunctionName("SynchronizeData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var updatedContacts = await BusinessService.SetIntercomContactElearningAttributesAsync();


            return new OkObjectResult($"Processed '{updatedContacts.Count}' contacts");
        }
    }
}
