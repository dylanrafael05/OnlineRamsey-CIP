using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ramsey.Core
{
    public class Node
    {
        internal Node(long id) 
        {
            ID = id;
        }   

        private HashSet<Edge> edges = new HashSet<Edge>();

        public IEnumerable<Edge> Edges => edges;
        public long ID { get; }

        internal void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }
        internal void RemoveEdge(Edge edge)
        {
            edges.Add(edge);
        }
    }
}