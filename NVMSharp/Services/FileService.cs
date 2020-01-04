// Copyright (c) 2020 Ratish Philip 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public Task<string> ShowOpenFileDialogAsync(string title, string defaultExtension, params Tuple<string, List<string>>[] extensions)
        {
            var tcs = new TaskCompletionSource<string>();

            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                var result = string.Empty;

                // Create a CommonOpenFileDialog to select source file
                OpenFileDialog cfd = new OpenFileDialog
                {
                    Multiselect = false, // One file at a time
                    Title = title ?? "Select File",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = defaultExtension,
                };


                if ((extensions != null) && (extensions.Any()))
                {
                    var sb = new StringBuilder();
                    var lastExt = extensions.Last();
                    foreach (var ext in extensions)
                    {
                        sb.Append($"{ext.Item1} | ");
                        var last = ext.Item2.Last();
                        foreach (var item in ext.Item2)
                        {
                            sb.Append(item);
                            if (item != last)
                            {
                                sb.Append(";");
                            }
                        }

                        if (ext != lastExt)
                        {
                            sb.Append("|");
                        }
                    }

                    cfd.Filter = sb.ToString();
                }

                if (cfd.ShowDialog() == DialogResult.OK)
                {
                    // Get the file name
                    result = cfd.FileName;
                }

                tcs.SetResult(result);
            }));

            return tcs.Task;
        }

        public Task<string> ShowSaveFileDialogAsync(string title, string defaultExtension, params Tuple<string, List<string>>[] extensions)
        {
            var tcs = new TaskCompletionSource<string>();

            _dispatcherService.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                var result = string.Empty;

                // Create a CommonSaveFileDialog to select destination file
                var sfd = new SaveFileDialog
                {
                    CheckPathExists = true,
                    DefaultExt = defaultExtension,
                    Title = title ?? "Save File",
                };

                if ((extensions != null) && (extensions.Any()))
                {
                    var sb = new StringBuilder();
                    var lastExt = extensions.Last();
                    foreach (var ext in extensions)
                    {
                        sb.Append($"{ext.Item1} | ");
                        var last = ext.Item2.Last();
                        foreach (var item in ext.Item2)
                        {
                            sb.Append(item);
                            if (item != last)
                            {
                                sb.Append(";");
                            }
                        }

                        if (ext != lastExt)
                        {
                            sb.Append("|");
                        }
                    }

                    sfd.Filter = sb.ToString();
                }

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Get the file name
                    result = sfd.FileName;
                }

                tcs.SetResult(result);
            }));

            return tcs.Task;
        }
    }
}
