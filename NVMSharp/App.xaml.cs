using System;
using System.Collections.Generic;
using System.Windows;
using NVMSharp.Services;
using Microsoft.Shell;

namespace NVMSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// 
    /// See: http://blogs.microsoft.co.il/arik/2010/05/28/wpf-single-instance-application/
    /// 
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string UniqueName = @"{C4F588CD-D652-4329-8FE3-8F73CA2B7E04}";
        private static App _application;

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(UniqueName))
            {
                ServiceInjector.InjectServices();
                _application = new App();

                _application.InitializeComponent();
                _application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // this is executed in the original instance

            // we get the arguments to the second instance and can send them to the existing instance if desired

            // here we bring the existing instance to the front
            _application.MainWindow.BringToFront();

            // handle command line arguments of second instance
            
            return true;
        }

        #endregion
    }
}