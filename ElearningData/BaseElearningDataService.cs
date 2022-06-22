using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace ElearningData
{
    public abstract class BaseElearningDataService
    {
        private string ElearningDbConnectionString => Environment.GetEnvironmentVariable("ElearningDbConnectionString");
        private string ElearningDbContainerId => Environment.GetEnvironmentVariable("ElearningDbContainerId");
        private string ElearningDbId => Environment.GetEnvironmentVariable("ElearningDbId");

        private CosmosClient Client { get; }

        protected Container Container { get; }

        protected BaseElearningDataService()
        {
            Client = new CosmosClient(ElearningDbConnectionString, new CosmosClientOptions()
            {
                MaxRetryAttemptsOnRateLimitedRequests = 5,
            });

            Container = Client.GetContainer(ElearningDbId, ElearningDbContainerId);
        }

    }
}
