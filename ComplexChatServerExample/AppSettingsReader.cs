using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ComplexWpfChatServerExample
{
    /// <summary>
    /// Pomocna trida pro praci s AppSettings.
    /// </summary>
    public static class AppSettingsReader
    {
        /// <summary>
        /// Cteni textove hodnoty z AppSettings.
        /// </summary>
        /// <param name="name">klic</param>
        /// <returns>hodnota</returns>
        public static string ReadProperty(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        /// <summary>
        /// Pokus o precteni textove hodnoty z AppSettings.
        /// </summary>
        /// <param name="name">klic</param>
        /// <param name="prop">vystupni hodnota, pokud se cteni povede</param>
        /// <returns>true, pokud se povedlo</returns>
        public static bool TryReadProperty(string name, out string prop)
        {
            prop = string.Empty;
            try
            {
                prop = ConfigurationManager.AppSettings[name];
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cteni ciselne hodnoty z AppSettings.
        /// </summary>
        /// <param name="name">klic</param>
        /// <returns>hodnota</returns>
        public static int ReadIntProperty(string name)
        {
            return Int32.Parse(ConfigurationManager.AppSettings[name]);
        }

        /// <summary>
        /// Pokus o precteni ciselne hodnoty z AppSettings.
        /// </summary>
        /// <param name="name">klic</param>
        /// <param name="prop">vystupni hodnota, pokud se cteni povede</param>
        /// <returns>true, pokud se povedlo</returns>
        public static bool TryReadProperty(string name, out int prop)
        {
            return Int32.TryParse(ConfigurationManager.AppSettings[name], out prop);
        }
    }
}
