using System;
using System.Collections.Generic;
using System.Linq;

namespace Kogler.Framework
{
    public interface INavigationService
    {
        bool Navigate(string path, IDictionary<string, string> state = null);
        bool Navigate(object viewModel, IDictionary<string, string> state = null);
    }

    public abstract class NavigationServiceBase : INavigationService
    {
        public const string Path = "__Path";

        public class PathResolve
        {
            public PathResolve(object viewModel, Dictionary<string, string> state = null)
            {
                ViewModel = viewModel;
                State = state;
            }

            public object ViewModel { get; }
            public Dictionary<string, string> State { get; }
        }

        public bool Navigate(string path, IDictionary<string, string> state = null)
        {
            var resolved = ResolvePath(path);
            if (resolved.State != null && state != null)
            {
                resolved.State.Add(state);
                state = resolved.State;
            }
            return Navigate(resolved.ViewModel, state);
        }

        public abstract bool Navigate(object viewModel, IDictionary<string, string> state = null);

        /// <summary>
        /// Resolves navigation path to Tuple with viewModel and state dictionary
        /// </summary>
        public static Func<string, PathResolve> ResolvePath { get; set; } = path => { throw new NotImplementedException(); };

        public static Dictionary<string, string> ParsePath(string path)
        {
            Dictionary<string, string> dic;
            var delimiterIndex = path.IndexOf('?');
            if (delimiterIndex > 0)
            {
                dic = path
                    .Substring(delimiterIndex + 1)
                    .Split('&')
                    .Select(s => new {s, index = s.IndexOf('=')})
                    .ToDictionary(s => s.index < 0 ? s.s : s.s.Substring(0, s.index),
                        s => s.index < 0 ? null : s.s.Substring(s.index + 1));
                dic.Add(Path, path.Substring(0, delimiterIndex));
            }
            else dic = new Dictionary<string, string> {{ Path , path.Substring(0, delimiterIndex)}};
            return dic;
        }
    }
}