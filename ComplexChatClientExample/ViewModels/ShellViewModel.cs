using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ComplexWpfChatClientExample.Core;
using ComplexWpfChatClientExample.Interfaces;
using ComplexWpfChatClientExample.Models;
using ComplexWpfChatClientExample.Views;
using Newtonsoft.Json;
using Palo.ChatLibrary;
using Palo.ChatLibrary.Arguments;
using Palo.ChatLibrary.Shared;
using Palo.ChatLibrary.UserManagements;

namespace ComplexWpfChatClientExample.ViewModels
{
    /// <summary>
    /// ViewModel chatovaciho klienta.
    /// Zajisteni komunikace s uzivateli, pripojeni na server aj.
    /// </summary>
    public class ShellViewModel : BaseViewModel, IShellViewModel
    {
        #region Properties

        public IShellView View { get; set; }

        public ChatMessage LoginMessage { get; private set; }

        #endregion // Properties

        #region Binding Properties

        private ChatMessage _pingMessage;
        /// <summary>
        /// Reference na zpravu pingu.
        /// </summary>
        public ChatMessage PingMessage
        {
            get { return _pingMessage; }
            private set
            {
                if (_pingMessage != value)
                    _pingMessage = value;
                NotifyPropertyChanged(() => PingMessage);
                NotifyPropertyChanged(() => CanPing);
            }
        }

        private SimpleChatClient _chatClient;
        /// <summary>
        /// Instance socket klienta.
        /// </summary>
        public SimpleChatClient ChatClient
        {
            get { return _chatClient; }
            set
            {
                if (_chatClient != value) 
                    _chatClient = value;
                NotifyPropertyChanged(() => ChatClient);
                NotifyPropertyChanged(() => CanRefresh);
                NotifyPropertyChanged(() => CanPing);
                OnChatClientSet();
            }
        }

        private ObservableCollection<ChatMessage> _message;
        /// <summary>
        /// Kolekce vsech zobrazovanych zprav.
        /// </summary>
        public ObservableCollection<ChatMessage> Message
        {
            get { return _message; }
            set
            {
                if (_message != value) _message = value;
                NotifyPropertyChanged(() => Message);
            }
        }

        private ObservableCollection<LoggedUser> _user;
        /// <summary>
        /// Kolekce vsech prihlasenych uzivatelu.
        /// </summary>
        public ObservableCollection<LoggedUser> User
        {
            get { return _user; }
            set
            {
                if (_user != value) _user = value;
                NotifyPropertyChanged(() => User);
            }
        }

        private LoggedUser _selectedUser;
        /// <summary>
        /// Reference na vybraneho uzivatele.
        /// </summary>
        public LoggedUser SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value) _selectedUser = value;
                NotifyPropertyChanged(() => SelectedUser);
            }
        }

        private LoggedUser _currentUser;
        /// <summary>
        /// Prave prihlaseny uzivatel.
        /// </summary>
        public LoggedUser CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser != value) _currentUser = value;
                NotifyPropertyChanged(() => CurrentUser);
            }
        }

        private string _userMessage;
        /// <summary>
        /// Zprava, kterou posila uzivatel.
        /// </summary>
        public string UserMessage
        {
            get { return _userMessage; }
            set
            {
                if (_userMessage != value) _userMessage = value;
                NotifyPropertyChanged(() => UserMessage);
                NotifyPropertyChanged(() => CanSend);
            }
        }

        private string _connectButtonName;
        /// <summary>
        /// Nazev tlacitka "Connect".
        /// </summary>
        public string ConnectButtonName
        {
            get { return _connectButtonName; }
            set
            {
                if (_connectButtonName != value)
                    _connectButtonName = value;
                NotifyPropertyChanged(() => ConnectButtonName);
            }
        }

        #endregion // Binding Properties

        #region Guard Properties

        public bool CanRefresh
        {
            get { return ChatClient != null; }
        }

        public bool CanSend
        {
            get { return ChatClient != null && !string.IsNullOrEmpty(UserMessage); }
        }

        public bool CanPing
        {
            get { return ChatClient != null && PingMessage == null; }
        }

        #endregion // Guard Properties

        #region Commands

        private ICommand _pingCommand;
        /// <summary>
        /// Prikaz, ktery vraci akci na kontrolu odezvy.
        /// </summary>
        public ICommand PingCommand
        {
            get
            {
                if (_pingCommand == null)
                    _pingCommand = new RelayCommand<object>((obj) => PingAction(obj));
                return _pingCommand;
            }
        }

        private ICommand _connectCommand;
        /// <summary>
        /// Prikaz, ktery vraci akci pripojeni na server.
        /// </summary>
        public ICommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                    _connectCommand = new RelayCommand<object>((obj) => ConnectAction(obj));
                return _connectCommand;
            }
        }

        private ICommand _sendCommand;
        /// <summary>
        /// Prikaz, ktery vraci akci, ktera odesila zpravy.
        /// </summary>
        public ICommand SendCommand
        {
            get
            {
                if (_sendCommand == null)
                    _sendCommand = new RelayCommand<object>((obj) => SendAction(obj));
                return _sendCommand;
            }
        }

        private ICommand _refreshCommand;
        /// <summary>
        /// Prikaz, ktery obnovuje seznam uzivatelu.
        ///  </summary>
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                    _refreshCommand = new RelayCommand<object>((obj) => RefreshAction(obj));
                return _refreshCommand;
            }
        }

        #endregion //  Commands

        #region Constructor

        /// <summary>
        /// Inicializace ViewModelu.
        /// </summary>
        public ShellViewModel()
        {
            Message = new ObservableCollection<ChatMessage>();
            User = new ObservableCollection<LoggedUser>();
            OnChatClientSet();
        }

        #endregion // Constructor

        #region Action methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void PingAction(object obj)
        {
            try
            {
                PingMessage = new ChatMessage(ChatMessageType.PING, UserMessage, SelectedUser.Id);
                ChatClient.SendMessage(JsonConvert.SerializeObject(PingMessage));
                // vyprazdneni zasobniku
                UserMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Obnoveni seznamu uzivatelu.
        /// </summary>
        /// <param name="obj"></param>
        public void RefreshAction(object obj)
        {
            try
            {
                ChatMessage msg = new ChatMessage(ChatMessageType.USERS, string.Empty, Guid.Empty);
                ChatClient.SendMessage(JsonConvert.SerializeObject(msg));
                // vyprazdneni zasobniku
                UserMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Pripojeni na server.
        /// </summary>
        /// <param name="obj"></param>
        public void ConnectAction(object obj)
        {
            if (ChatClient == null)
            {
                LoginModel model = new LoginModel();
                if (!View.ShowLoginWindow(ref model))
                {
                    try
                    {
                        // zalozeni klienta
                        ChatClient = new SimpleChatClient(model.ServerAddress, model.Port);
                        ChatClient.MessageReceived += OnMessageReceived;
                        ChatClient.Connect();
                        // odeslani prihlaseni
                        LoginMessage = new ChatMessage(ChatMessageType.LOGIN, model.NickName, Guid.Empty);
                        ChatClient.SendMessage(JsonConvert.SerializeObject(LoginMessage));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                try
                {
                    // odeslani prihlaseni
                    ChatMessage msg = new ChatMessage(ChatMessageType.LOGOUT, string.Empty, Guid.Empty);
                    ChatClient.SendMessage(JsonConvert.SerializeObject(msg));
                    // ukonceni chatu
                    ChatClient.Close();
                    // vymazani referenci
                    ChatClient = null;
                    User = new ObservableCollection<LoggedUser>();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Odesilani zprav.
        /// </summary>
        /// <param name="obj"></param>
        public void SendAction(object obj)
        {
            try
            {
                Guid to = Guid.Empty;
                ChatMessageType typ = ChatMessageType.ALL_MSG;
                if (!SelectedUser.IsEmpty)
                {
                    to = SelectedUser.Id;
                    typ = ChatMessageType.PRIV_MSG;
                }
                ChatMessage msg = new ChatMessage(typ, UserMessage, to);
                ChatClient.SendMessage(JsonConvert.SerializeObject(msg));
                // vyprazdneni zasobniku
                UserMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion // Action methods

        #region Event methods

        /// <summary>
        /// Reakce na prijeti zpravy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnMessageReceived(object sender, SimpleMessageArgs e)
        {
            ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(e.Message);

            LoggedUser user = null;
            switch (msg.Type)
            {
                case ChatMessageType.LOGIN:
                    // pripojeni uzivatele
                    if (msg.ID == LoginMessage.ID)
                    {
                        CurrentUser = JsonConvert.DeserializeObject<LoggedUser>(msg.Message);
                        RefreshAction(null);
                    }
                    else
                    {
                        user = JsonConvert.DeserializeObject<LoggedUser>(msg.Message);
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                                 () =>
                                     // pridame uzivatele do kolekce
                     User.Add(user)
                     ));
                    }
                    break;
                case ChatMessageType.LOGOUT:
                    // odhlaseni uzivatele
                    user = JsonConvert.DeserializeObject<LoggedUser>(msg.Message);
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        // odebereme uzivatele z kolekce
                        User.Remove(user)
                    ));
                    break;
                case ChatMessageType.ALL_MSG:
                    // vsechny zpravy
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        // pridame zpravu do kolekce
                        Message.Add(msg)
                    ));
                    break;
                case ChatMessageType.PRIV_MSG:
                    // soukrome zpravy
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        // pridame zpravu do kolekce
                        Message.Add(msg)
                    ));
                    break;
                case ChatMessageType.USERS:
                    // seznam uzivatelu
                    User = new ObservableCollection<LoggedUser>(
                        JsonConvert.DeserializeObject<ICollection<LoggedUser>>(msg.Message)
                        );
                    // pridani uzivatele "vsech"
                    var loggedUser = LoggedUser.Empty;
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                        User.Add(loggedUser)
                    ));
                    SelectedUser = loggedUser;
                    break;
                case ChatMessageType.PING:
                    // zprava ode mne
                    if (PingMessage != null && msg.ID == PingMessage.ID)
                    {
                        string pingUser = User.FirstOrDefault(u => u.Id == msg.From).DisplayName;
                        TimeSpan ping = DateTime.Now - msg.TimeStamp;
                        msg.Message = string.Format("Odezva na uživatele {1}: {0} ms", (int)ping.TotalMilliseconds, pingUser ?? "Server");
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        () =>
                           // pridame zpravu do kolekce
                        Message.Add(msg)
                        ));
                        // vynulovani referenci!
                        PingMessage = null;
                    }
                    // zprava od jinych
                    else
                    {
                        // presmerujeme zpet
                        Guid tmp = msg.From;
                        msg.From = msg.To;
                        msg.To = tmp;
                        // odesleme zpet
                        ChatClient.SendMessage(JsonConvert.SerializeObject(msg));
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion // Event methods

        #region Helper methods

        /// <summary>
        /// Pri nastaveni klienta zmenime prihlasovani na odhlasovani.
        /// </summary>
        private void OnChatClientSet()
        {
            if (ChatClient == null)
            {
                ConnectButtonName = "Connect";
            }
            else
            {
                ConnectButtonName = "Disconnect";
            }
        }

        #endregion // Helper methods
    }
}
