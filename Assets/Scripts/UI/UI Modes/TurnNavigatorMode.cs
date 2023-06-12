using Ramsey.Board;
using Ramsey.Utilities;
using Ramsey.Gameplayer;
using UnityEngine;
using System.Collections.Generic;

namespace Ramsey.UI
{
    public class TurnNavigatorMode : IUserMode<BoardManager>
    {

        public void Init(BoardManager board) { }
        public void Update(InputData input, BoardManager board)
        {
            int d = -1 * input.lkd.ToInt() + 1 * input.rkd.ToInt();
            board.OffsetTurn(d);

            IMove.Enable = board.IsCurrentTurn;
            UserModeHandler<BoardManager>.GameplayModes.Foreach(m => UserModeHandler<BoardManager>.SetStatus(m, board.IsCurrentTurn));
        }
        public void End(BoardManager board) { }

        public bool IsGameplayMode => false;

    }
}