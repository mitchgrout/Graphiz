using System.Drawing;

namespace Graphiz
{
    public class GlobalState
    {
        private GlobalState() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GlobalState Instance
        {
            get
            {
                return _instance ?? (_instance = new GlobalState());
            }
        }
        private static GlobalState _instance;

        /// <summary>
        /// Array of colours used to paint vertices when they have been assigned a proper vertex colouring
        /// </summary>
        public Brush[] VertexColors = new Brush[] { Brushes.Green, Brushes.Black, Brushes.Gray, Brushes.Yellow, Brushes.Aqua };

        /// <summary>
        /// Colours used to paint vertices which are being interacted with
        /// </summary>
        public Brush VertexGrabColor = Brushes.Blue,
                     VertexEdgeColor = Brushes.Red;

        /// <summary>
        /// Colour used to paint edges
        /// </summary>
        public Pen EdgeColor = Pens.Red;

        /// <summary>
        /// Color used to paint text
        /// </summary>
        public Brush TextColor = Brushes.Black;

        /// <summary>
        /// World size of a vertex at normal zoom. Vertex search radius is (Radius),
        /// edge search radius is (Radius / 2)
        /// </summary>
        public int Radius = 15;

        /// <summary>
        /// Spacing between minor gridlines
        /// </summary>
        public const int GridlineSpacing = 50;

        /// <summary>
        /// Spacing between major gridlines
        /// </summary>
        public const int GridlineMult = 5;

        /// <summary>
        /// Color used to render gridlines
        /// </summary>
        public Pen GridlineMinorColor  = Pens.LightGray,
                   GridlineMajorColor  = Pens.Black,
                   GridlineOriginColor = Pens.Blue;
    }
}
