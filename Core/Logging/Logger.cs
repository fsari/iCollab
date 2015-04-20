using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace Core.Logging
{
    public class Logger : ILogger
    { 
        private readonly Serilog.ILogger _logger;

        public Logger(Serilog.ILogger logger)
        {
            _logger = logger;  
        }

        private void Log(LogEventLevel logEventLevel, Exception exception, string format, params object[] args)
        {
            if (_logger.IsEnabled(logEventLevel) == false)
            {
                return;
            }

            _logger.Write(logEventLevel, exception, format, args);
        }
         
        public void Verbose(string format, params object[] args)
        {
            Log(LogEventLevel.Verbose, null, format, args);
        }

        public void Verbose(Exception ex, string format, params object[] args)
        {
            Log(LogEventLevel.Verbose, ex, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Log(LogEventLevel.Information, null, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            Log(LogEventLevel.Debug, null, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Log(LogEventLevel.Warning, null, format, args);
        }

        public void Warn(Exception ex, string format, params object[] args)
        {
            Log(LogEventLevel.Warning, ex, format, args);
        }

        public void Error(string format, params object[] args)
        {
            Log(LogEventLevel.Error, null, format, args);
        }

        public void Error(Exception ex)
        {
            Log(LogEventLevel.Error, ex, null, null);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Log(LogEventLevel.Error, ex, format, args);
        }

        public void Fatal(string format, params object[] args)
        {
            Log(LogEventLevel.Fatal, null, format, args);
        }

        public void Fatal(Exception ex)
        {
            Log(LogEventLevel.Fatal, ex, null, null);
        }

        public void Fatal(Exception ex, string format, params object[] args)
        {
            Log(LogEventLevel.Fatal, ex, format, args);
        }

        public bool IsVerboseEnabled {
            get { return _logger.IsEnabled(LogEventLevel.Information); } 
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Debug); } 
        }

        public bool IsInfoEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Information); }
        }

        public bool IsWarnEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Warning); } 
        }

        public bool IsErrorEnabled
        {
            get { return _logger.IsEnabled(LogEventLevel.Error); } 
        }

        public bool IsFatalEnabled
        { 
            get { return _logger.IsEnabled(LogEventLevel.Fatal); } 
        }
        public long ReferenceNumber { get; private set; }
    }
}
