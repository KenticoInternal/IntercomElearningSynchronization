using System.Collections.Generic;
using Intercom.Models;

namespace Business.Models
{
    public class IntercomContactSynchronizationResult
    {
        public IntercomContact Contact { get; }
        public List<UpdateContactCustomAttributeData> UpdatedAttributes { get; }
        public string NextCourse { get; }
        public string LatestCourse { get; }

        public IntercomContactSynchronizationResult(IntercomContact contact)
        {
            Contact = contact;
        }

        public IntercomContactSynchronizationResult(IntercomContact contact, List<UpdateContactCustomAttributeData> updatedAttributes, string nextCourse, string latestCourse)
        {
            Contact = contact;
            NextCourse = nextCourse;
            LatestCourse = latestCourse;
            UpdatedAttributes = updatedAttributes;
        }
    }
}
