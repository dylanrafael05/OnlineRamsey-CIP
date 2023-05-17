using System.Collections;
using System.Collections.Generic;
using Ramsey.Core;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using System.Diagnostics;

public class Test : MonoBehaviour
{
    BoardManager board;

    // Start is called before the first frame update
    void Start()
    {
        board = new BoardManager(Camera.current, new EnginePreferences
        {
            blackColor = Color.cyan,
            blueColor = Color.blue,
            redColor = Color.red,
            edgeThickness = 0.15f,
            nodeRadius = 0.3f,
        });

        const int NodeCount = 100;

        for(var i = 0; i < NodeCount; i++)
        {
            var n = board.CreateNode(new(i / Mathf.FloorToInt(Mathf.Sqrt(NodeCount)), i % Mathf.FloorToInt(Mathf.Sqrt(NodeCount))));
        }

        for(var i = 0; i < NodeCount; i++)
        {
            var nodeA = UnityEngine.Random.Range(0, NodeCount);
            var nodeB = UnityEngine.Random.Range(0, NodeCount);

            var nodes = new[] {nodeA, nodeB};

            while(nodeA == nodeB || board.Graph.Edges.Any(e => nodes.Contains(e.Start.ID) && nodes.Contains(e.End.ID)))
            {
                nodeA = UnityEngine.Random.Range(0, NodeCount);
                nodeB = UnityEngine.Random.Range(0, NodeCount);

                nodes = new[] {nodeA, nodeB};
            }

            board.CreateEdge(
                board.Graph.Nodes[nodeA], 
                board.Graph.Nodes[nodeB], 
                UnityEngine.Random.Range(0, 2)
            );
        }

        // var stopwatch = new Stopwatch();
        // stopwatch.Start();
        // var (x, _) = GraphTraversal.BestPath(graph);
        // stopwatch.Stop();

        // UnityEngine.Debug.Log("Finding longest path for 10x10 took " + stopwatch.ElapsedMilliseconds + " milliseconds");
        // UnityEngine.Debug.Log("Found path of length " + x.Count());

        // UnityEngine.Debug.Log(board.Graph);
        
    }

    // Update is called once per frame
    void Update()
    {
        board.RenderAPI.Draw();
    }

    void OnDestroy() 
    {
        board.RenderAPI.Cleanup();
    }
}