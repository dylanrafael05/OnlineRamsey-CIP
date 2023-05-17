using System.Collections;
using System.Collections.Generic;
using Ramsey.Core;
using UnityEngine;
using Unity.Mathematics;
using System;

public class BoardManager : IGraph
{
    private EngineManager renderManager;
    private GraphManager graphManager;

    public ReadingInterface RenderAPI => renderManager.ReadingInterface;

    public IReadOnlyGraph Graph => graphManager;

    public BoardPreferences Preferences { get; private set; }

    public IReadOnlyList<Node> Nodes => graphManager.Nodes;
    public IReadOnlyList<Edge> Edges => graphManager.Edges;

    private BoardManager(Camera camera, BoardPreferences prefs, GraphManager graphManager) 
    {
        this.graphManager = graphManager;
        renderManager = new(camera, prefs.drawingPreferences);

        this.Preferences = prefs;
    }

    public BoardManager(Camera camera, BoardPreferences prefs) : this(camera, prefs, new())
    {}

    public Node CreateNode(float2 position = default)
    {
        var n = graphManager.CreateNode(position);

        renderManager.WritingInterface.AddNode(n);

        return n;
    }
    public Edge CreateEdge(Node start, Node end, int type)
    {
        var e = graphManager.CreateEdge(start, end, type);
        renderManager.WritingInterface.AddEdge(e);

        return e;
    }

    public void MoveNode(Node node, float2 position)
    {
        graphManager.MoveNode(node, position);
        renderManager.WritingInterface.UpdateNodePosition(node);
    }
    public void PaintEdge(Edge edge, int type)
    {
        graphManager.PaintEdge(edge, type);
        renderManager.WritingInterface.UpdateEdgeType(edge);
    }

    public void Clear()
    {
        graphManager.Clear();
        renderManager.WritingInterface.Clear();
    }

    public void IterateThroughNodes(Action<Node> action)
    {
        Graph.Nodes.Foreach(action);
    }

    public bool IsValidEdge(Node start, Node end)
    {
        return graphManager.IsValidEdge(start, end);
    }
}
