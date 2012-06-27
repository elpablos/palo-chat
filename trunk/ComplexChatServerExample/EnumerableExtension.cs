using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComplexWpfChatServerExample
{
    public static class EnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
	        {
                action(item);
	        }
        }
    }
}
