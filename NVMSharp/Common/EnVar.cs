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

        public EnVar(string key, string data)
        {
            Key = key;
            Data = data;
        }

        #endregion

        #region APIs

        public void AddValue(string newValue)
        {
            if (String.IsNullOrWhiteSpace(newValue) ||
                String.IsNullOrWhiteSpace(Data))
                return;

            var values = Values;
            values.Add(newValue);
            Data = String.Join(Constants.SEPARATOR, values);
        }

        public void UpdateValue(string oldValue, string newValue)
        {
            if (String.IsNullOrWhiteSpace(oldValue) ||
                String.IsNullOrWhiteSpace(newValue) ||
                String.IsNullOrWhiteSpace(Data) ||
                !Values.Contains(oldValue))
                return;

            var values = Values;
            int index = values.IndexOf(oldValue);
            values.Insert(index, newValue);
            values.Remove(oldValue);
            Data = String.Join(Constants.SEPARATOR, values);
        }

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

        public void ReverseValues()
        {
            if (String.IsNullOrWhiteSpace(Data) ||
                (Values.Count == 1))
                return;

            var values = Values;
            values.Reverse();
            Data = String.Join(Constants.SEPARATOR, values);
        }

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
