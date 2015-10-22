﻿using System;
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
using System.Net.Sockets;
using RedditSharp;

namespace HypeBotCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isIrcConnected = false;

        public static IrcClient ircClient;
        public static IrcUser ircConnectionUser;
        public static string ircHostname;
        public static string ircNick;
        public static string ircPass = null;
        public static string ircUser = null;
        public static bool ircUsePass = false;


        public static string redditUsername;
        public static string redditPassword;

        List<string> previousCommandList = new List<string>();
        int previousCommandListIndex;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string commandText = cmdInputTextBox.Text;
            string[] commandList = Regex.Split(commandText, " ");
            previousCommandList.Add(commandText);
            previousCommandListIndex = previousCommandList.Count;
            cmdInputTextBox.Clear();

            if (commandList[0] == ":IRC")
            {
                if (isIrcConnected)
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
                        // Admin has chosen to manually send a message
                        if (ircClient.Channels.Any()) {
                            // Client is connected to at least one channel
                            IrcChannel channel = null;
                            if (commandList[2].StartsWith("#"))
                            {
                                // Channel was specified in command
                                try
                                {
                                    channel = ircClient.Channels[commandList[2]];
                                }
                                catch (Exception sendMessageException)
                                {
                                    AppendErrorText(botOutputBox, "You must specify a channel!");
                                }
                            }
                            else
                            {
                                // Channel was not specified in command; send message to most recently connected channel
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
                                // Message was not sent successfully
                                botOutputBox.AppendText(sendMessageException.ToString());
                                return;
                            }
                            // Write bot name and message to output box
                            botOutputBox.AppendText("    <" + ircClient.User.Nick + "> " + message);
                        }
                        else
                        {
                            // Client is not connected to any channel
                            AppendErrorText(botOutputBox, "You must join a channel to send a message!");
                        }
                    }
                    else
                    {
                        // Command unrecognized
                        AppendErrorText(botOutputBox, "Command not recognized!");
                    }
                }
                else
                {
                    // Admin has attempted to make an IRC command without being connected to an IRC server
                    // Command must fail
                    AppendErrorText(botOutputBox, "Please connect to an IRC server by using 'Setup -> Connect' before making IRC commands");
                }
            }
            else
            {
                // Command unrecognized
                AppendErrorText(botOutputBox, "Command not recognized!");
            }

            botOutputBox.AppendText("\r");
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // Check if computer has valid internet connection
            if (UtilFunc.IsHostnameValid("www.microsoft.com"))
            {
                // Internet connection exists, continue program run
                return;
            }
            else
            {
                // Internet connection does not exist, halt program run
                AppendErrorText(botOutputBox, "You must have a working internet connection to use this program!");
                AppendErrorText(botOutputBox, "Please establish an internet connection and restart the program.");
                mainWindowMenuSetupDropDown.IsEnabled = false;
                cmdInputTextBox.IsEnabled = false;
                cmdSubmitButton.IsEnabled = false;
            }
        }

        private void handleMessage(PrivateMessageEventArgs messageReceivedEventArgs)
        {
            string messageText = messageReceivedEventArgs.PrivateMessage.Message.ToString();
            string[] messageTextArr = Regex.Split(messageText, " ");
            if (messageTextArr[0] == "!hypeBot")
            {
                // Message is command for HypeBot
                if (messageTextArr[1] == "hello")
                {
                    string returnMessage = "Hello, " + messageReceivedEventArgs.PrivateMessage.User.Nick + "!";
                    ircClient.Channels[messageReceivedEventArgs.PrivateMessage.Source].SendMessage(returnMessage);
                    publicOutputBoxAppend(botOutputBox, "    <" + ircClient.User.Nick + "> " + returnMessage);
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
            switch (e.Key)
            {
                case Key.Return:
                    // Admin pressed return; submit command
                    RoutedEventArgs blankEventArgs = null;
                    cmdSubmitButton_Click(this, blankEventArgs);
                    break;
            }
        }

        private void AppendErrorText(RichTextBox targetBox, string text)
        {
            // Append text with automatic styling for error text
            TextRange tr = new TextRange(targetBox.Document.ContentEnd, targetBox.Document.ContentEnd);
            tr.Text = text;
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
        }

        private void cmdInputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    // Admin pressed up; switch to previous command
                    if (previousCommandListIndex - 1 >= 0)
                    {
                        previousCommandListIndex -= 1;
                        cmdInputTextBox.Text = previousCommandList[previousCommandListIndex];
                    }
                    e.Handled = true;
                    break;
                case Key.Down:
                    // Admin pressed down; switch to next command
                    if (previousCommandListIndex + 1 < previousCommandList.Count)
                    {
                        previousCommandListIndex += 1;
                        cmdInputTextBox.Text = previousCommandList[previousCommandListIndex];
                    }
                    else
                    {
                        cmdInputTextBox.Text = "";
                    }
                    e.Handled = true;
                    break;
            }
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

            // Check for full information entry
            if (ircNick == "" || ircUser == "" || ircHostname == "" || ircNick == null || ircUser == null || ircHostname == null)
            {
                // User did not enter full information; do not attempt to connect
                AppendErrorText(botOutputBox, "Please enter all required fields (Host, nick, user)\r");
                return;
            }

            botOutputBox.Document.Blocks.Clear();
            if (ircUsePass)
            {
                ircConnectionUser = new IrcUser(ircNick, ircUser, ircPass);
            }
            else
            {
                ircConnectionUser = new IrcUser(ircNick, ircUser);
            }

            if (UtilFunc.IsHostnameValid(ircHostname))
            {
                // Provided hostname is valid, continue
                ircClient = new IrcClient(ircHostname, ircConnectionUser);
            }
            else
            {
                // Provided hostname is not valid, stop and inform user
                AppendErrorText(botOutputBox, "Please enter a valid IRC server hostname!\r");
                return;
            }

            ircClient.ConnectionComplete += (s, e2) =>
            {
                publicOutputBoxAppend(botOutputBox, "Connection Completed");
                isIrcConnected = true;
            };

            ircClient.PrivateMessageRecieved += (s, channelMessageReceivedEventArgs) => handleMessage(channelMessageReceivedEventArgs);

            ircClient.RawMessageRecieved += (s, messageReceivedEventArgs) => publicOutputBoxAppend(botOutputBox, "    " + messageReceivedEventArgs.Message.ToString());

            try
            {
                ircClient.ConnectAsync();
            }
            catch (SocketException connectionError)
            {
                AppendErrorText(botOutputBox, "Something went wrong. Error:\r");
                AppendErrorText(botOutputBox, connectionError.Message + "\r");
                AppendErrorText(botOutputBox, "Please try with a different IRC server.\r");
            }
        }

        private void mainWindowMenuSetupDropDownRedditConnectButton_Click(object sender, RoutedEventArgs e)
        {
            RedditConnectionDialog redditConnectResult = new RedditConnectionDialog();
            redditConnectResult.ShowDialog();

            // Handle connection info dialog
            if (redditConnectResult != null)
            {
                redditConnectResult.Close();
            }

            // Check for full information entry
            if (redditUsername == "" || redditPassword == "" || redditUsername == null || redditPassword == null)
            {
                // User did not enter all required information, do not attempt to connect
                AppendErrorText(botOutputBox, "Please enter all required fields (Username, password)\r");
                return;
            }


        }

        private void mainWindowMenuSetupDropDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdInputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear text box for entry
            cmdInputTextBox.Text = "";
        }

        private void cmdInputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Fill text box with placeholder
            cmdInputTextBox.Text = "Input Command";
        }
    }
}
