using System.Collections;
using System.Collections.Generic;
using Ramsey.Graph;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using Ramsey.Board;
using Ramsey.Drawing;
using Ramsey.UI;
using Ramsey.Utilities;
using Ramsey.Screen;
using Ramsey.Graph.Experimental;
using Ramsey.Visualization;
using System;

namespace Ramsey 
{
    public class Main : MonoBehaviour
    {
        public static GameManager Game { get; private set; }
        public static BoardManager Board { get; private set; }

        public static RealtimeGameBehavior GameBehaviour { get; private set; }
        public static MenuBehavior MenuBehavior { get; private set; }

        [SerializeField] Camera boardCamera;
        [SerializeField] Camera screenCamera;

        // Start is called before the first frame update
        void Awake()
        {
            Board = BoardManager.UsingAlgorithm<JobPathFinder>(CameraManager.BoardCamera, new BoardPreferences
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

            Game = new GameManager(Board)
            {
                Delay = 0.0f
            };
            
            CameraManager.Create(screenCamera, boardCamera);

            GameBehaviour = new();
            MenuBehavior = new(new GraphPreferences(new(5), new(1), new(), new(10), Color.black, .1f) {});

            IBehavior.SwitchTo(MenuBehavior);
        }

        void Update()
        {
            TextRenderer.Begin();

            var input = InputManager.Update();
            IBehavior.Active.Loop(input);
            
            TextRenderer.End();
        }

        void OnDestroy() 
        {
            IBehavior.Cleanup();
            Game.Cleanup();
        }
    }
}
