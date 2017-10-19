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
        // TODO: Wrap up the below into a state object in preparation for serialization, etc

        /// <summary>
        /// Array of colours used to paint vertices when they have been assigned a proper vertex colouring
        /// </summary>
        public Brush[] VertexColors = new Brush[] { Brushes.Green, Brushes.Black, Brushes.Red, Brushes.Blue, Brushes.Gray, Brushes.Yellow, Brushes.Aqua };

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
        /// World size of a vertex at normal zoom. Vertex search radius is (Radius),
        /// edge search radius is (Radius / 2)
        /// </summary>
        public int Radius = 15;

        /// <summary>
        /// Spacing between minor gridlines
        /// </summary>
        private const int GridlineMinor = 50;
        
        /// <summary>
        /// Spacing between major gridlines. Must be an integer multiple of gridlineMinor
        /// </summary>
        private const int GridlineMajor = 5 * GridlineMinor;

        /// <summary>
        /// Color used to render gridlines
        /// </summary>
        private Pen GridlineMinorColor  = Pens.LightGray,
                    GridlineMajorColor  = Pens.Black,
                    GridlineOriginColor = Pens.Blue;
        
        // END TODO


        /// <summary>
        /// Current graph
        /// </summary>
        private Graph Graph;

        /// <summary>
        /// Viewport for the current graph
        /// </summary>
        private Viewport View;

        /// <summary>
        /// Vertex manipulation state
        /// </summary>
        private Vertex GrabVertex,
                       EdgeVertex,
                       EraseVertex;

        /// <summary>
        /// Edge manipulation state
        /// </summary>
        private Edge EraseEdge;


        public FormMain()
        {
            InitializeComponent();

            // Sneakily enable double-buffering
            typeof(Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                         .SetValue(this.panelRender, true);
            
            // Ensure our default tool is the pointer
            this.buttonPointer.PerformClick();

            this.Graph = new Graph();
            this.View = new Viewport(-this.panelRender.Width / 2, -this.panelRender.Height / 2, 100);

            // Small 'hack' for mouse-wheel zoom (panels cannot gain focus from users, only via .Focus, .MouseWheel only procs when focused)
            this.panelRender.MouseHover += (o, e) => this.panelRender.Focus();
            this.panelRender.MouseWheel += (o, e) =>
                {
                    if (e.Delta > 0) this.View.Scale = Math.Max(10,   this.View.Scale - 10);
                    if (e.Delta < 0) this.View.Scale = Math.Min(1000, this.View.Scale + 10);
                    this.panelRender.Invalidate();
                };
        }

        #region TOOLS
        /// <summary>
        /// Represents the state of the tool-picker
        /// </summary>
        private enum State
        {
            Pointer,
            Eraser,
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

        private void buttonPointer_Click(object sender, EventArgs e)  { setTool(sender, State.Pointer);  }
        private void buttonEraser_Click(object sender, EventArgs e)   { setTool(sender, State.Eraser);   }
        private void buttonVertices_Click(object sender, EventArgs e) { setTool(sender, State.Vertices); }
        private void buttonEdges_Click(object sender, EventArgs e)    { setTool(sender, State.Edges);    }
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
                 .Where(pair => pair.Left.ID < pair.Right.ID &&
                                Graph.Edges.Count(edge => pair.Left.ID == edge.LeftID && pair.Right.ID == edge.RightID) == 0)
                    .Each(pair => @new.Add(new Edge(pair.Left.ID, pair.Right.ID)));
            Graph.Edges = @new;
            this.panelRender.Invalidate();
        }

        private void buttonColour_Click(object sender, EventArgs e)
        {
            // IMPORTANT: This actually doesn't give the proper vertex colouring
            // Consider V(G) = { 0, ..., 6 }, E(G) = {{0,2},{0,6},{1,3},{2,3},{3,6}}
            // Clearly bipartite so we expect χ(G) = 2, but this algorithm gives χ(G) = 3.
            // TODO: Fix

            int n = this.Graph.Vertices.Count;

            // No vertices to colour
            if (n == 0)
                return;

            // Simple case, no edges => 1-colourable
            if (this.Graph.Edges.Count == 0)
            {
                this.Graph.Vertices.Values.Each(vertex => vertex.Color = 0);
                this.panelRender.Invalidate();
                return;
            }

            // Simple case, all edges => n-colourable
            if (this.Graph.Edges.Count == (n * (n - 1)) / 2)
            {
                this.Graph.Vertices.Values
                    .Zip(Ext.Iota(0, n, 1), (v, k) => new Pair<Vertex, int>(v, k))
                    .Each(pair => pair.Left.Color = pair.Right);
                this.panelRender.Invalidate();
                return;
            }

            // Use a simple backtracking algorithm to try to paint our graph
            // We *could* use some simple heuristics like finding odd/even cycles to reduce
            // the cases, but that can be implemented later on
            int[] vertexIDs = this.Graph.Vertices.Keys.ToArray();

            // Reset colorings
            this.Graph.Vertices.Values.Each(vertex => vertex.Color = -1);

            // Initial choice for colour doesn't matter, so set it to the first colour we have
            this.Graph.Vertices[vertexIDs[0]].Color = 0;

            // State for backtracking
            Stack<Queue<int>> state = new Stack<Queue<int>>();
            // Minor offset
            state.Push(null);

            for (int k = 1; k < n; k++)
            {
                int vID = vertexIDs[k];

                // Is this our first run?
                if (state.Count() == k)
                {
                    // Grab all our edge colours
                    var edges = this.Graph.Edges
                                    .Where(edge => edge.LeftID == vID || edge.RightID == vID)
                                    .Select(edge => this.Graph.Vertices[edge.LeftID == vID ? edge.RightID : edge.LeftID].Color)
                                    .Where(c => c >= 0)
                                    .ToArray();

                    // Figure out what colours we can use
                    var choices = Ext.Iota(0, n, 1)
                                     .Where(i => !edges.Contains(i));

                    state.Push(new Queue<int>(choices));
                }

                // Do we have any colours left?
                if (state.Peek().Count() == 0)
                {
                    // Backtrack
                    state.Pop();
                    k -= 2;
                    this.Graph.Vertices[vID].Color = -1;
                    continue;
                }

                // Try the next colour
                this.Graph.Vertices[vID].Color = state.Peek().Dequeue();
            }


            // Redraw
            this.panelRender.Invalidate();

            // Count the chromatic index; technically the number of colours we use, but since we use 
            // lowest colours first, χ(G) = (max colour) + 1
            int χ = this.Graph.Vertices.Values.Max(vertex => vertex.Color) + 1;
            MessageBox.Show("Coloured in " + χ + " colours");
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

            Ext.Iota(origin.X - (origin.X % GridlineMinor), max.X, GridlineMinor)
               .Each(x =>
                {
                    var p = View.FromWorldPos(new Point(x, 0));
                    gfx.DrawLine(x == 0 ? GridlineOriginColor : x % GridlineMajor == 0 ? GridlineMajorColor : GridlineMinorColor,
                                 p.X, 0, p.X, this.Height);
                });

            Ext.Iota(origin.Y - (origin.Y % GridlineMinor), max.Y, GridlineMinor)
               .Each(y =>
                {
                    var p = View.FromWorldPos(new Point(0, y));
                    gfx.DrawLine(y == 0 ? GridlineOriginColor : y % GridlineMajor == 0 ? GridlineMajorColor : GridlineMinorColor,
                                 0, p.Y, this.Width, p.Y);
                });

            // Render each edge
            Graph.Edges
                 .Select(edge => new Pair<Point, Point>(View.FromWorldPos(Graph.Vertices[edge.LeftID].Location),
                                                        View.FromWorldPos(Graph.Vertices[edge.RightID].Location)))
                 .Each(pair => gfx.DrawLine(EdgeColor, pair.Left, pair.Right));

            foreach (var vertex in Graph.Vertices.Values)
            {
                // Choose the colour to use
                Brush vertexColor = this.VertexColors[vertex.Color];
                if (GrabVertex != null && vertex.ID == GrabVertex.ID) vertexColor = VertexGrabColor;
                if (EdgeVertex != null && vertex.ID == EdgeVertex.ID) vertexColor = VertexEdgeColor;
                
                // Transform to real coordinates
                var pos = View.FromWorldPos(vertex.Location);
                int rad = View.Rescale(Radius);

                // Draw the vertex itself
                gfx.FillEllipse(vertexColor, pos.X - rad / 2, pos.Y - rad / 2, rad, rad);
                
                // Draw the name just below
                var dims = gfx.MeasureString(vertex.Name, DefaultFont);
                gfx.DrawString(vertex.Name, DefaultFont, Brushes.Black, pos.X - dims.Width / 2, pos.Y + View.Rescale(Radius));
            }
        }

        private void panelRender_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && toolState == State.Pointer)
            {
                // Grab the nearest vertex
                GrabVertex = this.Graph.NearestVertex(View.ToWorldPos(e.Location), Radius);
                this.panelRender.Invalidate();
            }
            
        }

        private void panelRender_MouseUp(object sender, MouseEventArgs e)
        {
            Vertex selectedVertex = this.Graph.NearestVertex(View.ToWorldPos(e.Location), Radius);
            Edge selectedEdge     = this.Graph.NearestEdge(View.ToWorldPos(e.Location), Radius / 2);

            switch (e.Button)
            {
                // Left-click: tool dependent
                case MouseButtons.Left:
                    switch (toolState)
                    {
                        // Release the grabbed vertex
                        case State.Pointer:
                            GrabVertex = null;
                            this.panelRender.Invalidate();
                            break;

                        case State.Eraser:
                            if(selectedVertex != null)
                            {
                                // Remove all associated edges
                                this.Graph.Edges.RemoveWhere(edge => edge.LeftID == selectedVertex.ID || edge.RightID == selectedVertex.ID);
                                this.Graph.Vertices.Remove(selectedVertex.ID);
                                // Unset selected vertices if we removed it
                                if (this.GrabVertex == selectedVertex) this.GrabVertex = null;
                                if (this.EdgeVertex == selectedVertex) this.EdgeVertex = null;
                                this.panelRender.Invalidate();
                            }
                            else if(selectedEdge != null)
                            {
                                // Remove only the relevant edge
                                this.Graph.Edges.Remove(selectedEdge);
                                this.panelRender.Invalidate();
                            }
                            break;

                        // Add a vertex
                        case State.Vertices:
                            var newVertex = new Vertex(View.ToWorldPos(e.Location));
                            Graph.Vertices.Add(newVertex.ID, newVertex);
                            this.panelRender.Invalidate();
                            break;

                        // Add an edge
                        case State.Edges:
                            if (selectedVertex != null)
                            {
                                if (EdgeVertex == null)
                                {
                                    EdgeVertex = selectedVertex;
                                    this.panelRender.Invalidate();
                                }
                                else
                                {
                                    if (Graph.Edges.Count(edge => edge.LeftID == EdgeVertex.ID && edge.RightID == selectedVertex.ID ||
                                                                  edge.LeftID == selectedVertex.ID && edge.RightID == EdgeVertex.ID) == 0)
                                    {
                                        Graph.Edges.Add(new Edge(EdgeVertex.ID, selectedVertex.ID));
                                        this.panelRender.Invalidate();
                                    }
                                    EdgeVertex = null;
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
            switch (toolState)
            {
                case State.Pointer:
                    if (e.Button == MouseButtons.Left)
                    {
                        // Move a vertex
                        if (GrabVertex != null)
                        {
                            var newLocation = View.ToWorldPos(e.Location);
                            if (GrabVertex.Location != newLocation)
                            {
                                GrabVertex.Location = newLocation;
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
                    break;
            }
            oldMouseLocation = e.Location;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            this.panelRender.Width = this.Width - 16;
            this.panelRender.Height = this.Height - 64;
        }
        #endregion EVENTS
    }
}
