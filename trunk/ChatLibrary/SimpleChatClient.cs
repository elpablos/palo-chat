using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Palo.ChatLibrary.Shared;
using Palo.ChatLibrary.Arguments;
using Palo.ChatLibrary.Converters;
using Palo.ChatLibrary.ReadingLoop;

namespace Palo.ChatLibrary
{
    /// <summary>
    /// Trida reprezentujici jednoduchy chatovaci klient.
    /// </summary>
    public class SimpleChatClient : ChatSocket
    {
        #region Fields

        /// <summary>
        /// Reference na klientsky socket.
        /// </summary>
        protected TcpClient _client;

        #endregion // Fields

        #region Events

        /// <summary>
        /// Udalost, ktera pripojeni na server.
        /// </summary>
        public event SocketHandler ServerSocketConnected = delegate { };

        #endregion // Events

        #region Constructors

        /// <summary>
        /// Konstruktor, ktery preda promenne do lokalnich promennych 
        /// a inicializuje instanci klienta.
        /// Vyuziti defaultniho ByteToStringConverteru.
        /// </summary>
        /// <param name="ip">ip adresa serveru</param>
        /// <param name="port">cislo portu serveru</param>
        public SimpleChatClient(IPAddress ip, int port)
            : base(ip, port)
        {
            Ctor();
        }

        /// <summary>
        /// Konstruktor, ktery preda promenne do lokalnich promennych 
        /// a inicializuje instanci klienta.
        /// </summary>
        /// <param name="ip">ip adresa serveru</param>
        /// <param name="port">cislo portu serveru</param>
        /// <param name="converter">konverter</param>
        public SimpleChatClient(IPAddress ip, int port, IByteToStringConverter converter)
            : base(ip, port, converter)
        {
            Ctor();
        }

        /// <summary>
        /// Konstruktor, ktery preda promenne do lokalnich promennych 
        /// a inicializuje instanci klienta.
        /// </summary>
        /// <param name="ip">ip adresa serveru</param>
        /// <param name="port">cislo portu serveru</param>
        /// <param name="converter">konverter</param>
        /// <param name="creator">tovarni trida, vytvarejici instance cteci smycky</param>
        public SimpleChatClient(IPAddress ip, int port, IByteToStringConverter converter, IReadingLoopCreator creator)
            : base(ip, port, converter)
        {
            Ctor();
        }

        /// <summary>
        /// Pomocna metoda pro konstruktory, 
        /// aby se veskera inicializace dela na jednom miste v tride.
        /// </summary>
        private void Ctor()
        {
            _client = new TcpClient();
            ServerSocketConnected += OnServerSocketConnected;
        }

        #endregion // Constructors

        #region Public methods

        /// <summary>
        /// Pripojeni na server.
        /// </summary>
        public virtual void Connect()
        {
            // pripojeni na server
            _client.Connect(Ip, Port);
            // vyvolani udalosti o pripojeni na server
            ServerSocketConnected(this, new SocketArgs(_client.Client));
        }

        /// <summary>
        /// Odeslani zpravy na server.
        /// </summary>
        /// <param name="message">zprava</param>
        public virtual void SendMessage(string message)
        {
            this.Send(_client.Client, message);
        }

        #endregion // Public methods

        #region Events methods

        /// <summary>
        /// Metoda, ktera je vyvolana pri pripojeni na server.
        /// </summary>
        /// <param name="sender">odesilatel</param>
        /// <param name="e">argument</param>
        protected virtual void OnServerSocketConnected(object sender, SocketArgs e)
        {
            StartReadingOnBackground(e.Socket);
        }

        #endregion // Events methods
    }
}
