using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexWpfChatClientExample.Models;

namespace ComplexWpfChatClientExample.Interfaces
{
    public interface ILoginViewModel
    {
        ILoginView View { get; set; }
        LoginModel Model { get; set; }
    }
}
