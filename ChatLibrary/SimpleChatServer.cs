using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Palo.ChatLibrary.Shared;
using System.Threading;
using Palo.ChatLibrary.Arguments;
using Palo.ChatLibrary.Converters;
using Palo.ChatLibrary.ReadingLoop;

namespace Palo.ChatLibrary
{
    /// <summary>
    /// Trida reprezentujici jednoduchy chatovaci server.
    /// </summary>
    public class SimpleChatServer : ChatSocket
    {
        #region Fields

        /// <summary>
        /// Reference na listener.
        /// </summary>
        protected TcpListener _serverListener;

        #endregion // Fields

        #region Events

        /// <summary>
        /// Udalost, ktera nastane pri akceptaci socketu.
        /// </summary>
        public event SocketHandler SocketAccepted = delegate { };

        #endregion // Events

        #region Constructors

        /// <summary>
        /// Konstruktor inicializujici listener.
        /// Vyuziti defaultniho ByteToStringConverteru.
        /// </summary>
        /// <param name="ip">ip adresa serveru</param>
        /// <param name="port">cislo portu serveru</param>
        public SimpleChatServer(IPAddress ip, int port)
            : base(ip, port)
        {
            Ctor(ip, port);
        }

        /// <summary>
        /// Konstruktor inicializujici listener.
        /// </summary>
        /// <param name="ip">ip adresa serveru</param>
        /// <param name="port">cislo portu serveru</param>
        /// <param name="converter">konverter</param>
        public SimpleChatServer(IPAddress ip, int port, IByteToStringConverter converter)
            : base(ip, port, converter)
        {
            Ctor(ip, port);
        }

        /// <summary>
        /// Konstruktor inicializujici listener.
        /// </summary>
        /// <param name="ip">ip adresa serveru</param>
        /// <param name="port">cislo portu serveru</param>
        /// <param name="converter">konverter</param>
        public SimpleChatServer(IPAddress ip, int port, IByteToStringConverter converter, IReadingLoopCreator creator)
            : base(ip, port, converter, creator)
        {
            Ctor(ip, port);
        }

        /// <summary>
        /// Pomocna metoda pro konstruktory, 
        /// aby se veskera inicializace dela na jednom miste v tride.
        /// </summary>
        private void Ctor(IPAddress ip, int port)
        {
            _serverListener = new TcpListener(ip, port);
            SocketAccepted += OnSocketAccepted;
        }

        #endregion // Constructors

        #region Public methods

        /// <summary>
        /// Naslouchani serveru na urcenem portu.
        /// </summary>
        public virtual void StartListen()
        {
            // nastartovani serveru
            _serverListener.Start();

            // smycky, ktera prijima klienty
            while (!_closing)
            {
                // vytvoreni socketu pro kazdeho klienta
                Socket socket = _serverListener.AcceptSocket();
                // odpalime udalost akceptace socketu
                SocketAccepted(this, new SocketArgs(socket));
            }
        }

        /// <summary>
        /// Odeslani textove zpravy na dany socket.
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="message">zprava</param>
        public virtual void SendMessage(Socket socket, string message)
        {
            this.Send(socket, message);
        }

        #endregion // Public methods

        #region Events methods

        /// <summary>
        /// Metoda, ktera je vyvolana pri akceptaci socketu.
        /// </summary>
        /// <param name="sender">odesilatel</param>
        /// <param name="e">argument</param>
        protected virtual void OnSocketAccepted(object sender, SocketArgs e)
        {
            StartReadingOnBackground(e.Socket);
        }

        #endregion // Events methods
    }
}
