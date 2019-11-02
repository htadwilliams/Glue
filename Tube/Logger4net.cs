using log4net;
using NerfDX.Logging;
using System;

namespace Glue
{
    /// <summary>
    /// NerfDX ILogger wrapper plug-in for log4net.
    /// </summary>
    class Logger4net : ILogger
    {
        public string Name => LoggerInternal.GetType().ToString();

        public ILog LoggerInternal { get; }

        public Logger4net(string name)
        {
            LoggerInternal = log4net.LogManager.GetLogger(name);
        }

        public void Debug(string message)
        {
            LoggerInternal.Debug(message);
        }

        public void Error(string message)
        {
            LoggerInternal.Error(message);
        }

        public void Error(string message, Exception e)
        {
            LoggerInternal.Error(message, e);
        }

        public void Info(string message)
        {
            LoggerInternal.Info(message);
        }

        public void Warning(string message)
        {
            LoggerInternal.Warn(message);
        }
    }
}
