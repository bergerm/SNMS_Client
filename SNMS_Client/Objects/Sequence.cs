using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class Sequence
    {
        int m_dwId;
        Configuration m_configuration;
        string m_sName;
        bool m_bEnabled;

        public void SetID(int id) { m_dwId = id; }
        public int GetID() { return m_dwId; }

        public void SetName(string name) { m_sName = name; }
        public string GetName() { return m_sName; }

        public void SetEnabled(bool isEnabled) { m_bEnabled = isEnabled; }
        public bool GetEnabled() { return m_bEnabled; }

        public void SetConfiguration(Configuration configuration) { m_configuration = configuration; }
        public Configuration GetConfiguration() { return m_configuration; }

        public static List<Sequence> ParseMessage(ProtocolMessage message, Configuration configuration)
        {
            List<Sequence> sequenceList = new List<Sequence>();

            int numOfSequences = message.GetParameterAsInt(0);

            int dwCurrentSequence = 0;

            for (dwCurrentSequence = 0; dwCurrentSequence < numOfSequences; dwCurrentSequence++)
            {
                Sequence sequence = new Sequence();

                sequence.SetConfiguration(configuration);

                int dwAccountParameterOffset = 1 + dwCurrentSequence * 3;

                sequence.SetID(message.GetParameterAsInt(dwAccountParameterOffset));

                sequence.SetName(message.GetParameterAsString(dwAccountParameterOffset + 1));

                sequence.SetEnabled(message.GetParameterAsBool(dwAccountParameterOffset + 2));

                sequenceList.Add(sequence);
            }

            return sequenceList;
        }
    }
}
