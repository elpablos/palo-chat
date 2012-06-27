using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Palo.ChatLibrary.Shared
{
    /// <summary>
    /// Vycet vsech typu posilanych zprav.
    /// </summary>
    public enum ChatMessageType
    {
        LOGIN,
        LOGOUT,
        ALL_MSG,
        PRIV_MSG,
        USERS,
        PING
    }

    /// <summary>
    /// Vycet vsech typu odpovedi.
    /// </summary>
    public enum ChatMessageResponse
    {
        OK,
        ERR
    }
}
