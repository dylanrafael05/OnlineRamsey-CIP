using Ramsey.Graph;
using System.Threading.Tasks;
using Ramsey.Utilities;

namespace Ramsey.Gameplayer
{
    public class UserBuilder : Builder
    {
        public override async Task<BuilderMove> GetMove(GameState gameState)
        {
            await Utils.WaitUntil(() => UserInteraction.Ins.CurrNode != null && UserInteraction.Ins.PrevNode != null, 10);

            return new BuilderMove(UserInteraction.Ins.CurrNode, UserInteraction.Ins.PrevNode);
        }

    }

    public class UserPainter : Painter
    {
        public override async Task<PainterMove> GetMove(GameState gameState)
        {
            await Utils.WaitUntil(() => UserInteraction.Ins.CurrEdge != null, 10);

            return new PainterMove(UserInteraction.Ins.CurrEdge, UserInteraction.Ins.CurrEdgeType);
        }
    }
}