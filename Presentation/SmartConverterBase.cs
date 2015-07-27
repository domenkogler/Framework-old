using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Kogler.Framework
{
    public abstract class SmartConverterBase : IValueConverter
    {
        protected static bool Any(IEnumerable enumerable)
        {
            var en = enumerable.GetEnumerator();
            try { en.Reset(); }
            catch (NotImplementedException) { return en.Current != null; }
            return en.MoveNext();
        }

        protected static bool GetInverse(object parameter)
        {
            if (parameter == null) return false;
            if (parameter.ToString().ToLowerInvariant() != "inverse")
                throw new ArgumentOutOfRangeException(nameof(parameter), @"Converter parameter can only be null or string: 'Inverse'!");
            return true;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }

    public sealed class VisibilityConverter : SmartConverterBase<Visibility>
    {
        #region Implementation of IValueConverter

        private static Visibility BoolToVisibility(bool value, bool inverse)
        {
            if (inverse) value = !value;
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CheckTargetType(targetType);
            bool inverse = GetInverse(parameter);
            if (value == null) { return BoolToVisibility(false, inverse); }
            if (value is bool) { return BoolToVisibility((bool)value, inverse); }
            if (value is int) { return BoolToVisibility((int)value > 0, inverse); }
            if (value is string) { return BoolToVisibility(!string.IsNullOrEmpty(value.ToString()), inverse); }
            if (value is IEnumerable) { return BoolToVisibility(Any((IEnumerable)value), inverse); }
            return BoolToVisibility(true, inverse);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (!(value is Visibility)) throw new ArgumentOutOfRangeException(nameof(value), @"VisibilityConverter can only convert from Visibility!");

            if (targetType == typeof(bool))
            {
                return ((Visibility)value == (GetInverse(parameter) ? Visibility.Collapsed : Visibility.Visible));
            }

            throw new ArgumentOutOfRangeException(nameof(targetType), @"VisibilityConverter can only convert to Boolean!");
        }

        #endregion
    }
}