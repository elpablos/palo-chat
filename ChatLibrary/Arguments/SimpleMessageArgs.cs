using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Palo.ChatLibrary.Arguments
{
    /// <summary>
    /// Predpis pro akci vracejici textovou zpravu v argumentu.
    /// </summary>
    /// <param name="sender">odesilatel</param>
    /// <param name="e">argument</param>
    public delegate void SimpleMessageHander(object sender, SimpleMessageArgs e);

    /// <summary>
    /// Argument s textovou zpravou.
    /// </summary>
    public class SimpleMessageArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Textova zprava.
        /// </summary>
        public string Message { get; private set; }

        #endregion // Properties

        #region Constructor

        /// <summary>
        /// Konstruktor ukladajici referenci na textovou zpravu
        /// do lokalni promenne argumentu.
        /// </summary>
        /// <param name="message">textova zprava</param>
        public SimpleMessageArgs(string message)
        {
            Message = message;
        }

        #endregion // Constructor
    }
}
