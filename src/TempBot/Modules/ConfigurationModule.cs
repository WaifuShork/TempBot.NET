using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using TempBot.Infrastructure.Models.Impl;

namespace TempBot.Modules
{
    public class ConfigurationModule : TemplateModuleBase
    {
        private readonly GuildConfigs _configs;
        
        public ConfigurationModule(GuildConfigs config)
        {
            _configs = config;
        }
        
        [Command("prefix")]
        public async Task GetOrUpdatePrefixAsync(string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = await _configs.GetPrefixAsync(Context.Guild.Id);
                await ReplyAsync($"Your prefix is: {prefix}");
                return;
            }

            await _configs.UpdatePrefixAsync(Context.Guild.Id, prefix);
            await ReplyAsync($"Successfully updated prefix");
        }
        
        [Command("logs")]
        public async Task GetOrUpdateLogChannelAsync(string logChannel = "")
        {
            if (string.IsNullOrWhiteSpace(logChannel))
            {
                var temp = await _configs.GetLogChannelAsync(Context.Guild.Id);
                await ReplyAsync($"Your logging channel is: <#{temp}>");
                return;
            }
            
            if (!MentionUtils.TryParseChannel(logChannel, out var logChannelId))
            {
                await ReplyAsync($"{logChannel} is not a valid channel");
                return;
            }
            
            await _configs.UpdateLogChannelAsync(Context.Guild.Id, logChannelId);
            await ReplyAsync($"Successfully updated log channel");
        }
    }
}