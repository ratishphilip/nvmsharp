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
