using System.Collections;
using System.Collections.Generic;
using Ramsey.Graph;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using Ramsey.Board;
using Ramsey.Drawing;
using Ramsey.UI;
using Ramsey.Gameplayer;
using Ramsey.Screen;
using Ramsey.Graph.Experimental;
using Ramsey.Visualization;

public class Main : MonoBehaviour
{
    BoardManager board;
    TurnManager turns;

    [SerializeField] Camera boardCamera;
    [SerializeField] Camera screenCamera;

    // Start is called before the first frame update
    void Awake()
    {

        board = BoardManager.UsingAlgorithm<JobsPathFinder>(CameraManager.BoardCamera, new BoardPreferences()
        {
            drawingPreferences = new DrawingPreferences
            {
                nullColor = Color.black,
                colors = new[] { Color.blue, Color.red },
                edgeThickness = 0.15f,
                highlightThickness = 0.1f,

                nodeColor = Color.white,
                nodeRadius = 0.3f,
                highlightRadius = 0.5f,
                highlightColor = Color.green,

                recorderColor = Color.white,

                loadingCircleOuterRadius = 1.0f,
                loadingCircleInnerRadius = 0.7f,
            }
        });

        new CameraManager(screenCamera, boardCamera);

        var ub = new RandomBuilder(.4f, .55f, .05f); var up = new RandomPainter();
        turns = new TurnManager(board, ub, up);

        TextRenderer.Create();

        UserModeHandler.Create(board);
        InputManager.Create(board);

        var nodeEditingMode = new NodeEditingMode();
        var turnNavigatorMode = new TurnNavigatorMode(new IUserMode[] { nodeEditingMode }); //put locks in here

        UserModeHandler.AddMode(nodeEditingMode);
        UserModeHandler.AddMode(turnNavigatorMode);

        board.StartGame(10);

        visualizer = new(CameraManager.BoardCamera, new() { position = new float2(0f), scale = new float2(1f), sizeBounds = new float2(10f) });
        visualizer.AddCurve(new() { data = new() { new(0, 0), new(1, 2), new(2, 3), new(5, 2), new(10, 2) } }, new() { color = Color.red, lineThickness = .1f }, 0.2f);
    }
    Visualizer visualizer;

    bool effectPlayed;

    void Update()
    {
        visualizer.Draw();

        UserModeHandler.Update(InputManager.Update());

        turns.Update();
        board.Update();

        NodeSmoothing.Smooth(board);

        if (board.GameState.IsGameDone && !effectPlayed)
        {
            UnityReferences.GoalText.text = "Game Over";
            UnityReferences.ScreenMaterial.SetFloat("_TimeStart", Time.timeSinceLevelLoad);
            effectPlayed = true;
        }
    }

    void OnDestroy() 
    {
        board.Cleanup();
    }
}
