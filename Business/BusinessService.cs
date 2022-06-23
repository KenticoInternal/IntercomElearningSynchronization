using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Models;
using ElearningData;
using ElearningData.Models;
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

        private readonly string IntercomElearningLastSynchronizedAttribute = "elearning_last_synchronized_at";
        private readonly string IntercomSubscriptionAttribute = "subscription-plan";
        private readonly string IntercomCourseToTakeAttribute = "elearning_course_to_take";
        private readonly string IntercomCourseToTakeAttributeUrl = "elearning_course_to_take_url";
        private readonly string IntercomLatestCompletedCourseAttribute = "elearning_latest_completed_course";
        private readonly string IntercomLatestCompletedCourseUrlAttribute = "elearning_latest_completed_course_url";
        private readonly string IntercomLatestCompletedCourseDateAttribute = "elearning_latest_completed_course_at";

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
                UsersWithCompletedCourse = new List<IntercomContactSynchronizationResult>(),
                UsersWithoutAccessToElearning = new List<IntercomContactSynchronizationResult>(),
                UsersWithoutCompletedCourses = new List<IntercomContactSynchronizationResult>(),
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

            if (!intercomContacts.Any())
            {
                return result;
            }

            // get user -> courses
            var userEmails = intercomContacts.Select(m => m.Email).ToList();
            var userCourses = await ElearningDataService.GetLatestCompletedCoursesAsync(userEmails);

            // pre-load courses from kontent
            var courseIds = userCourses.SelectMany(m => m.CompletedCourses.Select(s => s.CourseId)).Distinct().ToList();
            var courses = await KontentService.GetTrainingCoursesByIds(courseIds);

            foreach (var contact in intercomContacts)
            {
                // check if user has access to e-learning
                if (!ContactHasAccessToElearning(contact, isTest))
                {
                    result.UsersWithoutAccessToElearning.Add(new IntercomContactSynchronizationResult(contact));
                    continue;
                }

                // get latest completed course for given user
                var userWithCourses = userCourses
                    .FirstOrDefault(m => m.Email.Equals(contact.Email, StringComparison.OrdinalIgnoreCase));

                if (userWithCourses == null)
                {
                    continue;
                }

                if (!userWithCourses.CompletedCourses.Any())
                {
                    // user haven't completed any courses
                    result.UsersWithoutCompletedCourses.Add(new IntercomContactSynchronizationResult(contact));
                    continue;
                }

                // get next course in path
                var nextCourseInPathResult = GetNextTrainingCourseResult(courses, userWithCourses);

                // latest completed course date
                long? latestCompletedCourseUnixTimeStamp = null;

                if (nextCourseInPathResult.latestCourseCompletedDate != null)
                {
                    latestCompletedCourseUnixTimeStamp = ((DateTimeOffset)nextCourseInPathResult.latestCourseCompletedDate.Value).ToUnixTimeSeconds();
                }

                var nextCourseInPath = nextCourseInPathResult.nextCourse;
                var latestCompletedCourse = nextCourseInPathResult.latestCompletedCourse;

                var synchronizedUnixTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                // update user in intercom with next course in learning path
                var updateAttributes = new List<UpdateContactCustomAttributeData>()
                {
                    new UpdateContactCustomAttributeData(IntercomElearningLastSynchronizedAttribute, synchronizedUnixTimeStamp.ToString()),
                    new UpdateContactCustomAttributeData(IntercomLatestCompletedCourseDateAttribute, latestCompletedCourseUnixTimeStamp?.ToString()),
                    new UpdateContactCustomAttributeData(IntercomCourseToTakeAttribute, nextCourseInPath?.Title),
                    new UpdateContactCustomAttributeData(IntercomCourseToTakeAttributeUrl, nextCourseInPath == null ? null : GetCourseUrl(nextCourseInPath)),
                    new UpdateContactCustomAttributeData(IntercomLatestCompletedCourseAttribute, latestCompletedCourse?.Title),
                    new UpdateContactCustomAttributeData(IntercomLatestCompletedCourseUrlAttribute, latestCompletedCourse == null ? null : GetCourseUrl(latestCompletedCourse)),
                };

                var updatedContact = IntercomService.UpdateContactAsync(contact, updateAttributes);

                result.UsersWithCompletedCourse.Add(new IntercomContactSynchronizationResult(contact, updateAttributes, nextCourseInPath?.Title, latestCompletedCourse?.Title));
            }

            return result;
        }

        private (DateTime? latestCourseCompletedDate, TrainingCourseModel nextCourse, TrainingCourseModel latestCompletedCourse) GetNextTrainingCourseResult(List<TrainingCourseModel> courses, CompletedUserCoursesModel userCourses)
        {
            // order completed courses by date
            var orderedCompletedCourses = userCourses.CompletedCourses.OrderByDescending(m => m.CompletedUtc).ToList();
            var latestCompletedCourseId = orderedCompletedCourses.FirstOrDefault()?.CourseId;
            var latestCourseCompletedDate = orderedCompletedCourses.FirstOrDefault()?.CompletedUtc;
            var latestCompletedCourse = courses.FirstOrDefault(m =>
                m.System.Id.Equals(latestCompletedCourseId, StringComparison.OrdinalIgnoreCase));
            

            foreach (var completedCourse in orderedCompletedCourses)
            {
                var course = courses.FirstOrDefault(m => m.System.Id.Equals(completedCourse.CourseId, StringComparison.OrdinalIgnoreCase));

                var nextInPath = course?.NextInPath.FirstOrDefault();

                if (nextInPath == null)
                {
                    // there is no next course or course was not found
                    continue;
                }

                // check if user completed the next course
                var userCompletedNextCourse = orderedCompletedCourses.FirstOrDefault(m =>
                    m.CourseId.Equals(nextInPath.System.Id, StringComparison.OrdinalIgnoreCase)) != null;

                if (userCompletedNextCourse)
                {
                    // user already completed the next course
                    continue;
                }

                return (latestCourseCompletedDate, nextInPath, latestCompletedCourse);
            }

            return (latestCourseCompletedDate, null, latestCompletedCourse);
        }

        private string GetCourseUrl(TrainingCourseModel course)
        {
            return $"https://kontent.ai/learn/e-learning/{course.Url}";
        }

        private bool ContactHasAccessToElearning(IntercomContact contact, bool isTest)
        {
            if (isTest)
            {
                return true;
            }
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
