using TempBot.Infrastructure.Models.Impl;
using TempBot.Infrastructure.Repositories;

namespace TempBot.Common
{
    public class DbHelper
    {
        public DbHelper(GuildConfigs guildConfigs)
        {
            GuildConfigs = guildConfigs;
        }
        
        public IGuildConfigsRepository GuildConfigs { get; private set; }
    }
}