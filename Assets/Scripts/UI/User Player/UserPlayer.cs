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
            await Utils.WaitUntil(() => currNode != null && prevNode != null);
            UserModeHandler.DelMode(this);

            return new BuilderMove(currNode, prevNode);
        }

        //
        Node prevNode;
        Node currNode;
        
        public void Init(BoardManager board) => prevNode = currNode = null;
        public void Update(InputData input, BoardManager board)
        {
            if (input.lmbp)
            {
                if(prevNode != null) board.UnhighlightNode(prevNode);

                prevNode = currNode;
                currNode = input.collidingNodes.FirstOrDefault();

                if(currNode != null) board.HighlightNode(currNode);
                else
                {
                    if(prevNode != null) board.UnhighlightNode(prevNode);
                    prevNode = null;
                }
            }
        }
        public void End(BoardManager board) 
        { 
            if(prevNode != null)
                board.UnhighlightNode(prevNode);
            if(currNode != null)
                board.UnhighlightNode(currNode);
        }

    }

    public class UserPainter : Painter, IUserMode
    {
        public override async Task<PainterMove> GetMove(GameState gameState)
        {
            UserModeHandler.AddMode(this);
            await Utils.WaitUntil(() => currEdge != null);
            UserModeHandler.DelMode(this);

            return new PainterMove(currEdge, currEdgeType);
        }

        //
        Edge currEdge;
        int currEdgeType;

        public void Init(BoardManager board) => currEdge = null;
        public void Update(InputData input, BoardManager board)
        {
            if(input.rmbp || input.lmbp)
            {
                currEdge = input.collidingEdges.FirstOrDefault();
                currEdgeType = input.lmbp ? 0 : 1;
            }
        }
        public void End(BoardManager board) { }
    }
}