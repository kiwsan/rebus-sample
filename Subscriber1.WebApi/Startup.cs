using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace Subscriber1.WebApi
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

            // 1. Service registration pipeline...
            services.AutoRegisterHandlersFromAssemblyOf<Handler>();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // 1.1. Configure Rebus
            services.AddRebus(configure => configure
                .Logging(x => x.MicrosoftExtensionsLogging(loggerFactory))
                .Transport(t => t.UsePostgreSql("server=localhost; database=rebus; user id=postgres; password=postgres; maximum pool size=30", "messages", "Subscriber1.WebApi"))
                .Routing(r => r.TypeBased().MapAssemblyOf<StringMessage>("Publisher.WebApi"))
                .Options(o =>
                    {
                        o.SetNumberOfWorkers(10);
                        o.SetMaxParallelism(20);
                    }));

            // 1.2. Potentially add more service registrations for the application, some of which
            //      could be required by handlers.

            // 2. Application starting pipeline...
            // Make sure we correctly dispose of the provider (and therefore the bus) on application shutdown

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.UseRebus(async bus =>
            {

                await bus.Subscribe<StringMessage>();
                await bus.Subscribe<DateTimeMessage>();
                await bus.Subscribe<TimeSpanMessage>();

            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

//dotnet run --urls=http://localhost:5002/
