using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Palo.ChatLibrary;

namespace ComplexWpfChatServerExample
{
    /// <summary>
    /// Vstupni bod aplikace chat server.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Reference na instanci serveru.
        /// </summary>
        public static Server Server { get; private set; }

        /// <summary>
        /// Hlavni vlakno chat serveru.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Startuju chat server..");
                // inicializace promennych
                string server = string.Empty;
                int port = 0;
                // pokud jsou predany parametrem, tak je nacteme
                if (args.Length > 1)
                {
                    Console.WriteLine("Nalezeny argumenty..");
                    server = args[0];
                    port = Int32.Parse(args[1]);
                }
                // jinak vyuzijeme config
                else
                {
                    Console.WriteLine("Nastaveni nacitam z configu..");
                    server = AppSettingsReader.ReadProperty("ServerAddress");
                    port = AppSettingsReader.ReadIntProperty("ServerPort");
                }

                Console.WriteLine("Ziskavam informace z DNS [{0}:{1}]..", server, port);
                // zistani IPAddress
                var he = Dns.GetHostEntry(server);
                IPAddress ip = he.AddressList.FirstOrDefault(a => AddressFamily.InterNetwork == a.AddressFamily) ?? he.AddressList[0];

                Console.WriteLine("Pokus o naslouchani na [{0}:{1}]..", server, port);
                // odstartovani serveru
                Thread thread = new Thread(StartServer);
                string buffer = string.Empty;
                Server = new Server(ip, port);
                
                while (buffer.ToLower() != "close")
                {
                    buffer = Console.ReadLine();

                    if (buffer.ToLower() == "help")
                    {
                        Help();
                    }
                }
                Server.ChatServer.Close();
              
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        /// <summary>
        /// V novym vlakne odstartujeme server.
        /// </summary>
        public static void StartServer()
        {
            Server.StartListen();
        }

        public static void Help()
        {
            Console.WriteLine("Write \"close\" for close server");
        }
    }
}
