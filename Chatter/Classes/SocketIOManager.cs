using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Classes
{
    public static class SocketIOManager
    {
        public static Socket socket = IO.Socket("http://192.168.1.10:8080/");
    }
}
