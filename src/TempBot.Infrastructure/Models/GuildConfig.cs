using System.ComponentModel.DataAnnotations;

namespace TempBot.Infrastructure.Models
{
    public class GuildConfig
    {
        [Key]
        public ulong GuildId { get; init; } // this is considered a key, so it doesn't need to be initialized
        public string Prefix { get; set; } = string.Empty; // set it as an empty string so it won't be null at least
        
        public ulong LogChannelId { get; set; }
    }
}