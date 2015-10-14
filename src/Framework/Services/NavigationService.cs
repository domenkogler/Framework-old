using System;
using System.Collections.Generic;

namespace Kogler.Framework
{
    public interface INavigationService
    {
        bool Navigate(string path, IDictionary<string, string> state = null);
        bool Navigate(object viewModel, IDictionary<string, string> state = null);
    }

    public abstract class NavigationServiceBase : INavigationService
    {
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
    }
}