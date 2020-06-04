using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    public class ApiConnection
    {
        public static string ipaddress = "http://amareapp.000webhostapp.com/";
        public static string Url
        {
            get
            {
                return ipaddress;
            }
            set
            {
                ipaddress = value;
            }
        }
        public static string SocketUrl
        {
            get
            {
                return "192.168.1.10";
            }
        }
    }
}
