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

        static List<Configuration> configurationList;
        static List<Configuration> configurationWorkingSetList;
        static List<Sequence> configurationWorkingSetSequences;

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
            Configuration_Available_Plugins.Items.Clear();
            Configuration_ComboBox.SelectedIndex = -1;

            foreach (Plugin plugin in pluginList)
            {
                Configuration_Available_Plugins.Items.Add(plugin.GetName());
            }
            return true;
        }

        bool LoadTriggerTypesTab()
        {
            TriggerTypes_Available_Plugins.Items.Clear();
            TriggerTypes_ComboBox.SelectedIndex = -1;

            foreach (Plugin plugin in pluginList)
            {
                TriggerTypes_Available_Plugins.Items.Add(plugin.GetName());
            }
            return true;
        }

        bool LoadTriggersTab()
        {
            Trigger_Available_Plugin.Items.Clear();
            Trigger_ComboBox.SelectedIndex = -1;

            foreach (Plugin plugin in pluginList)
            {
                Trigger_Available_Plugin.Items.Add(plugin.GetName());
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
            Account_ComboBox.SelectedIndex = -1;
            Account_ComboBox.Items.Clear();

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

                    case 2:
                        LoadConfigurationsTab();
                        break;

                    case 3:
                        LoadTriggerTypesTab();
                        break;

                    case 4:
                        LoadTriggersTab();
                        break;

                    default:
                        break;
                }
            }
        }

        private void Configuration_Available_Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Configuration_Account_ComboBox.SelectedIndex = -1;
 
            if (Configuration_Available_Plugins.SelectedIndex != -1)
            {
                int dwIndex = Accounts_Available_Plugins.SelectedIndex;
                Plugin plugin = pluginList[dwIndex];
                int dwPluginID = plugin.GetID();

                ProtocolMessage accountsMessage = new ProtocolMessage();
                accountsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS);

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

        private void Configuration_Account_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Configuration_ComboBox.SelectedIndex = -1;
            Configuration_ComboBox.Items.Clear();

            int index = Configuration_Account_ComboBox.SelectedIndex;
            if (index != -1)
            {
                Account account = accountList[index];
                configurationWorkingSetList = Configuration.CreateWorkingSet(configurationList, account);

                foreach (Configuration configuration in configurationWorkingSetList)
                {
                    Configuration_ComboBox.Items.Add(configuration.GetName());
                }
            }
        }

        private void Configuration_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Configuration_ComboBox.SelectedIndex;
            if (index == -1)
            {
                Configuration_Name.Text = "";
                Configuration_Description.Text = "";
                Configuration_Sequence_ComboBox.SelectedIndex = -1;
                Configuration_Enabled_Button.Content = "Enabled";
            }
            else
            {
                Configuration configuration = configurationWorkingSetList[index];
                Configuration_Name.Text = configuration.GetName();
                Configuration_Description.Text = configuration.GetDescription();
                
                Configuration_Sequence_ComboBox.SelectedIndex = -1;
                
                bool isEnabled = configuration.GetEnabled();
                if (isEnabled)
                {
                    Configuration_Enabled_Button.Content = "Enabled";
                }
                else
                {
                    Configuration_Enabled_Button.Content = "Disabled";
                }
            }
        }

        private void Configuration_Sequence_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Configuration_Sequence_ComboBox.SelectedIndex;
            if (index == -1)
            {
                Configuration_Sequence_Enabled_CheckBox.IsChecked = false;
            }
            else
            {
                // set value
            }
            
        }
        
    }
}
