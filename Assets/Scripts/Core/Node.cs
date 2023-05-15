using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        public int ID { get; }

        internal void AddEdge(Edge edge)
        {
            var oppositeNode = edge.NodeOpposite(this);
            Assert.IsFalse(edgesByOpposingID.ContainsKey(oppositeNode), "Cannot connect an edge twice");

            edgesByOpposingID[oppositeNode] = edge;
        }
    }
}