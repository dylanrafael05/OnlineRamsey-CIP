using Ramsey.Board;
using Ramsey.UI;
using Ramsey.Gameplayer;
using Ramsey.Screen;
using Ramsey.Graph.Experimental;
using System.Collections;
using UnityEngine;
using Ramsey.Utilities;
using Ramsey.Drawing;

namespace Ramsey.UI
{
    public class RealtimeGameBehavior : Behavior
    {
        bool gameOverShown;
        bool updateSmoothing = true;

        public RealtimeGameBehavior()
        {
            UserModeHandler<BoardManager>.Create(Main.Game.Board);
            InputManager.Create(Main.Game.Board);

            var nodeEditingMode = new NodeEditingMode();
            var turnNavigatorMode = new TurnNavigatorMode(); //put locks in here

            var cameraControlMode = new CameraControlMode();

            UserModeHandler<BoardManager>.AddMode(nodeEditingMode);
            UserModeHandler<BoardManager>.AddMode(turnNavigatorMode);
            UserModeHandler<BoardManager>.AddMode(cameraControlMode);
        }

        public void StartGame(int target, Builder builder, Painter painter)
        {
            Main.Game.StartGame(target, builder, painter);
            gameOverShown = false;
        }

        public override void Loop(InputData input)
        {
            UserModeHandler<BoardManager>.Update(input);
            CameraManager.Update();

            if(!IBehavior.IsSwitching && input.escape)
                CheckMeantEscape.EscapePressed(this, input);

            if(input.tabd)
                Utils.Toggle(ref updateSmoothing);

            Main.Game.UpdateGameplay();

            if(updateSmoothing)
                NodeSmoothing.Update(Main.Board);

            DisplayText.Update(Main.Game.State);

            if (Main.Game.State.IsGameDone && !gameOverShown)
            {
                GameOverHandler.Display(Main.Game.State);

                gameOverShown = true;
            }
            
            Main.Game.RenderBoard();
        }

        public void ExitToMenu()
        {
            IBehavior.SwitchTo(Main.MenuBehavior);
        }

        public override void OnEnter()
        {
            UnityReferences.GoalText.gameObject.SetActive(true);
            UnityReferences.TurnText.gameObject.SetActive(true);

            Canvas.willRenderCanvases += Main.Game.RenderUI;
            
            UserModeHandler<BoardManager>.SetAllStatuses(true);
        }

        public override void OnExit()
        {
            UnityReferences.GoalText.gameObject.SetActive(false);
            UnityReferences.TurnText.gameObject.SetActive(false);

            Main.Game.Leave();

            CameraControlMode.ReturnToStart();
            GameOverHandler.ForceStop();

            Main.Board.ClearScreen();
            
            Canvas.willRenderCanvases -= Main.Game.RenderUI;

            UserModeHandler<BoardManager>.SetAllStatuses(false);
        }

        public override void OnCleanup()
            => Canvas.willRenderCanvases -= Main.Game.RenderUI;
    }
}