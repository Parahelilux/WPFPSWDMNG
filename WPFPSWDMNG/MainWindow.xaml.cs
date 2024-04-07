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
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?";
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
            string website = txtWebsite.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;

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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgPasswords.SelectedItem is PasswordEntry selectedEntry)
            {
                PasswordEntries.Remove(selectedEntry);
                SavePasswordEntries();
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
                txtPassword.Foreground = Brushes.Black;
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