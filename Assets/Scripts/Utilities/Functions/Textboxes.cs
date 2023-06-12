using Unity.Mathematics;
using UnityEngine;

using TextInput = TMPro.TMP_InputField;

namespace Ramsey.Utilities
{
    public static class Textboxes
    {
        private static GameObject inputPrefab;
        private static Canvas canvas;

        public static void Create() 
        {
            inputPrefab = Resources.Load<GameObject>("Prefabs/InputPrefab");
            canvas = GameObject.Find("Screen Canvas").GetComponent<Canvas>();
        }

        public static TextInput CreateTextbox(float2 position, float2 scale) 
        {
            var go = GameObject.Instantiate(inputPrefab, position.xyzV(), Quaternion.identity, canvas.transform);
            go.transform.localScale = new Vector3(scale.x, scale.y, 1);

            return go.GetComponent<TextInput>();
        }
    }
}