using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    public class SpotifyModelLocal
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string artist_name { get; set; }
        public string genres { get; set; }
        public string followers { get; set; }
        public string image { get; set; } = "";

    }
}
