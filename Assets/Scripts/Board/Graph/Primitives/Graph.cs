using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine;
using Ramsey.Utilities;
using System.Linq;

namespace Ramsey.Graph
{
    public class Graph : IGraph
    {
        private readonly List<Node> nodes = new();
        private readonly List<Edge> edges = new();

        public IReadOnlyList<Node> Nodes => nodes;
        public IReadOnlyList<Edge> Edges => edges;

        public int NumTypes { get; private set; } = 0;

        /// <summary>
        /// Get a node from its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Node NodeFromID(int id) 
        {
            // Assert.IsTrue(nodes.Count > id, $"Cannot get a node of id {id} from this graph, it does not yet exist!");

            return nodes[id];
        }
        /// <summary>
        /// Get an edge from its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Edge EdgeFromID(int id) 
        {
            Assert.IsTrue(edges.Count > id, $"Cannot get an edge of id {id} from this graph, it does not yet exist!");

            return edges[id];
        }

        /// <summary>
        /// Check if an edge is valid.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool IsValidEdge(Node start, Node end) 
        {
            if(start == end) return false;
            if(start.IsConnectedTo(end)) return false;

            return true;
        }
        /// <summary>
        /// Check if the Graph is a Complete Graph.
        /// </summary>
        /// <returns></returns>
        public bool IsComplete()
        {
            foreach(var node in nodes)
            {
                foreach(var other in nodes)
                {
                    if(node == other) continue;

                    if(!node.IsConnectedTo(other)) return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Check if the Graph is a Complete Graph and that every edge is colored.
        /// </summary>
        /// <returns></returns>
        public bool IsCompleteColored()
            => IsComplete() && edges.All(e => e.Type != Edge.NullType);

        public Edge CreateEdge(Node start, Node end)
        {
            var edge = new Edge(start, end, Edge.NullType, edges.Count);
            edges.Add(edge);

            start.RegisterToEdge(edge);
            end.RegisterToEdge(edge);

            return edge;
        }

        public Node CreateNode(float2 position = default) 
        {
            var node = new Node(nodes.Count) {Position = position};
            nodes.Add(node);

            return node;
        }

        public void MoveNode(Node node, float2 position)
        {
            node.Position = position;
        }

        /// <summary>
        /// Paint an edge a certain color.  Can't change its color if it's already painted (not NullType).
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="type"></param>
        public void PaintEdge(Edge edge, int type)
        {
            Assert.AreEqual(edge.Type, Edge.NullType, "Cannot change the color of a painted edge!");

            edge.Type = type;

            edge.Start.PaintEdge(edge);
            edge.End.PaintEdge(edge);

            NumTypes = Mathf.Max(NumTypes, type + 1);
        }
        
        public void Clear()
        {
            nodes.Clear();
            edges.Clear();
            NumTypes = 0;
        }

        internal bool Empty => nodes.Count == 0 && edges.Count == 0;
    }
}