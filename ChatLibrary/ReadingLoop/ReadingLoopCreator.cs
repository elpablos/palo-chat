using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Palo.ChatLibrary.ReadingLoop
{
    /// <summary>
    /// Trida reprezentujici tovarnu vytvarejici instance ReadingLoopThread tridy.
    /// </summary>
    public class ReadingLoopCreator : IReadingLoopCreator
    {
        /// <summary>
        /// Metoda vracejici instanci ReadingLoopThread.
        /// </summary>
        /// <returns>instance</returns>
        public IReadingLoop FactoryMethod()
        {
            return new ReadingLoopThread();
        }
    }
}
