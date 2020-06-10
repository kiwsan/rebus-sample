using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.ServiceProvider;

namespace Publisher.WebApi
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

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // 1. Configure Rebus
            services.AddRebus(configure => configure
                .Logging(x => x.MicrosoftExtensionsLogging(loggerFactory))
                .Transport(t => t.UseSqlServer("Server=localhost;Database=rebus;User Id=sa;Password=P@ssw0rd1;", "Publisher.WebApi"))
                .Subscriptions(x => x.StoreInSqlServer("Server=localhost;Database=rebus;User Id=sa;Password=P@ssw0rd1;", "subscriptions")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.UseRebus();

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
