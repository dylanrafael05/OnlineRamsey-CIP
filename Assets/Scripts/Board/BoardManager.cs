using System.Collections;
using System.Collections.Generic;
using Ramsey.Graph;
using UnityEngine;
using Unity.Mathematics;
using System;
using Ramsey.Drawing;
using Ramsey.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine.Profiling;
using System.Linq;

namespace Ramsey.Board
{
    public class BoardManager : IGraphManager
    {
        private readonly RecordingManager recordingManager;
        private readonly DrawingManager renderManager;
        private readonly GraphManager graphManager;

        internal DrawingActionInterface RenderAPI => renderManager.ActionInterface;
        internal DrawingIOInterface RenderIO => renderManager.IOInterface;

        public int TargetPathLength
        {
            get => gameState.TargetPathLength;
            set => gameState.TargetPathLength = value;
        }

        public IReadOnlyGraph Graph => graphManager.Graph;
        public GameState GameState => gameState;

        public BoardPreferences Preferences { get; private set; }

        public bool IsAwaitingPathTask => graphManager.IsAwaitingPathTask;
        public UniTask AwaitPathTask() => graphManager.AwaitPathTask();

        public IReadOnlyList<Node> Nodes => graphManager.Nodes;
        public IReadOnlyList<Edge> Edges => graphManager.Edges;
        public int? MaxNodeCount => graphManager.MaxNodeCount;

        private readonly GameState gameState;

        private BoardManager(Camera boardCamera, Camera screenCamera, BoardPreferences prefs, GraphManager graphManager)
        {
            this.graphManager = graphManager;
            renderManager = new(boardCamera, screenCamera, prefs.drawingPreferences);
            recordingManager = new(this);

            Preferences = prefs;

            graphManager.OnFinishPathCalculation += delegate 
            {
                gameState.MaxPaths = graphManager.MaxPathsByType;
                gameState.MaxPath = gameState.MaxPaths?.Where(p => p is not null).MaxBy(p => p.Length);
                
                RenderIO.SetHighlightedPathAsync(gameState.MaxPath);
            };

            gameState = new()
            {
                Board = this,
                MaxPaths = graphManager.MaxPathsByType
            };
        }

        public BoardManager(Camera boardCamera, Camera screenCamera, BoardPreferences prefs, IIncrementalPathFinder pathFinder) : this(boardCamera, screenCamera, prefs, new GraphManager(pathFinder))
        { }

        public static BoardManager UsingAlgorithm<TAlgo>(Camera boardCamera, Camera screenCamera, BoardPreferences prefs)
            where TAlgo : IIncrementalPathFinder, new()
        {
            return new BoardManager(boardCamera, screenCamera, prefs, GraphManager.UsingAlgorithm<TAlgo>());
        }

        public void LoadGraph(IGraph graph)
            => graphManager.LoadGraph(graph);

        public Node CreateNode(float2? position = default)
        {
            var n = graphManager.CreateNode(position ?? new float2(UnityEngine.Random.Range(-6f, 6f), UnityEngine.Random.Range(-3f, 3f)));

            renderManager.IOInterface.AddNode(n);

            return n;
        }
        public Edge CreateEdge(Node start, Node end)
        {
            var e = graphManager.CreateEdge(start, end);

            GameState.NewestEdge = e;
            renderManager.IOInterface.AddEdge(e);

            return e;
        }

        public void MoveNode(Node node, float2 position)
        {
            graphManager.MoveNode(node, position);
            renderManager.IOInterface.UpdateNodePosition(node);
        }
        public void PaintEdge(Edge edge, int type, bool synchronous = false)
        {   
            GameState.NewestEdge = null;
            GameState.NewestPaint = type;

            graphManager.PaintEdge(edge, type, synchronous);
            renderManager.IOInterface.UpdateEdgeType(edge);
        }

        public void HighlightNode(Node n)
        {
            renderManager.IOInterface.HighlightNode(n);
        }
        public void UnhighlightNode(Node n)
        {
            renderManager.IOInterface.UnhighlightNode(n);
        }

        public void MarkGraphTooComplex()
        {
            gameState.GraphTooComplex = true;
        }

        public void RenderBoard()
        {
            renderManager.ActionInterface.RenderBoard();
        }
        public void RenderUI()
        {
            RenderIO.SetLoading(graphManager.IsAwaitingPathTask);
            renderManager.ActionInterface.RenderUI();
        }
        public void RenderLoadingDirect()
        {
            renderManager.ActionInterface.RenderLoadingDirect();
        }

        public void Cleanup()
        {
            renderManager.ActionInterface.Cleanup();
        }

        public void Clear()
        {
            graphManager.Clear();
            renderManager.IOInterface.Clear();
        }

        public void ClearScreen()
        {
            renderManager.IOInterface.ClearScreen();
        }

        public void IterateThroughNodes(Action<Node> action)
            => Graph.Nodes.Foreach(action);

        public bool IsValidEdge(Node start, Node end)
            => graphManager.IsValidEdge(start, end);

        public void SetMousePosition(float2 position)
            => renderManager.IOInterface.SetMousePosition(position);

        public void LoadTurn(int i)
            => recordingManager.LoadTurn(i);

        public void OffsetTurn(int delta)
            => recordingManager.OffsetTurn(delta);

        public bool IsCurrentTurn => recordingManager.IsCurrentTurn;

        public void MarkNewTurn()
        {
            recordingManager.AddCurrentTurn();
            gameState.TurnNum++;
        }

        public void StartGame(int pathLength)
        {
            Clear();
            recordingManager.Clear();

            gameState.GraphTooComplex = false;
            gameState.MaxPaths = new List<IPath>();
            gameState.MaxPath = null;

            gameState.TargetPathLength = pathLength;
            gameState.TurnNum = 0;

            UnityReferences.GoalText.text = "" + gameState.TargetPathLength;
        }
    }
}