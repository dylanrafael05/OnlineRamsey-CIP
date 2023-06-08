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
        var cameraControlMode = new CameraControlMode();

        UserModeHandler.AddMode(nodeEditingMode);
        UserModeHandler.AddMode(turnNavigatorMode);
        UserModeHandler.AddMode(cameraControlMode);

        var builder = new CapBuilder(game.State); //new RandomBuilder(.15f, .8f, .05f); 
        var painter = new RandomPainter();

        game.StartGame(40, builder, painter);
        //game.RunUntilDone();

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
        

        // visualizer.Draw();

        UserModeHandler.Update(InputManager.Update());

        game.UpdateGameplay();

        // NodeSmoothing.Smooth(board, 100);

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
                highlightColor = Color.green,

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
            game.StartGame(t, builder, painter);
            game.RunUntilDone();
            matchupData.data.Add(game.GetMatchupData());
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