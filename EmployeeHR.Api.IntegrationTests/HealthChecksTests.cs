using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeHR.Api.IntegrationTests
{
    public class HealthChecksTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly HttpClient _httpClient;

        public HealthChecksTests(WebApplicationFactory<Startup> factory)
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
