using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Palo.ChatLibrary;
using Palo.ChatLibrary.ReadingLoop;
using Palo.ChatLibrary.Shared;
using Palo.ChatLibrary.UserManagements;

namespace ComplexWpfChatServerExample
{
    public class Server
    {
        #region Properties

        /// <summary>
        /// Instance chat serveru.
        /// </summary>
        public SimpleChatServer ChatServer { get; private set; }

        /// <summary>
        /// Kolekce prihlasenych uzivatelu.
        /// </summary>
        public ICollection<LoggedUser> UserList { get; private set; }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public Server(IPAddress address, int port)
        {
            // inicializujeme chat server
            ChatServer = new SimpleChatServer(address, port);
            ChatServer.MessageReceived += ChatServer_MessageReceived;
            ChatServer.ReadingInterrupted += ChatServer_ReadingInterrupted;
            // inicializujeme kolekci uzivatelu
            UserList = new HashSet<LoggedUser>();
        }

        #endregion // Constructors

        #region Public methods

        /// <summary>
        /// Zapocni naslouchani.
        /// </summary>
        public void StartListen()
        {
            ChatServer.StartListen();
        }

        #endregion // Public methods

        #region Event methods

        /// <summary>
        /// Reakce na vyjimku pri prijimani zpravy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ChatServer_ReadingInterrupted(object sender, Palo.ChatLibrary.Arguments.SocketExceptionArgs e)
        {
            Socket sock = ((IReadingLoop)sender).Socket;
            LoggedUser user = UserList.FirstOrDefault(u => u.Sock == sock);
            if (user != null)
            {
                ChatMessage msg = new ChatMessage(ChatMessageType.LOGOUT, JsonConvert.SerializeObject(user), Guid.Empty);
                msg.From = user.Id;
                string msgStr = JsonConvert.SerializeObject(msg);
                UserList.Remove(user);
                UserList.ForEach(u => ChatServer.SendMessage(u.Sock, msgStr));
            }

        }

        /// <summary>
        /// Reakce na prijeti zpravy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ChatServer_MessageReceived(object sender, Palo.ChatLibrary.Arguments.SimpleMessageArgs e)
        {
            ProcessMessage(e.Message, ((IReadingLoop)sender).Socket);
        }

        #endregion // Event methods

        #region Helper methods

        /// <summary>
        /// Chovani serveru dle typu zpravy.
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessMessage(string message, Socket sock)
        {
            ChatMessage msg = null;
            // deseralizujeme objekt
            try
            {
                msg = JsonConvert.DeserializeObject<ChatMessage>(message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                ChatServer.SendMessage(sock, "Unknown format of message");
                return;
            }

            // pokud uzivatel neni prihlasen a zprava neni typu login, tak mu odesleme zpravu a konec
            if (!CheckIsUserLogged(sock) && !(msg.Type == ChatMessageType.LOGIN))
            {
                ChatServer.SendMessage(sock, "You must login before");
                return;
            }

            // chovani dle typu zpravy
            switch (msg.Type)
            {
                case ChatMessageType.LOGIN:
                    // pridame uzivatele do kolekce
                    var loggedUser = new LoggedUser(msg.Message, sock);
                    msg.Message = JsonConvert.SerializeObject(loggedUser);
                    message = JsonConvert.SerializeObject(msg);
                    UserList.Add(loggedUser);
                    UserList.ForEach(u => ChatServer.SendMessage(u.Sock, message));
                    break;
                case ChatMessageType.LOGOUT:
                    // odeberemee uzivatele do kolekce
                    var user = UserList.First(u => u.Sock == sock);
                    UserList.Remove(user);
                    msg.Message = JsonConvert.SerializeObject(user);
                    message = JsonConvert.SerializeObject(msg);
                    UserList.ForEach(u => ChatServer.SendMessage(u.Sock, message));
                    break;
                case ChatMessageType.ALL_MSG:
                    // preposleme zpravu na vsechny
                    msg.From = UserList.First(u => u.Sock == sock).Id;
                    message = JsonConvert.SerializeObject(msg);
                    UserList.ForEach(u => ChatServer.SendMessage(u.Sock, message));
                    break;
                case ChatMessageType.PRIV_MSG:
                    // preposleme zpravu jen na konkretni osobu
                    msg.From = UserList.First(u => u.Sock == sock).Id;
                    message = JsonConvert.SerializeObject(msg);
                    ChatServer.SendMessage(UserList.First(u => u.Id == msg.To).Sock, message);
                    break;
                case ChatMessageType.USERS:
                    // posleme seznam uzivatelu
                    msg.Message = JsonConvert.SerializeObject(UserList);
                    message = JsonConvert.SerializeObject(msg);
                    ChatServer.SendMessage(sock, message);
                    break;
                case ChatMessageType.PING:
                    // zkusime ping
                    msg.From = UserList.First(u => u.Sock == sock).Id;
                    message = JsonConvert.SerializeObject(msg);
                    ChatServer.SendMessage(UserList.First(u => u.Id == msg.To).Sock, message);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Kontrola, zdali existuje prihlaseny uzivatel z daneho socketu.
        /// </summary>
        /// <param name="sock">socket</param>
        /// <returns>true, pokud ano</returns>
        private bool CheckIsUserLogged(Socket sock)
        {
            return UserList.Any(u => u.Sock == sock);
        }

        #endregion // Helper methods
    }
}
