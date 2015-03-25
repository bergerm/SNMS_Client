using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class Trigger
    {
        int m_dwID;
        Configuration m_configuration;
        TriggerType m_triggerType;
        string m_sName;
        string m_sDescription;
        string m_sValue;
        Sequence m_reaction;
        string m_sReactionValue;
        bool m_bEnabled;

        public void SetID(int id) { m_dwID = id; }
        public int GetID() { return m_dwID; }

        public void SetConfiguration(Configuration configuration) { m_configuration = configuration; }
        public Configuration GetConfiguration() { return m_configuration; }

        public void SetTriggerType(TriggerType type) { m_triggerType = type; }
        public TriggerType GetTriggerType() { return m_triggerType; }

        public void SetName(string name) { m_sName = name; }
        public string GetName() { return m_sName; }

        public void SetDescription(string desc) { m_sDescription = desc; }
        public string GetDescription() { return m_sDescription; }

        public void SetValue(string value) { m_sValue = value; }
        public string GetValue() { return m_sValue; }

        public void SetReaction(Sequence reaction) { m_reaction = reaction; }
        public Sequence GetReaction() { return m_reaction; }

        public void SetReactionValue(string value) { m_sReactionValue = value; }
        public string GetReactionValue() { return m_sReactionValue; }

        public void SetEnabled(bool isEnabled) { m_bEnabled = isEnabled; }
        public bool GetEnabled() { return m_bEnabled; }

        public static List<Trigger> ParseMessage(ProtocolMessage message, Configuration configuration, TriggerType triggerType, List<Sequence> sequencesList)
        {
            List<Trigger> triggerList = new List<Trigger>();

            int numOfTriggers = message.GetParameterAsInt(0);

            int dwCurrentTrigger = 0;

            for (dwCurrentTrigger = 0; dwCurrentTrigger < numOfTriggers; dwCurrentTrigger++)
            {
                Trigger trigger = new Trigger();

                trigger.SetConfiguration(configuration);
                trigger.SetTriggerType(triggerType);

                int dwTriggerParameterOffset = 1 + dwCurrentTrigger * 7;

                trigger.SetID(message.GetParameterAsInt(dwTriggerParameterOffset));

                trigger.SetName(message.GetParameterAsString(dwTriggerParameterOffset + 1));

                trigger.SetDescription(message.GetParameterAsString(dwTriggerParameterOffset + 2));

                trigger.SetValue(message.GetParameterAsString(dwTriggerParameterOffset + 3));

                int dwResponseID = message.GetParameterAsInt(dwTriggerParameterOffset + 4);
                Sequence reaction = null;
                foreach (Sequence seq in sequencesList)
                {
                    if (seq.GetID() == dwResponseID)
                    {
                        reaction = seq;
                    }
                }
                trigger.SetReaction(reaction);

                trigger.SetReactionValue(message.GetParameterAsString(dwTriggerParameterOffset + 5));

                trigger.SetEnabled(message.GetParameterAsBool(dwTriggerParameterOffset + 6));

                triggerList.Add(trigger);
            }

            return triggerList;
        }
    }
}
