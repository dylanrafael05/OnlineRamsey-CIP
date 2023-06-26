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

        public override PainterMove GetMove(GameState gameState)
        {
            return new(gameState.NewestEdge, FavoriteColor);
        }

        public override void Reset() {}
    }
}
