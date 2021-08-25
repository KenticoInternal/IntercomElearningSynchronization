using System;
using System.Collections.Generic;
using System.Text;
using Intercom.Models;

namespace Business.Models
{
    public class SynchronizationResult
    {
        public List<IntercomContactSynchronizationResult> UsersWithNextCourseInPath { get; set; }
        public List<IntercomContactSynchronizationResult> UsersWithoutAccessToElearning { get; set; }
        public List<IntercomContactSynchronizationResult> UsersWithoutCompletedCourses { get; set; }
        public List<IntercomContactSynchronizationResult> UsersWithCourseButNoNextInPathCourses { get; set; }

    }
}
