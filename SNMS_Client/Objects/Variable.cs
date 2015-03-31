using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNMS_Client.Connection;

namespace SNMS_Client.Objects
{
    class Variable
    {
        int m_dwID;
        Configuration m_configuration;
        string m_sName;
        string m_sType;
        string m_sValue;

        public int GetID() { return m_dwID; }
        public void SetID(int id) { m_dwID = id; }

        public Configuration GetConfiguration() { return m_configuration; }
        public void SetConfiguration(Configuration configuration) { m_configuration = configuration; }

        public string GetName() { return m_sName; }
        public void SetName(string sName) { m_sName = sName; }

        public string GetVariableType() { return m_sType; }
        public void SetVariableType(string sType) { m_sType = sType; }

        public string GetVariableValue() { return m_sValue; }
        public void SetVariableValue(string sValue) { m_sValue = sValue; }

        public static List<Variable> ParseMessage(ProtocolMessage message, Configuration configuration)
        {
            List<Variable> variableList = new List<Variable>();

            int numOfVariables = message.GetParameterAsInt(0);

            int dwCurrentVariable = 0;

            for (dwCurrentVariable = 0; dwCurrentVariable < numOfVariables; dwCurrentVariable++)
            {
                Variable variable = new Variable();

                variable.SetConfiguration(configuration);

                int dwVariableParameterOffset = 1 + dwCurrentVariable * 4;

                variable.SetID(message.GetParameterAsInt(dwVariableParameterOffset));

                variable.SetName(message.GetParameterAsString(dwVariableParameterOffset + 1));

                variable.SetVariableType(message.GetParameterAsString(dwVariableParameterOffset + 2));

                variable.SetVariableValue(message.GetParameterAsString(dwVariableParameterOffset + 3));

                variableList.Add(variable);
            }

            return variableList;
        }
    }
}
