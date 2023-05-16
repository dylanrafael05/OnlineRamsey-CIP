using System.Collections;
using System.Collections.Generic;
using Ramsey.Core;
using UnityEngine;
using Unity.Mathematics;

public class BoardManager
{
    private EngineManager renderManager;
    private GraphManager graphManager;

    internal BoardManager(EnginePreferences prefs, GraphManager graphManager) 
    {

    }

    public Node CreateNode(float2 position = default)
    {
        var n = graphManager.CreateNode();

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
        graphManager.SetNodePosition(node, position);
        renderManager.WritingInterface.UpdateNodePosition(node, position);
    }
}
