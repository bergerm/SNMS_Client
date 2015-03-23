using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class Account
    {
        int m_dwID;
        Plugin m_plugin;
        string m_sName;
        string m_sDescription;
        string m_sUserName;
        string m_sPassword;

        public void SetID(int id) { m_dwID = id; }
        public int GetID() { return m_dwID; }

        public void SetPlugin(Plugin plugin) { m_plugin = plugin; }
        public Plugin GetPlugin() { return m_plugin; }

        public void SetName(string sName) { m_sName = sName; }
        public string GetName() { return m_sName; }

        public void SetDescription(string description) { m_sDescription = description; }
        public string GetDescription() { return m_sDescription; }

        public void SetUserName(string userName) { m_sUserName = userName; }
        public string GetUserName() { return m_sUserName; }

        public void SetPassword(string password) { m_sPassword = password; }
        public string GetPassword() { return m_sPassword; }

        public static List<Account> ParseMessage(ProtocolMessage message, Plugin plugin)
        {
            List<Account> accountList = new List<Account>();

            int numOfAccounts = message.GetParameterAsInt(0);

            int dwCurrentAccount = 0;

            for (dwCurrentAccount = 0; dwCurrentAccount < numOfAccounts; dwCurrentAccount++)
            {
                Account account = new Account();
                
                account.SetPlugin(plugin);

                int dwAccountParameterOffset = 1 + dwCurrentAccount * 5;

                account.SetID(message.GetParameterAsInt(dwAccountParameterOffset));

                account.SetName(message.GetParameterAsString(dwAccountParameterOffset + 1));

                account.SetDescription(message.GetParameterAsString(dwAccountParameterOffset + 2));

                account.SetUserName(message.GetParameterAsString(dwAccountParameterOffset + 3));

                account.SetPassword(message.GetParameterAsString(dwAccountParameterOffset + 4));

                accountList.Add(account);
            }

            return accountList;
        }
    }
}
