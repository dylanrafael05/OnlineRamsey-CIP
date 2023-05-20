using System;
using System.Threading.Tasks;
using Ramsey.Board;
using Ramsey.Gameplayer;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ramsey.UI 
{
    public class TurnManager
    {
        private BoardManager board;

        private Builder builder;
        private Painter painter;

        private bool isAwaitingTask;
        private bool isBuilderTurn;

        private Task<IMove> awaitingTask;

        public event Action<IMove> OnMoveFailure;

        public TurnManager(BoardManager board, Builder builder, Painter painter)
        {
            this.board = board;

            this.builder = builder;
            this.painter = painter;

            isBuilderTurn = true;
        }

        public void Update() 
        {
            if(isAwaitingTask)
            {
                if(awaitingTask.IsCompleted)
                {
                    var move = awaitingTask.Result;

                    if(move.MakeMove(board))
                    {
                        awaitingTask = null;
                        isAwaitingTask = false;

                        isBuilderTurn = !isBuilderTurn;
                    }
                    else 
                    {
                        OnMoveFailure.Invoke(move);
                    }
                }
            }
            else 
            {
                if(isBuilderTurn)
                {
                    Debug.Log("Getting builder move . . .");
                    awaitingTask = (builder as IPlayer).GetMove(board.GameState);
                }
                else 
                {
                    Debug.Log("Getting painter move . . .");
                    awaitingTask = (painter as IPlayer).GetMove(board.GameState);
                }

                isAwaitingTask = true;
            }
        }
    }
}