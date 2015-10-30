using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NVMSharp.ViewModel;

namespace NVMSharp.Services.Interfaces
{
    public interface IModificationService
    {
        Task<bool?> InitModification(CoreViewModel vm);
    }
}
