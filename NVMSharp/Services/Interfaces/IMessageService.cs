using System.Threading.Tasks;
using System.Windows;

namespace NVMSharp.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageBoxResult> ShowAsync(string message, string title, MessageBoxButton buttons, MessageBoxImage image);
    }
}
