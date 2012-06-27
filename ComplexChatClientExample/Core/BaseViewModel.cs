using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexWpfChatClientExample.Interfaces;

namespace ComplexWpfChatClientExample.Core
{
    public class BaseViewModel : PropertyChangedBase
    {
        public IShellViewModel Shell { get; set; }

        /// <summary>
        /// Metoda, ktera je volana po inicializaci VM.
        /// </summary>
        public virtual void OnReconstruct()
        {
        }
    }
}
