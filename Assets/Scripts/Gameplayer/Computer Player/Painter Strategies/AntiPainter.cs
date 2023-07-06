using Ramsey.Board;
using System.Threading.Tasks;

namespace Ramsey.Gameplayer
{
    public class AntiPainter : Painter 
    {
        //Will try to lose the game by painting the same color each time
        public AntiPainter(int favoriteColor)
            => FavoriteColor = favoriteColor;

        public int FavoriteColor { get; }

        public override PainterMove GetMove(GameState gameState)
            => new(gameState.NewestEdge, FavoriteColor);

        public override void Reset() {}
    }
}
