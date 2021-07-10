using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using TempBot.Infrastructure.Models;

namespace TempBot.Infrastructure.Extensions
{
    public static class ContextExtensions
    {
        public static async Task<GuildConfig> GetOrAddGuildAsync(this TemplateBotContext context, ulong guildId)
        {
            var guild = await context.GuildConfigs.FirstOrDefaultAsync(g => g.GuildId == guildId);
            if (guild == null)
            {
                var newGuild = new GuildConfig
                {
                    GuildId = guildId
                };
                await context.AddAsync(newGuild);
                await context.SaveChangesAsync();
                return newGuild;
            }

            return guild;
        }

        public static async Task<bool> GuildExistsAsync(this TemplateBotContext context, ulong guildId)
        {
            var guild = await context.GuildConfigs.FirstOrDefaultAsync(g => g.GuildId == guildId);
            if (guild == null)
            {
                return false;
            }

            return true;
        }
    }
}