﻿using System;
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

        System.Windows.Threading.DispatcherTimer Timer { get; set; }

        #endregion // Properties

        #region Binding Properties

        private int _timerMessage;
        /// <summary>
        /// Odpocet do pingu serveru.
        /// </summary>
        public int TimerMessage
        {
            get { return _timerMessage; }
            set
            {
                if (_timerMessage != value)
                    _timerMessage = value;
                _timerMessage = value;
                NotifyPropertyChanged(() => TimerMessage);
            }
        }

        private string _checkMessage;
        /// <summary>
        /// Odezva na server.
        /// </summary>
        public string CheckMessage
        {
            get { return _checkMessage; }
            set
            {
                if (_checkMessage != value)
                    _checkMessage = value;
                _checkMessage = value;
                NotifyPropertyChanged(() => CheckMessage);
            }
        }

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

        private ObservableCollection<ChatMessageModel> _message;
        /// <summary>
        /// Kolekce vsech zobrazovanych zprav.
        /// </summary>
        public ObservableCollection<ChatMessageModel> Message
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
                NotifyPropertyChanged(() => CanPing);
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
            get { return ChatClient != null && PingMessage == null && SelectedUser != null; }
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
            Message = new ObservableCollection<ChatMessageModel>();
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
                MessageBox.Show(string.Format("Server is down!\n{0}", ex.Message), "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                ChatClient = null;
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
                MessageBox.Show(string.Format("Server is down!\n{0}", ex.Message), "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                ChatClient = null;
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
                        MessageBox.Show(string.Format("Server is down!\n{0}", ex.Message), "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                        ChatClient = null;
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
                    MessageBox.Show(string.Format("Server is down!\n{0}", ex.Message), "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChatClient = null;
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
                MessageBox.Show(string.Format("Server is down!\n{0}", ex.Message), "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                ChatClient = null;
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
            try
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
                        InsertMessage(msg);
                        break;
                    case ChatMessageType.PRIV_MSG:
                        // soukrome zpravy
                        InsertMessage(msg);
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
                            // kontrola serveru
                            if (string.IsNullOrEmpty(pingUser))
                            {
                                CheckMessage = string.Format("Odezva na server: {0} ms", (int)ping.TotalMilliseconds);
                            }
                            else
                            {
                                msg.Message = string.Format("Odezva na uživatele {1}: {0} ms", (int)ping.TotalMilliseconds, pingUser ?? "Server");
                                InsertMessage(msg);
                            }

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
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(string.Format("Server is down!\n{0}", ex.Message), "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                ChatClient = null;
            }
        }

        private void InsertMessage(ChatMessage msg)
        {
            var from = User.FirstOrDefault(u => u.Id == msg.From).DisplayName ?? "Všichni";
            var to = User.FirstOrDefault(u => u.Id == msg.To).DisplayName ?? "Všichni";
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                            () =>
                                // pridame zpravu do kolekce
                            Message.Add(new ChatMessageModel() { From = from, Message = msg.Message, To = to })
                        ));
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
                if (Timer != null) Timer.Stop();
                Timer = null;
            }
            else
            {
                ConnectButtonName = "Disconnect";
                Timer = new DispatcherTimer();
                Timer.Interval = TimeSpan.FromSeconds(1);
                Timer.Tick += Timer_Tick;
                Timer.Start();
                TimerMessage = 20;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimerMessage--;
            if (TimerMessage <= 0)
            {
                try
                {
                    PingMessage = new ChatMessage(ChatMessageType.PING, null, Guid.Empty);
                    ChatClient.SendMessage(JsonConvert.SerializeObject(PingMessage));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    MessageBox.Show(string.Format("Server is down!\n{0}", ex.Message), "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    ChatClient = null;
                }
                TimerMessage = 20;
            }
        }

        #endregion // Helper methods
    }
}
