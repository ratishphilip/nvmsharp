using System;
using System.Threading.Tasks;
using System.Windows;
using NVMSharp.Services.Interfaces;
using NVMSharp.ViewModel;
using NVMSharp.Views;

namespace NVMSharp.Services
{
    public class ModificationService : IModificationService
    {
        private readonly IDispatcherService _dispatcherService;

        public ModificationService(IDispatcherService service)
        {
            _dispatcherService = service;
        }

        public Task<bool?> InitModification(CoreViewModel vm)
        {
            var tcs = new TaskCompletionSource<bool?>();

            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                bool? result = null;
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
                    var win = new EditWindow(vm) { Owner = activeWindow };
                    result = win.ShowDialog();
                }

                tcs.SetResult(result);
            }));


            return tcs.Task;
        }
    }
}
