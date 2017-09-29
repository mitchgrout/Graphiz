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
        class Vertex
        {
            /// <summary>
            /// Internal counter used to assign each vertex a unique ID
            /// </summary>
            protected static int nextID = 0;

            /// <summary>
            /// Unique ID assigned to each vertex
            /// </summary>
            public readonly int ID;

            /// <summary>
            /// Name to display
            /// </summary>
            public string Name;

            /// <summary>
            /// Position relative to the origin
            /// </summary>
            public Point Location;

            public Vertex(Point Location)
            {
                this.ID = nextID++;
                this.Name = this.ID.ToString();
                this.Location = Location;
            }

            public Vertex(string Name, Point Location)
            {
                this.ID = nextID++;
                this.Name = Name;
                this.Location = Location;
            }
        }

        class Edge
        {
            /// <summary>
            /// The two IDs of the vertices incident with the edge
            /// </summary>
            public readonly int LeftID, RightID;

            public Edge(int LeftID, int RightID)
            {
                this.LeftID = LeftID;
                this.RightID = RightID;
            }
        }


        /// <summary>
        /// Maps Vertex.ID => Vertex; Vertex.ID is guaranteed to be unique
        /// </summary>
        private Dictionary<int, Vertex> vertices = new Dictionary<int, Vertex>();

        /// <summary>
        /// Collection of edges
        /// </summary>
        private HashSet<Edge> edges = new HashSet<Edge>();

        /// <summary>
        /// Radius of all vertices
        /// </summary>
        private const int radius = 15;

        public FormMain()
        {
            InitializeComponent();
            // Sneakily enable double-buffering
            typeof(Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                         .SetValue(this.panelRender, true);

            // Ensure our default tool is the pointer
            // this.buttonPointer.PerformClick();
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
            return vertices.Values
                           .FirstOrDefault(vertex => vertex.Location.Sub(p).NormSq() < r * r);
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
            /*foreach (var cntl in this.toolStripMain.Items)
            {
                var c = cntl as ToolStripButton;
                if (c != null && c != sender && c.CheckOnClick)
                    c.Checked = false;
            }*/
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
            vertices.Clear();
            edges.Clear();
            this.panelRender.Invalidate();
        }

        private void buttonComplement_Click(object sender, EventArgs e)
        {
            var @new = new HashSet<Edge>();
            vertices.Values
                    .Product(vertices.Values)
                    .Where(pair => pair.Left.ID != pair.Right.ID &&
                                   edges.Count(edge => pair.Left.ID == edge.LeftID && pair.Right.ID == edge.RightID ||
                                                       pair.Left.ID == edge.RightID && pair.Right.ID == edge.LeftID) == 0)
                    .Each(pair => @new.Add(new Edge(pair.Left.ID, pair.Right.ID)));
            edges = @new;
            this.panelRender.Invalidate();
        }

        #endregion FUNCS

        #region EVENTS
        private void panelRender_Paint(object sender, PaintEventArgs e)
        {
            var gfx = e.Graphics;
            // using(var gfx = this.panelRender.CreateGraphics())
            {
                gfx.Clear(panelRender.BackColor);

                edges.Select(edge => new Pair<Point, Point>(
                                        vertices[edge.LeftID].Location,
                                        vertices[edge.RightID].Location
                                     ))
                     .Each(pair => gfx.DrawLine(Pens.Red, pair.Left, pair.Right));

                foreach (var vertex in vertices.Values)
                {
                    Brush vertexColor = Brushes.Green;
                    if (grabVertex != null && vertex.ID == grabVertex.ID) vertexColor = Brushes.Blue;
                    if (edgeVertex != null && vertex.ID == edgeVertex.ID) vertexColor = Brushes.Red;
                    gfx.FillEllipse(vertexColor, vertex.Location.X - radius / 2, vertex.Location.Y - radius / 2, radius, radius);
                    gfx.DrawString(vertex.Name, DefaultFont, Brushes.Black, vertex.Location);
                }
            }
        }


        // Used by various tools in mouse events to remember a vertex ID
        private Vertex grabVertex = null,
                       edgeVertex = null;

        private void panelRender_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && toolState == State.Pointer)
            {
                // Grab the nearest vertex
                grabVertex = NearestVertex(e.Location);
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
                            grabVertex = null;
                            this.panelRender.Invalidate();
                            break;

                        // Add a vertex
                        case State.Vertices:
                            var newVertex = new Vertex(e.Location);
                            vertices.Add(newVertex.ID, newVertex);
                            this.panelRender.Invalidate();
                            break;

                        // Add an edge
                        case State.Edges:
                            var selected = NearestVertex(e.Location);
                            if (selected != null)
                            {
                                if (edgeVertex == null)
                                {
                                    edgeVertex = selected;
                                    this.panelRender.Invalidate();
                                }
                                else
                                {
                                    if (edges.Count(edge => edge.LeftID == edgeVertex.ID && edge.RightID == selected.ID ||
                                                            edge.LeftID == selected.ID && edge.RightID == edgeVertex.ID) == 0)
                                    {
                                        edges.Add(new Edge(edgeVertex.ID, selected.ID));
                                        this.panelRender.Invalidate();
                                    }
                                    edgeVertex = null;
                                }
                            }
                            break;

                    }

                    break;

                // Right-click: tool independent
                case MouseButtons.Right:
                // TODO: Context menu

                default:
                    break;
            }
        }

        private void panelRender_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && grabVertex != null)
            {
                grabVertex.Location = e.Location;
                this.panelRender.Invalidate();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.panelRender.Width = this.Width - 16;
            this.panelRender.Height = this.Height - 64;
        }
        #endregion EVENTS
    }
}
