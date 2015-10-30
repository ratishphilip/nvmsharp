using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NVMSharp.Services.Interfaces
{
    public interface IDispatcherService
    {
        Dispatcher CurrentDispatcher { get; }
    }
}
