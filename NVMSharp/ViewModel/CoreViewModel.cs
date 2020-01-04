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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Win32;
using NVMSharp.Common;
using NVMSharp.Services;
using NVMSharp.Services.Interfaces;

namespace NVMSharp.ViewModel
{
    /// <summary>
    /// The Core ViewModel for the entire App
    /// </summary>
    public class CoreViewModel : BindableBase
    {
        #region Delegates and Events

        #endregion

        #region Fields

        private Dictionary<string, EnVar> _userVars;
        private Dictionary<string, EnVar> _systemVars;
        private Dictionary<string, EnVar> _currentVars;
        private EnvironmentVariableTarget _target = EnvironmentVariableTarget.User;

        private IMessageService _messageService;
        private IModificationService _modificationService;
        private IFileService _fileService;
        private IProgressService _progressService;

        #endregion

        #region Properties

        #region CurrentAppMode

        private AppMode _currentAppMode = AppMode.None;

        /// <summary>
        /// Gets or sets the CurrentAppMode property. This observable property 
        /// indicates the current App Mode.
        /// </summary>
        public AppMode CurrentAppMode
        {
            get => _currentAppMode;
            set
            {
                SetProperty(ref _currentAppMode, value);
                OnAppModeChanged();
            }
        }

        #endregion

        #region AppModeTitle

        private string _appModeTitle = string.Empty;

        /// <summary>
        /// Gets or sets the AppModeTitle property. This observable property 
        /// indicates the title to be displayed for the current App Mode.
        /// </summary>
        public string AppModeTitle
        {
            get => _appModeTitle;
            set => SetProperty(ref _appModeTitle, value);
        }

        #endregion

        #region InitResult

        private InitResultType _initResult = InitResultType.None;

        /// <summary>
        /// Gets or sets the InitResult property. This observable property 
        /// indicates the result of the Initialization.
        /// </summary>
        public InitResultType InitResult
        {
            get => _initResult;
            set => SetProperty(ref _initResult, value);
        }

        #endregion

        #region DisplayKeys

        private ObservableCollection<string> _displayKeys;

        /// <summary>
        /// Gets or sets the DisplayKeys property. This observable property 
        /// indicates the list of Keys to be displayed.
        /// </summary>
        public ObservableCollection<string> DisplayKeys
        {
            get => _displayKeys;
            set => SetProperty(ref _displayKeys, value);
        }

        #endregion

        #region ActiveKey

        private string _activeKey = string.Empty;

        /// <summary>
        /// Gets or sets the ActiveKey property. This observable property 
        /// indicates the currently selected key.
        /// </summary>
        public string ActiveKey
        {
            get => _activeKey;
            set
            {
                SetProperty(ref _activeKey, value);
                UpdateDisplayValues();
            }
        }

        #endregion

        #region DisplayValues

        private ObservableCollection<string> _displayValues;

        /// <summary>
        /// Gets or sets the DisplayValues property. This observable property 
        /// indicates the list of Values to be displayed for the selected DisplayKey.
        /// </summary>
        public ObservableCollection<string> DisplayValues
        {
            get => _displayValues;
            set => SetProperty(ref _displayValues, value);
        }

        #endregion

        #region ActiveValue

        private string _activeValue = string.Empty;

        /// <summary>
        /// Gets or sets the ActiveValue property. This observable property 
        /// indicates the currently selected value.
        /// </summary>
        public string ActiveValue
        {
            get => _activeValue;
            set => SetProperty(ref _activeValue, value);
        }

        #endregion

        #region ModificationMode

        private ModificationModeType _modificationMode = ModificationModeType.None;

        /// <summary>
        /// Gets or sets the ModificationMode property. This observable property 
        /// indicates the type of modification (New/Edit) being done on either the 
        /// key or the value.
        /// </summary>
        public ModificationModeType ModificationMode
        {
            get => _modificationMode;
            set => SetProperty(ref _modificationMode, value);
        }

        #endregion

        #region ModificationTitle

        private string _modificationTitle = string.Empty;

        /// <summary>
        /// Gets or sets the ModificationTitle property. This observable property 
        /// indicates the title of the Modification Popup window.
        /// </summary>
        public string ModificationTitle
        {
            get => _modificationTitle;
            set => SetProperty(ref _modificationTitle, value);
        }

        #endregion

        #region ModifiedKey

        private string _modifiedKey = string.Empty;

        /// <summary>
        /// Gets or sets the ModifiedKey property. This observable property 
        /// indicates the key that has been modified/newly created.
        /// </summary>
        public string ModifiedKey
        {
            get => _modifiedKey;
            set
            {
                SetProperty(ref _modifiedKey, value);
                ValidateModification();
            }
        }

        #endregion

        #region ModifiedValue

        private string _modifiedValue = string.Empty;

        /// <summary>
        /// Gets or sets the ModifiedValue property. This observable property 
        /// indicates the KeyValue that has been modified/newly created.
        /// </summary>
        public string ModifiedValue
        {
            get => _modifiedValue;
            set
            {
                SetProperty(ref _modifiedValue, value);
                ValidateModification();
            }
        }

        #endregion

        #region ValidationMessage

        private string _validationMessage = string.Empty;

        /// <summary>
        /// Gets or sets the ValidationMessage property. This observable property 
        /// indicates the validation error message to be displayed to the user
        /// while creating/editing Variable Key or Variable Value.
        /// </summary>
        public string ValidationMessage
        {
            get => _validationMessage;
            set => SetProperty(ref _validationMessage, value);
        }

        #endregion

        #region HasUserImportVariables

        private bool _hasUserImportVariables;

        /// <summary>
        /// Gets or sets the HasUserImportVariables property. This observable property 
        /// indicates whether there are any User Environment Variables that can be
        /// imported.
        /// </summary>
        public bool HasUserImportVariables
        {
            get => _hasUserImportVariables;
            set => SetProperty(ref _hasUserImportVariables, value);
        }

        #endregion

        #region HasSystemImportVariables

        private bool _hasSystemImportVariables;

        /// <summary>
        /// Gets or sets the HasSystemImportVariables property. This observable property 
        /// indicates whether there are any System Environment Variables that can be
        /// imported.
        /// </summary>
        public bool HasSystemImportVariables
        {
            get => _hasSystemImportVariables;
            set => SetProperty(ref _hasSystemImportVariables, value);
        }

        #endregion

        #region HasUserExportVariables

        private bool _hasUserExportVariables;

        /// <summary>
        /// Gets or sets the HasUserExportVariables property. This observable property 
        /// indicates whether there are any User Environment Variables that can be
        /// exported.
        /// </summary>
        public bool HasUserExportVariables
        {
            get => _hasUserExportVariables;
            set => SetProperty(ref _hasUserExportVariables, value);
        }

        #endregion

        #region HasSystemExportVariables

        private bool _hasSystemExportVariables;

        /// <summary>
        /// Gets or sets the HasSystemExportVariables property. This observable property 
        /// indicates whether there are any System Environment Variables that can be
        /// exported.
        /// </summary>
        public bool HasSystemExportVariables
        {
            get => _hasSystemExportVariables;
            set => SetProperty(ref _hasSystemExportVariables, value);
        }

        #endregion

        #region UserImportVariables

        private ObservableCollection<ImportExportVM> _userImportVariables;

        /// <summary>
        /// Gets or sets the UserImportVariables property. This observable property 
        /// indicates the list of user environment variables that can be imported.
        /// </summary>
        public ObservableCollection<ImportExportVM> UserImportVariables
        {
            get => _userImportVariables;
            set => SetProperty(ref _userImportVariables, value);
        }

        #endregion

        #region SystemImportVariables

        private ObservableCollection<ImportExportVM> _systemImportVariables;

        /// <summary>
        /// Gets or sets the SystemImportVariables property. This observable property 
        /// indicates the list of system environment variables that can be imported.
        /// </summary>
        public ObservableCollection<ImportExportVM> SystemImportVariables
        {
            get => _systemImportVariables;
            set => SetProperty(ref _systemImportVariables, value);
        }

        #endregion

        #region UserExportVariables

        private ObservableCollection<ImportExportVM> _userExportVariables;

        /// <summary>
        /// Gets or sets the UserExportVariables property. This observable property 
        /// indicates the list of user environment variables that can be exported.
        /// </summary>
        public ObservableCollection<ImportExportVM> UserExportVariables
        {
            get => _userExportVariables;
            set => SetProperty(ref _userExportVariables, value);
        }

        #endregion

        #region SystemExportVariables

        private ObservableCollection<ImportExportVM> _systemExportVariables;

        /// <summary>
        /// Gets or sets the SystemExportVariables property. This observable property 
        /// indicates the list of system environment variables that can be exported.
        /// </summary>
        public ObservableCollection<ImportExportVM> SystemExportVariables
        {
            get => _systemExportVariables;
            set => SetProperty(ref _systemExportVariables, value);
        }

        #endregion

        #region ImportSource

        private string _importSource = string.Empty;

        /// <summary>
        /// Gets or sets the ImportSource property. This observable property 
        /// indicates the name of the file from which the environment variables
        /// have to be imported.
        /// </summary>
        public string ImportSource
        {
            get => _importSource;
            set => this.SetProperty(ref this._importSource, value);
        }

        #endregion

        #region InitImportStatus

        private string _initImportStatus = string.Empty;

        /// <summary>
        /// Gets or sets the InitImportStatus property. This observable property 
        /// indicates the text to be displayed based on the result of the parsing
        /// of the import file.
        /// </summary>
        public string InitImportStatus
        {
            get => _initImportStatus;
            set => this.SetProperty(ref this._initImportStatus, value);
        }

        #endregion

        #region HasInitImportStatus

        private bool _hasInitImportStatus = false;

        /// <summary>
        /// Gets or sets the HasInitImportStatus property. This observable property 
        /// indicates whether a valid import status is available.
        /// </summary>
        public bool HasInitImportStatus
        {
            get => _hasInitImportStatus;
            set => this.SetProperty(ref this._hasInitImportStatus, value);
        }

        #endregion

        #region HasImportConflicts

        private bool _hasImportConflicts = false;

        /// <summary>
        /// Gets or sets the HasImportConflicts property. This observable property 
        /// indicates whether the environment variables being imported are already
        /// present in the current set of environment variables.
        /// </summary>
        public bool HasImportConflicts
        {
            get => _hasImportConflicts;
            set => this.SetProperty(ref this._hasImportConflicts, value);
        }

        #endregion

        #region ImportStatus

        private string _importStatus = string.Empty;

        /// <summary>
        /// Gets or sets the ImportStatus property. This observable property 
        /// indicates the status of the conflicts / result of import operation.
        /// </summary>
        public string ImportStatus
        {
            get => _importStatus;
            set => this.SetProperty(ref this._importStatus, value);
        }

        #endregion

        #region InitExportStatus

        private string _initExportStatus = string.Empty;

        /// <summary>
        /// Gets or sets the InitExportStatus property. This observable property 
        /// indicates the text to be displayed based on the availability of the
        /// environment variables.
        /// </summary>
        public string InitExportStatus
        {
            get => _initExportStatus;
            set => SetProperty(ref _initExportStatus, value);
        }

        #endregion

        #region ExportStatus

        private string _exportStatus = string.Empty;

        /// <summary>
        /// Gets or sets the ExportStatus property. This observable property 
        /// indicates the text to be displayed based on the result of the 
        /// export operation.
        /// </summary>
        public string ExportStatus
        {
            get => _exportStatus;
            set => this.SetProperty(ref this._exportStatus, value);
        }

        #endregion

        #region IsProgressVisible

        private bool _isProgressVisible = false;

        /// <summary>
        /// Gets or sets the IsProgressVisible property. This observable property 
        /// indicates whether the indeterminate progress bar (used to indicate that
        /// environment variables are being added/modified/deleted in the registry) 
        /// is visible or not. This will not be displayed when the App Mode is ImportAsync
        /// or ExportAsync or About.
        /// </summary>
        public bool IsProgressVisible
        {
            get => _isProgressVisible;
            set => this.SetProperty(ref this._isProgressVisible, value);
        }

        #endregion

        #endregion

        #region Commands

        // For Keys

        /// <summary>
        /// Gets or sets the NewKeyCommand property. This command
        /// is for creating a new Environment Variable.
        /// </summary>
        public RelayCommand NewKeyCommand { get; set; }

        /// <summary>
        /// Gets or sets the EditKeyCommand property. This command
        /// is for editing the selected Environment Variable.
        /// </summary>
        public RelayCommand EditKeyCommand { get; set; }

        /// <summary>
        /// Gets or sets the CopyKeyCommand property. This command
        /// is for copying the Name of the environment variable.
        /// </summary>
        public RelayCommand CopyKeyCommand { get; set; }

        /// <summary>
        /// Gets or sets the DeleteKeyCommand property. This command
        /// is for deleting the selected Key.
        /// </summary>
        public RelayCommand DeleteKeyCommand { get; set; }


        // For KeyValues

        /// <summary>
        /// Gets or sets the NewValueCommand property. This command
        /// is for adding a new value to the ActiveKey.
        /// </summary>
        public RelayCommand NewValueCommand { get; set; }

        /// <summary>
        /// Gets or sets the EditValueCommand property. This command
        /// is for editing an existing value of the ActiveKey.
        /// </summary>
        public RelayCommand EditValueCommand { get; set; }

        /// <summary>
        /// Gets or sets the CopyValueCommand property. This command
        /// is for copying the selected value.
        /// </summary>
        public RelayCommand CopyValueCommand { get; set; }

        /// <summary>
        /// Gets or sets the MoveToTopCommand property. This command
        /// is for moving the selected KeyValue to the top of the KeyValues list.
        /// </summary>
        public RelayCommand MoveToTopCommand { get; set; }

        /// <summary>
        /// Gets or sets the MoveUpCommand property. This command
        /// is for moving the selected KeyValue on row up in the KeyValue list.
        /// </summary>
        public RelayCommand MoveUpCommand { get; set; }

        /// <summary>
        /// Gets or sets the MoveDownCommand property. This command
        /// is for moving the selected KeyValue one row down in the KeyValues list.
        /// </summary>
        public RelayCommand MoveDownCommand { get; set; }

        /// <summary>
        /// Gets or sets the MoveToBottomCommand property. This command
        /// is for moving the selected KeyValue to the bottom of the KeyValues list.
        /// </summary>
        public RelayCommand MoveToBottomCommand { get; set; }

        /// <summary>
        /// Gets or sets the ReverseValuesCommand property. This command
        /// is for reversing the order of the KeyValues in the KeyValues list.
        /// </summary>
        public RelayCommand ReverseValuesCommand { get; set; }

        /// <summary>
        /// Gets or sets the DeleteValueCommand property. This command
        /// is for deleting the selected KeyValue from the KeyValues list.
        /// </summary>
        public RelayCommand DeleteValueCommand { get; set; }

        // For ImportAsync

        /// <summary>
        /// Gets or sets the InitImportCommand property. This command
        /// is for selecting the file from which the environment variables
        /// have to be imported.
        /// </summary>
        public RelayCommand InitImportCommand { get; set; }

        /// <summary>
        /// Gets or sets the ImportCommand property. This command
        /// is for Importing Environment Variables from a file.
        /// </summary>
        public RelayCommand ImportCommand { get; set; }

        /// <summary>
        /// Gets or sets the MergeImportCommand property. This command
        /// is for merging the conflicting environment variable values.
        /// </summary>
        public RelayCommand MergeImportCommand { get; set; }

        /// <summary>
        /// Gets or sets the ImportDiscardCommand property. This command
        /// is for importing only the non-conflicting environment variables.
        /// </summary>
        public RelayCommand ImportDiscardCommand { get; set; }

        /// <summary>
        /// Gets or sets the ClearImportCommand property. This command
        /// is for clearing the parsed data shown when the import file is 
        /// successfully parsed.
        /// </summary>
        public RelayCommand ClearImportCommand { get; set; }

        // For ExportAsync

        /// <summary>
        /// Gets or sets the ExportCommand property. This command
        /// is for Exporting Environment variables to a file.
        /// </summary>
        public RelayCommand ExportCommand { get; set; }

        #endregion

        #region Construction / Initialization

        public CoreViewModel()
        {
            _currentAppMode = AppMode.None;
            _initResult = InitResultType.None;
            _userVars = new Dictionary<string, EnVar>();
            _systemVars = new Dictionary<string, EnVar>();
            _modificationMode = ModificationModeType.None;
            _modificationTitle = string.Empty;
            _modifiedKey = string.Empty;
            _modifiedValue = string.Empty;
            _validationMessage = string.Empty;
            _hasUserImportVariables = false;
            _hasSystemImportVariables = false;
            _hasUserExportVariables = false;
            _hasSystemExportVariables = false;
            _hasInitImportStatus = false;
            _hasImportConflicts = false;

            // Get the services
            _messageService = ServiceManager.Instance.GetService<IMessageService>();
            _modificationService = ServiceManager.Instance.GetService<IModificationService>();
            _fileService = ServiceManager.Instance.GetService<IFileService>();
            _progressService = ServiceManager.Instance.GetService<IProgressService>();

            // InitializeEnvironmentVariables the commands
            InitializeCommands();
        }

        #endregion

        #region APIs

        public void InitializeEnvironmentVariables()
        {
            InitResult = InitResultType.None;

            // User Environment Variables
            try
            {
                _userVars.Clear();

                // Get the User Environment Variables
                var theKey = Registry.CurrentUser;
                theKey = theKey.OpenSubKey(Constants.USER_ENV_SUBKEY, true);

                // GetValueNames will return the names of all the keys within the Enviroment Key
                foreach (var key in theKey.GetValueNames())
                {
                    var keyValue = (theKey.GetValue(key)).ToString();

                    _userVars[key] = new EnVar(key, keyValue);
                }

                theKey.Close();
            }
            catch (SecurityException)
            {
                // MessageBox.Show(e.Message + " Please restart application in Administrator privileges.", "NVM");
                InitResult = InitResultType.AccessDenied;
            }
            catch (Exception)
            {
                InitResult = InitResultType.OtherError;
            }

            // System Environment Variables
            try
            {
                _systemVars.Clear();

                // Get the System Environment Variables
                var theKey = Registry.LocalMachine;
                theKey = theKey.OpenSubKey(Constants.SYSTEM_ENV_SUBKEY, true);

                // GetValueNames will return the names of all the keys within the Enviroment Key
                foreach (var key in theKey.GetValueNames())
                {
                    var keyValue = (theKey.GetValue(key)).ToString();

                    _systemVars[key] = new EnVar(key, keyValue);
                }

                theKey.Close();
            }
            catch (SecurityException)
            {
                // Unable to access Environment Variables
                InitResult = InitResultType.AccessDenied;
            }
            catch (Exception e)
            {
                // Some other unexpected error has occured. Log it!
                EventLog.WriteEntry("NVM#", "Initialization Error: " + Environment.NewLine + e, EventLogEntryType.Error);
                InitResult = InitResultType.OtherError;
            }

            if (InitResult == InitResultType.None)
            {
                // Environment Variables accessed successfully
                InitResult = InitResultType.InitOk;
            }
        }

        public void HandleImportSelectionChanged()
        {
            CheckForImportConflicts();
            ImportCommand.RaiseCanExecuteChanged();
            MergeImportCommand.RaiseCanExecuteChanged();
            ClearImportCommand.RaiseCanExecuteChanged();
        }

        public void HandleExportSelectionChanged()
        {
            ExportCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Event Handlers

        private async void OnNewKey()
        {
            // Init modification status
            ModificationMode = ModificationModeType.NewKey;
            ModificationTitle = (_currentAppMode == AppMode.User)
                ? Constants.MODE_NEW_USER_KEY
                : Constants.MODE_NEW_SYSTEM_KEY;
            ModifiedKey = string.Empty;
            ModifiedValue = string.Empty;
            ValidationMessage = string.Empty;
            // Show modification dialog
            var result = await _modificationService.InitModification(this);
            if (result != true)
                return;

            IsProgressVisible = true;
            // Create the new Environment variable 
            var newVar = new EnVar(_modifiedKey, _modifiedValue);

            _currentVars[newVar.Key] = newVar;
            var keyList = new List<string>(_currentVars.Keys);
            // Sort the keys
            keyList.Sort();
            DisplayKeys = new ObservableCollection<string>(keyList);
            ActiveKey = _modifiedKey;

            // Commit the new Environment variable
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(newVar, _target);
            }));
            IsProgressVisible = false;
        }

        private async void OnEditKey()
        {
            // Init modification status
            ModificationMode = ModificationModeType.EditKey;
            ModificationTitle = (_currentAppMode == AppMode.User)
                ? Constants.MODE_EDIT_USER_KEY
                : Constants.MODE_EDIT_SYSTEM_KEY;
            ModifiedKey = ActiveKey;
            ModifiedValue = _currentVars[ActiveKey].Data;
            ValidationMessage = string.Empty;
            // Show modification dialog
            var result = await _modificationService.InitModification(this);
            if (result != true)
                return;

            // NOTE: Each Environment Variable should have a unique key and
            // since the Key of the Environment variable is being edited,
            // it should result in the creation of a new EnVar object and
            // the deletion of the old EnVar object

            IsProgressVisible = true;
            // Create the new Environment variable 
            var newVar = new EnVar(_modifiedKey, _modifiedValue);
            var keyList = new List<string>();
            var delVar = _currentVars[ActiveKey];

            _currentVars.Remove(ActiveKey);
            _currentVars[newVar.Key] = newVar;
            keyList.AddRange(_currentVars.Keys);
            // Sort the keys
            keyList.Sort();
            DisplayKeys = new ObservableCollection<string>(keyList);
            ActiveKey = _modifiedKey;

            await Task.Run(new Action(() =>
            {
                // Remove the old Environment variable
                RegistryManager.DeleteEnvironmentVariable(delVar, _target);
                // Commit the new Environment variable
                RegistryManager.SaveEnvironmentVariable(newVar, _target);
            }));
            IsProgressVisible = false;
        }

        private void OnCopyKey()
        {
            Clipboard.SetText(ActiveKey);
            _messageService.ShowAsync($"Copied '{ActiveKey}' to clipboard.", "Copy Environment Variable",
                MessageBoxButton.OK, MessageBoxImage.None);
        }

        private async void OnDeleteKey()
        {
            var result = await _messageService.ShowAsync(Constants.CONFIRM_DELETE_KEY, Constants.CONFIRM_DELETE_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            IsProgressVisible = true;
            var delVar = _currentVars[ActiveKey];

            _currentVars.Remove(ActiveKey);
            var keyList = new List<string>(_currentVars.Keys);
            // Sort the keys
            keyList.Sort();
            DisplayKeys = new ObservableCollection<string>(keyList);
            ActiveKey = DisplayKeys.Count > 0 ? DisplayKeys[0] : null;

            // Remove the Environment variable
            await Task.Run(new Action(() =>
            {
                RegistryManager.DeleteEnvironmentVariable(delVar, _target);
            }));

            IsProgressVisible = false;
        }

        private async void OnNewValue()
        {
            // Init modification status
            ModificationMode = ModificationModeType.NewValue;
            ModificationTitle = Constants.MODE_NEW_VALUE;
            ModifiedKey = ActiveKey;
            ModifiedValue = string.Empty;
            ValidationMessage = string.Empty;
            // Show modification dialog
            var result = await _modificationService.InitModification(this);
            if (result != true)
                return;

            IsProgressVisible = true;
            _currentVars[ActiveKey].AddValue(ModifiedValue);

            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            ActiveValue = ModifiedValue;

            // Commit Environment Variable to Registry
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
            }));
            IsProgressVisible = false;
        }

        private async void OnEditValue()
        {
            // Init modification status
            ModificationMode = ModificationModeType.EditValue;
            ModificationTitle = Constants.MODE_EDIT_VALUE;
            ModifiedKey = ActiveKey;
            ModifiedValue = ActiveValue;
            ValidationMessage = string.Empty;
            // Show modification dialog
            var result = await _modificationService.InitModification(this);
            if (result != true)
                return;

            IsProgressVisible = true;
            _currentVars[ActiveKey].UpdateValue(ActiveValue, ModifiedValue);

            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            ActiveValue = ModifiedValue;

            // Commit Environment Variable to Registry
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
            }));
            IsProgressVisible = false;
        }

        private void OnCopyValue()
        {
            Clipboard.SetText(ActiveValue);
            _messageService.ShowAsync($"Copied '{ActiveValue}' to clipboard.", "Copy Value",
                MessageBoxButton.OK, MessageBoxImage.None);
        }

        private async void OnMoveToTop()
        {
            IsProgressVisible = true;
            _currentVars[ActiveKey].MoveToTop(ActiveValue);
            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            OnPropertyChanged(nameof(ActiveValue));

            // Commit Environment Variable to Registry
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
            }));
            IsProgressVisible = false;
        }

        private async void OnMoveUp()
        {
            IsProgressVisible = true;
            _currentVars[ActiveKey].MoveUp(ActiveValue);
            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            OnPropertyChanged(nameof(ActiveValue));

            // Commit Environment Variable to Registry
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
            }));
            IsProgressVisible = false;
        }

        private async void OnMoveDown()
        {
            IsProgressVisible = true;
            _currentVars[ActiveKey].MoveDown(ActiveValue);
            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            OnPropertyChanged(nameof(ActiveValue));

            // Commit Environment Variable to Registry
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
            }));
            IsProgressVisible = false;
        }

        private async void OnMoveToBottom()
        {
            IsProgressVisible = true;
            _currentVars[ActiveKey].MoveToBottom(ActiveValue);
            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            OnPropertyChanged(nameof(ActiveValue));

            // Commit Environment Variable to Registry
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
            }));
            IsProgressVisible = false;
        }

        private async void OnReverseValues()
        {
            IsProgressVisible = true;
            _currentVars[ActiveKey].ReverseValues();
            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            OnPropertyChanged(nameof(ActiveValue));

            // Commit Environment Variable to Registry
            await Task.Run(new Action(() =>
            {
                RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
            }));
            IsProgressVisible = false;
        }

        private async void OnDeleteValue()
        {
            // Check if the Environment Variable has only one value
            var hasSingleValue = (_currentVars[ActiveKey].Values.Count == 1);
            var result = await _messageService.ShowAsync(hasSingleValue ? Constants.CONFIRM_DELETE_VALUE_AND_KEY : Constants.CONFIRM_DELETE_VALUE,
                Constants.CONFIRM_DELETE_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            IsProgressVisible = true;
            if (hasSingleValue)
            {
                // Delete the Environment variable as the variable cannot exist without any value
                var delVar = _currentVars[ActiveKey];
                _currentVars.Remove(ActiveKey);
                var keyList = new List<string>(_currentVars.Keys);
                // Sort the keys
                keyList.Sort();
                DisplayKeys = new ObservableCollection<string>(keyList);
                ActiveKey = DisplayKeys.Count > 0 ? DisplayKeys[0] : null;

                await Task.Run(new Action(() =>
                {
                    RegistryManager.DeleteEnvironmentVariable(delVar, _target);
                }));
            }
            else
            {
                // Delete the value
                _currentVars[ActiveKey].DeleteValue(ActiveValue);
                DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
                ActiveValue = DisplayValues.Count > 0 ? DisplayValues[0] : null;

                // Commit Environment Variable to Registry
                await Task.Run(new Action(() =>
                {
                    RegistryManager.SaveEnvironmentVariable(_currentVars[ActiveKey], _target);
                }));
            }

            IsProgressVisible = false;
        }

        private async void OnInitImport()
        {
            var fileName =
                await _fileService.ShowOpenFileDialogAsync("Select Source File", "xml",
                        Tuple.Create("XML Files (*.xml)", new List<string>() {"*.xml"}));

            if (String.IsNullOrWhiteSpace(fileName))
                return;

            IsProgressVisible = true;
            await InitImportAsync(fileName);
            IsProgressVisible = false;
        }

        /// <summary>
        /// Imports all the selected environment variables. If there are conflicts,
        /// it will replace the old environment variables with new ones.
        /// </summary>
        private async void OnImport()
        {
            IsProgressVisible = true;
            _progressService.ShowProgress("Importing...");
            await ImportAsync();
            _progressService.HideProgress();
            IsProgressVisible = false;
        }

        /// <summary>
        /// Imports all the selected environment variables. If there are conflicts,
        /// it will merge the values of the old environment variables with the values
        /// of the imported environment variables.
        /// </summary>
        private async void OnMergeImport()
        {
            IsProgressVisible = true;
            _progressService.ShowProgress("Importing...");
            await MergeImportAsync();
            _progressService.HideProgress();
            IsProgressVisible = false;
        }

        /// <summary>
        /// Imports only those selected environment variables
        /// which have no conflicts with existing environment variables
        /// </summary>
        private async void OnImportDiscard()
        {
            IsProgressVisible = true;
            _progressService.ShowProgress("Importing...");
            await ImportDiscardAsync();
            _progressService.HideProgress();
            IsProgressVisible = false;
        }

        private void OnClearImport()
        {
            IsProgressVisible = true;
            UserImportVariables.Clear();
            SystemImportVariables.Clear();
            HasUserImportVariables = false;
            HasSystemImportVariables = false;
            ImportSource = string.Empty;
            InitImportStatus = string.Empty;
            HasInitImportStatus = false;
            ImportStatus = string.Empty;
            HasImportConflicts = false;
            ImportCommand.RaiseCanExecuteChanged();
            ClearImportCommand.RaiseCanExecuteChanged();
            IsProgressVisible = false;
        }

        private async void OnExport()
        {
            var fileName =
                await _fileService.ShowSaveFileDialogAsync("Save Environment Variable(s) As", ".xml",
                        Tuple.Create("XML Files", new List<string>() {"*.xml"}));

            if (String.IsNullOrWhiteSpace(fileName))
                return;

            IsProgressVisible = true;
            await ExportAsync(fileName);
            IsProgressVisible = false;
        }

        #endregion

        #region Helpers

        private void InitializeCommands()
        {
            // Key Commands
            NewKeyCommand = CreateCommand(OnNewKey, () => true, null);
            EditKeyCommand = CreateCommand(OnEditKey, () => (ActiveKey != null), nameof(ActiveKey));
            CopyKeyCommand = CreateCommand(OnCopyKey, () => (ActiveKey != null), nameof(ActiveKey));
            DeleteKeyCommand = CreateCommand(OnDeleteKey, () => (ActiveKey != null), nameof(ActiveKey));
            // Value Commands
            NewValueCommand = CreateCommand(OnNewValue, () => (ActiveKey != null), nameof(ActiveKey));
            EditValueCommand = CreateCommand(OnEditValue, () => (ActiveValue != null), nameof(ActiveValue));
            CopyValueCommand = CreateCommand(OnCopyValue, () => (ActiveValue != null), nameof(ActiveValue));
            MoveToTopCommand = CreateCommand(OnMoveToTop, CanMoveUp, nameof(ActiveValue));
            MoveUpCommand = CreateCommand(OnMoveUp, CanMoveUp, nameof(ActiveValue));
            MoveDownCommand = CreateCommand(OnMoveDown, CanMoveDown, nameof(ActiveValue));
            MoveToBottomCommand = CreateCommand(OnMoveToBottom, CanMoveDown, nameof(ActiveValue));
            ReverseValuesCommand = CreateCommand(OnReverseValues, CanReverseValues, nameof(ActiveKey));
            DeleteValueCommand = CreateCommand(OnDeleteValue, () => (ActiveValue != null), nameof(ActiveValue));
            // ImportAsync Commands
            InitImportCommand = CreateCommand(OnInitImport, () => true, null);
            ImportCommand = CreateCommand(OnImport, CanImport, null);
            MergeImportCommand = CreateCommand(OnMergeImport, () => HasImportConflicts, nameof(HasImportConflicts));
            ImportDiscardCommand = CreateCommand(OnImportDiscard, () => HasImportConflicts, nameof(HasImportConflicts));
            ClearImportCommand = CreateCommand(OnClearImport, CanClearImport, null);
            // ExportAsync Commands
            ExportCommand = CreateCommand(OnExport, CanExport, null);
        }

        private bool CanMoveUp()
        {
            var result = (!String.IsNullOrWhiteSpace(ActiveKey)) && (ActiveValue != null);

            if (result)
            {
                result = _currentVars[ActiveKey].Values.IndexOf(ActiveValue) != 0;
            }

            return result;
        }

        private bool CanMoveDown()
        {
            var result = (!String.IsNullOrWhiteSpace(ActiveKey)) && (ActiveValue != null);

            if (result)
            {
                result = _currentVars[ActiveKey].Values.IndexOf(ActiveValue) != _currentVars[ActiveKey].Values.Count - 1;
            }

            return result;
        }

        private bool CanReverseValues()
        {
            var result = (!String.IsNullOrWhiteSpace(ActiveKey)) && (ActiveValue != null);

            if (result)
            {
                result = _currentVars[ActiveKey].Values.Count > 1;
            }

            return result;
        }

        /// <summary>
        /// Updates the DisplayKeys and the ActiveKey based on the AppMode
        /// </summary>
        private async void OnAppModeChanged()
        {
            AppModeTitle = _currentAppMode.ToString();

            var keyList = new List<string>();

            switch (_currentAppMode)
            {
                case AppMode.User:
                    _currentVars = _userVars;
                    _target = EnvironmentVariableTarget.User;
                    keyList.AddRange(_userVars.Keys);
                    // Sort the keys
                    keyList.Sort();
                    DisplayKeys = new ObservableCollection<string>(keyList);
                    ActiveKey = DisplayKeys.Count > 0 ? DisplayKeys[0] : null;
                    break;
                case AppMode.System:
                    _currentVars = _systemVars;
                    _target = EnvironmentVariableTarget.Machine;
                    keyList.AddRange(_systemVars.Keys);
                    // Sort the keys
                    keyList.Sort();
                    DisplayKeys = new ObservableCollection<string>(keyList);
                    ActiveKey = DisplayKeys.Count > 0 ? DisplayKeys[0] : null;
                    break;
                case AppMode.Import:
                    // Nothing to do as of now
                    break;
                case AppMode.Export:
                    await InitExportAsync();
                    break;
            }
        }

        /// <summary>
        /// Updates the DisplayValues based on the Active Key
        /// </summary>
        private void UpdateDisplayValues()
        {
            if (string.IsNullOrWhiteSpace(ActiveKey))
                return;

            DisplayValues = new ObservableCollection<string>(_currentVars?[ActiveKey]?.Values);
            ActiveValue = DisplayValues.Count > 0 ? DisplayValues[0] : null;
        }

        private void ValidateModification()
        {
            switch (ModificationMode)
            {
                case ModificationModeType.NewKey:
                    ValidationMessage = ValidateKey(false);
                    break;
                case ModificationModeType.EditKey:
                    ValidationMessage = ValidateKey(true);
                    break;
                case ModificationModeType.NewValue:
                    ValidationMessage = ValidateValue(false);
                    break;
                case ModificationModeType.EditValue:
                    ValidationMessage = ValidateValue(true);
                    break;
                default:
                    ValidationMessage = string.Empty;
                    break;
            }
        }

        private string ValidateKey(bool exceptActiveKey)
        {
            if (String.IsNullOrWhiteSpace(_modifiedKey))
                return string.Empty;

            var keys = exceptActiveKey ? DisplayKeys.Except(new List<string> { ActiveKey }) : DisplayKeys;

            var validationMsg = string.Empty;

            if (keys.Contains(_modifiedKey))
            {
                switch (CurrentAppMode)
                {
                    case AppMode.User:
                        validationMsg = Constants.VALIDATION_USER_KEY_ERROR;
                        break;
                    case AppMode.System:
                        validationMsg = Constants.VALIDATION_SYSTEM_KEY_ERROR;
                        break;
                }
            }

            return validationMsg;
        }

        private string ValidateValue(bool exceptActiveValue)
        {
            if (String.IsNullOrWhiteSpace(_modifiedValue))
                return string.Empty;

            var values = exceptActiveValue ? DisplayValues.Except(new List<string> { ActiveValue }) : DisplayValues;

            return (values.Contains(_modifiedValue)) ? Constants.VALIDATION_VALUE_ERROR : string.Empty;
        }

        private bool CanImport()
        {
            if ((UserImportVariables == null) || (SystemImportVariables == null))
                return false;

            return UserImportVariables.Any(v => (v.IsSelected == true)) ||
                   SystemImportVariables.Any(v => (v.IsSelected == true));
        }

        private bool CanClearImport()
        {
            if ((UserImportVariables == null) || (SystemImportVariables == null))
                return false;

            return UserImportVariables.Any() || SystemImportVariables.Any();
        }

        private void CheckForImportConflicts()
        {
            if ((UserImportVariables == null) ||
                (SystemImportVariables == null))
            {
                HasImportConflicts = false;
                ImportStatus = string.Empty;
                return;
            }

            HasImportConflicts = (UserImportVariables.Any(v => ((v.IsSelected == true) && v.HasConflict)) ||
                                  SystemImportVariables.Any(v => ((v.IsSelected == true) && v.HasConflict)));

            if (HasImportConflicts)
            {
                var count = UserImportVariables.Count(v => ((v.IsSelected == true) && v.HasConflict)) +
                                        SystemImportVariables.Count(v => ((v.IsSelected == true) && v.HasConflict));

                ImportStatus = count > 1 ? $"{count} import conflicts" : "1 import conflict";
            }
            else
            {
                ImportStatus = "No import conflicts";
            }
        }

        private async Task InitImportAsync(string fileName)
        {
            await Task.Run(async () =>
            {
                var loaded = true;
                var userVars = new List<ImportExportVM>();
                var sysVars = new List<ImportExportVM>();
                ImportSource = fileName;
                ImportStatus = string.Empty;

                // Parse the file for Environment variables
                try
                {
                    var xDoc = XDocument.Load(fileName);
                    var xRoot = xDoc.Root;
                    var xUser = xRoot?.Element(Constants.X_USER);
                    if (xUser != null)
                    {
                        userVars.AddRange(from xVar in xUser.Elements(Constants.X_VARIABLE)
                                          let xName = xVar?.Element(Constants.X_NAME)?.Value
                                          let xValue = xVar?.Element(Constants.X_VALUE)?.Value
                                          where !(String.IsNullOrWhiteSpace(xName) || String.IsNullOrWhiteSpace(xValue))
                                          select new ImportExportVM
                                          {
                                              Key = xName,
                                              Data = xValue,
                                              IsSelected = true,
                                              HasConflict = (_userVars.ContainsKey(xName) && (_userVars[xName].Data != xValue))
                                          });
                    }

                    var xSystem = xRoot?.Element(Constants.X_SYSTEM);
                    if (xSystem != null)
                    {
                        sysVars.AddRange(from xVar in xSystem.Elements(Constants.X_VARIABLE)
                                         let xName = xVar?.Element(Constants.X_NAME)?.Value
                                         let xValue = xVar?.Element(Constants.X_VALUE)?.Value
                                         where !(String.IsNullOrWhiteSpace(xName) || String.IsNullOrWhiteSpace(xValue))
                                         select new ImportExportVM
                                         {
                                             Key = xName,
                                             Data = xValue,
                                             IsSelected = true,
                                             HasConflict = (_systemVars.ContainsKey(xName) && (_systemVars[xName].Data != xValue))
                                         });
                    }
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry("NVM#", e.ToString(), EventLogEntryType.Error);
                    await _messageService.ShowAsync(Constants.INIT_IMPORT_ERROR, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    loaded = false;
                }

                if (loaded)
                {
                    userVars.Sort();
                    UserImportVariables = new ObservableCollection<ImportExportVM>(userVars);
                    HasUserImportVariables = userVars.Count > 0;

                    sysVars.Sort();
                    SystemImportVariables = new ObservableCollection<ImportExportVM>(sysVars);
                    HasSystemImportVariables = (sysVars.Count > 0);

                    InitImportStatus = (HasUserImportVariables || HasSystemImportVariables)
                        ? Constants.SELECT_ENVAR_IMPORT
                        : Constants.NO_ENVAR_AVAILABLE_FOR_IMPORT;

                    HasInitImportStatus = true;
                }
            });
        }

        /// <summary>
        /// Imports all the selected environment variables. If there are conflicts,
        /// it will replace the old environment variables with new ones.
        /// </summary>
        private async Task ImportAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var importCount = 0;
                    // Save the imported User environment variables
                    foreach (var enVar in UserImportVariables.Where(v => (v.IsSelected == true))
                        .Select(v => new EnVar(v.Key, v.Data)))
                    {
                        RegistryManager.SaveEnvironmentVariable(enVar, EnvironmentVariableTarget.User);
                        importCount++;
                    }

                    // Save the imported System environment variables
                    foreach (var enVar in SystemImportVariables.Where(v => (v.IsSelected == true))
                        .Select(v => new EnVar(v.Key, v.Data)))
                    {
                        RegistryManager.SaveEnvironmentVariable(enVar, EnvironmentVariableTarget.Machine);
                        importCount++;
                    }

                    // Reload the environment variables
                    InitializeEnvironmentVariables();

                    ImportStatus = importCount > 1
                        ? $"{importCount} Environment Variables successfully imported."
                        : "1 Environment Variable successfully imported.";
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry("NVM#", e.ToString(), EventLogEntryType.Error);
                    await _messageService.ShowAsync(Constants.IMPORT_ERROR, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Imports all the selected environment variables. If there are conflicts,
        /// it will merge the old environment variables with new ones.
        /// </summary>
        private async Task MergeImportAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var importCount = 0;
                    // First import the non-conflicting Environment variables
                    // User environment variable
                    foreach (var enVar in UserImportVariables.Where(v => (v.IsSelected == true) && (!v.HasConflict))
                                                             .Select(v => new EnVar(v.Key, v.Data)))
                    {
                        RegistryManager.SaveEnvironmentVariable(enVar, EnvironmentVariableTarget.User);
                        importCount++;
                    }

                    // System environment variables
                    foreach (var enVar in SystemImportVariables.Where(v => (v.IsSelected == true) && (!v.HasConflict))
                                                             .Select(v => new EnVar(v.Key, v.Data)))
                    {
                        RegistryManager.SaveEnvironmentVariable(enVar, EnvironmentVariableTarget.Machine);
                        importCount++;
                    }

                    // Merge the conflicting Environment variables
                    // User environment variable
                    foreach (var enVar in UserImportVariables.Where(v => (v.IsSelected == true) && (v.HasConflict)))
                    {
                        _userVars[enVar.Key].MergeData(enVar.Data);
                        RegistryManager.SaveEnvironmentVariable(_userVars[enVar.Key], EnvironmentVariableTarget.User);
                        importCount++;
                    }
                    // System environment variable
                    foreach (var enVar in SystemImportVariables.Where(v => (v.IsSelected == true) && (v.HasConflict)))
                    {
                        _systemVars[enVar.Key].MergeData(enVar.Data);
                        RegistryManager.SaveEnvironmentVariable(_systemVars[enVar.Key], EnvironmentVariableTarget.Machine);
                        importCount++;
                    }

                    // Reload the environment variables
                    InitializeEnvironmentVariables();

                    ImportStatus = importCount > 1
                        ? $"{importCount} Environment Variables successfully imported."
                        : "1 Environment Variable successfully imported.";
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry("NVM#", e.ToString(), EventLogEntryType.Error);
                    await _messageService.ShowAsync(Constants.IMPORT_ERROR, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Imports only those selected environment variables
        /// which have no conflicts with existing environment variables
        /// </summary>
        private async Task ImportDiscardAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var importCount = 0;
                    // Save the imported User environment variables
                    foreach (var enVar in UserImportVariables.Where(v => (v.IsSelected == true) && (!v.HasConflict))
                                                             .Select(v => new EnVar(v.Key, v.Data)))
                    {
                        RegistryManager.SaveEnvironmentVariable(enVar, EnvironmentVariableTarget.User);
                        importCount++;
                    }

                    // Save the imported System environment variables
                    foreach (var enVar in SystemImportVariables.Where(v => (v.IsSelected == true) && (!v.HasConflict))
                                                             .Select(v => new EnVar(v.Key, v.Data)))
                    {
                        RegistryManager.SaveEnvironmentVariable(enVar, EnvironmentVariableTarget.Machine);
                        importCount++;
                    }

                    // Reload the environment variables
                    InitializeEnvironmentVariables();

                    ImportStatus = importCount > 1
                        ? $"{importCount} Environment Variables successfully imported."
                        : "1 Environment Variable successfully imported.";
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry("NVM#", e.ToString(), EventLogEntryType.Error);
                    await _messageService.ShowAsync(Constants.IMPORT_ERROR, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        private bool CanExport()
        {
            if ((UserExportVariables == null) || (SystemExportVariables == null))
                return false;

            return UserExportVariables.Any(v => (v.IsSelected == true)) ||
                   SystemExportVariables.Any(v => (v.IsSelected == true));
        }

        private async Task InitExportAsync()
        {
            IsProgressVisible = true;

            await Task.Run(() =>
            {
                // Get the user environment variables
                var userEnVars = _userVars.Values.Select(enVar => new ImportExportVM
                {
                    Key = enVar.Key,
                    Data = enVar.Data,
                    IsSelected = true
                }).ToList();
                userEnVars.Sort();
                UserExportVariables = new ObservableCollection<ImportExportVM>(userEnVars);
                HasUserExportVariables = userEnVars.Count > 0;

                var sysEnVars = _systemVars.Values.Select(enVar => new ImportExportVM
                {
                    Key = enVar.Key,
                    Data = enVar.Data,
                    IsSelected = true
                }).ToList();
                sysEnVars.Sort();
                SystemExportVariables = new ObservableCollection<ImportExportVM>(sysEnVars);
                HasSystemExportVariables = (sysEnVars.Count > 0);

                InitExportStatus = (HasUserExportVariables || HasSystemExportVariables)
                    ? Constants.SELECT_ENVAR_EXPORT
                    : Constants.NO_ENVAR_AVAILABLE_FOR_EXPORT;

                ExportStatus = string.Empty;
            });

            IsProgressVisible = false;
        }

        private async Task ExportAsync(string fileName)
        {
            await Task.Run(async () =>
            {
                // ExportAsync the selected environment variables to the file
                try
                {
                    var exportCount = 0;
                    XElement xRoot = new XElement(Constants.X_ROOT);
                    // Version
                    var xVersion = new XElement(Constants.X_VERSION, Constants.VERSION);
                    xRoot.Add(xVersion);
                    // User Environment Variables
                    var xUser = new XElement(Constants.X_USER);
                    foreach (var xVar in UserExportVariables.Where(v => (v.IsSelected == true))
                        .Select(enVar => new XElement(Constants.X_VARIABLE,
                            new XElement(Constants.X_NAME, enVar.Key),
                            new XElement(Constants.X_VALUE, enVar.Data))))
                    {
                        xUser.Add(xVar);
                        exportCount++;
                    }
                    xRoot.Add(xUser);
                    // System Environment Variables
                    var xSys = new XElement(Constants.X_SYSTEM);
                    foreach (var xVar in SystemExportVariables.Where(v => (v.IsSelected == true))
                        .Select(enVar => new XElement(Constants.X_VARIABLE,
                            new XElement(Constants.X_NAME, enVar.Key),
                            new XElement(Constants.X_VALUE, enVar.Data))))
                    {
                        xSys.Add(xVar);
                        exportCount++;
                    }
                    xRoot.Add(xSys);
                    xRoot.Save(fileName);
                    ExportStatus = exportCount > 1
                        ? $"{exportCount} Environment Variables successfully exported."
                        : "1 Environment Variable successfully exported.";
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry("NVM#", e.ToString(), EventLogEntryType.Error);
                    await _messageService.ShowAsync(Constants.EXPORT_ERROR, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        #endregion
    }
}