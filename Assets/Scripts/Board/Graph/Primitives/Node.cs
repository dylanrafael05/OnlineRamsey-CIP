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
        {
            ID = id;
        }   

        private Dictionary<Node, Edge> edgesByOpposingID = new();
        private List<List<Edge>> edgesByType = new();

        public IEnumerable<Edge> ConnectedEdges => edgesByOpposingID.Values;
        public IEnumerable<Node> Neighbors => edgesByOpposingID.Keys;
        public int NeighborCount => edgesByOpposingID.Count;
        
        public int ID { get; }
        public float2 Position { get; internal set; }

        public bool IsConnectedTo(Node node) 
        {
            return edgesByOpposingID.ContainsKey(node);
        }

        public Edge EdgeConnectedTo(Node node) 
        {
            if(!IsConnectedTo(node)) return null;
            return edgesByOpposingID[node];
        }

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