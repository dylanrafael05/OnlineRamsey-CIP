﻿using Ramsey.Board;
using Ramsey.Graph;
using Ramsey.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    public static class BuilderUtils
    {
        /// <summary>
        /// Takes in a reference to a node object and extends it. 
        /// By extending it, I mean just creating a new node and connecting it to the node you gave.
        /// The node reference you gave to this function will then refer to the new node created.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static BuilderMove Extend(ref Node n1, GameState state)
        {
            var n1o = n1;
            
            var n2 = state.CreateNode();
            n1 = n2;

            var move = new BuilderMove(n1o, n2);

            return move;
        }

        /// <summary>
        /// Creates a new node and connects it to the node passed into this function.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static BuilderMove Extend(Node n1, GameState state)
            => new(n1, state.CreateNode());

        /// <summary>
        /// Get a random node in the graph that isn't connected to the 'other' node parameter.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Node RandomNode(Node other, GameState state)
        {
            IReadOnlyList<Node> nodes = other == null ? state.Nodes : state.Nodes.Where(n => !n.Neighbors.Contains(other) && other != n).ToList();
            if (nodes.Count == 0) { state.CreateNode(); return RandomNode(other, state); }
            return nodes[UnityEngine.Random.Range(0, nodes.Count)];
        }

        /// <summary>
        /// Get a pair of two random nodes in the graph as a Builder move.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static BuilderMove RandomPair(GameState state)
        {
            var n1 = RandomNode(state);
            var n2 = RandomNode(n1, state);
            return new(n1, n2);
        }

        /// <summary>
        /// Get a random node in the graph.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Node RandomNode(GameState state) => RandomNode(null, state);
    }
}