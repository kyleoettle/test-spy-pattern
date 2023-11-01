using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Test.Spy.Pattern
{
    public interface IDemoClient
    {
        Task<WorldTime> GetWorldTimeFromIP();
    }

    public class DemoClient : IDemoClient
    {
        private readonly ILogger<DemoClient> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public DemoClient(ILogger<DemoClient> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<WorldTime> GetWorldTimeFromIP()
        {
            var client = httpClientFactory.CreateClient("DemoClient");
            var response = await client.GetAsync("http://worldtimeapi.org/api/ip");
            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<WorldTime>(await response.Content.ReadAsStringAsync());

            logger.LogError("Failed to get WorldTime from IP. {StatusCode}: {Message}", response.StatusCode, await response.Content.ReadAsStringAsync());
            return null;


        }
    }
}