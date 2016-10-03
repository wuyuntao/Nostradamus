using System.Text;
using UnityEngine;
using UDebug = UnityEngine.Debug;

namespace NLog
{
    public class LogManager
    {
        public static Logger GetCurrentClassLogger()
        {
            return new Logger();
        }
    }

    public class Logger
    {
        public void Trace(string message, params object[] arguments)
        {
            message = BuildLogMessage( "TRACE", message, arguments );

            UDebug.Log( message );
        }

        public void Debug(string message, params object[] arguments)
        {
            message = BuildLogMessage( "DEBUG", message, arguments );

            UDebug.Log( message );
        }

        public void Info(string message, params object[] arguments)
        {
            message = BuildLogMessage( "INFO", message, arguments );

            UDebug.Log( message );
        }

        public void Warn(string message, params object[] arguments)
        {
            message = BuildLogMessage( "WARN", message, arguments );

            UDebug.LogWarning( message );
        }

        public void Error(string message, params object[] arguments)
        {
            message = BuildLogMessage( "ERROR", message, arguments );

            UDebug.LogWarning( message );
        }

        public void Fatal(string message, params object[] arguments)
        {
            message = BuildLogMessage( "FATAL", message, arguments );

            UDebug.LogWarning( message );
        }

        string BuildLogMessage(string level, string message, params object[] arguments)
        {
            var builder = new StringBuilder();
            builder.AppendFormat( "{0:f3}s|{1}|", Time.time, level );
            builder.AppendFormat( message, arguments );

            return builder.ToString();
        }
    }
}