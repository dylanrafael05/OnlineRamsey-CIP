using System.Collections.Generic;
using Unity.Mathematics;
using Ramsey.Core;
using UnityEngine.Assertions;

public class NodePositionManager
{
    private Dictionary<Node, float2> positions = new();
    
    public void CreateNode(Node node) 
    {
        Assert.IsFalse(positions.ContainsKey(node), "Cannot create a node twice");

        positions[node] = new(0, 0);
    }

    public float2 Get(Node node) 
        => positions[node];

    public void Set(Node node, float2 position)
        => positions[node] = position;
}