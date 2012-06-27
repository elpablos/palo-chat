using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Palo.ChatLibrary.Shared;
using System.Net.Sockets;
using Palo.ChatLibrary.Arguments;
using Palo.ChatLibrary.Converters;
using Palo.ChatLibrary.ReadingLoop;

namespace Palo.ChatLibrary
{
    /// <summary>
    /// Predek vsech chat klient/serveru.
    /// Implementuje vsechny spolecne prvky jako 
    /// odesilani zpravy, prijimani zprav ve vlakne na pozadi.
    /// </summary>
    public abstract class ChatSocket
    {
        #region Fields

        /// <summary>
        /// Konstanta s maximalni velikosti prijateho bufferu;
        /// </summary>
        protected static readonly int MAX_BUFFER_LENGTH = 1024;

        /// <summary>
        /// Vlakno, ktere vycita zpravy ze site.
        /// </summary>
        protected ReadingLoopThread _readingLoop;

        /// <summary>
        /// True, pokud chceme ukoncit vlakno serveru/klienta.
        /// </summary>
        protected bool _closing;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Adresa serveru.
        /// </summary>
        public IPAddress Ip { get; private set; }

        /// <summary>
        /// Port serveru.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Reference na konverter.
        /// </summary>
        public IByteToStringConverter Converter { get; private set; }

        /// <summary>
        /// Reference na faktory tridu vracejici instanci cteci smycky.
        /// </summary>
        public IReadingLoopCreator ReadingLoopCreator { get; private set; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// Udalost, ktera pripojeni na server.
        /// </summary>
        public event SimpleMessageHander MessageReceived = delegate { };

        /// <summary>
        /// Udalost, kterou odpali vyjimka pri cteni zprav.
        /// </summary>
        public event SocketExceptionHandler ReadingInterrupted = delegate { };

        #endregion // Events

        #region Constructors

        /// <summary>
        /// Konstruktor inicializujici instanci serveru a uklada predane reference
        /// do lokalnich promennych.
        /// Konverter je defaultni ByteToString.
        /// </summary>
        /// <param name="ip">adresa serveru</param>
        /// <param name="port">port serveru</param>
        public ChatSocket(IPAddress ip, int port)
            : this(ip, port, new ByteToStringConverter())
        {
        }

        /// <summary>
        /// Konstruktor inicializujici instanci serveru a uklada predane reference
        /// do lokalnich promennych.
        /// </summary>
        /// <param name="ip">adresa serveru</param>
        /// <param name="port">port serveru</param>
        /// <param name="converter">konverter</param>
        public ChatSocket(IPAddress ip, int port, IByteToStringConverter converter)
            : this(ip, port, converter, new ReadingLoopCreator())
        {
        }

        /// <summary>
        /// Konstruktor inicializujici instanci serveru a uklada predane reference
        /// do lokalnich promennych.
        /// </summary>
        /// <param name="ip">adresa serveru</param>
        /// <param name="port">port serveru</param>
        /// <param name="converter">konverter</param>
        /// <param name="creator">tovarni trida, vytvarejici instance cteci smycky</param>
        public ChatSocket(IPAddress ip, int port, IByteToStringConverter converter, IReadingLoopCreator creator)
        {
            // predani parametru
            Ip = ip;
            Port = port;
            Converter = converter;
            ReadingLoopCreator = creator;
            // registrace metody na udalost
            MessageReceived += OnMessageReceived;
            ReadingInterrupted += OnReadingInterrupted;
        }

        #endregion // Constructors

        #region Public methods

        /// <summary>
        /// Metoda, ktera ukoncuje funkci serveru/klienta.
        /// </summary>
        public virtual void Close()
        {
            _closing = true;
        }

        #endregion // Public methods

        #region Reading loop

        /// <summary>
        /// Metoda, ktera nastartuje cteni zprav ve vlakne na pozadi.
        /// Vsechny metody budou vyvolany pomoci OnMessageReceived.
        /// </summary>
        /// <param name="socket">socket</param>
        protected virtual void StartReadingOnBackground(Socket socket)
        {
            IReadingLoop loop = ReadingLoopCreator.FactoryMethod();
            loop.MessageReceived += MessageReceived;
            loop.ReadingInterrupted += ReadingInterrupted;
            loop.Start(socket);
        }

        #endregion // Reading loop

        #region Event methods

        /// <summary>
        /// Metoda, ktera je vyvolana pri vyskytu chyby pri prijmu zprav.
        /// </summary>
        /// <param name="sender">odesilatel</param>
        /// <param name="e">argument</param>
        protected virtual void OnReadingInterrupted(object sender, SocketExceptionArgs e)
        { }

        /// <summary>
        /// Metoda, ktera je vyvolana po prijeti zpravy.
        /// </summary>
        /// <param name="sender">odesilatel</param>
        /// <param name="e">argument</param>
        protected virtual void OnMessageReceived(object sender, SimpleMessageArgs e)
        { }

        #endregion // Event methods

        #region Helper methods

        /// <summary>
        /// Odesilani zprav na dany socket.
        /// </summary>
        /// <param name="socket">socket</param>
        /// <param name="message">zprava</param>
        protected virtual void Send(Socket socket, string message)
        {
            //if (string.IsNullOrEmpty(message))
            //    throw new ArgumentNullException("message");

            socket.Send(Converter.ConvertBack(message));
        }

        #endregion // Helper methods
    }
}
