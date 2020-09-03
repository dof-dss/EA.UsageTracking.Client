using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EA.Usage.Tracking.Client
{
    public class UsageTrackingOptions
    {
        public bool UseHostedUI { get; set; }
        public HttpClientHandler HttpClientHandler { get; set; }
    }
}
