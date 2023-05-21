using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Ramsey.Graph
{
    public struct GameState //struct so its cloned everytime we get it from board
    {
        internal GameState(IReadOnlyGraph Graph, Path MaxLengthPath) { this.Graph = Graph; this.MaxLengthPath = MaxLengthPath; }

        public IReadOnlyGraph Graph { get; internal set; }
        public Path MaxLengthPath { get; internal set; }
    }
}
