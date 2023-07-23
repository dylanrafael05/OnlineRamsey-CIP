using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Board;
using Ramsey.Gameplayer;
using Ramsey.Graph;
using Ramsey.Graph.Experimental;
using Ramsey.Utilities;
using Ramsey.Visualization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ramsey.Gameplayer 
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

            builder.Reset();
            painter.Reset();
        }

        public MatchupResult? SimulateGameOnce(int target, Builder builder, Painter painter) 
        {
            Assert.IsTrue(builder.IsAutomated, "Cannot simulate a game to its end with non-automated platers!");
            Assert.IsTrue(painter.IsAutomated, "Cannot simulate a game to its end with non-automated platers!");

            StartGame(target, builder, painter);
            RunUntilDone();
            return GetMatchupData();
        }

        public MatchupResult? SimulateGame(int target, Builder builder, Painter painter, int attempts = 1) 
        {
            var matchups = new List<MatchupResult>(attempts);

            for(int i = 0; i < attempts; i++)
            {
                if(SimulateGameOnce(target, builder, painter) is MatchupResult result)
                    matchups.Add(result);
            }

            if(matchups.Count == 0) return null;

            return MatchupResult.Average(target, builder.GetStrategyName(false), painter.GetStrategyName(false), builder.GetStrategyName(true), painter.GetStrategyName(true), matchups.Select(t => t.AverageGameLength).ToArray());
        }
        
        public MatchupData SimulateMany(int startTarget, int endTarget, int step, Builder builder, Painter painter, int attempts = 1)
        {
            MatchupData matchupData = new(startTarget, endTarget, step, attempts);

            for(int t = startTarget; t <= endTarget; t += step)
            {
                var s = SimulateGame(t, builder, painter, attempts);
                if(s is MatchupResult i) matchupData.Add(i);
            }

            return matchupData;
        }

        public GameManager(BoardManager board)
        {
            this.board = board;

            isBuilderTurn = true;
            InGame = false;
        }

        public static GameManager CreateDefault()
        {
            return new(new(Camera.main, new(), new JobPathFinder()));
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

                return await player.GetMoveAsync(State).AssertSync(synchronous);
            }
            
            // Repeat until move is valid
            while(true)
            {
                IMove move;

                await Utils.WaitUntil(() => board.IsCurrentTurn);

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
                
                await Utils.WaitUntil(() => board.IsCurrentTurn);

                // Run move
                if(move.MakeMove(board, synchronous))
                {
                    isBuilderTurn = !isBuilderTurn;
                    
                    if (isBuilderTurn) 
                        board.MarkNewTurn();
                    
                    Debug.Log("move");
                    
                    break;
                }
                else 
                {
                    Debug.Log("invalid move");
                }
            }

            // Wait for path task to complete
            await board.AwaitPathTask().AssertSync(synchronous);
        }

        public void UpdateGameplay() 
        {
            if(InGame && board.IsCurrentTurn)
            {
                if(currentTask is null || currentTask.IsCompleted)
                {
                    currentTask = RunMove();
                }
            }
        }

        public void RenderBoard() 
        {
            board.RenderBoard();
        }
        
        public void RenderUI() 
        {
            board.RenderUI();
        }

        public MatchupResult? GetMatchupData()
        {
            if(State.GraphTooComplex) return null;
            return new(State.TargetPathLength, State.TurnNum, builder.GetStrategyName(false), painter.GetStrategyName(false), builder.GetStrategyName(true), painter.GetStrategyName(true));
        }

        public void Cleanup()
            => board.Cleanup();
    }
}