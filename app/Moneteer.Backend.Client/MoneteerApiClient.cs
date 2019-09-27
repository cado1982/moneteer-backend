using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Moneteer.Backend.Client
{
    public class MoneteerApiClient
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public MoneteerApiClient(string apiBaseUrl, HttpClient httpClient)
        {
            _apiBaseUrl = apiBaseUrl;
            _httpClient = httpClient;
        }

        public Task<List<Budget>> GetBudgetsAsync(string accessToken)
        {
            return Get<List<Budget>>("budget", accessToken);
        }

        public Task<List<Transaction>> GetTransactionsAsync(Guid budgetId, string accessToken)
        {
            return Get<List<Transaction>>($"budget/{budgetId}/transactions", accessToken);
        }

        public Task<List<Account>> GetAccountsAsync(Guid budgetId, string accessToken)
        {
            return Get<List<Account>>($"budget/{budgetId}/accounts", accessToken);
        }
        
        public Task<List<Envelope>> GetEnvelopesAsync(Guid budgetId, string accessToken)
        {
            return Get<List<Envelope>>($"budget/{budgetId}/envelopes", accessToken);
        }

        public Task<List<EnvelopeCategory>> GetEnvelopeCategoriesAsync(Guid budgetId, string accessToken)
        {
            return Get<List<EnvelopeCategory>>($"budget/{budgetId}/envelopes/categories", accessToken);
        }

        private async Task<T> Get<T>(string route, string accessToken)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, _apiBaseUrl + route))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsAsync<T>();
                    return result;
                }
            }
        }

        private Task Post<T>(string route, T payload, string accessToken)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, _apiBaseUrl + route))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                if (payload != null)
                {
                    string content;
                    if (payload is string)
                    {
                        content = payload.ToString();
                    }
                    else
                    {
                        content = JsonConvert.SerializeObject(payload);
                    }
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                }
                
                return _httpClient.SendAsync(request);
            }
        }
    }
}
