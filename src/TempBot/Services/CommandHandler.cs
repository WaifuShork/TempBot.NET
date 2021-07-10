using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TempBot.Common;

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
            await Task.Run(() =>
            {
                Client.MessageReceived += async (message) => await OnMessageReceivedAsync(message);
                _commandService.CommandExecuted += async (command, context, result) => await OnCommandExecutedAsync(command, context, result);
            }, cancellationToken);
        }

        private async Task OnMessageReceivedAsync(IDeletable msg)
        {
            if (msg is not SocketUserMessage message)
            {
                return;
            }

            // A default prefix for if a command is executed inside DMs, cause obviously DMs don't store a guild config
            var prefix = "!";
            
            // Is the source channel from a guild? 
            if (message.Channel is IGuildChannel guildChannel)
            {
                prefix = await Singletons.DbHelper.GuildConfigs.GetPrefixAsync(guildChannel.GuildId);
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    // If the returned prefix from the database is null, then we update the prefix to be '!' for future use
                    prefix = "!";
                    await Singletons.DbHelper.GuildConfigs.UpdatePrefixAsync(guildChannel.GuildId, "!");
                }
                
            }
            
            var argPos = 0;
            // Is the prefix '!' or @Bot? this allows you to use a string prefix, or just mention the bot so the commands can
            // be very user friendly, this allows a user to simply type "@bot help" or "@bot" to get help
            if (!message.HasStringPrefix(prefix, ref argPos) &&
                !message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(Client, message);
            await _commandService.ExecuteAsync(context, argPos, _provider);
        }
        
        // This method has a ton of potential
        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case CommandError.UnknownCommand:
                        await context.Channel.SendMessageAsync("Unknown command");
                        break;
                    case CommandError.ParseFailed:
                        break;
                    case CommandError.BadArgCount:
                        // maybe send help on bad args?
                        break;
                    case CommandError.ObjectNotFound:
                        // print the exception for why?
                        break;
                    case CommandError.MultipleMatches:
                        // notify the user that multiple commands with the same arguments were located? 
                        break;
                    case CommandError.UnmetPrecondition:
                        // access denied?
                        break;
                    case CommandError.Exception:
                        // log error?
                        break;
                    case CommandError.Unsuccessful:
                        // why?
                        break;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}