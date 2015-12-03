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
    /// Interaction logic for RedditSubredditDialog.xaml
    /// </summary>
    public partial class RedditSubredditDialog : Window
    {
        public RedditSubredditDialog()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
        }

        private void parentSubredditDialogSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.parentSubredditString = this.parentSubredditTextBox.Text;
            MainWindow.parentSubredditCounterStickyId = this.parentSubredditCounterStickyTextPost.Text;
            this.Close();
        }

        private void parentSubredditTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear text box for entry
            if (parentSubredditTextBox.Text == "Parent Subreddit")
            {
                parentSubredditTextBox.Text = "";
            }
        }

        private void parentSubredditTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Fill text box with placeholder
            if (parentSubredditTextBox.Text == "")
            {
                parentSubredditTextBox.Text = "Parent Subreddit";
            }
        }

        private void parentSubredditCounterStickyTextPost_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear text box for entry
            if (parentSubredditCounterStickyTextPost.Text == "Counter Sticky Post ID")
            {
                parentSubredditCounterStickyTextPost.Text = "";
            }
        }

        private void parentSubredditCounterStickyTextPost_LostFocus(object sender, RoutedEventArgs e)
        {
            // Fill text box with placeholder
            if (parentSubredditCounterStickyTextPost.Text == "")
            {
                parentSubredditCounterStickyTextPost.Text = "Counter Sticky Post ID";
            }
        }
    }
}
