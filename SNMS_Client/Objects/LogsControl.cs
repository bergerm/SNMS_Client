using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class LogsControl
    {
        static public List<Log> GetLogs(NetworkStream stream)
        {
            ProtocolMessage getLogsMessage = new ProtocolMessage();
            getLogsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_LAST_100_LOGS);

            ConnectionHandler.SendMessage(stream, getLogsMessage);

            ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

            List<Log> logs = Log.ParseMessage(responseMessage);

            return logs;
        }
    }
}
