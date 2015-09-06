using System;
using System.Text;

namespace Kogler.Framework
{
    public static class ExceptionExtensions
    {
        public static string GetErrorMessage(this Exception e, bool includeStackTrace = true, bool nested = true)
        {
            if (e == null) return string.Empty;
            var nl = Environment.NewLine;
            var sb = new StringBuilder();
            var line = "----------------------------------------" + nl;
            sb.Append(line);
            sb.AppendFormat("{0}: {1}{2}", e.GetType().Name, e.Message, nl);
            if (includeStackTrace) sb.AppendFormat("StackTrace: {0}{1}", e.StackTrace, nl);
            if (nested)
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    sb.AppendFormat("Inner Exception: {0}{1}", e.Message, nl);
                }
            sb.Append(line);
            return sb.ToString();
        }
    }
}