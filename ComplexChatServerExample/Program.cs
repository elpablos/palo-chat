using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Palo.ChatLibrary;

namespace ComplexWpfChatServerExample
{
    /// <summary>
    /// Vstupni bod aplikace chat server.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Hlavni vlakno chat serveru.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // inicializace promennych
            string server = string.Empty;
            int port = 0;
            // pokud jsou predany parametrem, tak je nacteme
            if (args.Length > 1)
            {
                server = args[0];
                port =  Int32.Parse(args[1]);
            }
            // jinak vyuzijeme config
            else
            {
                server = AppSettingsReader.ReadProperty("ServerAddress");
                port = AppSettingsReader.ReadIntProperty("ServerPort");
            }

            // zistani IPAddress
            var he = Dns.GetHostEntry(server);
            IPAddress ip = he.AddressList.FirstOrDefault(a => AddressFamily.InterNetwork == a.AddressFamily) ?? he.AddressList[0];

            // odstartovani serveru
            Server srv = new Server(ip, port);
            srv.StartListen();
        }
    }
}
