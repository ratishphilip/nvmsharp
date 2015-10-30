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
using System.Collections.Generic;
using System.Linq;

namespace NVMSharp.Common
{
    /// <summary>
    /// This class encapsulates an Environment Variable
    /// </summary>
    internal class EnVar : IEquatable<EnVar>
    {
        #region Properties

        /// <summary>
        /// The Name of the Environment Variable
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// The Value of the Environment Variable 
        /// </summary>
        public string Data { get; private set; }
        /// <summary>
        /// The Value of the Environment Variable converted
        /// to a list of string values (if the value contains
        /// multiple sub-values separated by ';')
        /// </summary>
        public List<string> Values => Data?.Split(new[] { Constants.SEPARATOR }, StringSplitOptions.RemoveEmptyEntries).ToList();

        #endregion

        #region Construction / Initialization

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="key">The name of the Environment Variable</param>
        /// <param name="data">The value of the Environment Variable</param>
        public EnVar(string key, string data)
        {
            Key = key;
            Data = data;
        }

        #endregion

        #region APIs

        /// <summary>
        /// Adds a new value to the existing value
        /// </summary>
        /// <param name="newValue">new value to be added</param>
        public void AddValue(string newValue)
        {
            if (String.IsNullOrWhiteSpace(newValue) ||
                String.IsNullOrWhiteSpace(Data))
                return;

            var values = Values;
            values.Add(newValue);
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Replaces the environment variable's existing value
        /// with a new one.
        /// </summary>
        /// <param name="oldValue">Old Value</param>
        /// <param name="newValue">New Value</param>
        public void UpdateValue(string oldValue, string newValue)
        {
            if (String.IsNullOrWhiteSpace(oldValue) ||
                String.IsNullOrWhiteSpace(newValue) ||
                String.IsNullOrWhiteSpace(Data) ||
                !Values.Contains(oldValue))
                return;

            var values = Values;
            var index = values.IndexOf(oldValue);
            values.Insert(index, newValue);
            values.Remove(oldValue);
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Moves the value to the beginning of the
        /// Values array
        /// </summary>
        /// <param name="keyValue">value to be moved</param>
        public void MoveToTop(string keyValue)
        {
            if (String.IsNullOrWhiteSpace(keyValue) ||
                String.IsNullOrWhiteSpace(Data) ||
                !Values.Contains(keyValue))
                return;

            var values = Values;
            values.Remove(keyValue);
            values.Insert(0, keyValue);
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Moves the value one index up in the Values array
        /// </summary>
        /// <param name="keyValue">value to be moved</param>
        public void MoveUp(string keyValue)
        {
            if (String.IsNullOrWhiteSpace(keyValue) ||
                String.IsNullOrWhiteSpace(Data) ||
                !Values.Contains(keyValue))
                return;

            var values = Values;
            var index = values.IndexOf(keyValue);
            if (index > 0)
            {
                values.Remove(keyValue);
                values.Insert(index - 1, keyValue);
            }
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Moves the value one index below in the Values array
        /// </summary>
        /// <param name="keyValue">value to be moved</param>
        public void MoveDown(string keyValue)
        {
            if (String.IsNullOrWhiteSpace(keyValue) ||
                String.IsNullOrWhiteSpace(Data) ||
                !Values.Contains(keyValue))
                return;

            var values = Values;
            var index = values.IndexOf(keyValue);
            if (index < values.Count - 1)
            {
                values.Remove(keyValue);
                values.Insert(index + 1, keyValue);
            }
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Moves the value to the end of the Values array
        /// </summary>
        /// <param name="keyValue">value to be moved</param>
        public void MoveToBottom(string keyValue)
        {
            if (String.IsNullOrWhiteSpace(keyValue) ||
                String.IsNullOrWhiteSpace(Data) ||
                !Values.Contains(keyValue))
                return;

            var values = Values;
            values.Remove(keyValue);
            values.Add(keyValue);
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Reverses the order of the values in the Values array
        /// </summary>
        public void ReverseValues()
        {
            if (String.IsNullOrWhiteSpace(Data) ||
                (Values.Count == 1))
                return;

            var values = Values;
            values.Reverse();
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Deletes the value from the Values array
        /// </summary>
        /// <param name="keyValue">value to be moved</param>
        public void DeleteValue(string keyValue)
        {
            if (String.IsNullOrWhiteSpace(keyValue) ||
                String.IsNullOrWhiteSpace(Data) ||
                !Values.Contains(keyValue))
                return;

            var values = Values;
            values.Remove(keyValue);
            Data = String.Join(Constants.SEPARATOR, values);
        }

        /// <summary>
        /// Merges the existing values with the unique values in the given
        /// data string.
        /// </summary>
        /// <param name="data">string containing new values</param>
        public void MergeData(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
                return;

            var values = Values;
            var newValues = data.Split(new[] {Constants.SEPARATOR}, StringSplitOptions.RemoveEmptyEntries).ToList();
            values.AddRange(newValues.Except(values).Distinct());
            Data = String.Join(Constants.SEPARATOR, values);
        }

        #endregion

        #region IEquatable Implementation

        /// <summary>
        /// Checks if the given EnVar object has the same key
        /// as this EnVar object
        /// </summary>
        /// <param name="other">EnVar object</param>
        /// <returns></returns>
        public bool Equals(EnVar other)
        {
            if (other == null)
                return false;

            return other.Key == Key;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var enObj = obj as EnVar;
            return (enObj != null) && Equals(enObj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Key}: {Data}";
        }

        #endregion

        #region Operator Overloads

        public static bool operator ==(EnVar varA, EnVar varB)
        {
            bool result;

            if (ReferenceEquals(varA, varB))
            {
                result = true;
            } else if (((object) varA == null) || ((object) varB == null))
            {
                result = false;
            }
            else
            {
                result = varA.Key == varB.Key;
            }

            return result;
        }

        public static bool operator !=(EnVar varA, EnVar varB)
        {
            return !(varA == varB);
        }

        #endregion
    }
}
