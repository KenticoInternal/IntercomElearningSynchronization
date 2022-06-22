using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ElearningData.Models;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;

namespace ElearningData
{
    public class ElearningDataService : BaseElearningDataService, IElearningDataService
    {
        private readonly string CompletedStatusIdentifier = "COMPLETED";


        public ElearningDataService() : base() { }

        public async Task<GetLatestCompletedCoursesModel> GetLatestCompletedCoursesAsync(List<string> userEmails)
        {
            var reportingItems = new List<GetLatestCompletedUserCourseModel>();

            foreach (var userEmail in userEmails)
            {
                var items = await Container.GetItemLinqQueryable<ReportingItemModel>()
                    .Where(m => m.PartitionKey == userEmail && m.Status == CompletedStatusIdentifier && m.CompletedDate != null)
                    .OrderByDescending(m => m.CompletedDate)
                    .Take(1)
                    .ToFeedIterator()
                    .ReadNextAsync();

                var item = items.FirstOrDefault();

                if (item != null && item.CompletedDate != null)
                {
                    reportingItems.Add(new GetLatestCompletedUserCourseModel()
                    {
                        CompletedUtc = item.CompletedDate.Value,
                        CourseId = item.CourseId,
                        Email = item.PartitionKey
                    });
                }
            }

            return new GetLatestCompletedCoursesModel()
            {
                Users = reportingItems
            };
        }

    }
}
