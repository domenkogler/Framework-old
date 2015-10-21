using System;

namespace Kogler.Framework
{
    [Flags]
    public enum Languages : uint
    {
        None = 0,
        Slovenian = 1,
        English = 1 << 1,
        German = 1 << 2
    }

    public static class LanguagesExtension
    {
        public static uint ToUInt(this Languages languages)
        {
            return (uint)languages;
        }

        public static Languages ToLanguage(this uint languages)
        {
            return (Languages)languages;
        }
    }
}