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
        
        public MenuManager(List<IStrategyInitializer<Builder>> builderInitializers, List<IStrategyInitializer<Painter>> painterInitializers, float2? tickDim = null, float drawSize = 1f, float inputDistance = 1f, float wheelRadiusBuilder = 0.35f, float wheelRadiusPainter = 0.2f, float wheelThickness = .03f, float knobRadius = .05f)
        {
            this.builderInitializers = builderInitializers;
            this.painterInitializers = painterInitializers;

            builderSelect = new(wheelRadiusBuilder, wheelThickness, tickDim ?? new(0.02f, 0.1f), builderInitializers.Count, knobRadius);
            painterSelect = new(wheelRadiusPainter, wheelThickness, tickDim ?? new(0.02f, 0.1f), painterInitializers.Count, knobRadius);

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
            int curr = wheel.Update(input.mouse, input.lmb, input.lmbp);

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
        float currentContinuousTick;
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
            material.SetColor("_NodeColor", Color.white);
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
            float2 pos = float2(radius, (MathUtils.TAU * (currentContinuousTick+0.5f) / tickCount)).ToCartesian();
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
            currentContinuousTick = (polar.y - partitionSize*.5f) / partitionSize;
            if (abs(rtheta) > tickCollisionSize * .5f) return CurrentTick;

            //
            float id = (polar.y - rtheta - partitionSize * .5f) / partitionSize;
            CurrentTick = (int) id;
            currentContinuousTick = CurrentTick;

            return CurrentTick;
        }

        public void Draw()
        {

            material.SetFloat("_NodeLocation", currentContinuousTick);

            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.WheelSelectTransform.localToWorldMatrix, material, UnityReferences.ScreenLayer);

        }

    }
}
