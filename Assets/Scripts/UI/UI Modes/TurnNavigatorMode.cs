using Ramsey.Board;
using Ramsey.Utilities;
using Ramsey.Gameplayer;
using UnityEngine;

namespace Ramsey.UI
{
    public class TurnNavigatorMode : IUserMode
    {
        NodeEditingMode nodeEditingMode;

        public TurnNavigatorMode(NodeEditingMode nodeEditingMode)
            => this.nodeEditingMode = nodeEditingMode;

        public void Init(BoardManager board) { }
        public void Update(InputData input, BoardManager board)
        {
            int d = -1 * input.lkd.ToInt() + 1 * input.rkd.ToInt();
            Debug.Log(input.lkd);
            board.OffsetTurn(d);

            IMove.Enable = board.IsCurrentTurn;
            UserModeHandler.SetStatus(nodeEditingMode, board.IsCurrentTurn);
        }
        public void End(BoardManager board) { }

    }
}