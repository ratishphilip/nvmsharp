using System;
using System.Threading.Tasks;
using System.Windows;
using NVMSharp.Services.Interfaces;
using NVMSharp.Views;

namespace NVMSharp.Services
{
    public class MessageService : IMessageService
    {
        private readonly IDispatcherService _dispatcherService;

        public MessageService(IDispatcherService service)
        {
            _dispatcherService = service;
        }

        public Task<MessageBoxResult> ShowAsync(string message, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            var tcs = new TaskCompletionSource<MessageBoxResult>();

            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                MessageBoxResult result;
                Window activeWindow = null;
                for (var i = 0; i < Application.Current.Windows.Count; i++)
                {
                    var win = Application.Current.Windows[i];
                    if ((win != null) && (win.IsActive))
                    {
                        activeWindow = win;
                        break;
                    }
                }

                if (activeWindow != null)
                {
                    result = MessageBox.Show(activeWindow, message, title, buttons, image);
                }
                else
                {
                    result = MessageBox.Show(message, title, buttons, image);
                }

                tcs.SetResult(result);
            }));


            return tcs.Task;
        }
    }
}
