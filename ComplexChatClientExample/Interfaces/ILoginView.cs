using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexWpfChatClientExample.Models;

namespace ComplexWpfChatClientExample.Interfaces
{
    public interface ILoginView
    {
        LoginModel Model { get; }

        void Close();

        bool? ShowDialog();
    }
}
