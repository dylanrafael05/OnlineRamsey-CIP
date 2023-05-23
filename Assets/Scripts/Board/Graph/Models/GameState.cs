using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Ramsey.Graph
{
    public class GameState //struct so its cloned everytime we get it from board
    {
        internal GameState() { }

        public IReadOnlyGraph Graph { get; internal set; }
        public IReadOnlyList<IPath> MaxPaths { get; internal set; }

        public Edge LastUnpaintedEdge { get; internal set; }

        public Edge EdgeStart(int i) => MaxPaths[i].Edges.First();
        public Edge EdgeEnd(int i)   => MaxPaths[i].Edges.Last();
    }
}
