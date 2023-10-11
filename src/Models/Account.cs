using Newtonsoft.Json;
using System;

namespace Website.Models
{
    public class AccountLogin
    {
        [JsonProperty("id_token")]
        public string AccessToken { get; set; }

        public DateTime Expire { get; set; }
    }
}
