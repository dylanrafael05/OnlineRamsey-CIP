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
                    // Assert.IsFalse(awaitingTask.IsFaulted, "Move producers cannot fail! " + awaitingTask.);

                    var move = awaitingTask.Result;
                    move.MakeMove(board);

                    awaitingTask = null;
                    isAwaitingTask = false;
                }
            }
            else 
            {
                Debug.Log(isBuilderTurn ? "Builder time" : "Paint babyyy");
                
                if(isBuilderTurn)
                {
                    awaitingTask = (builder as IPlayer).GetMove(board.GameState);
                }
                else 
                {
                    awaitingTask = (painter as IPlayer).GetMove(board.GameState);
                }

                isBuilderTurn = !isBuilderTurn;
                isAwaitingTask = true;
            }
        }
    }
}