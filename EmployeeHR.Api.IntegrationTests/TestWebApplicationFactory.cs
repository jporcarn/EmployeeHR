using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EmployeeHR.Api.IntegrationTests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {

        public string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }

        public TestWebApplicationFactory() : base()
        {
            Assembly currentAssem = Assembly.GetExecutingAssembly();
            string currentAssemName = currentAssem.GetName().Name;


            System.Diagnostics.Debug.WriteLine("Currently executing assembly:");
            System.Diagnostics.Debug.WriteLine("   {0}\n", currentAssem.FullName);

            if (!File.Exists("Properties/launchSettings.json"))
            {
                var path = System.IO.Path.Combine(GetExecutingDirectoryName(), "Properties/launchSettings.json");
                throw new FileNotFoundException($"File not found {path}. Please ensure that 'Copy to Output Directory' is set to Copy always", path);
            }

            using (var file = File.OpenText("Properties/launchSettings.json"))
            {
                var reader = new JsonTextReader(file);
                var jObject = JObject.Load(reader);

                //var variables = jObject["profiles"]["Ohpen.ApplicationDrafts.WebApi.Tests"]["environmentVariables"]
                //    .Select(p => p as JProperty)
                //    .ToList();

                var variables = jObject
                    .GetValue("profiles")
                    // select a proper profile here
                    .Where(p => (p as JProperty).Name == currentAssemName)
                    .SelectMany(profiles => profiles.Children())
                    .SelectMany(profile => profile.Children<JProperty>())
                    .Where(prop => prop.Name == "environmentVariables")
                    .SelectMany(prop => prop.Value.Children<JProperty>())
                    .ToList();

                foreach (var variable in variables)
                {
                    System.Diagnostics.Debug.WriteLine($"{variable.Name}: {variable.Value}");
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                }
            }
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            System.Diagnostics.Debug.WriteLine($"ASPNETCORE_ENVIRONMENT: {env}");

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true);

            if (!String.IsNullOrWhiteSpace(env))
            {
                configurationBuilder.AddJsonFile($"appsettings.{env}.json", false);
            }

            IConfigurationRoot config = configurationBuilder
                .AddEnvironmentVariables()
                .Build();

            return
                new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<TStartup>()
                        .UseConfiguration(config);
                });

        }

    }
}
