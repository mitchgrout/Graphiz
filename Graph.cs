using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphiz
{
    class Graph
    {
        /// <summary>
        /// Maps Vertex.ID => Vertex; Vertex.ID is guaranteed to be unique
        /// </summary>
        public Dictionary<int, Vertex> Vertices = new Dictionary<int, Vertex>();

        /// <summary>
        /// Collection of edges
        /// </summary>
        public HashSet<Edge> Edges = new HashSet<Edge>();

        /// <summary>
        /// Graph manipulation state
        /// </summary>
        public Vertex GrabVertex,
                      EdgeVertex;

        /// <summary>
        /// Reset a graph object without explicitly calling new
        /// </summary>
        public void Reset()
        {
            this.Vertices.Clear();
            this.Edges.Clear();
            this.GrabVertex = null;
            this.EdgeVertex = null;
        }

        public Vertex NearestVertex(Point p, int radius)
        {
            return Vertices.Values
                           .FirstOrDefault(vertex => vertex.Location
                                                           .Sub(p)
                                                           .NormSq() < radius * radius);
        }

        public Edge NearestEdge(Point p, int radius)
        {
            return null;
        }
    }

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
}
