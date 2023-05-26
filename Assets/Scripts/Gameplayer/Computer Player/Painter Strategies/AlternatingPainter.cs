using Ramsey.Graph;
using Ramsey.Board;
using System.Threading.Tasks;

namespace Ramsey.Gameplayer
{
    public class AlternatingPainter : Painter
    {
        private int count = 0;

        public override Task<PainterMove> GetMove(GameState gameState)
        {
            var count = this.count;
            this.count++; 

            return Task.FromResult<PainterMove>(
                new(gameState.NewestEdge, count % 100) 
                //TODO: store target path length in game state!
            );
        }
    }
}
