using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EA.Usage.Tracking.Client
{
    public interface IUsageTrackingClient
    {
        Task Post(string idToken, int eventId);
        Task<PagedResponse<UsageTrackingEvent>> GetEvents();
    }

    public class UsageTrackingClient : IUsageTrackingClient
    {
        private HttpClient _httpClient;
        private IAuthClient _authClient;

        public UsageTrackingClient(IHttpClientFactory httpClientFactory, IAuthClient authClient)
        {
            _authClient = authClient;
            _httpClient = httpClientFactory.CreateClient("usageTracking");
        }

        public async Task Post(string idToken, int eventId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _authClient.GetAuthenticationHeader();

            var payload = new {ApplicationEventId = eventId, IdentityToken = idToken};
            var response = await _httpClient.PostAsync("ApplicationUsage", 
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
        }

        public async Task<PagedResponse<UsageTrackingEvent>> GetEvents()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _authClient.GetAuthenticationHeader();
            var response = await _httpClient.GetAsync("ApplicationEvent");

            return response.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<PagedResponse<UsageTrackingEvent>>(
                    await response.Content.ReadAsStringAsync())
                : new PagedResponse<UsageTrackingEvent>();

        }
    }
}
