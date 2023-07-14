using System.Collections.Generic;
using Unity.Mathematics;

namespace Ramsey.Graph
{
    /// <summary>
    /// Holds all of the information relating to a graph,
    /// including its nodes and edges.
    /// 
    /// Also contains helper methods to analyze the graph.
    /// </summary>
    public interface IReadOnlyGraph
    {
        /// <summary>
        /// Get a list of all the nodes in this graph.
        /// </summary>
        IReadOnlyList<Node> Nodes {get;}
        /// <summary>
        /// Get a list of all the edges in this graph.
        /// </summary>
        IReadOnlyList<Edge> Edges {get;}

        /// <summary>
        /// Get the number of types present in the graph.
        /// </summary>
        int NumTypes { get; }

        /// <summary>
        /// Determine if there exists a valid edge connecting 
        /// the two given nodes.
        /// </summary>
        bool IsValidEdge(Node start, Node end);
        /// <summary>
        /// Determine if this graph is a complete graph.
        /// </summary>
        bool IsComplete();
        /// <summary>
        /// Determine if this graph is a complete graph and
        /// contains no uncolored edges.
        /// </summary>
        bool IsCompleteColored();

        /// <summary>
        /// Get the node in this graph with the given ID.
        /// </summary>
        Node NodeFromID(int id);
        /// <summary>
        /// Get the edge in this graph with the given ID.
        /// </summary>
        Edge EdgeFromID(int id);
    }
}