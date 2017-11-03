using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Graphiz
{
    // Cartesian product on two types
    public struct Pair<A, B>
    {
        public A Left;
        public B Right;
        public Pair(A left, B right)
        {
            this.Left = left;
            this.Right = right;
        }
    }
    
    // Extension methods
    static class Ext
    {
        #region System.Drawing.Point        
        public static Point Add(this Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point Sub(this Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static double Norm(this Point p)
        {
            return Math.Sqrt(p.NormSquare());
        }

        public static int NormSquare(this Point p)
        {
            return p.X * p.X + p.Y * p.Y;
        }
        #endregion

        #region System.Collections.Generic.IEnumerable
        public static IEnumerable<int> Iota(this int start, int end, int step)
        {
            for (int x = start; x < end; x += step)
                yield return x;
        }

        public static IEnumerable<Pair<A, B>> Product<A, B>(this IEnumerable<A> @as, IEnumerable<B> @bs)
        {
            return from a in @as
                   from b in @bs
                   select new Pair<A, B>(a, b);
        }

        public static void Each<T>(this IEnumerable<T> ts, Action<T> fn)
        {
            foreach (T t in ts)
                fn(t);
        }

        public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> ts, T val)
        {
            int count = ts.Count();
            if (count == 0)
                yield break;

            foreach(var t in ts)
            {
                yield return t;
                if (--count > 0) yield return val;
            }
        }

        public static IEnumerable<T> Wrap<T>(this IEnumerable<T> ts, T start, T end)
        {
            yield return start;
            foreach(var t in ts) yield return t;
            yield return end;
        }

        public static string AsString(this IEnumerable<string> e)
        {
            return string.Join("", e);
        }
        #endregion
    }
}