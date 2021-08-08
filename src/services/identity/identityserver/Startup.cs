using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using common.api;
using identityserver.Models;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace identityserver
{
    public class Startup
    {
        public IConfiguration configuration { get; }
        public IWebHostEnvironment environment { get; }
        public ServiceSettings serviceSettings { get; private set; }
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.environment = environment;
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            serviceSettings = services.AddAndBindConfigurationSection<ServiceSettings>(configuration, "ServiceSettings");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            const string defaultSchema = "idsvr";
            Action<DbContextOptionsBuilder> OptionsConfigureDbContext()
            {
                return b =>
                {
                    b.UseSqlServer(serviceSettings.SqlConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                };
            }
            services.AddIdentityServer()
                .AddTestUsers(Config.Users)
                .AddConfigurationStore(options =>
                {
                    // can use below line or will use action (new way to do below stuff)
                    //options.ConfigureDbContext = builder => builder.UseSqlServer(serviceSettings.SqlConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.ConfigureDbContext = OptionsConfigureDbContext();
                    options.DefaultSchema = defaultSchema;
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = OptionsConfigureDbContext();
                    options.DefaultSchema = defaultSchema;

                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // 1 hour
                })
                .AddDeveloperSigningCredential();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
