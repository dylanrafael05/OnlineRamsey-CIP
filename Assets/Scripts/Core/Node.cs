using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ramsey.Core
{
    public class Node
    {
        public IReadOnlyList<Edge> Edges { get; }
    }
}