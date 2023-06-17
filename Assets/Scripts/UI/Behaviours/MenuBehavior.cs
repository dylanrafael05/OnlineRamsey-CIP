using Ramsey.Gameplayer;
using Ramsey.Screen;
using Ramsey.Utilities;
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
        readonly Visualizer visualizer;
        bool visualizing = false;
        bool prevCouldSetDelay = false;

        //
        readonly MenuManager menu;
        readonly GameObject menuObj;

        readonly Button goAhead;

        readonly Toggle runBulk;

        readonly GameObject standardMenu;
        readonly InputBox standardCount, standardDelay;
        
        readonly GameObject bulkMenu;
        readonly InputBox bulkStart, bulkEnd, bulkStep, bulkAtt;

        public MenuBehavior(GraphPreferences graphPreferences)
        {
            visualizer = new(CameraManager.ScreenCamera, graphPreferences);
            menu = new(
                new() 
                { 
                    StrategyInitializer.For<UserBuilder>(),
                    StrategyInitializer.For<CapBuilder>(() => new(Main.Game.State)),
                    StrategyInitializer.For<RandomBuilder>(o => new((float)o[0], (float)o[1], (float)o[2]), 
                        new TextParameter { Name = "Pendant Weight",  Verifier = new IInputVerifier.Float(0, 1), DefaultValue = "0.5" },
                        new TextParameter { Name = "Internal Weight", Verifier = new IInputVerifier.Float(0, 1), DefaultValue = "0.4" },
                        new TextParameter { Name = "Isolated Weight", Verifier = new IInputVerifier.Float(0, 1), DefaultValue = "0.1" }
                    ),
                    StrategyInitializer.For<ConstrainedRandomBuilder>(o => new((int)o[0]), 
                        new TextParameter { Name = "Node Count", Verifier = new IInputVerifier.Integer(2, 40), DefaultValue = "20" }
                    ),
                    StrategyInitializer.For<PolygonBuilder>(o => new((int)o[0], Main.Game.State),
                        new TextParameter { Name = "Side Count", Verifier = new IInputVerifier.Integer(3), DefaultValue = "8" }
                    ),
                }, 
                new() 
                { 
                    StrategyInitializer.For<UserPainter>(),
                    StrategyInitializer.For<RandomPainter>(),
                    StrategyInitializer.For<AlternatingPainter>(),
                    StrategyInitializer.For<LengthyPainter>(),
                    StrategyInitializer.For<FavoriteColorPainter>(o => new((int)o[0]), 
                        new TextParameter { Name = "Color", Verifier = new IInputVerifier.Integer(0, 2), DefaultValue = "0" }
                    ),
                }
            );

            menuObj = GameObject.Find("Menu");

            runBulk = GameObject.Find("Run in Bulk").GetComponent<Toggle>();

            standardMenu = GameObject.Find("Standard Options");
            standardCount = InputBox.Find("Game Length", new IInputVerifier.Integer(min: 1, max: 255), "10");
            standardDelay = InputBox.Find("Delay Between Turns", new IInputVerifier.Float(min: 0, max: 5), "0.2");

            bulkMenu = GameObject.Find("Bulk Options");
            bulkStart = InputBox.Find("Start", new IInputVerifier.Integer(min: 1, max: 255), "1");
            bulkEnd = InputBox.Find("End", new IInputVerifier.Integer(min: 1, max: 255), "10");
            bulkStep = InputBox.Find("Step", new IInputVerifier.Integer(min: 1, max: 255), "1");
            bulkAtt = InputBox.Find("Attempts Per", new IInputVerifier.Integer(min: 1), "20");

            goAhead = GameObject.Find("Menu Go").GetComponent<Button>();

            goAhead.onClick.AddListener(() => 
            {
                if(!menu.ValidParameters) return;

                if(runBulk.isOn)
                {
                    if(!InputBox.AllValid(bulkStart, bulkEnd, bulkStep, bulkAtt))
                        return;

                    visualizing = true;
                    InitAfterGather(Main.Game.SimulateMany(
                        (int)bulkStart.Input,
                        (int)bulkEnd.Input,
                        (int)bulkStep.Input,
                        menu.ConstructBuilder(),
                        menu.ConstructPainter(),
                        (int)bulkAtt.Input
                    ));
                }
                else 
                {
                    if(!InputBox.AllValid(standardCount))
                        return;
                    
                    visualizing = false;
                    Main.Game.Delay = (float)standardDelay.Input;
                    Main.GameBehaviour.StartGame(
                        (int)standardCount.Input, 
                        menu.ConstructBuilder(), 
                        menu.ConstructPainter()
                    );
                    IBehavior.SwitchTo(Main.GameBehaviour);
                }
            });

            runBulk.isOn = false;
            bulkMenu.SetActive(false);

            runBulk.onValueChanged.AddListener(bulk => 
            {
                if(bulk)
                {
                    bulkMenu.SetActive(true);
                    standardMenu.SetActive(false);
                }
                else 
                {
                    bulkMenu.SetActive(false);
                    standardMenu.SetActive(true);
                }
            });

            menu.OnStrategyChanged += (painter, builder) => 
            {
                var canSetDelay = !painter.IsAutomated || !builder.IsAutomated;

                if(prevCouldSetDelay && !canSetDelay)
                {
                    standardDelay.Textbox.DeactivateInputField(true);
                }
                else if(!prevCouldSetDelay && canSetDelay)
                {
                    standardDelay.Textbox.ActivateInputField();
                }

                prevCouldSetDelay = canSetDelay;
            };
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

            if (visualizing) 
            { 
                visualizer.UpdateInput(input.scr, input.mouse); 
                visualizer.Draw(); 
            }
        }

        public override void OnEnter()
        {
            menuObj.SetActive(true);
            menu.ShowActiveTextInputs();
        }

        public override void OnExit()
        {
            menuObj.SetActive(false);
            menu.HideAllTextInputs();
        }
    }
}