using System;
using NVMSharp.Common;
using System.Globalization;

namespace NVMSharp.ViewModel
{
    public class ImportExportVM : BindableBase, IComparable<ImportExportVM>
    {
        #region HasConflict

        private bool _hasConflict = false;

        /// <summary>
        /// Gets or sets the HasConflict property. This observable property 
        /// indicates whether this Environment Variable has a conflict
        /// with an existing Environment Variable.
        /// </summary>
        public bool HasConflict
        {
            get { return _hasConflict; }
            set { this.SetProperty(ref this._hasConflict, value); }
        }

        #endregion

        #region IsSelected

        private bool? _isSelected = false;

        /// <summary>
        /// Gets or sets the IsSelected property. This observable property 
        /// indicates whether this Environment variable has been selected for
        /// Import/Export.
        /// </summary>
        public bool? IsSelected
        {
            get { return _isSelected; }
            set { this.SetProperty(ref this._isSelected, value); }
        }

        #endregion

        #region Key

        private string _key = string.Empty;

        /// <summary>
        /// Gets or sets the Key property. This observable property 
        /// indicates the Name of the Environment Variable.
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { this.SetProperty(ref this._key, value); }
        }

        #endregion

        #region Data

        private string _data = string.Empty;

        /// <summary>
        /// Gets or sets the Data property. This observable property 
        /// indicates the Value of the Environment Variable.
        /// </summary>
        public string Data
        {
            get { return _data; }
            set { this.SetProperty(ref this._data, value); }
        }

        #endregion

        public int CompareTo(ImportExportVM other)
        {
            return String.Compare(Key, other.Key, StringComparison.CurrentCulture);
        }
    }
}
