using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Net.Sockets;

using SNMS_Client.Connection;

namespace SNMS_Client
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        NetworkStream m_stream;
        MainWindow m_mainWindow;

        public LoginScreen(MainWindow mainWindow, NetworkStream stream)
        {
            InitializeComponent();
            m_stream = stream;
            m_mainWindow = mainWindow;
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserName.Text == "" || Password.Password == "")
            {
                return;
            }

            ProtocolMessage loginMessage = new ProtocolMessage();
            loginMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_LOGIN_REQUEST);
            loginMessage.AddParameter(UserName.Text);
            loginMessage.AddParameter(Password.Password);
            ConnectionHandler.SendMessage(m_stream, loginMessage);

            ProtocolMessage responseMessage = ConnectionHandler.GetMessage(m_stream);

            string sResponse = responseMessage.GetParameterAsString(0);
            if (sResponse == ProtocolMessage.PROTOCOL_CONSTANT_FAILURE_MESSAGE)
            {
                MessageBox.Show("Username and Password do not match.\nPlease Try again.");
                return;
            }

            int dwUserType = responseMessage.GetParameterAsInt(1);
            switch (dwUserType)
            {
                case 1:
                    m_mainWindow.SetUserType("operator");
                    break;

                case 2:
                    m_mainWindow.SetUserType("admin");
                    break;

                default:
                    m_mainWindow.SetUserType("unknown");
                    break;
            }

            m_mainWindow.LoggedIn();

            this.Close();
        }
    }
}
