using System;
using System.Threading.Tasks;

namespace TempBot.Infrastructure.Repositories
{
    public interface IGuildConfigsRepository : IDisposable
    {
        Task UpdatePrefixAsync(ulong guildId, string prefix);
        Task<string> GetPrefixAsync(ulong guildId);

        Task UpdateLogChannelAsync(ulong guildId, ulong channelId);
        Task<ulong> GetLogChannelAsync(ulong guildId);
    }
}