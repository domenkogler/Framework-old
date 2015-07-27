using System;

namespace Kogler.Framework
{
    /// <summary>
    /// Interface for a module controller which is responsible for the module lifecycle.
    /// </summary>
    public interface IModuleController
    {
        /// <summary>
        /// Initializes the module controller.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Run the module controller.
        /// </summary>
        void Run();

        /// <summary>
        /// Shutdown the module controller.
        /// </summary>
        void Shutdown();
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