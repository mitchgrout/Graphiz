using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphiz
{
    class Viewport
    {
        /// <summary>
        /// The translated origin, which is at the same location as the actual viewport origin
        /// </summary>
        public Point Origin;
        

        /// <summary>
        /// The zoom scale for the viewport; 100 is no zoom, below is zoom in, above is zoom out 
        /// </summary>
        public int Scale
        {
            get { return scale; }
            set { if (value < 0) throw new ArgumentException("Scale cannot be below zero"); scale = value; }
        }
        private int scale;

        public Viewport()
            : this(Point.Empty, 100)
        {
        }

        public Viewport(int x, int y, int scale)
            : this(new Point(x, y), scale)
        {
            
        }

        public Viewport(Point origin, int scale)
        {
            this.Origin = origin;
            this.Scale = scale;
        }


        /// <summary>
        /// Take a point from the screen and map it to a graph point
        /// </summary>
        public Point ToWorldPos(Point p)
        {
            return new Point(this.Origin.X + (int)(p.X * this.Scale) / 100,
                             this.Origin.Y + (int)(p.Y * this.Scale) / 100);
        }

        /// <summary>
        /// Take a graph point and map it to the screen
        /// </summary>
        public Point FromWorldPos(Point p)
        {
            return new Point((int)((p.X - this.Origin.X) * 100) / (int)this.Scale,
                             (int)((p.Y - this.Origin.Y) * 100) / (int)this.Scale);
        }

        /// <summary>
        /// Rescale a scalar
        /// </summary>
        /// <param name="i">
        public int Rescale(int i)
        {
            return (i * 100) / Scale;
        }
    }
}
