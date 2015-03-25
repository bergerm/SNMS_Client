using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class TriggerType
    {
        int m_dwID;
        Configuration m_configuration;
        string m_sName;
        string m_sDescription;

        public void SetID(int id) { m_dwID = id; }
        public int GetID() { return m_dwID; }

        public void SetConfiguration(Configuration configuration) { m_configuration = configuration; }
        public Configuration GetConfiguration() { return m_configuration; }

        public void SetName(string sName) { m_sName = sName; }
        public string GetName() { return m_sName; }

        public void SetDescription(string sDescription) { m_sDescription = sDescription; }
        public string GetDescription() { return m_sDescription; }

        public static List<TriggerType> ParseMessage(ProtocolMessage message, Configuration configuration)
        {
            List<TriggerType> typesList = new List<TriggerType>();

            int numOfTypes = message.GetParameterAsInt(0);

            int dwCurrentType = 0;

            for (dwCurrentType = 0; dwCurrentType < numOfTypes; dwCurrentType++)
            {
                TriggerType type = new TriggerType();

                type.SetConfiguration(configuration);

                int dwAccountParameterOffset = 1 + dwCurrentType * 3;

                type.SetID(message.GetParameterAsInt(dwAccountParameterOffset));

                type.SetName(message.GetParameterAsString(dwAccountParameterOffset + 1));

                type.SetDescription(message.GetParameterAsString(dwAccountParameterOffset + 2));

                typesList.Add(type);
            }

            return typesList;
        }
    }
}
