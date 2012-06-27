using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexWpfChatClientExample.Models;

namespace ComplexWpfChatClientExample.Interfaces
{
    public interface IShellView
    {
        IShellViewModel ViewModel { get; }

        bool ShowLoginWindow(ref LoginModel model);
    }
}
