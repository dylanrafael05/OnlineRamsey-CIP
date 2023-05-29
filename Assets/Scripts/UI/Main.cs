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

public class Main : MonoBehaviour
{
    BoardManager board;
    TurnManager turns;

    [SerializeField] Camera boardCamera;
    [SerializeField] Camera screenCamera;

    // Start is called before the first frame update
    void Awake()
    {
        new CameraManager(screenCamera, boardCamera);

        board = BoardManager.UsingAlgorithm<DefaultPathFinder>(CameraManager.BoardCamera, new BoardPreferences()
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

        var ub = new UserBuilder(); var up = new UserPainter();
        turns = new TurnManager(board, ub, up);

        TextRenderer.Create();

        UserModeHandler.Create(board);
        InputManager.Create(board);

        var nodeEditingMode = new NodeEditingMode();
        var turnNavigatorMode = new TurnNavigatorMode(new IUserMode[] { nodeEditingMode, ub, up });

        UserModeHandler.AddMode(nodeEditingMode);
        UserModeHandler.AddMode(turnNavigatorMode);

        board.StartGame(10);
    }

    // Update is called once per frame
    void Update()
    {
        var input = InputManager.Update();
        UserModeHandler.Update(input);

        turns.Update();
        board.Update();
    }

    void OnDestroy() 
    {
        board.Cleanup();
    }
}