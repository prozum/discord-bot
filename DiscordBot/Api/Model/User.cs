using Newtonsoft.Json;

namespace Discord
{
	public class User
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("email")]
		public string EMail { get; set; }
	}
}

