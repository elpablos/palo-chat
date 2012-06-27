using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Palo.ChatLibrary.Shared
{
    /// <summary>
    /// Objekt, ktery reprezentuje zpravu chatu.
    /// </summary>
    public class ChatMessage
    {
        #region Properties

        /// <summary>
        /// Unikatni identifikacni cislo zpravy.
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// Typ zpravy.
        /// </summary>
        public ChatMessageType Type { get; private set; }

        /// <summary>
        /// Komu je zprava urcena.
        /// </summary>
        public Guid To { get; private set; }

        /// <summary>
        /// Od koho je zprava dorucena.
        /// </summary>
        public Guid From { get; set; }

        /// <summary>
        /// Zprava.
        /// </summary>
        public string Message { get; set; }

        #endregion // Properties

        #region Constructor

        /// <summary>
        /// Privatni konstruktor pro potreby deserializace JSONu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="message"></param>
        [JsonConstructor]
        private ChatMessage(Guid id, ChatMessageType type, Guid to, Guid from, string message)
        {
            ID = id;
            Type = type;
            To = to;
            From = from;
            Message = message;
        }

        /// <summary>
        /// Konstruktor, ktery zapouzdruje zpravu.
        /// </summary>
        /// <param name="type">typ zpravy</param>
        /// <param name="message">zprava</param>
        /// <param name="to">komu je urcena</param>
        public ChatMessage(ChatMessageType type, string message, Guid to)
        {
            // vygenerujeme ID zpravy
            ID = Guid.NewGuid();
            Type = type;
            Message = message;
            To = to;
        }

        #endregion // Constructor
    }
}
