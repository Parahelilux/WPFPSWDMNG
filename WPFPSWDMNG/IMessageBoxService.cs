using System.Windows;

namespace WPFPSWDMNG
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon);
    }
}
