using Ramsey.Board;
using Ramsey.Graph;
using System.Linq;
using Unity.Mathematics;

namespace Ramsey.UI
{
    public class NodeEditingMode : IUserMode
    {
        private Node cnode;
        private float2 nodeOffset;

        public void Init(BoardManager board)
        {
            cnode = null;
            nodeOffset = float2.zero;
        }

        public void Update(InputData input, BoardManager board)
        {
            if (input.alt && input.lmbp)
            {
                try 
                {
                    board.CreateNode(input.mouse);
                }
                catch(GraphTooComplexException) {}
            }

            if (input.shift && input.lmbp)
            {
                cnode = input.collidingNodes.FirstOrDefault();

                if (cnode != null) nodeOffset = cnode.Position - input.mouse;
            }

            if (input.lmbu)
            {
                cnode = null;
            }

            if (cnode != null)
            {
                board.MoveNode(cnode, input.mouse + nodeOffset);
            }
        }

        public void End(BoardManager board)
        { }
    }
}