using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace EA.Usage.Tracking.Client
{
    public class CognitoResponse
    {
        private DateTime _dateCreated;

        public CognitoResponse()
        {
            _dateCreated = DateTime.Now;
        }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        public bool IsValid => _dateCreated.AddSeconds(ExpiresIn) > DateTime.Now;
    }
}
