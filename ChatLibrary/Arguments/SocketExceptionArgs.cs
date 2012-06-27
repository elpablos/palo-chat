using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Palo.ChatLibrary.Arguments
{
    /// <summary>
    /// Predpis funkce vracejici vyjimku objevenou na socketu v argumentu.
    /// </summary>
    /// <param name="sender">odesilatel</param>
    /// <param name="e">argument</param>
    public delegate void SocketExceptionHandler(object sender, SocketExceptionArgs e);

    /// <summary>
    /// Trida, reprezentujici argument uchovavajici vyjimku a socket, na kterym se problem vyskytl.
    /// </summary>
    public class SocketExceptionArgs : SocketArgs
    {
        /// <summary>
        ///Reference na vyjimku.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Predani socketu a vyjimky do argumentu.
        /// </summary>
        /// <param name="ex">vyjimka</param>
        /// <param name="socket">socket</param>
        public SocketExceptionArgs(Exception ex, Socket socket) : base(socket) 
        {
            Exception = ex;
        }
    }
}
