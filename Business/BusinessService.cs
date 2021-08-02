using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ElearningData;
using Intercom;
using Intercom.Models;
using Kontent;

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
        private readonly string IntercomLatestCompletedCourseAttribute = "elearning_latest_completed_course";

        public BusinessService(IElearningDataService elearningDataService, IKontentService kontentService, IIntercomService intercomService)
        {
            ElearningDataService = elearningDataService;
            KontentService = kontentService;
            IntercomService = intercomService;
        }


        public async Task<List<IntercomContact>> SetIntercomContactElearningAttributesAsync(string testIntercomContactId = null)
        {
            var isTest = !string.IsNullOrEmpty(testIntercomContactId);

            var processedContacts = new List<IntercomContact>();
            var intercomContacts = new List<IntercomContact>();

            if (isTest)
            {
                // get only test contact
                intercomContacts.Add(await IntercomService.GetContactAsync(testIntercomContactId));
            }
            else
            {
                // get contacts from intercom
                intercomContacts.AddRange(await IntercomService.GetAllContactsAsync());
            }

            foreach (var contact in intercomContacts)
            {
                // check if user has access to e-learning
                if (!ContactHasAccessToElearning(contact, isTest))
                {
                    continue;
                }

                // get latest completed course for given user
                var latestCompletedCourseResult = await ElearningDataService.GetLatestCompletedCourseAsync(contact.Email);

                if (string.IsNullOrEmpty(latestCompletedCourseResult.CourseId))
                {
                    // user does not have any completed courses
                    continue;
                }

                // get next course in path
                var nextCourseInPathResult = await KontentService.GetNextTrainingCourseByTalentLmsId(latestCompletedCourseResult.CourseId);

                if (nextCourseInPathResult == null)
                {
                    // invalid result
                    continue;
                }

                var nextCourseInPath = nextCourseInPathResult.NextCourseInPath;
                var latestCompletedCourse = nextCourseInPathResult.LatestCompletedCourse;

                var synchronizedTimestamp = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

                // update user in intercom with next course in learning path
                var updatedContact = IntercomService.UpdateContactAsync(contact,
                    new List<UpdateContactCustomAttributeData>()
                    {
                        new UpdateContactCustomAttributeData(IntercomElearningLastSynchronizedAttribute, synchronizedTimestamp),
                        new UpdateContactCustomAttributeData(IntercomCourseToTakeAttribute, nextCourseInPath.Title),
                        new UpdateContactCustomAttributeData(IntercomLatestCompletedCourseAttribute, latestCompletedCourse.Title),
                    });

                processedContacts.Add(contact);
            }

            return processedContacts;
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
