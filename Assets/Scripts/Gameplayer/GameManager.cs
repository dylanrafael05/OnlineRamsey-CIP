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
            cancel = new();
            currentTask = null;

            this.builder = builder;
            this.painter = painter;

            isBuilderTurn = true;
            InGame = true;

            builder.Reset();
            painter.Reset();
        }

        public async UniTask<MatchupResult?> SimulateGameOnce(int target, Builder builder, Painter painter) 
        {
            Assert.IsTrue(builder.IsAutomated, "Cannot simulate a game to its end with non-automated platers!");
            Assert.IsTrue(painter.IsAutomated, "Cannot simulate a game to its end with non-automated platers!");

            StartGame(target, builder, painter);
            await RunUntilDone();
            return GetMatchupData();
        }

        public async UniTask<MatchupResult?> SimulateGame(int target, Builder builder, Painter painter, int attempts = 1) 
        {
            var matchups = new List<MatchupResult>(attempts);

            for(int i = 0; i < attempts; i++)
            {
                if(await SimulateGameOnce(target, builder, painter) is MatchupResult result)
                    matchups.Add(result);
            }

            if(matchups.Count == 0) return null;

            return MatchupResult.Average(target, builder.GetStrategyName(false), painter.GetStrategyName(false), builder.GetStrategyName(true), painter.GetStrategyName(true), matchups.Select(t => t.AverageGameLength).ToArray());
        }
        
        public async UniTask<MatchupData> SimulateMany(int startTarget, int endTarget, int step, Builder builder, Painter painter, int attempts = 1)
        {
            MatchupData matchupData = new(startTarget, endTarget, step, attempts);

            for(int t = startTarget; t <= endTarget; t += step)
            {
                var result = await SimulateGame(t, builder, painter, attempts);

                if(result is MatchupResult i) matchupData.Add(i);
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
        
        internal async UniTask RunUntilDone()
        {
            cancel = new();

            var accumDelay = 0.0;
            var prevTime = (DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;

            while(!State.IsGameDone && !cancel.IsRequested)
            {
                var curTime = (DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;
                accumDelay += curTime - prevTime;
                prevTime = curTime;

                if(accumDelay > 10)
                {
                    accumDelay = 0;
                    await UniTask.DelayFrame(1);
                }
                
                await RunMove(cancel, synchronous: true);
            }
        }

        internal async UniTask RunMove(CancellationToss cancel, bool synchronous = false)
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
            async UniTask<IMove> GetMove(IPlayer player)
            {
                if(player.IsAutomated && !synchronous) await UniTask.Delay((int)(Delay * 1000));

                return await player.GetMoveAsync(State, cancel);
            }
            
            // Repeat until move is valid
            while(true)
            {
                IMove move;
                
                if(cancel.IsRequested || !board.IsCurrentTurn) 
                    return;

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

                if(cancel.IsRequested || !board.IsCurrentTurn) 
                    return;

                // Run move
                if(move.MakeMove(board, synchronous))
                {
                    isBuilderTurn = !isBuilderTurn;
                    
                    if (isBuilderTurn) 
                        board.MarkNewTurn();

                    // Wait for path UniTask to complete
                    await board.AwaitPathTask();
                    
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