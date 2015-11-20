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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        enum SplitViewMenuWidth
        {
            Narrow = 48,
            Wide = 240
        }

        private CoreViewModel _coreViewModel = null;

        public CoreWindow()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            // Show the window in normal state
            WindowState = WindowState.Normal;

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

            SizeChanged += (o, a) =>
            {
                switch (WindowState)
                {
                    case WindowState.Maximized:
                        SplitViewMenu.Width = (int)SplitViewMenuWidth.Wide;
                        break;

                    case WindowState.Normal:
                        SplitViewMenu.Width = (int)SplitViewMenuWidth.Narrow;
                        break;
                }

                RootGrid.ColumnDefinitions[0] = new ColumnDefinition { Width = new GridLength(SplitViewMenu.Width) };
                RootGrid.InvalidateVisual();
            };

            SplitViewMenu.SizeChanged += (o, a) =>
                                         {
                                             var isNarrowMenu = (int) SplitViewMenu.Width == (int) SplitViewMenuWidth.Narrow;
                                             ToolTipService.SetIsEnabled(UserButton, isNarrowMenu);
                                             ToolTipService.SetIsEnabled(SystemButton, isNarrowMenu);
                                             ToolTipService.SetIsEnabled(ImportButton, isNarrowMenu);
                                             ToolTipService.SetIsEnabled(ExportButton, isNarrowMenu);
                                             ToolTipService.SetIsEnabled(AboutButton, isNarrowMenu);
                                         };
        }

        private int GetColumnZeroWidth()
        {
            // determine how wide column zero should be based on window size
            // if window is maximized, column zero width is equal to current menu width.
            // if window is normal, column zero width is equal to narrow menu width
            return WindowState == WindowState.Maximized
                       ? (int) SplitViewMenu.Width
                       : (int) SplitViewMenuWidth.Narrow;

        }

        private void OnMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            // toggle menu width
            SplitViewMenu.Width = (int) SplitViewMenu.Width == (int) SplitViewMenuWidth.Narrow
                                      ? (int) SplitViewMenuWidth.Wide
                                      : (int) SplitViewMenuWidth.Narrow;

            // reset column width in the column definition based on window size
            RootGrid.ColumnDefinitions[0].Width = new GridLength(GetColumnZeroWidth());
        }

        private void OnViewUserVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = GetColumnZeroWidth();
            _coreViewModel.CurrentAppMode = AppMode.User;
        }

        private void OnViewSystemVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = GetColumnZeroWidth();
            _coreViewModel.CurrentAppMode = AppMode.System;
        }

        private void OnImportVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = GetColumnZeroWidth();
            _coreViewModel.CurrentAppMode = AppMode.Import;
        }

        private void OnExportVariables(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = GetColumnZeroWidth();
            _coreViewModel.CurrentAppMode = AppMode.Export;
        }

        private void OnAboutButtonChecked(object sender, RoutedEventArgs e)
        {
            SplitViewMenu.Width = GetColumnZeroWidth();
            _coreViewModel.CurrentAppMode = AppMode.About;
        }

        /// <summary>
        /// Allows the ScrollViewer to handle Mousewheel interactions
        /// </summary>
        /// <param name="sender">ScrollViewer</param>
        /// <param name="e">MouseWheelEventArgs object</param>
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

        /// <summary>
        /// Sets the tooltip to the TextBlock only if the entire text does not
        /// fit in the available space (i.e. the TextBlock shows Ellipis at the end)
        /// </summary>
        /// <param name="textBlock">TextBlock object</param>
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
