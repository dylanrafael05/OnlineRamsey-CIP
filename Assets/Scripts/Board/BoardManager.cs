using System.Collections;
using System.Collections.Generic;
using Ramsey.Graph;
using UnityEngine;
using Unity.Mathematics;
using System;
using Ramsey.Drawing;
using Ramsey.Utilities;
using UnityEngine.Profiling;

namespace Ramsey.Board
{
    public class BoardManager : IGraph
    {
        private RecordingManager recordingManager;

        private DrawingManager renderManager;
        private GraphManager graphManager;

        public DrawingActionInterface RenderAPI => renderManager.ActionInterface;

        public IReadOnlyGraph Graph => graphManager;
        public GameState GameState => graphManager.State;
        public GameState GameStateClone => graphManager.State.Clone();

        public BoardPreferences Preferences { get; private set; }

        public IReadOnlyList<Node> Nodes => graphManager.Nodes;
        public IReadOnlyList<Edge> Edges => graphManager.Edges;
        public IEnumerable<Path> Paths => graphManager.Paths;

        private BoardManager(Camera camera, BoardPreferences prefs, GraphManager graphManager)
        {
            this.graphManager = graphManager;
            renderManager = new(camera, prefs.drawingPreferences);
            recordingManager = new();

            Preferences = prefs;
        }

        public BoardManager(Camera camera, BoardPreferences prefs) : this(camera, prefs, new())
        { }

        public Node CreateNode(float2 position = default)
        {
            var n = graphManager.CreateNode(position);

            renderManager.IOInterface.AddNode(n);

            return n;
        }
        public Edge CreateEdge(Node start, Node end, int type)
        {
            var e = graphManager.CreateEdge(start, end, type);
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