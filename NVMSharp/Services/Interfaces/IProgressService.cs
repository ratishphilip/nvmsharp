using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVMSharp.Services.Interfaces
{
    public interface IProgressService
    {
        void ShowProgress(string message);
        void HideProgress();
    }
}
