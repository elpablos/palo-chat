using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexWpfChatServerExample
{
    /// <summary>
    /// Rozsireni pro vyctovy typ.
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// Umozneni inline zapisu ForEach funkce.
        /// </summary>
        /// <typeparam name="T">typ prvku uvnitr vyctu</typeparam>
        /// <param name="enumeration">vycet, nad kterym akci provadime</param>
        /// <param name="action">akce</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
	        {
                action(item);
	        }
        }
    }
}
