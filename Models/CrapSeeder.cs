using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Models {
    public class CrapSeeder{

        private readonly ApplicationDbContext context;
        public CrapSeeder(IServiceProvider serviceProvider){
            context = serviceProvider.GetService<ApplicationDbContext>();
        }

        public void Seed(int count = 1000){
            return;
            for (var i = 0; i < count; i++){
                var entity = new Article{
                    Title = "Fucking article no. " + i.ToString(),
                    Url = "nemam",
                    Body = RandomString(200),
                    AuthorId = new Guid("fb0456da-e790-4562-da2b-08d3eec27e8f")
                };
                context.Articles.Add(entity);
            }
            context.SaveChanges();
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}