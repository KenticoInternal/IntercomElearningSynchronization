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

        public async Task<List<CompletedUserCoursesModel>> GetLatestCompletedCoursesAsync(List<string> userEmails)
        {
            var result = new List<CompletedUserCoursesModel>();

            foreach (var userEmail in userEmails)
            {
                var items = await Container.GetItemLinqQueryable<ReportingItemModel>()
                    .Where(m => m.PartitionKey == userEmail && m.Status == CompletedStatusIdentifier && m.CompletedDate != null)
                    .OrderByDescending(m => m.CompletedDate)
                    .ToFeedIterator()
                    .ReadNextAsync();


                result.Add(new CompletedUserCoursesModel()
                {
                    Email = userEmail,
                    CompletedCourses = items
                        .Where(m => m.CompletedDate != null)
                        .Select(m => new CompletedUserCourseModel()
                    {
                        CompletedUtc = m.CompletedDate.Value,
                        CourseId = m.CourseId,
                    }).ToList()
                });
            }

            return result;
        }

    }
}
