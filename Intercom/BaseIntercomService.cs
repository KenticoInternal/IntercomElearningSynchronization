using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Intercom
{
    public abstract class BaseIntercomService
    {
        private string ApiKey { get; }
        private IHttpClientFactory ClientFactory { get; }

        private readonly int RetryAttempts = 5;
        private readonly int RetryAttemptDelayMs = 1000;

        protected BaseIntercomService(IHttpClientFactory clientFactory, string apiKey)
        {
            ApiKey = apiKey;
            ClientFactory = clientFactory;
        }

        protected async Task<T> PutResponseAsync<T>(string json, string url) where T : class
        {
            return await PostResponseInternalAsync<T>(HttpMethod.Put, json, url, 0);
        }

        protected async Task<T> PostResponseAsync<T>(string json, string url) where T : class
        {
            return await PostResponseInternalAsync<T>(HttpMethod.Post, json, url, 0);
        }

        protected async Task<T> GetResponseAsync<T>(string url) where T : class
        {
            return await GetResponseInternalAsync<T>(url, 0);
        }

        private async Task<T> PostResponseInternalAsync<T>(HttpMethod method, string json, string url, int attempt) where T : class
        {
            try
            {
                var request = new HttpRequestMessage(method, url);
                request.Headers.Add("Authorization", $"Bearer {ApiKey}");
                request.Headers.Add("Accept", $"application/json");

                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = ClientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<T>(responseString);

                    return data;
                }
                else
                {
                    throw new Exception($"Could not post data to url '{url}'. Message: {response.ReasonPhrase}");
                }
            }
            catch (Exception)
            {
                if (attempt >= RetryAttempts)
                {
                    throw;
                }

                await Task.Delay(RetryAttemptDelayMs);

                return await PostResponseInternalAsync<T>(method, json, url, attempt + 1);
            }
        }


        private async Task<T> GetResponseInternalAsync<T>(string url, int attempt) where T : class
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {ApiKey}");
                request.Headers.Add("Accept", $"application/json");

                var client = ClientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<T>(responseString);

                    return data;
                }
                else
                {
                    throw new Exception($"Could not get response for url '{url}'. Message: {response.ReasonPhrase}");
                }
            }
            catch (Exception)
            {
                if (attempt >= RetryAttempts)
                {
                    throw;
                }

                await Task.Delay(RetryAttemptDelayMs);

                return await GetResponseInternalAsync<T>(url, attempt + 1);
            }
        }
    }
}
