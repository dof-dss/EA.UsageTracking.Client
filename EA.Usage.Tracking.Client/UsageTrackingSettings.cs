namespace EA.Usage.Tracking.Client
{
    public class UsageTrackingSettings
    {
        public string BaseAddress { get; set; }
        public AwsSettings AwsSettings { get; set; } = new AwsSettings();
        public CognitoSettings CognitoSettings { get; set; } = new CognitoSettings();
    }
}