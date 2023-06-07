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
        private bool isBuilderTurn;

        private Task currentTask;

        public float Delay { get; set; } = 1.5f;

        public int TurnNumber { get; private set; }

        public TurnManager(BoardManager board, Builder builder, Painter painter)
        {
            this.board = board;

            this.builder = builder;
            this.painter = painter;

            isBuilderTurn = true;
        }

        public async Task RunMoveAsync(bool skipWaits = false)
        {
            // Early exit if cannot update
            if(board.GameState.GraphTooComplex) return;

            // Check for game end by force
            if(board.Nodes.Count == board.MaxNodeCount && board.Graph.IsCompleteColored())
            {
                board.MarkGraphTooComplex();
                return;
            }

            // Get next move
            async Task<IMove> GetMove(IPlayer player)
            {
                if(player.CanDelay && !skipWaits) await Task.Delay((int)(Delay * 1000));
                return await player.GetMove(board.GameState);
            }
            
            // Repeat until move is valid
            while(true)
            {
                IMove move;

                try 
                {
                    if(isBuilderTurn)
                    {
                        move = await GetMove(builder);
                    }
                    else 
                    {
                        move = await GetMove(painter);
                    }
                }
                catch(GraphTooComplexException) 
                {
                    board.MarkGraphTooComplex();
                    return;
                }

                // Run move
                if(move.MakeMove(board))
                {
                    isBuilderTurn = !isBuilderTurn;
                    
                    if (isBuilderTurn) 
                    {
                        board.MarkNewTurn();
                    }

                    Debug.Log("Game End: " + board.GameState.IsGameDone);
                    Debug.Log("Current Turn: " + board.GameState.TurnNum);
                    
                    break;
                }
            }

            // Wait for path task to complete
            await board.AwaitPathTask();
        }

        public void RunMove()
        {
            RunMoveAsync(true).Wait();
        }

        public void RunUntilDone()
        {
            while(!board.GameState.IsGameDone)
            {
                RunMove();
            }
        }

        public void Update() 
        {
            if(currentTask is null || currentTask.IsCompleted)
            {
                currentTask = RunMoveAsync();
            }

            // if(GraphTooComplex) return;

            // if(board.Nodes.Count == board.MaxNodeCount && board.Graph.IsCompleteColored())
            // {
            //     GraphTooComplex = true;
            //     return;
            // }

            // if(isAwaiting)
            // {
            //     if(isAwaitingMove)
            //     {
            //         if(awaitingMove.IsCompleted)
            //         {
            //             IMove move;

            //             try 
            //             {
            //                 move = awaitingMove.Result;
            //             }
            //             catch(GraphTooComplexException) 
            //             {
            //                 GraphTooComplex = true;
            //                 return;
            //             }

            //             if(move.MakeMove(board))
            //             {
            //                 isBuilderTurn = !isBuilderTurn;
            //                 if (isBuilderTurn) 
            //                 {
            //                     board.MarkNewTurn();
            //                 }
            //                 Debug.Log("Game End: " + board.GameState.IsGameDone);
            //                 Debug.Log("Current Turn: " + board.GameState.TurnNum);
            //             }

            //             if(board.IsAwaitingPathTask)
            //             {
            //                 awaiting = board.AwaitPathTask();
            //             }
            //             else
            //             {
            //                 isAwaiting = false;
            //             }
                        
            //             awaitingMove = null;
            //             isAwaitingMove = false;
            //         }
            //     }
            //     else if(awaiting.IsCompleted)
            //     {
            //         isAwaiting = false;
            //         awaiting = null;
            //     }
            // }
            // else 
            // {
            //     async Task<IMove> Move(IPlayer player)
            //     {
            //         if(player.CanDelay) await Task.Delay((int)(Delay * 1000));
            //         return await player.GetMove(board.GameState);
            //     }

            //     if(isBuilderTurn)
            //     {
            //         awaitingMove = Move(builder);
            //     }
            //     else 
            //     {
            //         awaitingMove = Move(painter);
            //     }

            //     isAwaiting = true;
            //     isAwaitingMove = true;
            // }
        }
    }
}