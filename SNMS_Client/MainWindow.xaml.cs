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
        static List<Account> accountList;

        static TcpClient client;
        static NetworkStream stream;

        public MainWindow()
        {
            pluginList = new List<Plugin>();
            InitializeComponent();
            LoadStartValues();
            //Available_Plugins;
        }

        bool ConnectionProcedure()
        {
            ProtocolMessage connectionMessage = new ProtocolMessage();
            connectionMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONNECTION);
            connectionMessage.AddParameter("client");

            client = new TcpClient();
            client.Connect("127.0.0.1", TCP_PORT);

            stream = client.GetStream();

            ConnectionHandler.SendMessage(stream, connectionMessage);
            ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

            return true;
        }

        bool LoadPluginsTab()
        {
            Plugins_Available_Plugins.Items.Clear();
            
            ProtocolMessage pluginsMessage = new ProtocolMessage();
            pluginsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_PLUGINS);

            ConnectionHandler.SendMessage(stream, pluginsMessage);
            ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

            pluginList = Plugin.ParseMessage(responseMessage);
            foreach (Plugin plugin in pluginList)
            {
                Plugins_Available_Plugins.Items.Add(plugin.GetName());
            }

            return true;
        }

        bool LoadAccountsTab()
        {
            Accounts_Available_Plugins.Items.Clear();
            Account_ComboBox.SelectedIndex = -1;

            foreach (Plugin plugin in pluginList)
            {
                Accounts_Available_Plugins.Items.Add(plugin.GetName());
            }
            return true;
        }

        bool LoadConfigurationsTab()
        {
            Accounts_Available_Plugins.Items.Clear();
            Account_ComboBox.SelectedIndex = -1;

            foreach (Plugin plugin in pluginList)
            {
                Accounts_Available_Plugins.Items.Add(plugin.GetName());
            }
            return true;
        }

        bool LoadStartValues()
        {
            try
            {
                ConnectionProcedure();
                //LoadPluginsTab();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ERROR!");
            }

            return true;
        }

        private void Plugins_Available_Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Plugins_Available_Plugins.SelectedIndex == -1)
            {
                Plugin_Name.Text = "";
                Plugin_Description.Text = "";
                Plugin_Enable_Button.Content = "Enabled";
            }
            else
            {
                Plugin plugin = pluginList[Plugins_Available_Plugins.SelectedIndex];
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

        private void Accounts_Available_Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Account_ComboBox.Items.Clear();

            Account_ComboBox.SelectedIndex = -1;
            Account_Name.Text = "";
            Account_Description.Text = "";
            Account_User_Name.Text = "";
            Account_Password.Password = "";

            if (Accounts_Available_Plugins.SelectedIndex != -1)
            {
                int dwIndex = Accounts_Available_Plugins.SelectedIndex;
                Plugin plugin = pluginList[dwIndex];
                int dwPluginID = plugin.GetID();

                ProtocolMessage accountsMessage = new ProtocolMessage();
                accountsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_ACCOUNTS);

                accountsMessage.AddParameter(dwPluginID);

                ConnectionHandler.SendMessage(stream, accountsMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                accountList = Account.ParseMessage(responseMessage, plugin);

                foreach (Account account in accountList)
                {
                    Account_ComboBox.Items.Add(account.GetName());
                }
            }
        }

        private void Account_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Account_ComboBox.SelectedIndex;
            if ( index == -1)
            {
                Account_Name.Text = "";
                Account_Description.Text = "";
                Account_User_Name.Text = "";
                Account_Password.Password = "";
            }
            else
            {
                Account account = accountList[index];
                Account_Name.Text = account.GetName();
                Account_Description.Text = account.GetDescription();
                Account_User_Name.Text = account.GetUserName();
                Account_Password.Password = account.GetPassword();
            }
        }

        private void Main_Tab_Controller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem)
            {
                switch (Main_Tab_Controller.SelectedIndex)
                {
                    case 0:
                        LoadPluginsTab();
                        break;

                    case 1:
                        LoadAccountsTab();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
