using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Ramsey.Graph
{
    public class Edge 
    {
        public const int NullType = -1;

        internal Edge(Node start, Node end, int type, int id)
        {
            Assert.AreNotEqual(start, end, "Cannot create an edge which connects a node to itself");

            Start = start;
            End = end;
            Type = type;
            ID = id;
        }

        public Node Start { get; }
        public Node End { get; }
        public int Type { get; internal set; }
        public int ID { get;}

        internal Node NodeOpposite(Node node) 
        {
            if(node == Start) return End;
            if(node == End) return Start;

            throw new InvalidOperationException("Cannot find node opposite to a node not related to this edge.");
        }
    }
}