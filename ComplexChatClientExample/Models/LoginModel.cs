using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ComplexWpfChatClientExample.Models
{
    public class LoginModel
    {
        public int Port { get; set; }
        public IPAddress ServerAddress { get; set; }
        public string NickName { get; set; }
    }
}
