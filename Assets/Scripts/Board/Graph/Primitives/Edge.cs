using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        /// <summary>
        /// The node at the start of the edge (arbitrary direction).
        /// </summary>
        public Node Start { get; }
        /// <summary>
        /// The node at the end of the edge (arbitrary direction).
        /// </summary>
        public Node End { get; }
        /// <summary>
        /// -1 - Nulltype
        /// 0 - Blue
        /// 1 - Red
        /// (All Configurable)
        /// </summary>
        public int Type { get; internal set; }
        /// <summary>
        /// Unique identifier that distinguishes it from other edges.
        /// </summary>
        public int ID { get;}

        /// <summary>
        /// If the node is on the edge, it'll get the opposite node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal Node NodeOpposite(Node node) 
        {
            if(node == Start) return End;
            if(node == End) return Start;

            throw new InvalidOperationException("Cannot find node opposite to a node not related to this edge.");
        }

        public override string ToString()
            => $"edge[{ID}] {Start.ID} -> {End.ID}";
    }
}