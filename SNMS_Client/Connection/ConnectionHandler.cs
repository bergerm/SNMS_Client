using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.IO;

namespace SNMS_Client.Connection
{
    abstract class ConnectionHandler
    {
        private const int CONNECTION_TIMEOUT = 300000;

        public static ProtocolMessage GetMessage(Stream stream)
        {
            try
            {
                stream.ReadTimeout = CONNECTION_TIMEOUT;
                int bufferSize = 2048;
                byte[] resp = new byte[bufferSize];
                int bytesread = stream.Read(resp, 0, 4);
                int messageLength = BitConverter.ToInt32(resp, 0);
                int totalRead = 0;

                var memStream = new MemoryStream();
                bytesread = 0;
                while (totalRead < messageLength)
                {
                    int bytesToRead = Math.Min(resp.Length, messageLength - totalRead);
                    bytesread = stream.Read(resp, 0, bytesToRead);
                    totalRead += bytesread;
                    memStream.Write(resp, 0, bytesread);
                }

                ProtocolMessage message = Protocol.ParseMessage(memStream.ToArray());
                return message;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void SendMessage(Stream stream, ProtocolMessage message)
        {
            byte[] response = Protocol.CraftMessage(message);
            byte[] responseSize = BitConverter.GetBytes(response.Length);
            // Send message size
            stream.Write(responseSize, 0, 4);
            // Send message
            stream.Write(response, 0, response.Length);
            stream.Flush();
        }
    }
}
