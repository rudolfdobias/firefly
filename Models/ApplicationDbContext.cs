using System;
using Firefly.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Firefly.Models
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, Guid>
  {
    
    protected readonly IOptions<Config> _config;
    public ApplicationDbContext(DbContextOptions options, IOptions<Config> config) : base(options)
    {
        //Database.EnsureCreated();
        _config = config;
    }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<TestModel> TestModels {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema(_config.Value.DbContextSettings.DefaultSchema); 
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("public.uuid_generate_v4()");
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("public.uuid_generate_v4()");
            });
            base.OnModelCreating(modelBuilder);
        }
  }
}