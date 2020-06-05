using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EA.Usage.Tracking.Client
{
    public static class UsageTrackingConfiguration
    {
        private static readonly UsageTrackingSettings usageTrackingSettings = new UsageTrackingSettings();
        private static UsageTrackingOptions usageTrackingOptions = new UsageTrackingOptions();

        static UsageTrackingConfiguration() =>
            UsageTrackingConfigurationBuilder.Build().GetSection("UsageTracking").Bind(usageTrackingSettings);

        public static void ConfigureUsageTracking(this IServiceCollection services, Action<UsageTrackingOptions> options = null)
        {
            options?.Invoke(usageTrackingOptions);

            if(usageTrackingOptions.UseHostedUI)
                services.ConfigureCognitoHostedUI();
            else
                services.ConfigureCognitoForUsers();

            services.ConfigureCognitoForApp();

            services.AddHttpClient("usageTracking", c =>
            {
                c.BaseAddress = new Uri(usageTrackingSettings.BaseAddress);
            });

            services.AddScoped<IUsageTrackingClient, UsageTrackingClient>();
        }
    }
}
