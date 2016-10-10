using System;
using Firefly.Models;
using Firefly.Properties;
using Firefly.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Firefly.CLI{

    public class Handler : ICLIHandler{

        private readonly ILogger<Handler> _logger;
        private readonly IServiceProvider _serviceProvider;
        public Handler(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetService<ILogger<Handler>>();
        }

        public void HandleCommand(CLIActions action)
        {
            switch (action){
                case CLIActions.MIGRATE:
                    Migrate();
                break;

                case CLIActions.CREATE_MASTER_USER:
                case CLIActions.CREATEMASTERUSER:
                    SeedMasterUser();
                break;

            }
        }

        private async void Migrate(){
            _logger.LogInformation("Starting database migration...");
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            _logger.LogInformation("Migration ended.");
        }

        private void SeedMasterUser(){
            _logger.LogInformation("Seeding a master application user");
            var seeder = _serviceProvider.GetService<IMasterUserSeeder>();
            seeder.Seed();
        }
    }
}