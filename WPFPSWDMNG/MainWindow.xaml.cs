using System.Windows;

namespace WPFPSWDMNG
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new BindingsModel();
        }
    }
}