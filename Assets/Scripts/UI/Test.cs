using System.Collections;
using System.Collections.Generic;
using Ramsey.Graph;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using System.Diagnostics;
using Ramsey.Board;
using Ramsey.Drawing;
using Ramsey.UI;
using Ramsey.Gameplayer;
using Ramsey.Screen;

public class Test : MonoBehaviour
{
    BoardManager board;
    TurnManager turns;

    [SerializeField] Camera boardCamera;
    [SerializeField] Camera screenCamera;

    // Start is called before the first frame update
    void Start()
    {
        new CameraManager(screenCamera, boardCamera);

        board = new BoardManager(CameraManager.BoardCamera, new BoardPreferences() 
        {
            drawingPreferences = new DrawingPreferences
            {
                nullColor = Color.cyan,
                colors = new[] { Color.blue, Color.red },
                edgeThickness = 0.15f,
                highlightThickness = .1f,
                nodeColor = Color.black,
                nodeRadius = 0.3f,
                highlightRadius = 0.4f,
                highlightColor = Color.green
            }
        });

        turns = new TurnManager(board, new UserBuilder(), new UserPainter());

        UserModeHandler.Create(board);

        const int NodeCount = 100;

        for(var i = 0; i < NodeCount; i++)
        {
            var n = board.CreateNode(new(i / Mathf.FloorToInt(Mathf.Sqrt(NodeCount)), i % Mathf.FloorToInt(Mathf.Sqrt(NodeCount))));
            // board.HighlightNode(n);
        }

        // for(var i = 0; i < NodeCount; i++)
        // {
        //     var nodeA = UnityEngine.Random.Range(0, NodeCount);
        //     var nodeB = UnityEngine.Random.Range(0, NodeCount);

        //     var nodes = new[] {nodeA, nodeB};

        //     while(nodeA == nodeB || board.Graph.Edges.Any(e => nodes.Contains(e.Start.ID) && nodes.Contains(e.End.ID)))
        //     {
        //         nodeA = UnityEngine.Random.Range(0, NodeCount);
        //         nodeB = UnityEngine.Random.Range(0, NodeCount);

        //         nodes = new[] {nodeA, nodeB};
        //     }

        //     board.CreateEdge(
        //         board.Graph.Nodes[nodeA], 
        //         board.Graph.Nodes[nodeB], 
        //         UnityEngine.Random.Range(0, 2)
        //     );
        // }

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
        var input = InputManager.Update();
        UserModeHandler.Update(input);

        turns.Update();
        board.RenderAPI.Draw();
    }

    void OnDestroy() 
    {
        board.RenderAPI.Cleanup();
    }
}
