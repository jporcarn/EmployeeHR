using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EmployeeHR.Tests
{
    [TestClass()]
    public class TestHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = (env ?? String.Empty).Equals("Development", StringComparison.InvariantCultureIgnoreCase);

            var builder = new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile($"appsettings.json", optional: false);

            if (isDevelopment)
            {
                builder.AddJsonFile($"appsettings.{env}.json", optional: true);
            }

            builder.AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            return configurationRoot;
        }


    }
}
