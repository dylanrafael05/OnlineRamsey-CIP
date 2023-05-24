using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Mathematics;
using System;

namespace Ramsey.Graph
{
    public class Node
    {
        internal Node(int id) 
        {
            ID = id;
        }   

        private Dictionary<Node, Edge> edgesByOpposingID = new();

        public IEnumerable<Edge> ConnectedEdges => edgesByOpposingID.Values;
        public IEnumerable<Node> Neighbors => edgesByOpposingID.Keys;
        
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

        internal void RegisterToEdge(Edge edge)
        {
            var oppositeNode = edge.NodeOpposite(this);
            Assert.IsFalse(edgesByOpposingID.ContainsKey(oppositeNode), "Cannot connect an edge twice");

            edgesByOpposingID[oppositeNode] = edge;
        }
    }
}