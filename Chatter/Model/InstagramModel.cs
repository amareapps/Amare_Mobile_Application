using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chatter.Model
{
    class InstagramModel
    {
        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }
    public partial class Datum
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("media_url")]
        public string MediaUrl { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
