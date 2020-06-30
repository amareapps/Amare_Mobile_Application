using Android.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    public class ApiConnection
    {
        public static string ipaddress = "amareapp.000webhostapp.com/";
        public static string hostUrl = "192.168.1.2";
        public static string Url
        {
            get
            {
                return ipaddress;
            }
        }
        public static string SocketUrl
        {
            get
            {
                return hostUrl;
            }
            set
            {
                hostUrl = value;
            }
        }
    }
}
