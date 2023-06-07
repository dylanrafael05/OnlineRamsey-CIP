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

        public Edge EdgeStart(int i) => MaxPaths[i].Edges.First();
        public Edge EdgeEnd(int i)   => MaxPaths[i].Edges.Last();

        public Edge NewestEdge { get; internal set; }
        public int NewestPaint { get; internal set; }

        //Painter Commands
        public Node CreateNode()
        {
            return Board.CreateNode();
        }

    }
}
