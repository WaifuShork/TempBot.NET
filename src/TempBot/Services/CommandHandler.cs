using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace TempBot.Services
{
    public class CommandHandler : DiscordClientService
    {
        private readonly IServiceProvider _provider;
        private readonly CommandService _commandService;
        private readonly IConfiguration _config;
        public CommandHandler(DiscordSocketClient client, 
                              ILogger<DiscordClientService> logger,
                              CommandService commandService,
                              IServiceProvider provider,
                              IConfiguration configuration) : base(client, logger)
        {
            _commandService = commandService;
            _provider = provider;
            _config = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Client.MessageReceived += OnMessageReceived;
            _commandService.CommandExecuted += OnCommandExecuted;
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message)
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) &&
                !message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(Client, message);
            await _commandService.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            switch (result.Error)
            {

            }
            if (command.IsSpecified && !result.IsSuccess)
            {
                await context.Channel.SendMessageAsync($"Error: {result}");
            }
        }
    }
}