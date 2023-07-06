using Ramsey.Board;
using System.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    public class LengthyPainter : Painter
    {
        public override PainterMove GetMove(GameState gameState)
            => new(gameState.NewestEdge, gameState.TurnNum / Mathf.Max(gameState.TargetPathLength - 1, 1) % 2);
        
        public override void Reset() {}
    }
}
