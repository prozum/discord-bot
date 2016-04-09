using Discord.Api.Model;
using Newtonsoft.Json;

namespace Discord.API.Model
{
    public class Channel
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("permission_overwrites")]
        public Permission[] PermissionOverwrites { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }

    }
}
