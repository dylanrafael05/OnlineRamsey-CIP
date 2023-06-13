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
                colors = new[] { Color.blue * 0.9f, Color.red * 0.9f, Color.green * 0.9f, Color.yellow * 0.9f },
                edgeThickness = 0.15f,
                highlightThickness = 0.1f,

                nodeColor = Color.white,
                nodeRadius = 0.3f,
                highlightRadius = 0.5f,
                nodeHighlightColor = Color.green,

                recorderColor = Color.white,

                loadingCircleOuterRadius = 1.0f,
                loadingCircleInnerRadius = 0.7f,

                backgroundColor = Utils.ColorFromHex("#957A5E"),
                backgroundHighlightColor = Utils.ColorFromHex("#856A4E") * 0.5f + Utils.ColorFromHex("#957A5E") * 0.5f,
            }
        });

        new CameraManager(screenCamera, boardCamera);

        game = new GameManager(board)
        {
            Delay = 0.2f
        };

        UserModeHandler<BoardManager>.Create(board);
        InputManager.Create(board);

        var nodeEditingMode = new NodeEditingMode();
        var turnNavigatorMode = new TurnNavigatorMode(); //put locks in here

        var cameraControlMode = new CameraControlMode();

        UserModeHandler<BoardManager>.AddMode(nodeEditingMode);
        UserModeHandler<BoardManager>.AddMode(turnNavigatorMode);
        UserModeHandler<BoardManager>.AddMode(cameraControlMode);

        var builder = new CapBuilder(game.State); //new RandomBuilder(.15f, .8f, .05f); 
        var painter = new RandomPainter();

        game.StartGame(20, builder, painter);

        // prefs = new() { position = new float2(0f), axisScale = new float2(2f), sizeBounds = new float2(3.4f, 8f), color = Color.black, drawSize = 5f, thickness = .075f, tickCount = 8 };
        // visualizer = new(CameraManager.BoardCamera, prefs);
        // visualizer.AddCurve(new() { data = new() { new(0, 0), new(1, 2), new(2, 3), new(3,25), new(4,-5), new(5,1), new(6,1),new(7,1),new(8,1),new(9,2)} }, new() { color = Color.red, lineThickness = .9f }, 4f);
        // visualizer.AddCurve(new() { data = new() { new(0, 0), new(1, 5), new(2, 4), new(3,5), new(4,7), new(5,2), new(6,4),new(7,1),new(8,1),new(9,2)} }, new() { color = Color.blue, lineThickness = .9f }, 4f);
    }

    Visualizer visualizer;
    GraphPreferences prefs;

    bool effectPlayed;

    void Update()
    {
        UserModeHandler<BoardManager>.Update(InputManager.Update());
        CameraManager.Update();

        game.UpdateGameplay();
        NodeSmoothing.Smooth(game.Board, 2); //TODO: must be even in order to remove jitter

        DisplayText.Update(game.State);

        if (game.State.IsGameDone && !effectPlayed)
        {
            GameOverHandler.Display(game.State);

            effectPlayed = true;
        }
        
        game.Render();
    }

    void OnDestroy() 
        => game.Cleanup();
}

internal class GameBehavior : IBehavior
{

    //
    MenuBehavior menuBehavior;

    GameManager game;

    bool effectPlayed;

    public GameBehavior(Builder builder, Painter painter)
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
                nodeHighlightColor = Color.green,

                recorderColor = Color.white,

                loadingCircleOuterRadius = 1.0f,
                loadingCircleInnerRadius = 0.7f,
            }
        });

        game = new(board)
        {
            Delay = 0f
        };
    }

    public void InitRealtime(int target, Builder builder, Painter painter)
        => game.StartGame(target, builder, painter);

    public void InitGather(int startTarget, int endTarget, int step, Builder builder, Painter painter)
    {

        MatchupData matchupData = new() { data = new() };

        for(int t = startTarget; t <= endTarget; t += step)
        {
            var s = game.SimulateGame(t, builder, painter);
            if(s is int2 i) matchupData.data.Add(i);
        }

        //menubehavior may want a different init for gathering vs realtime.. how will i store realtime? a dictionary? well i should prolly store matchupdata out of the curves anyways
        //slight variations of builder and painter, probability distributions, parameters like double alternating, give reason to gathering the data now
    }

    public void Loop()
    {

    }

    public void EndRealtime()
    {

    }

    public void EndGather()
    {
        IBehavior.Current = menuBehavior;
        //menuBehavior.Init(..);
    }

}

internal class MenuBehavior : IBehavior
{

    //
    GameBehavior gameBehavior;

    //
    Visualizer visualizer;

    public MenuBehavior(GraphPreferences graphPreferences)
    {
        visualizer = new(CameraManager.BoardCamera, graphPreferences);
    }

    public void Init()
    {
        
    }

    public void InitAfterRealtime(int2 gameData)
    {

    }

    public void InitAfterGather(MatchupData data)
    {

    }

    public void Loop()
    {

    }

}

internal interface IBehavior { static IBehavior Current; void Loop(); }