using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphiz
{
    /// <summary>
    /// Sole purpose of this form is to display computed information about the given graph.
    /// A reference to the object is not stored.
    /// The form is not updated if the graph is mutated after creation.
    /// </summary>
    public partial class FormGraphInfo : Form
    {
        public FormGraphInfo(Graph graph)
        {
            InitializeComponent();

            this.textBoxOrder.Text = graph.Vertices.Count.ToString();
            this.textBoxSize.Text = graph.Edges.Count.ToString();
            
            this.textBoxDegreeSeq.Text =
                graph.Vertices
                     .Select(vertex => graph.Edges
                                            .Where(edge => edge.LeftID == vertex.Key || edge.RightID == vertex.Key).Count())
                     .OrderByDescending(i => i)
                     .Select(i => i.ToString())
                     .Intersperse(", ")
                     .Wrap("(", ")")
                     .AsString();

            this.textBoxVertices.Text =
                graph.Vertices
                     .Keys
                     .Select(i => i.ToString())
                     .Intersperse(", ")
                     .Wrap("{", "}")
                     .AsString();

            this.richTextBoxEdges.Text =
                graph.Edges
                     .Select(edge => string.Format("{{{0}, {1}}}", graph.Vertices[edge.LeftID].Name, graph.Vertices[edge.RightID].Name))
                     .Intersperse(", ")
                     .Wrap("{", "}")
                     .AsString();
        }
    }
}
