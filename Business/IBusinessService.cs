using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Models;
using Intercom.Models;

namespace Business
{
    public interface IBusinessService
    {
        public Task<SynchronizationResult> SetIntercomContactElearningAttributesAsync(string testIntercomContactId = null);
    }
}
