using Ramsey.Core;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Ramsey.Utils;

public class UserBuilder : IBuilder
{
    public async Task<BuilderMove> GetMove(GameState gameState)
    {
        await Utils.WaitUntil(() => UserInteraction.Ins.CurrNode != null && UserInteraction.Ins.PrevNode != null, 10);

        return new BuilderMove(UserInteraction.Ins.CurrNode, UserInteraction.Ins.PrevNode);
    }

}

public class UserPainter : IPainter
{
    public async Task<PainterMove> GetMove(GameState gameState)
    {
        await Utils.WaitUntil(() => UserInteraction.Ins.CurrEdge != null, 10);

        return new PainterMove(UserInteraction.Ins.CurrEdge, UserInteraction.Ins.CurrEdgeType);
    }
}

public class UserInteraction
{

    //
    public static UserInteraction Ins { get; private set; }

    BoardManager board;

    public UserInteraction(BoardManager board, EngineManager drawer)
    {
        this.board = board;

        Ins ??= this;
    }

    //
    bool CollideNode(float2 mouse, Node n)
        => length(mouse - n.Position) <= board.Preferences.drawingPreferences.nodeRadius;

    bool CollideEdge(float2 mouse, Edge e)
    {
        float2 StartToEnd = e.End.Position - e.Start.Position;  float2 lp = mouse - .5f * (e.Start.Position + e.End.Position); float2 dir = normalize(StartToEnd);
        lp = new float2(dot(lp, dir), length(cross(float3(lp, 0f), float3(dir, 0f)))); lp.x = abs(lp.x);
        return lp.x <= length(StartToEnd) * .5f && lp.y <= board.Preferences.drawingPreferences.edgeThickness * .5f;
    }

    public Node PrevNode { get; private set; }
    public Node CurrNode { get; private set; }

    public Edge CurrEdge { get; private set; }
    public int CurrEdgeType { get; private set; }

    public void DoInput(float2 mouse, bool lmbp, bool rmbp)
    {

        if (lmbp)
        {
            PrevNode = CurrNode ?? PrevNode;
            CurrNode = board.Nodes.FirstOrDefault((Node n) => CollideNode(mouse, n));
        }

        if(lmbp || rmbp)
        {
            CurrEdge = board.Edges.FirstOrDefault((Edge e) => CollideEdge(mouse, e));
            CurrEdgeType = rmbp ? 0 : 1;
        }

    }

}