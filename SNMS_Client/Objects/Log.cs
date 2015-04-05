using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class Log
    {
        public string ID { get; set; }
        public string Time { get; set; }
        public string Component { get; set; }
        public string UserName { get; set; }
        public string LogType { get; set; }
        public string LogMessage { get; set; }
        public string LogLink { get; set; }

        public static List<Log> ParseMessage(ProtocolMessage message)
        {
            List<Log> logsList = new List<Log>();

            int numOfLogs = message.GetParameterAsInt(0);

            int dwCurrentLog = 0;

            for (dwCurrentLog = 0; dwCurrentLog < numOfLogs; dwCurrentLog++)
            {
                Log log = new Log();

                int dwLogParameterOffset = 1 + dwCurrentLog * 7;

                log.ID = message.GetParameterAsString(dwLogParameterOffset);

                log.Time = message.GetParameterAsString(dwLogParameterOffset + 1);

                log.Component = message.GetParameterAsString(dwLogParameterOffset + 2);

                log.UserName = message.GetParameterAsString(dwLogParameterOffset + 3);

                log.LogType = message.GetParameterAsString(dwLogParameterOffset + 4);

                log.LogMessage = message.GetParameterAsString(dwLogParameterOffset + 5);

                log.LogLink = message.GetParameterAsString(dwLogParameterOffset + 6);

                logsList.Add(log);
            }

            return logsList;
        }
    }
}
