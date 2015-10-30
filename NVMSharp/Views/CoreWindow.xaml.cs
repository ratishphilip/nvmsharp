using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using NVMSharp.Common;
using NVMSharp.ViewModel;
using WPFSpark;

namespace NVMSharp.Views
{
    /// <summary>
    /// Interaction logic for CoreWindow.xaml
    /// </summary>
    public partial class CoreWindow : SparkWindow
    {
        private CoreViewModel _coreViewModel = null;

        public CoreWindow()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            WindowState = WindowState.Maximized;
            _coreViewModel = new CoreViewModel();
            DataContext = _coreViewModel;
            _coreViewModel.InitializeEnvironmentVariables();
            switch (_coreViewModel.InitResult)
            {
                case InitResultType.InitOk:
                    break;
                case InitResultType.AccessDenied:
                    MessageBox.Show("Cannot access Environment variables! Please restart NVM# as an Administrator.", "NVM#");
                    Application.Current.Shutdown();
                    break;
                case InitResultType.OtherError:
                    MessageBox.Show("NVM# encountered an error and needs to close!", "NVM#");
                    Application.Current.Shutdown();
                    break;
            }

            UserButton.IsChecked = true;
        }

        private void OnMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = (SplitViewMenu.Width == 48) ? 240 : 48;
        }

        private void OnViewUserVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = 48;
            _coreViewModel.CurrentAppMode = AppMode.User;
        }

        private void OnViewSystemVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = 48;
            _coreViewModel.CurrentAppMode = AppMode.System;
        }

        private void OnImportVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = 48;
            _coreViewModel.CurrentAppMode = AppMode.Import;
        }

        private void OnExportVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = 48;
            _coreViewModel.CurrentAppMode = AppMode.Export;
        }

        private void OnAboutButtonChecked(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = 48;
            _coreViewModel.CurrentAppMode = AppMode.About;


        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void OnImportSelectionChanged(object sender, RoutedEventArgs e)
        {
            _coreViewModel.HandleImportSelectionChanged();
        }

        private void OnExportSelectionChanged(object sender, RoutedEventArgs e)
        {
            _coreViewModel.HandleExportSelectionChanged();
        }

        private void OnImportSourceChanged(object sender, DataTransferEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            SetTooltip(textBlock);
        }

        private void OnTextSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            SetTooltip(textBlock);
        }

        private static void SetTooltip(TextBlock textBlock)
        {
            if (textBlock == null)
                return;

            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            var width = textBlock.DesiredSize.Width;

            ToolTipService.SetToolTip(textBlock, textBlock.ActualWidth < width ? textBlock.Text : null);
        }
    }
}
