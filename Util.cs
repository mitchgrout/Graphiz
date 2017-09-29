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

    // Ext methods
    static class Ext
    {
        public static double Norm(this Point p)
        {
            return Math.Sqrt(p.NormSq());
        }

        public static int NormSq(this Point p)
        {
            return p.X * p.X + p.Y * p.Y;
        }

        public static Point Sub(this Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
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
    }
}
