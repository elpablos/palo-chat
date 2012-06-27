using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Palo.ChatLibrary.Arguments
{
    /// <summary>
    /// Predpis funkce vracejici socket v argumentu.
    /// </summary>
    /// <param name="sender">odesilatel</param>
    /// <param name="e">argument</param>
    public delegate void SocketHandler(object sender, SocketArgs e);

    /// <summary>
    /// Trida, reprezentujici argument uchovavajici socket.
    /// </summary>
    public class SocketArgs : EventArgs
    {
        /// <summary>
        /// Reference na ulozeny socket.
        /// </summary>
        public Socket Socket { private set; get; }

        /// <summary>
        /// Predani socketu do argumentu.
        /// </summary>
        /// <param name="socket">socket</param>
        public SocketArgs(Socket socket)
        {
            Socket = socket;
        }
    }
}
