using System.Collections;
using System.Collections.Generic;
using Ramsey.Graph;
using UnityEngine;
using Unity.Mathematics;
using System;
using Ramsey.Drawing;
using Ramsey.Utilities;
using System.Threading.Tasks;
using UnityEngine.Profiling;
using static Codice.CM.Common.Merge.MergePathResolver;

namespace Ramsey.Board
{
    public class BoardManager
    {
        private readonly RecordingManager recordingManager;
        private readonly DrawingManager renderManager;
        private readonly GraphManager graphManager;

        internal DrawingActionInterface RenderAPI => renderManager.ActionInterface;
        internal DrawingIOInterface RenderIO => renderManager.IOInterface;

        public IReadOnlyGraph Graph => graphManager.Graph;
        public GameState GameState => gameState;

        public BoardPreferences Preferences { get; private set; }

        public bool IsAwaitingPathTask => graphManager.IsAwaitingPathTask;
        public Task AwaitPathTask() => graphManager.AwaitPathTask();

        public IReadOnlyList<Node> Nodes => graphManager.Nodes;
        public IReadOnlyList<Edge> Edges => graphManager.Edges;
        public IEnumerable<Path> Paths => graphManager.Paths;

        private readonly GameState gameState;

        private BoardManager(Camera camera, BoardPreferences prefs, GraphManager graphManager)
        {
            this.graphManager = graphManager;
            renderManager = new(camera, prefs.drawingPreferences);
            recordingManager = new(this);

            Preferences = prefs;

            graphManager.OnFinishPathCalculation += delegate 
            {
                SetHighlightedPath(GameState.MaxPaths.MaxBy(p => p.Length));
                gameState.MaxPaths = graphManager.MaxPathsByType;
            };

            gameState = new()
            {
                //Graph = graphManager.Graph,
                Board = this,
                MaxPaths = graphManager.MaxPathsByType
            };
        }

        public BoardManager(Camera camera, BoardPreferences prefs, IIncrementalPathFinder pathFinder) : this(camera, prefs, new GraphManager(pathFinder))
        { }

        public static BoardManager UsingAlgorithm<TAlgo>(Camera camera, BoardPreferences prefs)
            where TAlgo : IIncrementalPathFinder, new()
        {
            return new BoardManager(camera, prefs, GraphManager.UsingAlgorithm<TAlgo>());
        }

        public Node CreateNode(float2 position = default)
        {
            var n = graphManager.CreateNode(position);

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
        public void PaintEdge(Edge edge, int type)
        {
            GameState.NewestEdge = null;
            graphManager.PaintEdge(edge, type);
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

        public void SetLoading(bool isLoading)
        {
            renderManager.IOInterface.SetLoading(isLoading);
        }

        public void Draw()
        {
            renderManager.IOInterface.SetLoading(graphManager.IsAwaitingPathTask);
            renderManager.ActionInterface.Draw();
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

        public void IterateThroughNodes(Action<Node> action)
            => Graph.Nodes.Foreach(action);

        public bool IsValidEdge(Node start, Node end)
            => graphManager.IsValidEdge(start, end);

        public void SetMousePosition(float2 position)
            => renderManager.IOInterface.SetMousePosition(position);

        public void SetHighlightedPath(Path path)
            => renderManager.IOInterface.SetHighlightedPath(path);

        public void SaveCurrentTurn()
            => recordingManager.AddCurrentTurn(); 

        public void LoadTurn(int i)
            => recordingManager.LoadTurn(i);

        public void OffsetTurn(int delta)
            => recordingManager.OffsetTurn(delta);

        public bool IsCurrentTurn => recordingManager.IsCurrentTurn;
    }
}