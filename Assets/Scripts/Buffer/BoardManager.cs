using System.Collections;
using System.Collections.Generic;
using Ramsey.Core;
using UnityEngine;
using Unity.Mathematics;

public class BoardManager
{
    private EngineManager renderManager;
    private GraphManager graphManager;

    private BoardManager(Camera camera, EnginePreferences prefs, GraphManager graphManager) 
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

    public void SetNodePosition(Node node, float2 position)
    {
        graphManager.MoveNode(node, position);
        renderManager.WritingInterface.UpdateNodePosition(node, position);
    }

    public void Clear()
    {
        graphManager.Clear();
        renderManager.WritingInterface.Clear();
    }

    public void LoadFromString(string source)
    {
        graphManager = GraphSerialization.LoadFromString(source);

        foreach(var node in graphManager.Nodes)
        {
            renderManager.WritingInterface.AddNode(node);
        }

        foreach(var edge in graphManager.Edges)
        {
            renderManager.WritingInterface.AddEdge(edge);
        }
    }
}
