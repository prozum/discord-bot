using Newtonsoft.Json;

namespace Discord.API.Model
{
    public class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; }

		[JsonProperty("author")]
		public User Author { get; set; }
        
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
