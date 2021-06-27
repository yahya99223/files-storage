using System;
using System.Linq;
using System.Net.Mime;
using Amazon;
using Amazon.S3;
using FlyingDonkey.Storage.Api.Pipeline;
using FlyingDonkey.Storage.DataLayer;
using FlyingDonkey.Storage.Handlers.Implementations;
using FlyingDonkey.Storage.Handlers.Interfaces;
using FlyingDonkey.Storage.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FlyingDonkey.Storage.Api
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

            services.AddControllers();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SCD.Storage.Service", Version = "v1" });
            });
            services.AddOptions();
            services.Configure<Settings>(Configuration);
            services.AddTransient<IStorageHandler, StorageHandler>();
            services.AddTransient<UploadFileValidator>();
            services.AddTransient<IFilesInfoRepository, FilesInfoRepository>();
            var options = Configuration.GetAWSOptions();
            var settings = services.BuildServiceProvider().GetService<IOptions<Settings>>().Value;
            options.Region = RegionEndpoint.GetBySystemName(settings.S3Settings.Region);
            services.AddDefaultAWSOptions(options);
            services.AddAWSService<IAmazonS3>();
            services.AddDbContext<StorageServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("StorageServiceDb"));
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlyingDonkey.Storage.Api v1"));
            }
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(
                        new
                        {
                            version = "1.0.2",
                            status = report.Status.ToString(),
                            errors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                        });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
