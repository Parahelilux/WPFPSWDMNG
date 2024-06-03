using System.Windows;

namespace WPFPSWDMNG
{
    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return MessageBox.Show(message, caption, buttons, icon);
        }
    }
}
