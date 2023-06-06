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
        
        private bool graphTooComplex;

        public bool GraphTooComplex => graphTooComplex;

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
            if(graphTooComplex) return;

            if(board.Nodes.Count == board.MaxNodeCount && board.Graph.IsCompleteColored())
            {
                graphTooComplex = true;
                return;
            }

            if(isAwaiting)
            {
                if(isAwaitingMove)
                {
                    if(awaitingMove.IsCompleted)
                    {
                        IMove move;

                        try 
                        {
                            move = awaitingMove.Result;
                        }
                        catch(GraphTooComplexException) 
                        {
                            graphTooComplex = true;
                            return;
                        }

                        if(move.MakeMove(board))
                        {
                            isBuilderTurn = !isBuilderTurn;
                            if (isBuilderTurn) 
                            {
                                board.MarkNewTurn();
                            }
                            Debug.Log("Game End: " + board.GameState.IsGameDone);
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
                }
                else if(awaiting.IsCompleted)
                {
                    isAwaiting = false;
                    awaiting = null;
                }
            }
            else 
            {
                async Task<IMove> Move(IPlayer player)
                {
                    await Task.Delay((int)(player.Delay * 1000));
                    return await player.GetMove(board.GameState);
                }

                if(isBuilderTurn)
                {
                    awaitingMove = Move(builder);
                }
                else 
                {
                    awaitingMove = Move(painter);
                }

                isAwaiting = true;
                isAwaitingMove = true;
            }
        }
    }
}