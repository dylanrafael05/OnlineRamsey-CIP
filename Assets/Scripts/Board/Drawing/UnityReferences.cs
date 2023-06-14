using UnityEngine;

using Text = TMPro.TMP_Text;

namespace Ramsey.Drawing
{
    public static class UnityReferences
    {
        public static readonly Bounds Bounds = new(Vector3.zero, Vector3.one * 100f);

        public static RectTransform RecordingTransform { get; private set; }
        public static RectTransform LoadingTransform { get; private set; }
        public static RectTransform WheelSelectTransform { get; private set; }
        public static Text GoalText { get; private set; }
        public static Text TurnText { get; private set; }
        public static Text OverText { get; private set; }
        public static MeshRenderer BackgroundRenderer { get; private set; }

        public static Material EdgeMaterial { get; private set; }
        public static Material NodeMaterial { get; private set; }
        public static Material RecorderMaterial { get; private set; }
        public static Material LoadingMaterial { get; private set; }
        public static Material ScreenMaterial { get; private set; }
        public static Material BackgroundMaterial { get; private set; }

        public static void Initialize()
        {
            var rgo = GameObject.Find("Recording");
            RecordingTransform = rgo.GetComponent<RectTransform>();

            var lgo = GameObject.Find("Loading");
            LoadingTransform = lgo.GetComponent<RectTransform>();

            // var wso = GameObject.Find("WheelSelect");
            // WheelSelectTransform = wso.GetComponent<RectTransform>();

            GoalText = GameObject.Find("Goal Text").GetComponent<Text>();
            TurnText = GameObject.Find("Turn Text").GetComponent<Text>();
            OverText = GameObject.Find("Over Text").GetComponent<Text>();

            OverText.gameObject.SetActive(false);

            EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
            NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));
            RecorderMaterial = new(Shader.Find("Unlit/UIShaders/RecordingShader"));
            LoadingMaterial = new(Shader.Find("Unlit/UIShaders/LoadingCircle"));
            ScreenMaterial = new(Shader.Find("Unlit/Fullscreen/Pulse"));
            BackgroundMaterial = new(Shader.Find("Unlit/Screen/Background"));

            EdgeMaterial.enableInstancing = true;
            NodeMaterial.enableInstancing = true;

            BackgroundRenderer = GameObject.Find("Background").GetComponent<MeshRenderer>();
            BackgroundRenderer.material = BackgroundMaterial;
        }
    }
}