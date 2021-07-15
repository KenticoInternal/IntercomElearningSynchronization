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

        public BusinessService(IElearningDataService elearningDataService, IKontentService kontentService, IIntercomService intercomService)
        {
            ElearningDataService = elearningDataService;
            KontentService = kontentService;
            IntercomService = intercomService;
        }


        public async Task<List<IntercomContact>> SetIntercomContactElearningAttributesAsync()
        {
            // var intercomContacts = await IntercomService.GetAllContactsAsync();
            var processedContacts = new List<IntercomContact>();

            var intercomContacts = new List<IntercomContact>()
            {
                await IntercomService.GetContactAsync("60e6d1be725a222954ae42a5")
            };

            foreach (var contact in intercomContacts)
            {
                // check if user has access to e-learning
                if (!ContactHasAccessToElearning(contact))
                {
                    continue;
                }

                // get latest completed course for given user
                var latestCompletedCourse = await ElearningDataService.GetLatestCompletedCourseAsync("janc@kentico.com");

                if (string.IsNullOrEmpty(latestCompletedCourse.CourseId))
                {
                    // user does not have any completed courses
                    continue;
                }

                // get next course in path
                // var nextCourseInPath = await KontentService.GetNextTrainingCourseByTalentLmsId(latestCompletedCourse.CourseId);
                var nextCourseInPath = await KontentService.GetNextTrainingCourseByTalentLmsId(195.ToString());

                if (nextCourseInPath == null)
                {
                    // no next course in path
                    continue;
                }

                // update user in intercom with next course in learning path
                var updatedContact = IntercomService.UpdateContactAsync(contact,
                    new List<UpdateContactCustomAttributeData>()
                    {
                        new UpdateContactCustomAttributeData("course_to_take", nextCourseInPath.Title)
                    });

                processedContacts.Add(contact);
            }

            return processedContacts;
        }

        private bool ContactHasAccessToElearning(IntercomContact contact)
        {
            return true;
        }
    }
}
