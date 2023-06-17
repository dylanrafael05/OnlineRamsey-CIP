using Ramsey.Gameplayer;
using Ramsey.Screen;
using Ramsey.Utilities.UI;
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

        Button startRealtimeGameButton;
        Button startBulkGameButton;

        public MenuBehavior(GraphPreferences graphPreferences)
        {
            visualizer = new(CameraManager.ScreenCamera, graphPreferences);
            menu = new(
                new() 
                { 
                    StrategyInitializer.For<UserBuilder>(),
                    StrategyInitializer.For<CapBuilder>(() => new(Main.Game.State)),
                    StrategyInitializer.For<RandomBuilder>(o => new((float)o[0], (float)o[1], (float)o[2]), 
                        new TextParameter { Name = "Pendant Weight",  Verifier = new IInputVerifier.Float(0, 1) },
                        new TextParameter { Name = "Internal Weight", Verifier = new IInputVerifier.Float(0, 1) },
                        new TextParameter { Name = "Isolated Weight", Verifier = new IInputVerifier.Float(0, 1) }
                    ),
                    StrategyInitializer.For<ConstrainedRandomBuilder>(o => new((int)o[0]), 
                        new TextParameter { Name = "Node Count", Verifier = new IInputVerifier.Integer(2, 40) }
                    ),
                    StrategyInitializer.For<PolygonBuilder>(o => new((int)o[0], Main.Game.State),
                        new TextParameter { Name = "Side Count", Verifier = new IInputVerifier.Integer(min: 3) }
                    ),
                }, 
                new() 
                { 
                    StrategyInitializer.For<UserPainter>(),
                    StrategyInitializer.For<RandomPainter>(),
                    StrategyInitializer.For<AlternatingPainter>(),
                    StrategyInitializer.For<LengthyPainter>()
                }
            );

            // var painterObj = GameObject.Find("Painter Select").GetComponent<Dropdown>();
            // var builderObj = GameObject.Find("Builder Select").GetComponent<Dropdown>();

            // painter = new(painterObj,
            //     ("User", () => new UserPainter()),
            //     ("Random", () => new RandomPainter()),
            //     ("Alternating", () => new AlternatingPainter())
            // );
            // builder = new(builderObj,
            //     ("User", () => new UserBuilder()),
            //     ("Random", () => new RandomBuilder(0.5f, 0.4f, 0.1f)),
            //     ("Constrained", () => new ConstrainedRandomBuilder(15))
            // );

            startRealtimeGameButton = GameObject.Find("Enter Realtime Game").GetComponent<Button>();
            startBulkGameButton = GameObject.Find("Enter Bulk Game").GetComponent<Button>();

            startRealtimeGameButton.onClick.AddListener(() => 
            {
                if(!menu.ValidParameters) return;

                visualizing = false;
                Main.GameBehaviour.StartGame(20, menu.Builder, menu.Painter);
                IBehavior.SwitchTo(Main.GameBehaviour);
            });

            startBulkGameButton.onClick.AddListener(() =>
            {
                if(!menu.ValidParameters) return;

                visualizing = true;
                InitAfterGather(Main.Game.SimulateMany(1, 10, 1, menu.Builder, menu.Painter, 30));
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
            Debug.Log(data.Count);

            visualizer.AddCurve(data, 0f);
        }

        public override void Loop(InputData input)
        {
            menu.UpdateWheels(input);
            menu.Draw();

            if (visualizing) { visualizer.UpdateInput(input.scr, input.mouse); visualizer.Draw(); }
        }

        public override void OnEnter()
        {
            startRealtimeGameButton.gameObject.SetActive(true);
            startBulkGameButton.gameObject.SetActive(true);

            menu.ShowActiveTextInputs();
        }

        public override void OnExit()
        {
            startRealtimeGameButton.gameObject.SetActive(false);
            startBulkGameButton.gameObject.SetActive(false);

            menu.HideAllTextInputs();
        }
    }
}