using UnityEngine;

using Text = TMPro.TMP_Text;

namespace Ramsey.Drawing
{
    public static class UnityReferences
    {
        public static readonly Mesh QuadMesh = new()
        {
            vertices = new Vector3[]
            {
                new (-1f, -1f, 0f),
                new (-1f, 1f, 0f),
                new (1f, -1f, 0f),
                new (1f, 1f, 0f)
            },
            triangles = new int[]
            {
                0, 1, 3,
                0, 3, 2
            },
            uv = new Vector2[]
            {
                new (-1f, -1f),
                new (-1f, 1f),
                new (1f, -1f),
                new (1f, 1f)
            }
        };

        public static readonly Bounds Bounds = new(Vector3.zero, Vector3.one * 100f);

        public static RectTransform RecordingTransform { get; private set; }
        public static RectTransform LoadingTransform { get; private set; }
        public static Text GoalText { get; private set; }

        public static Material EdgeMaterial { get; private set; }
        public static Material NodeMaterial { get; private set; }
        public static Material RecorderMaterial { get; private set; }
        public static Material LoadingMaterial { get; private set; }

        public static void Initialize()
        {
            var rgo = GameObject.Find("Recording");
            RecordingTransform = rgo.GetComponent<RectTransform>();

            var lgo = GameObject.Find("Loading");
            LoadingTransform = lgo.GetComponent<RectTransform>();

            GoalText = GameObject.Find("Goal Text").GetComponent<Text>();

            EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
            NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));
            RecorderMaterial = new(Shader.Find("Unlit/UIShaders/RecordingShader"));
            LoadingMaterial = new(Shader.Find("Unlit/UIShaders/LoadingCircle"));
        }
    }
}