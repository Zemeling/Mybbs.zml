using My.Common.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Util
{
    public static class LogUtil
    {
        public delegate void LogEvent(object sender, LogEventArgs e);

        public class LogEventArgs : EventArgs
        {
            public string Message
            {
                get;
                set;
            }

            public LogLevel Level
            {
                get;
                set;
            }

            public long TimeInMillis
            {
                get;
                set;
            }
        }

        private static ILogger loggerStatic;

        public static ILogger Logger
        {
            get
            {
                return LogUtil.loggerStatic;
            }
            set
            {
                if (LogUtil.loggerStatic != value)
                {
                    if (LogUtil.loggerStatic != null)
                    {
                        throw new Exception("Logger must only be set once.");
                    }
                    LogUtil.loggerStatic = value;
                }
            }
        }

        public static event LogEvent LogEventHandlers;

        public static void RaiseLogEvent(string msg, LogLevel level, long timeInMillis)
        {
            if (LogUtil.LogEventHandlers != null)
            {
                LogUtil.LogEventHandlers(null, new LogEventArgs
                {
                    Message = msg,
                    Level = level,
                    TimeInMillis = timeInMillis
                });
            }
        }

        public static void LogDebug(string msg, string detail = null, long timeInMillis = 0L, long expectedMaxTimeInMillis = 0L)
        {
            if (LogUtil.Logger != null)
            {
                if (expectedMaxTimeInMillis > 0 && timeInMillis > 0 && expectedMaxTimeInMillis < timeInMillis)
                {
                    LogUtil.Logger.Log(msg, detail, LogLevel.WARNING, timeInMillis);
                }
                else
                {
                    LogUtil.Logger.Log(msg, detail, LogLevel.DEBUG, timeInMillis);
                    LogUtil.RaiseLogEvent(msg, LogLevel.DEBUG, timeInMillis);
                }
            }
        }

        public static void LogInfo(string msg, string detail = null, long timeInMillis = 0L, long expectedMaxTimeInMillis = 0L)
        {
            if (LogUtil.Logger != null)
            {
                if (expectedMaxTimeInMillis > 0 && timeInMillis > 0 && expectedMaxTimeInMillis < timeInMillis)
                {
                    LogUtil.Logger.Log(msg, detail, LogLevel.WARNING, timeInMillis);
                }
                else
                {
                    LogUtil.Logger.Log(msg, detail, LogLevel.INFO, timeInMillis);
                    LogUtil.RaiseLogEvent(msg, LogLevel.INFO, timeInMillis);
                }
            }
        }

        public static void LogException(Exception e)
        {
            if (LogUtil.Logger != null && e != null)
            {
                if (null != e.InnerException)
                {
                    LogUtil.Logger.Log(e.InnerException.Message, e.InnerException.StackTrace, LogLevel.ERROR, 0L);
                }
                else
                {
                    LogUtil.Logger.Log(e.Message, e.StackTrace, LogLevel.ERROR, 0L);
                }
                LogUtil.RaiseLogEvent(e.Message, LogLevel.INFO, 0L);
            }
        }

        public static void LogException(string msg, Exception e)
        {
            LogUtil.Logger.Log(msg, string.Empty, LogLevel.ERROR, 0L);
            if (LogUtil.Logger != null && e != null)
            {
                if (null != e.InnerException)
                {
                    LogUtil.Logger.Log(e.InnerException.Message, e.InnerException.StackTrace, LogLevel.ERROR, 0L);
                }
                else
                {
                    LogUtil.Logger.Log(e.Message, e.StackTrace, LogLevel.ERROR, 0L);
                }
                LogUtil.RaiseLogEvent(e.Message, LogLevel.INFO, 0L);
            }
        }

        public static void LogError(string msg, string detail = null, long timeInMillis = 0L)
        {
            if (LogUtil.Logger != null)
            {
                LogUtil.Logger.Log(msg, detail, LogLevel.ERROR, timeInMillis);
                LogUtil.RaiseLogEvent(msg, LogLevel.ERROR, timeInMillis);
            }
        }

        public static void LogFatal(string msg, string detail = null, long timeInMillis = 0L)
        {
            if (LogUtil.Logger != null)
            {
                LogUtil.Logger.Log(msg, detail, LogLevel.FATAL, timeInMillis);
                LogUtil.RaiseLogEvent(msg, LogLevel.FATAL, timeInMillis);
            }
        }

        public static void LogWarning(string msg, string detail = null, long timeInMillis = 0L)
        {
            if (LogUtil.Logger != null)
            {
                LogUtil.Logger.Log(msg, detail, LogLevel.WARNING, timeInMillis);
                LogUtil.RaiseLogEvent(msg, LogLevel.WARNING, timeInMillis);
            }
        }

        public static void LogExclamation(string msg, string detail = null, long timeInMillis = 0L)
        {
            if (LogUtil.Logger != null)
            {
                LogUtil.Logger.Log(msg, detail, LogLevel.EXCLAMATION, timeInMillis);
                LogUtil.RaiseLogEvent(msg, LogLevel.EXCLAMATION, timeInMillis);
            }
        }

        public static void LogWarningIfExceedExpectedTime(string msg, string detail, long timeInMillis, long expectedMaxTimeInMillis)
        {
            if (LogUtil.Logger != null && expectedMaxTimeInMillis > 0 && timeInMillis > 0 && expectedMaxTimeInMillis < timeInMillis)
            {
                LogUtil.Logger.Log(msg, detail, LogLevel.WARNING, timeInMillis);
            }
        }
    }
}
