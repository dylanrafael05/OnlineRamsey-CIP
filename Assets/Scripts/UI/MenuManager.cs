using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Ramsey.Drawing;
using Ramsey.Gameplayer;

using static Unity.Mathematics.math;

using TextInput = TMPro.TMP_InputField;

namespace Ramsey.UI
{

    public struct StrategyParameter
    {
        public string Name;
        public TextInput Text;
    }

    public interface IStrategyInitializer<T> where T : IPlayer
    {
        StrategyParameter[] Params { get; set; }
        T Initialize(StrategyParameter[] filledParams);
        static void Fail(string message) => Debug.Log("Strategy Failed to Initialize - Message: " + message);
    }

    public class MenuManager
    {


        IReadOnlyList<IStrategyInitializer<Builder>> builderInitializers;
        IReadOnlyList<IStrategyInitializer<Painter>> painterInitializers;

        WheelSelect builderSelect;
        WheelSelect painterSelect;

        float inputDistance;
        
        public MenuManager(List<IStrategyInitializer<Builder>> builderInitializers, List<IStrategyInitializer<Painter>> painterInitializers, float2 tickDim, float drawSize = 1f, float inputDistance = .2f, float wheelRadiusBuilder = 1f, float wheelRadiusPainter = 0.7f, float wheelThickness = .2f, float knobRadius = .1f)
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

        void UpdateWheel<T>(InputData input, WheelSelect wheel, IReadOnlyList<IStrategyInitializer<T>> initializers) where T : IPlayer
        {
            int prev = wheel.CurrentTick;
            int curr = wheel.Update(input.mouse, input.lmb, input.lmbp);

            if(prev != curr)
            {
                initializers[prev].Params.Foreach(sp => GameObject.Destroy(sp.Text));

                var knobPos = wheel.KnobPos;
                initializers[curr].Params.ForEachIndex((sp, i) =>
                {
                    sp.Text = Textboxes.CreateTextbox(knobPos + float2(inputDistance, i * 1f * MathUtils.TAU / initializers.Count).ToCartesian(), float2(1f));
                });
            }
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
        public float2 KnobPos => float2(radius, CurrentTick * MathUtils.TAU / tickCount).ToCartesian();
        bool hasNode;

        //
        Material material;

        public WheelSelect(float radius, float wheelThickness, float2 tickDim, int tickCount, float knobSize, float tickCollisionSize = 0.1f)
        {
            this.radius = radius;
            this.wheelThickness = wheelThickness;
            this.tickCount = tickCount;
            this.tickCollisionSize = tickCollisionSize;
            this.knobSize = knobSize;

            //
            material = new(Shader.Find("Unlit/UIShaders/WheelShader"));

            material.SetVector("_Color", Color.white);
            material.SetFloat("_Radius", radius);

            material.SetInt("_TickCount", tickCount);
            material.SetVector("_TickDim", tickDim.xyzw());

            material.SetFloat("_NodeRadius", knobSize);
        }

        bool CollideKnob(float2 mouse)
        {
            mouse -= UnityReferences.WheelSelectTransform.position.xy();
            float2 pos = float2(radius, (PI * 2f * CurrentTick / tickCount)).ToCartesian();
            return (length(mouse - pos) - knobSize) <= 0f;
        }
        public int Update(float2 mouse, bool isDown, bool isPress)
        {
            //
            hasNode |= hasNode && isDown;
            hasNode |= CollideKnob(mouse) && isPress;

            if (!hasNode) return CurrentTick;

            //
            float2 polar = mouse.ToPolar();
            float partitionSize = 2f * PI / tickCount;
            float rtheta = fmod(polar.y, partitionSize)-partitionSize*.5f;

            if (abs(rtheta) > tickCollisionSize * .5f) return CurrentTick;

            //
            float id = (polar.y - rtheta - partitionSize * .5f) / partitionSize;
            CurrentTick = (int) id;

            return CurrentTick;
        }

        public void Draw()
        {

            material.SetInt("_NodeLocation", CurrentTick);
            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.WheelSelectTransform.localToWorldMatrix, UnityReferences.WheelMaterial, UnityReferences.BoardLayer);

        }

    }
}
