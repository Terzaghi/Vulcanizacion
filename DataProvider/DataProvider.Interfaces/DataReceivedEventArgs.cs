using Memory.Common;
using System;
using System.Collections.Generic;

namespace DataProvider.Interfaces
{
    /// <summary>
    /// Proporciona información del cambio de valor en una señal
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Valor recogido de los tags (señales)
        /// </summary>
        public TagValue Value { get; set; }
    }
}
