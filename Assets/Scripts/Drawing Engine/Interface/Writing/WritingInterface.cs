using Ramsey.Core;

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
        data.EdgeTransforms.Add(EngineTransformGenerator.GenerateEdgeTransform(e.Start.Position, e.End.Position, preferences.edgeThickness));
        data.EdgeTypes.Add((float) e.Type);

        drawer.UpdateEdgeBuffer();
        drawer.UpdateArgsBuffers();
    }
    public void AddNode(Node n)
    {
        data.NodePositions.Add(n.Position);

        drawer.UpdateNodeBuffer();
        drawer.UpdateArgsBuffers();
    }

}