using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeHR.Api.IntegrationTests
{
    public class HealthChecksTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> _factory;

        private readonly HttpClient _httpClient;

        public HealthChecksTests(TestWebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
            this._httpClient = this._factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheckReturnsOK()
        {
            var response = await this._httpClient.GetAsync("/health");

            Assert.True(HttpStatusCode.OK == response.StatusCode, $"Response status code doesn't match. Expected: {HttpStatusCode.OK}, Actual: {response.StatusCode}");

        }
    }
}
