using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Palo.ChatLibrary.Converters
{
    /// <summary>
    /// Prevadec pole bytu na textovy retezec a zpet.
    /// </summary>
    public class ByteToStringConverter : IByteToStringConverter 
    {
        /// <summary>
        /// Metoda, ktera konvertuje pole bytu na textovy retezec.
        /// </summary>
        /// <param name="value">pole bytu</param>
        /// <returns>textovy retezec</returns>
        public string Convert(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        /// <summary>
        /// Metoda, ktera konvertuje pole bytu na textovy retezec.
        /// Pole orizne na specifickou delku.
        /// </summary>
        /// <param name="value">pole bytu</param>
        /// <param name="length">velikost pole</param>
        /// <returns>textovy retezec</returns>
        public string Convert(byte[] value, int length)
        {
            return Encoding.UTF8.GetString(value, 0, length);
        }

        /// <summary>
        /// Metoda, ktera konvertuje textovy retezec na pole bytu.
        /// </summary>
        /// <param name="value">textovy retezec</param>
        /// <returns>pole bytu</returns>
        public byte[] ConvertBack(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
