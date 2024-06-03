using System.Windows;

namespace WPFPSWDMNG
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the services
            IPasswordGenerator passwordGenerator = new PasswordGenerator();
            IFileService fileService = new FileService();
            IPasswordProtector passwordProtector = new PasswordProtector();
            IMessageBoxService messageBoxService = new MessageBoxService();

            // Pass the services to the view model
            DataContext = new PasswordManagerViewModel(passwordGenerator, fileService, passwordProtector, messageBoxService);
        }
    }
}
