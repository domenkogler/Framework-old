using System;
using Caliburn.Micro;

namespace Kogler.Framework
{
    public class Logger : NLog.Logger, ILog
    {
        public void Error(Exception exception)
        {
            Error(exception, exception.GetErrorMessage());
        }
    }
}