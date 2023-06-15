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

        DropdownWrapper<Painter> painter;
        DropdownWrapper<Builder> builder;

        Button game;

        public MenuBehavior(GraphPreferences graphPreferences)
        {
            visualizer = new(CameraManager.BoardCamera, graphPreferences);

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

            game = GameObject.Find("Enter Game").GetComponent<Button>();

            game.onClick.AddListener(() => 
            {
                Main.GameBehaviour.StartGame(10, builder.Selected, painter.Selected);
                IBehavior.SwitchTo(Main.GameBehaviour);
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

        }

        public override void Loop(InputData input)
        {
        }

        public override void OnEnter()
        {
            painter.UI.gameObject.SetActive(true);
            builder.UI.gameObject.SetActive(true);

            game.gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            painter.UI.gameObject.SetActive(false);
            builder.UI.gameObject.SetActive(false);

            game.gameObject.SetActive(false);
        }
    }
}