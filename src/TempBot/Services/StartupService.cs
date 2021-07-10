using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TempBot.Common;

namespace TempBot.Services
{
    public class StartupService : DiscordClientService
    {
        private readonly DbHelper _dbHelper;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;
        
        public StartupService(DiscordSocketClient client, 
                              ILogger<DiscordClientService> logger, 
                              DbHelper dbHelper, 
                              CommandService commandService,
                              IServiceProvider provider) : base(client, logger)
        {
            _dbHelper = dbHelper;
            _commandService = commandService;
            _provider = provider;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                Singletons.Inject(_dbHelper);
                await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            }, stoppingToken);
        }
    }
}