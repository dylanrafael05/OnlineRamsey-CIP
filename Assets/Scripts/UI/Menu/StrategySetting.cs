using Ramsey.Utilities;
using System.Collections.Generic;
using Unity.Mathematics;
using Ramsey.Gameplayer;
using Ramsey.Utilities.UI;
using UnityEngine;

namespace Ramsey.UI
{
    public sealed class StrategySetting<T> : IStrategySetting<T> where T : IPlayer
    {
        public StrategySetting(IStrategyInitializer<T> init)
        {
            Initializer = init;
        }

        public IStrategyInitializer<T> Initializer { get; }

        private InputBox[] inputs;
        public IReadOnlyList<InputBox> Inputs => inputs;

        public void ShowTextInputs()
        {
            if(inputs is null)
            {
                inputs = new InputBox[Initializer.Parameters.Count];

                foreach(var (param, i) in Initializer.Parameters.Indexed())
                {
                    var box = InputBox.Base(param.Name, param.Verifier, param.DefaultValue);

                    var parent = "Painter Params Title";
                    if(typeof(Builder).IsAssignableFrom(typeof(T)))
                        parent = "Builder Params Title";

                    box.MainTransform.SetParent(GameObject.Find("Leftside Menu Content").transform);
                    box.MainTransform.SetSiblingIndex(GameObject.Find(parent).transform.GetSiblingIndex() + 1);
                    box.MainTransform.localPosition = new float3(box.MainTransform.localPosition.xy(), 0);
                    box.MainTransform.localScale = new float3(1.3f, 1.3f, 1f);

                    inputs[i] = box;
                }
            }

            inputs.Foreach(i => i.GameObject.SetActive(true));
        }

        public void HideTextInputs()
        {
            if(inputs != null)
                inputs.Foreach(i => i.GameObject.SetActive(false));
        }

        public bool InputIsValid()
        {
            for(int i = 0; i < Initializer.Parameters.Count; i++)
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
            var objs = new object[Initializer.Parameters.Count];

            for(int i = 0; i < Initializer.Parameters.Count; i++)
            {
                objs[i] = Inputs[i].Input;
            }

            return Initializer.Initialize(objs);
        }
    }
}
