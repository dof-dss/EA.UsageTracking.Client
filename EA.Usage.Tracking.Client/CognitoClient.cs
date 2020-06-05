using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EA.Usage.Tracking.Client
{
    public interface IAuthClient
    {
        Task<CognitoResponse> GetAccessToken();
        Task<AuthenticationHeaderValue> GetAuthenticationHeader();
    }

    public class CognitoClient : IAuthClient
    {
        private readonly HttpClient _httpClient;

        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<CognitoSettings> _cognitoSettings;
        private readonly HttpRequestMessage _request;

        public CognitoClient(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IOptions<CognitoSettings> cognitoSettings)
        {
            _httpClient = httpClientFactory.CreateClient("cognito");
            _memoryCache = memoryCache;
            _cognitoSettings = cognitoSettings;

            _request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>())
        };
        }

        public async Task<CognitoResponse> GetAccessToken()
        {
            if (_memoryCache.TryGetValue(_cognitoSettings.Value.ClientId, out CognitoResponse result) && result.IsValid)
                return result;

            var response = await _httpClient.SendAsync(_request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<CognitoResponse>(responseString);
                _memoryCache.Set(_cognitoSettings.Value.ClientId, result);
            }

            return result;
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeader()
        {
            var cognitoResponse = await GetAccessToken();
            return new AuthenticationHeaderValue("Bearer", cognitoResponse.AccessToken);
        }

    }
}
