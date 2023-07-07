using Ramsey.Graph;
using Ramsey.Board;
using System.Threading.Tasks;
using Ramsey.Utilities.UI;

namespace Ramsey.Gameplayer
{
    public class AlternatingPainter : Painter.Synchronous
    {
        public AlternatingPainter(int lengthPerColor)
        {
            this.lengthPerColor = lengthPerColor;
        }

        public int lengthPerColor;

        public override PainterMove GetMove(GameState gameState)
        {
            return new(gameState.NewestEdge, gameState.TurnNum / lengthPerColor % 2);
        }

        public override void Reset() {}

        static AlternatingPainter()
            => StrategyInitializer.RegisterFor<AlternatingPainter>(
                p => new((int)p[0]),
                new TextParameter
                {
                    Name = "Stride",
                    Verifier = new IInputVerifier.Integer(1),
                    DefaultValue = "1"
                }
            );

        public override string ToString()
            => "Alternating Painter";
    }
}
