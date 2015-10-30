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
using NVMSharp.Common;

namespace NVMSharp.ViewModel
{
    /// <summary>
    /// The ViewModel used for displaying Environment Variables in 
    /// Import/Export app mode.
    /// </summary>
    public class ImportExportVM : BindableBase, IComparable<ImportExportVM>
    {
        #region Properties

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

        #endregion

        #region IComparable Implementation

        /// <summary>
        /// Compares the current object to the given
        /// ImportExportVM object
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ImportExportVM other)
        {
            return String.Compare(Key, other.Key, StringComparison.CurrentCulture);
        }

        #endregion
    }
}
