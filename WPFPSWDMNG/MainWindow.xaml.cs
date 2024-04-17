using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFPSWDMNG
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<PasswordEntry> PasswordEntries { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            PasswordEntries = new ObservableCollection<PasswordEntry>();
            dgPasswords.ItemsSource = PasswordEntries;
            LoadSavedPasswords();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            int passwordLength = (int)sliderPasswordLength.Value;
            string generatedPassword = GeneratePassword(passwordLength);
            txtPassword.Text = generatedPassword;
        }

        private string GeneratePassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?/~`";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(validChars.Length);
                password.Append(validChars[index]);
            }

            return password.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string website = txtWebsite.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(website) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields before saving.");
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.");
                return;
            }

            string encryptedPassword = PasswordProtector.EncryptPassword(password);

            string passwordEntry = $"{website},{username},{encryptedPassword}";
            File.AppendAllText("passwords.txt", passwordEntry + Environment.NewLine);

            PasswordEntry newEntry = new PasswordEntry
            {
                Website = website,
                Username = username,
                Password = password
            };
            PasswordEntries.Add(newEntry);

            MessageBox.Show("Password saved successfully!");
        }

        private void LoadSavedPasswords()
        {
            if (File.Exists("passwords.txt"))
            {
                string[] lines = File.ReadAllLines("passwords.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        string website = parts[0];
                        string username = parts[1];
                        string encryptedPassword = parts[2];
                        string decryptedPassword = PasswordProtector.DecryptPassword(encryptedPassword);

                        if (!string.IsNullOrEmpty(decryptedPassword))
                        {
                            PasswordEntry passwordEntry = new PasswordEntry
                            {
                                Website = website,
                                Username = username,
                                Password = decryptedPassword
                            };
                            PasswordEntries.Add(passwordEntry);
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgPasswords.SelectedItem is PasswordEntry selectedEntry)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected password entry?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PasswordEntries.Remove(selectedEntry);
                    SavePasswordEntries();
                    MessageBox.Show("Password entry deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a password entry to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnPurgeAll_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordEntries.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to purge all password entries?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    PasswordEntries.Clear();
                    SavePasswordEntries();
                    MessageBox.Show("All password entries have been purged successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("There are no password entries to purge.", "No Entries", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgPasswords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPasswords.SelectedItem is PasswordEntry selectedEntry)
            {
                txtWebsite.Text = selectedEntry.Website;
                txtUsername.Text = selectedEntry.Username;
                txtPassword.Text = selectedEntry.Password;
            }
        }

        private void SavePasswordEntries()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PasswordEntry entry in PasswordEntries)
            {
                string encryptedPassword = PasswordProtector.EncryptPassword(entry.Password);
                string passwordEntry = $"{entry.Website},{entry.Username},{encryptedPassword}";
                sb.AppendLine(passwordEntry);
            }

            File.WriteAllText("passwords.txt", sb.ToString());
        }

        private void txtWebsite_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtWebsite.Text == "Website/Service")
            {
                txtWebsite.Text = string.Empty;
                txtWebsite.Foreground = Brushes.Black;
            }
        }

        private void txtWebsite_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWebsite.Text))
            {
                txtWebsite.Text = "Website/Service";
                txtWebsite.Foreground = Brushes.Gray;
            }
        }

        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "Username/Email")
            {
                txtUsername.Text = string.Empty;
                txtUsername.Foreground = Brushes.Black;
            }
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Username/Email";
                txtUsername.Foreground = Brushes.Gray;
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Text == "Password")
                {
                txtPassword.Text = string.Empty;
                txtPassword.Foreground = Brushes.Gray;
            }
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.Text = "Password";
                txtPassword.Foreground = Brushes.Gray;
            }
        }
    }
}