using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using TempBot.Infrastructure.Extensions;
using TempBot.Infrastructure.Repositories;

namespace TempBot.Infrastructure.Models.Impl
{
    public class GuildConfigs : IGuildConfigsRepository
    {
        private readonly TemplateBotContext _context;

        public GuildConfigs(TemplateBotContext context)
        {
            _context = context;
        }
        
        public async Task UpdatePrefixAsync(ulong guildId, string prefix)
        {
            var guild = await _context.GetOrAddGuildAsync(guildId);
            guild.Prefix = prefix;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetPrefixAsync(ulong guildId)
        {
            var guild = await _context.GetOrAddGuildAsync(guildId);
            return guild.Prefix;
        }

        public async Task UpdateLogChannelAsync(ulong guildId, ulong channelId)
        {
            var guild = await _context.GetOrAddGuildAsync(guildId);
            guild.LogChannelId = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task<ulong> GetLogChannelAsync(ulong guildId)
        {
            var guild = await _context.GetOrAddGuildAsync(guildId);
            return guild.LogChannelId;        
        }

        private bool _disposed = false;

        protected virtual async void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await _context.DisposeAsync();
                }
            }

            _disposed = true;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}