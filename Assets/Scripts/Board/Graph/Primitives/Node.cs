using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Mathematics;
using System;
using System.Linq;

namespace Ramsey.Graph
{
    public class Node
    {
        internal Node(int id) 
            => ID = id;

        private readonly Dictionary<Node, Edge> edgesByOpposingID = new();
        private readonly List<List<Edge>> edgesByType = new();

        /// <summary>
        /// A sequence of edges connected to this node.
        /// </summary>
        public IEnumerable<Edge> ConnectedEdges => edgesByOpposingID.Values;
        /// <summary>
        /// A sequence of neighbor nodes connected to this node.
        /// </summary>
        public IEnumerable<Node> Neighbors => edgesByOpposingID.Keys;
        /// <summary>
        /// How many edges are connected to this node?
        /// </summary>
        public int NeighborCount => edgesByOpposingID.Count;
        
        /// <summary>
        /// Unique Node identifier that distinguishes it from other nodes.
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// Only has influence on graphics.
        /// </summary>
        public float2 Position { get; internal set; }

        public bool IsConnectedTo(Node node) 
        {
            return edgesByOpposingID.ContainsKey(node);
        }

        /// <summary>
        /// If the node you pass in is a neighbour of this node, return the edge between them. If not, return null.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Edge EdgeConnectedTo(Node node) 
        {
            if(!IsConnectedTo(node)) return null;
            return edgesByOpposingID[node];
        }

        /// <summary>
        /// If the node you pass in is a neighbour of this node, return the edge between them. If not, return null.
        /// The directed edge only has influence on the path highlight graphics.  It simply makes sure all the edges are moving in the same direction.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public DirectedEdge? DirectedEdgeConnectedTo(Node node) 
        {
            if(!IsConnectedTo(node)) return null;
            return new(edgesByOpposingID[node], edgesByOpposingID[node].End == this);
        }

        /// <summary>
        /// Sequence of all edges that are of the type you pass in.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<Edge> ConnectedEdgesOfType(int type)
            => type < edgesByType.Count ? edgesByType[type] : Enumerable.Empty<Edge>();

        internal void RegisterToEdge(Edge edge)
        {
            var oppositeNode = edge.NodeOpposite(this);
            Assert.IsFalse(edgesByOpposingID.ContainsKey(oppositeNode), "Cannot connect an edge twice");

            edgesByOpposingID[oppositeNode] = edge;
        }

        internal void PaintEdge(Edge edge)
        {
            while(edge.Type >= edgesByType.Count)
            {
                edgesByType.Add(new());
            }

            edgesByType[edge.Type].Add(edge);
        }
    }
}