using Ramsey.Core;
using UnityEngine.Assertions;
using Unity.Mathematics;

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
        data.EdgeTransforms.Add(EngineTransformGenerator.GenerateEdgeTransform(e.Start.Position, e.End.Position, e.Type, preferences.edgeThickness));
        data.EdgeTypes.Add((float) e.Type);

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

    public void UpdateNodePosition(Node n, float2 position)
    {
        Assert.IsTrue(data.NodePositions.Count > n.ID, "Nodes must be added to renderer upon creation!");

        data.NodePositions[n.ID] = position;
        
        drawer.UpdateNodeBuffer();
        drawer.UpdateArgsBuffers();
    }

}