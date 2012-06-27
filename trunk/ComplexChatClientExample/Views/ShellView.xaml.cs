using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ComplexWpfChatClientExample.Interfaces;
using ComplexWpfChatClientExample.Models;

namespace ComplexWpfChatClientExample.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IShellView
    {
        #region Properties

        /// <summary>
        /// Reference na ViewModel.
        /// </summary>
        public IShellViewModel ViewModel { get { return (IShellViewModel)DataContext; } }

        #endregion // Properties

        #region Constructor

        /// <summary>
        /// Konstruktor okna.
        /// </summary>
        public ShellView()
        {
            InitializeComponent();

            ViewModel.View = this;
        }

        #endregion // Constructor

        #region Public methods

        /// <summary>
        /// Otevreni okna pro logovani.
        /// </summary>
        public bool ShowLoginWindow(ref LoginModel model)
        {
            ILoginView wind = new LoginView(this, model);
            wind.ShowDialog();
            return wind.Model == null;
        }

        #endregion // Public methods
    }
}
