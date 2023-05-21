using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Board;
using Ramsey.Gameplayer;
using Ramsey.Graph;
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

        private List<GameState> gameStates; //for recording

        public TurnManager(BoardManager board, Builder builder, Painter painter)
        {
            this.board = board;

            this.builder = builder;
            this.painter = painter;

            isBuilderTurn = true;

            gameStates = new() { board.GameState };
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
                        isBuilderTurn = !isBuilderTurn;
                        gameStates.Add(board.GameState);
                    }
                    
                    awaitingTask = null;
                    isAwaitingTask = false;

                    Debug.Log("Longest path = " + (board.GameState.MaxLengthPath?.ToString() ?? "none"));
                    Debug.Log("Paths found = " + board.Paths.Count());
                    
                    // Debug.Log("All paths \n" + string.Join("\n", board.Paths.Select(t => t.ToString())));
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