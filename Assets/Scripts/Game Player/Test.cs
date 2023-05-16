using System.Collections;
using System.Collections.Generic;
using Ramsey.Core;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

public class Test : MonoBehaviour
{
    GraphManager graph; 
    EngineManager engine; 

    // Start is called before the first frame update
    void Start()
    {
        graph = new GraphManager();
        engine = new EngineManager(Camera.current, new EnginePreferences
        {
            blueColor = Color.blue,
            redColor = Color.red,
            edgeThickness = 0.15f,
            nodeRadius = 0.3f,
        });

        for(var i = 0; i < 100; i++)
        {
            var n = graph.CreateNode();
            n.Position = new(i / 10, i % 10);
        }

        for(var i = 0; i < 500; i++)
        {
            var nodeA = UnityEngine.Random.Range(0, 100);
            var nodeB = UnityEngine.Random.Range(0, 100);

            var nodes = new[] {nodeA, nodeB};

            while(nodeA == nodeB || graph.Edges.Any(e => nodes.Contains(e.Start.ID) && nodes.Contains(e.End.ID)))
            {
                nodeA = UnityEngine.Random.Range(0, 100);
                nodeB = UnityEngine.Random.Range(0, 100);

                nodes = new[] {nodeA, nodeB};
            }

            graph.CreateEdge(
                graph.Nodes[nodeA], 
                graph.Nodes[nodeB], 
                UnityEngine.Random.Range(0, 2)
            );
        }

        foreach(var node in graph.Nodes)
        {
            engine.WritingInterface.AddNode(node);
        }
        foreach(var edge in graph.Edges)
        {
            engine.WritingInterface.AddEdge(edge);
        }
    }

    // Update is called once per frame
    void Update()
    {
        engine.ReadingInterface.Draw();
    }

    void OnDestroy() 
    {
        engine.ReadingInterface.Cleanup();
    }
}
