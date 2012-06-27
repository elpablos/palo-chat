using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ComplexWpfChatClientExample.Views;

namespace ComplexWpfChatClientExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Prepsana metoda, ktera reaguje na start aplikace.
        /// Po startu spustime hlavni okno.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // spusteni hlavniho okna
            Window wind = new ShellView();
            wind.Show();
        }
    }
}
