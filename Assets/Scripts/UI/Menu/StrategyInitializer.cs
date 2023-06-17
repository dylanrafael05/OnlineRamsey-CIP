using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Ramsey.Gameplayer;

using static Unity.Mathematics.math;
using System.Text.RegularExpressions;
using Ramsey.Utilities.UI;

namespace Ramsey.UI
{
    public sealed class StrategyInitializer<T> : IStrategyInitializer<T> where T : IPlayer
    {
        public StrategyInitializer(Func<object[], T> construct, params TextParameter[] parameters)
        {
            Parameters = parameters;
            Construct = construct;

            Name = string.Join(
                ' ', 
                Regex.Replace(typeof(T).Name, @"[A-Z][a-z0-9]+", m => m.Value + " ")
                    .TrimEnd()
                    .Split(' ')
                    .SkipLast(1)
            );
        }

        public IReadOnlyList<TextParameter> Parameters { get; }
        public Func<object[], T> Construct { get; }

        private InputBox[] inputs;
        public IReadOnlyList<InputBox> Inputs => inputs;

        public void ShowTextInputs(float2 knobPos, float inputDistance)
        {
            if(inputs is null)
            {
                inputs = Parameters.Select((param, i) => 
                    Textboxes.CreateTextbox(
                        knobPos + float2(inputDistance, i * 1f * MathUtils.TAU / Parameters.Count).ToCartesian(), 
                        float2(1f),
                        param.Name,
                        param.Verifier
                    )
                ).ToArray();
            }

            foreach(var i in inputs)
                i.GameObject.SetActive(true);
        }

        public void HideTextInputs()
        {
            if(inputs != null)
                foreach(var i in inputs)
                    i.GameObject.SetActive(false);
        }

        public bool InputIsValid()
        {
            for(int i = 0; i < Parameters.Count; i++)
            {
                if(!Inputs[i].InputValid)
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
                objs[i] = Inputs[i].Input;
            }

            return Construct(objs);
        }

        public string Name { get; set; }
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
}
