// Based on Josh Smith's Article
// http://www.codeproject.com/Articles/70223/Using-a-Service-Locator-to-Work-with-MessageBoxes
//

using System;
using System.Collections.Generic;

namespace NVMSharp.Services
{
    public class ServiceManager
    {
        public static readonly ServiceManager Instance = new ServiceManager();

        private ServiceManager()
        {
            _serviceMap = new Dictionary<Type, object>();
            _serviceMapLock = new object();
        }

        public void AddService<TServiceContract>(TServiceContract implementation)
            where TServiceContract : class
        {
            lock (_serviceMapLock)
            {
                _serviceMap[typeof(TServiceContract)] = implementation;
            }
        }

        public TServiceContract GetService<TServiceContract>()
            where TServiceContract : class
        {
            object service;
            lock (_serviceMapLock)
            {
                _serviceMap.TryGetValue(typeof(TServiceContract), out service);
            }
            return service as TServiceContract;
        }

        readonly Dictionary<Type, object> _serviceMap;
        readonly object _serviceMapLock;
    }
}