using System.Collections;
using System.Collections.Generic;
using Ramsey.Core;
using UnityEngine;
using Unity.Mathematics;
using System;
using JetBrains.Annotations;

public class BoardManager
{
    private EngineManager renderManager;
    private Graph graphManager;

    public ReadingInterface RenderAPI => renderManager.ReadingInterface;

    public IGraphView Graph => graphManager;

    private BoardManager(Camera camera, EnginePreferences prefs, Graph graphManager) 
    {
        this.graphManager = graphManager;
        renderManager = new(camera, prefs);
    }

    public BoardManager(Camera camera, EnginePreferences prefs) : this(camera, prefs, new())
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

    public void LoadFromString(string source)
    {
        graphManager = GraphSerialization.LoadFromString(source);

        renderManager.WritingInterface.Clear();

        foreach(var node in graphManager.Nodes)
        {
            renderManager.WritingInterface.AddNode(node);
        }

        foreach(var edge in graphManager.Edges)
        {
            renderManager.WritingInterface.AddEdge(edge);
        }
    }

    public void IterateThroughNodes(Action<Node> action)
    {
        Graph.Nodes.Foreach(action);
    }
}
