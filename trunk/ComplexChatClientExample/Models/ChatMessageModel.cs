using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexWpfChatClientExample.Core;

namespace ComplexWpfChatClientExample.Models
{
    public class ChatMessageModel : PropertyChangedBase
    {
        private string _from;
        /// <summary>
        /// Jmeno od koho to prislo.
        /// </summary>
        public string From
        {
            get { return _from; }
            set { if (_from != value) _from = value; 
                NotifyPropertyChanged(() => From); }
        }

        private string _to;
        /// <summary>
        /// Jmeno komu to bylo cileno.
        /// </summary>
        public string To
        {
            get { return _to; }
            set
            {
                if (_to != value) _to = value;
                NotifyPropertyChanged(() => To);
            }
        }

        private string _message;
        /// <summary>
        /// Zprava.
        /// </summary>
        public string Message 
        {
            get { return _message; }
            set { if (_message != value) _message = value; 
                NotifyPropertyChanged(() => Message); }
        }
    }
}
