using Ramsey.Graph;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Ramsey.Board;
using Ramsey.Drawing;

namespace Ramsey.Gameplayer
{
    public class UserInteraction //generalize like node being hovered and stuff and helper methods like that put the specific clicking stuff in UserPlayer
    {

        //
        public static UserInteraction Ins { get; private set; }

        BoardManager board;

        public UserInteraction(BoardManager board)
        {
            this.board = board;

            Ins ??= this;
        }

        //
        bool CollideNode(float2 mouse, Node n)
            => length(mouse - n.Position) <= board.Preferences.drawingPreferences.nodeRadius;

        bool CollideEdge(float2 mouse, Edge e)
        {
            float2 StartToEnd = e.End.Position - e.Start.Position; float2 lp = mouse - .5f * (e.Start.Position + e.End.Position); float2 dir = normalize(StartToEnd);
            lp = new float2(dot(lp, dir), length(cross(float3(lp, 0f), float3(dir, 0f)))); lp.x = abs(lp.x);
            return lp.x <= length(StartToEnd) * .5f && lp.y <= board.Preferences.drawingPreferences.edgeThickness * .5f;
        }

        public Node PrevNode { get; private set; }
        public Node CurrNode { get; private set; }

        public Edge CurrEdge { get; private set; }
        public int CurrEdgeType { get; private set; }

        public void DoInput(float2 mouse, bool lmbp, bool rmbp)
        {
            var nodeCur = board.Nodes.FirstOrDefault(n => CollideNode(mouse, n));
            var edgeCur = board.Edges.FirstOrDefault(e => CollideEdge(mouse, e));

            if (lmbp)
            {
                if(nodeCur != null)
                {
                    PrevNode = CurrNode ?? PrevNode;
                    CurrNode = nodeCur;
                }
                else 
                {
                    PrevNode = null;
                    CurrNode = null;
                }
            }

            if (lmbp || rmbp)
            {
                if(nodeCur == null)
                {
                    CurrEdge = edgeCur;
                    CurrEdgeType = rmbp ? 0 : 1;
                }
                else 
                {
                    CurrEdge = null;
                }
            }

        }

    }
}