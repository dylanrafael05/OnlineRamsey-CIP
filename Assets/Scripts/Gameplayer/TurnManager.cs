using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Board;
using Ramsey.Gameplayer;
using Ramsey.Graph;
using Ramsey.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ramsey.UI 
{
    public class TurnManager
    {
        private BoardManager board;

        private Builder builder;
        private Painter painter;

        private bool isAwaitingMove;
        private bool isAwaiting;
        private bool isBuilderTurn;

        private Task awaiting;
        private Task<IMove> awaitingMove;

        public bool IsAwaitingMove => isAwaitingMove;
        public bool IsAwaitingBoard => isAwaiting && !isAwaitingMove;

        public int TurnNumber { get; private set; }

        public TurnManager(BoardManager board, Builder builder, Painter painter)
        {
            this.board = board;

            this.builder = builder;
            this.painter = painter;

            isBuilderTurn = true;
        }

        public void Update() 
        {
            if(isAwaiting)
            {
                if(isAwaitingMove)
                {
                    if(awaitingMove.IsCompleted)
                    {
                        
                        var move = awaitingMove.Result;

                        if(move.MakeMove(board))
                        {
                            isBuilderTurn = !isBuilderTurn;
                            if (isBuilderTurn) 
                            {
                                board.MarkNewTurn();
                            }
                            Debug.Log("Game End: " + (Math.Max(board.GameState.MaxPaths[0].Length, board.GameState.MaxPaths[1].Length) >= board.TargetPathLength));
                            Debug.Log("Current Turn: " + board.GameState.TurnNum);
                        }

                        if(board.IsAwaitingPathTask)
                        {
                            awaiting = board.AwaitPathTask();
                        }
                        else
                        {
                            isAwaiting = false;
                        }
                        
                        awaitingMove = null;
                        isAwaitingMove = false;
                    }

                    // Debug.Log("Longest path = " + (board.GameState.MaxPaths?.ToString() ?? "none"));
                    // Debug.Log("Paths found = " + board.Paths.Count());
                    // Debug.Log("All paths \n" + string.Join("\n", board.Paths.Select(t => t.ToString())));
                }
                else if(awaiting.IsCompleted)
                {
                    isAwaiting = false;
                    awaiting = null;
                }
            }
            else 
            {
                if(isBuilderTurn)
                {
                    //Debug.Log("Getting builder move . . .");
                    awaitingMove = (builder as IPlayer).GetMove(board.GameState)
                           .ContinueWith(async t => { await Task.Delay(200); return t.Result; })
                           .Unwrap();
                }
                else 
                {
                    //Debug.Log("Getting painter move . . .");
                    awaitingMove = (painter as IPlayer).GetMove(board.GameState)
                           .ContinueWith(async t => { await Task.Delay(200); return t.Result; })
                           .Unwrap();
                }

                isAwaiting = true;
                isAwaitingMove = true;
            }
        }
    }
}