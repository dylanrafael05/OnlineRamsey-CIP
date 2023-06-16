using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Ramsey.Drawing;
using Ramsey.Gameplayer;

using static Unity.Mathematics.math;

namespace Ramsey.UI
{
    public class MenuManager
    {
        IReadOnlyList<IStrategyInitializer<Builder>> builderInitializers;
        IReadOnlyList<IStrategyInitializer<Painter>> painterInitializers;

        WheelSelect builderSelect;
        WheelSelect painterSelect;

        float inputDistance;
        
        public MenuManager(List<IStrategyInitializer<Builder>> builderInitializers, List<IStrategyInitializer<Painter>> painterInitializers, float2? tickDim = null, float drawSize = 1f, float inputDistance = 1f, float wheelRadiusBuilder = 0.5f, float wheelRadiusPainter = 0.2f, float wheelThickness = .05f, float knobRadius = .05f)
        {
            this.builderInitializers = builderInitializers;
            this.painterInitializers = painterInitializers;

            builderSelect = new(wheelRadiusBuilder, wheelThickness, tickDim, builderInitializers.Count, knobRadius);
            painterSelect = new(wheelRadiusPainter, wheelThickness, tickDim, painterInitializers.Count, knobRadius);

            this.inputDistance = inputDistance;
        }

        public void UpdateWheels(InputData input)
        {
            UpdateWheel(input, builderSelect, builderInitializers);
            UpdateWheel(input, painterSelect, painterInitializers);
        }

        void UpdateWheel(InputData input, WheelSelect wheel, IReadOnlyList<IStrategyInitializer<IPlayer>> initializers)
        {
            int prev = wheel.CurrentTick;
            int curr = wheel.Update(input.mouse, input.lmb, input.lmbp);

            if(prev != curr)
            {
                initializers[prev].HideTextInputs();

                var knobPos = wheel.KnobPos;
                initializers[curr].ShowTextInputs(knobPos, inputDistance);
            }
        }

        public IStrategyInitializer<Builder> BuilderInit => builderInitializers[builderSelect.CurrentTick];
        public IStrategyInitializer<Painter> PainterInit => painterInitializers[painterSelect.CurrentTick];

        public bool ValidParameters => BuilderInit.InputIsValid() && PainterInit.InputIsValid();

        public Builder Builder => BuilderInit.Initialize();
        public Painter Painter => PainterInit.Initialize();

        public void Draw()
        {
            builderSelect.Draw();
            painterSelect.Draw();

            TextRenderer.Draw(builderSelect.KnobPos + float2(5, +2), $"Painter = {painterInitializers[painterSelect.CurrentTick].Name}");
            TextRenderer.Draw(builderSelect.KnobPos + float2(5, -2), $"Builder = {builderInitializers[builderSelect.CurrentTick].Name}");
        }

        public void ShowActiveTextInputs()
        {
            BuilderInit.ShowTextInputs(builderSelect.KnobPos, inputDistance);
            PainterInit.ShowTextInputs(painterSelect.KnobPos, inputDistance);
        }

        public void HideAllTextInputs()
        {
            foreach(var i in builderInitializers) i.HideTextInputs();
            foreach(var i in painterInitializers) i.HideTextInputs();
        }
    }
}
