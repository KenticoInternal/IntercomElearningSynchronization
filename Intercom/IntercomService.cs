using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Intercom.Models;

namespace Intercom
{
    public class IntercomService : BaseIntercomService, IIntercomService
    {

        private readonly int ContactsPerPageCount = 150;

        public IntercomService(IHttpClientFactory clientFactory, string apiKey) : base(clientFactory, apiKey) {}

        public async Task<List<IntercomContact>> GetAllContactsAsync()
        {
            var allContacts = new List<IntercomContact>();
            var iterate = true;
            var startingAfter = string.Empty;

            while (iterate)
            {
                var response = await GetContactsAsync(startingAfter);

                allContacts.AddRange(response.Data);

                if (!string.IsNullOrEmpty(response.Pages.Next?.StartingAfter))
                {
                    startingAfter = response.Pages.Next.StartingAfter;
                }
                else
                {
                    iterate = false;
                }
            }

            return allContacts;
        }

        private async Task<IntercomListContactsResponse> GetContactsAsync(string startingAfter)
        {
            var response = await GetResponseAsync<IntercomListContactsResponse>($"https://api.intercom.io/contacts?per_page={ContactsPerPageCount}&starting_after={startingAfter}");

            return response;
        }
    }
}
