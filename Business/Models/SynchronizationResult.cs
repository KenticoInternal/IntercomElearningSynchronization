using System;
using System.Collections.Generic;
using System.Text;
using Intercom.Models;

namespace Business.Models
{
    public class SynchronizationResult
    {
        public List<IntercomContact> UsersWithNextCourseInPath { get; set; }
        public List<IntercomContact> UsersWithoutAccessToElearning { get; set; }
        public List<IntercomContact> UsersWithoutCompletedCourses { get; set; }
        public List<IntercomContact> UsersWithCourseButNoNextInPathCourses { get; set; }

    }
}
