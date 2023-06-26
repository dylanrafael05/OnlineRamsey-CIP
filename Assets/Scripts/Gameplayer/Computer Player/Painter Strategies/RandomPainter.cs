using Ramsey.Board;
using Ramsey.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    [NonDeterministicStrategy]
    public class RandomPainter : Painter
    {
        public override PainterMove GetMove(GameState gameState)
        {
            return new(gameState.NewestEdge, ThreadSafeRandom.Range(0, 2));
        }
        
        public override void Reset() {}
    }
}
