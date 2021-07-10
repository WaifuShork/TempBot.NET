using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TempBot.Common;
using TempBot.Infrastructure;

namespace TempBot.Modules
{
    public class ConfigurationModule : TemplateModuleBase
    {
        [Command("prefix")]
        public async Task GetOrUpdatePrefixAsync(string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = await Singletons.DbHelper.GuildConfigs.GetPrefixAsync(Context.Guild.Id);
                await ReplyAsync($"Your prefix is: {prefix}");
                return;
            }

            await Singletons.DbHelper.GuildConfigs.UpdatePrefixAsync(Context.Guild.Id, prefix);
            await ReplyAsync($"Successfully updated prefix");
        }
        
        [Command("logs")]
        public async Task GetOrUpdateLogChannelAsync(string logChannel = "")
        {
            if (string.IsNullOrWhiteSpace(logChannel))
            {
                var temp = await Singletons.DbHelper.GuildConfigs.GetLogChannelAsync(Context.Guild.Id);
                await ReplyAsync($"Your logging channel is: <#{temp}>");
                return;
            }
            
            if (!MentionUtils.TryParseChannel(logChannel, out var logChannelId))
            {
                await ReplyAsync($"{logChannel} is not a valid channel");
                return;
            }
            
            await Singletons.DbHelper.GuildConfigs.UpdateLogChannelAsync(Context.Guild.Id, logChannelId);
            await ReplyAsync($"Successfully updated log channel");
        }
    }
}