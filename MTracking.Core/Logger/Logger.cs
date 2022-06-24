using log4net;
using log4net.Config;
using System;
using System.Reflection;

namespace MTracking.Core.Logger
{
    public static class Logger
    {
        private static readonly ILog _infoLogger;
        private static readonly ILog _fatalLogger;
        private static readonly ILog _errorLogger;

        static Logger()
        {
            var repository = LogManager.GetRepository(Assembly.GetExecutingAssembly());

            _infoLogger = LogManager.GetLogger(repository.Name, "LOGGER_INFO");
            _fatalLogger = LogManager.GetLogger(repository.Name, "LOGGER_FATAL");
            _errorLogger = LogManager.GetLogger(repository.Name, "LOGGER_ERROR");

            XmlConfigurator.Configure(repository);
        }

        public static void Info(string message)
        {
            _infoLogger.Info(message);
        }

        public static void Fatal(string message)
        {
            _fatalLogger.Fatal(message);
        }

        public static void Fatal(string message, Exception exception)
        {
            _fatalLogger.Fatal(message, exception);
        }

        public static void Error(string message)
        {
            _errorLogger.Error(message);
        }

        public static void Error(string message, Exception exception)
        {
            _errorLogger.Error(message, exception);
        }
    }
}
