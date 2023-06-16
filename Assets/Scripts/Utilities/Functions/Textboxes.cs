using Unity.Mathematics;
using UnityEngine;

using TextInput = TMPro.TMP_InputField;

namespace Ramsey.Utilities
{

    public static class Textboxes
    {
        private static bool isSetup;
        private static GameObject inputPrefab;
        private static RectTransform rect;

        private static void Create() 
        {
            inputPrefab = Resources.Load<GameObject>("Prefabs/TextInput");
            rect = GameObject.Find("Menu").GetComponent<RectTransform>();
        }

        public static TextInput CreateTextbox(float2 position, float2 scale) 
        {
            if(!isSetup)
            {
                Create();
                isSetup = true;
            }

            var go = GameObject.Instantiate(inputPrefab);
            var trans = go.GetComponent<RectTransform>();

            trans.SetParent(rect);

            trans.position   = position.xyzV();
            trans.localPosition = new(trans.localPosition.x, trans.localPosition.y, 0f);
            trans.localScale = new Vector3(scale.x, scale.y, 1);

            //trans.SetPositionAndRotation(trans.position, Quaternion.identity);

            return go.GetComponent<TextInput>();
        }
    }
}