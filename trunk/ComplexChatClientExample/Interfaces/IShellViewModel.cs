using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ComplexWpfChatClientExample.Views;
using Palo.ChatLibrary;
using Palo.ChatLibrary.Arguments;

namespace ComplexWpfChatClientExample.Interfaces
{
    public interface IShellViewModel
    {
        IShellView View { get; set; }
    }
}
