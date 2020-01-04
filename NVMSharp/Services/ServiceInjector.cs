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

// Based on Josh Smith's Article
// http://www.codeproject.com/Articles/70223/Using-a-Service-Locator-to-Work-with-MessageBoxes
//

using NVMSharp.Services.Interfaces;

namespace NVMSharp.Services
{
    public static class ServiceInjector
    {
        // Loads service objects into the ServiceManager on startup.
        public static void InjectServices()
        {
            var svcMgr = ServiceManager.Instance;
            // Dispatcher Service should be added first
            svcMgr.AddService<IDispatcherService>(new DispatcherService());
            svcMgr.AddService<IMessageService>(new MessageService(svcMgr.GetService<IDispatcherService>()));
            svcMgr.AddService<IModificationService>(new ModificationService(svcMgr.GetService<IDispatcherService>()));
            svcMgr.AddService<IFileService>(new FileService(svcMgr.GetService<IDispatcherService>()));
            svcMgr.AddService<IProgressService>(new ProgressService(svcMgr.GetService<IDispatcherService>()));
        }
    }
}
