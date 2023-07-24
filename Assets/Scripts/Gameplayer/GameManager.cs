using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        private CancellationToss cancel;

        public bool InGame { get; private set; }

        public BoardManager Board => board;
        public GameState State => board.GameState;

        private Builder builder;
        private Painter painter;
        private bool isBuilderTurn;

        private UniTask? currentTask;

        public float Delay { get; set; } = 1.5f;

        public void StartGame(int target, Builder builder, Painter painter) 
        {
            board.StartGame(target);

            cancel.Request();
            currentTask = null;

            this.builder = builder;
            this.painter = painter;

            isBuilderTurn = true;
            InGame = true;

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

            cancel = new();
        }

        public static GameManager CreateDefault()
        {
            return new(new(Camera.main, new(), new JobPathFinder()));
        }
        
        internal void RunUntilDone()
        {
            while(!board.GameState.IsGameDone)
                RunMove(cancel, synchronous: true).Forget();
        }

        internal async UniTask RunMove(CancellationToss cancel, bool synchronous = false)
        {
            bool isBuilderTurnAtStart = isBuilderTurn;

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
            async UniTask<IMove> GetMove(IPlayer player, CancellationToss cancel)
            {
                if(player.IsAutomated && !synchronous) await UniTask.Delay((int)(Delay * 1000));

                return await player.GetMoveAsync(State, cancel)
                    .AssertSync(synchronous);
            }
            
            // Repeat until move is valid
            while(true)
            {
                IMove move;
                
                if(cancel.IsRequested || !board.IsCurrentTurn) 
                    return;

                try 
                {
                    if(isBuilderTurnAtStart)
                    {
                        move = await GetMove(builder, cancel);
                    }
                    else 
                    {
                        move = await GetMove(painter, cancel);
                    }
                }
                catch(GraphTooComplexException) 
                {
                    board.MarkGraphTooComplex();
                    return;
                }

                if(cancel.IsRequested || !board.IsCurrentTurn) 
                    return;


                // Run move
                if(move.MakeMove(board, synchronous))
                {
                    isBuilderTurn = !isBuilderTurn;
                    
                    if (isBuilderTurn) 
                        board.MarkNewTurn();
                    

                    // Wait for path UniTask to complete
                    await board.AwaitPathUniTask().AssertSync(synchronous);
                    
                    return;
                }
            }
        }

        public void UpdateGameplay() 
        {
            if(InGame && board.IsCurrentTurn)
            {
                if(currentTask is null || currentTask.Value.Status.IsCompleted())
                {
                    cancel = new();
                    currentTask = RunMove(cancel);
                }
            }
        }

        public void Leave()
        {
            cancel.Request();
            currentTask = null;
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