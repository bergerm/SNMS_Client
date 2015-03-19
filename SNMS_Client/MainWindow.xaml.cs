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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Net.Sockets;

using SNMS_Client.Connection;

namespace SNMS_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int TCP_PORT = 56824;

        public MainWindow()
        {
            InitializeComponent();
            LoadStartValues();
            //Available_Plugins;
        }

        bool LoadStartValues()
        {
            try
            {
                ProtocolMessage pluginsMessage = new ProtocolMessage();
                pluginsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_PLUGINS);

                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", TCP_PORT);

                Stream stream = client.GetStream();

                byte[] response = Protocol.CraftMessage(pluginsMessage);
                stream.Write(response, 0, response.Length);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ERROR!");
            }

            return true;
        }
    }
}
