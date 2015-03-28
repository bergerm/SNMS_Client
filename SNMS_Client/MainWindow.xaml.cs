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

        static List<Sequence> sequenceList;

        static List<TriggerType> triggerTypesList;

        static List<SNMS_Client.Objects.Trigger> triggersList;

        static List<UserType> userTypesList;

        static List<User> usersList;

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
            Configuration_Account_ComboBox.Items.Clear();
            Configuration_Account_ComboBox.SelectedIndex = -1;

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

        bool LoadUsersTab()
        {
            User_ComboBox.Items.Clear();
            User_ComboBox.SelectedIndex = -1;

            ProtocolMessage userTypesMessage = new ProtocolMessage();
            userTypesMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USER_TYPES);

            ConnectionHandler.SendMessage(stream, userTypesMessage);
            ProtocolMessage responseTypesMessage = ConnectionHandler.GetMessage(stream);

            userTypesList = UserType.ParseMessage(responseTypesMessage);

            ProtocolMessage usersMessage = new ProtocolMessage();
            usersMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USERS);

            ConnectionHandler.SendMessage(stream, usersMessage);
            ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

            usersList = User.ParseMessage(responseMessage, userTypesList);

            foreach (User user in usersList)
            {
                User_ComboBox.Items.Add(user.GetName());
            }
            if (usersList.Count > 0)
            {
                User_ComboBox.SelectedIndex = 0;
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

                    case 5:
                        LoadUsersTab();
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
                int dwIndex = Configuration_Available_Plugins.SelectedIndex;
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
                    Configuration_Account_ComboBox.Items.Add(account.GetName());
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
                //configurationWorkingSetList = Configuration.CreateWorkingSet(configurationList, account);

                ProtocolMessage accountsMessage = new ProtocolMessage();
                accountsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS);

                accountsMessage.AddParameter(account.GetID());

                ConnectionHandler.SendMessage(stream, accountsMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                configurationList = Configuration.ParseMessage(responseMessage, account);

                foreach (Configuration configuration in configurationList)
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
                Configuration configuration = configurationList[index];
                Configuration_Name.Text = configuration.GetName();
                Configuration_Description.Text = configuration.GetDescription();
                
                Configuration_Sequence_ComboBox.SelectedIndex = -1;
                Configuration_Sequence_ComboBox.Items.Clear();
                
                bool isEnabled = configuration.GetEnabled();
                if (isEnabled)
                {
                    Configuration_Enabled_Button.Content = "Enabled";
                }
                else
                {
                    Configuration_Enabled_Button.Content = "Disabled";
                }

                ProtocolMessage sequencesMessage = new ProtocolMessage();
                sequencesMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_SEQUENCES);

                sequencesMessage.AddParameter(configuration.GetID());

                ConnectionHandler.SendMessage(stream, sequencesMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                sequenceList = Sequence.ParseMessage(responseMessage, configuration);

                foreach (Sequence sequence in sequenceList)
                {
                    Configuration_Sequence_ComboBox.Items.Add(sequence.GetName());
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
                Sequence sequence = sequenceList[index];
                Configuration_Sequence_Enabled_CheckBox.IsChecked = sequence.GetEnabled();
            }
            
        }

        private void TriggerTypes_Available_Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TriggersTypes_Account_ComboBox.SelectedIndex = -1;
            TriggersTypes_Account_ComboBox.Items.Clear();

            if (TriggerTypes_Available_Plugins.SelectedIndex != -1)
            {
                int dwIndex = TriggerTypes_Available_Plugins.SelectedIndex;
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
                    TriggersTypes_Account_ComboBox.Items.Add(account.GetName());
                }
            }
        }

        private void TriggersTypes_Account_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TriggersTypes_Configuration_ComboBox.SelectedIndex = -1;
            TriggersTypes_Configuration_ComboBox.Items.Clear();

            int index = TriggersTypes_Account_ComboBox.SelectedIndex;
            if (index != -1)
            {
                Account account = accountList[index];

                ProtocolMessage accountsMessage = new ProtocolMessage();
                accountsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS);

                accountsMessage.AddParameter(account.GetID());

                ConnectionHandler.SendMessage(stream, accountsMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                configurationList = Configuration.ParseMessage(responseMessage, account);

                foreach (Configuration configuration in configurationList)
                {
                    TriggersTypes_Configuration_ComboBox.Items.Add(configuration.GetName());
                }
            }
        }

        private void TriggersTypes_Configuration_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TriggerTypes_ComboBox.SelectedIndex = -1;
            TriggerTypes_ComboBox.Items.Clear();

            int index = TriggersTypes_Configuration_ComboBox.SelectedIndex;
            if (index != -1)
            {
                Configuration configuration = configurationList[index];

                ProtocolMessage configurationMessage = new ProtocolMessage();
                configurationMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGER_TYPES);

                configurationMessage.AddParameter(configuration.GetID());

                ConnectionHandler.SendMessage(stream, configurationMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                triggerTypesList = TriggerType.ParseMessage(responseMessage, configuration);

                foreach (TriggerType type in triggerTypesList)
                {
                    TriggerTypes_ComboBox.Items.Add(type.GetName());
                }
            }
        }

        private void TriggerTypes_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index =TriggerTypes_ComboBox.SelectedIndex;
            if (index == -1)
            {
                TriggerTypes_Name.Text = "";
                TriggerTypes_Description.Text = "";
            }
            else
            {
                TriggerType type = triggerTypesList[index];
                TriggerTypes_Name.Text = type.GetName();
                TriggerTypes_Description.Text = type.GetDescription();
            }
        }

        private void Trigger_Available_Plugin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Trigger_Account_ComboBox.SelectedIndex = -1;
            Trigger_Account_ComboBox.Items.Clear();

            if (Trigger_Available_Plugin.SelectedIndex != -1)
            {
                int dwIndex = Trigger_Available_Plugin.SelectedIndex;
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
                    Trigger_Account_ComboBox.Items.Add(account.GetName());
                }
            }
        }

        private void Trigger_Account_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Trigger_Configuration_ComboBox.SelectedIndex = -1;
            Trigger_Configuration_ComboBox.Items.Clear();

            int index = Trigger_Account_ComboBox.SelectedIndex;
            if (index != -1)
            {
                Account account = accountList[index];

                ProtocolMessage configurationsMessage = new ProtocolMessage();
                configurationsMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS);

                configurationsMessage.AddParameter(account.GetID());

                ConnectionHandler.SendMessage(stream, configurationsMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                configurationList = Configuration.ParseMessage(responseMessage, account);

                foreach (Configuration configuration in configurationList)
                {
                    Trigger_Configuration_ComboBox.Items.Add(configuration.GetName());
                }
            }

        }

        private void Trigger_Configuration_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Trigger_TriggerType_ComboBox.SelectedIndex = -1;
            Trigger_TriggerType_ComboBox.Items.Clear();

            int index = Trigger_Configuration_ComboBox.SelectedIndex;
            if (index != -1)
            {
                Configuration configuration = configurationList[index];

                ProtocolMessage triggerTypesMessage = new ProtocolMessage();
                triggerTypesMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGER_TYPES);

                triggerTypesMessage.AddParameter(configuration.GetID());

                ConnectionHandler.SendMessage(stream, triggerTypesMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                triggerTypesList = TriggerType.ParseMessage(responseMessage, configuration);

                foreach (TriggerType type in triggerTypesList)
                {
                    Trigger_TriggerType_ComboBox.Items.Add(type.GetName());
                }
            }
        }

        private void Trigger_TriggerType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Trigger_ComboBox.SelectedIndex = -1;
            Trigger_ComboBox.Items.Clear();

            int index = Trigger_TriggerType_ComboBox.SelectedIndex;
            if (index != -1)
            {
                TriggerType triggerType = triggerTypesList[index];

                ProtocolMessage triggersMessage = new ProtocolMessage();
                triggersMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGERS);

                int configurationIndex = Trigger_Configuration_ComboBox.SelectedIndex;
                Configuration configuration = configurationList[configurationIndex];
                triggersMessage.AddParameter(configuration.GetID());
                triggersMessage.AddParameter(triggerType.GetID());

                ConnectionHandler.SendMessage(stream, triggersMessage);
                ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

                ProtocolMessage sequencesMessage = new ProtocolMessage();
                triggersMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_GET_SEQUENCES);
                sequencesMessage.AddParameter(configuration.GetID());
                ConnectionHandler.SendMessage(stream, triggersMessage);
                ProtocolMessage responseSequencesMessage = ConnectionHandler.GetMessage(stream);

                sequenceList = Sequence.ParseMessage(responseSequencesMessage, configuration);

                triggersList = SNMS_Client.Objects.Trigger.ParseMessage(responseMessage, configuration, triggerType, sequenceList);

                foreach (SNMS_Client.Objects.Trigger trigger in triggersList)
                {
                    Trigger_ComboBox.Items.Add(trigger.GetName());
                }
            }
        }

        private void Trigger_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Trigger_ComboBox.SelectedIndex;
            if (index == -1)
            {
                Trigger_Name.Text = "";
                Trigger_Description.Text = "";
                Trigger_Value.Text = "";
                Trigger_Reaction_ComboBox.SelectedIndex = -1;
                Trigger_Reaction_ComboBox.Items.Clear();
                Trigger_Reaction_Value.Text = "";
                Trigger_Enabled_CheckBox.IsChecked = false;
            }
            else
            {
                SNMS_Client.Objects.Trigger trigger = triggersList[index];
                Trigger_Name.Text = trigger.GetName();
                Trigger_Description.Text = trigger.GetDescription();
                Trigger_Value.Text = trigger.GetValue();
                Trigger_Reaction_Value.Text = trigger.GetReactionValue();
                Trigger_Enabled_CheckBox.IsChecked = trigger.GetEnabled();

                Trigger_Reaction_ComboBox.SelectedIndex = -1;
                Trigger_Reaction_ComboBox.Items.Clear();

                int reactionIndex = 0;
                int loopIndex = 0;
                foreach (Sequence seq in sequenceList)
                {
                    if (seq.GetName() == trigger.GetReaction().GetName())
                    {
                        reactionIndex = loopIndex;
                    }

                    Trigger_Reaction_ComboBox.Items.Add(seq.GetName());

                    loopIndex++;
                }

                Trigger_Reaction_ComboBox.SelectedIndex = reactionIndex;
            }
        }

        private void User_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int dwIndex = User_ComboBox.SelectedIndex;
            if (dwIndex == -1)
            {
                User_Name.Text = "";
                User_Password.Password = "";
                User_Type_ComboBox.SelectedIndex = -1;
                User_Type_ComboBox.Items.Clear();
                User_Enable_Read_CheckBox.IsChecked = false;
                User_Enable_Write_CheckBox.IsChecked = false;
            }
            else
            {
                User user = usersList[dwIndex];

                User_Name.Text = user.GetName();
                User_Password.Password = user.GetPassword();
                User_Enable_Read_CheckBox.IsChecked = user.GetEnableRead();
                User_Enable_Write_CheckBox.IsChecked = user.GetEnableWrite();

                User_Type_ComboBox.SelectedIndex = -1;
                User_Type_ComboBox.Items.Clear();

                int currIndex = 0;
                int userTypeIndex = 0;
                foreach (UserType type in userTypesList)
                {
                    User_Type_ComboBox.Items.Add(type.GetName());
                    if (type.GetID() == user.GetUserType().GetID())
                    {
                        userTypeIndex = currIndex;
                    }
                    currIndex++;
                }
                User_Type_ComboBox.SelectedIndex = userTypeIndex;

            }
        }

        private void Account_New_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Accounts_Available_Plugins.SelectedIndex < 0)
            {
                return;
            }
            
            ProtocolMessage newAccountMessage = new ProtocolMessage();
            newAccountMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_ACCOUNT);

            Plugin plugin = pluginList[Accounts_Available_Plugins.SelectedIndex];
            newAccountMessage.AddParameter(plugin.GetID());

            ConnectionHandler.SendMessage(stream, newAccountMessage);
            ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);  

            List<Account> list = Account.ParseMessage(responseMessage, plugin);
            Account account = list[0];

            accountList.Add(account);
            Account_ComboBox.Items.Add(account.GetName());
            Account_ComboBox.SelectedIndex = Account_ComboBox.Items.Count - 1;
        }

        private void Account_Save_Button_Click(object sender, RoutedEventArgs e)
        {
            int index = Account_ComboBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            Account account = accountList[index];
            account.SetName(Account_Name.Text);
            account.SetDescription(Account_Description.Text);
            account.SetUserName(Account_User_Name.Text);
            account.SetPassword(Account_Password.Password);

            ProtocolMessage updateAccountMessage = new ProtocolMessage();
            updateAccountMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_ACCOUNT);

            updateAccountMessage.AddParameter(account.GetID());
            updateAccountMessage.AddParameter(account.GetPlugin().GetID());
            updateAccountMessage.AddParameter(account.GetName());
            updateAccountMessage.AddParameter(account.GetDescription());
            updateAccountMessage.AddParameter(account.GetUserName());
            updateAccountMessage.AddParameter(account.GetPassword());

            ConnectionHandler.SendMessage(stream, updateAccountMessage);
        }

        private void Configuration_New_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Configuration_Available_Plugins.SelectedIndex < 0 || Configuration_Account_ComboBox.SelectedIndex < 0)
            {
                return;
            }

            ProtocolMessage newConfigurationMessage = new ProtocolMessage();
            newConfigurationMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_CONFIGURATION);

            Account account = accountList[Configuration_Account_ComboBox.SelectedIndex];
            newConfigurationMessage.AddParameter(account.GetID());

            ConnectionHandler.SendMessage(stream, newConfigurationMessage);
            ProtocolMessage responseMessage = ConnectionHandler.GetMessage(stream);

            List<Configuration> list = Configuration.ParseMessage(responseMessage, account);
            Configuration configuration = list[0];

            configurationList.Add(configuration);
            Configuration_ComboBox.Items.Add(configuration.GetName());
            Configuration_ComboBox.SelectedIndex = Configuration_ComboBox.Items.Count - 1;

        }

        private void Configuration_Enabled_Button_Click(object sender, RoutedEventArgs e)
        {
            Configuration_Enabled_Button.Content = (Configuration_Enabled_Button.Content == "Enabled") ? "Disabled" : "Enabled";
        }

        private void Configuration_Save_Button_Click(object sender, RoutedEventArgs e)
        {
            int index = Configuration_ComboBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            bool bEnabled = (Configuration_Enabled_Button.Content == "Enabled") ? true : false;
            Configuration configuration = configurationList[index];
            configuration.SetName(Configuration_Name.Text);
            configuration.SetDescription(Configuration_Description.Text);
            configuration.SetEnabled(bEnabled);

            ProtocolMessage updateConfigurationMessage = new ProtocolMessage();
            updateConfigurationMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_CONFIGURATION);

            updateConfigurationMessage.AddParameter(configuration.GetID());
            updateConfigurationMessage.AddParameter(configuration.GetAccount().GetID());
            updateConfigurationMessage.AddParameter(configuration.GetName());
            updateConfigurationMessage.AddParameter(configuration.GetDescription());
            updateConfigurationMessage.AddParameter(configuration.GetEnabled());

            ConnectionHandler.SendMessage(stream, updateConfigurationMessage);
        }

        private void Configuration_Sequence_Enabled_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int index = Configuration_Sequence_ComboBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }
            
            Sequence sequence = sequenceList[index];
            bool? checkStatus = Configuration_Sequence_Enabled_CheckBox.IsChecked;
            bool bEnabled = false;
            if (checkStatus != null)
            {
                bEnabled = (bool)checkStatus;
            }
            
            sequence.SetEnabled(bEnabled);

            ProtocolMessage updateSequenceMessage = new ProtocolMessage();
            updateSequenceMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_SEQUENCE);

            updateSequenceMessage.AddParameter(sequence.GetID());
            updateSequenceMessage.AddParameter(sequence.GetConfiguration().GetID());
            updateSequenceMessage.AddParameter(sequence.GetEnabled());

            ConnectionHandler.SendMessage(stream, updateSequenceMessage);
        }

        private void TriggerTypes_New_Button_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
