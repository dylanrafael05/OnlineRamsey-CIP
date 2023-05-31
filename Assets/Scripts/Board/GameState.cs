using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Ramsey.Graph;

namespace Ramsey.Board
{
    public class GameState 
    {
        internal GameState() { }

        internal BoardManager Board { get; set; }

        public IReadOnlyList<Node> Nodes => Board.Nodes;

        public int TargetPathLength { get; internal set; }
        public int TurnNum { get; internal set; }

        public IReadOnlyList<IPath> MaxPaths { get; internal set; }

        public Edge EdgeStart(int i) => MaxPaths[i].Edges.First();
        public Edge EdgeEnd(int i)   => MaxPaths[i].Edges.Last();

        public Edge NewestEdge { get; internal set; }
        public int NewestPaint { get; internal set; }

        //Painter Commands
        public Path MaxPathConnectedToNull(int i) => throw new System.NotImplementedException(); //shouldnt be array dont think

        public Node CreateNode()
        {
            return Board.CreateNode();
        }

    }
}
