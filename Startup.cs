using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Firefly.Models;
using Firefly.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            var connectionString = Configuration["DbContextSettings:ConnectionString"];
            services.AddDbContext<ArticleContext>(
                opts => opts.UseNpgsql(connectionString)
            );

            services.AddAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            configureAuth(app, env, loggerFactory);
        }

        private void configureAuth(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory){
            // Add a new middleware validating access tokens issued by the server.
        app.UseJwtBearerAuthentication(new JwtBearerOptions
        {
            AutomaticAuthenticate = true,
            AutomaticChallenge = true,
            Audience = "resource_server_1",
            Authority = "http://localhost:5000/",
            RequireHttpsMetadata = false
        });

        // Add a new middleware issuing tokens.
        app.UseOpenIdConnectServer(options =>
        {
            // Disable the HTTPS requirement.
            options.AllowInsecureHttp = true;
        
            // Disable the authorization endpoint as it's not used in this scenario.
            options.AuthorizationEndpointPath = PathString.Empty;
            options.TokenEndpointPath = "/oauth/token";

            options.Provider = new AuthorizationProvider();

            // Force the OpenID Connect server middleware to use JWT
            // instead of the default opaque/encrypted format.
            options.AccessTokenHandler = new JwtSecurityTokenHandler();
        });

        
        }
    }
}
