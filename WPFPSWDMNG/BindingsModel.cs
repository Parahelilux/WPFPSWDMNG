using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WPFPSWDMNG
{
    public class BindingsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private PasswordEntry _selectedPasswordEntry;
        public PasswordEntry SelectedPasswordEntry
        {
            get { return _selectedPasswordEntry; }
            set { _selectedPasswordEntry = value; OnPropertyChanged(nameof(SelectedPasswordEntry)); }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<PasswordEntry> PasswordEntries { get; set; }

        private string? _website;
        public string? Website
        {
            get { return _website; }
            set { _website = value; OnPropertyChanged(nameof(Website)); }
        }

        private string? _username;
        public string? Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string? _password;
        public string? Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        private int _passwordLength;
        public int PasswordLength
        {
            get { return _passwordLength; }
            set { _passwordLength = value; OnPropertyChanged(nameof(PasswordLength)); }
        }

        public ICommand GenerateCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand PurgeAllCommand { get; set; }

        public BindingsModel()
        {
            PasswordEntries = new ObservableCollection<PasswordEntry>();
            LoadSavedPasswords();

            GenerateCommand = new RelayCommand(GeneratePassword);
            SaveCommand = new RelayCommand(SavePassword);
            DeleteCommand = new RelayCommand<PasswordEntry>(DeletePassword);
            PurgeAllCommand = new RelayCommand(PurgeAllPasswords);
        }

        private void GeneratePassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?/~`";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < PasswordLength; i++)
            {
                int index = random.Next(validChars.Length);
                password.Append(validChars[index]);
            }

            Password = password.ToString();
        }

        private void SavePassword()
        {
            if (string.IsNullOrWhiteSpace(Website) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please fill in all fields before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string encryptedPassword = PasswordProtector.EncryptPassword(Password);
            string passwordEntry = $"{Website},{Username},{encryptedPassword}";
            File.AppendAllText("passwords.txt", passwordEntry + Environment.NewLine);

            PasswordEntry newEntry = new PasswordEntry
            {
                Website = Website,
                Username = Username,
                Password = Password
            };
            PasswordEntries.Add(newEntry);

            MessageBox.Show("Password entry saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

                        PasswordEntry entry = new PasswordEntry
                        {
                            Website = website,
                            Username = username,
                            Password = decryptedPassword
                        };
                        PasswordEntries.Add(entry);
                    }
                }
            }
        }

        private void DeletePassword(PasswordEntry selectedEntry)
        {
            if (selectedEntry != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected password entry?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PasswordEntries.Remove(selectedEntry);
                    SavePasswordEntries();
                    MessageBox.Show("Password entry deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a password entry to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PurgeAllPasswords()
        {
            if (PasswordEntries.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to purge all password entries?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    PasswordEntries.Clear();
                    SavePasswordEntries();
                    MessageBox.Show("All password entries have been purged successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("There are no password entries to purge.", "No Entries", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute();
            }

            public void Execute(object parameter)
            {
                _execute();
            }
        }

        private class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Predicate<T> _canExecute;

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute((T)parameter);
            }

            public void Execute(object parameter)
            {
                _execute((T)parameter);
            }
        }
    }
}