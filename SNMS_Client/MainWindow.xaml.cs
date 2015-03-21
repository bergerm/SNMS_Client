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
using SNMS_Client.Objects;

namespace SNMS_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int TCP_PORT = 56824;
        static List<Plugin> pluginList;

        public MainWindow()
        {
            pluginList = new List<Plugin>();
            InitializeComponent();
            LoadStartValues();
            //Available_Plugins;
        }

        bool LoadStartValues()
        {
            try
            {
                ProtocolMessage connectionMessage = new ProtocolMessage();
                connectionMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONNECTION);
                connectionMessage.AddParameter("client");

                ProtocolMessage pluginsMessage = new ProtocolMessage();
                pluginsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_PLUGINS);

                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", TCP_PORT);

                Stream stream = client.GetStream();

                //byte[] response = Protocol.CraftMessage(pluginsMessage);
                //stream.Write(response, 0, response.Length);
                //stream.Flush();
                ConnectionHandler.SendMessage(stream, connectionMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);
                ConnectionHandler.SendMessage(stream, pluginsMessage);
                responseMessage = ConnectionHandler.GetMessage(stream);

                pluginList = Plugin.ParseMessage(responseMessage);
                foreach (Plugin plugin in pluginList)
                {
                    Available_Plugins.Items.Add(plugin.GetName());
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ERROR!");
            }

            return true;
        }

        private void Available_Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Available_Plugins.SelectedIndex == -1)
            {
                Plugin_Name.Text = "";
                Plugin_Description.Text = "";
                Plugin_Enable_Button.Content = "Enabled";
            }
            else
            {
                Plugin plugin = pluginList[Available_Plugins.SelectedIndex];
                Plugin_Name.Text = plugin.GetName();
                Plugin_Description.Text = plugin.GetDescription();
                bool isEnabled = plugin.GetEnabled();
                if (isEnabled)
                {
                    Plugin_Enable_Button.Content = "Enabled";
                }
                else
                {
                    Plugin_Enable_Button.Content = "Disabled";
                }
                
            }
        }
    }
}
