using System;
using Palo.ChatLibrary.Arguments;
using System.Net.Sockets;
namespace Palo.ChatLibrary.ReadingLoop
{
    /// <summary>
    /// Rozhrani vlakna, ktere vycita zpravy na socketu a odpaluje pritom udalost prijeti.
    /// </summary>
    public interface IReadingLoop
    {
        /// <summary>
        /// Mekky ukonceni vlakna.
        /// Nejprve se dokonci smycka a az pote dojde k ukonceni.
        /// </summary>
        void Closing();

        /// <summary>
        /// Udalost, ktera je vyvolana prijetim zpravy.
        /// </summary>
        event SimpleMessageHander MessageReceived;

        /// <summary>
        /// Udalost, kterou odpali vyjimka pri cteni zprav.
        /// </summary>
        event SocketExceptionHandler ReadingInterrupted;

        /// <summary>
        /// Vlastnost uchovavajici reference na socket, ze ktereho cteme zpravy.
        /// </summary>
        Socket Socket { get; }

        /// <summary>
        /// Metoda, ktera odstartuje cteni.
        /// </summary>
        /// <param name="socket">socket, ze kteryho budeme cist</param>
        void Start(Socket socket);
    }
}
