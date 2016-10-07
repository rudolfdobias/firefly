using System;
using Firefly.Models;
using Firefly.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Firefly
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddUserSecrets()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

         // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            ConfigureAuthServer(app);
            UserSeeder.Initialize(app.ApplicationServices);
            app.UseMvc();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabase(services);
            ConfigureAuth(services);
            ConfigureMisc(services);
        }

        private void ConfigureMisc(IServiceCollection services){
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = Configuration["DbContextSettings:ConnectionString"];
            services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(connectionString));
        }

        private void ConfigureAuth(IServiceCollection services){
            services.AddAuthentication();
            services.AddIdentity<ApplicationUser, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext, Guid>()
            //.AddDefaultTokenProviders()
            .AddUserStore<UserStore<ApplicationUser, Role, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<Role, ApplicationDbContext, Guid>>();
        }

        private void ConfigureAuthServer(IApplicationBuilder app){
            app.UseOAuthValidation();
            app.UseOpenIdConnectServer(options => {
                options.Provider = new AuthorizationProvider();
                options.AuthorizationEndpointPath = "/oauth/authorize";
                options.TokenEndpointPath = "/oauth/token";
                options.AllowInsecureHttp = true;
            });
        }

    }
}
 