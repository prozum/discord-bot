using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Api.Model
{
    public class Permission
    {
        [JsonProperty("allow")]
        public int Allow { get; set; }

        [JsonProperty("deny")]
        public int Deny { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
