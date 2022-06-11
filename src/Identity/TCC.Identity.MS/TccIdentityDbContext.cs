using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TCC.Identity.MS.Models;

namespace TCC.Identity.MS
{
    public class TccIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public TccIdentityDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationBuilder builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                IConfiguration configuration = builder.Build();

                optionsBuilder.UseSqlServer(configuration["ConnectionStrings:TccIdentitySQL"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}