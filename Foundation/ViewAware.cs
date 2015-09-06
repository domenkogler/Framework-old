using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace Kogler.Framework
{
    public class ViewAware : Caliburn.Micro.ViewAware
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
            propertyNames.Apply(NotifyOfPropertyChange);
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
    }
}