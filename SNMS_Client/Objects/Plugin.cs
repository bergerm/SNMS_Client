using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class Plugin
    {
        int m_dwPluginID;
        string m_sName;
        string m_sDescription;
        bool m_bEnabled;

        public Plugin()
        {
            m_dwPluginID = 0;
            m_sName = "";
            m_sDescription = "";
            m_bEnabled = false;
        }

        public void SetID(int dwID) { m_dwPluginID = dwID; }
        public int GetID() { return m_dwPluginID; }

        public void SetName(string sName) {m_sName = sName;}
        public string GetName() { return m_sName; }

        public void SetDescription(string sDescription) { m_sDescription = sDescription; }
        public string GetDescription() { return m_sDescription; }

        public void SetEnabled(bool bEnabled) { m_bEnabled = bEnabled; }
        public bool GetEnabled() { return m_bEnabled; }

        public static List<Plugin> ParseMessage(ProtocolMessage message)
        {
            if (message == null)
            {
                return null;
            }

            List<Plugin> pluginList = new List<Plugin>();

            byte[] arr = null;
            message.GetParameter(ref arr, 0);
            int numOfPlugins = BitConverter.ToInt32(arr, 0);

            int dwCurrentPlugin = 0;

            for (dwCurrentPlugin = 0; dwCurrentPlugin < numOfPlugins; dwCurrentPlugin++)
            {
                Plugin plugin = new Plugin();
                int dwPluginParameterOffset = 1 + dwCurrentPlugin * 4;

                plugin.SetID(message.GetParameterAsInt(dwPluginParameterOffset));

                plugin.SetName(message.GetParameterAsString(dwPluginParameterOffset + 1));

                plugin.SetDescription(message.GetParameterAsString(dwPluginParameterOffset + 2));

                plugin.SetEnabled(message.GetParameterAsBool(dwPluginParameterOffset + 3));

                pluginList.Add(plugin);
            }

            return pluginList;
        }
    }
}
