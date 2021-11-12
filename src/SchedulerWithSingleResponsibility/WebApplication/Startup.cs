using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddinEngine.Abstract;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace WebApplication
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
            services.AddHostedService<AnyBackgroundService>();
            services.AddSingleton<ObservableConcurrentQueue<string>>();//The task queue use to connect with Background Service
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApplication", Version = "v1"});
            });
            
            services.AddHangfire((provider, configuration) =>
                {
                    //Using memory storage to act faster when send bunch of commands
                    // configuration.UseSQLiteStorage(currentDirectory + Configuration["Hangfire:Connection"]);
                    configuration.UseMemoryStorage();
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication v1"));
            }

            // var dashboardOptions = new DashboardOptions
            // {
            //     AppPath = Env.BaseUri + "/true" //Not initialize UI when go back to index page
            // };
            app.UseHangfireDashboard("/hangfire"/*, dashboardOptions*/);
            var options = new BackgroundJobServerOptions
            {
                // WorkerCount = Environment.ProcessorCount * 5
                WorkerCount = 1024
            };
            app.UseHangfireServer(options);
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}