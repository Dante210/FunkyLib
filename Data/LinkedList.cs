using System;
using System.Collections.Generic;
using System.Linq;

namespace funkylib.Data
{
    public sealed class LinkedList<T>
    {
        readonly T head;
        readonly bool isEmpty;
        readonly LinkedList<T> tail;

        // the empty list
        internal LinkedList() { isEmpty = true; }

        // the non empty list
        internal LinkedList(T head, LinkedList<T> tail) {
            this.head = head;
            this.tail = tail;
        }

        // for convenience
        public T Head => Match(
            () => throw new IndexOutOfRangeException(),
            (head, _) => head);

        public LinkedList<T> Tail => Match(
            () => throw new IndexOutOfRangeException(),
            (_, tail) => tail);

        // not really required, but hey...
        public T this[int index] => Match(
            () => throw new IndexOutOfRangeException(),
            (head, tail) => index == 0 ? head : tail[index - 1]);

        public R Match<R>(Func<R> Empty, Func<T, LinkedList<T>, R> Cons)
            => isEmpty ? Empty() : Cons(head, tail);

        public IEnumerable<T> AsEnumerable() {
            if (isEmpty) yield break;
            yield return head;
            foreach (var t in tail.AsEnumerable()) yield return t;
        }

        public override string ToString() => Match(
            () => "{ }",
            (_, __) => $"{{ {string.Join(", ", AsEnumerable().map(v => v.ToString()))} }}");
    }

    public static class LinkedList
    {
        // factory functions
        public static LinkedList<T> List<T>(T h, LinkedList<T> t) => new LinkedList<T>(h, t);

        public static LinkedList<T> List<T>(params T[] items)
            => items.Reverse().Aggregate(
                new LinkedList<T>()
                , (tail, head) => List(head, tail));

        public static int Length<T>(this LinkedList<T> @this) => @this.Match(
            () => 0,
            (t, ts) => 1 + ts.Length());

        public static LinkedList<T> Add<T>(this LinkedList<T> @this, T value)
            => List(value, @this);

        public static LinkedList<T> Append<T>(this LinkedList<T> @this, T value)
            => @this.Match(
                () => List(value, List<T>()),
                (head, tail) => List(head, tail.Append(value)));

        public static LinkedList<A> insertAt<A>(this LinkedList<A> @this, int index, A value) =>
            index == 0
                ? List(value, @this)
                : @this.Match(
                    () => throw new IndexOutOfRangeException(),
                    (head, tail) => List(head, tail.insertAt(index - 1, value)));

        public static LinkedList<A> removeAt<A>(this LinkedList<A> @this, int index) =>
            index == 0
                ? @this.Tail
                : @this.Match(
                    () => throw new IndexOutOfRangeException(),
                    (head, tail) => List(head, tail.removeAt(index - 1)));

        public static LinkedList<A> takeWhile<A>(this LinkedList<A> @this, Func<A, bool> predicate) =>
            @this.Match(
                () => @this, (head, tail)
                    => predicate(head) ? List(head, tail.takeWhile(predicate)) : List<A>());

        public static LinkedList<A> takeWhile<A>(this IEnumerable<A> @this, Func<A, bool> predicate) {
            var temp = List<A>();
            foreach (var item in @this)
                if (predicate(item))
                    temp.Add(item);
            return temp;
        }

        public static LinkedList<A> dropWhile<A>(this LinkedList<A> @this, Func<A, bool> predicate) =>
            @this.Match(() => @this, (head, tail) => predicate(head) ? tail.dropWhile(predicate) : @this);

        public static LinkedList<R> Map<T, R>(this LinkedList<T> @this, Func<T, R> f)
            => @this.Match(
                () => List<R>(),
                (head, tail) => List(f(head), tail.Map(f)));

        public static Unit ForEach<T>(this LinkedList<T> @this, Action<T> action) {
            @this.Map(action.toFunc());
            return new Unit();
        }

        public static LinkedList<R> Bind<T, R>(this LinkedList<T> @this, Func<T, LinkedList<R>> f)
            => @this.Map(f).Join();

        public static LinkedList<T> Join<T>(this LinkedList<LinkedList<T>> @this) => @this.Match(
            () => List<T>(),
            (xs, xss) => concat(xs, Join(xss)));

        public static Acc Aggregate<A, Acc>(this LinkedList<A> @this, Acc acc, Func<Acc, A, Acc> func) =>
            @this.Match(() => acc, (head, tail) => Aggregate(tail, func(acc, head), func));

        static LinkedList<T> concat<T>(LinkedList<T> l, LinkedList<T> r) => l.Match(
            () => r,
            (h, t) => List(h, concat(t, r)));
    }
}