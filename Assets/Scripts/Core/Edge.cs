using System;
using System.Linq;
using UnityEngine.Assertions;

namespace Ramsey.Core
{
    public class Edge 
    {
        internal Edge(Node start, Node end, int type)
        {
            Assert.AreNotEqual(start, end, "Cannot create an edge which connects a node to itself");

            Start = start;
            End = end;
            Type = type;
        }

        public Node Start { get; }
        public Node End { get; }
        public int Type { get; }

        internal Node NodeOpposite(Node node) 
        {
            if(node == Start) return End;
            if(node == End) return Start;

            throw new InvalidOperationException("Cannot find node opposite to a node not related to this edge.");
        }
    }
}