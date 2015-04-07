using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Core.Infrastructure;

namespace Core.Extensions
{
    public static class ConversionExtensions
    { 

        #region int

        public static char ToHex(this int value)
        {
            if (value <= 9)
            {
                return (char)(value + 48);
            }
            return (char)((value - 10) + 97);
        }

        /// <summary>
        /// Returns kilobytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToKb(this int value)
        {
            return value * 1024;
        }

        /// <summary>
        /// Returns megabytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToMb(this int value)
        {
            return value * 1024 * 1024;
        }

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of minutes.</summary>
        /// <param name="minutes">number of minutes</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>3.Minutes()</example>
        public static TimeSpan ToMinutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of seconds.
        /// </summary>
        /// <param name="seconds">number of seconds</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>2.Seconds()</example>
        public static TimeSpan ToSeconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">milliseconds for this timespan</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToMilliseconds(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of days.
        /// </summary>
        /// <param name="days">Number of days.</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToDays(this int days)
        {
            return TimeSpan.FromDays(days);
        }


        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of hours.
        /// </summary>
        /// <param name="hours">Number of hours.</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToHours(this int hours)
        {
            return TimeSpan.FromHours(hours);
        }

        #endregion

        #region double

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of minutes.</summary>
        /// <param name="minutes">number of minutes</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>3D.Minutes()</example>
        public static TimeSpan ToMinutes(this double minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }


        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of hours.</summary>
        /// <param name="hours">number of hours</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>3D.Hours()</example>
        public static TimeSpan ToHours(this double hours)
        {
            return TimeSpan.FromHours(hours);
        }

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of seconds.</summary>
        /// <param name="seconds">number of seconds</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        /// <example>2D.Seconds()</example>
        public static TimeSpan ToSeconds(this double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>Returns a <see cref="TimeSpan"/> that represents a specified number of milliseconds.</summary>
        /// <param name="milliseconds">milliseconds for this timespan</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToMilliseconds(this double milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents a specified number of days.
        /// </summary>
        /// <param name="days">Number of days, accurate to the milliseconds.</param>
        /// <returns>A <see cref="TimeSpan"/> that represents a value.</returns>
        public static TimeSpan ToDays(this double days)
        {
            return TimeSpan.FromDays(days);
        }

        #endregion

        #region String

        public static T ToEnum<T>(this string value, T defaultValue) where T : IComparable, IFormattable
        {
            T convertedValue = defaultValue;

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    convertedValue = (T)Enum.Parse(typeof(T), value.Trim(), true);
                }
                catch (ArgumentException)
                {
                }
            }

            return convertedValue;
        }

        public static string[] ToArray(this string value)
        {
            return value.ToArray(new char[] { ',' });
        }

        public static string[] ToArray(this string value, params char[] separator)
        {
            return value.Trim().Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int ToInt(this string value, int defaultValue = 0)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static float ToFloat(this string value, float defaultValue = 0)
        {
            float result;
            if (float.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static bool ToBool(this string value, bool defaultValue = false)
        {
            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static DateTime? ToDateTime(this string value, DateTime? defaultValue)
        {
            return value.ToDateTime(null, defaultValue);
        }

        public static DateTime? ToDateTime(this string value, string[] formats, DateTime? defaultValue)
        {
            return value.ToDateTime(formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces, defaultValue);
        }

        public static DateTime? ToDateTime(this string value, string[] formats, IFormatProvider provider, DateTimeStyles styles, DateTime? defaultValue)
        {
            DateTime result;

            if (formats.IsNullOrEmpty())
            {
                if (DateTime.TryParse(value, provider, styles, out result))
                {
                    return result;
                }
            }

            if (DateTime.TryParseExact(value, formats, provider, styles, out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static Guid ToGuidAlt(this string value)
        {
            if ((!String.IsNullOrEmpty(value)) && (value.Trim().Length == 22))
            {
                string encoded = string.Concat(value.Trim().Replace("-", "+").Replace("_", "/"), "==");

                byte[] base64 = System.Convert.FromBase64String(encoded);

                return new Guid(base64);
            }

            return Guid.Empty;
        }

        public static byte[] ToByteArray(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }

        [DebuggerStepThrough]
        public static Version ToVersion(this string value, Version defaultVersion = null)
        {
            try
            {
                return new Version(value);
            }
            catch
            {
                return defaultVersion ?? new Version("1.0");
            }
        }

        #endregion
         
        #region Stream

        public static byte[] ToByteArray(this Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");

            byte[] buffer;

            if (stream is MemoryStream && stream.CanRead && stream.CanSeek)
            {
                int len = System.Convert.ToInt32(stream.Length);
                buffer = new byte[len];
                stream.Read(buffer, 0, len);
                return buffer;
            }

            MemoryStream memStream = null;
            try
            {
                buffer = new byte[1024];
                memStream = new MemoryStream();
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                if (memStream != null)
                    memStream.Close();
            }

            if (memStream != null)
            {
                return memStream.ToArray();
            }

            return null;
        }

        public static string AsString(this Stream stream)
        {
            // convert memory stream to string
            string result;
            stream.Position = 0;

            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }

            return result;

        }

        #endregion

        #region ByteArray

        /// <summary>
        /// Converts a byte array into an object.
        /// </summary>
        /// <param name="bytes">Object to deserialize. May be null.</param>
        /// <returns>Deserialized object, or null if input was null.</returns>
        public static object ToObject(this byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return new BinaryFormatter().Deserialize(stream);
            }
        }
          
        public static Stream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        public static string AsString(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }


        /// <summary>
        /// Computes the MD5 hash of a byte array
        /// </summary>
        /// <param name="value">The byte array to compute the hash for</param>
        /// <returns>The hash value</returns>
        //[DebuggerStepThrough]
        public static string Hash(this byte[] value, bool toBase64 = false)
        {
            Guard.ArgumentNotNull(value, "value");

            using (MD5 md5 = MD5.Create())
            {

                if (toBase64)
                {
                    byte[] hash = md5.ComputeHash(value);
                    return System.Convert.ToBase64String(hash);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] hashBytes = md5.ComputeHash(value);
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2").ToLower());
                    }

                    return sb.ToString();
                }
            }
        }

        #endregion

 
 

    }

}
