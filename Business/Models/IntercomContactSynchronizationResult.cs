using Intercom.Models;

namespace Business.Models
{
    public class IntercomContactSynchronizationResult
    {
        public IntercomContact Contact { get; }
        public string NextCourse { get; }
        public string LatestCourse { get; }

        public IntercomContactSynchronizationResult(IntercomContact contact)
        {
            Contact = contact;
        }

        public IntercomContactSynchronizationResult(IntercomContact contact, string nextCourse, string latestCourse)
        {
            Contact = contact;
            NextCourse = nextCourse;
            LatestCourse = latestCourse;
        }
    }
}
