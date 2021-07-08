using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intercom.Models;

namespace Intercom
{
    public interface IIntercomService
    {
        Task<List<IntercomContact>> GetAllContactsAsync();
    }
}
