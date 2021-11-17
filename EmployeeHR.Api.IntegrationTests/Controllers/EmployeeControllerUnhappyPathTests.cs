using EmployeeHR.Api.IntegrationTests;
using EmployeeHR.Api.Models;
using EmployeeHR.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Theory()]
        [MemberData(nameof(GetInvalidInputModels))]
        public async Task PostAsyncWithInvalidInputModelsReutrnsStatus400BadRequestTest(CreateEmployeeRequest createEmployeeRequest)
        {

            var response = await this._httpClient.PostAsJsonAsync("/api/employee", createEmployeeRequest, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Theory()]
        [MemberData(nameof(GetInvalidInputModelsAndProblemDetailsErrorValidators))]
        public async Task PostAsyncWithInvalidInputModelReutrnsExpectedProblemDetailsTest(CreateEmployeeRequest createEmployeeRequest, Action<KeyValuePair<string, string[]>> validator)
        {

            var response = await this._httpClient.PostAsJsonAsync("/api/employee", createEmployeeRequest, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            Assert.Collection(problemDetails.Errors, validator);

        }

        public static IEnumerable<object[]> GetInvalidInputModels()
        {
            return GetInvalidInputModelsAndProblemDetailsErrorValidators().Select(x => new[] { x[0] });
        }

        public static IEnumerable<object[]> GetInvalidInputModelsAndProblemDetailsErrorValidators()
        {
            yield return new object[]
            {
                new CreateEmployeeRequest
                {
                    FirstName = null,
                    LastName = "Doe",
                    SocialSecurityNumber = Guid.NewGuid().ToString(),
                },
                new Action<KeyValuePair<string, string[]>>
                (
                    (kvp) =>
                    {
                        Assert.Equal(nameof(Employee.FirstName), kvp.Key);
                        var error = Assert.Single(kvp.Value);
                        Assert.Equal($"The {nameof(Employee.FirstName)} field is required.", error);
                    }
                )
            };

            yield return new object[]
            {
                new CreateEmployeeRequest
                {
                    FirstName = "John",
                    LastName = null,
                    SocialSecurityNumber = Guid.NewGuid().ToString(),
                },
                new Action<KeyValuePair<string, string[]>>
                (
                    (kvp) =>
                    {
                        Assert.Equal(nameof(Employee.LastName), kvp.Key);
                        var error = Assert.Single(kvp.Value);
                        Assert.Equal($"The {nameof(Employee.LastName)} field is required.", error);
                    }
                )
            };


            yield return new object[]
            {
                new CreateEmployeeRequest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    SocialSecurityNumber = null,
                },
                new Action<KeyValuePair<string, string[]>>
                (
                    (kvp) =>
                    {
                        Assert.Equal(nameof(Employee.SocialSecurityNumber), kvp.Key);
                        var error = Assert.Single(kvp.Value);
                        Assert.Equal($"The {nameof(Employee.SocialSecurityNumber)} field is required.", error);
                    }
                )
            };
        }

    }
}