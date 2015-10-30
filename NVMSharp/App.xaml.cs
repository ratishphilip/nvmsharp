using System.Windows;
using NVMSharp.Services;

namespace NVMSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Inject the required services into the ServiceManager
            ServiceInjector.InjectServices();
        }
    }
}
