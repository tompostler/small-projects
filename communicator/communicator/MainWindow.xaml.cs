﻿namespace Unlimitedinf.OneOff.Communicator
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime StatusTextClearTime = DateTime.UtcNow;
        private bool NotClosed = true;
        private bool HasBeenEncryptedOrObscured = false;

        public MainWindow()
        {
            InitializeComponent();
            new Thread(StatusTextClear).Start();
            this.Closed += (s, e) => this.NotClosed = false;
        }

        /// <summary>
        /// Don't clear the status text for 10 seconds.
        /// </summary>
        /// <param name="message"></param>
        private void StatusTextUpdate(string message)
        {
            this.statusText.Text = message;
            this.StatusTextClearTime = DateTime.UtcNow + new TimeSpan(0, 0, 10);
        }

        /// <summary>
        /// Loop indefinitely to check if the status text should be cleared.
        /// </summary>
        private void StatusTextClear()
        {
            while (this.NotClosed)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (DateTime.UtcNow > this.StatusTextClearTime && !string.IsNullOrEmpty(this.statusText.Text))
                        this.statusText.Text = string.Empty;
                });
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Open a file and handle it accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_File_Open_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = this.NzcmOpenFileDialog();
            switch (fileDialog.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    string fileName = fileDialog.FileName;
                    string fileContents = File.ReadAllText(fileName);
                    this.mainText.Text = fileContents;
                    this.StatusTextUpdate($"Loaded {fileName}.");
                    break;

                default:
                    this.StatusTextUpdate("File not opened.");
                    break;
            }
        }

        /// <summary>
        /// Save the current text to a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_File_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (!this.HasBeenEncryptedOrObscured)
            {
                MessageBoxResult result = MessageBox.Show(this, "You want to save the file without first encrypting or obfuscating it?", "Are you sure...", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No || result == MessageBoxResult.None)
                {
                    this.StatusTextUpdate("File not saved.");
                    return;
                }
            }

            System.Windows.Forms.OpenFileDialog fileDialog = this.NzcmOpenFileDialog(false);
            switch (fileDialog.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    string fileName = fileDialog.FileName;
                    string fileContents = this.mainText.Text;
                    File.WriteAllText(fileName, fileContents);
                    this.StatusTextUpdate($"Saved as {fileName}");
                    break;

                default:
                    this.StatusTextUpdate("File not saved.");
                    break;
            }
        }

        /// <summary>
        /// Get a password, encrypt the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Security_Encrypt(object sender, RoutedEventArgs e)
        {
            PasswordPromptWindow passwordWindow = new PasswordPromptWindow(true);
            passwordWindow.Owner = this;
            passwordWindow.ShowDialog();
            string password = passwordWindow.Password;

            // Fail out
            if (string.IsNullOrEmpty(password))
            {
                this.StatusTextUpdate("No password entered.");
                return;
            }

            this.mainText.Text = Encryption.AESGCM.SimpleEncryptWithPassword(this.mainText.Text, password);
            this.HasBeenEncryptedOrObscured = true;
        }

        /// <summary>
        /// Get a password, decrypt the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Security_Decrypt(object sender, RoutedEventArgs e)
        {
            PasswordPromptWindow passwordWindow = new PasswordPromptWindow();
            passwordWindow.Owner = this;
            passwordWindow.ShowDialog();
            string password = passwordWindow.Password;

            // Fail out
            if (string.IsNullOrEmpty(password))
            {
                this.StatusTextUpdate("No password entered.");
                return;
            }

            try
            {
                this.mainText.Text = Encryption.AESGCM.SimpleDecryptWithPassword(this.mainText.Text, password);
            }
            catch (FormatException)
            {
                this.StatusTextUpdate("Could not decrypt text.");
            }
        }

        /// <summary>
        /// Display info in the status bar about this menu option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Security_Info_Click(object sender, RoutedEventArgs e)
        {
            this.StatusTextUpdate("The security menu provides ways to encrypt and decrypt the text currently displayed.");
        }

        /// <summary>
        /// Obfuscate the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Obscurity_Obfuscate(object sender, RoutedEventArgs e)
        {
            this.mainText.Text = Convert.ToBase64String(Encoding.ASCII.GetBytes(this.mainText.Text));
            this.HasBeenEncryptedOrObscured = true;
        }

        /// <summary>
        /// Deobfuscate the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Obscurity_Deobfuscate(object sender, RoutedEventArgs e)
        {
            try
            { 
            this.mainText.Text = Encoding.ASCII.GetString(Convert.FromBase64String(this.mainText.Text));
            }
            catch (FormatException)
            {
                this.StatusTextUpdate("Could not deobfuscate text.");
            }
        }

        /// <summary>
        /// Display info in the status bar about this menu option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Obscurity_Info_Click(object sender, RoutedEventArgs e)
        {
            this.StatusTextUpdate("The obscurity menu provides ways to (not very well) hide the text currently displayed.");
        }

        /// <summary>
        /// Open the type of file dialog we care about.
        /// </summary>
        /// <param name="checkFileExists"></param>
        /// <returns></returns>
        private System.Windows.Forms.OpenFileDialog NzcmOpenFileDialog(bool checkFileExists = true)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.CheckFileExists = checkFileExists;
            fileDialog.CheckPathExists = true;
            fileDialog.Filter = "Message file (*.nzcm)|*.nzcm";
            fileDialog.Multiselect = false;
            return fileDialog;
        }

        /// <summary>
        /// Clear the status text on click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statusText_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.statusText.Text = string.Empty;
        }

        /// <summary>
        /// Open a file dropped onto the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                FileInfo file = new FileInfo(files[0]);
                if (file.Extension != ".nzcm")
                {
                    this.StatusTextUpdate($"Invalid file extension: {file.Extension}");
                    return;
                }

                string fileName = file.FullName;
                string fileContents = File.ReadAllText(fileName);
                this.mainText.Text = fileContents;
                this.StatusTextUpdate($"Loaded {fileName}.");
            }
        }

        private void mainText_PreviewDrag(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }
    }
}
