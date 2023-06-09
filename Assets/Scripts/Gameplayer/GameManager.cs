using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Board;
using Ramsey.Gameplayer;
using Ramsey.Graph;
using Ramsey.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ramsey.UI 
{
    public class GameManager
    {
        private readonly BoardManager board;

        public bool InGame { get; private set; }

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
            InGame = true;

            currentTask = null;
        }

        public int2? SimulateGame(int target, Builder builder, Painter painter) 
        {
            Assert.IsTrue(builder.IsAutomated, "Cannot simulate a game to its end with non-automated platers!");
            Assert.IsTrue(painter.IsAutomated, "Cannot simulate a game to its end with non-automated platers!");

            StartGame(target, builder, painter);
            RunUntilDone();
            return GetMatchupData();
        }

        public GameManager(BoardManager board)
        {
            this.board = board;

            isBuilderTurn = true;
            InGame = false;
        }
        
        internal void RunUntilDone()
        {
            while(!board.GameState.IsGameDone)
                RunMove(synchronous: true).Wait();
        }

        internal async Task RunMove(bool synchronous = false)
        {
            InGame = !State.IsGameDone;

            // Early exit if cannot update
            if(!InGame) return;

            // Check for game end by force
            if(board.Nodes.Count == board.MaxNodeCount && board.Graph.IsCompleteColored())
            {
                board.MarkGraphTooComplex();
                return;
            }

            // Get next move
            async Task<IMove> GetMove(IPlayer player)
            {
                if(player.IsAutomated && !synchronous) await Task.Delay((int)(Delay * 1000));

                // TODO: how to handle these awaits while running syncronously?
                // TODO: can we assume that these will always instant-return?
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
                if(move.MakeMove(board, synchronous))
                {
                    isBuilderTurn = !isBuilderTurn;
                    
                    if (isBuilderTurn) 
                        board.MarkNewTurn();
                    
                    break;
                }
            }

            // Wait for path task to complete
            if(!synchronous) await board.AwaitPathTask();
        }

        public void UpdateGameplay() 
        {
            if(InGame)
            {
                if(currentTask is null || currentTask.IsCompleted)
                {
                    currentTask = RunMove();
                }
            }
        }

        public void Render() 
        {
            board.Render();
        }

        public int2? GetMatchupData()
        {
            if(State.GraphTooComplex || InGame) return null;
            return new(State.TurnNum, State.TargetPathLength);
        }

        public void Cleanup()
            => board.Cleanup();
    }
}