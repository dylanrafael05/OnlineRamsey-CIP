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

    internal class WheelSelect
    {

        // Prefs
        readonly float radius;
        readonly float wheelThickness;
        readonly float2 tickDim;
        readonly int tickCount;

        readonly float tickCollisionSize;

        readonly float knobSize;

        // Current
        public int CurrentTick { get; private set; }
        public float2 KnobPos 
            => UnityReferences.WheelSelectTransform.position.xy() 
                + UnityReferences.WheelSelectTransform.lossyScale.xy() * float2(radius, (CurrentTick + .5f) * MathUtils.TAU / tickCount).ToCartesian();
        bool hasNode;

        //
        Material material;

        public WheelSelect(float radius, float wheelThickness, float2? tickDim, int tickCount, float knobSize, float tickCollisionSize = 0.3f)
        {
            this.radius = radius;
            this.wheelThickness = wheelThickness;
            this.tickCount = tickCount;
            this.tickCollisionSize = tickCollisionSize;
            this.knobSize = knobSize;

            //
            material = new(Shader.Find("Unlit/UIShaders/WheelSelect"));

            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_WheelRadius", radius);
            material.SetFloat("_WheelThickness", wheelThickness);

            material.SetInteger("_TickCount", tickCount);
            if(tickDim != null) material.SetVector("_TickDim", ((float2) tickDim).xyzw());

            material.SetFloat("_NodeRadius", knobSize);
        }

        bool CollideKnob(float2 mouse)
        {
            //mouse -= UnityReferences.WheelSelectTransform.position.xy(); Debug.Log(mouse);
            //mouse /= UnityReferences.WheelSelectTransform.localScale.xy();
            mouse = UnityReferences.WheelSelectTransform.InverseTransformPoint(mouse.xyz()).xy(); //(UnityReferences.WheelSelectTransform.worldToLocalMatrix * (Vector4) mouse.xyzw(0f, 1f)).xy();//
            float2 pos = float2(radius, (MathUtils.TAU * (CurrentTick*1f+0.5f) / tickCount)).ToCartesian();
            return (length(mouse - pos) - knobSize) <= 0f;
        }
        public int Update(float2 mouse, bool isDown, bool isPress)
        {
            //
            hasNode |= CollideKnob(mouse) && isPress;
            hasNode &= isDown;

            if (!hasNode) return CurrentTick;

            //
            float2 polar = UnityReferences.WheelSelectTransform.InverseTransformPoint(mouse.xyz()).xy().ToPolar();
            float partitionSize = MathUtils.TAU / tickCount;
            float rtheta = fmod(polar.y, partitionSize)-partitionSize*.5f;
            if (abs(rtheta) > tickCollisionSize * .5f) return CurrentTick;

            //
            float id = (polar.y - rtheta - partitionSize * .5f) / partitionSize;
            CurrentTick = (int) id;

            return CurrentTick;
        }

        public void Draw()
        {

            material.SetInteger("_NodeLocation", CurrentTick);

            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.WheelSelectTransform.localToWorldMatrix, material, UnityReferences.BoardLayer);

        }

    }
}
