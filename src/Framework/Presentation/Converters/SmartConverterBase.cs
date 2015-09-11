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

    public abstract class SmartConverterBase<TType> : SmartConverterBase
    {
        protected static void CheckTargetType(Type targetType)
        {
            CheckTargetType<TType>(targetType);
        }

        protected static void CheckTargetType<TType1>(Type targetType)
        {
            var type = typeof(TType1);
            if (type == targetType) return;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) && targetType.GetGenericArguments()[0] == type) return;
            throw new ArgumentOutOfRangeException("targetType", string.Format(@"Converter can only convert to {0}!", typeof(TType1)));
        }
    }
}