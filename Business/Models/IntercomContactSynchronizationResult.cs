using Intercom.Models;

namespace Business.Models
{
    public class IntercomContactSynchronizationResult
    {
        public IntercomContact Contact { get; }
        public string NextCourse { get; }

        public IntercomContactSynchronizationResult(IntercomContact contact)
        {
            Contact = contact;
        }

        public IntercomContactSynchronizationResult(IntercomContact contact, string nextCourse)
        {
            Contact = contact;
            NextCourse = nextCourse;
        }
    }
}
