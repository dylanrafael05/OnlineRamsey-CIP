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
using System.Text.RegularExpressions;

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

        public class Integer : IParameterVerifier
        {
            public Integer(int? min = null, int? max = null)
            {
                Min = min;
                Max = max;
            }

            public int? Min { get; }
            public int? Max { get; }

            public bool IsValid(string str) 
            {
                if(!int.TryParse(str, out var val))
                {
                    return false;
                }

                if(Min != null && val < Min) return false;
                if(Max != null && val > Max) return false;

                return true;
            }

            public object Parse(string str)
                => int.Parse(str);
        }
    }

    public struct TextParameter
    {
        public string Name;
        public IParameterVerifier Verifier;
    }

    public interface IStrategyInitializer<out T> where T : IPlayer
    {
        IReadOnlyList<TextParameter> Parameters { get; }
        IReadOnlyList<TextInput> TextInputs { get; }

        void SetupTextInputs(float2 knobPos, float inputDistance);
        T Initialize();

        string Name { get; }
    }

    public static class StrategyInitializer
    {
        public static StrategyInitializer<T> For<T>() where T : IPlayer, new()
            => new(o => new());
        
        public static StrategyInitializer<T> For<T>(Func<T> construct) where T : IPlayer
            => new(o => construct());

        public static StrategyInitializer<T> For<T>(Func<object[], T> construct, params TextParameter[] parameters) where T : IPlayer
            => new(construct, parameters);

    }

    public sealed class StrategyInitializer<T> : IStrategyInitializer<T> where T : IPlayer
    {
        public StrategyInitializer(Func<object[], T> construct, params TextParameter[] parameters)
        {
            Parameters = parameters;
            Construct = construct;

            Name = string.Join(' ', Regex.Split(typeof(T).Name, @"[A-Z]?[a-z0-9]+"));
        }

        public IReadOnlyList<TextParameter> Parameters { get; }
        public Func<object[], T> Construct { get; }

        private TextInput[] inputs;
        public IReadOnlyList<TextInput> TextInputs => inputs;

        public void SetupTextInputs(float2 knobPos, float inputDistance)
        {
            if(inputs is null)
            {
                inputs = Parameters.Select((param, i) => 
                {
                    var textbox = Textboxes.CreateTextbox(knobPos + float2(inputDistance, i * 1f * MathUtils.TAU / Parameters.Count).ToCartesian(), float2(1f));
                    textbox.text = param.Name;

                    textbox.onEndEdit.AddListener(str => 
                    {
                        if(!param.Verifier.IsValid(str)) 
                        {
                            textbox.textComponent.color = Color.red;
                        }
                        else 
                        {
                            textbox.textComponent.color = Color.black;
                        }
                    });

                    return textbox;
                }).ToArray();
            }

            foreach(var i in inputs)
                i.gameObject.SetActive(true);
        }

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

            return Construct(objs);
        }

        public string Name { get; set; }
    }

    public class MenuManager
    {

        IReadOnlyList<IStrategyInitializer<Builder>> builderInitializers;
        IReadOnlyList<IStrategyInitializer<Painter>> painterInitializers;

        WheelSelect builderSelect;
        WheelSelect painterSelect;

        float inputDistance;
        
        public MenuManager(List<IStrategyInitializer<Builder>> builderInitializers, List<IStrategyInitializer<Painter>> painterInitializers, float2? tickDim = null, float drawSize = 1f, float inputDistance = .2f, float wheelRadiusBuilder = 0.5f, float wheelRadiusPainter = 0.2f, float wheelThickness = .05f, float knobRadius = .05f)
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
                initializers[prev].TextInputs?.Foreach(sp => sp.gameObject.SetActive(false));

                var knobPos = wheel.KnobPos;
                initializers[curr].SetupTextInputs(knobPos, inputDistance);
            }
        }


        public void Draw()
        {
            builderSelect.Draw();
            painterSelect.Draw();

            TextRenderer.Draw(builderSelect.KnobPos + float2(5, +2), $"Painter = {painterInitializers[painterSelect.CurrentTick].Name}");
            TextRenderer.Draw(builderSelect.KnobPos + float2(5, -2), $"Builder = {builderInitializers[builderSelect.CurrentTick].Name}");
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

        public WheelSelect(float radius, float wheelThickness, float2? tickDim, int tickCount, float knobSize, float tickCollisionSize = 0.3f)
        {
            this.radius = radius;
            this.wheelThickness = wheelThickness;
            this.tickCount = tickCount;
            this.tickCollisionSize = tickCollisionSize;
            this.knobSize = knobSize;

            //
            material = new(Shader.Find("Unlit/UIShaders/WheelSelect"));

            material.SetColor("_Color", Color.white);
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

            material.SetInteger("_NodeLocation", CurrentTick);

            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.WheelSelectTransform.localToWorldMatrix, material, UnityReferences.BoardLayer);

        }

    }
}
