using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NVMSharp.Common
{
    /// <summary>
    ///     Implementation of <see cref="INotifyPropertyChanged" /> to simplify models.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        protected Dictionary<string, List<RelayCommand>> _commandRoutes;

        /// <summary>
        ///     Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers
        ///     that support <see cref="CallerMemberNameAttribute" />.
        /// </param>
        //[NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if ((propertyName != null) && (_commandRoutes != null) &&
                (_commandRoutes.ContainsKey(propertyName)) && (_commandRoutes?[propertyName] != null))
            {
                foreach (var command in _commandRoutes[propertyName])
                {
                    command.RaiseCanExecuteChanged();
                }
            }
            
        }

        /// <summary>
        /// Maps a command to the CoreViewModel's properties so that when those properties change
        /// the respective Command can be refreshed
        /// </summary>
        /// <param name="command">RelayCommand</param>
        /// <param name="properties">List of CoreViewModel property names which collectively determine
        /// whether the command should be enabled or not</param>
        protected void MapCommand(RelayCommand command, params string[] properties)
        {
            if (_commandRoutes == null)
                _commandRoutes = new Dictionary<string, List<RelayCommand>>();

            if ((properties == null) || (properties.Length == 0))
                return;

            foreach (var prop in properties)
            {
                if ((!_commandRoutes.ContainsKey(prop)) || (_commandRoutes[prop] == null))
                {
                    _commandRoutes[prop] = new List<RelayCommand>();
                }

                _commandRoutes[prop].Add(command);
            }
        }

        protected RelayCommand CreateCommand(Action execute, params string[] properties)
        {
            var command = new RelayCommand(execute);
            MapCommand(command, properties);
            return command;
        }

        protected RelayCommand CreateCommand(Action execute, Func<bool> canExecute, params string[] properties)
        {
            var command = new RelayCommand(execute, canExecute);
            MapCommand(command, properties);
            return command;
        }
    }
}

