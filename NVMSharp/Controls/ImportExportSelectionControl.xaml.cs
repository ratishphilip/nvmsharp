using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NVMSharp.Common;
using NVMSharp.ViewModel;

namespace NVMSharp.Controls
{
    /// <summary>
    /// Interaction logic for ImportExportSelectionControl.xaml
    /// </summary>
    public partial class ImportExportSelectionControl : UserControl
    {
        #region Fields

        bool _headerFlag = false;
        bool _itemFlag = false;

        #endregion

        #region Events

        #region SelectionChanged

        /// <summary>
        /// SelectionChanged Routed Event
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImportExportSelectionControl));

        /// <summary>
        /// Occurs when the user selects/deselects an item in the list
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the SelectionChanged event.
        /// </summary>
        protected RoutedEventArgs RaiseSelectionChangedEvent()
        {
            return RaiseSelectionChangedEvent(this);
        }

        /// <summary>
        /// A static helper method to raise the SelectionChanged event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        public static RoutedEventArgs RaiseSelectionChangedEvent(DependencyObject target)
        {
            if (target == null)
                return null;

            var args = new RoutedEventArgs
            {
                RoutedEvent = SelectionChangedEvent
            };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #endregion

        #region Dependency Properties

        #region Header

        /// <summary>
        /// Header Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ImportExportSelectionControl),
                new PropertyMetadata(string.Empty,
                    new PropertyChangedCallback(OnHeaderChanged)));

        /// <summary>
        /// Gets or sets the Header property. This dependency property 
        /// indicates the Name to be displayed in the Header.
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Header property.
        /// </summary>
        /// <param name="d">ImportExportSelectionControl</param>
		/// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ImportExportSelectionControl)d;
            var oldHeader = (string)e.OldValue;
            var newHeader = ctrl.Header;
            ctrl.OnHeaderChanged(oldHeader, newHeader);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Header property.
        /// </summary>
		/// <param name="oldHeader">Old Value</param>
		/// <param name="newHeader">New Value</param>
        void OnHeaderChanged(string oldHeader, string newHeader)
        {

        }

        #endregion

        #region Data

        /// <summary>
        /// Data Dependency Property
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IEnumerable<ImportExportVM>), typeof(ImportExportSelectionControl),
                new PropertyMetadata(null,
                    new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Gets or sets the Data property. This dependency property 
        /// indicates the list of Environment Variables to be displayed.
        /// </summary>
        public IEnumerable<ImportExportVM> Data
        {
            get { return (IEnumerable<ImportExportVM>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Data property.
        /// </summary>
        /// <param name="d">ImportExportSelectionControl</param>
		/// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ImportExportSelectionControl)d;
            var oldData = (IEnumerable<ImportExportVM>)e.OldValue;
            var newData = ctrl.Data;
            ctrl.OnDataChanged(oldData, newData);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Data property.
        /// </summary>
		/// <param name="oldData">Old Value</param>
		/// <param name="newData">New Value</param>
        void OnDataChanged(IEnumerable<ImportExportVM> oldData, IEnumerable<ImportExportVM> newData)
        {
            DataList.ItemsSource = newData;
        }

        #endregion

        #endregion

        #region Construction / Initialization

        public ImportExportSelectionControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void HandleHeaderSelection(object sender, RoutedEventArgs e)
        {
            if (_itemFlag)
                return;

            _headerFlag = true;

            foreach (ImportExportVM item in DataList.Items)
            {
                item.IsSelected = HeaderCB.IsChecked;
            }

            // Raise the event to notify that the selection has changed
            RaiseSelectionChangedEvent(this);

            _headerFlag = false;
        }

        private void HandleItemSelection(object sender, RoutedEventArgs e)
        {
            if (_headerFlag)
                return;

            _itemFlag = true;

            UpdateUserSelection();
            // Raise the event to notify that the selection has changed
            RaiseSelectionChangedEvent(this);

            _itemFlag = false;
        }

        #endregion

        #region Helpers

        private void UpdateUserSelection()
        {
            bool? state = null;

            bool firstCheck = true;
            foreach (ImportExportVM item in DataList.Items)
            {
                bool? currentState = item.IsSelected;
                if (firstCheck)
                {
                    state = currentState;
                    firstCheck = false;
                }
                else if (state != currentState)
                {
                    state = null;
                    break;
                }
            }

            HeaderCB.IsChecked = state;
        }

        #endregion
    }
}
