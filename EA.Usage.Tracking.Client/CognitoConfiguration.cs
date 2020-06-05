using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EA.Usage.Tracking.Client
{
    public static class CognitoConfiguration
    {
        private static readonly UsageTrackingSettings usageTrackingSettings = new UsageTrackingSettings();
        private static IConfiguration _configuration;

        static CognitoConfiguration()
        {
            _configuration = UsageTrackingConfigurationBuilder.Build();
            _configuration.GetSection("UsageTracking").Bind(usageTrackingSettings);
            _configuration.GetSection("UsageTracking").GetSection("Cognito").Bind(usageTrackingSettings.CognitoSettings);
            _configuration.GetSection("UsageTracking").GetSection("AWS").Bind(usageTrackingSettings.AwsSettings);
        }

        public static void ConfigureCognitoForApp(this IServiceCollection services)
        {
            services.AddHttpClient("cognito", c =>
            {
                c.DefaultRequestHeaders.Add("Authorization", usageTrackingSettings.CognitoSettings.BuildBasicAuthorizationHeader());
                c.BaseAddress = new Uri(usageTrackingSettings.CognitoSettings.ClientCredentialsUrl);
            });

            services.Configure<CognitoSettings>(_configuration.GetSection("UsageTracking").GetSection("Cognito"));
            services.AddScoped<IAuthClient, CognitoClient>();
        }

        public static void ConfigureCognitoForUsers(this IServiceCollection services)
        {
            var cognitoIdentityProvider =
                new AmazonCognitoIdentityProviderClient(new BasicAWSCredentials(usageTrackingSettings.AwsSettings.AccessKey, usageTrackingSettings.AwsSettings.SecretKey), RegionEndpoint.EUWest2);
            var cognitoUserPool = new CognitoUserPool(usageTrackingSettings.AwsSettings.UserPoolId,
                usageTrackingSettings.AwsSettings.UserPoolClientId,
                cognitoIdentityProvider,
                usageTrackingSettings.AwsSettings.UserPoolClientSecret);

            services.AddSingleton<IAmazonCognitoIdentityProvider>(cognitoIdentityProvider);
            services.AddSingleton<CognitoUserPool>(cognitoUserPool);
            services.AddCognitoIdentity();
        }

        public static void ConfigureCognitoHostedUI(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ResponseType = "code";
                    options.MetadataAddress = usageTrackingSettings.CognitoSettings.MetaDataAddress;
                    options.ClientId = usageTrackingSettings.CognitoSettings.HostedUIClientId;
                    options.SaveTokens = true;

                    options.Events.OnRedirectToIdentityProvider = async context =>
                    {
                        context.ProtocolMessage.RedirectUri = $"https://{context.HttpContext.Request.Host.Value}/signin-oidc";
                        await Task.FromResult(0);
                    };

                });
        }
    }
}
