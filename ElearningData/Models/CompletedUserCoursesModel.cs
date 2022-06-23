using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElearningData.Models
{
    public class CompletedUserCoursesModel
    {

        public string Email { get; set; }
        public List<CompletedUserCourseModel> CompletedCourses { get; set; }

    }

    public class CompletedUserCourseModel
    {
        public string CourseId { get; set; }

        public DateTime CompletedUtc { get; set; }

    }
}
