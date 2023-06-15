using Ramsey.Gameplayer;
using Ramsey.Screen;
using Ramsey.Visualization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = TMPro.TMP_Dropdown;

namespace Ramsey.UI
{
    public class MenuBehavior : Behavior
    {
        //
        Visualizer visualizer;
        bool visualizing = false;

        //
        MenuManager menu;

        DropdownWrapper<Painter> painter;
        DropdownWrapper<Builder> builder;

        Button startRealtimeGameButton;
        Button startBulkGameButton;

        public MenuBehavior(GraphPreferences graphPreferences)
        {
            visualizer = new(CameraManager.ScreenCamera, graphPreferences);
            menu = new(
                new() 
                { 
                    StrategyInitializer.Direct<UserBuilder>(),
                    new RandomBuilderIntializer(),
                    new CapBuilderInitializer(),
                }, 
                new() 
                { 
                    StrategyInitializer.Direct<UserPainter>(),
                    StrategyInitializer.Direct<AlternatingPainter>(),
                    StrategyInitializer.Direct<RandomPainter>(),
                    StrategyInitializer.Direct<LengthyPainter>(),
                }
            );

            var painterObj = GameObject.Find("Painter Select").GetComponent<Dropdown>();
            var builderObj = GameObject.Find("Builder Select").GetComponent<Dropdown>();

            painter = new(painterObj,
                ("User", () => new UserPainter()),
                ("Random", () => new RandomPainter()),
                ("Alternating", () => new AlternatingPainter())
            );
            builder = new(builderObj,
                ("User", () => new UserBuilder()),
                ("Random", () => new RandomBuilder(0.5f, 0.4f, 0.1f))
            );

            startRealtimeGameButton = GameObject.Find("Enter Realtime Game").GetComponent<Button>();
            startBulkGameButton = GameObject.Find("Enter Bulk Game").GetComponent<Button>();

            startRealtimeGameButton.onClick.AddListener(() => 
            {
                visualizing = false;
                Main.GameBehaviour.StartGame(20, builder.Selected, painter.Selected);
                IBehavior.SwitchTo(Main.GameBehaviour);
            });

            startBulkGameButton.onClick.AddListener(() =>
            {
                visualizing = true;
                InitAfterGather(Main.Game.SimulateGames(1, 30, 1, builder.Selected, painter.Selected));
            });
        }

        public void Init()
        {
            
        }

        public void InitAfterRealtime(int2 gameData)
        {
            
        }

        public void InitAfterGather(MatchupData data)
        {
            visualizer.AddCurve(data, new() { lineThickness = 1f, color = Color.red });
        }

        public override void Loop(InputData input)
        {
            menu.UpdateWheels(input);
            menu.Draw();

            if (visualizing) visualizer.Draw();
        }

        public override void OnEnter()
        {
            painter.UI.gameObject.SetActive(true);
            builder.UI.gameObject.SetActive(true);

            startRealtimeGameButton.gameObject.SetActive(true);
            startBulkGameButton.gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            painter.UI.gameObject.SetActive(false);
            builder.UI.gameObject.SetActive(false);

            startRealtimeGameButton.gameObject.SetActive(false);
            startBulkGameButton.gameObject.SetActive(false);
        }
    }
}