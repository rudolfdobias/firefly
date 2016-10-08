using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Firefly.Models
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, Guid>
  {

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<TestModel> TestModels {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            
            // PostgreSQL uses the public schema by default - not dbo.
            string schema = "firefly";
            modelBuilder.HasDefaultSchema(schema); 
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