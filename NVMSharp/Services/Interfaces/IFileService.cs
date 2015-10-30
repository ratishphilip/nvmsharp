using System;
using System.Threading.Tasks;

namespace NVMSharp.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> ShowOpenFileDialogAsync(string title, string defaultExtension, params Tuple<string, string>[] extensions);
        Task<string> ShowSaveFileDialogAsync(string title, string defaultExtension, params Tuple<string, string>[] extensions);
    }
}
