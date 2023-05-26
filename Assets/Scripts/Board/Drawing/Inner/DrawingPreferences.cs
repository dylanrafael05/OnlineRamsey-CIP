using UnityEngine;

namespace Ramsey.Drawing
{
    public struct DrawingPreferences
    {

        public Color nullColor;
        public Color[] colors;

        public float edgeThickness;
        public float nodeRadius;
        public float highlightRadius;

        public float highlightThickness;

        public float loadingCircleOuterRadius;
        public float loadingCircleInnerRadius;

        public Color nodeColor;
        public Color highlightColor;

        public Color recorderColor;

        public Color TypeToColor(int type)
            => type == -1 ? nullColor : colors[type];

        public Matrix4x4 RecorderTransform;
        public Matrix4x4 LoadingCircleTransform;

        public void UniformPreferences()
        {
            NodeMaterial.SetFloat("_Radius", nodeRadius);
            NodeMaterial.SetFloat("_HighlightRadius", highlightRadius);

            NodeMaterial.SetFloat("_HighlightThickness", highlightThickness);

            NodeMaterial.SetColor("_NodeColor", nodeColor);
            NodeMaterial.SetColor("_HighlightColor", highlightColor);

            EdgeMaterial.SetColor("_HighlightColor", nodeColor); //think setting it to be nodecolor would be cool

            RecorderMaterial.SetColor("_Color", recorderColor);

            LoadingMaterial.SetFloat("_InnerRadius", loadingCircleInnerRadius);
            LoadingMaterial.SetFloat("_OuterRadius", loadingCircleOuterRadius);
        }

        public static readonly Material EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
        public static readonly Material NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));
        public static readonly Material RecorderMaterial = new(Shader.Find("Unlit/UIShaders/RecordingShader"));
        public static readonly Material LoadingMaterial = new(Shader.Find("Unlit/UIShaders/LoadingCircle"));

    }
}