using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMS_Client.Connection
{
    enum ProtocolMessageType
    {
        PROTOCOL_MESSAGE_CONNECTION = 1,
        PROTOCOL_MESSAGE_CONNECTION_RESPONSE,
        
        PROTOCOL_MESSAGE_LOGIN_REQUEST,
        PROTOCOL_MESSAGE_LOGIN_ANSWER,

        PROTOCOL_MESSAGE_GET_PLUGINS,
        PROTOCOL_MESSAGE_PLUGINS_LIST,
        PROTOCOL_MESSAGE_NEW_PLUGIN,
        PROTOCOL_MESSAGE_UPDATE_PLUGIN,

        PROTOCOL_MESSAGE_GET_ACCOUNTS,
        PROTOCOL_MESSAGE_ACCOUNTS_LIST,
        PROTOCOL_MESSAGE_NEW_ACCOUNT,
        PROTOCOL_MESSAGE_UPDATE_ACCOUNT,

        PROTOCOL_MESSAGE_GET_CONFIGURATIONS,
        PROTOCOL_MESSAGE_CONFIGURATIONS_LIST,
        PROTOCOL_MESSAGE_NEW_CONFIGURATION,
        PROTOCOL_MESSAGE_UPDATE_CONFIGURATION,

        PROTOCOL_MESSAGE_GET_SEQUENCES,
        PROTOCOL_MESSAGE_SEQUENCES_LIST,
        PROTOCOL_MESSAGE_NEW_SEQUENCE,
        PROTOCOL_MESSAGE_UPDATE_SEQUENCE,

        PROTOCOL_MESSAGE_GET_TRIGGER_TYPES,
        PROTOCOL_MESSAGE_TRIGGER_TYPES_LIST,
        PROTOCOL_MESSAGE_NEW_TRIGGER_TYPE,
        PROTOCOL_MESSAGE_UPDATE_TRIGGER_TYPE,

        PROTOCOL_MESSAGE_GET_TRIGGERS,
        PROTOCOL_MESSAGE_TRIGGERS_LIST,
        PROTOCOL_MESSAGE_NEW_TRIGGER,
        PROTOCOL_MESSAGE_UPDATE_TRIGGER,

        PROTOCOL_MESSAGE_GET_USER_TYPES,
        PROTOCOL_MESSAGE_USER_TYPES_LIST,
        PROTOCOL_MESSAGE_NEW_USER_TYPE,
        PROTOCOL_MESSAGE_UPDATE_USER_TYPE,

        PROTOCOL_MESSAGE_GET_USERS,
        PROTOCOL_MESSAGE_USERS_LIST,
        PROTOCOL_MESSAGE_NEW_USER,
        PROTOCOL_MESSAGE_UPDATE_USER
    }

    class ProtocolMessage
    {
        public const string PROTOCOL_CONSTANT_SUCCESS_MESSAGE = "success";
        public const string PROTOCOL_CONSTANT_FAILURE_MESSAGE = "failure";

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        ProtocolMessageType m_messageType;
        List<byte[]> m_listOfArrays;
        List<int> m_listOfSizes;
        int m_messageSize;

        public ProtocolMessage()
        {
            m_messageType = new ProtocolMessageType();
            m_listOfArrays = new List<byte[]>();
            m_listOfSizes = new List<int>();
            m_messageSize = 8; // Size of messageType(4) + Size of number of Parameters(4)
        }

        public void SetMessageType(ProtocolMessageType type)
        {
            m_messageType = type;
        }

        public ProtocolMessageType GetMessageType()
        {
            return m_messageType;
        }

        public int GetParametersCount()
        {
            return m_listOfArrays.Count;
        }

        public int GetMessageSize()
        {
            return m_messageSize;
        }

        public bool AddParameter(byte[] parameter, int size)
        {
            if (parameter.Length < size)
            {
                return false;
            }

            m_listOfArrays.Add(parameter);
            m_listOfSizes.Add(size);

            // 4 bytes for parameter size
            m_messageSize += 4;
            m_messageSize += size;

            return true;
        }

        public bool AddParameter(string str)
        {
            byte[] arr = GetBytes(str);
            return AddParameter(arr, arr.Length);
        }

        public bool AddParameter(int num)
        {
            byte[] arr = BitConverter.GetBytes(num);
            return AddParameter(arr, 4);
        }

        public bool AddParameter(bool b)
        {
            int parameter;

            if (b)
            {
                parameter = 1;
            }
            else
            {
                parameter = 0;
            }

            byte[] arr = BitConverter.GetBytes(parameter);
            return AddParameter(arr, 4);
        }

        public int GetParameter(ref byte[] parameter, int index)
        {
            if (index < 0 || index >= m_listOfArrays.Count)
            {
                return -1;
            }

            parameter = m_listOfArrays[index];
            return m_listOfSizes[index];
        }

        public string GetParameterAsString(int index)
        {
            byte[] array = null;
            int size = GetParameter(ref array, index);

            if (array == null)
            {
                return "";
            }

            string str = GetString(array);
            return str;
        }

        public int GetParameterAsInt(int index)
        {
            byte[] array = null;
            int size = GetParameter(ref array, index);

            if (array == null)
            {
                return 0;
            }

            int num = BitConverter.ToInt32(array, 0) ;
            return num;
        }

        public bool GetParameterAsBool(int index)
        {
            byte[] array = null;
            int size = GetParameter(ref array, index);

            if (array == null)
            {
                return false;
            }

            int num = BitConverter.ToInt32(array, 0) ;
            return (num != 0) ? true : false;
        }
    }

    class Protocol
    {
        public static byte[] CraftMessage(ProtocolMessage message)
        {
            byte[] craftedMessage = new byte[message.GetMessageSize()];
            int byteCount = 0;

            byte[] tempArray = BitConverter.GetBytes((int)message.GetMessageType());
            Array.Copy(tempArray, 0, craftedMessage, byteCount, 4);
            byteCount += 4;

            int numOfParameters = message.GetParametersCount();
            tempArray = BitConverter.GetBytes(numOfParameters);
            Array.Copy(tempArray, 0, craftedMessage, byteCount, 4);
            byteCount += 4;

            for (int i = 0; i < numOfParameters; i++)
            {
                int tempSize = message.GetParameter(ref tempArray, i);
                byte[] sizeArray = BitConverter.GetBytes(tempSize);
                Array.Copy(sizeArray, 0, craftedMessage, byteCount, 4);
                byteCount += 4;
                Array.Copy(tempArray, 0, craftedMessage, byteCount, tempSize);
                byteCount += tempSize;
            }

            return craftedMessage;
        }

        public static ProtocolMessage ParseMessage(byte[] message)
        {
            ProtocolMessage parsedMessage = new ProtocolMessage();
            int byteCount = 0;

            parsedMessage.SetMessageType((ProtocolMessageType)BitConverter.ToInt32(message, byteCount));
            byteCount += 4;

            int numOfParameters = BitConverter.ToInt32(message, byteCount);
            byteCount += 4;

            for (int i = 0; i < numOfParameters; i++)
            {
                int parameterSize = BitConverter.ToInt32(message, byteCount);
                byteCount += 4;

                byte[] tempParameter = new byte[parameterSize];
                Array.Copy(message, byteCount, tempParameter, 0, parameterSize);
                parsedMessage.AddParameter(tempParameter, parameterSize);
                byteCount += parameterSize;
            }

            return parsedMessage;
        }
    }
}
