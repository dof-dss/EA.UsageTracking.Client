using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Configuration;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace EA.Usage.Tracking.Client.Test
{
    public class UsageTrackingClientShouldBeAbleTo
    {
        private static WireMockServer stub;
        private static string baseUrl;
        private ServiceProvider _servicesProvider;

        [OneTimeSetUp]
        public static void PrepareClass()
        {
            var port = new Random().Next(5000, 6000);

            baseUrl = "http://localhost:" + port;

            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port },
                ReadStaticMappings = true
            });
        }


        [OneTimeTearDown]
        public static void TearDown()
        {
            stub.Stop();
        }

        [SetUp]
        public async Task Setup()
        {
            var services = new ServiceCollection();

            string eventsJson;
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var eventsStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("EA.Usage.Tracking.Client.Test.Events.json");
            using (var reader = new StreamReader(eventsStream, Encoding.UTF8))
                eventsJson = await reader.ReadToEndAsync();

            var events = new PagedResponse<UsageTrackingEvent>()
            {
                Data = new List<UsageTrackingEvent>()
                    {new UsageTrackingEvent() {Id = 1, Name = "Test", DateCreated = DateTime.Now}}
            };

            services.AddHttpClient("usageTracking", c =>
            {
                c.BaseAddress = new Uri(baseUrl);
            });

            services.AddScoped<IUsageTrackingClient, UsageTrackingClient>();

            stub.Given(
                    Request
                        .Create()
                        .WithPath("/ApplicationEvent"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(events));

            stub.Given(
                    Request
                        .Create()
                        .WithPath("/ApplicationUsage"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(201));

            var mockAuthClient = new Mock<IAuthClient>();
            mockAuthClient.Setup(x => x.GetAuthenticationHeader())
                .Returns(Task.FromResult(new AuthenticationHeaderValue("test")));

            services.AddScoped<IAuthClient>(c => mockAuthClient.Object);

            _servicesProvider = services.BuildServiceProvider();
        }

        [Test]
        public async Task GetEvents()
        {
            var usageTrackingClient = _servicesProvider.GetRequiredService<IUsageTrackingClient>();

            var events = await usageTrackingClient.GetEvents();

            Assert.IsTrue(events.Data.Count() == 1);
        }

        [Test]
        public async Task PostUsageTrackingEvent()
        {
            var usageTrackingClient = _servicesProvider.GetRequiredService<IUsageTrackingClient>();

            await usageTrackingClient.Post("idToken", 1);

            Assert.Pass();
        }
    }
}