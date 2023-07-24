using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Ramsey.Drawing;
using Ramsey.Gameplayer;

using static Unity.Mathematics.math;

using UnityEngine;
using Ramsey.Utilities;
using System;

using Text = TMPro.TMP_Text;
using System.Linq;

namespace Ramsey.UI
{
    public class MenuManager
    {
        IReadOnlyList<IStrategySetting<Builder>> builderSettings;
        IReadOnlyList<IStrategySetting<Painter>> painterSettings;

        WheelSelect builderSelect;
        WheelSelect painterSelect;

        Text builderParamsTitle;
        Text painterParamsTitle;

        float inputDistance;

        bool firstUpdate;

        public event Action<IStrategySetting<Builder>, IStrategySetting<Painter>> OnStrategyChanged;
        
        public MenuManager(float drawSize = 1f, float inputDistance = 1f, float wheelRadiusBuilder = 0.35f, float wheelRadiusPainter = 0.2f, float wheelThickness = .029f, float knobRadius = .04f)
        {
            builderSettings = StrategyInitializer.BuilderInitializers.Select(b => new StrategySetting<Builder>(b)).ToArray();
            painterSettings = StrategyInitializer.PainterInitializers.Select(b => new StrategySetting<Painter>(b)).ToArray();

            builderSelect = new(wheelRadiusBuilder, wheelThickness, builderSettings.Count, knobRadius);
            painterSelect = new(wheelRadiusPainter, wheelThickness, painterSettings.Count, knobRadius);

            this.inputDistance = inputDistance;

            builderParamsTitle = GameObject.Find("Builder Params Title").GetComponent<Text>();
            painterParamsTitle = GameObject.Find("Painter Params Title").GetComponent<Text>();

            firstUpdate = true;
        }

        public void UpdateWheels(InputData input)
        {
            bool strategyChanged = false;

            strategyChanged |= UpdateWheel(input, builderSelect, builderSettings, false);
            strategyChanged |= UpdateWheel(input, painterSelect, painterSettings, true);

            if(strategyChanged)
                OnStrategyChanged?.Invoke(BuilderCurrInit, PainterCurrInit);
            
            firstUpdate = false;
        }

        bool UpdateWheel(InputData input, WheelSelect wheel, IReadOnlyList<IStrategySetting<IPlayer>> initializers, bool painter)
        {
            int prev = wheel.CurrentTick;
            int curr = wheel.Update(input.mouse, input.lmb, input.lmbp, Time.deltaTime);

            // If Current Tick Changed
            if(prev != curr || firstUpdate)
            {
                initializers[prev].HideTextInputs();

                StrategyNameDisplayer.UpdateName(initializers[curr].Initializer.Name, painter);

                if(initializers[curr].Initializer.Parameters.Count > 0)
                {
                    (painter ? painterParamsTitle : builderParamsTitle).gameObject.SetActive(true);
                    (painter ? painterParamsTitle : builderParamsTitle).text = initializers[curr].Initializer.Name + (painter ? " Painter" : " Builder");

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

        public IStrategySetting<Builder> BuilderCurrInit => builderSettings[builderSelect.CurrentTick];
        public IStrategySetting<Painter> PainterCurrInit => painterSettings[painterSelect.CurrentTick];

        public bool ValidParameters => BuilderCurrInit.InputIsValid() && PainterCurrInit.InputIsValid();

        public Builder ConstructBuilder() => BuilderCurrInit.Initialize();
        public Painter ConstructPainter() => PainterCurrInit.Initialize();

        public void Draw()
        {
            builderSelect.Draw();
            painterSelect.Draw();

            /*TextRenderer.Draw(builderSelect.KnobPos, BuilderCurrInit.Initializer.Name, Color.black, screen: true);
            TextRenderer.Draw(painterSelect.KnobPos, PainterCurrInit.Initializer.Name, Color.black, screen: true);*/
        }

        public void ShowActiveTextInputs()
        {
            BuilderCurrInit.ShowTextInputs();
            PainterCurrInit.ShowTextInputs();
        }

        public void HideAllTextInputs()
        {
            foreach(var i in builderSettings) i.HideTextInputs();
            foreach(var i in painterSettings) i.HideTextInputs();
        }
    }

}
