using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Ramsey.Graph
{
    public class GameState
    {
        internal GameState() {}

        public IReadOnlyGraph Graph { get; internal set; }
        public Path MaxLengthPath { get; internal set; }
    }
}
