﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class User
    {
        int m_dwID;
        string m_sName;
        string m_sPassword;
        UserType m_userType;
        bool m_bEnableRead;
        bool m_bEnableWrite;

        public void SetID(int id) { m_dwID = id; }
        public int GetID() { return m_dwID; }

        public void SetName(string sName) { m_sName = sName; }
        public string GetName() { return m_sName; }

        public void SetPassword(string sPassword) { m_sPassword = sPassword; }
        public string GetPassword() { return m_sPassword; }

        public void SetUserType(UserType type) { m_userType = type; }
        public UserType GetUserType() { return m_userType; }

        public void SetEnableRead(bool bEnabled) { m_bEnableRead = bEnabled; }
        public bool GetEnableRead() { return m_bEnableRead; }

        public void SetEnableWrite(bool bEnabled) { m_bEnableWrite = bEnabled; }
        public bool GetEnableWrite() { return m_bEnableWrite; }

        public static List<User> ParseMessage(ProtocolMessage message, List<UserType> userTypesList)
        {
            List<User> usersList = new List<User>();

            int numOfUsers = message.GetParameterAsInt(0);

            int dwCurrentUser = 0;

            for (dwCurrentUser = 0; dwCurrentUser < numOfUsers; dwCurrentUser++)
            {
                User user = new User();

                int dwUserParameterOffset = 1 + dwCurrentUser * 5;

                user.SetID(message.GetParameterAsInt(dwUserParameterOffset));

                user.SetName(message.GetParameterAsString(dwUserParameterOffset + 1));

                int dwUserTypeId = message.GetParameterAsInt(dwUserParameterOffset + 2);

                user.SetEnableRead(message.GetParameterAsBool(dwUserParameterOffset + 3));

                user.SetEnableWrite(message.GetParameterAsBool(dwUserParameterOffset + 4));

                user.SetPassword("");

                UserType userType = null;
                foreach (UserType type in userTypesList)
                {
                    if (type.GetID() == dwUserTypeId)
                    {
                        userType = type;
                    }
                }
                user.SetUserType(userType);

                usersList.Add(user);
            }

            return usersList;
        }
    }
}
