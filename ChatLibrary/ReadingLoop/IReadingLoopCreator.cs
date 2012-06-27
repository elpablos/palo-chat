using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Palo.ChatLibrary.ReadingLoop
{
    /// <summary>
    /// Rozhrani pro factory tridu, ktera vytvori instance 
    /// IReadingLoop rozhrani.
    /// </summary>
    public interface IReadingLoopCreator
    {
        /// <summary>
        /// Metoda vracejici instanci IReadingLoop rozhrani.
        /// </summary>
        /// <returns>instance</returns>
        IReadingLoop FactoryMethod();
    }
}
