using Ramsey.Core;
using UnityEngine;
using Unity.Mathematics;

public class TestBehavior : MonoBehaviour
{

    EngineManager drawingEngine;

    GraphManager graph;
    NodePositionManager nodePositionManager;

    private void Awake()
    {

        drawingEngine = new(Camera.current, new EnginePreferences()
        {
            redColor = Color.red,
            blueColor = Color.blue,
            edgeThickness = 1,
            nodeRadius = 1
        });

        graph = new GraphManager();
        Node n = graph.CreateNode();

        nodePositionManager = new();
        nodePositionManager.CreateNode(n);
        nodePositionManager.Set(n, new float2(1f, 0.5f));

        drawingEngine.data.NodePositions.Add(nodePositionManager.Get(n));

    }

    private void Update()
    {
        drawingEngine.Interface.Draw();
    }

    private void OnDestroy()
    {
        drawingEngine.Interface.Cleanup();
    }


}