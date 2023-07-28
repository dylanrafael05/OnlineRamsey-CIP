using Ramsey.Drawing;
using Ramsey.Gameplayer;
using Ramsey.Screen;
using Ramsey.Utilities;
using Ramsey.Utilities.UI;
using Ramsey.Visualization;
using SimpleFileBrowser;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Ramsey.UI
{
    // public class HeadlessInteractionBehaviour : Behavior
    // {

    // }

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

        readonly Toggle runBulk, bulkCompactExport;

        readonly GameObject standardMenu;
        readonly InputBox standardCount, standardDelay;
        
        readonly GameObject bulkMenu;
        readonly InputBox bulkStart, bulkEnd, bulkStep, bulkAtt;

        public MenuBehavior(GraphPreferences graphPreferences)
        {
            visualizer = new(CameraManager.ScreenCamera, graphPreferences);
            menu = new();

            menuObj = GameObject.Find("Menu");

            runBulk = GameObject.Find("Run in Bulk").GetComponent<Toggle>();
            bulkCompactExport = GameObject.Find("Compact Export").GetComponent<Toggle>();

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
                if (IBehavior.IsSwitching) return;

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
                    if(!InputBox.AllValid(standardCount, standardDelay))
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
                var canSetDelay = !painter.Initializer.IsAutomated || !builder.Initializer.IsAutomated;

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
            //export
            FileUtils.PickFileToOperate(
                path => File.WriteAllText(path, data.ToString(bulkCompactExport.isOn)), 
                "bulkData.txt", 
                "Save the Generated Matchup Data", 
                "Save Data", 
                new FileBrowser.Filter("Text File", ".txt", ".csv"));
        }

        public override void Loop(InputData input)
        {
            menu.UpdateWheels(input);
            menu.Draw();
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