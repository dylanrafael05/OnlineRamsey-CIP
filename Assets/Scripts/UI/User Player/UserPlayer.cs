using Ramsey.Graph;
using System.Threading.Tasks;
using Ramsey.Utilities;
using Ramsey.Board;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Ramsey.UI;

namespace Ramsey.Gameplayer
{
    [NonAutomatedStrategy, UnsupportedInHeadless]
    public class UserBuilder : Builder, IUserMode<BoardManager>
    {
        public override BuilderMove GetMove(GameState gameState)
        {
            UserModeHandler<BoardManager>.AddMode(this);
            Utils.WaitUntil(() => currNode != null && prevNode != null);
            UserModeHandler<BoardManager>.DelMode(this);

            return new BuilderMove(currNode, prevNode);
        }

        public override void Reset() {}

        //
        Node prevNode;
        Node currNode;
        
        public void Init(BoardManager board) => prevNode = currNode = null;
        public void Update(InputData input, BoardManager board)
        {
            if (!input.shift && input.lmbp)
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

        public bool IsGameplayMode => true;

    }

    [NonAutomatedStrategy, UnsupportedInHeadless]
    public class UserPainter : Painter, IUserMode<BoardManager>
    {
        public override PainterMove GetMove(GameState gameState)
        {
            UserModeHandler<BoardManager>.AddMode(this);
            Utils.WaitUntil(() => currEdge != null);
            UserModeHandler<BoardManager>.DelMode(this);

            return new PainterMove(currEdge, currEdgeType);
        }

        public override void Reset() {}

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

        public bool IsGameplayMode => true;
    }
}