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
    GameManager game;

    [SerializeField] Camera boardCamera;
    [SerializeField] Camera screenCamera;

    // Start is called before the first frame update
    void Awake()
    {
        var board = BoardManager.UsingAlgorithm<JobPathFinder>(CameraManager.BoardCamera, new BoardPreferences()
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

        game = new GameManager(board)
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

        var builder = new RandomBuilder(.15f, .8f, .05f); 
        var painter = new RandomPainter();

        game.StartGame(20, builder, painter);

        visualizer = new(CameraManager.BoardCamera, new() { position = new float2(0f), scale = new float2(1f), sizeBounds = new float2(3.4f, 8f) , color = Color.black, drawSize = 5f, thickness = 1f});
        visualizer.AddCurve(new() { data = new() { new(0, 0), new(1, 2), new(2, 3), new(3,4), new(4,-2), new(5,1)} }, new() { color = Color.red, lineThickness = .3f }, 2f);
    }
    Visualizer visualizer;

    bool effectPlayed;

    void Update()
    {
        //visualizer.Draw();

        UserModeHandler.Update(InputManager.Update());

        game.UpdateGameplay();

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

        if (game.State.IsGameDone && !effectPlayed)
        {
            UnityReferences.GoalText.text = "Game Over";
            UnityReferences.ScreenMaterial.SetFloat("_TimeStart", Time.timeSinceLevelLoad);
            effectPlayed = true;

            NodeSmoothing.Smooth(game.Board, 1000);

            // TODO: not this
            // turns.builder = new UserBuilder();
        }
    }

    void OnDestroy() 
    {
        game.Cleanup();
    }
}
