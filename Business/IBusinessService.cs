using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intercom.Models;

namespace Business
{
    public interface IBusinessService
    {
        public Task<List<IntercomContact>> SetIntercomContactElearningAttributesAsync();
    }
}
