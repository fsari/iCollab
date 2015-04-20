using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Logs the message in the verbose level whether the verbose level is enabled.
        /// </summary>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Verbose(string format,
                      params object[] args);

        /// <summary>
        /// Logs the message in the verbose level whether the verbose level is enabled.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Verbose(Exception ex,
                   string format,
                   params object[] args);

        /// <summary>
        /// Logs the message in the info level whether the information level is enabled.
        /// </summary>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Info(string format,
                   params object[] args);

        /// <summary>
        /// Logs the message in the debug level whether the debug level is enabled.
        /// </summary>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format..</param>
        void Debug(string format,
                    params object[] args);

        /// <summary>
        /// Logs the message in the warning level whether the warning level is enabled.
        /// </summary>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Warn(string format,
                   params object[] args);

        /// <summary>
        /// Logs the message in the warning level whether the warning level is enabled.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Warn(Exception ex,
                   string format,
                   params object[] args);

        /// <summary>
        /// Logs the message in the error level whether the error level is enabled.
        /// </summary>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Error(string format,
                    params object[] args);

        /// <summary>
        /// Logs the message in the error level whether the error level is enabled.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        void Error(Exception ex);

        /// <summary>
        /// Logs the message in the error level whether the error level is enabled.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Error(Exception ex,
                    string format,
                    params object[] args);

        /// <summary>
        /// Logs the message in the error level whether the fatal level is enabled.
        /// </summary>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Fatal(string format,
                    params object[] args);

        /// <summary>
        /// Logs the message in the error level whether the fatal level is enabled.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        void Fatal(Exception ex);

        /// <summary>
        /// Logs the message in the error level whether the fatal level is enabled.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        /// <param name="format">The <see cref="System.String"/> to format the log.</param>
        /// <param name="args">The <see cref="System.Array"/> that contains the <see cref="System.Object"/> to format.</param>
        void Fatal(Exception ex,
                    string format,
                    params object[] args);


        /// <summary>
        /// Gets a value indicating whether the verbose level is enabled.
        /// </summary>
        bool IsVerboseEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the debugging level is enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the information level is enabled.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the warning level is enabled.
        /// </summary>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the error level is enabled.
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the fatal level is enabled.
        /// </summary>
        bool IsFatalEnabled { get; }

        /// <summary>
        /// Gets the current message number
        /// </summary>
        long ReferenceNumber { get; }
    }
}
