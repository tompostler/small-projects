namespace Unlimitedinf.OneOff.Communicator
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for PasswordPromptWindow.xaml
    /// </summary>
    public partial class PasswordPromptWindow : Window
    {
        private bool InClearText = false;
        internal string Password => this.InClearText ? this.clearpassbox.Text: this.passbox.Password;

        public PasswordPromptWindow(bool inClearText = false)
        {
            InitializeComponent();
            this.passbox.Focus();
            if (inClearText)
                this.ShowAsClearText();
        }

        private void ShowAsClearText()
        {
            this.InClearText = true;
            this.passbox.Visibility = Visibility.Hidden;
            this.clearpassbox.Visibility = Visibility.Visible;
            this.clearpassbox.Focus();
        }

        private void box_keyup(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
