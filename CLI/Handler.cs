using System;
using System.Threading.Tasks;
using Firefly.Models;
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

        public async Task HandleCommand(CLIActions action)
        {
            switch (action){
                case CLIActions.MIGRATE:
                   await Migrate();
                break;

                case CLIActions.CREATE_USER:
                case CLIActions.CREATEUSER:
                    SeedMasterUser();
                break;

                case CLIActions.HELP:
                    Help();
                break;
            }
        }

        private async Task Migrate(){
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

        private void Help(){
            _logger.LogInformation(
                "\n--- Firefly CLI ---\n\n" +
                "Available commands:\n" +
                "--perform=migrate\n" +
                "--perform=createUser \n" +
                "\n"
            );
        }
    }
}