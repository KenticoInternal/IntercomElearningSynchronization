using System;
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

        public async Task<TrainingCourseModel> GetTrainingCourseByTalentLmsId(string id)
        {
            var trainingCourses = await GetDeliveryClient().GetItemsAsync<TrainingCourseModel>(
                new DepthParameter(2),
                new EqualsFilter("elements.talentlms_course_id", id));

            if (!trainingCourses.Items.Any())
            {
                return null;
            }

            if (trainingCourses.Items.Count > 1)
            {
                throw new NotSupportedException($"There are multiple training courses with talent lms id '{id}'");
            }

            return trainingCourses.Items.First();
        }

        public async Task<NextTrainingCourseResult> GetNextTrainingCourseByTalentLmsId(string id)
        {
            var course = await GetTrainingCourseByTalentLmsId(id);

            var nextInPath = course?.NextInPath.FirstOrDefault();

            return new NextTrainingCourseResult()
            {
                LatestCompletedCourse = course,
                NextCourseInPath = nextInPath
            };
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
