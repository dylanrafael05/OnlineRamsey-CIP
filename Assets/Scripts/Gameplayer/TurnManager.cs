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
    public class GameManager
    {
        private readonly BoardManager board;

        private bool inGame;

        public BoardManager Board => board;
        public GameState State => board.GameState;

        private Builder builder;
        private Painter painter;
        private bool isBuilderTurn;

        private Task currentTask;

        public float Delay { get; set; } = 1.5f;

        public void StartGame(int target, Builder builder, Painter painter) 
        {
            board.StartGame(target);

            this.builder = builder;
            this.painter = painter;

            isBuilderTurn = true;
            inGame = true;
        }

        public GameManager(BoardManager board)
        {
            this.board = board;

            isBuilderTurn = true;
            inGame = false;
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

        public void UpdateGameplay() 
        {
            inGame &= !State.IsGameDone;

            if(inGame)
            {
                if(currentTask is null || currentTask.IsCompleted)
                {
                    currentTask = RunMoveAsync();
                }
            }

            board.Update();
        }

        public void Cleanup()
        {
            board.Cleanup();
        }
    }
}