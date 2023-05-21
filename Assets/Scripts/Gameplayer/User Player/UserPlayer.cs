using Ramsey.Graph;
using System.Threading.Tasks;
using Ramsey.Utilities;
using Ramsey.Board;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Ramsey.Gameplayer
{
    public class UserBuilder : Builder, IUserMode
    {
        public override async Task<BuilderMove> GetMove(GameState gameState)
        {
            UserModeHandler.AddMode(this);
            await Utils.WaitUntil(() => currNode != null && prevNode != null, 10);
            UserModeHandler.DelMode(this);

            return new BuilderMove(currNode, prevNode);
        }

        //
        Node prevNode;
        Node currNode;
        bool CollideNode(float2 mouse, Node n, BoardManager board)
            => math.length(mouse - n.Position) <= board.Preferences.drawingPreferences.nodeRadius;

        public void Init() => prevNode = currNode = null;
        public void Update(InputData input, BoardManager board)
        {
            if (input.lmbp)
            {
                prevNode = currNode;
                currNode = board.Nodes.FirstOrDefault(n => CollideNode(input.mouse, n, board));
            }
        }
        public void End() { }

    }

    public class UserPainter : Painter, IUserMode
    {
        public override async Task<PainterMove> GetMove(GameState gameState)
        {
            UserModeHandler.AddMode(this);
            await Utils.WaitUntil(() => currEdge != null, 10);
            UserModeHandler.DelMode(this);

            return new PainterMove(currEdge, currEdgeType);
        }

        //
        Edge currEdge;
        int currEdgeType;
        bool CollideEdge(float2 mouse, Edge e, BoardManager board)
        {
            float2 StartToEnd = e.End.Position - e.Start.Position; float2 lp = mouse - .5f * (e.Start.Position + e.End.Position); float2 dir = normalize(StartToEnd);
            lp = new float2(dot(lp, dir), length(cross(float3(lp, 0f), float3(dir, 0f)))); lp.x = abs(lp.x);
            return lp.x <= length(StartToEnd) * .5f && lp.y <= board.Preferences.drawingPreferences.edgeThickness * .5f;
        }

        public void Init() => currEdge = null;
        public void Update(InputData input, BoardManager board)
        {
            if(input.rmbp || input.lmbp)
            {
                currEdge = board.Edges.FirstOrDefault(e => CollideEdge(input.mouse, e, board));
                currEdgeType = input.lmbp ? 0 : 1;
            }
        }
        public void End() { }
    }
}