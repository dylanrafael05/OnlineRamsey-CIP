using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Ramsey.Graph;
using Ramsey.Utilities;

namespace Ramsey.Board
{
    public class GameState 
    {
        internal GameState() { }

        internal BoardManager Board { get; set; }

        public IReadOnlyList<Node> Nodes => Board.Nodes;

        public int TargetPathLength { get; internal set; }
        public bool GraphTooComplex { get; internal set; }
        public int TurnNum { get; internal set; }

        public IReadOnlyList<IPath> MaxPaths { get; internal set; }
        public IPath MaxPath { get; internal set; }

        public bool IsGameWon => MaxPath is not null && MaxPath.Length >= TargetPathLength;
        public bool IsGameDone => GraphTooComplex || IsGameWon;

        public Edge NewestEdge { get; internal set; }
        public int NewestPaint { get; internal set; }

        /// <summary>
        /// Creates a node at (0,0) - Though, because of the physics, it'll quickly be attracted to the graph as a whole.
        /// </summary>
        /// <returns></returns>
        public Node CreateNode()
            => Board.CreateNode();

    }
}
