using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class UserType
    {
        int m_dwID;
        string m_sName;

        public void SetID(int id) { m_dwID = id; }
        public int GetID() { return m_dwID; }

        public void SetName(string sName) { m_sName = sName; }
        public string GetName() { return m_sName; }

        public static List<UserType> ParseMessage(ProtocolMessage message)
        {
            List<UserType> typesList = new List<UserType>();

            int numOfTypes = message.GetParameterAsInt(0);

            int dwCurrentType = 0;

            for (dwCurrentType = 0; dwCurrentType < numOfTypes; dwCurrentType++)
            {
                UserType type = new UserType();

                int dwAccountParameterOffset = 1 + dwCurrentType * 2;

                type.SetID(message.GetParameterAsInt(dwAccountParameterOffset));

                type.SetName(message.GetParameterAsString(dwAccountParameterOffset + 1));

                typesList.Add(type);
            }

            return typesList;
        }
    }
}
