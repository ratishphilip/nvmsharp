// Copyright (c) 2015 Ratish Philip 
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using NVMSharp.Services.Interfaces;

namespace NVMSharp.Services
{
    public class FileService : IFileService
    {
        private readonly IDispatcherService _dispatcherService;

        public FileService(IDispatcherService service)
        {
            _dispatcherService = service;
        }

        public Task<string> ShowOpenFileDialogAsync(string title, string defaultExtension, params Tuple<string, string>[] extensions)
        {
            var tcs = new TaskCompletionSource<string>();

            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                var result = string.Empty;

                // Create a CommonOpenFileDialog to select source file
                CommonOpenFileDialog cfd = new CommonOpenFileDialog
                {
                    AllowNonFileSystemItems = true,
                    EnsureReadOnly = true,
                    EnsurePathExists = true,
                    EnsureFileExists = true,
                    DefaultExtension = defaultExtension,
                    Multiselect = false, // One file at a time
                    Title = title ?? "Select File",
                };

                if ((extensions != null) && (extensions.Any()))
                {
                    foreach (var ext in extensions)
                    {
                        cfd.Filters.Add(new CommonFileDialogFilter(ext.Item1, ext.Item2));
                    }
                }

                if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ShellObject selectedObj = null;

                    try
                    {
                        // Try to get the selected item 
                        selectedObj = cfd.FileAsShellObject;
                    }
                    catch
                    {
                        //MessageBox.Show("Could not create a ShellObject from the selected item");
                    }

                    if (selectedObj != null)
                    {
                        // Get the file name
                        result = selectedObj.ParsingName;
                    }
                }

                tcs.SetResult(result);
            }));

            return tcs.Task;
        }

        public Task<string> ShowSaveFileDialogAsync(string title, string defaultExtension, params Tuple<string, string>[] extensions)
        {
            var tcs = new TaskCompletionSource<string>();

            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                var result = string.Empty;

                // Create a CommonSaveFileDialog to select destination file
                var sfd = new CommonSaveFileDialog
                {
                    EnsureReadOnly = true,
                    EnsurePathExists = true,
                    DefaultExtension = defaultExtension,
                    Title = title ?? "Save File",
                };

                if ((extensions != null) && (extensions.Any()))
                {
                    foreach (var ext in extensions)
                    {
                        sfd.Filters.Add(new CommonFileDialogFilter(ext.Item1, ext.Item2));
                    }
                }

                if (sfd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ShellObject selectedObj = null;

                    try
                    {
                        // Try to get the selected item 
                        selectedObj = sfd.FileAsShellObject;
                    }
                    catch
                    {
                        //MessageBox.Show("Could not create a ShellObject from the selected item");
                    }

                    if (selectedObj != null)
                    {
                        // Get the file name
                        result = selectedObj.ParsingName;
                    }
                }

                tcs.SetResult(result);
            }));

            return tcs.Task;
        }
    }
}
