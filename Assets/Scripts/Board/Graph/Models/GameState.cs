using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Ramsey.Graph
{
    public class GameState //struct so its cloned everytime we get it from board
    {
        internal GameState() { }

        public IReadOnlyGraph Graph { get; internal set; }

        public IPath[] MaxPaths { get; internal set; }

        public Edge EdgeStart(int i) => throw new System.NotImplementedException();
        public Edge EdgeEnd(int i) => throw new System.NotImplementedException();
    }
}
