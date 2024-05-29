using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WPFPSWDMNG
{
    public interface IPasswordManagerViewModel
    {
        ObservableCollection<PasswordEntry> PasswordEntries { get; set; }
        PasswordEntry SelectedPasswordEntry { get; set; }
        string FilePath { get; set; }
        string Website { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        int PasswordLength { get; set; }

        ICommand GenerateCommand { get; }
        ICommand SaveCommand { get; }
        ICommand DeleteCommand { get; }
        ICommand PurgeAllCommand { get; }
        ICommand ChangeFilePathCommand { get; }
        ICommand LoadFromFileCommand { get; }
    }
}
