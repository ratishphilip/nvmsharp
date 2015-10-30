using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using NVMSharp.Common;
using NVMSharp.ViewModel;
using WPFSpark;

namespace NVMSharp.Views
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : SparkWindow
    {
        private const double GridRowHeight = 48;

        private CoreViewModel _coreViewModel;

        public EditWindow(CoreViewModel coreViewModel)
        {
            InitializeComponent();
            _coreViewModel = coreViewModel;
            this.DataContext = coreViewModel;
            UpdateInputGrid();
            UpdateSaveButtonState();
        }

        private void UpdateInputGrid()
        {
            switch (_coreViewModel.ModificationMode)
            {
                case ModificationModeType.NewValue:
                case ModificationModeType.EditValue:
                    inputGrid.RowDefinitions[0].Height = new GridLength(0);
                    break;
                default:
                    inputGrid.RowDefinitions[0].Height = new GridLength(GridRowHeight);
                    break;
            }
        }

        private void UpdateSaveButtonState()
        {
            var hasValidationError = !(String.IsNullOrWhiteSpace(_coreViewModel.ValidationMessage));
            switch (_coreViewModel.ModificationMode)
            {
                case ModificationModeType.NewValue:
                case ModificationModeType.EditValue:
                    saveBtn.IsEnabled = !hasValidationError && !(String.IsNullOrWhiteSpace(valueBox.Text));
                    break;
                default:
                    saveBtn.IsEnabled = !hasValidationError && !(String.IsNullOrWhiteSpace(nameBox.Text) || String.IsNullOrWhiteSpace(valueBox.Text));
                    break;
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnBrowseForFile(object sender, RoutedEventArgs e)
        {
            // Create a CommonOpenFileDialog to select files
            var cfd = new CommonOpenFileDialog
            {
                AllowNonFileSystemItems = true,
                EnsureReadOnly = true,
                EnsurePathExists = true,
                EnsureFileExists = true,
                Multiselect = false, // One file at a time
                Title = "Select File",
            };

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
                    // Append the deimited path in the value textbox 
                    valueBox.Text = String.IsNullOrWhiteSpace(valueBox.Text)
                        ? selectedObj.ParsingName
                        : $"{valueBox.Text};{selectedObj.ParsingName}";
                }
            }
        }

        private void OnBrowseForFolder(object sender, RoutedEventArgs e)
        {
            // Display a CommonOpenFileDialog to select only folders  
            var cfd = new CommonOpenFileDialog
            {
                EnsureReadOnly = true,
                IsFolderPicker = true,
                EnsurePathExists = true,
                AllowNonFileSystemItems = true
            };

            if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ShellContainer selectedObj = null;

                try
                {
                    // Try to get a valid selected item 
                    selectedObj = cfd.FileAsShellObject as ShellContainer;
                }
                catch
                {
                    //MessageBox.Show("Could not create a ShellObject from the selected item");
                }

                if (selectedObj != null)
                {
                    // Append the deimited path in the value textbox 
                    valueBox.Text = String.IsNullOrWhiteSpace(valueBox.Text)
                        ? selectedObj.ParsingName 
                        : $"{valueBox.Text};{selectedObj.ParsingName}";
                }
            }
        }

        private void HandleUserInput(object sender, TextChangedEventArgs e)
        {
            UpdateSaveButtonState();
        }
    }
}
