using System;
using System.Collections.Generic;
using System.Linq;

namespace Kogler.Framework
{
    public interface IHaveState
    {
        void SetState(IDictionary<string, string> state);
    }

    public interface IHaveTypeState : IHaveState
    {
        void SetTypeState(object type, IDictionary<string, string> state = null);
    }

    public class State
    {
        public const string AllStates = "__All";
        public const string EmptyState = "__Empty";
        public const string TypeState = "__Type";
        public const string PathState = "path";
        public const string UriState = "uri";
        public const string NewInstanceState = "__NewInstance";
        public const char StateSplit = '=';

        public static KeyValuePair<string, string> NewInstancePair
        {
            get { return new KeyValuePair<string, string>(NewInstanceState, "true"); }
        }

        public static string Type(object obj)
        {
            return Type(obj.GetType());
        }

        public static string Type(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        public static string Type<T>()
        {
            return Type(typeof(T));
        }

        public static Dictionary<string, string> FromUri(string uri, params KeyValuePair<string, string>[] states)
        {
            Dictionary<string, string> dic;
            var delimiterIndex = uri.IndexOf('?');
            if (delimiterIndex > 0)
            {
                dic = uri
                    .Substring(delimiterIndex + 1)
                    .Split('&')
                    .Select(s => new { s, index = s.IndexOf(StateSplit) })
                    .ToDictionary(s => s.index < 0 ? s.s : s.s.Substring(0, s.index),
                        s => s.index < 0 ? null : s.s.Substring(s.index + 1));
                dic.Add(UriState, uri.Substring(0, delimiterIndex));
            }
            else dic = new Dictionary<string, string> { { UriState, uri } };
            return dic;
        }
    }

    public static class StateExtensions
    {
        public static KeyValuePair<string, string> ToTypePair<T>(T obj) where T : class
        {
            return new KeyValuePair<string, string>(State.TypeState, State.Type(obj));
        }

        public static Dictionary<string, string> ToState<T>(T obj, params KeyValuePair<string, string>[] states) where T : class
        {
            var dic = new Dictionary<string, string> { ToTypePair(obj)};
            dic.Add(states);
            return dic;
        }

        public static void SetState<T>(this IHaveState viewModel, T type, IDictionary<string, string> state = null) where T : class
        {
            if (state == null) state = ToState(type);
            else state.Add(ToTypePair(type));
            viewModel.SetState(state);
        }

        public static Dictionary<string, string> ToState(this IEnumerable<string> states)
        {
            var dic = states
                .Select(s => s.Split(State.StateSplit))
                .Select(s => new KeyValuePair<string, string>(s[0], s.Count() > 1 ? s[1] : s[0]));
            return new Dictionary<string, string> {dic};
        }

        public static KeyValuePair<string, string> ToPair(this string state, string key = null)
        {
            return new KeyValuePair<string, string> (key ?? state, state);
        }

        public static Dictionary<string, string> ToState(this string state, string key = null, params KeyValuePair<string, string>[] states)
        {
            var dic = new Dictionary<string, string> { ToPair(state, key) };
            dic.Add(states);
            return dic;
        }
    }
}