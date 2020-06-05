using System;
using System.Collections.Generic;
using System.Text;

namespace EA.Usage.Tracking.Client
{
    public partial class AwsSettings
    {
        public string Region { get; set; }
        public string UserPoolClientId { get; set; }
        public string UserPoolClientSecret { get; set; }
        public string UserPoolId { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
