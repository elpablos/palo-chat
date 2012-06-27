using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ComplexWpfChatClientExample.Core
{
    /// <summary>
    /// Zakladni trida, ktera usnadnuje praci s vyvolanim zmeny.
    /// </summary>
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Udalost vyvolana pri zmene vlastnosti.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Vyvolani zmeny vlastnosti.
        /// </summary>
        /// <param name="propertyName">jmeno zmenene vlastnosti</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            NotifyPropertyChanged(property.GetMemberInfo().Name);
        }
    }
}
