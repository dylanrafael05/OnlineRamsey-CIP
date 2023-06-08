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
using Ramsey.Utilities;
using Ramsey.Screen;
using Ramsey.Graph.Experimental;
using Ramsey.Visualization;
using System;

public class Main : MonoBehaviour
{
    BoardManager board;
    TurnManager turns;

    [SerializeField] Camera boardCamera;
    [SerializeField] Camera screenCamera;

    // Start is called before the first frame update
    void Awake()
    {
        board = BoardManager.UsingAlgorithm<JobPathFinder>(CameraManager.BoardCamera, new BoardPreferences()
        {
            drawingPreferences = new DrawingPreferences
            {
                nullColor = Color.black,
                colors = new[] { Color.blue, Color.red, Color.green, Color.yellow },
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

        var ub = new CapBuilder(board.GameState)/*new RandomBuilder(.15f, .8f, .05f)*/; var up = new RandomPainter();
        turns = new TurnManager(board, ub, up)
        {
            Delay = 0.0f
        };

        TextRenderer.Create();

        UserModeHandler.Create(board);
        InputManager.Create(board);

        var nodeEditingMode = new NodeEditingMode();
        var turnNavigatorMode = new TurnNavigatorMode(new IUserMode[] { nodeEditingMode }); //put locks in here

        UserModeHandler.AddMode(nodeEditingMode);
        UserModeHandler.AddMode(turnNavigatorMode);

        board.StartGame(40);
        // turns.RunUntilDone();

        prefs = new() { position = new float2(0f), scale = new float2(2f), sizeBounds = new float2(3.4f, 8f), color = Color.black, drawSize = 5f, thickness = .1f, tickCount = 8 };
        visualizer = new(CameraManager.BoardCamera, prefs);
        visualizer.AddCurve(new() { data = new() { new(0, 0), new(1, 2), new(2, 3), new(3,5), new(4,-5), new(5,1), new(6,1),new(7,1),new(8,1),new(9,2)} }, new() { color = Color.red, lineThickness = .9f }, 4f);
        visualizer.AddCurve(new() { data = new() { new(0, 0), new(1, 5), new(2, 4), new(3,5), new(4,7), new(5,2), new(6,4),new(7,1),new(8,1),new(9,2)} }, new() { color = Color.blue, lineThickness = .9f }, 4f);
    }
    Visualizer visualizer;
    GraphPreferences prefs;

    bool effectPlayed;

    void Update()
    {
        visualizer.Draw();

        UserModeHandler.Update(InputManager.Update());

        //if(!board.GameState.IsGameDone) turns.Update();
        board.Update();
        prefs.tickCount = math.max(8, (int)Time.timeSinceLevelLoad);
        visualizer.SetPreferences(prefs);

        // NodeSmoothing.Smooth(board, 100);

        var scl = Input.GetKey(KeyCode.LeftShift).ToInt() * 2 + 1;

        if (Input.GetKey(KeyCode.W))
        {
            CameraManager.BoardCamera.transform.position += scl * new Vector3(0, 0.05f, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            CameraManager.BoardCamera.transform.position += scl * new Vector3(0, -0.05f, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            CameraManager.BoardCamera.transform.position += scl * new Vector3(0.05f, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            CameraManager.BoardCamera.transform.position += scl * new Vector3(-0.05f, 0, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            CameraManager.BoardCamera.orthographicSize += scl * 0.5f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            CameraManager.BoardCamera.orthographicSize += scl * -0.5f;
        }

        visualizer.UpdateInput(1f, Input.mouseScrollDelta.y); //TODO: not here and make only scroll when mouse hovering over kinda generous range prolly the square not the width 

        if (board.GameState.IsGameDone && !effectPlayed)
        {
            UnityReferences.GoalText.text = "Game Over";
            UnityReferences.ScreenMaterial.SetFloat("_TimeStart", Time.timeSinceLevelLoad);
            effectPlayed = true;

            NodeSmoothing.Smooth(board, 1000);

            // TODO: not this
            // turns.builder = new UserBuilder();
        }
    }

    void OnDestroy() 
    {
        board.Cleanup();
    }
}
