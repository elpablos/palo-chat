using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palo.ChatLibrary;
using System.Net.Sockets;
using System.Net;
using Palo.ChatLibrary.Arguments;

namespace SimpleChatServerExample
{
    /// <summary>
    /// Hlavni vlakno jednoducheho chat serveru.
    /// </summary>
    public class Program
    {
        #region Fields

        protected static SimpleChatServer _server;

        protected static IList<Socket> _clients;

        #endregion // Fields

        #region Main methods

        /// <summary>
        /// Spousteci bod aplikace.
        /// Zahajeni prijimani na portu 5000.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            _clients = new List<Socket>();

            _server = new SimpleChatServer(IPAddress.Loopback, 5000);
            _server.ReadingInterrupted += OnReadingInterrupted;
            _server.MessageReceived += OnMessageReceived;
            _server.SocketAccepted += OnSocketAccepted;
            _server.StartListen();
        }

        #endregion // Main methods

        #region Events methods

        /// <summary>
        /// Reakce na vyjimku pri cteni zpravy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void OnReadingInterrupted(object sender, SocketExceptionArgs e)
        {
            _clients.Remove(e.Socket);
        }

        /// <summary>
        /// Reakce na prijeti klienta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void OnSocketAccepted(object sender, SocketArgs e)
        {
            _clients.Add(e.Socket);
            Console.WriteLine("Uzivatel se pripojil..");
        }

        /// <summary>
        /// Reakce na prijeti zpravy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void OnMessageReceived(object sender, SimpleMessageArgs e)
        {
            Console.WriteLine(e.Message);
            SendMessageToAll(e.Message);
        }

        #endregion // Events methods

        #region Helper methods

        /// <summary>
        /// Pomocna metoda odesilajici zpravy vsem klientum.
        /// </summary>
        /// <param name="message"></param>
        protected static void SendMessageToAll(string message)
        {
            foreach (Socket sock in _clients)
            {
                _server.SendMessage(sock, message);
            }
        }

        #endregion // Helper methods
    }
}
