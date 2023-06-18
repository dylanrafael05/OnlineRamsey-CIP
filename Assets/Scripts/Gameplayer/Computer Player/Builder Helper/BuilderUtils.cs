using Ramsey.Board;
using Ramsey.Graph;

namespace Ramsey.Gameplayer
{
    public class BuilderUtils
    {
        public static BuilderMove Extend(ref Node n1, GameState state)
        {
            var n2 = state.CreateNode();
            var move = new BuilderMove(n1, n2);
            n1 = n2;
            return move;
        }

        public static BuilderMove Extend(Node n1, GameState state)
            => new(n1, state.CreateNode());
    }
}