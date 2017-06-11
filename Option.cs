using System;
using System.Collections.Generic;

namespace funkylib
{
    public struct Unit
    {
    }

    public struct Option
    {
        public static Option<A> Some<A>(A value) { return new Some<A>(value); }

        public static None None => None.none;
    }

    public struct Option<A> : IEquatable<Option<A>>, IEquatable<None>
    {
        readonly A value;
        public readonly bool isSome;
        bool isNone => !isSome;

        internal Option(A value) {
            if (value == null) throw new ArgumentNullException();
            isSome = true;
            this.value = value;
        }

        public R fold<R>(Func<R> onNone, Func<A, R> onSome) { return isSome ? onSome(value) : onNone(); }

        public void fold(Action onNone, Action<A> onSome) {
            if (isSome)
                onSome(value);
            else
                onNone();
        }

        /// <summary>
        ///     Calls on some action
        /// </summary>
        /// <param name="action">Action to perform</param>
        public void each(Action<A> action) {
            if (isSome) action(value);
        }


        public static implicit operator Option<A>(None _) { return new Option<A>(); }

        public static implicit operator Option<A>(Some<A> some) { return new Option<A>(some.value); }

        public static implicit operator Option<A>(A value) { return value == null ? Option.None : Option.Some(value); }

        public bool Equals(Option<A> other) {
            return isSome == other.isSome
                && (isNone || value.Equals(other.value));
        }

        public bool Equals(None _) { return isNone; }

        public override string ToString() { return isSome ? $"Some({value})" : "None"; }
    }

    public struct Some<A>
    {
        internal A value;
        internal Some(A value) { this.value = value; }
    }

    public struct None
    {
        internal static readonly None none = new None();
    }

    public static class OptionExt
    {
        public static Option<A> some<A>(this A value) { return new Option<A>(value); }

        //Map   (A -> R) -> Option<R>
        public static Option<R> map<A, R>(this Option<A> @this, Func<A, R> func) {
            return @this.fold(
                () => Option.None, value => Option.Some(func(value)));
        }

        //Bind  (A -> Option<R>) ->Option<R>
        public static Option<R> flatMap<A, R>(this Option<A> @this, Func<A, Option<R>> func) {
            return @this.fold(
                () => Option.None, func);
        }

        public static IEnumerable<A> asEnumerable<A>(this IEnumerable<Option<A>> @this) {
            var temp = new List<A>();
            foreach (var option in @this)
                option.fold(() => { }, some => temp.Add(some));
            return temp;
        }
    }
}