using System;
using System.Collections.Generic;

namespace DataProvider.DTO
{
    /// <summary>
    /// Proporciona información del cambio de valor en una señal
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Hora de recepción de los datos
        /// </summary>
        public DateTime Moment { get; set; }

        /// <summary>
        /// Valores recogidos de los tags (señales)
        /// </summary>
        public List<TagData> Values { get; set; }        
    }

}
