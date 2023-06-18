using Ramsey.Board;
using System.Threading.Tasks;

namespace Ramsey.Gameplayer
{
    public class FavoriteColorPainter : Painter 
    {
        public FavoriteColorPainter(int favoriteColor)
        {
            FavoriteColor = favoriteColor;
        }

        public int FavoriteColor { get; }

        public override Task<PainterMove> GetMove(GameState gameState)
        {
            return Task.FromResult<PainterMove>(
                new(gameState.NewestEdge, FavoriteColor)
            );
        }

        public override void Reset() {}
    }
}
