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
