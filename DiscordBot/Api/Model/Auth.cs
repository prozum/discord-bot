using Newtonsoft.Json;

namespace Discord.API.Model
{
    public class Authentication
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
