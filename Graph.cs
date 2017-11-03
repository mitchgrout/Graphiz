using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphiz
{
    public class Graph
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
        /// Reset a graph object without explicitly calling new
        /// </summary>
        public void Reset()
        {
            this.Vertices.Clear();
            this.Edges.Clear();
        }

        /// <summary>
        /// Returns a vertex which is within radius world units of the given point.
        /// Remembers the last returned result, which is returned so long as it is still
        /// within radius world units of the given point.
        /// </summary>
        public Vertex NearestVertex(Point p, int r)
        {
            if (_oldVertex != null && Vertices.ContainsKey(_oldVertex.ID) && closeToVertex(_oldVertex, p, r)) return _oldVertex;
            else return (_oldVertex = Vertices.Values.FirstOrDefault(vertex => closeToVertex(vertex, p, r)));
        }
        private Vertex _oldVertex;

        /// <summary>
        /// Returns an edge which is within radius world units of the given point.
        /// Remembers the last returned result, which is returned so long as it is still
        /// within radius world units of the given edge.
        /// </summary>
        public Edge NearestEdge(Point p, int r)
        {
            if (_oldEdge != null && Edges.Contains(_oldEdge) && closeToEdge(_oldEdge, p, r)) return _oldEdge;
            else return (_oldEdge = Edges.FirstOrDefault(edge => closeToEdge(edge, p, r)));
        }
        private Edge _oldEdge;

        private bool closeToVertex(Vertex v, Point p, int r)
        {
            return v.Location.Sub(p).NormSquare() < r * r;
        }

        private bool closeToEdge(Edge e, Point p, int r)
        {
            Point P1 = Vertices[e.LeftID].Location,
                  P2 = Vertices[e.RightID].Location;
            return Math.Abs((P2.Y - P1.Y) * p.X - (P2.X - P1.X) * p.Y + (P2.X * P1.Y) - (P2.Y * P1.X)) / P2.Sub(P1).Norm() < r;
        }
    }

    public class Vertex
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

        /// <summary>
        /// Colour of a vertex; the colour to graphically paint it is given by Graph.VertexColors[Vertex.Color]
        /// </summary>
        public int Color;

        public Vertex(Point Location)
        {
            this.ID       = nextID++;
            this.Name     = this.ID.ToString();
            this.Location = Location;
            this.Color    = 0;
        }

        public Vertex(string Name, Point Location)
        {
            this.ID       = nextID++;
            this.Name     = Name;
            this.Location = Location;
            this.Color    = 0;
        }

        public static bool operator ==(Vertex lhs, Vertex rhs)
        {
            if (ReferenceEquals(lhs, rhs))  return true;
            if (ReferenceEquals(lhs, null)) return false;
            if (ReferenceEquals(rhs, null)) return false;
            return lhs.ID == rhs.ID;
        }

        public static bool operator !=(Vertex lhs, Vertex rhs)
        {
            return !(lhs == rhs);
        }        
        
        public override bool Equals(object obj)
        {
            if (obj is Vertex) return this == (Vertex) obj;
            else               return false;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }

    public class Edge
    {
        /// <summary>
        /// The two IDs of the vertices incident with the edge.
        /// In all edges, LeftID <= RightID
        /// </summary>
        public readonly int LeftID, RightID;

        public Edge(int LeftID, int RightID)
        {
            this.LeftID = Math.Min(LeftID, RightID);
            this.RightID = Math.Max(LeftID, RightID);
        }

        public static bool operator ==(Edge lhs, Edge rhs)
        {
            if (ReferenceEquals(lhs, rhs))  return true;
            if (ReferenceEquals(lhs, null)) return false;
            if (ReferenceEquals(rhs, null)) return false;
            return lhs.LeftID == rhs.LeftID &&
                   lhs.RightID == rhs.RightID;
        }

        public static bool operator !=(Edge lhs, Edge rhs)
        {
            return !(lhs == rhs);
        }        
        
        public override bool Equals(object obj)
        {
            if (obj is Edge) return this == (Edge)obj;
            else             return false;
        }

        public override int GetHashCode()
        {
            return (LeftID << 8) ^ RightID;
        }
    }
}
