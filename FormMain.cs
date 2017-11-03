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
        /// Current graph
        /// </summary>
        private Graph CurrentGraph;

        /// <summary>
        /// Viewport for the current graph
        /// </summary>
        private Viewport View;

        /// <summary>
        /// Vertex manipulation state
        /// </summary>
        private Vertex GrabVertex,
                       EdgeVertex;

        /// <summary>
        /// Edge manipulation state
        /// </summary>
        // private Edge EraseEdge;


        public FormMain()
        {
            InitializeComponent();
            
            // Sneakily enable double-buffering
            typeof(Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                         .SetValue(this.panelRender, true);
            
            // Ensure our default tool is the pointer
            this.buttonPointer.PerformClick();

            this.CurrentGraph = new Graph();
            this.View = new Viewport(-this.panelRender.Width / 2, -this.panelRender.Height / 2, 100);
            
            // Small hack for mouse-wheel zoom (panels cannot gain focus from users, only via .Focus, and .MouseWheel only procs when focused)
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
        private enum GraphTool
        {
            Pointer,
            Eraser,
            Vertices,
            Edges
        }
        private GraphTool SelectedTool = GraphTool.Pointer;

        /// <summary>
        /// Utility function for the tool-picker
        /// </summary>
        private void SetTool(object sender, GraphTool tool)
        {
            SelectedTool = tool;
            foreach (var cntl in this.toolStripMain.Items)
            {
                var c = cntl as ToolStripButton;
                if (c != null && c != sender && c.CheckOnClick)
                    c.Checked = false;
            }
            (sender as ToolStripButton).Checked = true;
        }

        private void buttonPointer_Click(object sender, EventArgs e)  { SetTool(sender, GraphTool.Pointer);  }
        private void buttonEraser_Click(object sender, EventArgs e)   { SetTool(sender, GraphTool.Eraser);   }
        private void buttonVertices_Click(object sender, EventArgs e) { SetTool(sender, GraphTool.Vertices); }
        private void buttonEdges_Click(object sender, EventArgs e)    { SetTool(sender, GraphTool.Edges);    }
        #endregion TOOLS

        #region FUNCS
        private void buttonClear_Click(object sender, EventArgs e)
        {
            CurrentGraph.Reset();
            this.panelRender.Invalidate();
        }

        private void buttonComplement_Click(object sender, EventArgs e)
        {
            var @new = new HashSet<Edge>();
            CurrentGraph.Vertices
                 .Values
                 .Product(CurrentGraph.Vertices.Values)
                 .Where(pair => pair.Left.ID < pair.Right.ID)
                 .Select(pair => new Edge(pair.Left.ID, pair.Right.ID))
                 .Where(edge => !CurrentGraph.Edges.Contains(edge))
                 .Each(edge => @new.Add(edge));
            CurrentGraph.Edges = @new;
            this.panelRender.Invalidate();
        }

        private void buttonColour_Click(object sender, EventArgs e)
        {
            // IMPORTANT: This actually doesn't give the proper vertex colouring
            // Consider V(G) = { 0, ..., 6 }, E(G) = {{0,2},{0,6},{1,3},{2,3},{3,6}}
            // Clearly bipartite so we expect χ(G) = 2, but this algorithm gives χ(G) = 3.
            // TODO: Fix

            int n = this.CurrentGraph.Vertices.Count;

            // No vertices to colour
            if (n == 0)
                return;

            // Simple case, no edges => 1-colourable
            else if (this.CurrentGraph.Edges.Count == 0)
            {
                this.CurrentGraph.Vertices.Values.Each(vertex => vertex.Color = 0);
            }

            // Simple case, all edges => n-colourable
            else if (this.CurrentGraph.Edges.Count == (n * (n - 1)) / 2)
            {
                this.CurrentGraph.Vertices.Values
                    .Zip(Ext.Iota(0, n, 1), (v, k) => new Pair<Vertex, int>(v, k))
                    .Each(pair => pair.Left.Color = pair.Right);
            }

            else
            {
                // Use a simple backtracking algorithm to try to paint our graph
                // We *could* use some simple heuristics like finding odd/even cycles to reduce
                // the cases, but that can be implemented later on
                int[] vertexIDs = this.CurrentGraph.Vertices.Keys.ToArray();

                // Reset colorings
                this.CurrentGraph.Vertices.Values.Each(vertex => vertex.Color = -1);

                // Initial choice for colour doesn't matter, so set it to the first colour we have
                this.CurrentGraph.Vertices[vertexIDs[0]].Color = 0;

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
                        var edges = this.CurrentGraph.Edges
                                        .Where(edge => edge.LeftID == vID || edge.RightID == vID)
                                        .Select(edge => this.CurrentGraph.Vertices[edge.LeftID == vID ? edge.RightID : edge.LeftID].Color)
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
                        this.CurrentGraph.Vertices[vID].Color = -1;
                        continue;
                    }

                    // Try the next colour
                    this.CurrentGraph.Vertices[vID].Color = state.Peek().Dequeue();
                }
            }

            // Redraw
            this.panelRender.Invalidate();

            // Count the chromatic index; technically the number of colours we use, but since we use 
            // lowest colours first, χ(G) = (max colour) + 1
            int χ = this.CurrentGraph.Vertices.Values.Max(vertex => vertex.Color) + 1;
            MessageBox.Show("Coloured in " + χ + " colours");
        }

        private void buttonClassify_Click(object sender, EventArgs e)
        {
            // Spawn a new window which takes a reference to our graph.
            // The reference is not stored
            (new FormGraphInfo(CurrentGraph)).Show();
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
            Func<int, Pen> selector = k => k == 0 ? GlobalState.Instance.GridlineOriginColor : 
                                           k % (GlobalState.GridlineMult * GlobalState.GridlineSpacing) == 0 ? GlobalState.Instance.GridlineMajorColor :
                                           GlobalState.Instance.GridlineMinorColor;

            foreach (var x in Ext.Iota(origin.X - (origin.X % GlobalState.GridlineSpacing), max.X, GlobalState.GridlineSpacing))
            {
                var p = View.FromWorldPos(new Point(x, 0));
                gfx.DrawLine(selector(x), p.X, 0, p.X, this.Height);
            }

            foreach(var y in Ext.Iota(origin.Y - (origin.Y % GlobalState.GridlineSpacing), max.Y, GlobalState.GridlineSpacing))
            {
                var p = View.FromWorldPos(new Point(0, y));
                gfx.DrawLine(selector(y), 0, p.Y, this.Width, p.Y);
            }

            // Render each edge
            foreach(var edge in CurrentGraph.Edges)
                gfx.DrawLine(GlobalState.Instance.EdgeColor, View.FromWorldPos(CurrentGraph.Vertices[edge.LeftID].Location), 
                                                             View.FromWorldPos(CurrentGraph.Vertices[edge.RightID].Location));
                       
            foreach (var vertex in CurrentGraph.Vertices.Values)
            {
                // Choose the colour to use
                Brush vertexColor = GlobalState.Instance.VertexColors[vertex.Color];
                if (GrabVertex != null && vertex.ID == GrabVertex.ID) vertexColor = GlobalState.Instance.VertexGrabColor;
                if (EdgeVertex != null && vertex.ID == EdgeVertex.ID) vertexColor = GlobalState.Instance.VertexEdgeColor;
                
                // Transform to real coordinates
                var pos = View.FromWorldPos(vertex.Location);
                int rad = View.Rescale(GlobalState.Instance.Radius);

                // Draw the vertex itself
                gfx.FillEllipse(vertexColor, pos.X - rad / 2, pos.Y - rad / 2, rad, rad);
                
                // Draw the name just below
                var dims = gfx.MeasureString(vertex.Name, DefaultFont);
                gfx.DrawString(vertex.Name, DefaultFont, GlobalState.Instance.TextColor, pos.X - dims.Width / 2, pos.Y + rad);
            }
        }

        private void panelRender_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && SelectedTool == GraphTool.Pointer)
            {
                // Grab the nearest vertex
                GrabVertex = this.CurrentGraph.NearestVertex(View.ToWorldPos(e.Location), GlobalState.Instance.Radius);
                this.panelRender.Invalidate();
            }
        }

        private void panelRender_MouseUp(object sender, MouseEventArgs e)
        {
            Vertex selectedVertex = this.CurrentGraph.NearestVertex(View.ToWorldPos(e.Location), GlobalState.Instance.Radius);
            Edge selectedEdge = this.CurrentGraph.NearestEdge(View.ToWorldPos(e.Location), GlobalState.Instance.Radius / 2);

            switch (e.Button)
            {
                // Left-click: tool dependent
                case MouseButtons.Left:
                    switch (SelectedTool)
                    {
                        // Release the grabbed vertex
                        case GraphTool.Pointer:
                            GrabVertex = null;
                            this.panelRender.Invalidate();
                            break;

                        case GraphTool.Eraser:
                            if(selectedVertex != null)
                            {
                                // Remove all associated edges
                                this.CurrentGraph.Edges.RemoveWhere(edge => edge.LeftID == selectedVertex.ID || edge.RightID == selectedVertex.ID);
                                this.CurrentGraph.Vertices.Remove(selectedVertex.ID);
                                // Unset selected vertices if we removed it
                                if (this.GrabVertex == selectedVertex) this.GrabVertex = null;
                                if (this.EdgeVertex == selectedVertex) this.EdgeVertex = null;
                                this.panelRender.Invalidate();
                            }
                            else if(selectedEdge != null)
                            {
                                // Remove only the relevant edge
                                this.CurrentGraph.Edges.Remove(selectedEdge);
                                this.panelRender.Invalidate();
                            }
                            break;

                        // Add a vertex
                        case GraphTool.Vertices:
                            var newVertex = new Vertex(View.ToWorldPos(e.Location));
                            CurrentGraph.Vertices.Add(newVertex.ID, newVertex);
                            this.panelRender.Invalidate();
                            break;

                        // Add an edge
                        case GraphTool.Edges:
                            if (selectedVertex != null)
                            {
                                if (EdgeVertex == null)
                                {
                                    EdgeVertex = selectedVertex;
                                    this.panelRender.Invalidate();
                                }
                                else
                                {
                                    if (CurrentGraph.Edges.Count(edge => edge.LeftID == EdgeVertex.ID && edge.RightID == selectedVertex.ID ||
                                                                  edge.LeftID == selectedVertex.ID && edge.RightID == EdgeVertex.ID) == 0)
                                    {
                                        CurrentGraph.Edges.Add(new Edge(EdgeVertex.ID, selectedVertex.ID));
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

        private Point _oldMouseLocation = Point.Empty;
        private void panelRender_MouseMove(object sender, MouseEventArgs e)
        {
            switch (SelectedTool)
            {
                case GraphTool.Pointer:
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
                            int dx = e.X - _oldMouseLocation.X,
                                dy = e.Y - _oldMouseLocation.Y;
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
            _oldMouseLocation = e.Location;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            this.panelRender.Width = this.Width - 16;
            this.panelRender.Height = this.Height - 64;
        }
        #endregion EVENTS

    }
}
