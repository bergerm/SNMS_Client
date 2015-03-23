using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class Configuration
    {
        int m_dwID;
        Account m_account;
        string m_sName;
        string m_sDescription;
        bool m_bIsEnabled;

        public void SetID(int id) { m_dwID = id; }
        public int GetID() { return m_dwID; }

        public void SetAccount(Account account) { m_account = account; }
        public Account GetAccount() { return m_account; }

        public void SetName(string sName) { m_sName = sName; }
        public string GetName() { return m_sName; }

        public void SetDescription(string description) { m_sDescription = description; }
        public string GetDescription() { return m_sDescription; }

        public void SetEnabled(bool isEnabled) { m_bIsEnabled = isEnabled; }
        public bool GetEnabled() { return m_bIsEnabled; }

        public static List<Configuration> ParseMessage(ProtocolMessage message, Account account)
        {
            List<Configuration> configurationList = new List<Configuration>();

            int numOfConfigurations = message.GetParameterAsInt(0);

            int dwCurrentConfiguration = 0;

            for (dwCurrentConfiguration = 0; dwCurrentConfiguration < numOfConfigurations; dwCurrentConfiguration++)
            {
                Configuration configuration = new Configuration();

                configuration.SetAccount(account);

                int dwAccountParameterOffset = 1 + dwCurrentConfiguration * 4;

                configuration.SetID(message.GetParameterAsInt(dwAccountParameterOffset));

                configuration.SetName(message.GetParameterAsString(dwAccountParameterOffset + 1));

                configuration.SetDescription(message.GetParameterAsString(dwAccountParameterOffset + 2));

                configuration.SetEnabled(message.GetParameterAsBool(dwAccountParameterOffset + 3));

                configurationList.Add(configuration);
            }

            return configurationList;
        }

        public static List<Configuration> CreateWorkingSet(List<Configuration> completeList, Account account)
        {
            List<Configuration> workingSet = new List<Configuration>();

            foreach (Configuration configuration in completeList)
            {
                if (configuration.GetAccount().GetID() == account.GetID())
                {
                    workingSet.Add(configuration);
                }
            }

            return workingSet;
        }
    }
}
