using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Intercom;
using Intercom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp
{
    public class SynchronizeData
    {
        private IIntercomService IntercomService { get; }

        public SynchronizeData(IIntercomService intercomService)
        {
            IntercomService = intercomService;
        }

        [FunctionName("SynchronizeData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var contact = await IntercomService.GetContactAsync("60e6d1be725a222954ae42a5");

            var updatedContact = await IntercomService.UpdateContactAsync(contact,
                new List<UpdateContactCustomAttributeData>()
                {
                    new UpdateContactCustomAttributeData("sign_up_date", "1/10/2021")
                });

            // var intercomContacts = await IntercomService.GetAllContactsAsync();}}}

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
