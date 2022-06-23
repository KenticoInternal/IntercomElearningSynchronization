using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.Builders.DeliveryClient;
using Kentico.Kontent.Delivery.Urls.QueryParameters;
using Kentico.Kontent.Delivery.Urls.QueryParameters.Filters;
using Kontent.Models;

namespace Kontent
{
    public class KontentService : IKontentService
    {
        private string KontentProjectId => Environment.GetEnvironmentVariable("KontentProjectId");
        private string KontentSecureApiKey => Environment.GetEnvironmentVariable("KontentSecureApiKey");

        public KontentService()
        {
        }

        public async Task<List<TrainingCourseModel>> GetTrainingCoursesByIds(List<string> ids)
        {
            var trainingCourses = await GetDeliveryClient().GetItemsAsync<TrainingCourseModel>(
                new DepthParameter(1),
                new ElementsParameter(TrainingCourseModel.DescriptionCodename, TrainingCourseModel.NextInPathCodename, TrainingCourseModel.TitleCodename, TrainingCourseModel.UrlCodename),
                new InFilter("system.id", ids.ToArray()));

            if (!trainingCourses.Items.Any())
            {
                return new List<TrainingCourseModel>();
            }

            return trainingCourses.Items.ToList();
        }

        private IDeliveryClient GetDeliveryClient()
        {
            return DeliveryClientBuilder
                .WithOptions(options => options.WithProjectId(KontentProjectId)
                    .UseProductionApi(KontentSecureApiKey)
                    .Build())
                .WithTypeProvider(new CustomTypeProvider())
                .Build();

        }

    }
}
