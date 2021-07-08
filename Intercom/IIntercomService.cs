using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intercom.Models;

namespace Intercom
{
    public interface IIntercomService
    {
        Task<List<IntercomContact>> GetAllContactsAsync();

        Task<IntercomContact> GetContactAsync(string id);

        Task<IntercomContact> UpdateContactAsync(IntercomContact contact,
            List<UpdateContactCustomAttributeData> attributes);

    }
}
