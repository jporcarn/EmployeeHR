using EmployeeHR.Api.IntegrationTests;
using EmployeeHR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeHR.Api.Controllers.IntegrationTests
{
    public class EmployeeControllerTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        const string skipReason = "Already covered by EmployeeControllerAllInOneTests";

        private readonly IEnumerable<Employee> _expectedEmployees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    FirstName = "Palmer",
                    LastName = "Matthew Hogan"
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Cody",
                    LastName = "Aristotle Pena"
                },
                new Employee
                {
                    Id = 3,
                    FirstName = "Phillip",
                    LastName = "Steel Haley"
                },
                new Employee
                {
                    Id = 4,
                    FirstName = "Ali",
                    LastName = "Isaac Lang"
                },
                new Employee
                {
                    Id = 5,
                    FirstName = "Dorian",
                    LastName = "Cullen Saunders"
                },
                new Employee
                {
                    Id = 6,
                    FirstName = "Sylvester",
                    LastName = "Allen Marks"
                },
                new Employee
                {
                    Id = 7,
                    FirstName = "Gavin",
                    LastName = "Tobias Douglas"
                },
                new Employee
                {
                    Id = 8,
                    FirstName = "Carl",
                    LastName = "Flynn Hoffman"
                },
                new Employee
                {
                    Id = 9,
                    FirstName = "Cullen",
                    LastName = "Dominic Solomon"
                },
                new Employee
                {
                    Id = 10,
                    FirstName = "Yasir",
                    LastName = "Cole Sherman"
                },
            };

        private readonly TestWebApplicationFactory<Startup> _factory;

        private readonly HttpClient _httpClient;

        public EmployeeControllerTests(TestWebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
            this._httpClient = this._factory.CreateDefaultClient();

        }

        [Fact(Skip = skipReason)]
        public async Task GetAsyncReturnsContentTest()
        {
            var response = await this._httpClient.GetAsync("/api/employee");

            Assert.NotNull(response.Content);
            Assert.True(response.Content.Headers.ContentLength > 0);

        }

        [Fact(Skip = skipReason)]
        public async Task GetAsyncReturnsExpectedContentTest()
        {

            var responseStream = await this._httpClient.GetStreamAsync("/api/employee");

            var model = await JsonSerializer.DeserializeAsync<IEnumerable<Employee>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(model);

            var orderedExpectedEmployees = this._expectedEmployees.OrderBy(e => e.Id);
            var orderedEmployees = model?.OrderBy(ae => ae.Id).Take(10).OrderBy(ae => ae.Id);
            Assert.True(orderedExpectedEmployees.SequenceEqual(orderedEmployees));
        }

        [Fact(Skip = skipReason)]
        public async Task GetAsyncReturnsExpectedMediaTypeTest()
        {
            var response = await this._httpClient.GetAsync("/api/employee");

            Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType.MediaType);

        }

        [Fact(Skip = skipReason)]
        public async Task GetAsyncReturnsStatus200OKTest()
        {
            var response = await this._httpClient.GetAsync("/api/employee");
            response.EnsureSuccessStatusCode();
        }


        [Fact(Skip = skipReason)]
        public async Task GetAsyncReturnsExpectedResponseTest()
        {

            var model = await this._httpClient.GetFromJsonAsync<IEnumerable<Employee>>("/api/employee");

            Assert.NotNull(model);

            var orderedExpectedEmployees = this._expectedEmployees.OrderBy(e => e.Id);
            var orderedEmployees = model?.OrderBy(ae => ae.Id).Take(10).OrderBy(ae => ae.Id);
            Assert.True(orderedExpectedEmployees.SequenceEqual(orderedEmployees));
        }
    }
}