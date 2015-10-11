using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace Kogler.Framework
{
    [Serializable]
    public abstract class PropertyChangedBase : Caliburn.Micro.PropertyChangedBase
    {
        /// <summary>
        /// Set the property with the specified value. PropertyChanged event is raised and it returns true if the value changes.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="force">Force change, even if the values are equal</param>
        /// <param name="propertyNames">The property names for PropertyChanged event.</param>
        /// <returns>True if the value has changed</returns>
        protected bool Set<T>(ref T field, T value, bool force, params string[] propertyNames)
        {
            if (!force)
            {
                if (Equals(field, value)) return false;
            }
            field = value;
            RaisePropertyChanged(propertyNames);
            return true;
        }

        /// <summary>
        /// Set the property with the specified value. PropertyChanged event is raised and it returns true if the value changes.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="force">Force change, even if the values are equal</param>
        /// <param name="callerPropertyName">The property name. This optional parameter can be skipped
        /// because the compiler is able to create it automatically.</param>
        /// <returns>True if the value has changed</returns>
        protected bool Set<T>(ref T field, T value, bool force, [CallerMemberName] string callerPropertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return Set(ref field, value, force, new[] { callerPropertyName });
        }

        /// <summary>
        /// Set the property with the specified value. If the value is not equal with the field then the field is
        /// set, a PropertyChanged event is raised and it returns true.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="callerPropertyName">The property name. This optional parameter can be skipped
        /// because the compiler is able to create it automatically.</param>
        /// <returns>True if the value has changed, false if the old and new value were equal.</returns>
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string callerPropertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return Set(ref field, value, false, callerPropertyName);
        }

        /// <summary>
        /// Set the property with the specified value. If the value is not equal with the field then the field is
        /// set, a PropertyChanged event is raised and it returns true.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="propertyNames">The property names for PropertyChanged event.</param>
        /// <returns>True if the value has changed, false if the old and new value were equal.</returns>
        protected bool Set<T>(ref T field, T value, params string[] propertyNames)
        {
            return Set(ref field, value, false, propertyNames);
        }

        /// <summary>
        /// a dictionary to cache property changed event args generated for each property
        /// </summary>
        [field: NonSerialized]
        private static readonly Dictionary<string, PropertyChangedEventArgs> EventArgsMap = new Dictionary<string, PropertyChangedEventArgs>();
        
        /// <summary>
        /// Get a PropertyChangedEventArgs instance fromt he dictionary or
        /// create a new one if not present
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <returns>Instance of the class</returns>
        private static PropertyChangedEventArgs GetEventArgs(string propertyName)
        {
            PropertyChangedEventArgs pe;
            if (EventArgsMap.TryGetValue(propertyName, out pe)) return pe;
            return EventArgsMap[propertyName] = new PropertyChangedEventArgs(propertyName);
        }

        /// <summary>
        /// Raises a change notification event to signal a change in the
        /// specified property's value.
        /// </summary>
        /// <param name="propertyNames">The properties that have changed.</param>
        public virtual void RaisePropertyChanged(params string[] propertyNames)
        {
            OnPropertyChanged(propertyNames);
        }

        /// <summary>
        /// Raises a change notification event to signal a change in the
        /// specified property's value.
        /// </summary>
        /// <param name="property">The property that has changed.</param>
        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(property?.ToPropertyName());
        }

        /// <summary>
        /// Raises a change notification event to signal a change in the
        /// specified property's value.
        /// </summary>
        /// <param name="properties">The properties that have changed.</param>
        public void RaisePropertyChanged<TProperty>(params Expression<Func<TProperty>>[] properties)
        {
            OnPropertyChanged(properties?.Select(p => p.ToPropertyName()).ToArray());
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// /// <param name="propertyNames">The properties that have changed.</param>
        // Use RaisePropertyChanged event outside this class (handles correct Threat execution)
        
        private void OnPropertyChanged(params string[] propertyNames)
        {
            if(propertyNames == null || !IsNotifying) return;
            propertyNames.Where(p => !string.IsNullOrEmpty(p)).Apply(NotifyOfPropertyChange);
        }

        [NotifyPropertyChangedInvocator]
        public sealed override void NotifyOfPropertyChange(string propertyName = null)
        {
            if (IsNotifying) Execute.OnUIThread(() => OnPropertyChanged(GetEventArgs(propertyName)));
        }
    }
}