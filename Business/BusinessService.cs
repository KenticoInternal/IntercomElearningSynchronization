using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Models;
using ElearningData;
using Intercom;
using Intercom.Models;
using Kontent;
using Kontent.Models;

namespace Business
{
    public class BusinessService : IBusinessService
    {
        private IElearningDataService ElearningDataService { get; }
        private IKontentService KontentService { get; }
        private IIntercomService IntercomService { get; }

        private readonly string IntercomElearningLastSynchronizedAttribute = "elearning_last_synchronized";
        private readonly string IntercomSubscriptionAttribute = "subscription-plan";
        private readonly string IntercomCourseToTakeAttribute = "elearning_course_to_take";
        private readonly string IntercomCourseToTakeAttributeUrl = "elearning_course_to_take_url";
        private readonly string IntercomLatestCompletedCourseAttribute = "elearning_latest_completed_course";
        private readonly string IntercomLatestCompletedCourseAttributeUrl = "elearning_latest_completed_course_url";

        public BusinessService(IElearningDataService elearningDataService, IKontentService kontentService, IIntercomService intercomService)
        {
            ElearningDataService = elearningDataService;
            KontentService = kontentService;
            IntercomService = intercomService;
        }


        public async Task<SynchronizationResult> SetIntercomContactElearningAttributesAsync(string testIntercomContactId = null)
        {
            var result = new SynchronizationResult()
            {
                UsersWithNextCourseInPath = new List<IntercomContactSynchronizationResult>(),
                UsersWithoutAccessToElearning = new List<IntercomContactSynchronizationResult>(),
                UsersWithoutCompletedCourses = new List<IntercomContactSynchronizationResult>(),
                UsersWithCourseButNoNextInPathCourses = new List<IntercomContactSynchronizationResult>()
            };

            var isTest = !string.IsNullOrEmpty(testIntercomContactId);

            var intercomContacts = new List<IntercomContact>();

            if (isTest)
            {
                // get only test contact
                intercomContacts.Add(await IntercomService.GetContactAsync(testIntercomContactId));
            }
            else
            {
                // get contacts from intercom
                intercomContacts.AddRange(await IntercomService.GetAllContactsWithSubscriptionAsync());
            }

            // get user -> courses
            var userEmails = intercomContacts.Select(m => m.Email).ToList();
            var userCourses = await ElearningDataService.GetLatestCompletedCoursesAsync(userEmails);

            foreach (var contact in intercomContacts)
            {
                // check if user has access to e-learning
                if (!ContactHasAccessToElearning(contact, isTest))
                {
                    result.UsersWithoutAccessToElearning.Add(new IntercomContactSynchronizationResult(contact));
                    continue;
                }

                // get latest completed course for given user
                var latestCompletedCourseResult = userCourses.Users
                    .FirstOrDefault(m => m.Email.Equals(contact.Email, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(latestCompletedCourseResult?.Course))
                {
                    // user does not have any completed courses
                    result.UsersWithoutCompletedCourses.Add(new IntercomContactSynchronizationResult(contact));
                    continue;
                }

                // get next course in path
                var nextCourseInPathResult = await KontentService.GetNextTrainingCourseByTalentLmsId(latestCompletedCourseResult.Course);

                var nextCourseInPath = nextCourseInPathResult.NextCourseInPath;
                var latestCompletedCourse = nextCourseInPathResult.LatestCompletedCourse;

                var synchronizedTimestamp = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

                // update user in intercom with next course in learning path
                var updatedContact = IntercomService.UpdateContactAsync(contact,
                    new List<UpdateContactCustomAttributeData>()
                    {
                        new UpdateContactCustomAttributeData(IntercomElearningLastSynchronizedAttribute, synchronizedTimestamp),
                        new UpdateContactCustomAttributeData(IntercomCourseToTakeAttribute, nextCourseInPath?.Title),
                        new UpdateContactCustomAttributeData(IntercomCourseToTakeAttributeUrl, nextCourseInPath == null ? null : GetCourseUrl(nextCourseInPath)),
                        new UpdateContactCustomAttributeData(IntercomLatestCompletedCourseAttribute, latestCompletedCourse?.Title),
                        new UpdateContactCustomAttributeData(IntercomLatestCompletedCourseAttributeUrl, latestCompletedCourse == null ? null : GetCourseUrl(latestCompletedCourse)),
                    });

                result.UsersWithNextCourseInPath.Add(new IntercomContactSynchronizationResult(contact, nextCourseInPath?.Title, latestCompletedCourse?.Title));
            }

            return result;
        }

        private string GetCourseUrl(TrainingCourseModel course)
        {
            return $"https://docs.kontent.ai/e-learning/{course.Url}";
        }

        private bool ContactHasAccessToElearning(IntercomContact contact, bool isTest)
        {
            if (!contact.CustomAttributes.ContainsKey(IntercomSubscriptionAttribute))
            {
                return false;
            }

            var subscriptionValue = contact.CustomAttributes[IntercomSubscriptionAttribute]?.ToString();

            if (string.IsNullOrEmpty(subscriptionValue))
            {
                return false;
            }

            // user has access to subscription if subscription is not empty
            return true;
        }
    }
}
