using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ComplexWpfChatClientExample.Core;
using ComplexWpfChatClientExample.Interfaces;
using Palo.ChatLibrary;

namespace ComplexWpfChatClientExample.ViewModels
{
    public class LoginViewModel : BaseViewModel, ILoginViewModel
    {
        #region Binding Properties

        private int _serverPort;
        /// <summary>
        /// Port chat serveru.
        /// </summary>
        public int ServerPort
        {
            get { return _serverPort; }
            set { if (_serverPort != value) _serverPort = value;
                NotifyPropertyChanged(() => ServerPort); 
                NotifyPropertyChanged(() => CanConnect); }
        }

        private string _serverAddress;
        /// <summary>
        /// Adresa chat serveru.
        /// </summary>
        public string ServerAddress
        {
            get { return _serverAddress; }
            set { if (_serverAddress != value) _serverAddress = value;
                NotifyPropertyChanged(() => ServerAddress);
                NotifyPropertyChanged(() => CanConnect); }
        }

        private string _nickName;
        /// <summary>
        /// Prezdivka.
        /// </summary>
        public string NickName
        {
            get { return _nickName; }
            set { if (_nickName != value) _nickName = value;
                NotifyPropertyChanged(() => NickName);
                NotifyPropertyChanged(() => CanConnect); }
        }

        public ILoginView View { get; set; }

        public Models.LoginModel Model { get; set; }

        #endregion // Binding Properties

        #region Guard Properties

        public bool CanConnect
        {
            get 
            { 
                return ServerPort != 0 
                && !string.IsNullOrEmpty(ServerAddress) 
                && !string.IsNullOrEmpty(NickName); 
            }
        }

        #endregion // Guard Properties

        #region Commands

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

        private ICommand _closeCommand;
        /// <summary>
        /// Prikaz, ktery vraci akci, ktera odesila zpravy.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand<object>((obj) => CloseAction(obj));
                return _closeCommand;
            }
        }

        #endregion //  Commands

        #region Constructor

        /// <summary>
        /// Pokus o vytazeni dat z configu.
        /// </summary>
        public LoginViewModel()
        {
            string temp;
            if (AppSettingsReader.TryReadProperty("ServerAddress", out temp))
            {
                ServerAddress = temp;
            }
            int tempInt;
            if (AppSettingsReader.TryReadProperty("ServerPort", out tempInt))
            {
                ServerPort = tempInt;
            }
            if (AppSettingsReader.TryReadProperty("NickName", out temp))
            {
                NickName = temp;
            }
        }

        #endregion

        #region Action methods

        /// <summary>
        /// Pripojeni na server.
        /// </summary>
        /// <param name="obj"></param>
        public void ConnectAction(object obj)
        {
            try
            {
                // vytvoreni instance
                var he = Dns.GetHostEntry(ServerAddress);
                IPAddress ip = he.AddressList.FirstOrDefault(a => AddressFamily.InterNetwork == a.AddressFamily) ?? he.AddressList[0];

                if (Model == null) 
                { 
                    Model = new Models.LoginModel();
                }
                // predani parametru
                Model.ServerAddress = ip;
                Model.Port = ServerPort;
                Model.NickName = NickName;
                // ukonceni okna
                View.Close();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Uzavreni okna s prihlasenim.
        /// </summary>
        /// <param name="obj"></param>
        public void CloseAction(object obj)
        {
            Model = null;
            View.Close();
        }

        #endregion // Action methods
    }
}
