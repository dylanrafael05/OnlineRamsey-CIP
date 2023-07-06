using Ramsey.Board;
using Ramsey.Utilities.UI;
using System.Threading.Tasks;

namespace Ramsey.Gameplayer
{
    public class AntiPainter : Painter 
    {
        public AntiPainter(int favoriteColor)
        {
            FavoriteColor = favoriteColor;
        }

        public int FavoriteColor { get; }

        public override PainterMove GetMove(GameState gameState)
        {
            return new(gameState.NewestEdge, FavoriteColor);
        }

        public override void Reset() {}

        static AntiPainter()
            => StrategyInitializer.RegisterFor<AntiPainter>(
                p => new((int)p[0]),
                new TextParameter
                {
                    Name = "Color", 
                    Verifier = new IInputVerifier.Integer(0, 2), 
                    DefaultValue = "0"
                }
            );
    }
}
