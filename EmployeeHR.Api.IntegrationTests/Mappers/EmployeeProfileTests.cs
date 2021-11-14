using AutoMapper;
using Xunit;

namespace EmployeeHR.Api.Mappers.IntegrationTests
{

    public class EmployeeProfileTests : IClassFixture<MapperConfigurationFixture>
    {

        private readonly IConfigurationProvider _mapperConfiguration;
        private readonly IMapper _mapper;

        public EmployeeProfileTests(MapperConfigurationFixture fixture)
        {
            this._mapperConfiguration = fixture.MapperConfiguration;
            this._mapper = fixture.Mapper;
        }

        [Fact()]
        public void EmployeeProfileShouldNotThrowExceptionTest()
        {

            // Assert
            this._mapperConfiguration.AssertConfigurationIsValid();

        }
    }
}