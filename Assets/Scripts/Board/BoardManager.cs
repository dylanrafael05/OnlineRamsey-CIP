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

namespace Ramsey.Board
{
    public class BoardManager
    {
        private RecordingManager recordingManager;

        private DrawingManager renderManager;
        private GraphManager graphManager;

        public DrawingActionInterface RenderAPI => renderManager.ActionInterface;

        public IReadOnlyGraph Graph => graphManager.Graph;
        public GameState GameState => graphManager.State;
        public GameState GameStateClone => graphManager.State.Clone();

        public BoardPreferences Preferences { get; private set; }

        public IEnumerable<Node> Nodes => graphManager.Nodes;
        public IEnumerable<Edge> Edges => graphManager.Edges;
        public IEnumerable<IPath> Paths => graphManager.Paths;

        private BoardManager(Camera camera, BoardPreferences prefs, GraphManager graphManager)
        {
            this.graphManager = graphManager;
            renderManager = new(camera, prefs.drawingPreferences);
            recordingManager = new();

            Preferences = prefs;
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
            renderManager.IOInterface.AddEdge(e);

            return e;
        }

        public void MoveNode(Node node, float2 position)
        {
            graphManager.MoveNode(node, position);
            renderManager.IOInterface.UpdateNodePosition(node);
        }
        public async Task PaintEdge(Edge edge, int type)
        {
            renderManager.IOInterface.UpdateEdgeType(edge);
            await graphManager.PaintEdge(edge, type);
        }

        public void HighlightNode(Node n)
        {
            renderManager.IOInterface.HighlightNode(n);
        }
        public void UnhighlightNode(Node n)
        {
            renderManager.IOInterface.UnhighlightNode(n);
        }

        public void Clear()
        {
            graphManager.Clear();
            renderManager.IOInterface.Clear();
        }

        public void IterateThroughNodes(Action<Node> action)
        {
            Graph.Nodes.Foreach(action);
        }

        public bool IsValidEdge(Node start, Node end)
        {
            return graphManager.IsValidEdge(start, end);
        }

        public void SetMousePosition(float2 position)
        {
            renderManager.IOInterface.SetMousePosition(position);
        }

        public void SaveCurrentTurn()
        { recordingManager.Add(new BoardState(renderManager.IOInterface.CreateDrawState())); }

        public void LoadTurn(int i)
            => recordingManager.LoadTurn(i, renderManager.IOInterface);

        public void OffsetTurn(int delta)
            => recordingManager.OffsetTurn(delta, renderManager.IOInterface);

        public bool IsCurrentTurn => recordingManager.IsCurrentTurn;
    }
}