using Ramsey.Board;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    public class RandomPainter : Painter
    {
        public override bool IsDeterministic => false;
        
        public override Task<PainterMove> GetMove(GameState gameState)
        {
            return Task.FromResult<PainterMove>(
                new(gameState.NewestEdge, Random.Range(0, 2))
            );
        }
        
        public override void Reset() {}
    }
}
