using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexWpfChatClientExample.Core;

namespace ComplexWpfChatClientExample.Models
{
    public class ChatMessageModel : PropertyChangedBase
    {
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
