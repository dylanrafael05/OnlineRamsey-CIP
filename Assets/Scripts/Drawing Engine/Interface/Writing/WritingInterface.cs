using Ramsey.Core;
using UnityEngine.Assertions;
using Unity.Mathematics;
using UnityEngine;

public class WritingInterface
{

    EngineData data;
    EnginePreferences preferences;
    EngineDrawer drawer;

    public WritingInterface(EngineData data, EnginePreferences preferences, EngineDrawer drawer)
    {
        this.data = data;
        this.preferences = preferences;
        this.drawer = drawer;
    }

    public void AddEdge(Edge e)
    {
        Assert.AreEqual(data.EdgeTransforms.Count, e.ID, "Edges must be added to renderer upon creation!");

        data.EdgeTransforms.Add(EngineTransformGenerator.GenerateEdgeTransform(e.Start.Position, e.End.Position, e.Type, preferences.edgeThickness));
        data.EdgeColors.Add(preferences.TypeToColor(e.Type));

        drawer.UpdateEdgeBuffer();
        drawer.UpdateArgsBuffers();
    }
    public void AddNode(Node n)
    {
        Assert.AreEqual(data.NodePositions.Count, n.ID, "Nodes must be added to renderer upon creation!");

        data.NodePositions.Add(n.Position);

        drawer.UpdateNodeBuffer();
        drawer.UpdateArgsBuffers();
    }

    public void UpdateNodePosition(Node n)
    {
        Assert.IsTrue(data.NodePositions.Count > n.ID, "Nodes must be added to renderer upon creation!");

        data.NodePositions[n.ID] = n.Position;
        
        drawer.UpdateNodeBuffer();
        drawer.UpdateArgsBuffers();
    }
    public void UpdateEdgeType(Edge e)
    {
        data.EdgeColors[e.ID] = preferences.TypeToColor(e.Type);
    }

    public void Clear()
    {
        data.NodePositions.Clear();
        data.EdgeTransforms.Clear();
        data.EdgeColors.Clear();
    }
}