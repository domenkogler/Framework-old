using System;
using System.Collections;
using System.Globalization;
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
}