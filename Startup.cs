using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Firefly.Extensions;
using Firefly.Models;
using Firefly.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
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
            ConfigureDevelopmentModeServices(app, env, loggerFactory);
            ConfigureAuthServer(app);
            app.UseCors(
                // very benevolent CORS for start
                builder => builder.AllowAnyOrigin().AllowAnyHeader()
                );

            //app.UseCurrentUserMiddleware();
            app.UseMvc();
        }

        private void ConfigureDevelopmentModeServices(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory){
            if (env.IsDevelopment() || env.IsStaging()){
                app.UseDebugHeadersMiddleware();
                loggerFactory.AddDebug();
                UserSeeder.Initialize(app.ApplicationServices);
                var crap = new CrapSeeder(app.ApplicationServices);
                crap.Seed();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCryptography(services);
            ConfigureDatabase(services);
            ConfigureAuth(services);
            ConfigureMisc(services);
        }

        private void ConfigureCryptography(IServiceCollection services)
        {
            services.AddDataProtection()
                .AddKeyManagementOptions(options => {
                    options.AutoGenerateKeys = false;
                })
                .SetApplicationName("Firefly")
                .PersistKeysToFileSystem(new DirectoryInfo(@"keys/"));
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
            .AddUserStore<UserStore<ApplicationUser, Role, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<Role, ApplicationDbContext, Guid>>();
            services.AddSingleton<ICurrentUserProvider, CurrentUserProvider>();
        }

        private void ConfigureAuthServer(IApplicationBuilder app){
            app.UseOAuthValidation();
        
            app.UseOpenIdConnectServer(options => {
                options.Provider = new AuthorizationProvider(app.ApplicationServices.GetRequiredService<ILogger<AuthorizationProvider>>());
                options.AuthorizationEndpointPath = "/oauth/authorize";
                options.TokenEndpointPath = "/oauth/token";
                options.AllowInsecureHttp = true;
                options.AutomaticAuthenticate = true;    
                options.DataProtectionProvider = app.ApplicationServices.GetDataProtectionProvider();
                if (Boolean.Parse(Configuration["Keys:OwnCertificate"])){
                    options.SigningCredentials.AddCertificate(CreateOauthCertificate());
                }
            });
        }

        private X509Certificate2 CreateOauthCertificate(){
            var path = Configuration["Keys:CertificatePath"];
            var password = Configuration["Keys:CertificatePassword"];
            return new X509Certificate2(path, password);
        }

    }
}
 