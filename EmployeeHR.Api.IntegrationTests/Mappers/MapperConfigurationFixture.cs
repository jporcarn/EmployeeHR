using AutoMapper;
using System.Reflection;

namespace EmployeeHR.Api.Mappers.IntegrationTests
{
    public class MapperConfigurationFixture
    {
        public IConfigurationProvider MapperConfiguration { get; private set; }
        public IMapper Mapper { get; private set; }

        public MapperConfigurationFixture()
        {
            var assembly = Assembly.GetAssembly(typeof(EmployeeProfile));

            IConfigurationProvider config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(new[] {
                    assembly
            });
            });

            this.MapperConfiguration = config;
            this.Mapper = new Mapper(this.MapperConfiguration);
        }
    }
}