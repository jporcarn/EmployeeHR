using EmployeeHR.Api.IntegrationTests;
using EmployeeHR.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeHR.Api.Controllers.IntegrationTests
{
    public class EmployeeControllerUnhappyPathTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {

        private readonly TestWebApplicationFactory<Startup> _factory;

        private readonly HttpClient _httpClient;

        public EmployeeControllerUnhappyPathTests(TestWebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
            this._httpClient = this._factory.CreateDefaultClient();

        }

        [Fact()]
        public async Task PostAsyncWithInvalidFirstNameReutrnsStatus400BadRequestTest()
        {
            var createEmployeeRequest = new Employee
            {
                FirstName = null,
                LastName = "Doe",
                SocialSecurityNumber = Guid.NewGuid().ToString(),
            };

            var response = await this._httpClient.PostAsJsonAsync("/api/employee", createEmployeeRequest, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Fact()]
        public async Task PostAsyncWithInvalidFirstNameReutrnsExpectedProblemDetailsTest()
        {
            var createEmployeeRequest = new Employee
            {
                FirstName = null,
                LastName = "Doe",
                SocialSecurityNumber = Guid.NewGuid().ToString(),
            };

            var response = await this._httpClient.PostAsJsonAsync("/api/employee", createEmployeeRequest, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            Assert.Collection(problemDetails.Errors, (kvp) =>
            {

                Assert.Equal(nameof(Employee.FirstName), kvp.Key);
                var error = Assert.Single(kvp.Value);

                Assert.Equal($"The {nameof(Employee.FirstName)} field is required.", error);

            });

        }

    }
}