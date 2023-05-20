using Unity.Mathematics;
using UnityEngine;
using Ramsey.Utilities;
using Ramsey.Screen;
using Ramsey.Graph;
using Ramsey.Board;
using static Unity.Mathematics.math;
using System.Collections.Generic;
using System.Linq;

public static class InputManager
{
    private static BoardManager board;

    public static void Create(BoardManager board)
    {
        InputManager.board = board;
    }

    public static float3 GetScreenMousePosition()
        => Input.mousePosition;

    public static float2 GetWorldMousePosition()
        => (Vector2)CameraManager.BoardCamera.ScreenToWorldPoint(GetScreenMousePosition().rescale(CameraManager.ScreenSize, CameraManager.BoardSize));

    public static bool CollideNode(float2 mouse, Node node)
        => length(mouse - node.Position) <= board.Preferences.drawingPreferences.nodeRadius;
    public static bool CollideEdge(float2 mouse, Edge e)
    {
        float2 StartToEnd = e.End.Position - e.Start.Position; float2 lp = mouse - .5f * (e.Start.Position + e.End.Position); float2 dir = normalize(StartToEnd);
        lp = new float2(dot(lp, dir), length(cross(float3(lp, 0f), float3(dir, 0f)))); lp.x = abs(lp.x);
        return lp.x <= length(StartToEnd) * .5f && lp.y <= board.Preferences.drawingPreferences.edgeThickness * .5f;
    }

    private static readonly InputData data = new();
    public static InputData Update()
    {
        data.rawMouse = GetScreenMousePosition().xy;
        data.mouse = GetWorldMousePosition();

        data.lmb  = Input.GetMouseButton(0);
        data.lmbp = Input.GetMouseButtonDown(0);
        
        data.mmb  = Input.GetMouseButton(2);
        data.mmbp = Input.GetMouseButtonDown(2);
        
        data.rmb  = Input.GetMouseButton(1);
        data.rmbp = Input.GetMouseButtonDown(1);

        data.collidingNodes = board.Nodes.Where(n => CollideNode(data.mouse, n)).ToHashSet();
        data.collidingEdges = board.Edges.Where(e => CollideEdge(data.mouse, e)).ToHashSet();

        board.SetMousePosition(data.rawMouse);

        return data;
    }   
}

public class InputData
{
    public float2 rawMouse; public float2 mouse;
    
    public bool lmb; public bool rmb; public bool mmb;
    public bool lmbp; public bool rmbp; public bool mmbp;

    public ISet<Node> collidingNodes;
    public ISet<Edge> collidingEdges;
}