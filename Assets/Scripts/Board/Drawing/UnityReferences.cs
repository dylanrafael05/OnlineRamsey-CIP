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
        public static RectTransform VisualizerGraphTransform { get; private set; }
        public static Text GoalText { get; private set; }
        public static Text TurnText { get; private set; }
        public static Text OverText { get; private set; }
        public static Text ConfirmMenuText { get; private set; }
        public static MeshRenderer BackgroundRenderer { get; private set; }
        public static MeshRenderer TransitionRenderer { get; private set; }

        public static Material EdgeMaterial { get; private set; }
        public static Material NodeMaterial { get; private set; }
        public static Material RecorderMaterial { get; private set; }
        public static Material LoadingMaterial { get; private set; }
        public static Material ScreenMaterial { get; private set; }
        public static Material TransitionMaterial { get; private set; }
        public static Material BackgroundMenuMaterial { get; private set; }
        public static Material BackgroundMaterial { get; private set; }
        public static Material WheelMaterial { get; private set; }

        public static int BoardLayer { get; private set; }
        public static int ScreenLayer { get; private set; }

        public static void Initialize()
        {

            RecordingTransform = GameObject.Find("Recording").GetComponent<RectTransform>();
            LoadingTransform = GameObject.Find("Loading").GetComponent<RectTransform>();
            WheelSelectTransform = GameObject.Find("Wheel Select").GetComponent<RectTransform>();
            VisualizerGraphTransform = GameObject.Find("Visualizer Graph").GetComponent<RectTransform>();

            GoalText = GameObject.Find("Goal Text").GetComponent<Text>();
            TurnText = GameObject.Find("Turn Text").GetComponent<Text>();
            OverText = GameObject.Find("Over Text").GetComponent<Text>();
            ConfirmMenuText = GameObject.Find("Confirm Leave").GetComponent<Text>();

            GoalText.gameObject.SetActive(false);
            TurnText.gameObject.SetActive(false);
            OverText.gameObject.SetActive(false);
            ConfirmMenuText.gameObject.SetActive(false);

            EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
            NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));
            RecorderMaterial = new(Shader.Find("Unlit/UIShaders/RecordingShader"));
            LoadingMaterial = new(Shader.Find("Unlit/UIShaders/LoadingCircle"));
            ScreenMaterial = new(Shader.Find("Unlit/Fullscreen/Pulse"));
            BackgroundMaterial = new(Shader.Find("Unlit/Screen/Background"));
            BackgroundMenuMaterial = new(Shader.Find("Unlit/Screen/MenuBackground"));

            WheelMaterial = new(Shader.Find("Unlit/UIShaders/WheelSelect"));
            TransitionMaterial = new(Shader.Find("Unlit/Screen/Transition"));

            EdgeMaterial.enableInstancing = true;
            NodeMaterial.enableInstancing = true;

            GameObject.Find("Menu Background").GetComponent<MeshRenderer>().material = BackgroundMenuMaterial;

            BackgroundRenderer = GameObject.Find("Background").GetComponent<MeshRenderer>();
            BackgroundRenderer.material = BackgroundMaterial;

            BoardLayer = LayerMask.NameToLayer("Board");
            ScreenLayer = LayerMask.NameToLayer("Screen");

            //TransitionRenderer = GameObject.Find("Transition").GetComponent<MeshRenderer>();
            //TransitionRenderer.material = TransitionMaterial;
        }
    }
}