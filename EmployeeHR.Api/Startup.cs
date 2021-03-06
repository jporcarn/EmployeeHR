using EmployeeHR.Dal;
using EmployeeHR.EF;
using EmployeeHR.Interfaces;
using EmployeeHR.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;

namespace EmployeeHR.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Mappers
            services.AddAutoMapper((cfg) =>
            {
                // scan all profiles in the assembly
                var assembly = Assembly.GetAssembly(typeof(Mappers.EmployeeProfile));
                cfg.AddMaps(new[] {
                    assembly
                });
            });

            // ASP.NET Core?s CORS policies
            services.AddCors(
                (options) =>
                {
                    // CorsPolicy
                    string allowedOrigins = this.Configuration.GetSection("Cors").GetValue<string>("AllowedOrigins");

                    string[] origins = allowedOrigins?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

                    options.AddPolicy(name: "CorsPolicy",
                        (builder) =>
                        {
                            builder
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .WithOrigins(origins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // CORS preflight request: Access-Control-Max-Age gives the value in seconds for how long the response to the preflight request can be cached for without sending another preflight request. In this case, 86400 seconds is 24 hours.
                        }
                    );
                }
            );

            services.AddControllers()
                .AddNewtonsoftJson((options) =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });

            services.AddHealthChecks();

            services.AddDbContextFactory<EmployeeHRDbContext>(
                (options) =>
                {
                    options.UseSqlServer("name=ConnectionStrings:DefaultConnection", x => x.MigrationsAssembly("EmployeeHR.EF"));
                });

            services.AddDbContext<EmployeeHRDbContext>(
                (options) =>
                {
                    options.UseSqlServer("name=ConnectionStrings:DefaultConnection", x => x.MigrationsAssembly("EmployeeHR.EF"));
                });

            services.AddScoped<IEmployeeDal, EmployeeDal>();

            services.AddScoped<IEmployeeUnitOfWork, EmployeeUnitOfWork>();

            services.AddScoped<IEmployeeLogic, EmployeeLogic>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmployeeHR.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EmployeeHRDbContext dbContext)
        {

            if (env.IsDevelopment())
            {
                dbContext.Database.Migrate(); // TODO: independent staging step in your CD pipeline which runs before application deployment

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmployeeHR.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy"); // ASP.NET Core?s CORS policies

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}