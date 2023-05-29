using Ramsey.Graph;
using Ramsey.Board;
using System.Threading.Tasks;

namespace Ramsey.Gameplayer
{
    public class AlternatingPainter : Painter
    {
        public override Task<PainterMove> GetMove(GameState gameState)
        {
            return Task.FromResult<PainterMove>(
                new(gameState.NewestEdge, gameState.TurnNo % gameState.TargetPathLength) 
            );
        }
    }

    public class LengthyPainter : Painter
    {
        public override Task<PainterMove> GetMove(GameState gameState)
        {
            return Task.FromResult<PainterMove>(
                new(gameState.NewestEdge, gameState.TurnNo / (gameState.TargetPathLength - 1) % 2) 
            );
        }
    }
}
