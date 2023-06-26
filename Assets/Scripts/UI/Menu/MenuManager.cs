using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Ramsey.Drawing;
using Ramsey.Gameplayer;

using static Unity.Mathematics.math;

using UnityEngine;
using Ramsey.Utilities;
using System;

using Text = TMPro.TMP_Text;

namespace Ramsey.UI
{
    public class MenuManager
    {
        IReadOnlyList<IStrategyInitializer<Builder>> builderInitializers;
        IReadOnlyList<IStrategyInitializer<Painter>> painterInitializers;

        WheelSelect builderSelect;
        WheelSelect painterSelect;

        Text builderParamsTitle;
        Text painterParamsTitle;

        float inputDistance;

        bool firstUpdate;

        public event Action<IStrategyInitializer<Builder>, IStrategyInitializer<Painter>> OnStrategyChanged;
        
        public MenuManager(List<IStrategyInitializer<Builder>> builderInitializers, List<IStrategyInitializer<Painter>> painterInitializers, float drawSize = 1f, float inputDistance = 1f, float wheelRadiusBuilder = 0.35f, float wheelRadiusPainter = 0.2f, float wheelThickness = .029f, float knobRadius = .04f)
        {
            this.builderInitializers = builderInitializers;
            this.painterInitializers = painterInitializers;

            builderSelect = new(wheelRadiusBuilder, wheelThickness, builderInitializers.Count, knobRadius);
            painterSelect = new(wheelRadiusPainter, wheelThickness, painterInitializers.Count, knobRadius);

            this.inputDistance = inputDistance;

            builderParamsTitle = GameObject.Find("Builder Params Title").GetComponent<Text>();
            painterParamsTitle = GameObject.Find("Painter Params Title").GetComponent<Text>();

            firstUpdate = true;
        }

        public void UpdateWheels(InputData input)
        {
            bool strategyChanged = false;

            strategyChanged |= UpdateWheel(input, builderSelect, builderInitializers, false);
            strategyChanged |= UpdateWheel(input, painterSelect, painterInitializers, true);

            if(strategyChanged)
                OnStrategyChanged?.Invoke(BuilderCurrInit, PainterCurrInit);
            
            firstUpdate = false;
        }

        bool UpdateWheel(InputData input, WheelSelect wheel, IReadOnlyList<IStrategyInitializer<IPlayer>> initializers, bool painter)
        {
            int prev = wheel.CurrentTick;
            int curr = wheel.Update(input.mouse, input.lmb, input.lmbp, Time.deltaTime);

            // If Current Tick Changed
            if(prev != curr || firstUpdate)
            {
                initializers[prev].HideTextInputs();

                if(initializers[curr].Parameters.Count > 0)
                {
                    (painter ? painterParamsTitle : builderParamsTitle).gameObject.SetActive(true);
                    (painter ? painterParamsTitle : builderParamsTitle).text = initializers[curr].Name + (painter ? " Painter" : " Builder");

                    initializers[curr].ShowTextInputs();
                }
                else 
                {
                    (painter ? painterParamsTitle : builderParamsTitle).gameObject.SetActive(false);
                }

                return true;
            }

            return false;
        }

        public IStrategyInitializer<Builder> BuilderCurrInit => builderInitializers[builderSelect.CurrentTick];
        public IStrategyInitializer<Painter> PainterCurrInit => painterInitializers[painterSelect.CurrentTick];

        public bool ValidParameters => BuilderCurrInit.InputIsValid() && PainterCurrInit.InputIsValid();

        public Builder ConstructBuilder() => BuilderCurrInit.Initialize();
        public Painter ConstructPainter() => PainterCurrInit.Initialize();

        public void Draw()
        {
            builderSelect.Draw();
            painterSelect.Draw();

            TextRenderer.Draw(builderSelect.KnobPos, BuilderCurrInit.Name, Color.black, screen: true);
            TextRenderer.Draw(painterSelect.KnobPos, PainterCurrInit.Name, Color.black, screen: true);
        }

        public void ShowActiveTextInputs()
        {
            BuilderCurrInit.ShowTextInputs();
            PainterCurrInit.ShowTextInputs();
        }

        public void HideAllTextInputs()
        {
            foreach(var i in builderInitializers) i.HideTextInputs();
            foreach(var i in painterInitializers) i.HideTextInputs();
        }
    }

}
