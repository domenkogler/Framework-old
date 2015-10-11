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
            return Type(typeof (T));
        }

        public const string AllStates = "__All";
        public const string EmptyState = "__Empty";
        public const string TypeState = "__Type";
        public const char StateSplit = ':';
    }

    public static class StateExtensions
    {
        public static KeyValuePair<string, string> ToPair(object obj)
        {
            return new KeyValuePair<string, string>(State.TypeState, State.Type(obj));
        }

        public static Dictionary<string, string> ToState(object obj)
        {
            return new Dictionary<string, string> {ToPair(obj)};
        }

        public static void SetState(this IHaveState viewModel, object type, IDictionary<string, string> state = null)
        {
            if (state == null) state = ToState(type);
            else state.Add(ToPair(type));
            viewModel.SetState(state);
        }

        public static Dictionary<string, string> ToState(this IEnumerable<string> states)
        {
            var dic = states
                .Select(s => s.Split(State.StateSplit))
                .Select(s => new KeyValuePair<string, string>(s[0], s.Count() > 1 ? s[1] : s[0]));
            return new Dictionary<string, string> {dic};
        }

        public static KeyValuePair<string, string> ToPair(this string state)
        {
            return new KeyValuePair<string, string> (state, state);
        }

        public static Dictionary<string, string> ToState(this string state)
        {
            return new Dictionary<string, string> { ToPair(state) };
        }
    }
}