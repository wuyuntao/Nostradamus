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
        public bool IsTraceEnabled { get; private set; }

        public bool IsDebugEnabled { get; private set; }

        public bool IsInfoEnabled { get; private set; }

        public bool IsWarnEnabled { get; private set; }

        public bool IsErrorEnabled { get; private set; }

        public bool IsFatalEnabled { get; private set; }

        public Logger()
        {
            IsTraceEnabled = false;
            IsDebugEnabled = false;
            IsInfoEnabled = true;
            IsWarnEnabled = true;
            IsErrorEnabled = true;
            IsFatalEnabled = true;
        }

        public void Trace(string message, params object[] arguments)
        {
            if (!IsTraceEnabled)
                return;

            message = BuildLogMessage("TRACE", message, arguments);

            UDebug.Log(message);
        }

        public void Debug(string message, params object[] arguments)
        {
            if (!IsDebugEnabled)
                return;

            message = BuildLogMessage("DEBUG", message, arguments);

            UDebug.Log(message);
        }

        public void Info(string message, params object[] arguments)
        {
            if (!IsInfoEnabled)
                return;

            message = BuildLogMessage("INFO", message, arguments);

            UDebug.Log(message);
        }

        public void Warn(string message, params object[] arguments)
        {
            if (!IsWarnEnabled)
                return;

            message = BuildLogMessage("WARN", message, arguments);

            UDebug.LogWarning(message);
        }

        public void Error(string message, params object[] arguments)
        {
            if (!IsErrorEnabled)
                return;

            message = BuildLogMessage("ERROR", message, arguments);

            UDebug.LogWarning(message);
        }

        public void Fatal(string message, params object[] arguments)
        {
            if (!IsFatalEnabled)
                return;

            message = BuildLogMessage("FATAL", message, arguments);

            UDebug.LogWarning(message);
        }

        string BuildLogMessage(string level, string message, params object[] arguments)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0:f3}s|{1}|", Time.time, level);
            builder.AppendFormat(message, arguments);

            return builder.ToString();
        }
    }
}