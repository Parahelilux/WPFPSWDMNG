using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace WPFPSWDMNG
{
    public class PasswordManagerViewModel : INotifyPropertyChanged, IPasswordManagerViewModel
    {
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IFileService _fileService;
        private readonly IPasswordProtector _passwordProtector;
        private readonly IMessageBoxService _messageBoxService;

        public event PropertyChangedEventHandler PropertyChanged;

        public PasswordManagerViewModel(IPasswordGenerator passwordGenerator, IFileService fileService, IPasswordProtector passwordProtector, IMessageBoxService messageBoxService)
        {
            _passwordGenerator = passwordGenerator;
            _fileService = fileService;
            _passwordProtector = passwordProtector;
            _messageBoxService = messageBoxService;

            PasswordEntries = new ObservableCollection<PasswordEntry>();
            LoadSavedPasswords();

            GenerateCommand = new RelayCommand(GeneratePassword);
            SaveCommand = new RelayCommand(SavePassword);
            DeleteCommand = new RelayCommand<PasswordEntry>(DeletePassword);
            PurgeAllCommand = new RelayCommand(PurgeAllPasswords);
            ChangeFilePathCommand = new RelayCommand(ChangeFilePath);
            LoadFromFileCommand = new RelayCommand(LoadFromFile);
        }

        public ObservableCollection<PasswordEntry> PasswordEntries { get; set; }

        private PasswordEntry _selectedPasswordEntry;
        public PasswordEntry SelectedPasswordEntry
        {
            get { return _selectedPasswordEntry; }
            set { _selectedPasswordEntry = value; OnPropertyChanged(); }
        }

        private string _filePath = "passwords.txt";
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; OnPropertyChanged(); }
        }

        private string _website;
        public string Website
        {
            get { return _website; }
            set { _website = value; OnPropertyChanged(); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private int _passwordLength;
        public int PasswordLength
        {
            get { return _passwordLength; }
            set { _passwordLength = value; OnPropertyChanged(); }
        }

        public ICommand GenerateCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PurgeAllCommand { get; }
        public ICommand ChangeFilePathCommand { get; }
        public ICommand LoadFromFileCommand { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GeneratePassword()
        {
            Password = _passwordGenerator.Generate(PasswordLength);
        }

        private void SavePassword()
        {
            if (string.IsNullOrWhiteSpace(Website) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                _messageBoxService.Show("Please fill in all fields before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password.Length < 8)
            {
                _messageBoxService.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string encryptedPassword = _passwordProtector.EncryptPassword(Password);
            string passwordEntry = $"{Website},{Username},{encryptedPassword}";
            _fileService.Save(FilePath, new[] { passwordEntry });

            PasswordEntry newEntry = new PasswordEntry
            {
                Website = Website,
                Username = Username,
                Password = Password
            };
            PasswordEntries.Add(newEntry);

            _messageBoxService.Show("Password entry saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadSavedPasswords()
        {
            var lines = _fileService.Load(FilePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 3)
                {
                    var website = parts[0];
                    var username = parts[1];
                    var encryptedPassword = parts[2];
                    var decryptedPassword = _passwordProtector.DecryptPassword(encryptedPassword);

                    var entry = new PasswordEntry
                    {
                        Website = website,
                        Username = username,
                        Password = decryptedPassword
                    };
                    PasswordEntries.Add(entry);
                }
            }
        }

        private void DeletePassword(PasswordEntry selectedEntry)
        {
            if (selectedEntry != null)
            {
                var result = _messageBoxService.Show("Are you sure you want to delete the selected password entry?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PasswordEntries.Remove(selectedEntry);
                    SavePasswordEntries();
                    _messageBoxService.Show("Password entry deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                _messageBoxService.Show("Please select a password entry to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PurgeAllPasswords()
        {
            if (PasswordEntries.Count > 0)
            {
                var result = _messageBoxService.Show("Are you sure you want to purge all password entries?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    PasswordEntries.Clear();
                    SavePasswordEntries();
                    _messageBoxService.Show("All password entries have been purged successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                _messageBoxService.Show("There are no password entries to purge.", "No Entries", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SavePasswordEntries()
        {
            var lines = new List<string>();
            foreach (var entry in PasswordEntries)
            {
                var encryptedPassword = _passwordProtector.EncryptPassword(entry.Password);
                var passwordEntry = $"{entry.Website},{entry.Username},{encryptedPassword}";
                lines.Add(passwordEntry);
            }

            _fileService.Save(FilePath, lines);
        }

        private void ChangeFilePath()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                LoadSavedPasswords();
            }
        }

        private void LoadFromFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                LoadSavedPasswords();
            }
        }
    }
}
