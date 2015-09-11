using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace Kogler.Framework
{
    public abstract class PropertyChangedBase : Caliburn.Micro.PropertyChangedBase
    {
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