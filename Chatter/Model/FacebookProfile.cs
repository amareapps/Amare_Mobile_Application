using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Chatter.Model
{
    public class Data
    {
        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Picture
    {
        public Data Data { get; set; }
    }

    public class FacebookProfile
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public Picture Picture { get; set; }

        [JsonProperty("name")]
        public string UserName { get; set; }
        [JsonProperty("birthday")]
        public string Birthday { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }

    }

}
