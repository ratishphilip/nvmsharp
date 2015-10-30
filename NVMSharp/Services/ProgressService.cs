using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NVMSharp.Services.Interfaces;
using NVMSharp.Views;

namespace NVMSharp.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IDispatcherService _dispatcherService;

        public ProgressService(IDispatcherService service)
        {
            _dispatcherService = service;
        }

        public void ShowProgress(string message)
        {
            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
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
                    var win = new ProgressWindow { Owner = activeWindow };
                    win.SetMessage(message);
                    win.ShowDialog();
                }

            }));
        }

        public void HideProgress()
        {
            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
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

                if ((activeWindow as ProgressWindow) != null)
                {
                    activeWindow.Close();
                }

            }));
        }
    }
}
