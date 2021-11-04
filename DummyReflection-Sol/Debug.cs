using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyReflection
{
    /// <summary>
    /// A static class for sending messages from anywhere without bounds.
    /// </summary>
    public static class Debug
    {
        private static List<Stream> _debugStreams = new List<Stream>();

        /// <summary>
        /// Add a stream to the debugger
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool Add(Stream stream)
        {
            if (stream == null)
                return false;
            else if (_debugStreams.Contains(stream))
                return false;
            else if (!stream.CanWrite)
                return false;

            _debugStreams.Add(stream);

            return true;
        }

        /// <summary>
        /// Remove a stream from the Debug
        /// </summary>
        /// <param name="ss">Remove us</param>
        public static void Remove(params Stream[] ss)
        {
            RemoveIEnum(ss);
        }

        /// <summary>
        /// Remove a stream from the Debug
        /// </summary>
        /// <param name="ss">Remove us</param>
        public static void RemoveIEnum(IEnumerable<Stream> ss)
        {
            foreach (Stream s in ss)
            {
                if (!_debugStreams.Contains(s))
                    _debugStreams.Remove(s);
            }
        }

        /// <summary>
        /// Send a message to all streams in the Debugger
        /// </summary>
        /// <param name="msg">Converted automatically to string.</param>
        /// <param name="predicate">Select streams in the Where method</param>
        public static void Send(object msg, Func<Stream, bool>? predicate = null)
        {
            if (msg == null)
            {
                Send("Send failed! 'msg' to send was null");
                return;
            }

            IEnumerable<Stream> streams = _debugStreams;

            if (predicate != null)
                streams = streams.Where(predicate);

            List<Stream> remove = new();
            foreach (Stream stream in streams)
            {
                if (!stream.CanWrite)
                {
                    remove.Add(stream);
                    continue;
                }
                //msg is not NULL
                byte[] buffer = Encoding.UTF8.GetBytes($"{msg}\r\n");
                stream.Write(buffer, 0, buffer.Length);

                if (stream.CanSeek)
                    stream.Position = 0;
            }

            RemoveIEnum(remove);
        }

        /// <summary>
        /// Reads a streams content and converts to UTF8 encoding.
        /// </summary>
        /// <param name="stream">To read</param>
        /// <returns>String content under UTF8 encoding</returns>
        public static string Read(Stream stream)
        {
            if (stream == null)
                return string.Empty;

            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
