using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Intercom.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intercom
{
    public class IntercomService : BaseIntercomService, IIntercomService
    {

        private readonly int ContactsPerPageCount = 150;

        public IntercomService(IHttpClientFactory clientFactory, string apiKey) : base(clientFactory, apiKey) {}

        public async Task<IntercomContact> UpdateContactAsync(IntercomContact contact, List<UpdateContactCustomAttributeData> attributes)
        {
            var requestBody = new JObject {{"email", contact.Email}, {"role", contact.Role}};

            var customAttributes = new JObject();

            foreach (var attribute in attributes)
            {
                customAttributes.Add(attribute.AttributeName, attribute.AttributeValue);
            }

            requestBody.Add("custom_attributes", customAttributes);

            var response = await PostResponseAsync<IntercomContact>(JsonConvert.SerializeObject(requestBody), GetUpdateContactUrl(contact));

            return response;
        }

        public async Task<IntercomContact> GetContactAsync(string id)
        {
            var response = await GetResponseAsync<IntercomContact>(GetRetrieveContactUrl(id));

            return response;
        }

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

        private string GetUpdateContactUrl(IntercomContact contact)
        {
            return $"https://api.intercom.io/contacts/{contact.Id}";
        }

        private string GetRetrieveContactUrl(string id)
        {
            return $"https://api.intercom.io/contacts/{id}";
        }

        private async Task<IntercomListContactsResponse> GetContactsAsync(string startingAfter)
        {
            var response = await GetResponseAsync<IntercomListContactsResponse>($"https://api.intercom.io/contacts?per_page={ContactsPerPageCount}&starting_after={startingAfter}");

            return response;
        }
    }
}
