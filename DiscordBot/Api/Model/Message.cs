using Newtonsoft.Json;
using System;

namespace Discord.API.Model
{
    public class Message
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        //Fix type
        [JsonProperty("attachments")]
        public object[] Attachments { get; set; }

        [JsonProperty("tts")]
        public bool TextToSpeak { get; set; }

        //Fix type
        [JsonProperty("embeds")]
        public object[] Embeds { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("mention_everyone")]
        public bool MentionsEveryone { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }

        //Fix type
        [JsonProperty("author")]
        public User Author { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        
        //Fix type
        [JsonProperty("mentions")]
        public object[] Mentions { get; set; }
    }
}
