using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TempBot.Infrastructure.Models.Impl;

namespace TempBot.Services
{
    public class StartupService : DiscordClientService
    {
        private readonly GuildConfigs _guildConfigs;
        private readonly CommandService _commandService;
        
        private readonly IServiceProvider _provider;
        
        
        public StartupService(DiscordSocketClient client, 
                              ILogger<DiscordClientService> logger,
                              IServiceProvider provider) : base(client, logger)
        {
            _provider = provider;

            _guildConfigs = provider.GetRequiredService<GuildConfigs>();
            _commandService = provider.GetRequiredService<CommandService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}