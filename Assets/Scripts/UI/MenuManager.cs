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
using UnityEngine.Assertions;

using TextInput = TMPro.TMP_InputField;

namespace Ramsey.UI
{
    public interface IParameterVerifier
    {
        bool IsValid(string str);
        object Parse(string str);

        public class Float : IParameterVerifier
        {
            public Float(float? min = null, float? max = null)
            {
                Min = min;
                Max = max;
            }

            public float? Min { get; }
            public float? Max { get; }

            public bool IsValid(string str) 
            {
                if(!float.TryParse(str, out var val))
                {
                    return false;
                }

                if(Min != null && val < Min) return false;
                if(Max != null && val > Max) return false;

                return true;
            }

            public object Parse(string str)
                => float.Parse(str);
        }
    }

    public struct StrategyParameter
    {
        public string Name;
        public IParameterVerifier Verifier;
    }

    public interface IStrategyInitializer<out T> where T : IPlayer
    {
        IReadOnlyList<StrategyParameter> Parameters { get; }
        IReadOnlyList<TextInput> TextInputs { get; }

        void SetupTextInputs(float2 knobPos, float inputDistance);
        T Initialize();
    }

    public static class StrategyInitializer
    {
        public static StrategyInitializer<T> Direct<T>() where T : IPlayer, new()
        {
            return new DirectInitializer<T>();
        }

        private class DirectInitializer<T> : StrategyInitializer<T> where T : IPlayer, new()
        {
            protected override T Parse(object[] parameters)
                => new();

            public override IReadOnlyList<StrategyParameter> Parameters => Array.Empty<StrategyParameter>();
        }
    }

    public abstract class StrategyInitializer<T> : IStrategyInitializer<T> where T : IPlayer
    {
        public abstract IReadOnlyList<StrategyParameter> Parameters { get; }

        private TextInput[] inputs;
        public IReadOnlyList<TextInput> TextInputs => inputs;

        public void SetupTextInputs(float2 knobPos, float inputDistance)
        {
            if(inputs is null)
            {
                inputs = Parameters.Select((p, i) => Textboxes.CreateTextbox(knobPos + math.float2(inputDistance, i * 1f * MathUtils.TAU / Parameters.Count).ToCartesian(), math.float2(1f))).ToArray();
            }

            foreach(var i in inputs)
            {
                i.gameObject.SetActive(true);
            }
        }

        protected abstract T Parse(object[] parameters);

        public bool InputIsValid()
        {
            for(int i = 0; i < Parameters.Count; i++)
            {
                if(!Parameters[i].Verifier.IsValid(TextInputs[i].text))
                {
                    return false;
                }
            }

            return true;
        }

        public T Initialize() 
        {
            var objs = new object[Parameters.Count];

            for(int i = 0; i < Parameters.Count; i++)
            {
                var v = Parameters[i].Verifier;
                var t = TextInputs[i].text;

                Assert.IsTrue(v.IsValid(t));
                
                objs[i] = v.Parse(t);
            }

            return Parse(objs);
        }
    }

    public class RandomPainterIntializer : StrategyInitializer<RandomBuilder>
    {
        public override IReadOnlyList<StrategyParameter> Parameters => new StrategyParameter[] 
        {
            new() { Name = "Pendant Weight",  Verifier = new IParameterVerifier.Float(0, 1) },
            new() { Name = "Internal Weight", Verifier = new IParameterVerifier.Float(0, 1) },
            new() { Name = "Isolated Weight", Verifier = new IParameterVerifier.Float(0, 1) }
        };

        protected override RandomBuilder Parse(object[] parameters)
        {
            return new(
                (float)parameters[0], 
                (float)parameters[1], 
                (float)parameters[2]
            );
        }
    }

    public class CapBuilderInitializer : StrategyInitializer<CapBuilder>
    {
        public override IReadOnlyList<StrategyParameter> Parameters => Array.Empty<StrategyParameter>();

        protected override CapBuilder Parse(object[] parameters)
            => new(Main.Game.State);
    }

    public class MenuManager
    {

        IReadOnlyList<IStrategyInitializer<Builder>> builderInitializers;
        IReadOnlyList<IStrategyInitializer<Painter>> painterInitializers;

        WheelSelect builderSelect;
        WheelSelect painterSelect;

        float inputDistance;
        
        public MenuManager(List<IStrategyInitializer<Builder>> builderInitializers, List<IStrategyInitializer<Painter>> painterInitializers, float2? tickDim = null, float drawSize = 1f, float inputDistance = .2f, float wheelRadiusBuilder = 1f, float wheelRadiusPainter = 0.7f, float wheelThickness = .2f, float knobRadius = .1f)
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
                initializers[prev].TextInputs.Foreach(sp => sp.gameObject.SetActive(false));

                var knobPos = wheel.KnobPos;
                initializers[curr].SetupTextInputs(knobPos, inputDistance);
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

        public WheelSelect(float radius, float wheelThickness, float2? tickDim, int tickCount, float knobSize, float tickCollisionSize = 0.1f)
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
            if(tickDim != null) material.SetVector("_TickDim", ((float2) tickDim).xyzw());

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
