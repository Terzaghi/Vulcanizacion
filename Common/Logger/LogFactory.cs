using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerManager
{
    public class LogFactory
    {
        #region Instanciadores
        public static ILogger GetLogger(string name)
        {
            return (ILogger)(new ELLogger(name));
        }

        public static ILogger GetLogger(Type type)
        {
            return (ILogger)(new ELLogger(type));
        }
        #endregion
    }
}
