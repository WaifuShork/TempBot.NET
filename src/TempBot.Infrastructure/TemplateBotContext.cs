using Microsoft.EntityFrameworkCore;
using TempBot.Infrastructure.Models;

namespace TempBot.Infrastructure
{
    public static class TemplateBotContextFactory
    {
        public static void Create(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableServiceProviderCaching()
                // Replace this with your sqlite .db file :) just don't forget to update the migrations
                .UseSqlite("Data Source=database.db");
        }
    }

    public class TemplateBotContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.Create();
        }
        
        public DbSet<GuildConfig> GuildConfigs { get; set; }
    }
}