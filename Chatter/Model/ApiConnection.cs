using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    public class ApiConnection
    {
        public static string ipaddress = "192.168.1.7";
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
    }
}
