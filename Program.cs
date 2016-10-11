using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Firefly.CLI;
using System.Threading.Tasks;

namespace Firefly
{
    public class Program
    {
        private static string[] args;
        public static void Main(string[] args)
        {
            Program.args = args;

            var config = new ConfigurationBuilder()  
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            // hack - come with something better
            if (config["perform"] != null){
                // only CLI Action, then exit
                HandleCLIAction(
                    host.Services.GetService(typeof(ICLIHandler)) as Handler,
                    host.Services.GetService(typeof(ILogger<Program>)) as ILogger<Program>,
                    config["perform"]
                ).Wait();

                if (config["continue"] == null || Boolean.Parse(config["continue"]) != true){
                    return;
                }
            } 
                
            host.Run();
        }

        public static string[] GetCliArgs(){
            return args;
        }


        private static async Task HandleCLIAction(ICLIHandler handler, ILogger<Program> logger, string perform){
            if (perform is string){
                perform = perform.ToUpper();
                    
                if (Enum.IsDefined(typeof(CLIActions), perform)){
                    CLIActions action;
                    Enum.TryParse(perform, out action);
                    await handler.HandleCommand(action);
                } else {
                    await handler.HandleCommand(CLIActions.HELP);
                }              
            }
        }
    }
}
