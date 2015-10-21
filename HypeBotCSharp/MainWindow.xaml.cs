using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using HypeBotCSharp;
using ChatSharp;
using ChatSharp.Events;

namespace HypeBotCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isConnected = false;
        public static IrcClient ircClient;
        public static IrcUser ircConnectionUser;
        public static string ircHostname;
        public static string ircNick;
        public static string ircPass = null;
        public static string ircUser = null;
        public static bool usePass = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string commandText = cmdInputTextBox.Text;
            string[] commandList = Regex.Split(commandText, " ");

            cmdInputTextBox.Clear();

            if (commandList[0] == ":IRC")
            {
                if (isConnected)
                {
                    // Bot is connected to an IRC server
                    // Admin can provide IRC commands
                    if (commandList[1] == "join")
                    {
                        // Admin has chosen to join an IRC channel
                        ircClient.JoinChannel(commandList[2]);
                    } 
                    else if (commandList[1] == "say")
                    {
                        // Admin has chosen to manually say a message
                        IrcChannel channel = null;
                        if (commandList[2].StartsWith("#"))
                        {
                            try
                            {
                                channel = ircClient.Channels[commandList[2]];
                            }
                            catch (Exception sendMessageException)
                            {
                                botOutputBox.AppendText("You must specify a channel!");
                            }
                        }
                        else
                        {
                            channel = ircClient.Channels.Reverse().ToList()[0];
                        }
                        string message = "";
                        if (commandList[2].StartsWith("#"))
                        {
                            // Channel was specified explicitly, continue as normal
                            for (int i = 3; i < commandList.Length; i++)
                            {
                                message += commandList[i] + " ";
                            }
                        }
                        else
                        {
                            // Channel was specified implicitly, begin message creation from index 2
                            for (int i = 2; i < commandList.Length; i++)
                            {
                                message += commandList[i] + " ";
                            }
                        }
                        try
                        {
                            channel.SendMessage(message);
                        }
                        catch (Exception sendMessageException)
                        {
                            botOutputBox.AppendText(sendMessageException.ToString());
                            return;
                        }
                        botOutputBox.AppendText("    <" + ircClient.User.Nick + "> " + message);
                    }
                    else
                    {
                        // Command unrecognized
                        botOutputBox.AppendText("Command not recognized!");
                    }
                }
                else
                {
                    // Admin has attempted to make an IRC command without being connected to an IRC server
                    // Command must fail
                    botOutputBox.AppendText("Please connect to an IRC server by using 'Setup -> Connect' before making IRC commands");
                }
            }
            else
            {
                botOutputBox.AppendText("Command not recognized!");
            }

            botOutputBox.AppendText("\r");
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }

        private void mainWindowMenuSetupDropDownConnectButton_Click(object sender, RoutedEventArgs e)
        {
            IrcConnectionDialog ircConnectResult = new IrcConnectionDialog();
            ircConnectResult.ShowDialog();

            // Handle connection info dialog
            if (ircConnectResult != null)
            {
                ircConnectResult.Close();
            }
            botOutputBox.Document.Blocks.Clear();

            if (usePass)
            {
                ircConnectionUser = new IrcUser(ircNick, ircUser, ircPass);
            }
            else
            {
                ircConnectionUser = new IrcUser(ircNick, ircUser);
            }
            ircClient = new IrcClient(ircHostname, ircConnectionUser);

            ircClient.ConnectionComplete += (s, e2) =>
            {
                publicOutputBoxAppend(botOutputBox, "Connection Completed");
                isConnected = true;
            };

            ircClient.PrivateMessageRecieved += (s, channelMessageReceivedEventArgs) => handleMessage(channelMessageReceivedEventArgs);

            ircClient.RawMessageRecieved += (s, messageReceivedEventArgs) => publicOutputBoxAppend(botOutputBox, "    " + messageReceivedEventArgs.Message.ToString());

            ircClient.ConnectAsync();
        }

        private void handleMessage(PrivateMessageEventArgs messageReceivedEventArgs)
        {
            string messageText = messageReceivedEventArgs.PrivateMessage.ToString();
            string[] messageTextArr = Regex.Split(messageText, " ");
            if (messageTextArr[0] == "!hypeBot")
            {
                // Message is command for HypeBot
                if (messageTextArr[1] == "hello")
                {
                    ircClient.Channels[messageReceivedEventArgs.PrivateMessage.Source].SendMessage("Hello, " + messageReceivedEventArgs.PrivateMessage.User + "!");
                }
            }
            else
            {
                // Message is not command for HypeBot, ignore
                return;
            }
        }

        private void publicOutputBoxAppend(RichTextBox targetBox, string text)
        {
            targetBox.Dispatcher.Invoke(new Action(() => targetBox.AppendText(text + "\r")));
        }

        private void cmdInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                RoutedEventArgs blankEventArgs = null;
                cmdSubmitButton_Click(this, blankEventArgs);
            }
        }
    }
}
