using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palo.ChatLibrary;
using Palo.ChatLibrary.Arguments;
using System.Net;

namespace SimpleChatClientExample
{
    /// <summary>
    /// Hlavni vlakno jednoducheho chat klienta.
    /// </summary>
    public class Program
    {
        #region Main methods

        /// <summary>
        /// Hlavni smycka.
        /// Spusteni klienta a odesilani zprav.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            SimpleChatClient client = new SimpleChatClient(IPAddress.Loopback, 5000);
            client.MessageReceived += OnMessageReceived;
            client.Connect();

            string str = string.Empty;
            while (!(str == "EXIT"))
            {
                str = Console.ReadLine();
                client.SendMessage(str);
            }

            client.Close();
        }

        #endregion // Main methods

        #region Event methods

        /// <summary>
        /// Reakce na prijeti zpravy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void OnMessageReceived(object sender, SimpleMessageArgs e)
        {
            Console.WriteLine(e.Message);
        }

        #endregion // Event methods
    }
}
