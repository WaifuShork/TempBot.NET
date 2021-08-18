using System;
using System.Threading.Tasks;

using Serilog;

using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;

namespace TempBot
{
    // You don't need to keep this class included, I just use it to extend functionality of InteractiveBase,
    // InteractiveBase is a command context built on top of ModuleBase for interactive commands, I've defined
    // a few helper methods below that you can call from any class that inherits from TemplateModuleBase
    public abstract class TemplateModuleBase : InteractiveBase<SocketCommandContext>
    {
        // Both of these methods provide some abstraction to the built in NextMessageAsync, this will 
        // allow you to way for a user response with a timeout set in TimeSpan seconds or double

        // example:
        // 
        // // Bot will stop waiting for a response for 10 seconds, then timeout
        // var message = await NextMessageAsync(TimeSpan.FromSeconds(10));
        // if (message != null)
        // {
        //      // You can do whatever you want with the message object                
        //      await ReplyAsync($"Echo: {message.Contents}");        
        // }
        //

        protected async Task<IMessage> NextMessageAsync(TimeSpan seconds)
        {
            return await NextMessageAsync(true, true, seconds);
        }

        protected async Task<IMessage> NextMessageAsync(double seconds)
        {
            return await NextMessageAsync(true, true, TimeSpan.FromSeconds(seconds));
        }

        // Useful for sending a message that you want to delete in X seconds
        protected async Task TimedDeletionAsync(string contents, bool isTTS, Embed embed, TimeSpan timeSpan)
        {
            try
            {
                var message = await Context.Channel.SendMessageAsync(contents, isTTS, embed);
                // Task.Delay provides a non-blocking timer for delays, avoid using Thread.Sleep in asynchronous operations 
                await Task.Delay(timeSpan);
                await message.DeleteAsync();
            }
            // Why try/catch a simple deletion of a message? I've had numerous occasions where my bot would throw when attempting to timed delete
            // I'm entirely unsure of the cause, but I started logging it just to keep up with when it does happen, you're more than welcome to remove this
            catch (HttpException e)
            {
                Log.Warning($"TimedDeletionAsync caught {typeof(HttpException)}\n\nMessage: {e.Message}\n\nInner Exception: {e.InnerException}");
            }
        }

        protected async Task SendSuccessEmbedAsync(string contents, bool isTTS, bool isEmbed = true)
        {
            if (isEmbed)
            {
                await Create("Success")
                    .WithSuccessColor()
                    .WithDescription(contents)
                    .SendToChannelAsync(Context.Channel);
                
                return;
            }

            await Context.Channel.SendMessageAsync($"Success: {contents}", isTTS, null);
        }

        protected async Task SendErrorEmbedAsync(string contents, bool isTTS, bool isEmbed = true)
        {
            if (isEmbed)
            {
                await Create("Error")
                    .WithErrorColor()
                    .WithDescription(contents)
                    .SendToChannelAsync(Context.Channel);
                
                return;
            }

            await Context.Channel.SendMessageAsync($"Error: {contents}", isTTS, null);
        }

        protected EmbedBuilder Create(string title)
        {
            return DefaultEmbedBuilder.Create(title);
        }

        protected EmbedBuilder Create()
        {
            return DefaultEmbedBuilder.Create();
        }
    }

    public static class DefaultEmbedBuilder 
    {
        public static EmbedBuilder Create(string title)
        {
            return new EmbedBuilder().WithTitle(title);
        }

        public static EmbedBuilder Create()
        {
            return new EmbedBuilder();
        }

        // Why bother with methods like this? because extensions are magic! this allows you to setup things like having a default color for embeds
        public static EmbedBuilder WithDefaultColor(this EmbedBuilder builder)
        {
            builder.WithColor(Color.Blue);
            return builder;
        }

        public static EmbedBuilder WithSuccessColor(this EmbedBuilder builder)
        {
            builder.WithColor(Color.Green);
            return builder;
        }

        public static EmbedBuilder WithErrorColor(this EmbedBuilder builder)
        {
            builder.WithColor(Color.DarkRed);
            return builder;
        }

        public static async Task SendToChannelAsync(this EmbedBuilder builder, IChannel channel)
        {
            await (channel as ISocketMessageChannel).SendMessageAsync(string.Empty, false, builder.Build());
        }
    }
}