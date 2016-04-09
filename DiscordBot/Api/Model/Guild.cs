using Newtonsoft.Json;

namespace Discord.API.Model
{
    public class Guild
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
