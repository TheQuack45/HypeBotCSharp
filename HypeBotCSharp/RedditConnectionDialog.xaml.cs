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

namespace HypeBotCSharp
{
    /// <summary>
    /// Interaction logic for RedditConnectionDialog.xaml
    /// </summary>
    public partial class RedditConnectionDialog : Window
    {
        public RedditConnectionDialog()
        {
            InitializeComponent();
        }

        private void ircConnectSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.redditUsername = redditConnectUsernameTextbox.Text;
            MainWindow.redditPassword = redditConnectPasswordTextbox.Text;

            this.Close();
        }

        private void redditConnectUsernameTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear text box for entry
            if (redditConnectUsernameTextbox.Text == "Username")
            {
                redditConnectUsernameTextbox.Text = "";
            }
        }

        private void redditConnectUsernameTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Fill text box with placeholder
            if (redditConnectUsernameTextbox.Text == "")
            {
                redditConnectUsernameTextbox.Text = "Username";
            }
        }

        private void redditConnectPasswordTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear text box for entry
            if (redditConnectPasswordTextbox.Text == "Password")
            {
                redditConnectPasswordTextbox.Text = "";
            }
        }

        private void redditConnectPasswordTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Fill text box with placeholder
            if (redditConnectPasswordTextbox.Text == "")
            {
                redditConnectPasswordTextbox.Text = "Password";
            }
        }
    }
}
