using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Graphiz
{
    partial class FormMain : Form
    {
        /// <summary>
        /// Radius of all vertices
        /// </summary>
        private const int radius = 15;

        /// <summary>
        /// Spacing between minor gridlines
        /// </summary>
        private const int gridlineMinor = 50;
        
        /// <summary>
        /// Spacing between major gridlines. Must be an integer multiple of gridlineMinor
        /// </summary>
        private const int gridlineMajor = 5 * gridlineMinor;

        /// <summary>
        /// Color used to render gridlines
        /// </summary>
        private Pen gridlineMinorColor = Pens.LightGray,
                    gridlineMajorColor = Pens.Black,
                    gridlineOrigin     = Pens.Blue;

        /// <summary>
        /// Current graph
        /// </summary>
        private Graph Graph;

        /// <summary>
        /// Viewport for the current graph
        /// </summary>
        private Viewport View;

        public FormMain()
        {
            InitializeComponent();
            // Sneakily enable double-buffering
            typeof(Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                         .SetValue(this.panelRender, true);
            
            // Ensure our default tool is the pointer
            this.buttonPointer.PerformClick();

            this.Graph = new Graph();
            this.View = new Viewport(-this.Width / 2, -this.Height / 2, 100);

            this.panelRender.MouseWheel += (o, e) =>
                {
                    if (e.Delta > 0) this.View.Scale = Math.Max(10,   this.View.Scale - 10);
                    if (e.Delta < 0) this.View.Scale = Math.Min(1000, this.View.Scale + 10);
                    this.panelRender.Invalidate();
                };
        }

        #region UTIL
        /// <summary>
        /// Find a vertex within r units of p
        /// </summary>
        /// <returns>
        /// Null if there is no point within r of p, otherwise the first point satisfying the condition
        /// </returns>
        private Vertex NearestVertex(Point p, int r = radius)
        {
            return Graph.Vertices
                        .Values
                        .FirstOrDefault(vertex => vertex.Location
                                                        .Sub(p)
                                                        .NormSq() < r * r);
        }
        #endregion

        #region TOOLS
        /// <summary>
        /// Represents the state of the tool-picker
        /// </summary>
        private enum State
        {
            Pointer,
            Vertices,
            Edges
        }
        private State toolState = State.Pointer;

        /// <summary>
        /// Utility function for the tool-picker
        /// </summary>
        private void setTool(object sender, State state)
        {
            toolState = state;
            foreach (var cntl in this.toolStripMain.Items)
            {
                var c = cntl as ToolStripButton;
                if (c != null && c != sender && c.CheckOnClick)
                    c.Checked = false;
            }
            (sender as ToolStripButton).Checked = true;
        }

        private void buttonPointer_Click(object sender, EventArgs e)
        {
            setTool(sender, State.Pointer);
        }

        private void buttonVertices_Click(object sender, EventArgs e)
        {
            setTool(sender, State.Vertices);
        }

        private void buttonEdges_Click(object sender, EventArgs e)
        {
            setTool(sender, State.Edges);
        }
        #endregion TOOLS

        #region FUNCS
        private void buttonClear_Click(object sender, EventArgs e)
        {
            Graph.Reset();
            this.panelRender.Invalidate();
        }

        private void buttonComplement_Click(object sender, EventArgs e)
        {
            var @new = new HashSet<Edge>();
            Graph.Vertices
                 .Values
                 .Product(Graph.Vertices.Values)
                 .Where(pair => pair.Left.ID != pair.Right.ID &&
                                Graph.Edges.Count(edge => pair.Left.ID == edge.LeftID && pair.Right.ID == edge.RightID ||
                                                          pair.Left.ID == edge.RightID && pair.Right.ID == edge.LeftID) == 0)
                    .Each(pair => @new.Add(new Edge(pair.Left.ID, pair.Right.ID)));
            Graph.Edges = @new;
            this.panelRender.Invalidate();
        }

        #endregion FUNCS

        #region EVENTS
        private void panelRender_Paint(object sender, PaintEventArgs e)
        {
            var gfx = e.Graphics;

            gfx.Clear(panelRender.BackColor);

            // Render gridlines (minor + major)
            Point origin = View.ToWorldPos(Point.Empty),
                  max    = View.ToWorldPos(new Point(this.Size));

            Ext.Iota(origin.X - (origin.X % gridlineMinor), max.X, gridlineMinor)
               .Each(x =>
                {
                    var p = View.FromWorldPos(new Point(x, 0));
                    gfx.DrawLine(x == 0 ? gridlineOrigin : x % gridlineMajor == 0 ? gridlineMajorColor : gridlineMinorColor,
                                 p.X, 0, p.X, this.Height);
                });

            Ext.Iota(origin.Y - (origin.Y % gridlineMinor), max.Y, gridlineMinor)
               .Each(y =>
                {
                    var p = View.FromWorldPos(new Point(0, y));
                    gfx.DrawLine(y == 0 ? gridlineOrigin : y % gridlineMajor == 0 ? gridlineMajorColor : gridlineMinorColor,
                                 0, p.Y, this.Width, p.Y);
                });

            Graph.Edges
                 .Select(edge => new Pair<Point, Point>(
                                    View.FromWorldPos(Graph.Vertices[edge.LeftID].Location),
                                    View.FromWorldPos(Graph.Vertices[edge.RightID].Location)
                                 ))
                 .Each(pair => gfx.DrawLine(Pens.Red, pair.Left, pair.Right));

            foreach (var vertex in Graph.Vertices.Values)
            {
                Brush vertexColor = Brushes.Green;
                if (Graph.GrabVertex != null && vertex.ID == Graph.GrabVertex.ID) vertexColor = Brushes.Blue;
                if (Graph.EdgeVertex != null && vertex.ID == Graph.EdgeVertex.ID) vertexColor = Brushes.Red;
                var pos = View.FromWorldPos(vertex.Location);
                gfx.FillEllipse(vertexColor, pos.X - radius / 2, pos.Y - radius / 2, radius, radius);
                gfx.DrawString(vertex.Name, DefaultFont, Brushes.Black, pos);
            }

        }

        private void panelRender_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && toolState == State.Pointer)
            {
                // Grab the nearest vertex
                Graph.GrabVertex = NearestVertex(View.ToWorldPos(e.Location));
                this.panelRender.Invalidate();
            }
            
        }

        private void panelRender_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                // Left-click: tool dependent
                case MouseButtons.Left:
                    switch (toolState)
                    {
                        // Release the grabbed vertex
                        case State.Pointer:
                            Graph.GrabVertex = null;
                            this.panelRender.Invalidate();
                            break;

                        // Add a vertex
                        case State.Vertices:
                            var newVertex = new Vertex(View.ToWorldPos(e.Location));
                            Graph.Vertices.Add(newVertex.ID, newVertex);
                            this.panelRender.Invalidate();
                            break;

                        // Add an edge
                        case State.Edges:
                            var selected = NearestVertex(View.ToWorldPos(e.Location));
                            if (selected != null)
                            {
                                if (Graph.EdgeVertex == null)
                                {
                                    Graph.EdgeVertex = selected;
                                    this.panelRender.Invalidate();
                                }
                                else
                                {
                                    if (Graph.Edges.Count(edge => edge.LeftID == Graph.EdgeVertex.ID && edge.RightID == selected.ID ||
                                                                  edge.LeftID == selected.ID && edge.RightID == Graph.EdgeVertex.ID) == 0)
                                    {
                                        Graph.Edges.Add(new Edge(Graph.EdgeVertex.ID, selected.ID));
                                        this.panelRender.Invalidate();
                                    }
                                    Graph.EdgeVertex = null;
                                }
                            }
                            break;

                    }
                    break;

                // Right-click: tool independent
                case MouseButtons.Right:
                    // TODO: Context menu
                    break;

                default:
                    break;
            }
        }

        private Point oldMouseLocation = Point.Empty;
        private void panelRender_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && toolState == State.Pointer)
            {
                // Move a vertex
                if (Graph.GrabVertex != null)
                {
                    var newLocation = View.ToWorldPos(e.Location);
                    if (Graph.GrabVertex.Location != newLocation)
                    {
                        Graph.GrabVertex.Location = newLocation;
                        this.panelRender.Invalidate();
                    }
                }
                // Move the viewport
                else
                {
                    int dx = e.X - oldMouseLocation.X,
                        dy = e.Y - oldMouseLocation.Y;
                    if (dx != 0 || dy != 0)
                    {
                        this.View.Origin.X -= dx;
                        this.View.Origin.Y -= dy;
                        this.panelRender.Invalidate();
                    }
                }
            }
            oldMouseLocation = e.Location;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.panelRender.Width = this.Width - 16;
            this.panelRender.Height = this.Height - 64;
        }
        #endregion EVENTS

        private void panelRender_MouseHover(object sender, EventArgs e)
        {
            this.panelRender.Focus();
        }
    }
}
