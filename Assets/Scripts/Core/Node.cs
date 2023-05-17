using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Mathematics;

namespace Ramsey.Core
{
    public class Node
    {
        internal Node(int id) 
        {
            ID = id;
        }   

        private Dictionary<Node, Edge> edgesByOpposingID = new();

        public IEnumerable<Edge> Edges => edgesByOpposingID.Values;
        public IEnumerable<Node> Neighbors => edgesByOpposingID.Keys;
        
        public int ID { get; }
        public float2 Position { get; internal set; }

        public bool IsConnectedTo(Node node) 
        {
            return edgesByOpposingID.ContainsKey(node);
        }

        internal void RegisterToEdge(Edge edge)
        {
            var oppositeNode = edge.NodeOpposite(this);
            Assert.IsFalse(edgesByOpposingID.ContainsKey(oppositeNode), "Cannot connect an edge twice");

            edgesByOpposingID[oppositeNode] = edge;
        }
    }
}