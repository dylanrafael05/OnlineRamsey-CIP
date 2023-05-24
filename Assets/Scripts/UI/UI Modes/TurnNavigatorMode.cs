using Ramsey.Board;
using Ramsey.Utilities;
using Ramsey.Gameplayer;
using UnityEngine;
using System.Collections.Generic;

namespace Ramsey.UI
{
    public class TurnNavigatorMode : IUserMode
    {
        IUserMode[] pastLockModes;

        public TurnNavigatorMode(IUserMode[] pastLockModes)
            => this.pastLockModes = pastLockModes;

        public void Init(BoardManager board) { }
        public void Update(InputData input, BoardManager board)
        {
            int d = -1 * input.lkd.ToInt() + 1 * input.rkd.ToInt();
            board.OffsetTurn(d);

            IMove.Enable = board.IsCurrentTurn;
            pastLockModes.Foreach(m => UserModeHandler.SetStatus(m, board.IsCurrentTurn));
        }
        public void End(BoardManager board) { }

    }
}