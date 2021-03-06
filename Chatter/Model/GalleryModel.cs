﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    public class GalleryModel
    {
        [PrimaryKey]
        public string id { get; set; } = "";
        public string user_id { get; set; } = "";
        public string is_dp { get; set; } = "";
        public string image { get; set; } = "";
        public string timestamp { get; set; } = "";
        public bool isShow { get; set; } = false;
    }
}
