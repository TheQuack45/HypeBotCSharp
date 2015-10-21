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
    /// Interaction logic for IrcConnectionDialog.xaml
    /// </summary>
    public partial class IrcConnectionDialog : Window
    {
        private string ircHostname;
        private string ircNick;
        private string ircPass = null;
        public IrcConnectionDialog()
        {
            InitializeComponent();
        }

        private void ircConnectSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ircHostname = this.ircConnectHostTextBox.Text;
            MainWindow.ircNick = this.ircConnectNickTextBox.Text;
            MainWindow.ircUser = this.ircConnectUserTextBox.Text;
            if ((bool)this.ircConnectUsePassCheckBox.IsChecked)
            {
                MainWindow.ircPass = this.ircConnectPassTextBox.Text;
                MainWindow.usePass = true;
            }

            this.Close();
        }

        private void ircConnectUsePassCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ircConnectPassTextBox.IsEnabled = true;
        }

        private void ircConnectUsePassCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ircConnectPassTextBox.IsEnabled = false;
        }
    }
}
