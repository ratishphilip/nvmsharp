using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using NVMSharp.Services.Interfaces;

namespace NVMSharp.Services
{
    public class DispatcherService : IDispatcherService
    {
        public Dispatcher CurrentDispatcher => GetCurrentDispatcher();

        private Dispatcher GetCurrentDispatcher()
        {
            return System.Windows.Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        }
    }
}
