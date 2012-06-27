using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window, ILoginView
    {
        #region Properties

        public ILoginViewModel ViewModel { get { return (ILoginViewModel)DataContext; } }

        public LoginModel Model 
        { 
            get { return ViewModel.Model; }
            set { ViewModel.Model = value; }
        }

        #endregion // Properties

        #region Constructors

        public LoginView(Window owner, LoginModel model)
        {
            Owner = owner;
            InitializeComponent();

            Model = model;
            ViewModel.View = this;
        }

        #endregion // Constructors
    }
}
