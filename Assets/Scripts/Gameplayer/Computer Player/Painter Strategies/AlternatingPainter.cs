using Ramsey.Graph;
using Ramsey.Board;
using System.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    public class AlternatingPainter : Painter
    {
        public override Task<PainterMove> GetMove(GameState gameState)
        {
            return Task.FromResult<PainterMove>(
                new(gameState.NewestEdge, gameState.TurnNum % 2) 
            );
        }

        public override void Reset() {}
    }

    public class LengthyPainter : Painter
    {
        public override Task<PainterMove> GetMove(GameState gameState)
        {
            return Task.FromResult<PainterMove>(
                new(gameState.NewestEdge, gameState.TurnNum / Mathf.Max(gameState.TargetPathLength - 1, 1) % 2) 
            );
        }
        
        public override void Reset() {}
    }
}
