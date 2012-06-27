using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Palo.ChatLibrary.Arguments;
using Palo.ChatLibrary.Converters;

namespace Palo.ChatLibrary.ReadingLoop
{
    /// <summary>
    /// Smycka, ktera opakovane prijima zpravy ze socketu
    /// a odpaluje udalost se zpravou.
    /// </summary>
    public class ReadingLoopThread : IReadingLoop
    {
        #region Fields

        /// <summary>
        /// Konstanta s maximalni velikosti prijateho bufferu;
        /// </summary>
        protected readonly int MAX_BUFFER_LENGTH = 1024;

        /// <summary>
        /// True, pokud mame vlakno predcasne ukoncit.
        /// </summary>
        protected bool _closing;

        /// <summary>
        /// Reference na cteci vlakno.
        /// </summary>
        protected Thread _readThread;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Reference na socket.
        /// </summary>
        public Socket Socket { get; private set; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// Udalost, kterou odpali prijata zprava.
        /// </summary>
        public event SimpleMessageHander MessageReceived = delegate { };

        /// <summary>
        /// Udalost, kterou odpali vyjimka pri cteni zprav.
        /// </summary>
        public event SocketExceptionHandler ReadingInterrupted = delegate { };

        #endregion // Events

        #region Constructor

        /// <summary>
        /// Konstruktor startujici vlakno urcene pro cteni.
        /// </summary>
        public ReadingLoopThread()
        {
            // vytvorime nove vlakno
            _readThread = new Thread(StartReading);
            // nastavime vlakno na pozadi
            _readThread.IsBackground = true;
        }

        /// <summary>
        /// Konstruktor startujici vlakno urcene pro cteni s moznosti prednastaveni velikosti bufferu.
        /// </summary>
        /// <param name="maxBuffer">maximalni velikost prijimaneho bufferu</param>
        public ReadingLoopThread(int maxBuffer) : this()
        {
            // upraveni maximalni hodnoty posilaneho bufferu
            MAX_BUFFER_LENGTH = maxBuffer;
        }

        #endregion // Constructor

        #region Public methods

        /// <summary>
        /// Nastartuje vlakno.
        /// </summary>
        /// <param name="socket">socket, ze kteryho vycitame</param>
        public virtual void Start(Socket socket)
        {
            Socket = socket;
            // spustime vlakno
            _readThread.Start();
        }

        /// <summary>
        /// Ukonceni smycky.
        /// </summary>
        public virtual void Closing()
        {
            // nastavime closing na true, a jakmile bude donactena posledni zprava, tak se smycka ukonci
            _closing = true;
        }

        #endregion // Public methods

        #region Loop methods

        /// <summary>
        /// Nekonecna smycka, ktera vycita prijate zpravy.
        /// </summary>
        protected virtual void StartReading()
        {
            try
            {
                // nekonecna smycka vycitani ze socketu
                while (!_closing)
                {
                    // inicializujeme pole bytu
                    byte[] buffer = new byte[MAX_BUFFER_LENGTH];
                    // prijmeme zpravu
                    int length = Socket.Receive(buffer);

                    // prevedeme pole na textovy retezec
                    string str = new ByteToStringConverter().Convert(buffer, length);

                    // odpalime udalost o prijeti
                    MessageReceived(this, new SimpleMessageArgs(str));

                    // vlozime minimalni pauzu, abychom zbytecne nevytezovali procesor
                    // netreba, Receive uplne pozastavi vlakno a procesor neni vytizen
                    // Thread.Sleep(500);
                }
            }
            catch (SocketException ex)
            {
                // vypis vyjimky
                Console.Error.WriteLine(ex.Message);
                // odpaleni vyjimky
                ReadingInterrupted(this, new SocketExceptionArgs(ex, Socket));
            }
            // po ukonceni smycky po sobe uklidime
            CleanUp();
        }

        #endregion // Loop methods

        #region CleanUp methods

        /// <summary>
        /// Uklidime po sobe.
        /// </summary>
        protected virtual void CleanUp()
        {
            // vynulujeme referenci na socket
            // socket zamerne nedisposuju, protoze ho pouzivam i jinde
            Socket = null;
        }

        #endregion // CleanUp methods
    }
}
