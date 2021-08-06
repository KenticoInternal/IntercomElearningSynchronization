using System;
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

        /// <summary>
        /// 150 is maximum pagination value allowed by Intercom
        /// </summary>
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

            var response = await PutResponseAsync<IntercomContact>(JsonConvert.SerializeObject(requestBody), GetUpdateContactUrl(contact));

            return response;
        }

        public async Task<IntercomContact> GetContactAsync(string id)
        {
            var response = await GetResponseAsync<IntercomContact>(GetRetrieveContactUrl(id));

            return response;
        }

        public async Task<List<IntercomContact>> GetAllContactsWithSubscriptionAsync()
        {
            var allContacts = new List<IntercomContact>();
            var iterate = true;
            var startingAfter = string.Empty;

            // get only users there were recently seen - no point in getting all users that might 
            // not be active
            var lastSeenAtThreshold = DateTime.UtcNow.AddDays(-7);
            var lastSeenAtThresholdUnixTimeStamp = ((DateTimeOffset)lastSeenAtThreshold).ToUnixTimeSeconds();

            while (iterate)
            {
                var requestBody = GetSearchRequestBody(lastSeenAtThresholdUnixTimeStamp, startingAfter);
                var response = await SearchContactsAsync(requestBody);

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

        private SearchContactRequest GetSearchRequestBody(long lastSeenThreshold, string startingAfter = null)
        {
            var requestBody = new SearchContactRequest()
            {
                Pagination = new SearchContactPaginationRequest()
                {
                    PerPage = ContactsPerPageCount,
                    StartingAfter = startingAfter
                },
                Query = new SearchContactQueryMultiple()
                {
                    Operator = "AND",
                    Value = new List<SearchContactQueryValue>()
                    {
                        // get only users with some subscription plan
                        new SearchContactQueryValue()
                        {
                            Field = "custom_attributes.subscription-plan",
                            Value = null,
                            Operator = "!="
                        },
                        // do not get users with Kentico plan
                        new SearchContactQueryValue()
                        {
                            Field = "custom_attributes.subscription-plan",
                            Value = "kentico",
                            Operator = "!="
                        },
                        // do not get users with trial plan
                        new SearchContactQueryValue()
                        {
                            Field = "custom_attributes.subscription-plan",
                            Value = "trial",
                            Operator = "!="
                        },
                        // get only users that were recently active
                        new SearchContactQueryValue()
                        {
                            Field = "last_seen_at",
                            Value = lastSeenThreshold.ToString(),
                            Operator = ">"
                        }
                    }
                }
            };

            return requestBody;
        }

        private string GetUpdateContactUrl(IntercomContact contact)
        {
            return $"https://api.intercom.io/contacts/{contact.Id}";
        }

        private string GetRetrieveContactUrl(string id)
        {
            return $"https://api.intercom.io/contacts/{id}";
        }
        private string GetSearchContactsUrl()
        {
            return $"https://api.intercom.io/contacts/search";
        }

        private async Task<IntercomListContactsResponse> SearchContactsAsync(SearchContactRequest search)
        {
            var response = await PostResponseAsync<IntercomListContactsResponse>(JsonConvert.SerializeObject(search),GetSearchContactsUrl());

            return response;
        }

        private async Task<IntercomListContactsResponse> GetContactsAsync(string startingAfter)
        {
            var response = await GetResponseAsync<IntercomListContactsResponse>($"https://api.intercom.io/contacts?per_page={ContactsPerPageCount}&starting_after={startingAfter}");

            return response;
        }
    }
}
